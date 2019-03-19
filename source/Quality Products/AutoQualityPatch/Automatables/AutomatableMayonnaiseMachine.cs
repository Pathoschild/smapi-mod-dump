using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Automate;
using StardewValley;
using SObject = StardewValley.Object;


/***
 * From https://github.com/Pathoschild/StardewMods/blob/4644fc87f2295a23f9a60caf462cdd880193c9e5/Automate/Framework/Machines/Objects/MayonnaiseMachine.cs
 ***/
namespace AutoQualityPatch.Automatables
{
    internal class AutomatableMayonnaiseMachine : AutomatableBase
    {
        /*********
         * Fields
         *********/

        private readonly IRecipe[] recipes =
        {
            // void egg => void mayonnaise
            new Recipe(
                input: 305,
                inputCount: 1,
                output: input => new SObject(Vector2.Zero, 308, null, false, true, false, false),
                minutes: 180
            ),

            // duck egg => duck mayonnaise
            new Recipe(
                input: 442,
                inputCount: 1,
                output: input => new SObject(Vector2.Zero, 307, null, false, true, false, false),
                minutes: 180
            ),

            // white/brown egg => normal mayonnaise
            new Recipe(
                input: 176,
                inputCount: 1,
                output: input => new SObject(Vector2.Zero, 306, null, false, true, false, false),
                minutes: 180
            ),
            new Recipe(
                input: 180,
                inputCount: 1,
                output: input => new SObject(Vector2.Zero, 306, null, false, true, false, false),
                minutes: 180
            ),
            
            // dinosaur or large white/brown egg => two mayonnaise
            new Recipe(
                input: 107,
                inputCount: 1,
                output: input => new SObject(Vector2.Zero, 306, null, false, true, false, false) { Stack = 2 },
                minutes: 180
            ),
            new Recipe(
                input: 174,
                inputCount: 1,
                output: input => new SObject(Vector2.Zero, 306, null, false, true, false, false) { Stack = 2 },
                minutes: 180
            ),
            new Recipe(
                input: 182,
                inputCount: 1,
                output: input => new SObject(Vector2.Zero, 306, null, false, true, false, false) { Stack = 2 },
                minutes: 180
            )
        };


        /*************
         * Properties
         *************/

        protected override IRecipe[] Recipes => recipes;


        /*******************
         * Internal methods
         *******************/

        internal AutomatableMayonnaiseMachine(SObject entity, GameLocation location, Vector2 tile) : base(entity, location, tile)
        {
        }
    }
}