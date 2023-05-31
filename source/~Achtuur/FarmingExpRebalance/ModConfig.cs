/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using FarmingExpRebalance;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Utilities;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmingExpRebalance
{
    internal class ModConfig
    {
        /// <summary>
        /// Bonus multiplier to total movespeed per level of Travelling skill. Defaults to 0.5.
        /// </summary>
        public static float ExpforWateringSoil { get; set; }

        /// <summary>
        /// Multiplier applied to exp gained from harvesting. Defaults to 0.75;
        /// </summary>
        public static float HarvestingExpMultiplier { get; set; }

        /// <summary>
        /// Percentage of stamina that recovers every 10 minutes by <see cref="ProfessionRestoreStamina"/>. Defaults to 1%
        /// </summary>
        public static float RestoreStaminaPercentage { get; set; }


        /// <summary>
        /// Number of steps to walk before getting sprint bonus
        /// </summary>
        public static int SprintSteps { get; set; }

        /// <summary>
        /// Bonus multiplier to movespeed that is applied by sprinting
        /// </summary>
        public static float SprintMovespeedBonus { get; set; }

        /// <summary>
        /// Use chance for a totem when profession is unlocked
        /// </summary>
        public static float TotemUseChance { get; set; }

        /// <summary>
        /// Number of steps to walk before getting 1 Exp
        /// </summary>
        public static int StepsPerExp { get; set; }



        public ModConfig()
        {
            ModConfig.ExpforWateringSoil = 0.05f;
            ModConfig.HarvestingExpMultiplier = 0.75f;
            //ModConfig.RestoreStaminaPercentage = 0.01f;
            //ModConfig.LevelMovespeedBonus = 0.01f;
            //ModConfig.SprintMovespeedBonus = 0.15f;
            //ModConfig.SprintSteps = 5;
            //ModConfig.TotemUseChance = 0.5f;
            //ModConfig.StepsPerExp = 25;
        }

        /// <summary>
        /// Constructs config menu for GenericConfigMenu mod
        /// </summary>
        /// <param name="instance"></param>
        public void createMenu(ModEntry instance)
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = instance.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: instance.ModManifest,
                reset: () => instance.Config = new ModConfig(),
                save: () => instance.Helper.WriteConfig(instance.Config)
            );

            /// General travel skill settings header
            configMenu.AddSectionTitle(
                mod: instance.ModManifest,
                text: I18n.CfgSection_General,
                tooltip: null
            );

            // Harvesting exp multiplier
            configMenu.AddTextOption(
                mod: instance.ModManifest,
                name: I18n.CfgExpperwatersoil_Name,
                tooltip: I18n.CfgExpperwatersoil_Desc,
                getValue: () => ExpforWateringSoil.ToString(),
                setValue: value => ExpforWateringSoil = float.Parse(value),
                allowedValues: new string[] { "0.25", "0.50", "0.75", "1", "2", "5000" },
                formatAllowedValue: displayExpGainValues
             );

            // Exp per watered soil
            configMenu.AddNumberOption(
                mod: instance.ModManifest,
                name: I18n.CfgHarvestexpmultiplier_Name,
                tooltip: I18n.CfgHarvestexpmultiplier_Desc,
                getValue: () => HarvestingExpMultiplier,
                setValue: value => HarvestingExpMultiplier = value,
                min: 50f / 100f,
                max: 100f / 100f,
                interval: 5f / 100f,
                formatValue: displayAsPercentage
             );

            //// Level movespeed bonus
            //configMenu.AddNumberOption(
            //    mod: instance.ModManifest,
            //    name: I18n.CfgLevelmovespeed_Name,
            //    tooltip: I18n.CfgLevelmovespeed_Desc,
            //    getValue: () => LevelMovespeedBonus,
            //    setValue: value => LevelMovespeedBonus = value,
            //    min: 0f / 100f,
            //    max: 2f / 100f,
            //    interval: 0.05f / 100f,
            //    formatValue: displayAsPercentage
            // );

            ///// profession settings header
            //configMenu.AddSectionTitle(
            //    mod: instance.ModManifest,
            //    text: I18n.CfgSection_Professions,
            //    tooltip: null
            //);

            //// Movespeed profession bonus
            //configMenu.AddNumberOption(
            //    mod: instance.ModManifest,
            //    name: I18n.CfgMovespeedbonus_Name,
            //    tooltip: I18n.CfgMovespeedbonus_Desc,
            //    getValue: () => MovespeedProfessionBonus,
            //    setValue: value => MovespeedProfessionBonus = value,
            //    min: 0f / 100f,
            //    max: 10f / 100f,
            //    interval: 0.5f / 100f,
            //    formatValue: displayAsPercentage
            // );

            //// Sprint profession bonus
            //configMenu.AddNumberOption(
            //    mod: instance.ModManifest,
            //    name: I18n.CfgSprintbonus_Name,
            //    tooltip: I18n.CfgSprintbonus_Desc,
            //    getValue: () => SprintMovespeedBonus,
            //    setValue: value => SprintMovespeedBonus = value,
            //    min: 0f / 100f,
            //    max: 30f / 100f,
            //    interval: 0.5f / 100f,
            //    formatValue: displayAsPercentage
            // );

            //// Restore stamina percentage
            //configMenu.AddNumberOption(
            //    mod: instance.ModManifest,
            //    name: I18n.CfgRestorestamina_Name,
            //    tooltip: I18n.CfgRestorestamina_Desc,
            //    getValue: () => RestoreStaminaPercentage,
            //    setValue: value => RestoreStaminaPercentage = value,
            //    min: 0f / 100f,
            //    max: 2f / 100f,
            //    interval: 0.05f / 100f,
            //    formatValue: displayAsPercentage
            // );

            //// Totem reuse
            //configMenu.AddNumberOption(
            //    mod: instance.ModManifest,
            //    name: I18n.CfgTotemreuse_Name,
            //    tooltip: I18n.CfgTotemreuse_Desc,
            //    getValue: () => TotemUseChance,
            //    setValue: value => TotemUseChance = value,
            //    min: 25f / 100f,
            //    max: 75f / 100f,
            //    interval: 5f / 100f,
            //    formatValue: displayAsPercentage
            // );

            // TODO add options for cheap recipes/obelisks
        }

        private static string displayExpGainValues(string expgain_option)
        {
            switch (expgain_option)
            {
                case "0.25": return "0.25 (Every 4 tiles)";
                case "0.50": return "0.50 (Every other tile)";
                case "0.75": return "0.75 (2 Exp for 3 tiles)";
                case "1": return "1 (Every tile)";
                case "2": return "2 (Every tile gives double exp)";
                case "5000": return "100 (debug option)";
            }
            return "Something went wrong... :(";
        }

        public static string displayAsPercentage(float value)
        {
            return Math.Round(100f * value, 2).ToString() + "%";
        }

        

    }

    /// <summary>The API which lets other mods add a config UI through Generic Mod Config Menu.</summary>
    public interface IGenericModConfigMenuApi
    {
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);
        void AddSectionTitle(IManifest mod, Func<string> text, Func<string> tooltip = null);
        void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);
        void AddNumberOption(IManifest mod, Func<int> getValue, Action<int> setValue, Func<string> name, Func<string> tooltip = null, int? min = null, int? max = null, int? interval = null, Func<int, string> formatValue = null, string fieldId = null);
        void AddNumberOption(IManifest mod, Func<float> getValue, Action<float> setValue, Func<string> name, Func<string> tooltip = null, float? min = null, float? max = null, float? interval = null, Func<float, string> formatValue = null, string fieldId = null);
        void AddTextOption(IManifest mod, Func<string> getValue, Action<string> setValue, Func<string> name, Func<string> tooltip = null, string[] allowedValues = null, Func<string, string> formatAllowedValue = null, string fieldId = null);
    }
}


