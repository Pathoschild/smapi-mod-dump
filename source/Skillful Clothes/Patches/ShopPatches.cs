/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

using SkillfulClothes.Types;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillfulClothes.Patches
{
    /// <summary>
    /// Changes shop behavior (e.g. items sold)
    /// </summary>
    class ShopPatches
    {
        static Dictionary<Shop, List<Shirt>> soldShirts;
        static Dictionary<Shop, List<Pants>> soldPants;
        static Dictionary<Shop, List<Hat>> soldHats;

        public static void Apply(IModHelper modHelper)
        {
            modHelper.Events.Display.MenuChanged += Display_MenuChanged;

            soldShirts = ItemDefinitions.ShirtEffects.Where(x => x.Value.SoldBy != Shop.None).GroupBy(x => x.Value.SoldBy).ToDictionary(x => x.Key, x => x.Select(v => v.Key).ToList());
            soldPants = ItemDefinitions.PantsEffects.Where(x => x.Value.SoldBy != Shop.None).GroupBy(x => x.Value.SoldBy).ToDictionary(x => x.Key, x => x.Select(v => v.Key).ToList());
            soldHats = ItemDefinitions.HatEffects.Where(x => x.Value.SoldBy != Shop.None).GroupBy(x => x.Value.SoldBy).ToDictionary(x => x.Key, x => x.Select(v => v.Key).ToList());
        }

        private static void Display_MenuChanged(object sender, StardewModdingAPI.Events.MenuChangedEventArgs e)
        {
            if (e.NewMenu is ShopMenu shopMenu)
            {
                Logger.Info($"Opened shop of {shopMenu.portraitPerson?.name}");

                var shop = shopMenu.GetShop();
                if (shop != Shop.None)
                {
                    EditShop(shopMenu, shop);
                }
            }
        }

        private static void EditShop(ShopMenu shopMenu, Shop shop)
        {
            // add the items of this shop

            if (soldShirts.TryGetValue(shop, out List<Shirt> shirts))
            {
                AddItems(shop, shopMenu, shirts);                
            }

            if (soldPants.TryGetValue(shop, out List<Pants> pants))
            {
                AddItems(shop, shopMenu, pants);
            }

            if (soldHats.TryGetValue(shop, out List<Hat> hats))
            {
                AddItems(shop, shopMenu, shirts);
            }

            // Todo: add tab buttons for clothing
            // see ShopMenu.setUpStoreForContext
        }

        private static void AddItems<T>(Shop shop, ShopMenu shopMenu, List<T> items)
        {
            foreach (var item in items)
            {
                if (ItemDefinitions.GetExtInfo(item, out ExtItemInfo extInfo) && extInfo.SellingCondition.IsFulfilled(shop)) 
                {
                    Item saleItem = CreateItemInstance(item);
                    shopMenu.forSale.Add(saleItem);
                    shopMenu.itemPriceAndStock.Add(saleItem, new int[] { extInfo.Price, 1 });
                }
            }
        }

        protected static Item CreateItemInstance<T>(T id)
        {
            int index = (int)(object)id;

            if (typeof(T) == typeof(Shirt) || typeof(T) == typeof(Pants))
            {                
                return new StardewValley.Objects.Clothing(index);
            }

            if (typeof(T) == typeof(Hat))
            {
                return new StardewValley.Objects.Hat(index);
            }

            return null;
        }
    }
}
