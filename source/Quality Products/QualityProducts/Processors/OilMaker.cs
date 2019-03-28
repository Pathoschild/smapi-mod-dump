using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using SObject = StardewValley.Object;

namespace SilentOak.QualityProducts.Processors
{
    internal class OilMaker : Processor
    {
        /*********
         * Fields
         *********/

        /// <summary>
        /// The available recipes for this entity.
        /// </summary>
        private static readonly Recipe[] recipes =
        {
            // Corn => Oil
            new Recipe(
                name: "Oil",
                inputID: 270,
                inputAmount: 1,
                minutes: 1000,
                process: _ => new SObject(247, 1)
            ),

            // Sunflower => Oil
            new Recipe(
                name: "Oil",
                inputID: 421,
                inputAmount: 1,
                minutes: 60,
                process: _ => new SObject(247, 1)
            ),

            // Sunflower Seeds => Oil
            new Recipe(
                name: "Oil",
                inputID: 431,
                inputAmount: 1,
                minutes: 3200,
                process: _ => new SObject(247, 1)
            ),

            // Truffle => Truffle Oil
            new Recipe(
                name: "Truffle Oil",
                inputID: 430,
                inputAmount: 1,
                minutes: 360,
                process: _ => new SObject(432, 1)
            )
        };


        /*************
         * Properties
         *************/

        /// <summary>
        /// Gets the available recipes for this entity.
        /// </summary>
        /// <value>The recipes.</value>
        public override IEnumerable<Recipe> Recipes => recipes;


        /****************
         * Public methods
         ****************/

        /// <summary>
        /// Instantiates an <see cref="T:QualityProducts.Processors.OilMaker"/>.
        /// </summary>
        /// <param name="location">Where the entity is.</param>
        public OilMaker() : base(ProcessorTypes.OilMaker)
        {
        }


        /*******************
         * Protected methods
         *******************/

        /// <summary>
        /// Executes if recipe doesn't specify any input effects.
        /// </summary>
        protected override void DefaultInputEffects(GameLocation location)
        {
            location.playSound("bubbles");
            location.playSound("sipTea");
        }

        /// <summary>
        /// Executes if recipe doesn't specify any working effects.
        /// </summary>
        protected override void DefaultWorkingEffects(GameLocation location)
        {
            Animation.PerformGraphics(location, Animation.Bubbles(TileLocation, Color.Yellow));
        }
    }
}
