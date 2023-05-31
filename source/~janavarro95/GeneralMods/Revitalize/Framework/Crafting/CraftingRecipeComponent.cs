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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace Omegasis.Revitalize.Framework.Crafting
{
    public class CraftingRecipeComponent
    {
        public Item item;
        protected int requiredAmount;
        protected int minAmount;
        protected int maxAmount;


        public CraftingRecipeComponent()
        {

        }

        public CraftingRecipeComponent(Item I, int RequiredAmount)
        {
            this.item = I;
            this.requiredAmount = RequiredAmount;
        }

        public CraftingRecipeComponent(Item I, int MinAmount, int MaxAmount)
        {
            this.item = I;
            this.minAmount = MinAmount;
            this.maxAmount = MaxAmount;
        }

        /// <summary>
        /// Returns the required amount for the crafting recipe component.
        /// </summary>
        /// <returns></returns>
        public virtual int getRequiredAmount()
        {
            if(this.minAmount!=0 && this.maxAmount != 0)
            {
                return Game1.random.Next(this.getMinStackSize(), this.getMaxStackSize() + 1);
            }
            else
            {
                return this.requiredAmount;
            }

        }

        public virtual int getMinStackSize()
        {
            if(this.minAmount==0) return this.requiredAmount;
            return this.minAmount;
        }

        public virtual int getMaxStackSize()
        {
            if (this.maxAmount == 0) return this.requiredAmount;
            return this.requiredAmount;
        }

    }
}
