/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/thimadera/StardewMods
**
*************************************************/

using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;

namespace StackEverythingRedux.Patches
{
    internal class RemoveQueuedFurniturePatches
    {
        public static bool Prefix(DecoratableLocation __instance, Guid guid)
        {
            RemoveQueuedFurniture(__instance, guid);
            return false;
        }

        private static void RemoveQueuedFurniture(DecoratableLocation instance, Guid guid)
        {
            Farmer who = Game1.player;
            if (!instance.furniture.TryGetValue(guid, out Furniture furnitureItem) || !who.couldInventoryAcceptThisItem(furnitureItem))
            {
                return;
            }
            furnitureItem.performRemoveAction();
            instance.furniture.Remove(guid);
            bool foundInToolbar = false;
            for (int j = 0; j < 12; j++)
            {
                if (who.Items[j] != null && who.Items[j].QualifiedItemId == furnitureItem.QualifiedItemId)
                {
                    who.Items[j].Stack++;
                    who.CurrentToolIndex = j;
                    foundInToolbar = true;
                    break;
                }
                else if (who.Items[j] == null)
                {
                    who.Items[j] = furnitureItem;
                    who.CurrentToolIndex = j;
                    foundInToolbar = true;
                    break;
                }
            }
            if (!foundInToolbar)
            {
                Item item = who.addItemToInventory(furnitureItem, 11);
                _ = who.addItemToInventory(item);
                who.CurrentToolIndex = 11;
            }
            instance.localSound("coin");
        }
    }
}
