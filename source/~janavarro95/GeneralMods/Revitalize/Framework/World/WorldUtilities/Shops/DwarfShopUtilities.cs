/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omegasis.Revitalize.Framework.Configs.ShopConfigs;
using Omegasis.Revitalize.Framework.Constants;
using Omegasis.Revitalize.Framework.Constants.Ids.Objects;
using Omegasis.Revitalize.Framework.Player;
using Omegasis.Revitalize.Framework.World.Objects.Items.Utilities;
using StardewValley;
using StardewValley.Menus;

namespace Omegasis.Revitalize.Framework.World.WorldUtilities.Shops
{
    public static class DwarfShopUtilities
    {

        public static int NormalGeodesRemainingToday;
        public static int FrozenGeodesRemainingToday;
        public static int MagmaGeodesRemainingToday;
        public static int OmniGeodesRemainingToday;

        /// <summary>
        /// The number of coal bushes for sale today.
        /// </summary>
        public static int CoalBushesForSaleToday;
        /// <summary>
        /// The number of copper bushes for sale today.
        /// </summary>
        public static int CopperBushesForSaleToday;
        /// <summary>
        /// The number of iron bushes for sale today.
        /// </summary>
        public static int IronBushesForSaleToday;
        /// <summary>
        /// The number of gold bushes for sale today.
        /// </summary>
        public static int GoldBushesForSaleToday;
        /// <summary>
        /// The number of iridium bushes for sale today.
        /// </summary>
        public static int IridiumBushesForSaleToday;
        /// <summary>
        /// The number of radioactive mushes for sale today.
        /// </summary>
        public static int RadioactiveBushesForSaleToday;


        public static Func<ISalable, Farmer, int, bool> DwarfShop_DefaultOnPurchaseMethod;

        /// <summary>
        /// Determine's this shop's stock at the beginning of the day.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public static void OnNewDay(object sender, StardewModdingAPI.Events.DayStartedEventArgs args)
        {
            DwarfShopConfig shopConfig = RevitalizeModCore.Configs.shopsConfigManager.dwarfShopConfig;

            NormalGeodesRemainingToday = shopConfig.NumberOfNormalGeodesToSell;
            if (Game1.player.deepestMineLevel >= 40)
            {
                FrozenGeodesRemainingToday = shopConfig.NumberOfFrozenGeodesToSell;
            }
            if (Game1.player.deepestMineLevel >= 80)
            {
                MagmaGeodesRemainingToday = shopConfig.NumberOfMagmaGeodesToSell;
            }
            if (Game1.player.hasSkullKey && (Game1.dayOfMonth % 7 == 0 || shopConfig.SellOmniGeodesEveryDayInsteadOnJustSundays))
            {
                //Add 1 omni geode on sundays.
                OmniGeodesRemainingToday = shopConfig.NumberOfOmniGeodesToSell;
            }
            else
            {
                OmniGeodesRemainingToday = 0;
            }


            if (Game1.player.MiningLevel >= 10 && Game1.player.hasSkullKey)
            {

                double coalChance = (double)Game1.random.Next(101) / 100;
                if (coalChance <= shopConfig.CoalResourceBushSellChance)
                {
                    CoalBushesForSaleToday = 1;
                }

                double copperChance = (double)Game1.random.Next(101) / 100;
                if (copperChance <= shopConfig.CoalResourceBushSellChance)
                {
                    CopperBushesForSaleToday = 1;
                }

                double ironChance = (double)Game1.random.Next(101) / 100;
                if (ironChance <= shopConfig.IronResourceBushSellChance)
                {
                    IronBushesForSaleToday = 1;
                }


                double goldChance = (double)Game1.random.Next(101) / 100;
                if (goldChance <= shopConfig.GoldResourceBushSellChance)
                {
                    GoldBushesForSaleToday = 1;
                }

                double iridiumChance = (double)Game1.random.Next(101) / 100;
                if (iridiumChance <= shopConfig.IrridiumResourceBushSellChance)
                {
                    IridiumBushesForSaleToday = 1;
                }


                //Only sell the radioactive bush if the player has a chance of getting the 'Danger in the Deep' quest.
                if (PlayerUtilities.GetNumberOfGoldenWalnutsFound() >= 100)
                {
                    double radioactiveChance = (double)Game1.random.Next(101) / 100;
                    if (radioactiveChance <= shopConfig.RadioactiveResourceBushSellChance)
                    {
                        RadioactiveBushesForSaleToday = 1;
                    }
                }

            }
        }

