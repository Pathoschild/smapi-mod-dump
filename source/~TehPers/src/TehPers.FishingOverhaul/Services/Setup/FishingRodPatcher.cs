/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using Ninject;
using Ninject.Activation;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;
using TehPers.Core.Api.Extensions;
using TehPers.Core.Api.Items;
using TehPers.Core.Api.Setup;
using TehPers.FishingOverhaul.Api;
using TehPers.FishingOverhaul.Api.Content;
using TehPers.FishingOverhaul.Api.Events;
using TehPers.FishingOverhaul.Api.Extensions;
using TehPers.FishingOverhaul.Config;
using TehPers.FishingOverhaul.Extensions;
using TehPers.FishingOverhaul.Extensions.Drawing;
using SObject = StardewValley.Object;

namespace TehPers.FishingOverhaul.Services.Setup
{
    [SuppressMessage(
        "ReSharper",
        "InconsistentNaming",
        Justification = "Harmony patches have a specific naming convention."
    )]
    [SuppressMessage(
        "Style",
        "IDE1006:Naming Styles",
        Justification = "Intentionally non-standard naming convention."
    )]
    internal class FishingRodPatcher : Patcher, ISetup
    {
        private static FishingRodPatcher? Instance { get; set; }

        private readonly IModHelper helper;
        private readonly IMonitor monitor;
        private readonly FishingTracker fishingTracker;
        private readonly FishingApi fishingApi;
        private readonly ICustomBobberBarFactory customBobberBarFactory;
        private readonly FishConfig fishConfig;
        private readonly INamespaceRegistry namespaceRegistry;

        private readonly IReflectedField<Multiplayer> game1MultiplayerField;

        private readonly Queue<Action> postUpdateActions;
        private bool initialized;

        private FishingRodPatcher(
            IModHelper helper,
            IMonitor monitor,
            Harmony harmony,
            FishingTracker fishingTracker,
            FishingApi fishingApi,
            ICustomBobberBarFactory customBobberBarFactory,
            FishConfig fishConfig,
            INamespaceRegistry namespaceRegistry
        )
            : base(harmony)
        {
            this.helper = helper ?? throw new ArgumentNullException(nameof(helper));
            this.monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
            this.fishingTracker =
                fishingTracker ?? throw new ArgumentNullException(nameof(fishingTracker));
            this.fishingApi = fishingApi ?? throw new ArgumentNullException(nameof(fishingApi));
            this.customBobberBarFactory = customBobberBarFactory
                ?? throw new ArgumentNullException(nameof(customBobberBarFactory));
            this.fishConfig = fishConfig ?? throw new ArgumentNullException(nameof(fishConfig));
            this.namespaceRegistry = namespaceRegistry
                ?? throw new ArgumentNullException(nameof(namespaceRegistry));

            this.game1MultiplayerField =
                helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer");

            this.postUpdateActions = new();
            this.initialized = false;
        }

        public static FishingRodPatcher Create(IContext context)
        {
            FishingRodPatcher.Instance ??= new(
                context.Kernel.Get<IModHelper>(),
                context.Kernel.Get<IMonitor>(),
                context.Kernel.Get<Harmony>(),
                context.Kernel.Get<FishingTracker>(),
                context.Kernel.Get<FishingApi>(),
                context.Kernel.Get<ICustomBobberBarFactory>(),
                context.Kernel.Get<FishConfig>(),
                context.Kernel.Get<INamespaceRegistry>()
            );
            return FishingRodPatcher.Instance;
        }

        public void Setup()
        {
            if (this.initialized)
            {
                return;
            }

            this.initialized = true;

            // Apply patches
            this.Patch(
                AccessTools.Method(typeof(FishingRod), nameof(FishingRod.tickUpdate)),
                prefix: new(
                    AccessTools.Method(
                        typeof(FishingRodPatcher),
                        nameof(FishingRodPatcher.tickUpdate_Prefix)
                    )
                ),
                postfix: new(
                    AccessTools.Method(
                        typeof(FishingRodPatcher),
                        nameof(FishingRodPatcher.tickUpdate_Postfix)
                    )
                )
            );
            this.Patch(
                AccessTools.Method(typeof(FishingRod), nameof(FishingRod.DoFunction)),
                prefix: new(
                    AccessTools.Method(
                        typeof(FishingRodPatcher),
                        nameof(FishingRodPatcher.DoFunction_Prefix)
                    )
                )
            );
            this.Patch(
                AccessTools.Method(typeof(FishingRod), nameof(FishingRod.draw)),
                prefix: new(
                    AccessTools.Method(
                        typeof(FishingRodPatcher),
                        nameof(FishingRodPatcher.draw_Prefix)
                    )
                )
            );
        }

        public override void Dispose()
        {
            if (!this.initialized)
            {
                return;
            }

            this.initialized = false;

            // Remove patches
            base.Dispose();
        }

