/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stardew-access/stardew-access
**
*************************************************/

using StardewValley;
using System.Text.RegularExpressions;

namespace stardew_access.Features
{
    internal class Other
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
            MainClass.GetScreenReader().Say($"{currentSlotItem.DisplayName} Selected", true);
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
            MainClass.GetScreenReader().Say($"{currentLocation.Name} Entered", true);
        }

        public static void SnapMouseToPlayer()
        {
            int x = Game1.player.GetBoundingBox().Center.X - Game1.viewport.X;
            int y = Game1.player.GetBoundingBox().Center.Y - Game1.viewport.Y;

            int offset = 64;

            switch (Game1.player.FacingDirection)
            {
                case 0:
                    y -= offset;
                    break;
                case 1:
                    x += offset;
                    break;
                case 2:
                    y += offset;
                    break;
                case 3:
                    x -= offset;
                    break;
            }

            Game1.setMousePosition(x, y);
        }

        public static async void narrateHudMessages()
        {
            MainClass.isNarratingHudMessage = true;
            try
            {
                if (Game1.hudMessages.Count > 0)
                {
                    int lastIndex = Game1.hudMessages.Count - 1;
                    HUDMessage lastMessage = Game1.hudMessages[lastIndex];
                    if (!lastMessage.noIcon)
                    {
                        string toSpeak = lastMessage.Message;
                        string searchQuery = toSpeak;

                        searchQuery = Regex.Replace(toSpeak, @"[\d+]", string.Empty);
                        searchQuery.Trim();


                        if (MainClass.hudMessageQueryKey != searchQuery)
                        {
                            MainClass.hudMessageQueryKey = searchQuery;

                            MainClass.GetScreenReader().Say(toSpeak, true);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MainClass.GetMonitor().Log($"Unable to narrate hud messages:\n{e.Message}\n{e.StackTrace}", StardewModdingAPI.LogLevel.Error);
            }

            await Task.Delay(300);
            MainClass.isNarratingHudMessage = false;
        }
    }
}