        /// <summary>
        /// Adds items to be sold by the dwarf.
        /// </summary>
        /// <param name="Menu"></param>
        public static void AddStockToDwarfShop(ShopMenu Menu)
        {
            DwarfShopConfig shopConfig = RevitalizeModCore.Configs.shopsConfigManager.dwarfShopConfig;

            DwarfShop_DefaultOnPurchaseMethod = Menu.onPurchase;
            Menu.onPurchase = OnPurchaseFromDwarfShop;

            if (NormalGeodesRemainingToday > 0)
            {
                ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(Enums.SDVObject.Geode, NormalGeodesRemainingToday), shopConfig.NormalGeodePrice, NormalGeodesRemainingToday);
            }
            if (FrozenGeodesRemainingToday > 0)
            {
                ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(Enums.SDVObject.FrozenGeode, FrozenGeodesRemainingToday), shopConfig.FrozenGeodePrice, FrozenGeodesRemainingToday);
            }
            if (MagmaGeodesRemainingToday > 0)
            {
                ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(Enums.SDVObject.MagmaGeode, MagmaGeodesRemainingToday), shopConfig.MagmaGeodePrice, MagmaGeodesRemainingToday);
            }
            if (OmniGeodesRemainingToday > 0)
            {
                ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(Enums.SDVObject.OmniGeode, OmniGeodesRemainingToday), shopConfig.OmniGeodePrice, OmniGeodesRemainingToday);
            }

            if (CoalBushesForSaleToday > 0)
            {
                ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(ResourceObjectIds.CoalBush), shopConfig.CoalResourceBushSellPrice, CoalBushesForSaleToday);
            }
            if (CopperBushesForSaleToday > 0)
            {
                ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(ResourceObjectIds.CopperOreBush), shopConfig.CopperResourceBushSellPrice, CopperBushesForSaleToday);
            }
            if (IronBushesForSaleToday > 0)
            {
                ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(ResourceObjectIds.IronOreBush), shopConfig.IronResourceBushSellPrice, IronBushesForSaleToday);
            }
            if (GoldBushesForSaleToday > 0)
            {
                ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(ResourceObjectIds.GoldOreBush), shopConfig.GoldResourceBushSellPrice, GoldBushesForSaleToday);
            }
            if (IridiumBushesForSaleToday > 0)
            {
                ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(ResourceObjectIds.IridiumOreBush), shopConfig.IridiumResourceBushSellPrice, IridiumBushesForSaleToday);
            }
            if (RadioactiveBushesForSaleToday > 0)
            {
                ShopUtilities.AddItemToShop(Menu, RevitalizeModCore.ModContentManager.objectManager.getItem(ResourceObjectIds.RadioactiveOreBush), shopConfig.RadioactiveResourceBushSellPrice, RadioactiveBushesForSaleToday);
            }

        }

        /// <summary>
        /// What happens when an item is bought from this shop.
        /// </summary>
        /// <param name="purchasedItem"></param>
        /// <param name="who"></param>
        /// <param name="AmountPurchased"></param>
        /// <returns>True closes the shop menu, false does not.</returns>
        private static bool OnPurchaseFromDwarfShop(ISalable purchasedItem, Farmer who, int AmountPurchased)
        {

            ItemReference item = new ItemReference(purchasedItem);
            if (item.StardewValleyItemId == Enums.SDVObject.Geode)
            {
                NormalGeodesRemainingToday -= AmountPurchased;
                return false;
            }
            if (item.StardewValleyItemId == Enums.SDVObject.FrozenGeode)
            {
                FrozenGeodesRemainingToday -= AmountPurchased;
                return false;
            }
            if (item.StardewValleyItemId == Enums.SDVObject.MagmaGeode)
            {
                MagmaGeodesRemainingToday -= AmountPurchased;
                return false;
            }
            if (item.StardewValleyItemId == Enums.SDVObject.OmniGeode)
            {
                OmniGeodesRemainingToday -= AmountPurchased;
                return false;
            }

            if (item.RegisteredObjectId.Equals(ResourceObjectIds.CoalBush))
            {
                CoalBushesForSaleToday--;
                return false;
            }
            if (item.RegisteredObjectId.Equals(ResourceObjectIds.CopperOreBush))
            {
                CopperBushesForSaleToday--;
                return false;
            }
            if (item.RegisteredObjectId.Equals(ResourceObjectIds.IronOreBush))
            {
                IronBushesForSaleToday--;
                return false;
            }
            if (item.RegisteredObjectId.Equals(ResourceObjectIds.GoldOreBush))
            {
                GoldBushesForSaleToday--;
                return false;
            }
            if (item.RegisteredObjectId.Equals(ResourceObjectIds.IridiumOreBush))
            {
                IridiumBushesForSaleToday--;
                return false;
            }
            if (item.RegisteredObjectId.Equals(ResourceObjectIds.RadioactiveOreBush))
            {
                RadioactiveBushesForSaleToday--;
                return false;
            }

            if (DwarfShop_DefaultOnPurchaseMethod != null)
            {
                return DwarfShop_DefaultOnPurchaseMethod.Invoke(purchasedItem, who, AmountPurchased);
            }

            return false;
        }

    }
}
