using System;
using System.Collections.Generic;
using System.Reflection;
using Harmony;
using StardewValley;
using TehPers.Core.Items.Managed;
using SObject = StardewValley.Object;

namespace TehPers.Core.Items.Delegators {
    internal static class ItemDelegator {
        private static bool _patched = false;

        public static void PatchIfNeeded() {
            if (ItemDelegator._patched) {
                return;
            }
            ItemDelegator._patched = true;

            // Create harmony instance
            HarmonyInstance harmony = HarmonyInstance.Create("TehPers.Core.Items.ItemDelegator");
            
            // SObject.DisplayName.Get()
            PropertyInfo targetProperty = typeof(SObject).GetProperty(nameof(SObject.DisplayName), BindingFlags.Public | BindingFlags.Instance);
            MethodInfo target = targetProperty?.GetMethod;
            MethodInfo replacement = typeof(ItemDelegator).GetMethod(nameof(ItemDelegator.ItemDisplayName_GetPrefix), BindingFlags.NonPublic | BindingFlags.Static);
            harmony.Patch(target, new HarmonyMethod(replacement));

            // SObject.getDescription()
            target = typeof(SObject).GetMethod(nameof(SObject.getDescription), BindingFlags.Public | BindingFlags.Instance);
            replacement = typeof(ItemDelegator).GetMethod(nameof(ItemDelegator.ItemGetDescriptionPrefix), BindingFlags.NonPublic | BindingFlags.Static);
            harmony.Patch(target, new HarmonyMethod(replacement));

            // Item.canStackwith(Item other)
            target = typeof(Item).GetMethod(nameof(Item.canStackWith), BindingFlags.Public | BindingFlags.Instance);
            replacement = typeof(ItemDelegator).GetMethod(nameof(ItemDelegator.ItemCanStackWithPrefix), BindingFlags.NonPublic | BindingFlags.Static);
            harmony.Patch(target, new HarmonyMethod(replacement));
        }

        private static bool ItemDisplayName_GetPrefix(Item __instance, ref string __result) {
            return !ItemDelegator.ExecuteIfManagedWithResult(__instance, ref __result, managedItem => managedItem.GetDisplayName());
        }

        private static bool ItemGetDescriptionPrefix(Item __instance, ref string __result) {
            return !ItemDelegator.ExecuteIfManagedWithResult(__instance, ref __result, managedItem => managedItem.GetDescription());
        }

        private static bool ItemCanStackWithPrefix(Item __instance, Item other, ref bool __result) {
            return ItemDelegator.ExecuteIfManagedWithResult(__instance, ref __result, thisManaged => {
                // Can't stack an unmanaged object with a managed object
                if (!ItemDelegator.TryGetManaged(other.ParentSheetIndex, out IApiManagedObject otherManaged)) {
                    return false;
                }

                // Can't stack unless both objects are stackable
                if (!(thisManaged is IStackableObject thisStackable) || !(otherManaged is IStackableObject otherStackable)) {
                    return false;
                }

                // Ask the item
                return thisStackable.CanStackWith(otherStackable);
            });
        }

        private static bool ExecuteIfManaged(Item item, ManagedItemAction action) {
            object _ = new object();
            return ItemDelegator.ExecuteIfManagedWithResult(item, ref _, managedItem => {
                action(managedItem);
                return default;
            });
        }

        private static bool ExecuteIfManagedWithResult<T>(Item item, ref T result, ManagedItemActionWithResult<T> action) {
            // Make sure this item has a parent sheet index that is being managed
            if (!ItemDelegator.TryGetManaged(item.ParentSheetIndex, out IApiManagedObject managedObject)) {
                return false;
            }

            result = action(managedObject);
            return true;
        }

        private static bool TryGetManaged(int index, out IApiManagedObject managedObject) {
            return ItemApi.IndexToItem.TryGetValue(index, out managedObject);
        }

        private delegate void ManagedItemAction(IApiManagedObject managedItem);
        private delegate T ManagedItemActionWithResult<out T>(IApiManagedObject managedItem);
    }
}
