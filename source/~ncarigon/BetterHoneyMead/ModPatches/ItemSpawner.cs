/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ncarigon/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using SObject = StardewValley.Object;

namespace BetterHoneyMead.ModPatches {
    internal static class ItemSpawner {
        public static void Register() {
            if (ModEntry.Instance?.Helper?.ModRegistry.IsLoaded("CJBok.ItemSpawner") ?? false) {
                ModEntry.ModHarmony?.Patch(
                    original: AccessTools.Method(typeof(StardewValley.ItemTypeDefinitions.ObjectDataDefinition), "CreateFlavoredHoney"),
                    postfix: new HarmonyMethod(typeof(ItemSpawner), nameof(Postfix_ObjectDataDefinition_CreateFlavoredHoney))
                );
                ModEntry.ModHarmony?.Patch(
                    original: AccessTools.Method("CJBItemSpawner.ModEntry:BuildMenu"),
                    prefix: new HarmonyMethod(typeof(ItemSpawner), nameof(Prefix_ModEntry_BuildMenu)),
                    postfix: new HarmonyMethod(typeof(ItemSpawner), nameof(Postfix_ModEntry_BuildMenu))
                );
                ModEntry.ModHarmony?.Patch(
                    original: AccessTools.Method("CJBItemSpawner.Framework.ItemMenu:ResetItemView"),
                    postfix: new HarmonyMethod(typeof(ItemSpawner), nameof(Postfix_ItemSpawner_ResetItemView))
                );
            }
        }

        private static bool IsLoading = false;
        private static readonly List<Item> CachedItems = new();

        private static void Prefix_ModEntry_BuildMenu() {
            IsLoading = true;
            CachedItems.Clear();
        }

        private static void Postfix_ModEntry_BuildMenu() {
            IsLoading = false;
        }

        private static void Postfix_ObjectDataDefinition_CreateFlavoredHoney(
            SObject ingredient
        ) {
            if (IsLoading) {
                var item = CreateFlavoredMead(ingredient);
                if (item is not null) {
                    CachedItems.Add(item);
                }
            }
        }

        private static void Postfix_ItemSpawner_ResetItemView(
            IList<Item> ___ItemsInView, TextBox ___SearchBox
        ) {
            var search = ___SearchBox?.Text?.Trim();
            foreach (var item in CachedItems) {
                if (string.IsNullOrWhiteSpace(search)
                    || (item.Name?.Contains(search, StringComparison.InvariantCultureIgnoreCase) ?? false)
                    || (item.DisplayName?.Contains(search, StringComparison.InvariantCultureIgnoreCase) ?? false)
                ) {
                    ___ItemsInView.Add(item);
                }
            } 
        }

        private static SObject? CreateFlavoredMead(SObject ingredient) {
            var color = TailoringMenu.GetDyeColor(ingredient) ?? Color.Gold;
            var item = new ColoredObject("459", 999, color) {
                displayNameFormat = "%PRESERVED_DISPLAY_NAME %DISPLAY_NAME",
            };
            item.preservedParentSheetIndex.Value = ingredient.ItemId;
            var price = ((Game1.objectData.FirstOrDefault(o => o.Key.Equals("340")).Value?.Price ?? 0) + (ingredient is null ? 0 : ingredient.Price * 2)) * 2;
            if (price > 0) {
                item.Price = price;
            }
            if (ingredient?.Name is not null) {
                item.Name = $"{item.QualifiedItemId}_{ingredient.Name} Honey";
            }
            return item;
        }
    }
}
