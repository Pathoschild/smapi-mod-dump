/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/hcoona/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using SaveAnywhereV3.DataContract;
using StardewValley;

namespace SaveAnywhereV3.Service
{
    public class ShippingBinService : SaveLoadServiceBase<List<ShippingBinItemRecord>>
    {
        public ShippingBinService()
            : base (model => model.ShippingBinItemRecordList)
        { }

        protected override void DoLoad(List<ShippingBinItemRecord> model)
        {
            Game1.getFarm().shippingBin = model
                    .Select(r => new Object(r.ParentSheetIndex, r.Stack, quality: r.Quality))
                    .ToList<Item>();
        }

        protected override List<ShippingBinItemRecord> DumpSaveModel()
        {
            var shippingBin = Game1.getFarm().shippingBin;
            Utility.consolidateStacks(shippingBin);

            return shippingBin.OfType<Object>()
                .Select(item => new ShippingBinItemRecord
                {
                    ParentSheetIndex = item.parentSheetIndex,
                    Stack = item.Stack,
                    Quality = item.quality
                }).ToList();
        }
    }
}
