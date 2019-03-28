using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using SObject = StardewValley.Object;

namespace SilentOak.QualityProducts.Processors
{
    internal class PreservesJar : Processor
    {
        /*********
         * Fields
         *********/

        /// <summary>
        /// The available recipes for this entity.
        /// </summary>
        private static readonly Recipe[] recipes =
        {
            // Vegetable => Pickles
            new Recipe(
                name: "Pickles",
                inputID: SObject.VegetableCategory,
                inputAmount: 1,
                minutes: 4000,
                process: input =>
                {
                    SObject output = new SObject(342, 1)
                    {
                        Price = 50 + input.Price * 2,
                        Name = "Pickled " + input.Name
                    };

                    output.preserve.Value = PreserveType.Pickle;
                    output.preservedParentSheetIndex.Value = input.ParentSheetIndex;
                    return output;
                },
                workingEffects: (location, tile) =>
                {
                    Animation.PerformGraphics(location, Animation.Bubbles(tile, Color.White));
                }
            ),

            // Fruit => Jelly
            new Recipe(
                name: "Jelly",
                inputID: SObject.FruitsCategory,
                inputAmount: 1,
                minutes: 4000,
                process: input =>
                {
                    SObject output = new SObject(344, 1)
                    {
                        Price = 50 + input.Price * 2,
                        Name = input.Name + " Jelly"
                    };

                    output.preserve.Value = PreserveType.Jelly;
                    output.preservedParentSheetIndex.Value = input.ParentSheetIndex;
                    return output;
                },
                workingEffects: (location, tile) =>
                {
                    Animation.PerformGraphics(location, Animation.Bubbles(tile, Color.LightBlue));
                }
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
        /// Instantiates a <see cref="T:QualityProducts.Processors.PreservesJar"/>.
        /// </summary>
        /// <param name="location">Where the entity is.</param>
        public PreservesJar() : base(ProcessorTypes.PreservesJar)
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
            Game1.stats.PreservesMade++;
        }
    }
}
