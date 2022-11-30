/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/eideehi/EideeEasyFishing
**
*************************************************/

using GenericModConfigMenu;
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
            I18n.Init(helper.Translation);

            Config = Helper.ReadConfig<ModConfig>();
            Keys = Config.Controls.ParseControls();

            helper.Events.Display.MenuChanged += OnMenuChanged;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu == null)
            {
                return;
            }

            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config));

            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: I18n.Config_Section_General_Name);

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: I18n.Config_BiteFaster_Name,
                tooltip: I18n.Config_BiteFaster_Description,
                getValue: () => Config.BiteFaster,
                setValue: value => Config.BiteFaster = value);

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: I18n.Config_HitAutomatically_Name,
                tooltip: I18n.Config_HitAutomatically_Description,
                getValue: () => Config.HitAutomatically,
                setValue: value => Config.HitAutomatically = value);

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: I18n.Config_TreasureAlwaysBeFound_Name,
                tooltip: I18n.Config_TreasureAlwaysBeFound_Description,
                getValue: () => Config.TreasureAlwaysBeFound,
                setValue: value => Config.TreasureAlwaysBeFound = value);

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: I18n.Config_AlwaysCaughtDoubleFish_Name,
                tooltip: I18n.Config_AlwaysCaughtDoubleFish_Description,
                getValue: () => Config.AlwaysCaughtDoubleFish,
                setValue: value => Config.AlwaysCaughtDoubleFish = value);

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: I18n.Config_CaughtDoubleFishOnAnyBait_Name,
                tooltip: I18n.Config_CaughtDoubleFishOnAnyBait_Description,
                getValue: () => Config.CaughtDoubleFishOnAnyBait,
                setValue: value => Config.CaughtDoubleFishOnAnyBait = value);

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: I18n.Config_AlwaysMaxCastPower_Name,
                tooltip: I18n.Config_AlwaysMaxCastPower_Description,
                getValue: () => Config.AlwaysMaxCastPower,
                setValue: value => Config.AlwaysMaxCastPower = value);

            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: I18n.Config_Section_Minigame_Name);

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: I18n.Config_SkipMinigame_Name,
                tooltip: I18n.Config_SkipMinigame_Description,
                getValue: () => Config.SkipMinigame,
                setValue: value => Config.SkipMinigame = value);

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: I18n.Config_FishEasyCaught_Name,
                tooltip: I18n.Config_FishEasyCaught_Description,
                getValue: () => Config.FishEasyCaught,
                setValue: value => Config.FishEasyCaught = value);

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: I18n.Config_TreasureEasyCaught_Name,
                tooltip: I18n.Config_TreasureEasyCaught_Description,
                getValue: () => Config.TreasureEasyCaught,
                setValue: value => Config.TreasureEasyCaught = value);
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
                            if (Config.CaughtDoubleFishOnAnyBait || rod.getBaitAttachmentIndex() == 774)
                            {
                                caughtDouble = Config.AlwaysCaughtDoubleFish || Game1.random.NextDouble() < (0.25 + (Game1.player.DailyLuck / 2.0));
                            }
                        }

                        if (Game1.isFestival())
                        {
                            Game1.CurrentEvent.perfectFishing();
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
                    rod.caughtDoubleFish = !rod.bossFish && (Config.CaughtDoubleFishOnAnyBait || rod.getBaitAttachmentIndex() == 774);
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
                Game1.addHUDMessage(new HUDMessage(I18n.Message_Config_Reload(), HUDMessage.error_type)
                {
                    noIcon = true,
                    timeLeft = HUDMessage.defaultTime
                });
            }
        }
    }
}
