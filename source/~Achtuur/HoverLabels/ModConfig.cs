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
using System.Reflection.Emit;

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
        public Dictionary<string, Dictionary<string, bool>> EnabledLabels { get; set; }

        public bool CompactLabels { get; set; }

        public bool SmallItemIconLabel { get; set; }

        public ModConfig()
        {
            // Initialise variables here
            LabelPopupDelayTicks = 30;
            LabelListMaxSize = 6;
            ShowDetailsButton = SButton.LeftControl;
            AlternativeSortButton = SButton.LeftShift;
            EnabledLabels = new();
            CompactLabels = false;
            SmallItemIconLabel = false;
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
                save: () => {
                    this.RegisteredLabelsToDictionary(ModEntry.Instance.LabelManager.RegisteredLabels);
                    ModEntry.Instance.LabelManager.SetLabelEnabled(this);
                    ModEntry.Instance.Helper.WriteConfig(ModEntry.Instance.Config);
                }
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

            configMenu.AddBoolOption(
                mod: ModEntry.Instance.ModManifest,
                name: I18n.CfgCompactLabel_Name,
                tooltip: I18n.CfgCompactLabel_Desc,
                getValue: () => this.CompactLabels,
                setValue: (val) => this.CompactLabels = val
            );

            configMenu.AddBoolOption(
                mod: ModEntry.Instance.ModManifest,
                name: I18n.CfgSmallItemIconLabel_Name,
                tooltip: I18n.CfgSmallItemIconLabel_Desc,
                getValue: () => this.SmallItemIconLabel,
                setValue: (val) => this.SmallItemIconLabel = val
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

            // Add bool option for each registered label, sorted by mod
            IEnumerable<RegisteredLabel> registeredLabels = ModEntry.Instance.LabelManager.RegisteredLabels;
            foreach (IManifest manifest in ModEntry.Instance.LabelManager.GetUniqueRegisteredManifests())
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

        /// <summary>
        /// Transform collection of <see cref="RegisteredLabel"/> into dictionary.
        /// 
        /// Each key of dictionary is a mod name, which holds all the `label name: enabled` combinations
        /// </summary>
        /// <param name="labels"></param>
        /// <returns></returns>
        public void RegisteredLabelsToDictionary(IEnumerable<RegisteredLabel> registeredLabels)
        {
            this.EnabledLabels = registeredLabels
                .GroupBy(label => label.Manifest)
                .ToDictionary(
                    group => group.Key.Name,
                    group => group.ToDictionary(mod_label => mod_label.Name, mod_label => mod_label.Enabled)
                );
        }

        /// <summary>
        /// Returns whether label from mod with manifest <paramref name="manifest"/> and name <paramref name="label_name"/> is enabled.
        /// 
        /// If this label didn't exist yet, returns true so new labels are always enabled by default
        /// </summary>
        /// <param name="manifest"></param>
        /// <param name="label_name"></param>
        /// <returns></returns>
        public bool IsLabelEnabled(IManifest manifest, string label_name)
        {
            if (!this.EnabledLabels.ContainsKey(manifest.Name) 
                || !this.EnabledLabels[manifest.Name].ContainsKey(label_name))
                return true;

            return this.EnabledLabels[manifest.Name][label_name];
        }
    }
}


