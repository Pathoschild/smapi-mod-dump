using System;
using StardewValley;
using Object = StardewValley.Object;

namespace Igorious.StardewValley.DynamicAPI.Helpers
{
    public static class InventoryHelper
    {
        public static void RemoveItem(int itemID, int count, Farmer farmer, bool isBig = false)
        {
            var items = farmer.Items;
            var remainedCount = count;
            for (var i = items.Count - 1; i >= 0; --i)
            {
                var item = items[i] as Object;
                if (item?.parentSheetIndex != itemID || item.bigCraftable != isBig) continue;

                var delta = Math.Min(item.Stack, remainedCount);
                item.Stack -= delta;
                remainedCount -= delta;

                if (item.Stack == 0) items[i] = null;
                if (remainedCount == 0) break;
            }
        }
    }
}
