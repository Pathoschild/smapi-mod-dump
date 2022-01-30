/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace RingOverhaul
{
    using StardewModdingAPI;
    using System;
    using System.Diagnostics.CodeAnalysis;

    public interface IGenericModConfigMenuApi
    {
        void RegisterModConfig(IManifest mod, Action revertToDefault, Action saveToFile);

        void AddSectionTitle(IManifest mod, Func<string> text, Func<string> tooltip = null);

        void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);

        void AddTextOption(IManifest mod, Func<string> getValue, Action<string> setValue, Func<string> name, Func<string> tooltip = null, string[] allowedValues = null, Func<string, string> formatAllowedValue = null, string fieldId = null);

        void AddParagraph(IManifest mod, Func<string> text);
    }

    /// <summary>
    /// Config file for the mod
    /// </summary>
    public class RingConfig
    {
        public bool CraftableGemRings { get; set; } = true;

        public int CraftableGemRingsMetalBar { get; set; } = 0;

        private static string[] ProgressionChoices { get; set; } = new string[] { "Progressive", "Copper", "Iron", "Gold" };

        public int CraftableGemRingsUnlockLevels { get; set; } = 0;

        private static string[] UnlockLevelChoices { get; set; } = new string[] { "2, 2, 4, 4, 6, 6", "2, 3, 4, 6, 7, 8" };

        public bool CraftableGemRingsCustomSprites { get; set; } = true;

        public bool MinorRingCraftingChanges { get; set; } = true;

        public bool RemoveCrabshellRingAndImmunityBandTooltipFromCombinedRing { get; set; } = true;

        public bool RemoveLuckyTooltipFromCombinedRing { get; set; } = false;

        public bool OldGlowStoneRingRecipe { get; set; } = false;

        public bool OldIridiumBandRecipe { get; set; } = false;

        public bool JukeboxRingEnabled { get; set; } = true;

        public bool JukeboxRingWorksInRain { get; set; } = false;

        public static void VerifyConfigValues(RingOverhaul mod, RingConfig config)
        {
            bool invalidConfig = false;

            if (config.CraftableGemRingsMetalBar < 0 || config.CraftableGemRingsMetalBar > 3)
            {
                invalidConfig = true;
                config.CraftableGemRingsMetalBar = 0;
            }

            if (!(config.CraftableGemRingsUnlockLevels is 0 or 1))
            {
                invalidConfig = true;
                config.CraftableGemRingsUnlockLevels = 0;
            }

            if (invalidConfig)
            {
                mod.DebugLog("At least one config value was out of range and was reset.");
                mod.Helper.WriteConfig(config);
            }

            try
            {
                mod.Helper.Content.InvalidateCache("Data/CraftingRecipes");
                mod.Helper.Content.InvalidateCache("Data/ObjectInformation");
                mod.Helper.Content.InvalidateCache("Maps/springobjects");
            }
            catch (Exception e)
            {
                mod.DebugLog($"Exception when trying to invalidate cache on config change {e}");
            }
        }

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1107:CodeMustNotContainMultipleStatementsOnOneLine", Justification = "Reviewed.")]
        [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Necessary.")]
        public static void SetUpModConfigMenu(RingConfig config, RingOverhaul mod)
        {
            IGenericModConfigMenuApi api = mod.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            if (api == null)
            {
                return;
            }

            var manifest = mod.ModManifest;

            api.RegisterModConfig(manifest, () => config = new RingConfig(), delegate { mod.Helper.WriteConfig(config); VerifyConfigValues(mod, config); });

            api.AddSectionTitle(manifest, () => mod.Helper.Translation.Get("ConfigCraftableGemRings"));

            api.AddBoolOption(manifest, () => config.CraftableGemRings, (bool val) => config.CraftableGemRings = val, () => mod.Helper.Translation.Get("ConfigCraftableGemRings"));

            api.AddTextOption(manifest, () => GetElementFromConfig(ProgressionChoices, config.CraftableGemRingsMetalBar), (string val) => config.CraftableGemRingsMetalBar = GetIndexFromArrayElement(ProgressionChoices, val), () => mod.Helper.Translation.Get("ConfigCraftableGemRingsMetalBar"), null, ProgressionChoices, (s) => TranslateProgressionChoice(s, mod));
            api.AddTextOption(manifest, () => GetElementFromConfig(UnlockLevelChoices, config.CraftableGemRingsUnlockLevels), (string val) => config.CraftableGemRingsUnlockLevels = GetIndexFromArrayElement(UnlockLevelChoices, val), () => mod.Helper.Translation.Get("ConfigCraftableGemRingsUnlockLevel"), null, UnlockLevelChoices);

            if (mod.Helper.ModRegistry.IsLoaded("BBR.BetterRings"))
            {
                api.AddParagraph(manifest, () => mod.Helper.Translation.Get("ConfigCraftableGemRingsCustomSpritesBetterRings"));
            }
            else
            {
                api.AddBoolOption(manifest, () => config.CraftableGemRingsCustomSprites, (bool val) => config.CraftableGemRingsCustomSprites = val, () => mod.Helper.Translation.Get("ConfigCraftableGemRingsCustomSprites"));
            }

            api.AddSectionTitle(manifest, () => mod.Helper.Translation.Get("ConfigOtherCategory"));

            api.AddBoolOption(manifest, () => config.JukeboxRingEnabled, (bool val) => config.JukeboxRingEnabled = val, () => mod.Helper.Translation.Get("ConfigJukeboxRing"));
            api.AddBoolOption(manifest, () => config.JukeboxRingWorksInRain, (bool val) => config.JukeboxRingWorksInRain = val, () => mod.Helper.Translation.Get("ConfigJukeboxRingWorksInRain"));

            api.AddBoolOption(manifest, () => config.MinorRingCraftingChanges, (bool val) => config.MinorRingCraftingChanges = val, () => mod.Helper.Translation.Get("ConfigMinorRingCraftingChanges"));
            api.AddBoolOption(manifest, () => config.RemoveCrabshellRingAndImmunityBandTooltipFromCombinedRing, (bool val) => config.RemoveCrabshellRingAndImmunityBandTooltipFromCombinedRing = val, () => mod.Helper.Translation.Get("ConfigRemoveCITooltip"));
            api.AddBoolOption(manifest, () => config.RemoveLuckyTooltipFromCombinedRing, (bool val) => config.RemoveLuckyTooltipFromCombinedRing = val, () => mod.Helper.Translation.Get("ConfigRemoveLTooltip"));
            api.AddBoolOption(manifest, () => config.OldGlowStoneRingRecipe, (bool val) => config.OldGlowStoneRingRecipe = val, () => mod.Helper.Translation.Get("ConfigOldGlowStoneRingRecipe"));
            api.AddBoolOption(manifest, () => config.OldIridiumBandRecipe, (bool val) => config.OldIridiumBandRecipe = val, () => mod.Helper.Translation.Get("ConfigOldIridiumBandRecipe"));

            api.AddParagraph(manifest, () => mod.Helper.Translation.Get("ConfigFeedback"));
        }

        private static string TranslateProgressionChoice(string englishValue, RingOverhaul mod)
        {
            switch (englishValue)
            {
                case "Progressive":
                    return mod.Helper.Translation.Get("ConfigCraftableGemRingsProgressive");

                case "Copper":
                    return new StardewValley.Object(334, 1).DisplayName;

                case "Iron":
                    return new StardewValley.Object(335, 1).DisplayName;

                case "Gold":
                    return new StardewValley.Object(336, 1).DisplayName;
            }

            return "Unknown Option";
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