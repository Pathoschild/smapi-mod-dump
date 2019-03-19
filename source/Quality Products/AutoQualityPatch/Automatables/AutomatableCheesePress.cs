using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Automate;
using StardewValley;
using SObject = StardewValley.Object;


/***
 * From https://github.com/Pathoschild/StardewMods/blob/4644fc87f2295a23f9a60caf462cdd880193c9e5/Automate/Framework/Machines/Objects/CheesePressMachine.cs
 ***/
namespace AutoQualityPatch.Automatables
{
    internal class AutomatableCheesePress : AutomatableBase
    {
        /*********
         * Fields
         *********/
        
        private readonly IRecipe[] recipes =
        {
            // goat milk => goat cheese
            new Recipe(
                input: 436,
                inputCount: 1,
                output: input => new SObject(Vector2.Zero, 426, null, false, true, false, false),
                minutes: 200
            ),

            // large goat milk => two goat cheese
            new Recipe(
                input: 438,
                inputCount: 1,
                output: input => new SObject(Vector2.Zero, 426, null, false, true, false, false) { Stack = 2 },
                minutes: 200
            ),

            // milk => cheese
            new Recipe(
                input: 184,
                inputCount: 1,
                output: input => new SObject(Vector2.Zero, 424, null, false, true, false, false),
                minutes: 200
            ),

            // large milk => two cheese
            new Recipe(
                input: 186,
                inputCount: 1,
                output: input => new SObject(Vector2.Zero, 424, "Cheese (=)", false, true, false, false) { Stack = 2 },
                minutes: 200
            )
        };


        /*************
         * Properties
         *************/

        protected override IRecipe[] Recipes => recipes;


        /*******************
         * Internal methods
         *******************/

        internal AutomatableCheesePress(SObject entity, GameLocation location, Vector2 tile) : base(entity, location, tile)
        {
        }
    }
}