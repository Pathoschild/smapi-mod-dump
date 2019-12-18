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
                        bool caughtDouble = false;
                        if (!rod.bossFish)
                        {
                            if (Config.CaughtDoubleFishOnAnyBait || (rod.attachments[0] == null ? -1 : rod.attachments[0].parentSheetIndex) == 774)
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
                    rod.caughtDoubleFish = !rod.bossFish && (Config.CaughtDoubleFishOnAnyBait || (rod.attachments[0] == null ? -1 : rod.attachments[0].parentSheetIndex) == 774);
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
            if (args.Button == Keys.ReloadConfig)
            {
                Config = Helper.ReadConfig<ModConfig>();
                Monitor.Log("Config reloaded", LogLevel.Info);
            }
        }
    }
}
