/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/SoapStuff/Remote-Fridge-Storage
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

namespace RemoteFridgeStorage.controller
{
    /// <summary>
    /// Handles the items used by the fridge from the chest controller
    /// </summary>
    public class FridgeController
    {
        private readonly ChestController _chestController;

        public FridgeController(ChestController chestController)
        {
            _chestController = chestController;
        }

        //Thanks to aEnigmatic
        public void InjectItems()
        {
            var page = Game1.activeClickableMenu;

            if (page == null)
                return;

            // Find nearby chests
            var nearbyChests = _chestController.GetChests().Select((chest, i) => chest.Items).ToList();
            if (!nearbyChests.Any())
                return;

            if (page is CraftingPage craftingPage)
            {
                craftingPage._materialContainers.AddRange(nearbyChests);
            }
            else
            {
                ModEntry.Instance.Monitor.Log($"Failed to inject items into: {page.GetType()}. Is it from an incompatible mod?",
                    LogLevel.Warn);
            }
        }
    }
}