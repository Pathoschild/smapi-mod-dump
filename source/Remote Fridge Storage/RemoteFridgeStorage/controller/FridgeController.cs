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
using StardewValley;
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
            var nearbyChests = _chestController.GetChests().ToList();
            if (!nearbyChests.Any())
                return;

            // Add them as material containers to current CraftingPage
            var prop = page.GetType().GetField("_materialContainers", BindingFlags.NonPublic | BindingFlags.Instance);
            if (prop == null)
            {
                ModEntry.Instance.Log($"CraftFromChests failed: {page.GetType()}._materialContainers not found.");
                return;
            }

            var original = prop.GetValue(page) as List<Chest>;
            var modified = new List<Chest>();
            if (original?.Count > 0)
                modified.AddRange(original);
            modified.AddRange(nearbyChests);

            prop.SetValue(page, modified.Distinct().ToList());
        }
    }
}