/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NoxChimaera/StardewValleyTODO
**
*************************************************/

using StardewValley.Menus;
using StardewValleyTodo.Game;
using StardewValleyTodo.Tracker;

namespace StardewValleyTodo.Controllers {
    public class JunimoBundleController {
        public void ProcessInput(JunimoNoteMenu menu, InventoryTracker inventoryTracker, JunimoBundles bundles) {
            // Not a bundle page
            if (menu.ingredientSlots.Count == 0) {
                return;
            }

            var currentPage = menu.currentPageBundle;
            if (currentPage.complete) {
                return;
            }

            var name = currentPage.label;
            if (inventoryTracker.Has(name)) {
                inventoryTracker.Off(name);
            }

            var bundle = bundles.Find(name);
            var todo = new TrackableJunimoBundle(bundle);
            inventoryTracker.Toggle(todo);
        }
    }
}
