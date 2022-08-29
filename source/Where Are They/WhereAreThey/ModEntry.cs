/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/Capaldi12/wherearethey
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

using GenericModConfigMenu;


namespace WhereAreThey
{
    class ModEntry : Mod
    {
        private LocationOverlay ld = null;
        internal OverlayConfig Config;

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<OverlayConfig>();

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            ld = new LocationOverlay(this);
        }

        private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            ld.Dispose();
            ld = null;
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            RegisterConfigMenu();
        }

        private void RegisterConfigMenu()
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var menu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            if (menu is null) return;

            menu.Register(
                mod: ModManifest,
                reset: () => Config = new(),
                save: () => Helper.WriteConfig(Config)
            );

            // This section is about how mod works
            menu.AddSectionTitle(ModManifest,

                text: () => Helper.Translation.Get("Config_SectionBehaviourName"),
                tooltip: () => Helper.Translation.Get("Config_SectionBehaviourTooltip")
            );

            // DisplaySelf
            menu.AddBoolOption(ModManifest,

                getValue: () => Config.DisplaySelf,
                setValue: (value) => Config.DisplaySelf = value,
                name: () => Helper.Translation.Get("Config_DisplaySelfName"),
                tooltip: () => Helper.Translation.Get("Config_DisplaySelfTooltip")

            );

            // HideInSingleplayer
            menu.AddBoolOption(ModManifest,

                getValue: () => Config.HideInSingleplayer,
                setValue: (value) => Config.HideInSingleplayer = value,
                name: () => Helper.Translation.Get("Config_HideInSingleplayerName"),
                tooltip: () => Helper.Translation.Get("Config_HideInSingleplayerTooltip")

            );

            // HighlightSameLocation
            menu.AddBoolOption(ModManifest,

                getValue: () => Config.HighlightSameLocation,
                setValue: (value) => Config.HighlightSameLocation = value,
                name: () => Helper.Translation.Get("Config_HighlightSameLocationName"),
                tooltip: () => Helper.Translation.Get("Config_HighlightSameLocationTooltip")

            );

            // HideInCutscene
            menu.AddBoolOption(ModManifest,

                getValue: () => Config.HideInCutscene,
                setValue: (value) => Config.HideInCutscene = value,
                name: () => Helper.Translation.Get("Config_HideInCutsceneName"),
                tooltip: () => Helper.Translation.Get("Config_HideInCutsceneTooltip")

            );

            // HideAtFestival
            menu.AddBoolOption(ModManifest,

                getValue: () => Config.HideAtFestival,
                setValue: (value) => Config.HideAtFestival = value,
                name: () => Helper.Translation.Get("Config_HideAtFestivalName"),
                tooltip: () => Helper.Translation.Get("Config_HideAtFestivalTooltip")

            );

            // This section is about how mod looks
            menu.AddSectionTitle(ModManifest,

                text: () => Helper.Translation.Get("Config_SectionAppearanceName"),
                tooltip: () => Helper.Translation.Get("Config_SectionAppearanceTooltip")
            );

            // Position
            menu.AddTextOption(ModManifest,

                getValue: () => Config.position,
                setValue: value => Config.position = value,
                name: () => Helper.Translation.Get("Config_PositionName"),
                tooltip: () => Helper.Translation.Get("Config_PositionTooltip"),
                allowedValues: new string[] { "TopLeft", "TopRight", "BottomLeft", "BottomRight" },
                formatAllowedValue: value => Helper.Translation.Get($"Config_{value}")
            );

            // VOffset
            menu.AddNumberOption(ModManifest,

                getValue: () => Config.VOffset,
                setValue: value => Config.VOffset = value,
                name: () => Helper.Translation.Get("Config_VOffsetName"),
                tooltip: () => Helper.Translation.Get("Config_VOffsetTooltip"),
                min: 0
            );

            // HOffset
            menu.AddNumberOption(ModManifest,

                getValue: () => Config.HOffset,
                setValue: value => Config.HOffset = value,
                name: () => Helper.Translation.Get("Config_HOffsetName"),
                tooltip: () => Helper.Translation.Get("Config_HOffsetTooltip"),
                min: 0
            );

            // VPadding
            menu.AddNumberOption(ModManifest,

                getValue: () => Config.VPadding,
                setValue: value => Config.VPadding = value,
                name: () => Helper.Translation.Get("Config_VPaddingName"),
                tooltip: () => Helper.Translation.Get("Config_VPaddingTooltip"),
                min: 0
            );

            // HPadding
            menu.AddNumberOption(ModManifest,

                getValue: () => Config.HPadding,
                setValue: value => Config.HPadding = value,
                name: () => Helper.Translation.Get("Config_HPaddingName"),
                tooltip: () => Helper.Translation.Get("Config_HPaddingTooltip"),
                min: 0
            );

            // Spacing
            menu.AddNumberOption(ModManifest,

                getValue: () => Config.Spacing,
                setValue: value => Config.Spacing = value,
                name: () => Helper.Translation.Get("Config_SpacingName"),
                tooltip: () => Helper.Translation.Get("Config_SpacingTooltip"),
                min: 0
            );

            // IconSpacing
            menu.AddNumberOption(ModManifest,

                getValue: () => Config.IconSpacing,
                setValue: value => Config.IconSpacing = value,
                name: () => Helper.Translation.Get("Config_IconSpacingName"),
                tooltip: () => Helper.Translation.Get("Config_IconSpacingTooltip"),
                min: 0
            );

            // IconPosition
            menu.AddTextOption(ModManifest,

                getValue: () => Config.iconPosition,
                setValue: value => Config.iconPosition = value,
                name: () => Helper.Translation.Get("Config_IconPositionName"),
                tooltip: () => Helper.Translation.Get("Config_IconPositionTooltip"),
                allowedValues: new string[] { "IconLeft", "IconRight" },
                formatAllowedValue: value => Helper.Translation.Get($"Config_{value}")
            );
        }
    }
}
