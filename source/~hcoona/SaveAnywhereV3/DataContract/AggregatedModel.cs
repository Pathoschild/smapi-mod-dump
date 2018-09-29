using System.Collections.Generic;

namespace SaveAnywhereV3.DataContract
{
    public class AggregatedModel
    {
        public PlayerPostion PlayerPosition { get; set; }

        public List<ShippingBinItemRecord> ShippingBinItemRecordList { get; set; }

        public List<NpcPosition> NpcPositionList { get; set; }

        public GlobalInfo GlobalInfo { get; set; }
    }
}
