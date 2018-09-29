using System;
using System.Collections.Generic;
using System.Linq;
using Igorious.StardewValley.DynamicApi2.Constants;
using Igorious.StardewValley.DynamicApi2.Extensions;
using Igorious.StardewValley.DynamicApi2.Utils;
using StardewValley;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace Igorious.StardewValley.ShowcaseMod.Core
{
    public class ItemFilter
    {
        public const string ShippableCategory = "Shippable";

        private ICollection<Func<Item, bool>> OrFilters { get; } = new List<Func<Item, bool>>();
        private ICollection<Func<Item, bool>> NotFilters { get; } = new List<Func<Item, bool>>();
        
        public ItemFilter(string filterString)
        {
            if (string.IsNullOrWhiteSpace(filterString)) return;

            var parts = filterString.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
            {
                if (part.StartsWith("!"))
                {
                    var filter = GetFilter(part.Substring(1));
                    if (filter != null) NotFilters.Add(filter);
                }
                else
                {
                    var filter = GetFilter(part);
                    if (filter != null) OrFilters.Add(filter);
                }
            }
        }

        public bool IsPass(Item item)
        {
            if ((item is Object o) && (o.bigCraftable || o is Furniture furniture && !IsSmallFurniture(furniture))) return false;
            return (!OrFilters.Any() || OrFilters.Any(f => f(item))) 
                && NotFilters.All(f => !f(item));
        }

        private static bool IsSmallFurniture(Furniture furniture)
        {
            return furniture.getTilesHigh() == 1 && furniture.getTilesWide() == 1;
        }

        private static Func<Item, bool> GetFilter(string filterName)
        {
            switch (filterName)
            {
                case ShippableCategory:
                    return Utility.highlightShippableObjects;
                default:
                    var category = filterName.TryToEnum<CategoryID>();
                    if (category == null)
                    {
                        Log.Error($@"Filter ""{filterName}"" is not recognized!");
                        return null;
                    }
                    return i => GetCategory(i) == category;
            }
        }

        private static CategoryID GetCategory(Item item)
        {
            return (item is Furniture)? CategoryID.Furniture : (CategoryID)item.category;
        }
    }
}