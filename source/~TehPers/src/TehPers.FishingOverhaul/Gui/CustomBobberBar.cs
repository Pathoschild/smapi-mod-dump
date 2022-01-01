/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;
using System.Linq;
using StardewValley.Tools;
using TehPers.FishingOverhaul.Api;
using TehPers.FishingOverhaul.Api.Content;
using TehPers.FishingOverhaul.Config;
using TehPers.FishingOverhaul.Extensions;
using TehPers.FishingOverhaul.Extensions.Drawing;

namespace TehPers.FishingOverhaul.Gui
{
    internal sealed class CustomBobberBar : BobberBar
    {
        private readonly IReflectedField<bool> treasureField;
        private readonly IReflectedField<bool> treasureCaughtField;
        private readonly IReflectedField<float> treasurePositionField;
        private readonly IReflectedField<float> treasureAppearTimerField;
        private readonly IReflectedField<float> treasureScaleField;

        private readonly IReflectedField<float> distanceFromCatchingField;
        private readonly IReflectedField<float> treasureCatchLevelField;

        private readonly IReflectedField<float> bobberBarPosField;
        private readonly IReflectedField<bool> bobberInBarField;
        private readonly IReflectedField<float> difficultyField;
        private readonly IReflectedField<int> fishQualityField;
        private readonly IReflectedField<bool> perfectField;
        private readonly IReflectedField<float> scaleField;
        private readonly IReflectedField<bool> flipBubbleField;
        private readonly IReflectedField<int> bobberBarHeightField;
        private readonly IReflectedField<float> reelRotationField;
        private readonly IReflectedField<float> bobberPositionField;
        private readonly IReflectedField<bool> bossFishField;
        private readonly IReflectedField<int> motionTypeField;
        private readonly IReflectedField<int> fishSizeField;
        private readonly IReflectedField<int> minFishSizeField;
        private readonly IReflectedField<int> maxFishSizeField;
        private readonly IReflectedField<bool> fromFishPondField;

        private readonly IReflectedField<Vector2> barShakeField;
        private readonly IReflectedField<Vector2> fishShakeField;
        private readonly IReflectedField<Vector2> treasureShakeField;
        private readonly IReflectedField<Vector2> everythingShakeField;
        private readonly IReflectedField<bool> fadeOutField;

        private readonly IReflectedField<SparklingText?> sparkleTextField;

        private readonly FishEntry fishEntry;
        private readonly Item fishItem;
        private readonly FishTraits fishTraits;
        private readonly FishConfig fishConfig;
        private readonly TreasureConfig treasureConfig;
        private readonly FishingInfo fishingInfo;

        private float lastDistanceFromCatching;
        private float lastTreasureCatchLevel;
        private MinigameState state;
        private bool notifiedCatch;

        /// <summary>
        /// Invoked whenever a fish is caught.
        /// </summary>
        public event EventHandler<CatchInfo.FishCatch>? CatchFish;

        /// <summary>
        /// Invoked whenever a perfect streak is lost.
        /// </summary>
        public event EventHandler<MinigameState>? StateChanged;

        /// <summary>
        /// Invoked whenever the fish is not caught.
        /// </summary>
        public event EventHandler? LostFish;

