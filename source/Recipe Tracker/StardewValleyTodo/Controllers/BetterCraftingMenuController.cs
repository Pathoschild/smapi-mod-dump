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
    public class BetterCraftingMenuController {
        private readonly InventoryTracker _inventoryTracker;

        public BetterCraftingMenuController(InventoryTracker inventoryTracker) {
            _inventoryTracker = inventoryTracker;
        }

        public void ProcessInput(IClickableMenu page) {
            var hoverRecipe = Reflect.GetPrivate<dynamic>(page, "hoverRecipe");
            if (hoverRecipe == null) {
                return;
            }

            var recipeName = (string) hoverRecipe.DisplayName;
            if (_inventoryTracker.Has(recipeName)) {
                _inventoryTracker.Off(recipeName);

                return;
            }

            var recipeProp = hoverRecipe.Recipe;
            var rawComponents = (Dictionary<string, int>) recipeProp.recipeList;
            var components = new List<TrackableItemBase>(rawComponents.Count);

            foreach (var kv in rawComponents) {
                var key = ObjectKey.Parse(kv.Key);
                var count = kv.Value;

                if (key.Contains("-")) {
                    var name = (string) recipeProp.getNameFromIndex(key);

                    components.Add(new CountableItemCategory(key, name, count));
                } else {
                    var info = Game1.objectData[key];
                    var name = LocalizedStringLoader.Load(info.DisplayName);

                    components.Add(new CountableItem(key, name, count));
                }
            }

            var todoRecipe = new TrackableRecipe(recipeName, components);
            _inventoryTracker.Toggle(todoRecipe);
        }
    }
}
