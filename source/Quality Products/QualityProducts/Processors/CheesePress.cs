using Microsoft.Xna.Framework;
using StardewValley;
using SObject = StardewValley.Object;

namespace QualityProducts.Processors
{
    internal class CheesePress : Processor
    {
        /****************
         * Public methods
         ****************/

        public CheesePress() : base(ProcessorType.CHEESE_PRESS)
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

        /***
         * From StardewValley.Object.checkForAction
         ***/
        /// <summary>
        /// Updates the game stats.
        /// </summary>
        /// <param name="object">Previously held object.</param>
        protected override void UpdateStats(SObject @object)
        {
            if (@object.ParentSheetIndex == 426)
            {
                Game1.stats.GoatCheeseMade++;
            }
            else
            {
                Game1.stats.CheeseMade++;
            }
        }
    }
}
