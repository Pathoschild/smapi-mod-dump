using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Automate;
using StardewValley;
using SObject = StardewValley.Object;


/***
 * From https://github.com/Pathoschild/StardewMods/blob/4644fc87f2295a23f9a60caf462cdd880193c9e5/Automate/Framework/Machines/Objects/OilMakerMachine.cs
 ***/
namespace AutoQualityPatch.Automatables
{
    internal class AutomatableOilMaker : AutomatableBase
    {
        /*********
         * Fields
         *********/

        private readonly IRecipe[] recipes =
        {
            // truffle => truffle oil
            new Recipe(
                input: 430,
                inputCount: 1,
                output: input => new SObject(Vector2.Zero, 432, null, false, true, false, false),
                minutes: 360
            ),

            // sunflower seed => oil
            new Recipe(
                input: 431,
                inputCount: 1,
                output: input => new SObject(247, 1),
                minutes: 3200
            ),

            // corn => oil
            new Recipe(
                input: 270,
                inputCount: 1,
                output: input => new SObject(Vector2.Zero, 247, null, false, true, false, false),
                minutes: 1000
            ), 

            // sunflower => oil
            new Recipe(
                input: 421,
                inputCount: 1,
                output: input => new SObject(Vector2.Zero, 247, null, false, true, false, false),
                minutes: 60
            )
        };


        /*************
         * Properties
         *************/

        protected override IRecipe[] Recipes => recipes;


        /*******************
         * Internal methods
         *******************/

        internal AutomatableOilMaker(SObject entity, GameLocation location, Vector2 tile) : base(entity, location, tile)
        {
        }
    }
}