        private void StartFishingMinigame(
            FishingInfo fishingInfo,
            Item fishItem,
            FishingRod rod,
            FishEntry fishEntry,
            bool fromFishPond
        )
        {
            // Update user
            this.fishingTracker.ActiveFisherData[fishingInfo.User] = new(
                rod,
                new FishingState.Fishing(fishingInfo, fishEntry.FishKey)
            );
            var beginReelingEvent = this.helper.Reflection
                .GetField<NetEvent0>(rod, "beginReelingEvent")
                .GetValue();
            beginReelingEvent.Fire();
            rod.isReeling = true;
            rod.hit = false;
            switch (fishingInfo.User.FacingDirection)
            {
                case 1:
                    fishingInfo.User.FarmerSprite.setCurrentSingleFrame(48);
                    break;
                case 3:
                    fishingInfo.User.FarmerSprite.setCurrentSingleFrame(48, flip: true);
                    break;
            }

            // Open fishing minigame
            var sizeDepthFactor = 1f * (fishingInfo.BobberDepth / 5f);
            var sizeLevelFactor = 1 + fishingInfo.FishingLevel / 2;
            var sizeFactor = sizeDepthFactor
                * Game1.random.Next(sizeLevelFactor, Math.Max(6, sizeLevelFactor))
                / 5f;
            if (rod.favBait)
            {
                sizeFactor *= 1.2f;
            }

            var fishSizePercent = Math.Clamp(
                sizeFactor * (1.0f + Game1.random.Next(-10, 11) / 100.0f),
                0.0f,
                1.0f
            );
            var treasure = !Game1.isFestival()
                && fishingInfo.User.fishCaught?.Count() > 1
                && Game1.random.NextDouble() < this.fishingApi.GetChanceForTreasure(fishingInfo);
            var customBobber = this.customBobberBarFactory.Create(
                fishingInfo,
                fishEntry,
                fishItem,
                fishSizePercent,
                treasure,
                rod.attachments[1]?.ParentSheetIndex ?? -1,
                fromFishPond
            );
            if (customBobber is null)
            {
                this.monitor.Log("Error creating fishing minigame GUI.", LogLevel.Error);
                Game1.showGlobalMessage(
                    $"There was an error starting the fishing minigame for {fishEntry.FishKey}. "
                    + "Check the console for more details."
                );
                fishingInfo.User.Halt();
                fishingInfo.User.completelyStopAnimatingOrDoingAction();
                fishingInfo.User.armOffset = Vector2.Zero;
                rod.castedButBobberStillInAir = false;
                rod.fishCaught = false;
                rod.isReeling = false;
                rod.isFishing = false;
                rod.pullingOutOfWater = false;
                fishingInfo.User.canReleaseTool = false;
                return;
            }

            var initialStreak = this.fishingApi.GetStreak(fishingInfo.User);

            customBobber.LostFish += (_, _) =>
            {
                // Notify user
                if (this.fishConfig.GetQualityIncrease(initialStreak) > 0)
                {
                    Game1.showGlobalMessage(
                        this.helper.Translation.Get(
                            "text.streak.lost",
                            new {streak = initialStreak}
                        )
                    );
                }

                // Update streak
                this.fishingApi.SetStreak(fishingInfo.User, 0);
            };
            customBobber.StateChanged += (_, state) =>
            {
                switch (state)
                {
                    // Lost perfect but haven't caught treasure yet
                    case (false, TreasureState.NotCaught)
                        when this.fishConfig.GetQualityIncrease(initialStreak) > 0:
                        {
                            // Notify user - streak is updated when fish is either caught or not caught
                            Game1.showGlobalMessage(
                                this.helper.Translation.Get(
                                    "text.streak.warning",
                                    new {streak = initialStreak}
                                )
                            );

                            break;
                        }
                }
            };
            customBobber.CatchFish += (_, info) =>
            {
                // Update fishing streak
                switch (info.State)
                {
                    // Perfect catch
                    case (true, _):
                        {
                            // Set new streak
                            this.fishingApi.SetStreak(fishingInfo.User, initialStreak + 1);

                            // Increase quality
                            var streakQualityIncrease =
                                this.fishConfig.GetQualityIncrease(initialStreak + 1);
                            info = info with
                            {
                                FishQuality = info.FishQuality + streakQualityIncrease + 1
                            };

                            break;
                        }
                    // Restored catch
                    case (false, TreasureState.Caught):
                        {
                            // Show restored streak message
                            if (this.fishConfig.GetQualityIncrease(initialStreak) > 0)
                            {
                                Game1.showGlobalMessage(
                                    this.helper.Translation.Get(
                                        "text.streak.restored",
                                        new {streak = initialStreak}
                                    )
                                );
                            }

                            // Increase quality
                            var streakQualityIncrease =
                                this.fishConfig.GetQualityIncrease(initialStreak);
                            info = info with
                            {
                                FishQuality = info.FishQuality + streakQualityIncrease
                            };

                            break;
                        }
                    // Not perfect
                    default:
                        {
                            // Show streak lost message
                            if (this.fishConfig.GetQualityIncrease(initialStreak) > 0)
                            {
                                Game1.showGlobalMessage(
                                    this.helper.Translation.Get(
                                        "text.streak.lost",
                                        new {streak = initialStreak}
                                    )
                                );
                            }

                            // Reset streak
                            this.fishingApi.SetStreak(fishingInfo.User, 0);

                            break;
                        }
                }

                // Catch item
                info = this.fishConfig.ClampQuality(info);
                this.CatchItem(rod, info);
            };

            Game1.activeClickableMenu = customBobber;
        }

