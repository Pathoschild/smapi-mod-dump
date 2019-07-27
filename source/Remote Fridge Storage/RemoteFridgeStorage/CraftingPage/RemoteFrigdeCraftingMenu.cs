using System.Collections.Generic;
using StardewValley;
using StardewValley.Menus;

namespace RemoteFridgeStorage.CraftingPage
{
    /// <summary>
    /// Overwrite the fridge().
    /// </summary>
    public class RemoteFridgeCraftingPage : StardewValley.Menus.CraftingPage
    {
        private readonly FridgeHandler _fridgeHandler;

        public RemoteFridgeCraftingPage(IClickableMenu page, FridgeHandler fridgeHandler) :
            base(page.xPositionOnScreen, page.yPositionOnScreen, page.width, page.height, true)
        {
            _fridgeHandler = fridgeHandler;
        }

        protected override IList<Item> fridge()
        {
            return _fridgeHandler.FridgeList;
        }
    }
}