/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/idermailer/DresserSorter
**
*************************************************/

namespace DresserSorter
{
    public sealed class ModConfig
    {
        public bool VeryTidySort_ByStats { get; set; } = false;
        public bool SortAdditionalCategories { get; set; } = false;
        public bool Cloth_ColorSortByHue { get; set; } = true;
        public bool Ring_CombinedRingIsPrior { get; set; } = true;
        public bool Object_NotSortResourceByDisplayName { get; set; } = true;
        public bool Furniture_NotCompareFurnitureType { get; set; } = true;
    }
}