        public CustomBobberBar(
            IModHelper helper,
            FishConfig fishConfig,
            TreasureConfig treasureConfig,
            FishingInfo fishingInfo,
            FishEntry fishEntry,
            FishTraits fishTraits,
            Item fishItem,
            float fishSizePercent,
            bool treasure,
            int bobber,
            bool fromFishPond
        )
            : base(0, fishSizePercent, treasure, bobber)
        {
            _ = helper ?? throw new ArgumentNullException(nameof(helper));
            this.fishConfig = fishConfig ?? throw new ArgumentNullException(nameof(fishConfig));
            this.treasureConfig =
                treasureConfig ?? throw new ArgumentNullException(nameof(treasureConfig));
            this.fishingInfo = fishingInfo ?? throw new ArgumentNullException(nameof(fishingInfo));
            this.fishEntry = fishEntry;
            this.fishTraits = fishTraits ?? throw new ArgumentNullException(nameof(fishTraits));
            this.fishItem = fishItem ?? throw new ArgumentNullException(nameof(fishItem));

            this.treasureField = helper.Reflection.GetField<bool>(this, "treasure");
            this.treasureCaughtField = helper.Reflection.GetField<bool>(this, "treasureCaught");
            this.treasurePositionField =
                helper.Reflection.GetField<float>(this, "treasurePosition");
            this.treasureAppearTimerField =
                helper.Reflection.GetField<float>(this, "treasureAppearTimer");
            this.treasureScaleField = helper.Reflection.GetField<float>(this, "treasureScale");

            this.distanceFromCatchingField =
                helper.Reflection.GetField<float>(this, "distanceFromCatching");
            this.treasureCatchLevelField =
                helper.Reflection.GetField<float>(this, "treasureCatchLevel");

            this.bobberBarPosField = helper.Reflection.GetField<float>(this, "bobberBarPos");
            this.bobberInBarField = helper.Reflection.GetField<bool>(this, "bobberInBar");
            this.difficultyField = helper.Reflection.GetField<float>(this, "difficulty");
            this.fishQualityField = helper.Reflection.GetField<int>(this, "fishQuality");
            this.perfectField = helper.Reflection.GetField<bool>(this, "perfect");
            this.scaleField = helper.Reflection.GetField<float>(this, "scale");
            this.flipBubbleField = helper.Reflection.GetField<bool>(this, "flipBubble");
            this.bobberBarHeightField = helper.Reflection.GetField<int>(this, "bobberBarHeight");
            this.reelRotationField = helper.Reflection.GetField<float>(this, "reelRotation");
            this.bobberPositionField = helper.Reflection.GetField<float>(this, "bobberPosition");
            this.bossFishField = helper.Reflection.GetField<bool>(this, "bossFish");
            this.motionTypeField = helper.Reflection.GetField<int>(this, "motionType");
            this.fishSizeField = helper.Reflection.GetField<int>(this, "fishSize");
            this.minFishSizeField = helper.Reflection.GetField<int>(this, "minFishSize");
            this.maxFishSizeField = helper.Reflection.GetField<int>(this, "maxFishSize");
            this.fromFishPondField = helper.Reflection.GetField<bool>(this, "fromFishPond");

            this.barShakeField = helper.Reflection.GetField<Vector2>(this, "barShake");
            this.fishShakeField = helper.Reflection.GetField<Vector2>(this, "fishShake");
            this.treasureShakeField = helper.Reflection.GetField<Vector2>(this, "treasureShake");
            this.everythingShakeField =
                helper.Reflection.GetField<Vector2>(this, "everythingShake");
            this.fadeOutField = helper.Reflection.GetField<bool>(this, "fadeOut");

            this.sparkleTextField = helper.Reflection.GetField<SparklingText?>(this, "sparkleText");

            // Track state
            this.lastDistanceFromCatching = 0f;
            this.lastTreasureCatchLevel = 0f;
            this.state = new(true, treasure ? TreasureState.NotCaught : TreasureState.None);

            // Track player streak
            this.perfectField.SetValue(true);
            var fishSizeReductionTimerField =
                helper.Reflection.GetField<int>(this, "fishSizeReductionTimer");
            fishSizeReductionTimerField.SetValue(800);

            // Fish size
            var minFishSize = fishTraits.MinSize;
            var maxFishSize = fishTraits.MaxSize;
            var fishSize = (int)(minFishSize + (maxFishSize - minFishSize) * fishSizePercent) + 1;
            this.minFishSizeField.SetValue(minFishSize);
            this.maxFishSizeField.SetValue(maxFishSize);
            this.fishSizeField.SetValue(fishSize);

            // Track other information (not all tracked by vanilla)
            this.fromFishPondField.SetValue(fromFishPond);
            this.bossFishField.SetValue(fishTraits.IsLegendary);

            // Adjust quality to be increased by streak
            var fishQuality = fishSizePercent switch
            {
                < 0.33f => 0,
                < 0.66f => 1,
                _ => 2,
            };

            // Quality bobber
            if (bobber is 877)
            {
                fishQuality += 1;
                if (fishQuality > 2)
                {
                    fishQuality += 1;
                }
            }

            // Beginner rod
            if (fishingInfo.User.CurrentTool is FishingRod { UpgradeLevel: 1 })
            {
                fishQuality = 0;
            }

            // Don't bump quality from 3 -> 4 here, that will be done later
            this.fishQualityField.SetValue(fishQuality);

            // Adjust fish difficulty
            this.difficultyField.SetValue(fishTraits.DartFrequency);
            this.motionTypeField.SetValue(
                fishTraits.DartBehavior switch
                {
                    DartBehavior.Mixed => BobberBar.mixed,
                    DartBehavior.Dart => BobberBar.dart,
                    DartBehavior.Smooth => BobberBar.smooth,
                    DartBehavior.Sink => BobberBar.sink,
                    DartBehavior.Floater => BobberBar.floater,
                    _ => throw new ArgumentOutOfRangeException(
                        nameof(fishTraits),
                        "Invalid dart behavior."
                    )
                }
            );
        }

