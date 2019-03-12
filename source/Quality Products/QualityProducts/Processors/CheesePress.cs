using Microsoft.Xna.Framework;
using StardewValley;
using SObject = StardewValley.Object;

namespace QualityProducts.Processors
{
    public class CheesePress : Processor
    {
        public CheesePress() : base(ProcessorType.CHEESE_PRESS)
        {
        }

        protected override bool PerformProcessing(SObject @object, bool probe, Farmer who)
        {
            switch (@object.ParentSheetIndex)
            {
                case 436:
                    heldObject.Value = new SObject(Vector2.Zero, 426, null, false, true, false, false);
                    if (!probe)
                    {
                        minutesUntilReady.Value = 200;
                        who.currentLocation.playSound("Ship");
                    }
                    return true;
                case 438:
                    heldObject.Value = new SObject(Vector2.Zero, 426, null, false, true, false, false)
                    {
                        Stack = 2
                    };
                    if (!probe)
                    {
                        minutesUntilReady.Value = 200;
                        who.currentLocation.playSound("Ship");
                    }
                    return true;
                case 184:
                    heldObject.Value = new SObject(Vector2.Zero, 424, null, false, true, false, false);
                    if (!probe)
                    {
                        minutesUntilReady.Value = 200;
                        who.currentLocation.playSound("Ship");
                    }
                    return true;
                case 186:
                    heldObject.Value = new SObject(Vector2.Zero, 424, "Cheese (=)", false, true, false, false)
                    {
                        Stack = 2
                    };
                    if (!probe)
                    {
                        minutesUntilReady.Value = 200;
                        who.currentLocation.playSound("Ship");
                    }
                    return true;
            }
            return false;
        }
    }
}
