/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NoxChimaera/StardewValleyTODO
**
*************************************************/

using System.Collections.Generic;
using StardewValley;
using StardewValley.Menus;
using StardewValleyTodo.Helpers;
using StardewValleyTodo.Tracker;

namespace StardewValleyTodo.Controllers {
    public class CarpenterMenuController {
        private readonly InventoryTracker _inventoryTracker;

        public CarpenterMenuController(InventoryTracker inventoryTracker) {
            _inventoryTracker = inventoryTracker;
        }

        public void ProcessInput(CarpenterMenu menu) {
            var blueprint = menu.Blueprint;
            var name = blueprint.DisplayName;
            var cost = blueprint.BuildCost;
            var displayName = $"{name} ({cost} g.)";

            if (_inventoryTracker.Has(displayName)) {
                _inventoryTracker.Off(displayName);

                return;
            }

            var materials = blueprint.BuildMaterials;
            var components = new List<CountableItem>(materials.Count);

            foreach (var material in materials) {
                var id = ObjectKey.Parse(material.Id);
                var info = Game1.objectData[id];
                var materialName = LocalizedStringLoader.Load(info.DisplayName);

                components.Add(new CountableItem(id, materialName, material.Amount));
            }

            var recipe = new TrackableRecipe(displayName, components);
            _inventoryTracker.Toggle(recipe);
        }
    }
}
