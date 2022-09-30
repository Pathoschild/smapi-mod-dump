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
using StardewValley;

namespace Omegasis.Revitalize.Framework.Crafting
{
    public class CraftingRecipeComponent
    {
        public Item item;
        public int requiredAmount;

        public CraftingRecipeComponent()
        {

        }

        public CraftingRecipeComponent(Item I, int RequiredAmount)
        {
            this.item = I;
            this.requiredAmount = RequiredAmount;
        }
    }
}
