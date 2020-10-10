/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ChroniclerCherry/stardew-valley-mods
**
*************************************************/

namespace ShopTileFramework.Data
{
    abstract class ItemStockModel
    {
        public string ItemType { get; set; }
        public bool IsRecipe { get; set; } = false;
        public int StockPrice { get; set; } = -1;
        public string StockItemCurrency { get; set; } = "Money";
        public int StockCurrencyStack { get; set; } = 1;
        public int Quality { get; set; } = 0;
        public int[] ItemIDs { get; set; } = null;
        public string[] JAPacks { get; set; } = null;
        public string[] ItemNames { get; set; } = null;
        public bool FilterSeedsBySeason { get; set; } = true;
        public int Stock { get; set; } = int.MaxValue;
        public int MaxNumItemsSoldInItemStock { get; set; } = int.MaxValue;
        public string[] When { get; set; } = null;
    }
}
