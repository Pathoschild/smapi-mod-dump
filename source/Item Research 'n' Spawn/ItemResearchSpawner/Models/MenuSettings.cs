/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TSlex/StardewValley
**
*************************************************/

using ItemResearchSpawner.Models.Enums;

namespace ItemResearchSpawner.Models
{
    internal class MenuSettings
    {
        public ItemQuality Quality { get; set; }
        public ItemSortOption SortOption { get; set; }
        public string SearchText { get; set; }
        public string Category { get; set; }

        public MenuSettings()
        {
            Quality = ItemQuality.Normal;
            SortOption = ItemSortOption.Category;
            SearchText = "";
        }
    }
}