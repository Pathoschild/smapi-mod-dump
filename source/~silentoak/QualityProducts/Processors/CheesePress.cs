using System.Collections.Generic;
using StardewValley;
using SObject = StardewValley.Object;

namespace SilentOak.QualityProducts.Processors
{
    internal class CheesePress : Processor
    {
        /*********
         * Fields
         *********/

        private static readonly Recipe[] recipes =
        {
            // Goat Milk => Goat Cheese
            new Recipe(
                name: "Goat Cheese",
                inputID: 436,
                inputAmount: 1,
                minutes: 200,
                process: _ => new SObject(426, 1)
            ),

            // L. Goat Milk => 2 Goat Cheese
            new Recipe(
                name: "Goat Cheese",
                inputID: 438,
                inputAmount: 1,
                minutes: 200,
                process: _ => new SObject(426, 2)
            ),

            // Milk => Cheese
            new Recipe(
                name: "Cheese",
                inputID: 184,
                inputAmount: 1,
                minutes: 200,
                process: _ => new SObject(424, 1)
            ),

            // Large Milk => 2 Cheese
            new Recipe(
                name: "Cheese",
                inputID: 186,
                inputAmount: 1,
                minutes: 200,
                process: _ => new SObject(424, 2)
            )
        };


        /*************
         * Properties 
         *************/

        public override IEnumerable<Recipe> Recipes => recipes;


        /****************
         * Public methods
         ****************/

        public CheesePress() : base(ProcessorTypes.CheesePress)
        {
        }


        /*******************
         * Protected methods
         *******************/

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
