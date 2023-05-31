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

namespace Omegasis.Revitalize.Framework.Crafting.JsonContent
{
    public class UnlockableJsonCraftingRecipe
    {
        public string whichTab;
        public bool isUnlocked;
        public JsonCraftingRecipe recipe;


        public UnlockableJsonCraftingRecipe()
        {
            this.whichTab = "";
            this.isUnlocked = true;
            this.recipe = new JsonCraftingRecipe();
        }

        public UnlockableJsonCraftingRecipe(string WhichTab, JsonCraftingRecipe recipe, bool HasUnlocked = false)
        {
            this.recipe = recipe;
            this.isUnlocked = HasUnlocked;
            this.whichTab = WhichTab;
        }

        public virtual UnlockableCraftingRecipe toUnlockableCraftingRecipe()
        {
            return new UnlockableCraftingRecipe(this.whichTab, this.recipe.toCraftingRecipe(), this.isUnlocked);
        }
    }
}
