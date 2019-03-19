using Microsoft.Xna.Framework;
using StardewValley;
using SObject = StardewValley.Object;

namespace QualityProducts.Processors
{
    internal class MayonnaiseMachine : Processor
    {
        /****************
         * Public methods
         ****************/

        public MayonnaiseMachine() : base(ProcessorType.MAYONNAISE_MACHINE)
        {
        }


        /*******************
         * Protected methods
         *******************/

        /***
         * From StardewValley.Object.performObjectDropInAction
         ***/
        /// <summary>
        /// Performs item processing.
        /// </summary>
        /// <returns><c>true</c> if started processing, <c>false</c> otherwise.</returns>
        /// <param name="object">Object to be processed.</param>
        /// <param name="probe">If set to <c>true</c> probe.</param>
        /// <param name="who">Farmer that initiated processing.</param>
        protected override bool PerformProcessing(SObject @object, bool probe, Farmer who)
        {
            switch (@object.ParentSheetIndex)
            {
                case 107:
                case 174:
                case 182:
                    heldObject.Value = new SObject(Vector2.Zero, 306, null, false, true, false, false)
                    {
                        Stack = 2
                    };
                    if (!probe)
                    {
                        minutesUntilReady.Value = 180;
                        who.currentLocation.playSound("Ship");
                    }
                    return true;
                case 176:
                case 180:
                    heldObject.Value = new SObject(Vector2.Zero, 306, null, false, true, false, false);
                    if (!probe)
                    {
                        minutesUntilReady.Value = 180;
                        who.currentLocation.playSound("Ship");
                    }
                    return true;
                case 442:
                    heldObject.Value = new SObject(Vector2.Zero, 307, null, false, true, false, false);
                    if (!probe)
                    {
                        minutesUntilReady.Value = 180;
                        who.currentLocation.playSound("Ship");
                    }
                    return true;
                case 305:
                    heldObject.Value = new SObject(Vector2.Zero, 308, null, false, true, false, false);
                    if (!probe)
                    {
                        minutesUntilReady.Value = 180;
                        who.currentLocation.playSound("Ship");
                    }
                    return true;
            }
            return false;
        }
    }
}