        public override void update(GameTime time)
        {
            // Speed warp on catching fish
            var distanceFromCatching = this.distanceFromCatchingField.GetValue();
            var delta = distanceFromCatching - this.lastDistanceFromCatching;
            var mult = delta switch
            {
                > 0f => this.fishConfig.CatchSpeed,
                < 0f => this.fishConfig.DrainSpeed,
                _ => 0f,
            };
            distanceFromCatching = this.lastDistanceFromCatching + delta * mult;
            this.lastDistanceFromCatching = distanceFromCatching;
            this.distanceFromCatchingField.SetValue(distanceFromCatching);

            // Speed warp on catching treasure
            var treasureCatchLevel = this.treasureCatchLevelField.GetValue();
            delta = treasureCatchLevel - this.lastTreasureCatchLevel;
            mult = delta switch
            {
                > 0f => this.treasureConfig.CatchSpeed,
                < 0f => this.treasureConfig.DrainSpeed,
                _ => 0f,
            };
            treasureCatchLevel = this.lastTreasureCatchLevel + delta * mult;
            this.lastTreasureCatchLevel = treasureCatchLevel;
            this.treasureCatchLevelField.SetValue(treasureCatchLevel);

            var perfect = this.perfectField.GetValue();
            var treasure = this.treasureField.GetValue();
            var treasureCaught = this.treasureCaughtField.GetValue();

            // Update state
            var newState = new MinigameState(
                perfect,
                (treasure, treasureCaught) switch
                {
                    (false, _) => TreasureState.None,
                    (_, false) => TreasureState.NotCaught,
                    (_, true) => TreasureState.Caught,
                }
            );
            if (this.state != newState)
            {
                this.OnStateChanged(newState);
                this.state = newState;
            }

            // Override post-catch logic
            if (this.fadeOutField.GetValue())
            {
                var scale = this.scaleField.GetValue();
                if (scale <= 0.05f)
                {
                    // Check for wild bait
                    var caughtDouble = !this.bossFishField.GetValue()
                        && Game1.player.CurrentTool is FishingRod { attachments: { } attachments }
                        && attachments[0]?.ParentSheetIndex is 774
                        && Game1.random.NextDouble() < 0.25 + Game1.player.DailyLuck / 2.0;
                    if (distanceFromCatching > 0.9 && Game1.player.CurrentTool is FishingRod)
                    {
                        // Notify that a fish was caught
                        var catchInfo = new CatchInfo.FishCatch(
                            this.fishingInfo,
                            this.fishEntry,
                            this.fishItem,
                            this.fishSizeField.GetValue(),
                            this.fishTraits.IsLegendary,
                            this.fishQualityField.GetValue(),
                            (int)this.difficultyField.GetValue(),
                            this.state,
                            this.fromFishPondField.GetValue(),
                            caughtDouble
                        );
                        this.OnCaughtFish(catchInfo);
                    }
                    else
                    {
                        Game1.player.completelyStopAnimatingOrDoingAction();
                        if (Game1.player.CurrentTool is FishingRod rod)
                        {
                            rod.doneFishing(Game1.player, true);
                        }

                        this.OnLostFish();
                    }

                    Game1.exitActiveMenu();
                    Game1.setRichPresence("location", Game1.currentLocation.Name);
                    return;
                }
            }

            // Base call
            base.update(time);
        }

