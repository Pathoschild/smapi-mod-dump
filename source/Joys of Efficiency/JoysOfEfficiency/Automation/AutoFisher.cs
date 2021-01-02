/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/pomepome/JoysOfEfficiency
**
*************************************************/

using System.Collections.Generic;
using JoysOfEfficiency.Core;
using JoysOfEfficiency.Utils;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Tools;

namespace JoysOfEfficiency.Automation
{
    internal class AutoFisher
    {
        private static Config Config => InstanceHolder.Config;

        private static readonly Logger Logger = new Logger("AFKFisher");

        public static bool AfkMode { get; private set; }

        private static bool CatchingTreasure { get; set; }
        private static int AutoFishingCounter { get; set; }
        private static int AfkCooltimeCounter { get; set; }

        private static IReflectionHelper Reflection => InstanceHolder.Reflection;

        public static void AfkFishing()
        {
            Farmer player = Game1.player;

            if(AfkMode && player.passedOut)
            {
                AfkMode = false;
                Util.ShowHudMessageTranslated("hud.afk.passedout");
                return;
            }

            if (!AfkMode || !(player.CurrentTool is FishingRod rod) || Game1.activeClickableMenu != null)
            {
                return;
            }

            if (!rod.inUse() && !rod.castedButBobberStillInAir)
            {

                if (player.Stamina <= (player.MaxStamina * Config.ThresholdStaminaPercentage) / 100.0f)
                {
                    AfkMode = false;
                    Util.ShowHudMessageTranslated("hud.afk.tired");
                    return;
                }
                AfkCooltimeCounter++;
                if(AfkCooltimeCounter < 10)
                {
                    return;
                }
                AfkCooltimeCounter = 0;
                rod.beginUsing(player.currentLocation, 0, 0, player);
            }
            if (rod.isTimingCast)
            {
                rod.castingPower = Config.ThrowPower;
            }
            if (rod.fishCaught)
            {
                CollectFish(player, rod);
            }
        }

        public static void AutoReelRod()
        {
            Farmer player = Game1.player;
            if (!(player.CurrentTool is FishingRod rod) || Game1.activeClickableMenu != null)
            {
                return;
            }
            int whichFish = Reflection.GetField<int>(rod, "whichFish").GetValue();

            if (!rod.isNibbling || !rod.isFishing || whichFish != -1 || rod.isReeling || rod.hit ||
                rod.isTimingCast || rod.pullingOutOfWater || rod.fishCaught || rod.castedButBobberStillInAir)
            {
                return;
            }

            rod.DoFunction(player.currentLocation, 1, 1, 1, player);
        }

        public static void CollectFish(Farmer who, FishingRod rod)
        {
            IReflectedField<int> recastTimerMs = Reflection.GetField<int>(rod, "recastTimerMs");

            int whichFish = Reflection.GetField<int>(rod, "whichFish").GetValue();
            int fishQuality = Reflection.GetField<int>(rod, "fishQuality").GetValue();
            
            string itemCategory = Reflection.GetField<string>(rod, "itemCategory").GetValue();

            if (!Game1.isFestival())
            {
                who.faceDirection(2);
                who.FarmerSprite.setCurrentFrame(84);
            }

            if (Game1.random.NextDouble() < 0.025)
            {
                who.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors",
                    new Rectangle(653, 858, 1, 1), 9999f, 1, 1,
                    who.Position + new Vector2(Game1.random.Next(-3, 2) * 4, -32f), false, false,
                    (float) (who.getStandingY() / 10000.0 + 1.0 / 500.0), 0.04f, Color.LightBlue, 5f, 0.0f,
                    0.0f, 0.0f)
                {
                    acceleration = new Vector2(0.0f, 0.25f)
                });
            }

            if (!who.IsLocalPlayer)
            {
                return;
            }

