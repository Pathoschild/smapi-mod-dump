/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

#nullable disable

using Microsoft.Xna.Framework;
using StardewValley;
using SObject = StardewValley.Object;

namespace Pathoschild.Stardew.Automate.Framework.Machines.Objects
{
    /// <summary>A mayonnaise that accepts input and provides output.</summary>
    /// <remarks>Derived from <see cref="SObject.performObjectDropInAction"/> (search for 'Mayonnaise Machine').</remarks>
    internal class MayonnaiseMachine : GenericObjectMachine<SObject>
    {
        /*********
        ** Fields
        *********/
        /// <summary>The recipes to process.</summary>
        private readonly IRecipe[] Recipes =
        {
            // void egg => void mayonnaise
            new Recipe(
                input: 305,
                inputCount: 1,
                output: _ => new SObject(Vector2.Zero, 308, null, false, true, false, false),
                minutes: 180
            ),

            // duck egg => duck mayonnaise
            new Recipe(
                input: 442,
                inputCount: 1,
                output: _ => new SObject(Vector2.Zero, 307, null, false, true, false, false),
                minutes: 180
            ),

            // dinosaur egg => dinosaur mayonnaise
            new Recipe(
                input: 107,
                inputCount: 1,
                output: _ => new SObject(Vector2.Zero, 807, null, false, true, false, false),
                minutes: 180
            ),

            // ostrich egg => 10x same-quality mayonnaise
            new Recipe(
                input: 289,
                inputCount: 1,
                output: input => new SObject(Vector2.Zero, 306, null, false, true, false, false) { Stack = 10, Quality = ((SObject)input).Quality },
                minutes: 180
            ),

            // white/brown egg => normal mayonnaise
            new Recipe(
                input: item => item.ParentSheetIndex is 176 or 180,
                inputCount: 1,
                output: _ => new SObject(Vector2.Zero, 306, null, false, true, false, false),
                minutes: _ => 180
            ),
            
            // dinosaur or large white/brown egg => gold-quality mayonnaise
            new Recipe(
                input: item => item.ParentSheetIndex is 107 or 174 or 182,
                inputCount: 1,
                output: _ => new SObject(Vector2.Zero, 306, null, false, true, false, false) { Quality = SObject.highQuality },
                minutes: _ => 180
            ),

            // golden egg => 3x gold-quality mayonnaise
            new Recipe(
                input: 928,
                inputCount: 1,
                output: _ => new SObject(Vector2.Zero, 306, null, false, true, false, false) { Stack = 3, Quality = SObject.highQuality },
                minutes: 180
            ),
        };


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="machine">The underlying machine.</param>
        /// <param name="location">The location containing the machine.</param>
        /// <param name="tile">The tile covered by the machine.</param>
        public MayonnaiseMachine(SObject machine, GameLocation location, Vector2 tile)
            : base(machine, location, tile, machineTypeId: "MayonnaiseMachine") { }

        /// <summary>Provide input to the machine.</summary>
        /// <param name="input">The available items.</param>
        /// <returns>Returns whether the machine started processing an item.</returns>
        public override bool SetInput(IStorage input)
        {
            return this.GenericPullRecipe(input, this.Recipes);
        }
    }
}
