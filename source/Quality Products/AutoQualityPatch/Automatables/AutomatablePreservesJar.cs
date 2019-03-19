using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Automate;
using StardewValley;
using SObject = StardewValley.Object;


/***
 * From https://github.com/Pathoschild/StardewMods/blob/4644fc87f2295a23f9a60caf462cdd880193c9e5/Automate/Framework/Machines/Objects/PreservesJarMachine.cs
 ***/
namespace AutoQualityPatch.Automatables
{
    internal class AutomatablePreservesJar : AutomatableBase
    {
        /*********
         * Fields
         *********/

        private readonly IRecipe[] recipes =
        {
            // fruit => jelly
            new Recipe(
                input: SObject.FruitsCategory,
                inputCount: 1,
                output: input =>
                {
                    SObject jelly = new SObject(Vector2.Zero, 344, input.Name + " Jelly", false, true, false, false)
                    {
                        Price = 50 + ((SObject) input).Price * 2,
                        name = input.Name + " Jelly"
                    };
                    jelly.preserve.Value = SObject.PreserveType.Jelly;
                    jelly.preservedParentSheetIndex.Value = input.ParentSheetIndex;
                    return jelly;
                },
                minutes: 4000
            ),

            // vegetable => pickled vegetable
            new Recipe(
                input: SObject.VegetableCategory,
                inputCount: 1,
                output: input =>
                {
                    SObject item = new SObject(Vector2.Zero, 342, "Pickled " + input.Name, false, true, false, false)
                    {
                        Price = 50 + ((SObject) input).Price * 2,
                        name = "Pickled " + input.Name
                    };
                    item.preserve.Value = SObject.PreserveType.Pickle;
                    item.preservedParentSheetIndex.Value = input.ParentSheetIndex;
                    return item;

                },
                minutes: 4000
            )
        };


        /*************
         * Properties
         *************/

        protected override IRecipe[] Recipes => recipes;


        /*******************
         * Internal methods
         *******************/

        internal AutomatablePreservesJar(SObject entity, GameLocation location, Vector2 tile) : base(entity, location, tile)
        {
        }
    }
}