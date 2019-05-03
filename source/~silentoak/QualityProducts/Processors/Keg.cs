using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SilentOak.QualityProducts.Extensions;
using StardewValley;
using SObject = StardewValley.Object;

namespace SilentOak.QualityProducts.Processors
{
    internal class Keg : Processor
    {
        /*********
         * Fields
         *********/

        private static readonly Recipe[] recipes =
        {
            // Wheat => Beer
            new Recipe(
                name: "Beer",
                inputID: 262,
                inputAmount: 1,
                minutes: 1750,
                process: _ => new SObject(346, 1),
                workingEffects: (location, tile) =>
                {
                    location.playSound("bubbles");
                    Animation.PerformGraphics(location, Animation.Bubbles(tile, Color.Yellow));
                }
            ),

            // Hops => Pale Ale
            new Recipe(
                name: "Pale Ale",
                inputID: 304,
                inputAmount: 1,
                minutes: 2250,
                process: _ => new SObject(303, 1),
                workingEffects: (location, tile) =>
                {
                    location.playSound("bubbles");
                    Animation.PerformGraphics(location, Animation.Bubbles(tile, Color.Yellow));
                }
            ),

            // 5 Coffee Beans => Coffee
            new Recipe(
                name: "Coffee",
                inputID: 433,
                inputAmount: 5,
                minutes: 120,
                process: input => new SObject(395, 1),
                failAmount: () =>
                {
                    Game1.showRedMessage(
                        Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12721")
                    );
                },
                workingEffects: (location, tile) =>
                {
                    location.playSound("bubbles");
                    Animation.PerformGraphics(location, Animation.Bubbles(tile, Color.DarkGray));
                }
            ),

            // Honey => Mead
            new Recipe(
                name: "Mead",
                inputID: 340,
                inputAmount: 1,
                minutes: 600,
                process: input =>
                {
                    SObject mead = new SObject(459, 1);
                    HoneyType? maybeHoneyType = input.honeyType.Value;
                    if (maybeHoneyType.HasValue && maybeHoneyType.Value != HoneyType.Wild)
                    {
                        mead.Name = maybeHoneyType.Value.ToString().SplitCamelCase(join: " ") + " Mead";
                        mead.Price = 2 * input.Price;
                    }

                    mead.honeyType.Value = maybeHoneyType.Value;
                    return mead;
                },
                workingEffects: (location, tile) =>
                {
                    location.playSound("bubbles");
                    Animation.PerformGraphics(location, Animation.Bubbles(tile, Color.Yellow));
                }
            ),

            // Vegetable => Juice
            new Recipe(
                name: "Juice",
                inputID: SObject.VegetableCategory,
                inputAmount: 1,
                minutes: 6000,
                process: input =>
                {
                    SObject juice = new SObject(350, 1)
                    {
                        Name = input.Name + " Juice",
                        Price = (int)(2.25 * input.Price)
                    };
                    juice.preserve.Value = PreserveType.Juice;
                    juice.preservedParentSheetIndex.Value = input.ParentSheetIndex;
                    return juice;
                },
                workingEffects: (location, tile) =>
                {
                    location.playSound("bubbles");
                    Animation.PerformGraphics(location, Animation.Bubbles(tile, Color.White));
                }
            ),
            
            // Fruit => Wine
            new Recipe(
                name: "Wine",
                inputID: SObject.FruitsCategory,
                inputAmount: 1,
                minutes: 10000,
                process: input =>
                {
                    SObject wine = new SObject(348, 1)
                    {
                        Name = input.Name + " Wine",
                        Price = 3 * input.Price
                    };
                    wine.preserve.Value = PreserveType.Wine;
                    wine.preservedParentSheetIndex.Value = input.ParentSheetIndex;
                    return wine;
                },
                workingEffects: (location, tile) =>
                {
                    location.playSound("bubbles");
                    Animation.PerformGraphics(location, Animation.Bubbles(tile, Color.Lavender));
                }
            )
        };


        /*************
         * Properties
         *************/

        public override IEnumerable<Recipe> Recipes => recipes;


        /****************
         * Public methods
         ****************/

        public Keg() : base(ProcessorTypes.Keg)
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
            Game1.stats.BeveragesMade++;
        }
    }
}
