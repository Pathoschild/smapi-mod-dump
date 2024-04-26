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

namespace FasterMiniObelisk
{
    internal class ModConfig
    {
 
        public ModConfig()
        {
            // Initialise variables here
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

            // register mod
            configMenu.Register(
                mod: ModEntry.Instance.ModManifest,
                reset: () => ModEntry.Instance.Config = new ModConfig(),
                save: () => ModEntry.Instance.Helper.WriteConfig(ModEntry.Instance.Config)
            );

            /// General travel skill settings header
            configMenu.AddSectionTitle(
                mod: ModEntry.Instance.ModManifest,
                text: I18n.CfgSection_General,
                tooltip: null
            );


        }

        private static string displayExpGainValues(string expgain_option)
        {
            switch (expgain_option)
            {
                case "0.25": return "0.25 (Every 4 tiles)";
                case "0.50": return "0.50 (Every other tile)";
                case "0.75": return "0.75 (2 Exp for 3 tiles)";
                case "1": return "1 (Every tile)";
                case "2": return "2 (Every tile gives two exp)";
                case "5000": return "100 (debug option)";
            }
            return "Something went wrong... :(";
        }

        public static string displayAsPercentage(float value)
        {
            return Math.Round(100f * value, 2).ToString() + "%";
        }

        

    }
}


