/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ferdaber/sdv-mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineralCraftingRecipes
{
    public class Ingredient
    {
        public int ItemId { get; set; }
        public int Amount { get; set; }

        override public string ToString()
        {
            return $"{ItemId} {Amount}";
        }
    }

    public class Recipe
    {
        public string Name { get; set; }
        public List<Ingredient> Ingredients { get; set; }
        public int OutputItemId { get; set; }
    }
}
