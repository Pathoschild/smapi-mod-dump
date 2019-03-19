using Microsoft.Xna.Framework;
using StardewValley;
using SObject = StardewValley.Object;

namespace QualityProducts.Processors
{
    internal class Keg : Processor
    {
        /****************
         * Public methods
         ****************/
        
        public Keg() : base(ProcessorType.KEG)
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
                case 262:
                    heldObject.Value = new SObject(Vector2.Zero, 346, "Beer", false, true, false, false);
                    if (!probe)
                    {
                        heldObject.Value.name = "Beer";
                        minutesUntilReady.Value = 1750;
                        who.currentLocation.playSound("Ship");
                        who.currentLocation.playSound("bubbles");
                        Animation.PerformGraphics(who.currentLocation, Animation.Bubbles(TileLocation, Color.Yellow));
                    }
                    return true;
                case 304:
                    heldObject.Value = new SObject(Vector2.Zero, 303, "Pale Ale", false, true, false, false);
                    if (!probe)
                    {
                        heldObject.Value.name = "Pale Ale";
                        minutesUntilReady.Value = 2250;
                        who.currentLocation.playSound("Ship");
                        who.currentLocation.playSound("bubbles");
                        Animation.PerformGraphics(who.currentLocation, Animation.Bubbles(TileLocation, Color.Yellow));
                    }
                    return true;
                case 433:
                    if (@object.Stack < 5 && !probe)
                    {
                        Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12721"));
                        return false;
                    }
                    heldObject.Value = new SObject(Vector2.Zero, 395, "Coffee", false, true, false, false);
                    if (!probe)
                    {
                        heldObject.Value.name = "Coffee";
                        @object.Stack -= 4;
                        if (@object.Stack <= 0)
                        {
                            who.removeItemFromInventory(@object);
                        }
                        minutesUntilReady.Value = 120;
                        who.currentLocation.playSound("Ship");
                        who.currentLocation.playSound("bubbles");
                        Animation.PerformGraphics(who.currentLocation, Animation.Bubbles(TileLocation, Color.DarkGray));
                    }
                    return true;
                case 340:
                    heldObject.Value = new SObject(Vector2.Zero, 459, "Mead", false, true, false, false);
                    if (!probe)
                    {
                        heldObject.Value.name = "Mead";
                        minutesUntilReady.Value = 600;
                        who.currentLocation.playSound("Ship");
                        who.currentLocation.playSound("bubbles");
                        Animation.PerformGraphics(who.currentLocation, Animation.Bubbles(TileLocation, Color.Yellow));
                    }
                    return true;
            }
            switch (@object.Category)
            {
                case -75:
                    heldObject.Value = new SObject(Vector2.Zero, 350, @object.Name + " Juice", false, true, false, false)
                    {
                        Price = (int)(@object.Price * 2.25)
                    };
                    if (!probe)
                    {
                        heldObject.Value.name = @object.Name + " Juice";
                        heldObject.Value.preserve.Value = PreserveType.Juice;
                        heldObject.Value.preservedParentSheetIndex.Value = @object.parentSheetIndex;
                        minutesUntilReady.Value = 6000;
                        who.currentLocation.playSound("Ship");
                        who.currentLocation.playSound("bubbles");
                        Animation.PerformGraphics(who.currentLocation, Animation.Bubbles(TileLocation, Color.White));
                    }
                    return true;
                case -79:
                    heldObject.Value = new SObject(Vector2.Zero, 348, @object.Name + " Wine", false, true, false, false)
                    {
                        Price = @object.Price * 3
                    };
                    if (!probe)
                    {
                        heldObject.Value.name = @object.Name + " Wine";
                        heldObject.Value.preserve.Value = PreserveType.Wine;
                        heldObject.Value.preservedParentSheetIndex.Value = @object.parentSheetIndex;
                        minutesUntilReady.Value = 10000;
                        who.currentLocation.playSound("Ship");
                        who.currentLocation.playSound("bubbles");
                        Animation.PerformGraphics(who.currentLocation, Animation.Bubbles(TileLocation, Color.Lavender));
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
            Game1.stats.BeveragesMade++;
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
            Color color = Color.DarkGray;
            if (heldObject.Value.Name.Contains("Wine"))
            {
                color = Color.Lavender;
            }
            else if (heldObject.Value.Name.Contains("Juice"))
            {
                color = Color.White;
            }
            else if (heldObject.Value.name.Equals("Beer"))
            {
                color = Color.Yellow;
            }

            environment.playSound("bubbles");
            Animation.PerformGraphics(environment, Animation.Bubbles(TileLocation, color));
        }
    }
}
