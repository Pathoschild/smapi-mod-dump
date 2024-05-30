/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using Newtonsoft.Json;
using StardewValley;
using DeluxeJournal.Util;

namespace DeluxeJournal.Task.Tasks
{
    internal abstract class ItemTaskBase : TaskBase
    {
        /// <summary>The qualified item IDs, or item categories, of the items to be checked.</summary>
        public IList<string> ItemIds { get; set; }

        /// <summary>Minimum required item quality.</summary>
        public int Quality { get; set; } = 0;

        /// <summary>
        /// The qualified base item IDs, or item categories, of the items to be checked.
        /// Stripped of any encoded flavor ID information.
        /// </summary>
        [JsonIgnore]
        protected List<string> BaseItemIds { get; set; } = new List<string>();

        /// <summary>The preserve item ID parent, if applicable.</summary>
        [JsonIgnore]
        protected string? IngredientItemId { get; set; }

        /// <summary>Serialization constructor.</summary>
        public ItemTaskBase(string id) : base(id)
        {
            ItemIds = Array.Empty<string>();
        }

        public ItemTaskBase(string id, string name, IList<string> itemIds, int count, int quality = 0) : base(id, name)
        {
            ItemIds = itemIds;
            MaxCount = count;
            Quality = quality;
            Validate();
        }

        public override void Validate()
        {
            IngredientItemId = FlavoredItemHelper.ConvertFlavoredList(ItemIds, out var baseItemIds, false);
            BaseItemIds.Clear();
            BaseItemIds.AddRange(baseItemIds);
        }

        /// <summary>Check if the item matches any of the <see cref="ItemIds"/>.</summary>
        /// <param name="item">The item to check.</param>
        /// <returns>Whether the item matches the item QID or category (if the <see cref="ItemIds"/> entry is a category)</returns>
        protected bool CheckItemMatch(Item item)
        {
            if (item.Quality < Quality)
            {
                return false;
            }
            else if (BaseItemIds.Contains(item.QualifiedItemId))
            {
                return string.IsNullOrEmpty(IngredientItemId) || (item is SObject obj && IngredientItemId == obj.preservedParentSheetIndex.Value);
            }
            else if (BaseItemIds.FirstOrDefault() is string category && category.StartsWith('-'))
            {
                return category == item.Category.ToString();
            }
            else
            {
                return false;
            }
        }
    }
}
