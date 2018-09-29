using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCrafting
{
    public class CategoryManager
    {
        public struct ItemCategory
        {
            public string id;

            public ItemCategory(string id)
            {
                this.id = id;
            }

            public override bool Equals(object obj)
            {
                if (obj is ItemCategory other)
                {
                    return this.id == other.id;
                }

                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return id.GetHashCode();
            }
        }

        private IMonitor monitor;

        private CategoryData data;

        private List<ItemCategory> itemCategories;

        public CategoryManager(IMonitor monitor, CategoryData data)
        {
            this.monitor = monitor;

            this.data = data;

            this.itemCategories = this.data.categories.Keys.Select(str => new ItemCategory(str)).ToList();

            if (!this.itemCategories.Contains(new ItemCategory("misc")))
            {
                if (!this.data.categoryNames.ContainsKey("misc"))
                {
                    this.data.categoryNames.Add("misc", "Misc");
                }
                
                this.itemCategories.Add(new ItemCategory("misc"));
            }
        }

        public ItemCategory GetDefaultItemCategory()
        {
            return new ItemCategory("misc");
        }

        public IEnumerable<ItemCategory> GetItemCategories()
        {
            return this.itemCategories;
        }

        public string GetItemCategoryName(ItemCategory category)
        {
            if (this.data.categoryNames.Keys.Contains(category.id))
            {
                return this.data.categoryNames[category.id];
            }
            else
            {
                this.monitor.Log("No name found for category '" + category.id + "'", LogLevel.Error);

                return "";
            }
        }

        public ItemCategory GetItemCategory(Item item)
        {
            foreach (var category in this.data.categories.Keys)
            {
                if (this.data.categories[category].Contains(item.Name))
                {
                    return new ItemCategory(category);
                }
            }

            return new ItemCategory("misc");
        }
    }
}