        public override void emergencyShutDown()
        {
            if (this.distanceFromCatchingField.GetValue() <= 0.9)
            {
                // Failed to catch fish
                this.OnLostFish();
            }

            base.emergencyShutDown();
        }

        public override void draw(SpriteBatch b)
        {
            Game1.StartWorldDrawInUI(b);

            var everythingShake = this.everythingShakeField.GetValue();
            var flipBubble = this.flipBubbleField.GetValue();
            var scale = this.scaleField.GetValue();
            b.Draw(
                Game1.mouseCursors,
                new Vector2(
                    this.xPositionOnScreen - (flipBubble ? 44 : 20) + 104,
                    this.yPositionOnScreen - 16 + 314
                )
                + everythingShake,
                new Rectangle(652, 1685, 52, 157),
                Color.White * 0.6f * scale,
                0.0f,
                new Vector2(26f, 78.5f) * scale,
                4f * scale,
                flipBubble ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                1f / 1000f
            );
            b.Draw(
                Game1.mouseCursors,
                new Vector2(this.xPositionOnScreen + 70, this.yPositionOnScreen + 296)
                + everythingShake,
                new Rectangle(644, 1999, 37, 150),
                Color.White * scale,
                0.0f,
                new Vector2(18.5f, 74f) * scale,
                4f * scale,
                SpriteEffects.None,
                0.01f
            );

            if (Math.Abs(scale - 1.0) < 0.001)
            {
                var bobberBarPos = this.bobberBarPosField.GetValue();
                var barShake = this.barShakeField.GetValue();
                var bobberInBar = this.bobberInBarField.GetValue();
                var bobberBarHeight = this.bobberBarHeightField.GetValue();
                var distanceFromCatching = this.distanceFromCatchingField.GetValue();
                var reelRotation = this.reelRotationField.GetValue();
                var color = bobberInBar
                    ? Color.White
                    : Color.White
                    * 0.25f
                    * ((float)Math.Round(
                            Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 100.0),
                            2
                        )
                        + 2f);

                // Draw background and components
                b.Draw(
                    Game1.mouseCursors,
                    new Vector2(
                        this.xPositionOnScreen + 64,
                        this.yPositionOnScreen + 12 + (int)bobberBarPos
                    )
                    + barShake
                    + everythingShake,
                    new Rectangle(682, 2078, 9, 2),
                    color,
                    0.0f,
                    Vector2.Zero,
                    4f,
                    SpriteEffects.None,
                    0.89f
                );
                b.Draw(
                    Game1.mouseCursors,
                    new Vector2(
                        this.xPositionOnScreen + 64,
                        this.yPositionOnScreen + 12 + (int)bobberBarPos + 8
                    )
                    + barShake
                    + everythingShake,
                    new Rectangle(682, 2081, 9, 1),
                    color,
                    0.0f,
                    Vector2.Zero,
                    new Vector2(4f, bobberBarHeight - 16),
                    SpriteEffects.None,
                    0.89f
                );
                b.Draw(
                    Game1.mouseCursors,
                    new Vector2(
                        this.xPositionOnScreen + 64,
                        this.yPositionOnScreen + 12 + (int)bobberBarPos + bobberBarHeight - 8
                    )
                    + barShake
                    + everythingShake,
                    new Rectangle(682, 2085, 9, 2),
                    color,
                    0.0f,
                    Vector2.Zero,
                    4f,
                    SpriteEffects.None,
                    0.89f
                );
                b.Draw(
                    Game1.staminaRect,
                    new Rectangle(
                        this.xPositionOnScreen + 124,
                        this.yPositionOnScreen + 4 + (int)(580.0 * (1.0 - distanceFromCatching)),
                        16,
                        (int)(580.0 * distanceFromCatching)
                    ),
                    Utility.getRedToGreenLerpColor(distanceFromCatching)
                );
                b.Draw(
                    Game1.mouseCursors,
                    new Vector2(this.xPositionOnScreen + 18, this.yPositionOnScreen + 514)
                    + everythingShake,
                    new Rectangle(257, 1990, 5, 10),
                    Color.White,
                    reelRotation,
                    new(2f, 10f),
                    4f,
                    SpriteEffects.None,
                    0.9f
                );

                // Draw treasure
                var treasurePosition = this.treasurePositionField.GetValue();
                var treasureShake = this.treasureShakeField.GetValue();
                var treasureScale = this.treasureScaleField.GetValue();
                b.Draw(
                    Game1.mouseCursors,
                    new Vector2(
                        this.xPositionOnScreen + 64 + 18,
                        this.yPositionOnScreen + 12 + 24 + treasurePosition
                    )
                    + treasureShake
                    + everythingShake,
                    new Rectangle(638, 1865, 20, 24),
                    Color.White,
                    0.0f,
                    new(10f, 10f),
                    2f * treasureScale,
                    SpriteEffects.None,
                    0.85f
                );
                var treasureCatchLevel = this.treasureCatchLevelField.GetValue();
                var treasureCaught = this.treasureCaughtField.GetValue();
                if (treasureCatchLevel > 0.0 && !treasureCaught)
                {
                    b.Draw(
                        Game1.staminaRect,
                        new Rectangle(
                            this.xPositionOnScreen + 64,
                            this.yPositionOnScreen + 12 + (int)treasurePosition,
                            40,
                            8
                        ),
                        Color.DimGray * 0.5f
                    );
                    b.Draw(
                        Game1.staminaRect,
                        new Rectangle(
                            this.xPositionOnScreen + 64,
                            this.yPositionOnScreen + 12 + (int)treasurePosition,
                            (int)(treasureCatchLevel * 40.0),
                            8
                        ),
                        Color.Orange
                    );
                }

                // Draw fish
                var bobberPosition = this.bobberPositionField.GetValue();
                var fishShake = this.fishShakeField.GetValue();
                var position = new Vector2(
                    this.xPositionOnScreen + 64 + 18,
                    this.yPositionOnScreen + bobberPosition + 12 + 24
                );
                if (this.fishConfig.ShowFishInMinigame)
                {
                    this.fishItem.DrawInMenuCorrected(
                        b,
                        position + fishShake + everythingShake,
                        0.5f,
                        1f,
                        0.88f,
                        StackDrawType.Hide,
                        Color.White,
                        false,
                        new CenterDrawOrigin()
                    );
                }
                else
                {
                    b.Draw(
                        Game1.mouseCursors,
                        position + fishShake + everythingShake,
                        new Rectangle(614 + (this.fishTraits.IsLegendary ? 20 : 0), 1840, 20, 20),
                        Color.White,
                        0.0f,
                        new(10f, 10f),
                        2f,
                        SpriteEffects.None,
                        0.88f
                    );
                }

                // Draw sparkle text
                var sparkleText = this.sparkleTextField.GetValue();
                sparkleText?.draw(b, new(this.xPositionOnScreen - 16, this.yPositionOnScreen - 64));
            }

