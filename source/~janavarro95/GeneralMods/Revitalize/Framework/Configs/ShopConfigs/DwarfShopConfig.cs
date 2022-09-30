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
    public class DwarfShopConfig
    {

        public int NumberOfNormalGeodesToSell;
        public int NumberOfFrozenGeodesToSell;
        public int NumberOfMagmaGeodesToSell;
        public int NumberOfOmniGeodesToSell;

        public int NormalGeodePrice;
        public int FrozenGeodePrice;
        public int MagmaGeodePrice;
        public int OmniGeodePrice;

        public bool SellOmniGeodesEveryDayInsteadOnJustSundays;

        public DwarfShopConfig()
        {

            this.NumberOfNormalGeodesToSell = 3;
            this.NumberOfFrozenGeodesToSell = 3;
            this.NumberOfMagmaGeodesToSell = 3;
            this.NumberOfOmniGeodesToSell = 1;

            this.NormalGeodePrice = 250;
            this.FrozenGeodePrice = 400;
            this.MagmaGeodePrice = 750;
            this.OmniGeodePrice = 1250;

            this.SellOmniGeodesEveryDayInsteadOnJustSundays = false;
        }

    }
}
