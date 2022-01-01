/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stardew-access/stardew-access
**
*************************************************/


using StardewModdingAPI;
using StardewValley;

namespace stardew_access.Game
{
    internal class SlotAndLocation
    {
        private static Item? currentSlotItem;
        private static Item? previousSlotItem;

        private static GameLocation? currentLocation;
        private static GameLocation? previousLocation;

        // Narrates current slected slot name
        public static void narrateCurrentSlot()
        {
            currentSlotItem = Game1.player.CurrentItem;

            if (currentSlotItem == null)
                return;

            if (previousSlotItem == currentSlotItem)
                return;

            previousSlotItem = currentSlotItem;
            ScreenReader.say($"{currentSlotItem.DisplayName} Selected", true);
        } 

        // Narrates current location's name
        public static void narrateCurrentLocation()
        {
            currentLocation = Game1.currentLocation;

            if (currentLocation == null)
                return;

            if (previousLocation == currentLocation)
                return;

            previousLocation = currentLocation;
            ScreenReader.say($"{currentLocation.NameOrUniqueName} Entered",true);
        }
    }
}
