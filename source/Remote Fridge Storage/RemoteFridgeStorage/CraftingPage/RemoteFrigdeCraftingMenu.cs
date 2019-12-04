using System.Collections.Generic;
using System.Linq;
using Harmony;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

namespace RemoteFridgeStorage.CraftingPage
{
    /// <summary>
    /// Create a craftubg page of the 
    /// </summary>
    public class RemoteFridgeCraftingPage : StardewValley.Menus.CraftingPage
    {

        public RemoteFridgeCraftingPage(StardewValley.Menus.CraftingPage page, FridgeHandler fridgeHandler) :
            base(page.xPositionOnScreen, page.yPositionOnScreen, page.width, page.height, true, true,
                MaterialContainers(page,fridgeHandler))
        {
            exitFunction = page.exitFunction;
            currentRegion = page.currentRegion;
            behaviorBeforeCleanup = page.behaviorBeforeCleanup;
        }

        private static List<Chest> MaterialContainers(StardewValley.Menus.CraftingPage handler, FridgeHandler fridgeHandler)
        {
            var materialContainers = ModEntry.Instance.Helper.Reflection.GetField<List<Chest>>(handler, "_materialContainers").GetValue();
            var chests = new List<Chest>();
            chests.AddRange(fridgeHandler.Chests);
            chests.AddRange(materialContainers);
            return chests.Distinct().ToList();
        }
    }
}