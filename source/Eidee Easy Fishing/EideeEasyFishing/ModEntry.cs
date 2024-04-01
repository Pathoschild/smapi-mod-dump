/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/eideehi/EideeEasyFishing
**
*************************************************/

using System;
using GenericModConfigMenu;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;

namespace EideeEasyFishing
{
    internal class ModEntry : Mod
    {
        private ModConfig _config;
        private ModConfigKeys _keys;

        private int _delayTick;
        private float _prevBobberPosition;
        private float _prevDistanceFromCatching;
        private float _prevTreasureCatchLevel;

        public override void Entry(IModHelper helper)
        {
            I18n.Init(helper.Translation);

            _config = Helper.ReadConfig<ModConfig>();
            _keys = _config.Controls.ParseControls();

            helper.Events.Display.MenuChanged += OnMenuChanged;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu == null) return;

            configMenu.Register(
                mod: ModManifest,
                reset: () => _config = new ModConfig(),
                save: () => Helper.WriteConfig(_config));

            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: I18n.Config_Section_General_Name);

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: I18n.Config_BiteFaster_Name,
                tooltip: I18n.Config_BiteFaster_Description,
                getValue: () => _config.BiteFaster,
                setValue: value => _config.BiteFaster = value);

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: I18n.Config_HitAutomatically_Name,
                tooltip: I18n.Config_HitAutomatically_Description,
                getValue: () => _config.HitAutomatically,
                setValue: value => _config.HitAutomatically = value);

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: I18n.Config_TreasureAlwaysBeFound_Name,
                tooltip: I18n.Config_TreasureAlwaysBeFound_Description,
                getValue: () => _config.TreasureAlwaysBeFound,
                setValue: value => _config.TreasureAlwaysBeFound = value);

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: I18n.Config_AlwaysCaughtDoubleFish_Name,
                tooltip: I18n.Config_AlwaysCaughtDoubleFish_Description,
                getValue: () => _config.AlwaysCaughtDoubleFish,
                setValue: value => _config.AlwaysCaughtDoubleFish = value);

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: I18n.Config_CaughtDoubleFishOnAnyBait_Name,
                tooltip: I18n.Config_CaughtDoubleFishOnAnyBait_Description,
                getValue: () => _config.CaughtDoubleFishOnAnyBait,
                setValue: value => _config.CaughtDoubleFishOnAnyBait = value);

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: I18n.Config_AlwaysMaxCastPower_Name,
                tooltip: I18n.Config_AlwaysMaxCastPower_Description,
                getValue: () => _config.AlwaysMaxCastPower,
                setValue: value => _config.AlwaysMaxCastPower = value);

            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: I18n.Config_Section_Minigame_Name);

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: I18n.Config_SkipMinigame_Name,
                tooltip: I18n.Config_SkipMinigame_Description,
                getValue: () => _config.SkipMinigame,
                setValue: value => _config.SkipMinigame = value);

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: I18n.Config_FishEasyCaught_Name,
                tooltip: I18n.Config_FishEasyCaught_Description,
                getValue: () => _config.FishEasyCaught,
                setValue: value => _config.FishEasyCaught = value);

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: I18n.Config_TreasureEasyCaught_Name,
                tooltip: I18n.Config_TreasureEasyCaught_Description,
                getValue: () => _config.TreasureEasyCaught,
                setValue: value => _config.TreasureEasyCaught = value);

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: I18n.Config_FishMovementSpeedMultiplier_Name,
                tooltip: I18n.Config_FishMovementSpeedMultiplier_Description,
                getValue: () => _config.FishMovementSpeedMultiplier,
                setValue: value => _config.FishMovementSpeedMultiplier = value);

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: I18n.Config_ProgressBarDecreaseMultiplier_Name,
                tooltip: I18n.Config_ProgressBarDecreaseMultiplier_Description,
                getValue: () => _config.ProgressBarDecreaseMultiplier,
                setValue: value => _config.ProgressBarDecreaseMultiplier = value);

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: I18n.Config_ProgressBarIncreaseMultiplier_Name,
                tooltip: I18n.Config_ProgressBarIncreaseMultiplier_Description,
                getValue: () => _config.ProgressBarIncreaseMultiplier,
                setValue: value => _config.ProgressBarIncreaseMultiplier = value);

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: I18n.Config_TreasureCatchSpeedMultiplier_Name,
                tooltip: I18n.Config_TreasureCatchSpeedMultiplier_Description,
                getValue: () => _config.TreasureCatchSpeedMultiplier,
                setValue: value => _config.TreasureCatchSpeedMultiplier = value);
        }

        private void OnMenuChanged(object sender, MenuChangedEventArgs args)
        {
            var player = Game1.player;
            if (player is not { IsLocalPlayer: true }) return;
            if (player.CurrentTool is not FishingRod rod) return;
            if (args.NewMenu is not BobberBar bar) return;

            if (_config.TreasureAlwaysBeFound)
            {
                bar.treasure = true;
            }

            if (!_config.SkipMinigame)
            {
                _delayTick = 8;
                _prevBobberPosition = 0f;
                _prevDistanceFromCatching = 0f;
                _prevTreasureCatchLevel = 0f;

                if (bar.distanceFromCatching != 0.1f)
                {
                    bar.distanceFromCatching = 0.3f;
                }
            }
            else
            {
                var numCaught = 1;

                if (!bar.bossFish)
                {
                    if (_config.CaughtDoubleFishOnAnyBait || rod?.GetBait()?.QualifiedItemId == "(O)774")
                    {
                        if (_config.AlwaysCaughtDoubleFish ||
                            Game1.random.NextDouble() < (0.25 + (Game1.player.DailyLuck / 2.0)))
                        {
                            numCaught = 2;
                        }
                    }

                    if (bar.challengeBaitFishes > 0)
                    {
                        numCaught = bar.challengeBaitFishes;
                    }
                }

                if (Game1.isFestival())
                {
                    Game1.CurrentEvent.perfectFishing();
                }

                rod.pullFishFromWater(bar.whichFish, bar.fishSize, bar.fishQuality, (int)bar.difficulty, bar.treasure,
                    true, bar.fromFishPond, bar.setFlagOnCatch, bar.bossFish, numCaught);

                Game1.exitActiveMenu();
                Game1.setRichPresence("location", Game1.currentLocation.Name);
            }
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs args)
        {
            var player = Game1.player;
            if (player is not { IsLocalPlayer: true }) return;

            if (player.CurrentTool is FishingRod rod)
            {
                if (_config.AlwaysMaxCastPower && rod.isTimingCast)
                {
                    rod.castingTimerSpeed = 0;
                    rod.castingPower = 1;
                }

                if (_config.BiteFaster && !rod.isNibbling && rod.isFishing && !rod.isReeling &&
                    !rod.pullingOutOfWater && !rod.hit)
                {
                    rod.timeUntilFishingBite = 0;
                }

                if (_config.HitAutomatically && rod.isNibbling && rod.isFishing && !rod.isReeling &&
                    !rod.pullingOutOfWater && !rod.hit)
                {
                    Farmer.useTool(player);
                }

                if (!_config.SkipMinigame && _config.AlwaysCaughtDoubleFish)
                {
                    rod.numberOfFishCaught =
                        (!rod.bossFish && (_config.CaughtDoubleFishOnAnyBait ||
                                           rod?.GetBait()?.QualifiedItemId == "(O)774"))
                            ? 2
                            : 1;
                }
            }

            if (Game1.activeClickableMenu is BobberBar bar && !_config.SkipMinigame)
            {
                if (_delayTick > 0)
                {
                    _delayTick--;
                }
                else
                {
                    if (_config.FishEasyCaught)
                    {
                        bar.bobberPosition = bar.bobberBarPos + (bar.bobberBarHeight / 2f) - 25;
                    }

                    if (_config.TreasureEasyCaught)
                    {
                        bar.treasurePosition = bar.bobberBarPos + (bar.bobberBarHeight / 2f) - 25;
                    }

                    if (_prevBobberPosition != 0 && bar.bobberPosition != 0 &&
                        _prevBobberPosition != bar.bobberPosition)
                    {
                        if (_prevBobberPosition > bar.bobberPosition)
                        {
                            bar.bobberPosition = _prevBobberPosition - ((_prevBobberPosition - bar.bobberPosition) *
                                                                        _config.FishMovementSpeedMultiplier);
                        }
                        else
                        {
                            bar.bobberPosition = _prevBobberPosition + ((bar.bobberPosition - _prevBobberPosition) *
                                                                        _config.FishMovementSpeedMultiplier);
                        }
                    }

                    if (_prevDistanceFromCatching != 0 && bar.distanceFromCatching != 0 &&
                        _prevDistanceFromCatching != bar.distanceFromCatching)
                    {
                        if (_prevDistanceFromCatching > bar.distanceFromCatching)
                        {
                            bar.distanceFromCatching = _prevDistanceFromCatching -
                                                       ((_prevDistanceFromCatching - bar.distanceFromCatching) *
                                                        _config.ProgressBarDecreaseMultiplier);
                        }
                        else
                        {
                            bar.distanceFromCatching = _prevDistanceFromCatching +
                                                       ((bar.distanceFromCatching - _prevDistanceFromCatching) *
                                                        _config.ProgressBarIncreaseMultiplier);
                        }

                        bar.distanceFromCatching = Math.Max(0f, Math.Min(1f, bar.distanceFromCatching));
                    }

                    if (_prevTreasureCatchLevel != 0 && bar.treasureCatchLevel != 0 &&
                        _prevTreasureCatchLevel != bar.treasureCatchLevel)
                    {
                        bar.treasureCatchLevel = _prevTreasureCatchLevel +
                                                 ((bar.treasureCatchLevel - _prevTreasureCatchLevel) *
                                                  _config.TreasureCatchSpeedMultiplier);
                    }
                }

                _prevBobberPosition = bar.bobberPosition;
                _prevDistanceFromCatching = bar.distanceFromCatching;
                _prevTreasureCatchLevel = bar.treasureCatchLevel;
            }
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs args)
        {
            if (!Context.IsWorldReady) return;

            if (args.Button == _keys.ReloadConfig)
            {
                _config = Helper.ReadConfig<ModConfig>();
                _keys = _config.Controls.ParseControls();
                Game1.addHUDMessage(new HUDMessage(I18n.Message_Config_Reload(), HUDMessage.error_type)
                {
                    noIcon = true,
                    timeLeft = HUDMessage.defaultTime
                });
            }
        }
    }
}