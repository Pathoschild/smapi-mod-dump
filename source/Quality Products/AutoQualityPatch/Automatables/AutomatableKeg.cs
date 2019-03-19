using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Automate;
using StardewValley;
using SObject = StardewValley.Object;

/***
 * From https://github.com/Pathoschild/StardewMods/blob/4644fc87f2295a23f9a60caf462cdd880193c9e5/Automate/Framework/Machines/Objects/KegMachine.cs
 ***/
namespace AutoQualityPatch.Automatables
{
    internal class AutomatableKeg : AutomatableBase
    {
        /*********
         * Fields
         *********/

        private readonly IRecipe[] recipes =
        {
            // honey => mead
            new Recipe(
                input: 340,
                inputCount: 1,
                output: input => new SObject(Vector2.Zero, 459, "Mead", false, true, false, false) { name = "Mead" },
                minutes: 600
            ),

            // coffee bean => coffee
            new Recipe(
                input: 433,
                inputCount: 5,
                output: input => new SObject(Vector2.Zero, 395, "Coffee", false, true, false, false) { name = "Coffee" },
                minutes: 120
            ),

            // wheat => beer
            new Recipe(
                input: 262,
                inputCount: 1,
                output: input => new SObject(Vector2.Zero, 346, "Beer", false, true, false, false) { name = "Beer" },
                minutes: 1750
            ),

            // hops => pale ale
            new Recipe(
                input: 304,
                inputCount: 1,
                output: input => new SObject(Vector2.Zero, 303, "Pale Ale", false, true, false, false) { name = "Pale Ale" },
                minutes: 2250
            ),

            // fruit => wine
            new Recipe(
                input: SObject.FruitsCategory,
                inputCount: 1,
                output: input =>
                {
                    SObject wine = new SObject(Vector2.Zero, 348, input.Name + " Wine", false, true, false, false)
                    {
                        name = input.Name + " Wine",
                        Price = ((SObject)input).Price * 3
                    };
                    wine.preserve.Value = SObject.PreserveType.Wine;
                    wine.preservedParentSheetIndex.Value = input.ParentSheetIndex;
                    return wine;
                },
                minutes: 10000
            ),

            // vegetable => juice
            new Recipe(
                input: SObject.VegetableCategory,
                inputCount: 1,
                output: input =>
                {
                    SObject juice = new SObject(Vector2.Zero, 350, input.Name + " Juice", false, true, false, false)
                    {
                        name = input.Name + " Juice",
                        Price = (int)(((SObject)input).Price * 2.25)
                    };
                    juice.preserve.Value = SObject.PreserveType.Juice;
                    juice.preservedParentSheetIndex.Value = input.ParentSheetIndex;
                    return juice;
                },
                minutes: 6000
            )
        };


        /*************
         * Properties
         *************/

        protected override IRecipe[] Recipes => recipes;


        /*******************
         * Internal methods
         *******************/

        internal AutomatableKeg(SObject entity, GameLocation location, Vector2 tile) : base(entity, location, tile)
        {
        }
    }
}