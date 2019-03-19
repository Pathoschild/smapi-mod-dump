using Microsoft.Xna.Framework;
using StardewValley;
using SObject = StardewValley.Object;

namespace QualityProducts.Processors
{
    internal class PreservesJar : Processor
    {
        /****************
         * Public methods
         ****************/

        public PreservesJar() : base(ProcessorType.PRESERVES_JAR)
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
            switch (@object.Category)
            {
                case -75:
                    heldObject.Value = new SObject(Vector2.Zero, 342, "Pickled " + @object.Name, false, true, false, false)
                    {
                        Price = 50 + @object.Price * 2
                    };
                    if (!probe)
                    {
                        heldObject.Value.name = "Pickled " + @object.Name;
                        heldObject.Value.preserve.Value = PreserveType.Pickle;
                        heldObject.Value.preservedParentSheetIndex.Value = @object.parentSheetIndex;
                        minutesUntilReady.Value = 4000;
                        who.currentLocation.playSound("Ship");
                        Animation.PerformGraphics(who.currentLocation, Animation.Bubbles(TileLocation, Color.White));
                    }
                    return true;
                case -79:
                    heldObject.Value = new SObject(Vector2.Zero, 344, @object.Name + " Jelly", false, true, false, false)
                    {
                        Price = 50 + @object.Price * 2
                    };
                    if (!probe)
                    {
                        minutesUntilReady.Value = 4000;
                        heldObject.Value.name = @object.Name + " Jelly";
                        heldObject.Value.preserve.Value = PreserveType.Jelly;
                        heldObject.Value.preservedParentSheetIndex.Value = @object.parentSheetIndex;
                        who.currentLocation.playSound("Ship");
                        Animation.PerformGraphics(who.currentLocation, Animation.Bubbles(TileLocation, Color.LightBlue));
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
            Game1.stats.PreservesMade++;
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
            Color color = Color.White;
            if (heldObject.Value.Name.Contains("Pickled"))
            {
                color = Color.White;
            }
            else if (heldObject.Value.Name.Contains("Jelly"))
            {
                color = Color.LightBlue;
            }

            Animation.PerformGraphics(environment, Animation.Bubbles(TileLocation, color));
        }
    }
}
