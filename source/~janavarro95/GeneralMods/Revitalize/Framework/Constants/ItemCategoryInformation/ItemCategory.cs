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
    /// A given category for a specific <see cref="StardewValley.Item"/>
    /// </summary>
    public class ItemCategory
    {
        /// <summary>
        /// The name of the category.
        /// </summary>
        public string name;
        /// <summary>
        /// The color of the category.
        /// </summary>
        public Color color;

        public ItemCategory()
        {

        }

        public ItemCategory(string name, Color color)
        {
            this.name = name;
            this.color = color;
        }
    }
}
