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
            var honey = CreateItems.CreateFlavoredHoney(null, 999);
            CachedItems.Add(honey); // non-flavored, non-wild honey
        }

        private static void Postfix_ModEntry_BuildMenu() {
            IsLoading = false;
        }

        private static void Postfix_ObjectDataDefinition_CreateFlavoredHoney(
            ref SObject __result,
            SObject ingredient
        ) {
            if (IsLoading) {
                var mead = CreateItems.CreateFlavoredMead(ingredient, __result.Stack, __result.Quality);
                CachedItems.Add(mead);
            }
        }

        private static void Postfix_ItemSpawner_ResetItemView(
            IList<Item> ___ItemsInView, TextBox ___SearchBox
        ) {
            var search = ___SearchBox?.Text?.Trim();
            var mead = ___ItemsInView.Where(i => i?.Name?.Equals("Mead") ?? false).FirstOrDefault();
            if (mead is not null) {
                ___ItemsInView.Remove(mead);
            }
            foreach (var item in CachedItems) {
                if (string.IsNullOrWhiteSpace(search)
                    || (item.Name?.Contains(search, StringComparison.InvariantCultureIgnoreCase) ?? false)
                    || (item.DisplayName?.Contains(search, StringComparison.InvariantCultureIgnoreCase) ?? false)
                ) {
                    ___ItemsInView.Add(item);
                }
            } 
        }
    }
}

