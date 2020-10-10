/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Stardew-Valley-Modding/Bookcase
**
*************************************************/

using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookcase.Events {
    /// <summary>
    /// Code run everytime a bundle is 'setup', essentially allowing you the ability to inject information into an already created bundle's display.
    /// </summary>
    public class PostBundleSetupEvent : Event {
        public List<ClickableTextureComponent> ingredientList;
        public List<ClickableTextureComponent> ingredientSlots;
        public InventoryMenu inventory;
        /// <summary>
        /// No point trying to change this, it's local scope to the method and is never referenced again - it is merely a convenience.
        /// </summary>
        public readonly Bundle currentBundle;

        public PostBundleSetupEvent(List<ClickableTextureComponent> ingredientList, List<ClickableTextureComponent> ingredientSlots, InventoryMenu inventory, Bundle currentBundle) {
            this.ingredientList = ingredientList;
            this.ingredientSlots = ingredientSlots;
            this.inventory = inventory;
            this.currentBundle = currentBundle;
        }
    }
}
