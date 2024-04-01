/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ChroniclerCherry/stardew-valley-mods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using StardewValley.Inventories;
using StardewValley.Menus;

namespace CustomCraftingStations.Framework
{
    public class CustomCraftingMenu : CraftingPage
    {
        private readonly List<string> CraftingRecipes;
        private readonly List<string> CookingRecipes;

        public CustomCraftingMenu(int x, int y, int width, int height, List<IInventory> materialContainers, List<string> craftingRecipes, List<string> cookingRecipes)
            : base(x, y, width, height, standaloneMenu: true, materialContainers: materialContainers)
        {
            this.CraftingRecipes = craftingRecipes;
            this.CookingRecipes = cookingRecipes;

            this.RepositionElements();
        }

        /// <summary>Get the recipes to display in the menu.</summary>
        protected override List<string> GetRecipesToDisplay()
        {
            return
                this.CraftingRecipes?.Union(this.CookingRecipes).ToList()
                ?? base.GetRecipesToDisplay(); // not initialized yet
        }
    }
}
