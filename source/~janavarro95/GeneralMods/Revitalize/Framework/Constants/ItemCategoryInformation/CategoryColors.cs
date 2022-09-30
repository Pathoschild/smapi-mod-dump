/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Omegasis.Revitalize.Framework.Constants.ItemCategoryInformation
{
    /// <summary>
    /// Used to determine the color of the machine category text when displayed in tool tips and inventories.
    /// </summary>
    public static class CategoryColors
    {
        public static Color Crafting = Color.Brown;

        public static Color Farming = Color.SaddleBrown;

        public static Color Machines = Color.SteelBlue;
        public static Color Misc = Color.LightGray;

        public static Color Ore = Color.Silver;

        public static Color Resource = Color.Brown;
    }
}
