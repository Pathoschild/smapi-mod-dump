/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/eideehi/EideeEasyFishing
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;

namespace EideeEasyFishing
{
    public class ModEntry : Mod
    {
        private ModConfig Config;
        private ModConfigKeys Keys;

        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();
            Keys = Config.Controls.ParseControls();

            helper.Events.Display.MenuChanged += OnMenuChanged;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
        }

        private void OnMenuChanged(object sender, MenuChangedEventArgs args)
        {
            Farmer player = Game1.player;
            if (player == null || !player.IsLocalPlayer)
            {
                return;
            }

            if (args.NewMenu is BobberBar bar)
            {
                if (Config.TreasureAlwaysBeFound)
                {
                    Helper.Reflection.GetField<bool>(bar, "treasure").SetValue(true);
                }

                if (Config.SkipMinigame)
                {
                    if (player.CurrentTool is FishingRod rod)
                    {
                        int whichFish = Helper.Reflection.GetField<int>(bar, "whichFish").GetValue();
                        int fishSize = Helper.Reflection.GetField<int>(bar, "fishSize").GetValue();
                        int fishQuality = Helper.Reflection.GetField<int>(bar, "fishQuality").GetValue();
                        float difficulty = Helper.Reflection.GetField<float>(bar, "difficulty").GetValue();
                        bool treasure = Helper.Reflection.GetField<bool>(bar, "treasure").GetValue();
                        bool fromFishPond = Helper.Reflection.GetField<bool>(bar, "fromFishPond").GetValue();
                        bool bossFish = Helper.Reflection.GetField<bool>(bar, "bossFish").GetValue();
                        bool caughtDouble = false;

                        if (!bossFish)
                        {
                            if (Config.CaughtDoubleFishOnAnyBait || (rod.attachments[0] == null ? -1 : rod.attachments[0].ParentSheetIndex) == 774)
                            {
                                caughtDouble = Config.AlwaysCaughtDoubleFish || Game1.random.NextDouble() < (0.25 + (Game1.player.DailyLuck / 2.0));
                            }
                        }

                        rod.caughtDoubleFish = caughtDouble;
                        rod.pullFishFromWater(whichFish, fishSize, fishQuality, (int)difficulty, treasure, true, fromFishPond, caughtDouble);

                        Game1.exitActiveMenu();
                        Game1.setRichPresence("location", (object)Game1.currentLocation.Name);
                    }
                }
            }
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs args)
        {
            Farmer player = Game1.player;
            if (player == null || !player.IsLocalPlayer)
            {
                return;
            }

            if (player.CurrentTool is FishingRod rod)
            {
                if (Config.AlwaysMaxCastPower && rod.isTimingCast)
                {
                    rod.castingTimerSpeed = 0;
                    rod.castingPower = 1;
                }

                if (Config.BiteFaster && !rod.isNibbling && rod.isFishing && !rod.isReeling && !rod.pullingOutOfWater && !rod.hit)
                {
                    rod.timeUntilFishingBite = 0;
                }

                if (Config.HitAutomatically && rod.isNibbling && rod.isFishing && !rod.isReeling && !rod.pullingOutOfWater && !rod.hit)
                {
                    Farmer.useTool(player);
                }

                if (!Config.SkipMinigame && Config.AlwaysCaughtDoubleFish)
                {
                    rod.caughtDoubleFish = !rod.bossFish && (Config.CaughtDoubleFishOnAnyBait || (rod.attachments[0] == null ? -1 : rod.attachments[0].ParentSheetIndex) == 774);
                }
            }

            if (Game1.activeClickableMenu is BobberBar bar)
            {
                float bobberBarPos = Helper.Reflection.GetField<float>(bar, "bobberBarPos").GetValue();
                int bobberBarHeight = Helper.Reflection.GetField<int>(bar, "bobberBarHeight").GetValue();

                if (Config.FishEasyCaught)
                {
                    Helper.Reflection.GetField<float>(bar, "bobberPosition").SetValue(bobberBarPos + (bobberBarHeight / 2) - 25);
                }

                if (Config.TreasureEasyCaught)
                {
                    Helper.Reflection.GetField<float>(bar, "treasurePosition").SetValue(bobberBarPos + (bobberBarHeight / 2) - 25);
                }
            }
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs args)
        {
            if (!Context.IsWorldReady)
            {
                return;
            }

            if (args.Button == Keys.ReloadConfig)
            {
                Config = Helper.ReadConfig<ModConfig>();
                string msg = Helper.Translation.Get("config.reload");
                Game1.addHUDMessage(new HUDMessage(msg, HUDMessage.error_type) { noIcon = true, timeLeft = HUDMessage.defaultTime });
            }
        }
    }
}
