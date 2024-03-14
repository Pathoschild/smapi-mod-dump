/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace CrabPotQuality
{
    using StardewModdingAPI;
    using System;
    using System.Diagnostics.CodeAnalysis;

    public interface IGenericModConfigMenuApi
    {
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);

        void AddSectionTitle(IManifest mod, Func<string> text, Func<string> tooltip = null);

        void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);
    }

    /// <summary>
    /// Config file for the mod
    /// </summary>
    public class CrabPotQualityConfig
    {
        public bool MarinerPerkForcesIridiumQuality { get; set; } = true;

        public bool LuremasterPerkForcesIridiumQuality { get; set; } = true;

        public bool EnableWildBaitEffect { get; set; } = true;

        public bool EnableMagicBaitEffect { get; set; } = true;

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1107:CodeMustNotContainMultipleStatementsOnOneLine", Justification = "Reviewed.")]
        [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Necessary.")]
        public static void SetUpModConfigMenu(CrabPotQualityConfig config, CrabPotQuality mod)
        {
            IGenericModConfigMenuApi api = mod.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            if (api == null)
            {
                return;
            }

            var manifest = mod.ModManifest;

            api.Register(
                mod: manifest,
                reset: () => config = new CrabPotQualityConfig(),
                save: () => mod.Helper.WriteConfig(config)
            );

            api.AddSectionTitle(manifest, () => "Iridium Quality When", null);

            api.AddBoolOption(manifest, () => config.MarinerPerkForcesIridiumQuality, (bool val) => config.MarinerPerkForcesIridiumQuality = val, () => "Has Mariner Perk", null);
            api.AddBoolOption(manifest, () => config.LuremasterPerkForcesIridiumQuality, (bool val) => config.LuremasterPerkForcesIridiumQuality = val, () => "Has Luremaster Perk", null);

            api.AddSectionTitle(manifest, () => "Bait Effects", null);

            api.AddBoolOption(manifest, () => config.EnableWildBaitEffect, (bool val) => config.EnableWildBaitEffect = val, () => "Wild Bait Double Quality Chance", null);
            api.AddBoolOption(manifest, () => config.EnableMagicBaitEffect, (bool val) => config.EnableMagicBaitEffect = val, () => "Magic Bait Creates Rainbow Shell", null);
        }
    }
}