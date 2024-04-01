/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;

namespace CustomObjectProduction
{
    public class ProductData
    {
        public string id;
        public List<ProductInfo> infoList = new List<ProductInfo>();
        public int amount;
        public int quality;
    }

    public class ProductInfo
    {
        public string id;
        public int min;
        public int max;
        public int minQuality;
        public int maxQuality;
        public int weight;
    }
}