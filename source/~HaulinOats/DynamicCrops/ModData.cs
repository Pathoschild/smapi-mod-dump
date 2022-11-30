/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HaulinOats/StardewMods
**
*************************************************/

using System.Collections.Generic;

namespace DynamicCrops
{
    public class ModData
    {
        //https://stardewcommunitywiki.com/Modding:Crop_data
        //[0]Growth Stages/[1]Growth Seasons/[2]Sprite Sheet Index/[3]Harvest Item Index/[4]Regrow After Harvest (-1 = no regrow)/[5]Harvest Method (0 = no scythe needed)/[6]Chance For Extra Harvest/[7]Raised Seeds (true if trellis item)/[8]Tint Color
        public Dictionary<int, string> CropData { get; set; }
        //[0]English Name/[1]Sell Price (Seed Cost)/[2]Edibility/[3]Seed Category/[4]Display Name/[5]Description
        public Dictionary<int, string> ObjectData { get; set; }
        public Dictionary<string, int> SeedPrices { get; set; }
    }
}

