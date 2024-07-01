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
        private readonly InventoryTracker _inventoryTracker;
        private readonly JunimoBundles _junimoBundles;

        public JunimoBundleController(InventoryTracker inventoryTracker, JunimoBundles junimoBundles) {
            _inventoryTracker = inventoryTracker;
            _junimoBundles = junimoBundles;
        }

        public void ProcessInput(JunimoNoteMenu menu) {
            // Not a bundle page
            if (menu.ingredientSlots.Count == 0) {
                return;
            }

            var currentPage = menu.currentPageBundle;
            if (currentPage.complete) {
                return;
            }

            var name = currentPage.label;
            if (_inventoryTracker.Has(name)) {
                _inventoryTracker.Off(name);

                return;
            }

            var bundle = _junimoBundles.Find(name);
            var todo = new TrackableJunimoBundle(bundle);
            _inventoryTracker.Toggle(todo);
        }
    }
}
