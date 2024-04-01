/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace EnchantableScythesConfig
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
    public class EnchantableScythesConfig
    {
        public bool GoldenScytheRespawns { get; set; } = false;

        public bool EnchantableScythes { get; set; } = true;

        public bool ScythesCanOnlyGetHaymaker { get; set; } = false;

        public bool OtherWeaponsCannotGetHaymakerAnymore { get; set; } = false;

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1107:CodeMustNotContainMultipleStatementsOnOneLine", Justification = "Reviewed.")]
        [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Necessary.")]
        public static void SetUpModConfigMenu(EnchantableScythesConfig config, EnchantableScythes mod)
        {
            IGenericModConfigMenuApi api = mod.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            if (api == null)
            {
                return;
            }

            var manifest = mod.ModManifest;

            api.Register(
                mod: manifest,
                reset: () => config = new EnchantableScythesConfig(),
                save: () => mod.Helper.WriteConfig(config)
            );

            api.AddSectionTitle(manifest, () => "Enchanting", null);

            api.AddBoolOption(manifest, () => config.EnchantableScythes, (bool val) => config.EnchantableScythes = val, () => "Enchantable Scythes", null);
            api.AddBoolOption(manifest, () => config.ScythesCanOnlyGetHaymaker, (bool val) => config.ScythesCanOnlyGetHaymaker = val, () => "Scythes Can Only Get Haymaker", null);
            api.AddBoolOption(manifest, () => config.OtherWeaponsCannotGetHaymakerAnymore, (bool val) => config.OtherWeaponsCannotGetHaymakerAnymore = val, () => "Other Weapons Cannot\nGet Haymaker Anymore", null);

            api.AddSectionTitle(manifest, () => "Fixes", null);

            api.AddBoolOption(manifest, () => config.GoldenScytheRespawns, (bool val) => config.GoldenScytheRespawns = val, () => "Golden Scythe Respawns", () => "The 1.6 update made it a lot harder to lose it, so this is disabled by default now.");
        }
    }
}