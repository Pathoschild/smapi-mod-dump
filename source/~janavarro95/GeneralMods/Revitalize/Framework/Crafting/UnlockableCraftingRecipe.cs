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

namespace Revitalize.Framework.Crafting
{
    public class UnlockableCraftingRecipe
    {
        public Recipe recipe;
        public bool hasUnlocked;
        public string whichTab;

        public UnlockableCraftingRecipe()
        {

        }

        public UnlockableCraftingRecipe(string WhichTab, Recipe recipe, bool HasUnlocked=false)
        {
            this.recipe = recipe;
            this.hasUnlocked = HasUnlocked;
            this.whichTab = WhichTab;
        }

        public void unlock()
        {
            this.hasUnlocked = true;
        }

    }
}
