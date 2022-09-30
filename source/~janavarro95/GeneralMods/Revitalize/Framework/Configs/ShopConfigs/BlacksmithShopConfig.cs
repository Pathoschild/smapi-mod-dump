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

namespace Omegasis.Revitalize.Framework.Configs.ShopConfigs
{
    /// <summary>
    /// Deals with the prices for item's in the blacksmith shop.
    /// </summary>
    public class BlacksmithShopConfig
    {
        /// <summary>
        /// The price for tin ore from the blacksmith.
        /// </summary>
        public int tinOrePrice;
        /// <summary>
        /// The price for bauxite ore from the blacksmith.
        /// </summary>
        public int bauxiteOrePrice;
        /// <summary>
        /// The price for lead ore from the blacksmith.
        /// </summary>
        public int leadOrePrice;
        /// <summary>
        /// The price for silver ore from the blacksmith.
        /// </summary>
        public int silverOrePrice;
        /// <summary>
        /// The price for titanium ore from the blacksmith.
        /// </summary>
        public int titaniumOrePrice;

        /// <summary>
        /// How much the anvil blueprints sell for in clint's shop.
        /// </summary>
        public int anvilBlueprintsPrice;

        /// <summary>
        /// How much a regular axe blueprint sells for in clint's shop.
        /// </summary>
        public int axeBlueprintPrice;
        public int copperAxeBlueprintPrice;
        public int steelAxeBlueprintPrice;
        public int goldAxeBlueprintPrice;
        public int iridiumAxeBlueprintPrice;

        public int hoeBlueprintPrice;
        public int copperHoeBlueprintPrice;
        public int steelHoeBlueprintPrice;
        public int goldHoeBlueprintPrice;
        public int iridiumHoeBlueprintPrice;

        public int pickaxeBlueprintPrice;
        public int copperPickaxeBlueprintPrice;
        public int steelPickaxeBlueprintPrice;
        public int goldPickaxeBlueprintPrice;
        public int iridiumPickaxeBlueprintPrice;

        public int wateringCanBlueprintPrice;
        public int copperWateringCanBlueprintPrice;
        public int steelWateringCanBlueprintPrice;
        public int goldWateringCanBlueprintPrice;
        public int iridiumWateringCanBlueprintPrice;

        /// <summary>
        /// Constructor.
        /// </summary>
        public BlacksmithShopConfig()
        {
            this.tinOrePrice = 100;
            this.bauxiteOrePrice = 150;
            this.leadOrePrice = 200;
            this.silverOrePrice = 250;
            this.titaniumOrePrice = 300;

            this.anvilBlueprintsPrice = 2500;

            int defaultStoneToolBlueprintPrice = 500;
            int defaultCopperToolBlueprintPrice = 1000;
            int defaultSteelToolBlueprintPrice = 2500;
            int defaultGoldToolBlueprintPrice = 5000;
            int defaultIridiumToolBlueprintPrice = 10000;


            this.axeBlueprintPrice = defaultStoneToolBlueprintPrice;
            this.copperAxeBlueprintPrice = defaultCopperToolBlueprintPrice;
            this.steelAxeBlueprintPrice = defaultSteelToolBlueprintPrice;
            this.goldAxeBlueprintPrice = defaultGoldToolBlueprintPrice;
            this.iridiumAxeBlueprintPrice = defaultIridiumToolBlueprintPrice;

            this.hoeBlueprintPrice = defaultStoneToolBlueprintPrice;
            this.copperHoeBlueprintPrice = defaultCopperToolBlueprintPrice;
            this.steelHoeBlueprintPrice = defaultSteelToolBlueprintPrice;
            this.goldHoeBlueprintPrice = defaultGoldToolBlueprintPrice;
            this.iridiumHoeBlueprintPrice = defaultIridiumToolBlueprintPrice;

            this.pickaxeBlueprintPrice = defaultStoneToolBlueprintPrice;
            this.copperPickaxeBlueprintPrice = defaultCopperToolBlueprintPrice;
            this.steelPickaxeBlueprintPrice = defaultSteelToolBlueprintPrice;
            this.goldPickaxeBlueprintPrice = defaultGoldToolBlueprintPrice;
            this.iridiumPickaxeBlueprintPrice = defaultIridiumToolBlueprintPrice;

            this.wateringCanBlueprintPrice = defaultStoneToolBlueprintPrice;
            this.copperWateringCanBlueprintPrice = defaultCopperToolBlueprintPrice;
            this.steelWateringCanBlueprintPrice = defaultSteelToolBlueprintPrice;
            this.goldWateringCanBlueprintPrice = defaultGoldToolBlueprintPrice;
            this.iridiumWateringCanBlueprintPrice = defaultIridiumToolBlueprintPrice;

        }

    }
}
