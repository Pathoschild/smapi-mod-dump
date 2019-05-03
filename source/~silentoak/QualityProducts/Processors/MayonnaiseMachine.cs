using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using SObject = StardewValley.Object;

namespace SilentOak.QualityProducts.Processors
{
    internal class MayonnaiseMachine : Processor
    {
        /*********
         * Fields 
         *********/

        /// <summary>
        /// The available recipes for this entity.
        /// </summary>
        private static readonly Recipe[] recipes =
        {
            // Large Egg (Brown or White) or Dinosaur Egg => 2 Mayo
            new Recipe(
                name: "Mayonnaise",
                inputIDs: new int[] {107, 174, 182},
                inputAmount: 1,
                minutes: 180,
                process: _ => new SObject(306, 2)
            ),

            // Egg (Brown or White) => Mayo
            new Recipe(
                name: "Mayonnaise",
                inputIDs: new int[] {176, 180},
                inputAmount: 1,
                minutes: 180,
                process: _ => new SObject(306, 1)
            ),

            // Duck Egg => Duck Mayo
            new Recipe(
                name: "Duck Mayonnaise",
                inputID: 442,
                inputAmount: 1,
                minutes: 180,
                process: _ => new SObject(307, 1)
            ),

            // Void Egg => Void Mayo
            new Recipe(
                name: "Void Mayonnaise",
                inputID: 305,
                inputAmount: 1,
                minutes: 180,
                process: _ => new SObject(308, 1)
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


        /*****************
         * Public methods
         *****************/

        /// <summary>
        /// Instantiates a <see cref="T:QualityProducts.Processors.MayonnaiseMachine"/>.
        /// </summary>
        /// <param name="location">Where the entity is.</param>
        public MayonnaiseMachine() : base(ProcessorTypes.MayonnaiseMachine)
        {
        }
    }
}
