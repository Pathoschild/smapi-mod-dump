/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tadfoster/StardewValleyMods
**
*************************************************/

﻿using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using GMCMOptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace DayLimiter
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        public static ModConfig _config;
        public static IModHelper _helper;
        public static int DayCount;
        public static bool IsQuitting;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            initMod();

            _helper = helper;

            _helper.Events.GameLoop.DayStarted += DayStartedEvent;

            _helper.Events.GameLoop.DayEnding += DayEndingEvent;

            _helper.Events.GameLoop.ReturnedToTitle += ReturnedToTitleEvent;

            _helper.Events.Display.MenuChanged += MenuChangedEvent;

            _helper.Events.GameLoop.GameLaunched += GameLaunchedEvent;
        }

        private void DayStartedEvent(object? sender, DayStartedEventArgs e)
        {
            if (_config.ModEnabled) 
            {
                if (DayCount == (_config.DayLimitCount - 1))
                {
                    Game1.drawObjectDialogue(_helper.Translation.Get("Message_FinalDay"));
                }
                else if (DayCount >= _config.DayLimitCount)
                {
                    Game1.drawObjectDialogue(_helper.Translation.Get("Message_ShutDown"));

                    IsQuitting = true;
                }
            }
        }

        private void DayEndingEvent(object? sender, DayEndingEventArgs e)
        {
            if (_config.ModEnabled)
            {
                DayCount++;
            }
        }

        private void ReturnedToTitleEvent(object? sender, ReturnedToTitleEventArgs e)
        {
            initMod();
        }

        private void MenuChangedEvent(object? sender, MenuChangedEventArgs e)
        {
            if (_config.ModEnabled && IsQuitting && Game1.hasLoadedGame && Game1.activeClickableMenu == null)
            {
                if (_config.ExitToTitle)
                {
                    Game1.exitToTitle = true;
                }
                else
                {
                    Game1.quit = true;
                }
            }
        }

        private void GameLaunchedEvent(object? sender, GameLaunchedEventArgs e)
        {
            IGenericModConfigMenuApi? configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            if (configMenu != null)
            {
                configMenu.Register(
                    mod: ModManifest,
                    reset: () => {
                        _config = new ModConfig();
                    },
                    save: () => {
                        ModConfig savedConfig = Helper.ReadConfig<ModConfig>();

                        Helper.WriteConfig(new ModConfig() { ModEnabled = savedConfig.ModEnabled, DayLimitCount = _config.DayLimitCount, ExitToTitle = _config.ExitToTitle });
                    }
                );

                IGMCMOptionsAPI? configMenuExt = Helper.ModRegistry.GetApi<IGMCMOptionsAPI>("jltaylor-us.GMCMOptions");

                if (configMenuExt != null)
                {
                    string daysPlayedStr = _helper.Translation.Get("GMCM_Option_DaysPlayed");
                    string daysRemainingStr = _helper.Translation.Get("GMCM_Option_DaysRemaining");

                    configMenuExt.AddDynamicParagraph(
                        mod: ModManifest,
                        logName: "dayCounter",
                        text: () => Game1.hasLoadedGame && _config.ModEnabled ? $"{daysPlayedStr}: {DayCount}    {daysRemainingStr}: {Math.Max(_config.DayLimitCount - DayCount, 0)}" : "",
                        isStyledText: true
                    );
                }

                configMenu.AddBoolOption(
                    mod: ModManifest,
                    name: () => _helper.Translation.Get("GMCM_Option_ModEnabled_Name"),
                    getValue: () => _config.ModEnabled,
                    setValue: value => setModEnabledConfig(value),
                    fieldId: "DayLimitEnabled"
                );

                configMenu.AddNumberOption(
                    mod: ModManifest,
                    name: () => _helper.Translation.Get("GMCM_Option_DayLimitCount_Name"),
                    getValue: () => _config.DayLimitCount,
                    setValue: value => _config.DayLimitCount = value,
                    min: 1,
                    max: 100,
                    fieldId: "DayLimitCount"
                );

                configMenu.AddBoolOption(
                    mod: ModManifest,
                    name: () => _helper.Translation.Get("GMCM_Option_ExitToTitle_Name"),
                    getValue: () => _config.ExitToTitle,
                    setValue: value => _config.ExitToTitle = value
                );
            }
        }

        private void initMod()
        {
            DayCount = 0;
            IsQuitting = false;

            _config = Helper.ReadConfig<ModConfig>();
        }

        private void setModEnabledConfig(bool value)
        {
            DayCount = _config.ModEnabled != value ? 0 : DayCount;

            _config.ModEnabled = value;
        }
    }
}