            if (Game1.player.fishCaught?.Any() == false)
            {
                var position = new Vector2(
                    this.xPositionOnScreen + (flipBubble ? this.width + 64 + 8 : -200),
                    this.yPositionOnScreen + 192
                );
                if (!Game1.options.gamepadControls)
                {
                    b.Draw(
                        Game1.mouseCursors,
                        position,
                        new Rectangle(644, 1330, 48, 69),
                        Color.White,
                        0.0f,
                        Vector2.Zero,
                        4f,
                        SpriteEffects.None,
                        0.88f
                    );
                }
                else
                {
                    b.Draw(
                        Game1.controllerMaps,
                        position,
                        Utility.controllerMapSourceRect(new(681, 0, 96, 138)),
                        Color.White,
                        0.0f,
                        Vector2.Zero,
                        2f,
                        SpriteEffects.None,
                        0.88f
                    );
                }
            }

            Game1.EndWorldDrawInUI(b);
        }

        private void OnCaughtFish(CatchInfo.FishCatch e)
        {
            if (this.notifiedCatch)
            {
                return;
            }

            this.notifiedCatch = true;
            this.CatchFish?.Invoke(this, e);
        }

        private void OnLostFish()
        {
            if (this.notifiedCatch)
            {
                return;
            }

            this.notifiedCatch = true;
            this.LostFish?.Invoke(this, EventArgs.Empty);
        }

        private void OnStateChanged(MinigameState e)
        {
            this.StateChanged?.Invoke(this, e);
        }
    }
}