        private void CatchItem(FishingRod rod, CatchInfo info)
        {
            var (fishingInfo, item, fromFishPond) = info;

            // Track fishing state
            var newState = new FishingState.Caught(fishingInfo, info);
            this.fishingTracker.ActiveFisherData[fishingInfo.User] = new(rod, newState);
            this.monitor.Log($"{fishingInfo.User.Name} caught {info}.");

            // Custom handling for fish catches
            if (info is CatchInfo.FishCatch (_, _, _, var fishSize, var isLegendary, var fishQuality
                , var fishDifficulty, var (isPerfect, treasureState), _, var caughtDouble))
            {
                // Update caught item
                if (item is SObject obj)
                {
                    obj.Quality = fishQuality;
                    if (caughtDouble)
                    {
                        obj.Stack = 2;
                    }
                }

                // Update fishing rod
                var wasTreasureCaught = treasureState is TreasureState.Caught;
                rod.treasureCaught = wasTreasureCaught;
                this.helper.Reflection.GetField<int>(rod, "fishSize").SetValue(fishSize);
                this.helper.Reflection.GetField<int>(rod, "fishQuality")
                    .SetValue(Math.Max(fishQuality, 0));
                this.helper.Reflection.GetField<int>(rod, "whichFish").SetValue(0);
                rod.fromFishPond = fromFishPond;
                rod.caughtDoubleFish = caughtDouble;
                this.helper.Reflection.GetField<string>(rod, "itemCategory").SetValue("Object");

                // Give the user experience
                if (!Game1.isFestival() && fishingInfo.User.IsLocalPlayer && !fromFishPond)
                {
                    rod.bossFish = isLegendary;
                    var experience = Math.Max(1, (fishQuality + 1) * 3 + fishDifficulty / 3)
                        * (wasTreasureCaught ? 2.2 : 1)
                        * (isPerfect ? 2.4 : 1)
                        * (rod.bossFish ? 5.0 : 1);
                    fishingInfo.User.gainExperience(1, (int)experience);
                }
            }
            else
            {
                // Update fishing rod
                rod.treasureCaught = false;
                this.helper.Reflection.GetField<int>(rod, "fishSize").SetValue(-1);
                this.helper.Reflection.GetField<int>(rod, "fishQuality").SetValue(-1);
                this.helper.Reflection.GetField<int>(rod, "whichFish").SetValue(0);
                rod.fromFishPond = fromFishPond;
                rod.caughtDoubleFish = false;
                this.helper.Reflection.GetField<string>(rod, "itemCategory").SetValue("Object");
            }

            // Send event
            this.fishingApi.OnCaughtItem(new(info));
            var onCatch = info switch
            {
                CatchInfo.FishCatch c => c.FishEntry.OnCatch,
                CatchInfo.TrashCatch c => c.TrashEntry.OnCatch,
                _ => throw new InvalidOperationException($"Unknown catch type {info}"),
            };
            onCatch?.OnCatch(this.fishingApi, info);

            // Get particle sprite
            var (textureName, sourceRect) = item switch
            {
                SObject {ParentSheetIndex: var index} => (@"Maps\springobjects",
                    Game1.getSourceRectForStandardTileSheet(
                        Game1.objectSpriteSheet,
                        index,
                        16,
                        16
                    )),
                _ => (@"LooseSprites\Cursors", new(228, 408, 16, 16)),
            };

            // Create animation
            float animationInterval;
            if (fishingInfo.User.FacingDirection is 1 or 3)
            {
                var distToBobber = Vector2.Distance(rod.bobber, fishingInfo.User.Position);
                const float y1 = 1f / 1000f;
                var num6 = 128.0f - (fishingInfo.User.Position.Y - rod.bobber.Y + 10.0f);
                const double a1 = 4.0 * Math.PI / 11.0;
                var f1 = (float)(distToBobber
                    * y1
                    * Math.Tan(a1)
                    / Math.Sqrt(2.0 * distToBobber * y1 * Math.Tan(a1) - 2.0 * y1 * num6));
                if (float.IsNaN(f1))
                {
                    f1 = 0.6f;
                }

                var num7 = f1 * (float)(1.0 / Math.Tan(a1));
                animationInterval = distToBobber / num7;
                rod.animations.Add(
                    new(
                        textureName,
                        sourceRect,
                        animationInterval,
                        1,
                        0,
                        rod.bobber,
                        false,
                        false,
                        rod.bobber.Y / 10000f,
                        0.0f,
                        Color.White,
                        4f,
                        0.0f,
                        0.0f,
                        0.0f
                    )
                    {
                        motion =
                            new((fishingInfo.User.FacingDirection == 3 ? -1f : 1f) * -num7, -f1),
                        acceleration = new(0.0f, y1),
                        timeBasedMotion = true,
                        endFunction = _ => this.FinishFishing(fishingInfo.User, rod, info),
                        endSound = "tinyWhip"
                    }
                );
                if (info is CatchInfo.FishCatch {CaughtDouble: true})
                {
                    var y2 = 0.0008f;
                    var f2 = (float)(distToBobber
                        * (double)y2
                        * Math.Tan(a1)
                        / Math.Sqrt(2.0 * distToBobber * y2 * Math.Tan(a1) - 2.0 * y2 * num6));
                    if (float.IsNaN(f2))
                    {
                        f2 = 0.6f;
                    }

                    var num10 = f2 * (float)(1.0 / Math.Tan(a1));
                    animationInterval = distToBobber / num10;
                    rod.animations.Add(
                        new(
                            textureName,
                            sourceRect,
                            animationInterval,
                            1,
                            0,
                            rod.bobber,
                            false,
                            false,
                            rod.bobber.Y / 10000f,
                            0.0f,
                            Color.White,
                            4f,
                            0.0f,
                            0.0f,
                            0.0f
                        )
                        {
                            motion = new(
                                (fishingInfo.User.FacingDirection == 3 ? -1f : 1f) * -num10,
                                -f2
                            ),
                            acceleration = new(0.0f, y2),
                            timeBasedMotion = true,
                            endSound = "fishSlap",
                            Parent = fishingInfo.User.currentLocation
                        }
                    );
                }
            }
            else
            {
                var num11 = rod.bobber.Y - (fishingInfo.User.getStandingY() - 64);
                var num12 = Math.Abs((float)(num11 + 256.0 + 32.0));
                if (fishingInfo.User.FacingDirection == 0)
                {
                    num12 += 96f;
                }

                const float y3 = 3f / 1000f;
                var num13 = (float)Math.Sqrt(2.0 * y3 * num12);
                animationInterval = (float)(Math.Sqrt(2.0 * (num12 - (double)num11) / y3)
                    + num13 / (double)y3);
                var x1 = 0.0f;
                if (animationInterval != 0.0)
                {
                    x1 = (fishingInfo.User.Position.X - rod.bobber.X) / animationInterval;
                }

                rod.animations.Add(
                    new(
                        textureName,
                        sourceRect,
                        animationInterval,
                        1,
                        0,
                        new(rod.bobber.X, rod.bobber.Y),
                        false,
                        false,
                        rod.bobber.Y / 10000f,
                        0.0f,
                        Color.White,
                        4f,
                        0.0f,
                        0.0f,
                        0.0f
                    )
                    {
                        motion = new(x1, -num13),
                        acceleration = new(0.0f, y3),
                        timeBasedMotion = true,
                        endFunction = _ => this.FinishFishing(fishingInfo.User, rod, info),
                        endSound = "tinyWhip"
                    }
                );
                if (info is CatchInfo.FishCatch {CaughtDouble: true})
                {
                    var num14 = rod.bobber.Y - (fishingInfo.User.getStandingY() - 64);
                    var num15 = Math.Abs((float)(num14 + 256.0 + 32.0));
                    if (fishingInfo.User.FacingDirection == 0)
                    {
                        num15 += 96f;
                    }

                    const float y4 = 0.004f;
                    var num16 = (float)Math.Sqrt(2.0 * y4 * num15);
                    animationInterval = (float)(Math.Sqrt(2.0 * (num15 - (double)num14) / y4)
                        + num16 / (double)y4);
                    var x2 = 0.0f;
                    if (animationInterval != 0.0)
                    {
                        x2 = (fishingInfo.User.Position.X - rod.bobber.X) / animationInterval;
                    }

                    rod.animations.Add(
                        new(
                            textureName,
                            sourceRect,
                            animationInterval,
                            1,
                            0,
                            new(rod.bobber.X, rod.bobber.Y),
                            false,
                            false,
                            rod.bobber.Y / 10000f,
                            0.0f,
                            Color.White,
                            4f,
                            0.0f,
                            0.0f,
                            0.0f
                        )
                        {
                            motion = new(x2, -num16),
                            acceleration = new(0.0f, y4),
                            timeBasedMotion = true,
                            endSound = "fishSlap",
                            Parent = fishingInfo.User.currentLocation
                        }
                    );
                }
            }

            if (fishingInfo.User.IsLocalPlayer)
            {
                fishingInfo.User.currentLocation.playSound("pullItemFromWater");
                fishingInfo.User.currentLocation.playSound("dwop");
            }

            rod.castedButBobberStillInAir = false;
            rod.pullingOutOfWater = true;
            rod.isFishing = false;
            rod.isReeling = false;
            fishingInfo.User.FarmerSprite.PauseForSingleAnimation = false;
            var animation = fishingInfo.User.FacingDirection switch
            {
                0 => 299,
                1 => 300,
                2 => 301,
                3 => 302,
                _ => 299,
            };
            fishingInfo.User.FarmerSprite.animateBackwardsOnce(animation, animationInterval);
        }

