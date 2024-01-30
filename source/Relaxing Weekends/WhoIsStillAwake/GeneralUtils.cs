/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jamespfluger/Stardew-ModCollection
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace WhoIsStillAwakeMod.Helpers
{
    public static class GeneralUtils
    {
        /// <summary>
        /// Gets the foramtted string of the number of farmers in bed vs the total number of farmers
        /// </summary>
        /// <returns>The formatted string of farmers in bed/farmers awake</returns>
        public static string GetRatioOfFarmersInBed()
        {
            List<Farmer> allFarmers = Game1.getOnlineFarmers().ToList();

            int farmersInBed = allFarmers.Select(f => f).Where(a => a.isInBed).Count();
            int totalFarmers = allFarmers.Count;

            return $"{Game1.player.team.GetNumberReady("sleep")}/{totalFarmers}";
        }

        /// <summary>
        /// Converts a hex color string into an XNA Framework Color object
        /// </summary>
        /// <param name="hexColor">String of a hex color</param>
        /// <returns>A new Color object</returns>
        public static Color ConvertHexToColor(string hexColor)
        {
            Regex hexValidator = new Regex("^#?(?:[0-9a-fA-F]{3}){2}$");

            if (!hexValidator.IsMatch(hexColor))
            {
                return Color.Gray;
            }

            if (hexColor.StartsWith("#"))
            {
                hexColor = hexColor.Substring(1);
            }

            int r = int.Parse(hexColor.Substring(0, 2), NumberStyles.AllowHexSpecifier);
            int g = int.Parse(hexColor.Substring(2, 2), NumberStyles.AllowHexSpecifier);
            int b = int.Parse(hexColor.Substring(4, 2), NumberStyles.AllowHexSpecifier);

            return new Color(r, g, b);
        }
    }
}
