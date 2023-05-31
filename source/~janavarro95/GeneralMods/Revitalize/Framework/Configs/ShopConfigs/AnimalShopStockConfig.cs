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
    /// Config related to selling various items at Marnie's shop.
    /// </summary>
    public class AnimalShopStockConfig
    {

        /// <summary>
        /// The price that a hay maker feed shop object costs in Marnie's shop.
        /// </summary>
        public int HayMakerFeedShopPrice = 10000;

        /// <summary>
        /// The cost for buying the hay maker blueprints from Marnie's ranch.
        /// </summary>
        public int HayMakerBlueprintsPrice = 2500;
        public AnimalShopStockConfig()
        {

        }
    }
}
