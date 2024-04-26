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
using System;
using System.Collections.Generic;
using SObject = StardewValley.Object;

namespace CopperStill.ModPatches {
    internal static class ItemSpawner {
        public static void Register() {
            if (ModEntry.Instance?.Helper?.ModRegistry.IsLoaded("CJBok.ItemSpawner") ?? false) {
                ModEntry.ModHarmony?.Patch(
                    original: AccessTools.Method(typeof(StardewValley.ItemTypeDefinitions.ObjectDataDefinition), "CreateFlavoredWine"),
                    postfix: new HarmonyMethod(typeof(ItemSpawner), nameof(Postfix_ObjectDataDefinition_CreateFlavoredWine))
                );
                ModEntry.ModHarmony?.Patch(
                    original: AccessTools.Method(typeof(StardewValley.ItemTypeDefinitions.ObjectDataDefinition), "CreateFlavoredJuice"),
                    postfix: new HarmonyMethod(typeof(ItemSpawner), nameof(Postfix_ObjectDataDefinition_CreateFlavoredJuice))
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

        private static void Postfix_ObjectDataDefinition_CreateFlavoredWine(
            SObject ingredient
        ) {
            if (IsLoading) {
                var item = CreateFlavoredType(ingredient, "Brandy");
                if (item is not null) {
                    CachedItems.Add(item);
                }
            }
        }

        private static void Postfix_ObjectDataDefinition_CreateFlavoredJuice(
            SObject ingredient
        ) {
            if (IsLoading) {
                var item = CreateFlavoredType(ingredient, "Vodka");
                if (item is not null) {
                    CachedItems.Add(item);
                    item = CreateFlavoredType(ingredient, "Gin");
                    if (item is not null) {
                        CachedItems.Add(item);
                    }
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

        private static SObject? CreateFlavoredType(SObject ingredient, string type) {
            var color = TailoringMenu.GetDyeColor(ingredient) ?? Color.White;
            var item = new ColoredObject($"NCarigon.CopperStillCP_{type}", 999, color);
            try {
                item.displayNameFormat = "%PRESERVED_DISPLAY_NAME %DISPLAY_NAME";
                item.preservedParentSheetIndex.Value = ingredient.ItemId;
                var price = AdjustPricing.CalcPrice(item.Name, ingredient.Name);
                if (price > 0) {
                    item.Price = price;
                }
                item.Name += $"_{ingredient.Name}";
            } catch { }
            return item;
        }
    }
}
