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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revitalize.Framework.Configs
{
    /// <summary>
    /// Deals with the sell prices for ores in the blacksmith shop.
    /// </summary>
    public class Shops_BlacksmithConfig
    {
        /// <summary>
        /// The sell price for tin ore from the blacksmith.
        /// </summary>
        public int tinOreSellPrice;
        /// <summary>
        /// The sell price for bauxite ore from the blacksmith.
        /// </summary>
        public int bauxiteOreSellPrice;
        /// <summary>
        /// The sell price for lead ore from the blacksmith.
        /// </summary>
        public int leadOreSellPrice;
        /// <summary>
        /// The sell price for silver ore from the blacksmith.
        /// </summary>
        public int silverOreSellPrice;
        /// <summary>
        /// The sell price for titanium ore from the blacksmith.
        /// </summary>
        public int titaniumOreSellPrice;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Shops_BlacksmithConfig()
        {
            this.tinOreSellPrice = 100;
            this.bauxiteOreSellPrice = 150;
            this.leadOreSellPrice = 200;
            this.silverOreSellPrice = 250;
            this.titaniumOreSellPrice = 300;
        }

        /// <summary>
        /// Initializes the config for the blacksmith shop prices.
        /// </summary>
        /// <returns></returns>
        public static Shops_BlacksmithConfig InitializeConfig()
        {
            if (File.Exists(Path.Combine(ModCore.ModHelper.DirectoryPath, "Configs","Shops","BlacksmithShopPricesConfig.json")))
                return ModCore.ModHelper.Data.ReadJsonFile<Shops_BlacksmithConfig>(Path.Combine("Configs","Shops", "BlacksmithShopPricesConfig.json"));
            else
            {
                Shops_BlacksmithConfig Config = new Shops_BlacksmithConfig();
                ModCore.ModHelper.Data.WriteJsonFile<Shops_BlacksmithConfig>(Path.Combine("Configs","Shops", "BlacksmithShopPricesConfig.json"), Config);
                return Config;
            }
        }

    }
}