        private void FinishFishing(Farmer user, FishingRod rod, CatchInfo info)
        {
            // This normally happens at this point:
            user.Halt();
            user.armOffset = Vector2.Zero;
            rod.castedButBobberStillInAir = false;
            rod.isReeling = false;
            rod.isFishing = false;
            rod.pullingOutOfWater = false;
            user.canReleaseTool = false;

            // Normally fishCaught is set to true here, but to avoid a case where vanilla logic can
            // cause the user to catch weeds (the item vanilla code *thinks* was caught), it's set
            // to false here and true at the end of the tick instead. This is because the
            // animations are updated at the start of tickUpdate, then the check if the user
            // clicked happens later in that same method so there's no chance to override it
            var fishCaught = this.helper.Reflection.GetField<bool>(rod, "fishCaught");
            fishCaught.SetValue(false);
            this.postUpdateActions.Enqueue(() => fishCaught.SetValue(true));

            // Transition state
            if (this.fishingTracker.ActiveFisherData.TryGetValue(user, out var fisherData)
                && fisherData.State is FishingState.Caught(var fishingInfo, var catchInfo))
            {
                this.fishingTracker.ActiveFisherData[user] = new(
                    rod,
                    new FishingState.Holding(fishingInfo, catchInfo)
                );
            }

            if (!user.IsLocalPlayer)
            {
                return;
            }

            (int parentSheetIndex, int fishSize, bool fromFishPond, int stack)? caughtParts =
                info switch
                {
                    CatchInfo.FishCatch
                    {
                        Item: SObject
                        {
                            ParentSheetIndex: var parentSheetIndex, Stack: var stack
                        },
                        FishSize: var fishSize,
                        FromFishPond: var fromFishPond
                    } => (parentSheetIndex, fishSize, fromFishPond, stack),
                    CatchInfo.TrashCatch
                    {
                        Item: SObject
                        {
                            ParentSheetIndex: var parentSheetIndex, Stack: var stack
                        },
                        FromFishPond: var fromFishPond,
                    } => (parentSheetIndex, 0, fromFishPond, stack),
                    _ => null,
                };

            if (!Game1.isFestival())
            {
                if (caughtParts is var (parentSheetIndex, fishSize, fromFishPond, stack))
                {
                    rod.recordSize = user.caughtFish(
                        parentSheetIndex,
                        fishSize,
                        fromFishPond,
                        stack
                    );
                }

                user.faceDirection(2);
            }
            else if (user.currentLocation.currentEvent is { } currentEvent)
            {
                if (caughtParts is var (parentSheetIndex, fishSize, _, _))
                {
                    currentEvent.caughtFish(parentSheetIndex, fishSize, user);
                }

                rod.fishCaught = false;
                rod.doneFishing(user);
            }

            if (info is CatchInfo.FishCatch {IsLegendary: true})
            {
                Game1.showGlobalMessage(
                    Game1.content.LoadString(@"Strings\StringsFromCSFiles:FishingRod.cs.14068")
                );
                var multiplayer = this.game1MultiplayerField.GetValue();
                multiplayer.globalChatInfoMessage(
                    "CaughtLegendaryFish",
                    Game1.player.Name,
                    info.Item.DisplayName
                );
            }
            else if (rod.recordSize)
            {
                rod.sparklingText = new(
                    Game1.dialogueFont,
                    Game1.content.LoadString(@"Strings\StringsFromCSFiles:FishingRod.cs.14069"),
                    Color.LimeGreen,
                    Color.Azure
                );
                user.currentLocation.localSound("newRecord");
            }
            else
            {
                user.currentLocation.localSound("fishSlap");
            }
        }

