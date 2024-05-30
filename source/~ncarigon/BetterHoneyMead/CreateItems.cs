/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ncarigon/StardewValleyMods
**
*************************************************/

using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley;
using SObject = StardewValley.Object;
using Microsoft.Xna.Framework;

namespace BetterHoneyMead {
    internal static class CreateItems {
        public static SObject CreateFlavoredHoney(SObject? ingredient, int stack = 1, int quality = 0) {
            var color = TailoringMenu.GetDyeColor(ingredient) ?? Color.Yellow;
            return new ColoredObject("340", stack, color) {
                Quality = quality
            };
        }

        public static SObject CreateFlavoredMead(SObject? ingredient, int stack = 1, int quality = 0) {
            var color = TailoringMenu.GetDyeColor(ingredient) ?? Color.Gold;
            var item = new ColoredObject("459", stack, color) {
                Quality = quality
            };
            if (ingredient?.ItemId is not null) {
                item.displayNameFormat = "%PRESERVED_DISPLAY_NAME %DISPLAY_NAME";
                item.preservedParentSheetIndex.Value = ingredient.ItemId;
                var price = ((Game1.objectData.FirstOrDefault(o => o.Key.Equals("340")).Value?.Price ?? 0) + (ingredient.Price * 2)) * 2;
                if (price > 0) {
                    item.Price = price;
                }
                if (ingredient?.Name is not null) {
                    item.Name = $"{item.QualifiedItemId}_{ingredient.Name}";
                }
            }
            return item;
        }
    }
}
