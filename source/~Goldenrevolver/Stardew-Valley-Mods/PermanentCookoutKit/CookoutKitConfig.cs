/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace PermanentCookoutKit
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
    public class CookoutKitConfig
    {
        public int WoodNeeded { get; set; } = 5;

        public int CoalNeeded { get; set; } = 1;

        public int FiberNeeded { get; set; } = 1;

        public float DriftwoodMultiplier { get; set; } = 5;

        public float HardwoodMultiplier { get; set; } = 5;

        public float NewspaperMultiplier { get; set; } = 2;

        public float WoolMultiplier { get; set; } = 5;

        public float ClothMultiplier { get; set; } = 10;

        public int CharcoalKilnWoodNeeded { get; set; } = 10;

        public int CharcoalKilnTimeNeeded { get; set; } = 30;

        public static void VerifyConfigValues(CookoutKitConfig config, PermanentCookoutKit mod)
        {
            bool invalidConfig = false;

            if (config.WoodNeeded < 0)
            {
                invalidConfig = true;
                config.WoodNeeded = 0;
            }

            if (config.FiberNeeded < 0)
            {
                invalidConfig = true;
                config.FiberNeeded = 0;
            }

            if (config.CoalNeeded < 0)
            {
                invalidConfig = true;
                config.CoalNeeded = 0;
            }

            if (config.DriftwoodMultiplier < 0)
            {
                invalidConfig = true;
                config.DriftwoodMultiplier = 0;
            }

            if (config.HardwoodMultiplier < 0)
            {
                invalidConfig = true;
                config.HardwoodMultiplier = 0;
            }

            if (config.NewspaperMultiplier < 0)
            {
                invalidConfig = true;
                config.NewspaperMultiplier = 0;
            }

            if (config.WoolMultiplier < 0)
            {
                invalidConfig = true;
                config.WoolMultiplier = 0;
            }

            if (config.ClothMultiplier < 0)
            {
                invalidConfig = true;
                config.ClothMultiplier = 0;
            }

            if (config.CharcoalKilnWoodNeeded < 1)
            {
                invalidConfig = true;
                config.CharcoalKilnWoodNeeded = 1;
            }

            if (config.CharcoalKilnTimeNeeded < 10)
            {
                invalidConfig = true;
                config.CharcoalKilnTimeNeeded = 10;
            }

            if (invalidConfig)
            {
                mod.DebugLog("At least one config value was out of range and was reset.");
                mod.Helper.WriteConfig(config);
            }
        }

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1107:CodeMustNotContainMultipleStatementsOnOneLine", Justification = "Reviewed.")]
        [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Necessary.")]
        public static void SetUpModConfigMenu(CookoutKitConfig config, PermanentCookoutKit mod)
        {
            IGenericModConfigMenuAPI api = mod.Helper.ModRegistry.GetApi<IGenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");

            if (api == null)
            {
                return;
            }

            var manifest = mod.ModManifest;

            api.RegisterModConfig(manifest, () => config = new CookoutKitConfig(), delegate { mod.Helper.WriteConfig(config); VerifyConfigValues(config, mod); });

            api.RegisterLabel(manifest, "Cookout Kit Reignition Cost", null);

            api.RegisterSimpleOption(manifest, "Wood Needed", null, () => config.WoodNeeded, (int val) => config.WoodNeeded = val);
            api.RegisterSimpleOption(manifest, "Coal Needed", null, () => config.CoalNeeded, (int val) => config.CoalNeeded = val);
            api.RegisterSimpleOption(manifest, "Fiber/ Kindling Needed", null, () => config.FiberNeeded, (int val) => config.FiberNeeded = val);

            api.RegisterLabel(manifest, "Charcoal Kiln", null);

            api.RegisterSimpleOption(manifest, "Wood Needed", "Also works with driftwood and hardwood", () => config.CharcoalKilnWoodNeeded, (int val) => config.CharcoalKilnWoodNeeded = val);
            api.RegisterSimpleOption(manifest, "Time Needed", "The game only checks every 10 minutes", () => config.CharcoalKilnTimeNeeded, (int val) => config.CharcoalKilnTimeNeeded = val);

            api.RegisterLabel(manifest, "Wood Multipliers", null);

            api.RegisterSimpleOption(manifest, "Driftwood Multiplier¹", null, () => config.DriftwoodMultiplier, (float val) => config.DriftwoodMultiplier = val);
            api.RegisterSimpleOption(manifest, "Hardwood Multiplier¹", null, () => config.HardwoodMultiplier, (float val) => config.HardwoodMultiplier = val);

            api.RegisterLabel(manifest, "Kindling Multipliers", null);

            api.RegisterSimpleOption(manifest, "Newspaper Multiplier¹", null, () => config.NewspaperMultiplier, (float val) => config.NewspaperMultiplier = val);
            api.RegisterSimpleOption(manifest, "Wool Multiplier¹", null, () => config.WoolMultiplier, (float val) => config.WoolMultiplier = val);
            api.RegisterSimpleOption(manifest, "Cloth Multiplier¹", null, () => config.ClothMultiplier, (float val) => config.ClothMultiplier = val);

            // this is a spacer
            api.RegisterLabel(manifest, string.Empty, null);
            api.RegisterLabel(manifest, "1: Set To 0 To Disallow Using It", null);
        }
    }
}