        private void OpenTreasureMenuEndFunction(
            FishingInfo fishingInfo,
            FishingRod rod,
            IEnumerable<CaughtItem> treasure,
            int bobberDepth
        )
        {
            // Finish fishing
            fishingInfo.User.gainExperience(5, 10 * (bobberDepth + 1));
            fishingInfo.User.UsingTool = false;
            fishingInfo.User.completelyStopAnimatingOrDoingAction();
            rod.doneFishing(fishingInfo.User, true);

            // Invoke opened chest events (some mods may want to modify the chest contents)
            var eventArgs = new OpeningChestEventArgs(fishingInfo, treasure.ToList());
            this.fishingApi.OnOpeningChest(eventArgs);

            // Show menu
            var treasureItems =
                eventArgs.CaughtItems.Select(caughtItem => caughtItem.Item).ToList();
            if (treasureItems.Any())
            {
                var menu = new ItemGrabMenu(treasureItems, rod)
                {
                    source = 3,
                }.setEssential(true);

                Game1.activeClickableMenu = menu;
            }

            // Track fishing state
            fishingInfo.User.completelyStopAnimatingOrDoingAction();
            this.fishingTracker.ActiveFisherData[fishingInfo.User] =
                new(rod, new FishingState.NotFishing());
        }

        public static bool DoFunction_Prefix(
            GameLocation location,
            int x,
            int y,
            Farmer who,
            FishingRod __instance,
            int ___clearWaterDistance,
            ref bool ___lastCatchWasJunk
        )
        {
            // Get patcher instance
            if (FishingRodPatcher.Instance is not { } patcher)
            {
                return true;
            }

            // Get active fisher data
            if (!patcher.fishingTracker.ActiveFisherData.TryGetValue(who, out var activeFisher))
            {
                activeFisher = new(__instance, FishingState.Start());
                patcher.fishingTracker.ActiveFisherData[who] = activeFisher;
            }

            // Ensure the correct rod is being tracked
            if (activeFisher.Rod != __instance)
            {
                activeFisher = new(__instance, FishingState.Start());
                patcher.fishingTracker.ActiveFisherData[who] = activeFisher;
            }

            // Create fishing info
            var fishingInfo = patcher.fishingApi.CreateDefaultFishingInfo(who);

            // Transition
            switch (activeFisher.State)
            {
                // Start fishing
                case FishingState.NotFishing:
                    {
                        patcher.fishingTracker.ActiveFisherData[who] = new(
                            __instance,
                            new FishingState.WaitingForBite(fishingInfo)
                        );
                        return true;
                    }

                // Pull line from water
                case FishingState.WaitingForBite:
                    {
                        // Update farmer's appearance
                        who.FarmerSprite.PauseForSingleAnimation = false;
                        int? nextAnim = who.FacingDirection switch
                        {
                            0 => 299,
                            1 => 300,
                            2 => 301,
                            3 => 302,
                            _ => null,
                        };
                        if (nextAnim is { } anim)
                        {
                            who.FarmerSprite.animateBackwardsOnce(anim, 35f);
                        }

                        // Check if fish is nibbling
                        if (!__instance.isNibbling)
                        {
                            return true;
                        }

                        // Check if fishing from fish pond
                        var bobberTile = patcher.helper.Reflection
                            .GetMethod(__instance, "calculateBobberTile")
                            .Invoke<Vector2>();
                        var fromFishPond = location.isTileBuildingFishable(
                            (int)bobberTile.X,
                            (int)bobberTile.Y
                        );
                        if (((IFishingApi)patcher.fishingApi).GetFishPondFish(who, bobberTile, true)
                            is { } fishKey)
                        {
                            if (patcher.namespaceRegistry.TryGetItemFactory(
                                    fishKey,
                                    out var factory
                                ))
                            {
                                patcher.CatchItem(
                                    __instance,
                                    new CatchInfo.FishCatch(
                                        fishingInfo,
                                        new(fishKey, new(0.0)),
                                        factory.Create(),
                                        -1,
                                        false,
                                        0,
                                        0,
                                        new(false, TreasureState.None),
                                        true
                                    )
                                );
                                return false;
                            }

                            patcher.monitor.Log(
                                $"No provider for {fishKey} (from fish pond)! Defaulting to normal fishing behavior.",
                                LogLevel.Error
                            );
                        }

                        // Select an item to catch
                        var possibleCatch = patcher.fishingApi.GetPossibleCatch(fishingInfo);

                        // Catch the item
                        while (true)
                        {
                            switch (possibleCatch)
                            {
                                // Begin fishing minigame
                                case PossibleCatch.Fish(var fishEntry):
                                    {
                                        ___lastCatchWasJunk = false;
                                        if (__instance.hit || !who.IsLocalPlayer)
                                        {
                                            return false;
                                        }

                                        // Try to create the fish
                                        if (!fishEntry.TryCreateItem(
                                                fishingInfo,
                                                patcher.namespaceRegistry,
                                                out var caughtFish
                                            ))
                                        {
                                            // Select a trash item and catch that instead
                                            var trashEntry =
                                                patcher.fishingApi.GetTrashChances(fishingInfo)
                                                    .ChooseOrDefault(Game1.random)
                                                    ?.Value
                                                ?? new TrashEntry(
                                                    NamespacedKey.SdvObject(0),
                                                    new(0.0)
                                                );
                                            possibleCatch = new PossibleCatch.Trash(trashEntry);
                                            continue;
                                        }

                                        __instance.hit = true;
                                        Game1.screenOverlayTempSprites.Add(
                                            new(
                                                "LooseSprites\\Cursors",
                                                new(612, 1913, 74, 30),
                                                1500f,
                                                1,
                                                0,
                                                Game1.GlobalToLocal(
                                                    Game1.viewport,
                                                    __instance.bobber + new Vector2(-140f, -160f)
                                                ),
                                                false,
                                                false,
                                                1f,
                                                0.005f,
                                                Color.White,
                                                4f,
                                                0.075f,
                                                0.0f,
                                                0.0f,
                                                true
                                            )
                                            {
                                                scaleChangeChange = -0.005f,
                                                motion = new(0.0f, -0.1f),
                                                endFunction = _ => patcher.StartFishingMinigame(
                                                    fishingInfo,
                                                    caughtFish.Item,
                                                    __instance,
                                                    fishEntry,
                                                    fromFishPond
                                                ),
                                                id = 9.876543E+08f
                                            }
                                        );
                                        location.localSound("FishHit");
                                        return false;
                                    }

                                // Pull trash from the water
                                case PossibleCatch.Trash(var trashEntry):
                                    {
                                        ___lastCatchWasJunk = true;
                                        if (trashEntry.TryCreateItem(
                                                fishingInfo,
                                                patcher.namespaceRegistry,
                                                out var caughtTrash
                                            ))
                                        {
                                            patcher.CatchItem(
                                                __instance,
                                                new CatchInfo.TrashCatch(
                                                    fishingInfo,
                                                    trashEntry,
                                                    caughtTrash.Item,
                                                    fromFishPond
                                                )
                                            );
                                        }
                                        else
                                        {
                                            patcher.monitor.Log(
                                                $"Could not create item for {trashEntry}.",
                                                LogLevel.Error
                                            );
                                            patcher.CatchItem(
                                                __instance,
                                                new CatchInfo.TrashCatch(
                                                    fishingInfo,
                                                    trashEntry,
                                                    new SObject(0, 1),
                                                    fromFishPond
                                                )
                                            );
                                        }

                                        return false;
                                    }

                                default:
                                    throw new InvalidOperationException(
                                        $"Unknown catch type: {possibleCatch}"
                                    );
                            }
                        }
                    }

                // No actions available
                default:
                    return false;
            }
        }

