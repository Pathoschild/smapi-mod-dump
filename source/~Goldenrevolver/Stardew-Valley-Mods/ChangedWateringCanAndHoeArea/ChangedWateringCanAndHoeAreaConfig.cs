/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace ChangedWateringCanAndHoeArea
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
    public class ChangedWateringCanAndHoeAreaConfig
    {
        public bool EnableWateringCanChange { get; set; } = true;

        public bool EnableHoeChange { get; set; } = true;

        public bool EnableIridiumWithReachingBuff { get; set; } = true;

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1107:CodeMustNotContainMultipleStatementsOnOneLine", Justification = "Reviewed.")]
        [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Necessary.")]
        public static void SetUpModConfigMenu(ChangedWateringCanAndHoeAreaConfig config, ChangedWateringCanAndHoeArea mod)
        {
            IGenericModConfigMenuApi api = mod.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            if (api == null)
            {
                return;
            }

            var manifest = mod.ModManifest;

            api.Register(
                mod: manifest,
                reset: () => config = new ChangedWateringCanAndHoeAreaConfig(),
                save: () => mod.Helper.WriteConfig(config)
            );

            api.AddBoolOption(manifest, () => config.EnableWateringCanChange, (bool val) => config.EnableWateringCanChange = val, () => "Enable Watering Can Change", null);
            api.AddBoolOption(manifest, () => config.EnableHoeChange, (bool val) => config.EnableHoeChange = val, () => "Enable Hoe Change", null);
            api.AddBoolOption(manifest, () => config.EnableIridiumWithReachingBuff, (bool val) => config.EnableIridiumWithReachingBuff = val, () => "Enable Iridium With Reaching Buff", null);
        }
    }
}