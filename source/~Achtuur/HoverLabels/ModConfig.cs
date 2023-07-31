/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Utilities;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AchtuurCore.Integrations;
using System.Security;
using HoverLabels.Framework;
using AchtuurCore.Utility;

namespace HoverLabels
{
    internal class ModConfig
    {
        private readonly SliderRange LabelPopupDelayMsRange = new(0f, 1f, 0.05f);

        private static bool registered = false;
        public int LabelPopupDelayTicks { get; set; }

        public int LabelListMaxSize { get; set; }

        public SButton ShowDetailsButton { get; set; }
        public SButton AlternativeSortButton { get; set; }

        public ModConfig()
        {
            // Initialise variables here
            LabelPopupDelayTicks = 30;
            LabelListMaxSize = 5;
            ShowDetailsButton = SButton.LeftControl;
            AlternativeSortButton = SButton.LeftShift;
        }

        /// <summary>
        /// Constructs config menu for GenericConfigMenu mod
        /// </summary>
        /// <param name="instance"></param>
        public void createMenu()
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = ModEntry.Instance.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            if (registered)
                configMenu.Unregister(ModEntry.Instance.ModManifest);

            // register mod
            configMenu.Register(
                mod: ModEntry.Instance.ModManifest,
                reset: () => ModEntry.Instance.Config = new ModConfig(),
                save: () => ModEntry.Instance.Helper.WriteConfig(ModEntry.Instance.Config)
            );

            registered = true;

            /// General travel skill settings header
            configMenu.AddSectionTitle(
                mod: ModEntry.Instance.ModManifest,
                text: I18n.CfgSection_General,
                tooltip: null
            );

            configMenu.AddNumberOption(
                mod: ModEntry.Instance.ModManifest,
                name: I18n.CfgDelayms_Name,
                tooltip: I18n.CfgDelayms_Desc,
                getValue: () => (float) (LabelPopupDelayTicks / 60f),
                setValue: (val) => this.LabelPopupDelayTicks = (int) (val * 60f),
                min: LabelPopupDelayMsRange.min,
                max: LabelPopupDelayMsRange.max,
                interval: LabelPopupDelayMsRange.interval
            );

            configMenu.AddKeybind(
                mod: ModEntry.Instance.ModManifest,
                name: I18n.CfgButtonShowdetails_Name,
                tooltip: I18n.CfgButtonShowdetails_Desc,
                getValue: () => this.ShowDetailsButton,
                setValue: (button) => this.ShowDetailsButton = button
            );

            configMenu.AddKeybind(
                mod: ModEntry.Instance.ModManifest,
                name: I18n.CfgButtonAlternativesort_Name,
                tooltip: I18n.CfgButtonAlternativesort_Desc,
                getValue: () => this.AlternativeSortButton,
                setValue: (button) => this.AlternativeSortButton = button
            );


            IEnumerable<RegisteredLabel> registeredLabels = LabelManager.RegisteredLabels;
            foreach (IManifest manifest in LabelManager.GetUniqueRegisteredManifests())
            {
                configMenu.AddSectionTitle(
                    mod: ModEntry.Instance.ModManifest,
                    text: () => I18n.CfgSection_Enablemod(manifest.Name, manifest.Author),
                    tooltip: null
                );

                IEnumerable<RegisteredLabel> modRegisteredLabels = registeredLabels
                    .Where(l => l.Manifest == manifest)
                    .OrderBy(l => l.Name);

                foreach(RegisteredLabel registeredLabel in modRegisteredLabels)
                {
                    configMenu.AddBoolOption(
                        mod: ModEntry.Instance.ModManifest,
                        name: () => I18n.CfgEnableLabel_Name(registeredLabel.Name),
                        tooltip: () => I18n.CfgEnableLabel_Desc(registeredLabel.Name),
                        getValue: () => registeredLabel.Enabled,
                        setValue: (val) => registeredLabel.Enabled = val
                    );
                }
            }
        }
    }
}