        public static bool tickUpdate_Prefix(
            FishingRod __instance,
            GameTime time,
            Farmer who,
            ref int ___recastTimerMs,
            int ___clearWaterDistance
        )
        {
            if (FishingRodPatcher.Instance is not { } patcher)
            {
                return true;
            }

            // Get last user
            if (__instance.getLastFarmerToUse() is not { } user)
            {
                return true;
            }

            // Ensure the user is using this rod
            if (user.CurrentTool != __instance)
            {
                return true;
            }

            // Get/init user's fishing state
            if (!patcher.fishingTracker.ActiveFisherData.TryGetValue(user, out var fisherData))
            {
                fisherData = new(__instance, FishingState.Start());
                patcher.fishingTracker.ActiveFisherData[user] = fisherData;
            }

            // Reset state if not fishing
            if (!__instance.inUse())
            {
                patcher.fishingTracker.ActiveFisherData[user] =
                    new(__instance, FishingState.Start());
            }

            // Prevent normal execution if an overridden animation is about to finish
            // foreach (var animation in __instance.animations)
            // {
            //     if (animation is not PullFishAnimation pullAnim)
            //     {
            //         continue;
            //     }
            // 
            //     if (pullAnim.WillComplete(time))
            //     {
            //         // Call the end function early to avoid a tick where the animation ends and
            //         // vanilla logic can execute (which makes it possible to catch weeds)
            //         pullAnim.endFunction(default);
            //         pullAnim.endFunction = _ => { };
            //     }
            // }

            // Transition state
            switch (fisherData.State)
            {
                case FishingState.Caught(var fishingInfo, var catchInfo):
                    {
                        // Check if user is holding the fish now
                        if (!__instance.bobber.Value.Equals(Vector2.Zero)
                            && (__instance.isFishing
                                || __instance.pullingOutOfWater
                                || __instance.castedButBobberStillInAir)
                            && user.FarmerSprite.CurrentFrame is not 57
                            && (user.FacingDirection is not 0 || !__instance.pullingOutOfWater)
                            || !__instance.fishCaught)
                        {
                            return true;
                        }

                        // Transition state
                        patcher.fishingTracker.ActiveFisherData[user] = new(
                            __instance,
                            new FishingState.Holding(fishingInfo, catchInfo)
                        );

                        // Execute prefix again - avoids 1 tick gap where player can get weeds
                        return FishingRodPatcher.tickUpdate_Prefix(
                            __instance,
                            time,
                            who,
                            ref ___recastTimerMs,
                            ___clearWaterDistance
                        );
                    }

                case FishingState.Holding(var fishingInfo, var catchInfo):
                    {
                        // Give the user the item they caught if they are the local player
                        if (!user.IsLocalPlayer
                            || Game1.input.GetMouseState().LeftButton != ButtonState.Pressed
                            && !Game1.didPlayerJustClickAtAll()
                            && !Game1.isOneOfTheseKeysDown(
                                Game1.oldKBState,
                                Game1.options.useToolButton
                            ))
                        {
                            return true;
                        }

                        var item = catchInfo switch
                        {
                            CatchInfo.FishCatch fishCatch => fishCatch.Item,
                            CatchInfo.TrashCatch(_, var trashItem, _) => trashItem,
                            _ => throw new InvalidOperationException(
                                $"Unknown catch info type: {catchInfo}"
                            ),
                        };

                        // Create caught item
                        if (item is SObject caughtObj)
                        {
                            // Quest items
                            switch (item.ParentSheetIndex)
                            {
                                case var index when index == GameLocation.CAROLINES_NECKLACE_ITEM:
                                    {
                                        caughtObj.questItem.Value = true;
                                        break;
                                    }
                                case 79 or 842:
                                    {
                                        item = user.currentLocation.tryToCreateUnseenSecretNote(
                                            user
                                        );
                                        if (item == null)
                                        {
                                            return false;
                                        }

                                        break;
                                    }
                            }
                        }

                        // Update special orders
                        user.currentLocation.localSound("coin");
                        var fromFishPond = __instance.fromFishPond;
                        if (!Game1.isFestival()
                            && !fromFishPond
                            && Game1.player.team.specialOrders is { } specialOrders)
                        {
                            foreach (var specialOrder in specialOrders)
                            {
                                specialOrder.onFishCaught?.Invoke(Game1.player, item);
                            }
                        }

                        if (catchInfo is not CatchInfo.FishCatch
                            {
                                State: {Treasure: TreasureState.Caught}
                            } caughtFish)
                        {
                            // Add item to user's inventory, or show the menu if not enough space
                            ___recastTimerMs = 200;
                            user.completelyStopAnimatingOrDoingAction();
                            __instance.doneFishing(user, !fromFishPond);
                            if (Game1.isFestival() || user.addItemToInventoryBool(item))
                            {
                                // Transition fishing state and prevent rod from being used this frame
                                patcher.fishingTracker.ActiveFisherData[user] = new(
                                    __instance,
                                    new FishingState.NotFishing()
                                );
                                __instance.isFishing = true;
                                return false;
                            }

                            Game1.activeClickableMenu =
                                new ItemGrabMenu(new List<Item> {item}, __instance).setEssential(
                                    true
                                );
                        }
                        else
                        {
                            // Show the treasure the user caught
                            __instance.fishCaught = false;
                            __instance.showingTreasure = true;
                            user.UsingTool = true;
                            var treasure = patcher.fishingApi.GetPossibleTreasure(caughtFish)
                                .SelectMany(
                                    entry =>
                                    {
                                        entry.OnCatch?.OnCatch(patcher.fishingApi, catchInfo);
                                        return entry.TryCreateItem(
                                            fishingInfo,
                                            patcher.namespaceRegistry,
                                            out var caughtItem
                                        )
                                            ? caughtItem.Yield()
                                            : Enumerable.Empty<CaughtItem>();
                                    }
                                );
                            if (!user.addItemToInventoryBool(item))
                            {
                                // Couldn't add fish to inventory so add it to the treasure
                                treasure = treasure.Append(new(item));
                            }

                            __instance.animations.Add(
                                new(
                                    @"LooseSprites\Cursors",
                                    new(64, 1920, 32, 32),
                                    500f,
                                    1,
                                    0,
                                    user.Position + new Vector2(-32f, -160f),
                                    false,
                                    false,
                                    user.getStandingY() / 10000.0f + 1.0f / 1000.0f,
                                    0.0f,
                                    Color.White,
                                    4f,
                                    0.0f,
                                    0.0f,
                                    0.0f
                                )
                                {
                                    motion = new(0.0f, -0.128f),
                                    timeBasedMotion = true,
                                    endFunction = _ =>
                                    {
                                        user.currentLocation.localSound("openChest");
                                        __instance.sparklingText = null;
                                        __instance.animations.Add(
                                            new(
                                                @"LooseSprites\Cursors",
                                                new(64, 1920, 32, 32),
                                                200f,
                                                4,
                                                0,
                                                user.Position + new Vector2(-32f, -228f),
                                                false,
                                                false,
                                                user.getStandingY() / 10000.0f + 1.0f / 1000.0f,
                                                0.0f,
                                                Color.White,
                                                4f,
                                                0.0f,
                                                0.0f,
                                                0.0f
                                            )
                                            {
                                                endFunction = _ =>
                                                    patcher.OpenTreasureMenuEndFunction(
                                                        fishingInfo,
                                                        __instance,
                                                        treasure,
                                                        ___clearWaterDistance
                                                    )
                                            }
                                        );
                                    },
                                    alpha = 0.0f,
                                    alphaFade = -1f / 500f
                                }
                            );
                        }

                        // Transition fishing state
                        patcher.fishingTracker.ActiveFisherData[user] = new(
                            __instance,
                            new FishingState.OpeningTreasure()
                        );
                        return false;
                    }
            }

            return true;
        }

