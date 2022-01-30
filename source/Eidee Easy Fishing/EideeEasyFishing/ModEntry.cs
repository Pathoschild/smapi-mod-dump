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
                text: () => Helper.Translation.Get("config.section.general.name"));

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("config.bitefaster.name"),
                tooltip: () => Helper.Translation.Get("config.bitefaster.description"),
                getValue: () => Config.BiteFaster,
                setValue: value => Config.BiteFaster = value);

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("config.hitautomatically.name"),
                tooltip: () => Helper.Translation.Get("config.hitautomatically.description"),
                getValue: () => Config.HitAutomatically,
                setValue: value => Config.HitAutomatically = value);

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("config.skipminigame.name"),
                tooltip: () => Helper.Translation.Get("config.skipminigame.description"),
                getValue: () => Config.SkipMinigame,
                setValue: value => Config.SkipMinigame = value);

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("config.fisheasycaught.name"),
                tooltip: () => Helper.Translation.Get("config.fisheasycaught.description"),
                getValue: () => Config.FishEasyCaught,
                setValue: value => Config.FishEasyCaught = value);

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("config.treasurealwaysbefound.name"),
                tooltip: () => Helper.Translation.Get("config.treasurealwaysbefound.description"),
                getValue: () => Config.TreasureAlwaysBeFound,
                setValue: value => Config.TreasureAlwaysBeFound = value);

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("config.treasureeasycaught.name"),
                tooltip: () => Helper.Translation.Get("config.treasureeasycaught.description"),
                getValue: () => Config.TreasureEasyCaught,
                setValue: value => Config.TreasureEasyCaught = value);

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("config.alwayscaughtdoublefish.name"),
                tooltip: () => Helper.Translation.Get("config.alwayscaughtdoublefish.description"),
                getValue: () => Config.AlwaysCaughtDoubleFish,
                setValue: value => Config.AlwaysCaughtDoubleFish = value);

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("config.caughtdoublefishonanybait.name"),
                tooltip: () => Helper.Translation.Get("config.caughtdoublefishonanybait.description"),
                getValue: () => Config.CaughtDoubleFishOnAnyBait,
                setValue: value => Config.CaughtDoubleFishOnAnyBait = value);

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("config.alwaysmaxcastpower.name"),
                tooltip: () => Helper.Translation.Get("config.alwaysmaxcastpower.description"),
                getValue: () => Config.AlwaysMaxCastPower,
                setValue: value => Config.AlwaysMaxCastPower = value);
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
                string msg = Helper.Translation.Get("message.config.reload");
                Game1.addHUDMessage(new HUDMessage(msg, HUDMessage.error_type) { noIcon = true, timeLeft = HUDMessage.defaultTime });
            }
        }
    }
}
