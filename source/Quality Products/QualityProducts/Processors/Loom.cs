using Microsoft.Xna.Framework;
using StardewValley;
using SObject = StardewValley.Object;

namespace QualityProducts.Processors
{
    public class Loom : Processor
    {
        public Loom() : base(ProcessorType.LOOM)
        {
        }

        protected override bool PerformProcessing(SObject @object, bool probe, Farmer who)
        {
            if (@object.ParentSheetIndex == 440)
            {
                heldObject.Value = new SObject(Vector2.Zero, 428, null, false, true, false, false);
                if (!probe)
                {
                    minutesUntilReady.Value = 240;
                    who.currentLocation.playSound("Ship");
                }
                return true;
            }
            return false;
        }
    }
}
