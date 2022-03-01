/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace MushroomRancher
{
    using StardewModdingAPI;
    using System;
    using System.Diagnostics.CodeAnalysis;

    public interface IGenericModConfigMenuApi
    {
        void RegisterModConfig(IManifest mod, Action revertToDefault, Action saveToFile);

        void RegisterLabel(IManifest mod, string labelName, string labelDesc);

        void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);

        void AddTextOption(IManifest mod, Func<string> getValue, Action<string> setValue, Func<string> name, Func<string> tooltip = null, string[] allowedValues = null, Func<string, string> formatAllowedValue = null, string fieldId = null);

        void AddParagraph(IManifest mod, Func<string> text);
    }

    /// <summary>
    /// Config file for the mod
    /// </summary>
    public class MushroomRancherConfig
    {
        public int HutchInterior { get; set; } = 1;

        public bool RandomizeMonsterPositionInHutch { get; set; } = true;

        public bool RemovableSlimeHutchIncubator { get; set; } = true;

        internal static string[] InteriorChoices { get; set; } = new string[] { "Disabled", "Dynamic", "Slime", "Cave", "Volcano" };

        public static void VerifyConfigValues(MushroomRancherConfig config, MushroomRancher mod)
        {
            if (config.HutchInterior < 0 || config.HutchInterior >= InteriorChoices.Length)
            {
                config.HutchInterior = 1;
                mod.DebugLog("At least one config value was out of range and was reset.");
                mod.Helper.WriteConfig(config);
            }
        }

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1107:CodeMustNotContainMultipleStatementsOnOneLine", Justification = "Reviewed.")]
        [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Necessary.")]
        public static void SetUpModConfigMenu(MushroomRancherConfig config, MushroomRancher mod)
        {
            IGenericModConfigMenuApi api = mod.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            if (api == null)
            {
                return;
            }

            var manifest = mod.ModManifest;

            api.RegisterModConfig(manifest, () => config = new MushroomRancherConfig(), delegate { mod.Helper.WriteConfig(config); VerifyConfigValues(config, mod); });

            api.AddTextOption(manifest, () => GetElementFromConfig(InteriorChoices, config.HutchInterior), (string val) => config.HutchInterior = GetIndexFromArrayElement(InteriorChoices, val), () => mod.Helper.Translation.Get("ConfigHutchInterior"), null, InteriorChoices, (s) => TranslateSpeechChoice(s, mod));
            api.AddBoolOption(manifest, () => config.RemovableSlimeHutchIncubator, (bool val) => config.RemovableSlimeHutchIncubator = val, () => mod.Helper.Translation.Get("ConfigRemovableSlimeHutchIncubator"));
            api.AddBoolOption(manifest, () => config.RandomizeMonsterPositionInHutch, (bool val) => config.RandomizeMonsterPositionInHutch = val, () => mod.Helper.Translation.Get("ConfigRandomizeMonsterPositionInHutch"));
        }

        private static string TranslateSpeechChoice(string englishValue, MushroomRancher mod)
        {
            return mod.Helper.Translation.Get("ConfigHutchInterior" + englishValue);
        }

        private static string GetElementFromConfig(string[] options, int config)
        {
            if (config >= 0 && config < options.Length)
            {
                return options[config];
            }
            else
            {
                return options[0];
            }
        }

        private static int GetIndexFromArrayElement(string[] options, string element)
        {
            var index = Array.IndexOf(options, element);

            return index == -1 ? 0 : index;
        }
    }
}