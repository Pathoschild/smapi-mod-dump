/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/silentoak/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using SObject = StardewValley.Object;

namespace SilentOak.QualityProducts.Processors
{
    internal class Loom : Processor
    {
        /*********
         * Fields
         *********/

        private static readonly Recipe[] recipes =
        {
            // Wool => Cloth
            new Recipe(
                name: "Cloth",
                inputID: 440,
                inputAmount: 1,
                minutes: 240,
                process: _ => new SObject(428, 1)
            )
        };


        /*************
         * Properties
         *************/

        public override IEnumerable<Recipe> Recipes => recipes;


        /****************
         * Public methods
         ****************/

        /// <summary>
        /// Instantiates a <see cref="T:QualityProducts.Processors.Loom"/>.
        /// </summary>
        /// <param name="location">Where the entity is.</param>
        public Loom() : base(ProcessorTypes.Loom)
        {
        }

        /*******************
         * Protected methods
         *******************/

        /// <summary>
        /// Executes if recipe doesn't specify any input effects
        /// </summary>
        protected override void DefaultInputEffects(GameLocation location)
        {
            location.playSound("Ship");
        }
    }
}
