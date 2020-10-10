/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ChroniclerCherry/stardew-valley-mods
**
*************************************************/

using ShopTileFramework.Shop;

namespace ShopTileFramework.Data
{
    class ContentPack
    {
        public string[] RemovePacksFromVanilla { get; set; }
        public string[] RemovePackRecipesFromVanilla { get; set; }

        public string[] RemoveItemsFromVanilla { get; set; }
        public ItemShop[] Shops { get; set; }
        public AnimalShop[] AnimalShops { get; set; }

        public VanillaShop[] VanillaShops { get; set; }
    }
}
