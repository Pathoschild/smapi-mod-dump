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
using Omegasis.Revitalize.Framework.World.Objects.Items.Utilities;
using StardewValley;

namespace Omegasis.Revitalize.Framework.Crafting
{
    /// <summary>
    /// Returns results on crafting recipes on whether or not they were successful as well as information on the actual crafting recipe.
    /// </summary>
    public class CraftingResult
    {

        public bool successful;

        /// <summary>
        /// Used in the case that a <see cref="Recipe"/> is not used. This is just so that automate can keep track of the proper items consumed in a crafting operation.
        /// </summary>
        public List<ItemReference> consumedItems = new List<ItemReference>();

        public CraftingResult()
        {
            this.successful = false;
        }

        public CraftingResult(bool Success)
        {
            this.successful = Success;
        }

        public CraftingResult(ItemReference item, bool Success)
        {
            this.consumedItems.Add(item);
            this.successful = Success;
        }

        public CraftingResult(List<ItemReference> consumedItems, bool successful) : this(successful)
        {
            this.consumedItems.AddRange(consumedItems);
        }
    }
}
