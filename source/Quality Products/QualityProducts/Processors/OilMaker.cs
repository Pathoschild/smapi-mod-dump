using Microsoft.Xna.Framework;
using StardewValley;
using SObject = StardewValley.Object;

namespace QualityProducts.Processors
{
    internal class OilMaker : Processor
    {
        /****************
         * Public methods
         ****************/

        public OilMaker() : base(ProcessorType.OIL_MAKER)
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
                case 270:
                    heldObject.Value = new SObject(Vector2.Zero, 247, null, false, true, false, false);
                    if (!probe)
                    {
                        minutesUntilReady.Value = 1000;
                        who.currentLocation.playSound("bubbles");
                        who.currentLocation.playSound("sipTea");
                        Animation.PerformGraphics(who.currentLocation, Animation.Bubbles(TileLocation, Color.Yellow));

                    }
                    return true;
                case 421:
                    heldObject.Value = new SObject(Vector2.Zero, 247, null, false, true, false, false);
                    if (!probe)
                    {
                        minutesUntilReady.Value = 60;
                        who.currentLocation.playSound("bubbles");
                        who.currentLocation.playSound("sipTea");
                        Animation.PerformGraphics(who.currentLocation, Animation.Bubbles(TileLocation, Color.Yellow));
                    }
                    return true;
                case 430:
                    heldObject.Value = new SObject(Vector2.Zero, 432, null, false, true, false, false);
                    if (!probe)
                    {
                        minutesUntilReady.Value = 360;
                        who.currentLocation.playSound("bubbles");
                        who.currentLocation.playSound("sipTea");
                        Animation.PerformGraphics(who.currentLocation, Animation.Bubbles(TileLocation, Color.Yellow));
                    }
                    return true;
                case 431:
                    heldObject.Value = new SObject(247, 1, false, -1, 0);
                    if (!probe)
                    {
                        minutesUntilReady.Value = 3200;
                        who.currentLocation.playSound("bubbles");
                        who.currentLocation.playSound("sipTea");
                        Animation.PerformGraphics(who.currentLocation, Animation.Bubbles(TileLocation, Color.Yellow));
                    }
                    return true;
            }
            return false;
        }

        /***
         * From StardewValley.Object.addWorkingAnimation
         ***/
        /// <summary>
        /// Adds this entity's working animation to the specified game location.
        /// </summary>
        /// <param name="environment">Game location.</param>
        protected override void AddWorkingAnimationTo(GameLocation environment)
        {
            Animation.PerformGraphics(environment, Animation.Bubbles(TileLocation, Color.Yellow));
        }
    }
}
