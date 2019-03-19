using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Automate;
using StardewValley;
using SObject = StardewValley.Object;


/***
 * From https://github.com/Pathoschild/StardewMods/blob/4644fc87f2295a23f9a60caf462cdd880193c9e5/Automate/Framework/Machines/Objects/LoomMachine.cs
 ***/
namespace AutoQualityPatch.Automatables
{
    internal class AutomatableLoom : AutomatableBase
    {
        /*********
         * Fields
         *********/

        private readonly IRecipe[] recipes =
        {
            // wool => cloth
            new Recipe(
                input: 440,
                inputCount: 1,
                output: item => new SObject(Vector2.Zero, 428, null, false, true, false, false),
                minutes: 240
            )
        };


        /*************
         * Properties
         *************/

        protected override IRecipe[] Recipes => recipes;


        /*******************
         * Internal methods
         *******************/

        internal AutomatableLoom(SObject entity, GameLocation location, Vector2 tile) : base(entity, location, tile)
        {
        }
    }
}