        public static void tickUpdate_Postfix()
        {
            if (FishingRodPatcher.Instance is not { } patcher)
            {
                return;
            }

            // Execute all post-update actions
            while (patcher.postUpdateActions.TryDequeue(out var action))
            {
                action();
            }
        }

        public static bool draw_Prefix(SpriteBatch b, FishingRod __instance)
        {
            if (FishingRodPatcher.Instance is not { } patcher)
            {
                return true;
            }

            // Get last user
            if (__instance.getLastFarmerToUse() is not { } user)
            {
                return true;
            }

            // Ensure the user is using this rod
            if (user.CurrentTool != __instance)
            {
                return true;
            }

            // Get user's fishing state
            if (!patcher.fishingTracker.ActiveFisherData.TryGetValue(user, out var fisherData))
            {
                return true;
            }

            // Render each fisher
            switch (fisherData.State)
            {
                case FishingState.Holding(_, var info):
                    {
                        var y = (float)(4.0
                            * Math.Round(
                                Math.Sin(
                                    Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0
                                ),
                                2
                            ));

                        // Draw bubble
                        var layerDepth = user.getStandingY() / 10000.0f + 0.06f;
                        b.Draw(
                            Game1.mouseCursors,
                            Game1.GlobalToLocal(
                                Game1.viewport,
                                user.Position + new Vector2(-120f, y - 288f)
                            ),
                            new Rectangle(31, 1870, 73, 49),
                            Color.White * 0.8f,
                            0.0f,
                            Vector2.Zero,
                            4f,
                            SpriteEffects.None,
                            layerDepth
                        );

                        // Draw item in bubble
                        info.Item.DrawInMenuCorrected(
                            b,
                            Game1.GlobalToLocal(
                                Game1.viewport,
                                user.Position + new Vector2(-124f, y - 284f) + new Vector2(44f, 68f)
                            ),
                            1f,
                            1f,
                            layerDepth + 0.0001f,
                            StackDrawType.Draw,
                            Color.White,
                            false,
                            new TopLeftDrawOrigin()
                        );

                        // Draw item in hand
                        var count = info.Item is SObject {Stack: var stack} ? stack : 1;
                        count = Math.Min(1, count);
                        foreach (var fishIndex in Enumerable.Range(0, count))
                        {
                            // TODO: some kind of jagged pattern with all the fish
                            // Maybe:
                            //  - X offset in range [-8, 8]
                            //  - Y offset in range [-8, 8]
                            var offset = new Vector2(0f, 0f);
                            info.Item.DrawInMenuCorrected(
                                b,
                                Game1.GlobalToLocal(
                                    Game1.viewport,
                                    user.Position + new Vector2(0.0f, -56f) + offset
                                ),
                                3f / 4f,
                                1f,
                                user.getStandingY() / 10000.0f + 1.0f / 500.0f + 0.06f,
                                StackDrawType.Hide,
                                Color.White,
                                false,
                                new CenterDrawOrigin()
                            );
                        }

                        // Draw item name
                        var isLegendary = info is CatchInfo.FishCatch {IsLegendary: true};
                        b.DrawString(
                            Game1.smallFont,
                            info.Item.DisplayName,
                            Game1.GlobalToLocal(
                                Game1.viewport,
                                user.Position
                                + new Vector2(
                                    (float)(26.0
                                        - Game1.smallFont.MeasureString(info.Item.DisplayName).X
                                        / 2.0),
                                    y - 278f
                                )
                            ),
                            isLegendary ? new(126, 61, 237) : Game1.textColor,
                            0.0f,
                            Vector2.Zero,
                            1f,
                            SpriteEffects.None,
                            user.getStandingY() / 10000.0f + 1.0f / 500.0f + 0.06f
                        );

                        // Draw fish specific labels
                        if (info is CatchInfo.FishCatch {FishSize: var fishSize})
                        {
                            // Draw fish length label
                            b.DrawString(
                                Game1.smallFont,
                                Game1.content.LoadString(
                                    "Strings\\StringsFromCSFiles:FishingRod.cs.14082"
                                ),
                                Game1.GlobalToLocal(
                                    Game1.viewport,
                                    user.Position + new Vector2(20f, y - 214f)
                                ),
                                Game1.textColor,
                                0.0f,
                                Vector2.Zero,
                                1f,
                                SpriteEffects.None,
                                user.getStandingY() / 10000.0f + 1.0f / 500.0f + 0.06f
                            );

                            // Draw fish length
                            b.DrawString(
                                Game1.smallFont,
                                Game1.content.LoadString(
                                    "Strings\\StringsFromCSFiles:FishingRod.cs.14083",
                                    LocalizedContentManager.CurrentLanguageCode
                                    != LocalizedContentManager.LanguageCode.en
                                        ? Math.Round(fishSize * 2.54)
                                        : fishSize
                                ),
                                Game1.GlobalToLocal(
                                    Game1.viewport,
                                    user.Position
                                    + new Vector2(
                                        (float)(85.0
                                            - Game1.smallFont.MeasureString(
                                                    Game1.content.LoadString(
                                                        "Strings\\StringsFromCSFiles:FishingRod.cs.14083",
                                                        LocalizedContentManager.CurrentLanguageCode
                                                        != LocalizedContentManager.LanguageCode.en
                                                            ? Math.Round(fishSize * 2.54)
                                                            : fishSize
                                                    )
                                                )
                                                .X
                                            / 2.0),
                                        y - 179f
                                    )
                                ),
                                __instance.recordSize
                                    ? Color.Blue * Math.Min(1f, (float)(y / 8.0 + 1.5))
                                    : Game1.textColor,
                                0.0f,
                                Vector2.Zero,
                                1f,
                                SpriteEffects.None,
                                user.getStandingY() / 10000.0f + 1.0f / 500.0f + 0.06f
                            );
                        }

                        return false;
                    }
            }

            return true;
        }
    }
}
