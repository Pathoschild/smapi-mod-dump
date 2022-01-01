/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/alanperrow/StardewModding
**
*************************************************/

using StardewValley;

namespace ConvenientInventory.Compatibility
{
    public class InventoryExpansions
    {
        /// <summary> Checks if the player's max number of items differs from the length of favoriteItemSlots. </summary>
        public static bool IsPlayerMaxItemsChanged(bool[] favoriteItemSlots) => Game1.player.MaxItems != favoriteItemSlots?.Length;

        /// <summary> Expands/trims favoriteItemSlots to have a length of newSize. </summary>
        public static bool[] ResizeFavoriteItemSlots(bool[] favoriteItemSlots, int newSize)
        {
            if (favoriteItemSlots?.Length < newSize)
            {
                // Expand
                bool[] favoriteItemSlotsExpanded = new bool[newSize];
                favoriteItemSlots.CopyTo(favoriteItemSlotsExpanded, 0);

                return favoriteItemSlotsExpanded;
            }

            if (favoriteItemSlots?.Length > newSize)
            {
                // Trim
                bool[] favoriteItemSlotsTrimmed = favoriteItemSlots[0..newSize];

                return favoriteItemSlotsTrimmed;
            }

            return favoriteItemSlots;
        }
    }
}