            who.currentLocation.localSound("coin");
            if (!rod.treasureCaught)
            {
                recastTimerMs.SetValue(200);
                Object @object = null;
                switch (itemCategory)
                {
                    case "Object":
                    {
                        @object = new Object(whichFish, 1, false, -1, fishQuality);
                        if (whichFish == GameLocation.CAROLINES_NECKLACE_ITEM)
                        {
                            @object.questItem.Value = true;
                        }

                        if (whichFish == 79 || whichFish == 842)
                        {
                            @object = who.currentLocation.tryToCreateUnseenSecretNote(who);
                            if (@object == null)
                                return;
                        }

                        if (rod.caughtDoubleFish)
                        {
                            @object.Stack = 2;
                        }

                        break;
                    }
                    case "Furniture":
                    {
                        @object = new Furniture(whichFish, Vector2.Zero);
                        break;
                    }
                }
                bool fromFishPond = rod.fromFishPond;
                who.completelyStopAnimatingOrDoingAction();
                rod.doneFishing(who, !fromFishPond);
                if (!Game1.isFestival() && !fromFishPond && (itemCategory == "Object" && Game1.player.team.specialOrders != null))
                {
                    foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
                    {
                        specialOrder.onFishCaught?.Invoke(Game1.player, @object);
                    }
                }

                if (Game1.isFestival() || who.addItemToInventoryBool(@object))
                {
                    return;
                }

                Game1.activeClickableMenu = new ItemGrabMenu(new List<Item>
                {
                    @object
                }, rod).setEssential(true);
            }
            else
            {
                rod.fishCaught = false;
                rod.showingTreasure = true;
                who.UsingTool = true;
                int initialStack = 1;
                if (rod.caughtDoubleFish)
                {
                    initialStack = 2;
                }

                Object @object = new Object(whichFish, initialStack, false, -1, fishQuality);
                if (Game1.player.team.specialOrders != null)
                {
                    foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
                    {
                        specialOrder.onFishCaught?.Invoke(Game1.player, @object);
                    }
                }
                bool inventoryBool = who.addItemToInventoryBool(@object);
                rod.animations.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(64, 1920, 32, 32), 500f, 1, 0, who.Position + new Vector2(-32f, -160f), false, false, (float)(who.getStandingY() / 10000.0 + 1.0 / 1000.0), 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
                {
                    motion = new Vector2(0.0f, -0.128f),
                    timeBasedMotion = true,
                    endFunction = rod.openChestEndFunction,
                    extraInfoForEndBehavior = inventoryBool ? 0 : 1,
                    alpha = 0.0f,
                    alphaFade = -1f / 500f
                });
            }
        }

        public static void AutoFishing(BobberBar bar)
        {
            AutoFishingCounter = (AutoFishingCounter + 1) % 3;
            if (AutoFishingCounter > 0)
            {
                return;
            }


            IReflectedField<float> bobberSpeed = Reflection.GetField<float>(bar, "bobberBarSpeed");

            float barPos = Reflection.GetField<float>(bar, "bobberBarPos").GetValue();
            int barHeight = Reflection.GetField<int>(bar, "bobberBarHeight").GetValue();
            float fishPos = Reflection.GetField<float>(bar, "bobberPosition").GetValue();
            float treasurePos = Reflection.GetField<float>(bar, "treasurePosition").GetValue();
            float distanceFromCatching = Reflection.GetField<float>(bar, "distanceFromCatching").GetValue();
            bool treasureCaught = Reflection.GetField<bool>(bar, "treasureCaught").GetValue();
            bool treasure = Reflection.GetField<bool>(bar, "treasure").GetValue();
            float treasureAppearTimer = Reflection.GetField<float>(bar, "treasureAppearTimer").GetValue();
            float bobberBarSpeed = bobberSpeed.GetValue();

            float top = barPos;

            if (treasure && treasureAppearTimer <= 0 && !treasureCaught)
            {
                if (!CatchingTreasure && distanceFromCatching > 0.7f)
                {
                    CatchingTreasure = true;
                }
                if (CatchingTreasure && distanceFromCatching < 0.3f)
                {
                    CatchingTreasure = false;
                }
                if (CatchingTreasure)
                {
                    fishPos = treasurePos;
                }
            }

            if (fishPos > barPos + (barHeight / 2f))
            {
                return;
            }

            float strength = (fishPos - (barPos + barHeight / 2f)) / 16f;
            float distance = fishPos - top;

            float threshold = Util.Cap(InstanceHolder.Config.CpuThresholdFishing, 0, 0.5f);
            if (distance < threshold * barHeight || distance > (1 - threshold) * barHeight)
            {
                bobberBarSpeed = strength;
            }

            bobberSpeed.SetValue(bobberBarSpeed);
        }

        public static void ToggleAfkFishing()
        {
            AfkMode = !AfkMode;
            Util.ShowHudMessageTranslated(AfkMode ? "hud.afk.on" : "hud.afk.off");
            Logger.Log($"AFK Mode is {(AfkMode ? "enabled" : "disabled")}.");
        }
    }
}
