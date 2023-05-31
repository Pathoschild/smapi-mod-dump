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

namespace Omegasis.Revitalize.Framework.Configs.ShopConfigs
{
    /// <summary>
    /// Deals with configs for shops.
    /// </summary>
    public class ShopsConfigManager
    {
        /// <summary>
        /// Config file for maanging extra items added to Clint's shop.
        /// </summary>
        public AdventureGuildShopConfig adventureGuildShopConfig;

        /// <summary>
        /// Config file for marnie's normal non-animal stock.
        /// </summary>
        public AnimalShopStockConfig animalShopStockConfig;

        /// <summary>
        /// Config file for maanging extra items added to Clint's shop.
        /// </summary>
        public BlacksmithShopConfig blacksmithShopsConfig;

        /// <summary>
        /// Config file for managing extra items added to dwarf's shop.
        /// </summary>
        public DwarfShopConfig dwarfShopConfig;

        /// <summary>
        /// Config file for things sold in robin's shop.
        /// </summary>
        public RobinsShopConfig robinsShopConfig;

        /// <summary>
        /// The config file for Qi's Walnut Room.
        /// </summary>
        public WalnutRoomShopConfig walnutRoomShopConfig;

        /// <summary>
        /// Config file for the custom hay maker shop outside of Marnie's ranch.
        /// </summary>
        public HayMakerShopConfig hayMakerShopConfig;

        /// <summary>
        /// Config file for handling movie theater sell prices.
        /// </summary>
        public MovieTheaterShopConfig movieTheaterShopConfig;

        public ShopsConfigManager()
        {
            this.adventureGuildShopConfig = ConfigManager.InitializeConfig<AdventureGuildShopConfig>("Configs", "Shops", "AdventureGuildShopConfig.json");
            this.animalShopStockConfig = ConfigManager.InitializeConfig<AnimalShopStockConfig>("Configs", "Shops", "AnimalShopConfig.json");
            this.blacksmithShopsConfig = ConfigManager.InitializeConfig<BlacksmithShopConfig>("Configs", "Shops", "BlacksmithShopConfig.json");
            this.dwarfShopConfig = ConfigManager.InitializeConfig<DwarfShopConfig>("Configs", "Shops", "DwarfShopConfig.json");
            this.hayMakerShopConfig = ConfigManager.InitializeConfig<HayMakerShopConfig>("Configs", "Shops", "HayMakerShopConfig.json");
            this.robinsShopConfig = ConfigManager.InitializeConfig<RobinsShopConfig>("Configs", "Shops", "RobinsShopConfig.json");

            this.movieTheaterShopConfig = ConfigManager.InitializeConfig<MovieTheaterShopConfig>("Configs", "Shops", "MovieTheaterConfig.json");

            this.walnutRoomShopConfig = ConfigManager.InitializeConfig<WalnutRoomShopConfig>("Configs", "Shops", "WalnutRoomShopConfig.json");
        }



    }
}
