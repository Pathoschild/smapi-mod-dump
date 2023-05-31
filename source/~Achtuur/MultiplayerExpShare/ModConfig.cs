/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using Pathoschild.Stardew.Common.Integrations.GenericModConfigMenu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewTravelSkill
{
    internal class ModConfig
    {
        /// <summary>
        /// Other farmers must be within this range to count as nearby
        /// </summary>
        public static int NearbyPlayerTileRange { get; set; }

        /// <summary>
        /// Percentage of exp that goes to actor, rest of exp is divided equally between nearby players
        /// </summary>
        public static float ExpPercentageToActor { get; set; }

        public ModConfig()
        {
            // Changable by player
            ModConfig.NearbyPlayerTileRange = 30;

            // Unchangable by player
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
                text: I18n.CfgGeneral,
                tooltip: null
            );

            // exp percentage to actor
            configMenu.AddTextOption(
                mod: instance.ModManifest,
                name: I18n.CfgExptoactor_Name,
                tooltip: I18n.CfgExptoactor_Desc,
                getValue: () => StepsPerExp.ToString(),
                setValue: value => StepsPerExp = float.Parse(value),
                min: 25f/100f,
                max: 75f/100f,
                interval: 5f/100f,
                formatAllowedValue: displayAsPercentage
             );

            // nearby player tile range
            configMenu.AddNumberOption(
                mod: instance.ModManifest,
                name: I18n.CfgNearbyplayertilerange_Name,
                tooltip: I18n.CfgLevelmovespeed_Desc,
                getValue: () => NearbyPlayerTileRange,
                setValue: value => NearbyPlayerTileRange = value,
                min: 10,
                max: 50,
                interval: 5,
             );
        }

        private static string textoption(string expgain_option)
        {
            switch (expgain_option)
            {
                case "0.25": return "25%";
                case "10": return "10 (Fast)";
                case "25": return "25 (Normal)";
                case "50": return "50 (Slow)";
                case "100": return "100 (Very Slow)";
            }
            // should be unreachable, if this ever appears then you made a mistake sir programmer
            return "Something went wrong... :(";
        }

        /// <summary>
        /// Displays <paramref name="value"/> as a percentage, rounded to two decimals.
        /// <c>ModConfig.displayAsPercentage(0.02542); // returns 2.54%</c>
        /// </summary>
        public static string displayAsPercentage(float value)
        {
            return Math.Round(100f * value, 2).ToString() + "%";
        }
    }
}
