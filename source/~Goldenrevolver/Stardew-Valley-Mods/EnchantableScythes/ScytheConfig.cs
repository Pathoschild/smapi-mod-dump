/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace ScytheFixes
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using StardewModdingAPI;
    using System;
    using System.Diagnostics.CodeAnalysis;

    public interface IGenericModConfigMenuAPI
    {
        void RegisterModConfig(IManifest mod, Action revertToDefault, Action saveToFile);

        void RegisterLabel(IManifest mod, string labelName, string labelDesc);

        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<bool> optionGet, Action<bool> optionSet);

        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<int> optionGet, Action<int> optionSet);

        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<float> optionGet, Action<float> optionSet);

        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<string> optionGet, Action<string> optionSet);

        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<SButton> optionGet, Action<SButton> optionSet);

        void RegisterClampedOption(IManifest mod, string optionName, string optionDesc, Func<int> optionGet, Action<int> optionSet, int min, int max);

        void RegisterClampedOption(IManifest mod, string optionName, string optionDesc, Func<float> optionGet, Action<float> optionSet, float min, float max);

        void RegisterChoiceOption(IManifest mod, string optionName, string optionDesc, Func<string> optionGet, Action<string> optionSet, string[] choices);

        void RegisterComplexOption(IManifest mod, string optionName, string optionDesc, Func<Vector2, object, object> widgetUpdate, Func<SpriteBatch, Vector2, object, object> widgetDraw, Action<object> onSave);
    }

    /// <summary>
    /// Config file for the mod
    /// </summary>
    public class ScytheConfig
    {
        public bool GoldenScytheRespawns { get; set; } = true;

        public bool EnchantableScythes { get; set; } = true;

        public bool ScythesCanOnlyGetHaymaker { get; set; } = false;

        public bool OtherWeaponsCannotGetHaymakerAnymore { get; set; } = false;

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1107:CodeMustNotContainMultipleStatementsOnOneLine", Justification = "Reviewed.")]
        [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Necessary.")]
        public static void SetUpModConfigMenu(ScytheConfig config, EnchantableScythes mod)
        {
            IGenericModConfigMenuAPI api = mod.Helper.ModRegistry.GetApi<IGenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");

            if (api == null)
            {
                return;
            }

            var manifest = mod.ModManifest;

            api.RegisterModConfig(manifest, () => config = new ScytheConfig(), delegate { mod.Helper.WriteConfig(config); });

            api.RegisterLabel(manifest, "Enchanting", null);

            api.RegisterSimpleOption(manifest, "Enchantable Scythes", null, () => config.EnchantableScythes, (bool val) => config.EnchantableScythes = val);
            api.RegisterSimpleOption(manifest, "Scythes Can Only Get Haymaker", null, () => config.ScythesCanOnlyGetHaymaker, (bool val) => config.ScythesCanOnlyGetHaymaker = val);
            api.RegisterSimpleOption(manifest, "Other Weapons Cannot\nGet Haymaker Anymore", null, () => config.OtherWeaponsCannotGetHaymakerAnymore, (bool val) => config.OtherWeaponsCannotGetHaymakerAnymore = val);

            api.RegisterLabel(manifest, "Fixes", null);

            api.RegisterSimpleOption(manifest, "Golden Scythe Respawns", null, () => config.GoldenScytheRespawns, (bool val) => config.GoldenScytheRespawns = val);
        }
    }
}