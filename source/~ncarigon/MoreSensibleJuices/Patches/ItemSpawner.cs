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
using StardewValley.ItemTypeDefinitions;
using System.Collections;
using SObject = StardewValley.Object;

namespace MoreSensibleJuices.Patches {
    internal static class ItemSpawner {
        public static void Register() {
            if (ModEntry.Instance?.Helper?.ModRegistry.IsLoaded("CJBok.ItemSpawner") ?? false) {
                ModEntry.Instance?.ModHarmony?.Patch(
                    original: AccessTools.Method("CJBItemSpawner.Framework.ItemData.ItemRepository:GetFlavoredObjectVariants"),
                    postfix: new HarmonyMethod(typeof(ItemSpawner), nameof(Postfix_ItemRepository_GetFlavoredObjectVariants))
                );
            }
        }

        private static IEnumerable<object> Postfix_ItemRepository_GetFlavoredObjectVariants(
            IEnumerable<object> values,
            object __instance,
            ObjectDataDefinition objectDataDefinition, SObject? item, IItemDataDefinition itemType

        ) {
            IList? results = null;
            if (item is not null && item.Category == -79) { //item makes wine
                var tryCreate = ModEntry.Instance?.Helper.Reflection.GetMethod(__instance, "TryCreate", false);
                if (tryCreate is not null) {
                    var newItem = tryCreate.Invoke<object>(new object[] { //also makes juice
                        itemType.Identifier,
                        $"350/{item.ItemId}",
                        (object p) => objectDataDefinition.CreateFlavoredJuice(item)
                    });
                    if (results is null) {
                        var listType = typeof(List<>).MakeGenericType(newItem.GetType());
                        if (listType is not null) {
                            results = Activator.CreateInstance(listType) as IList;
                            if (results is not null) {
                                foreach (var value in values) {
                                    results.Add(value);
                                }
                            }
                        }
                    }
                    results?.Add(newItem);
                }
            }
            return results as IEnumerable<object> ?? values;
        }
    }
}