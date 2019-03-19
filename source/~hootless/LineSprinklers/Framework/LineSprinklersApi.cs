using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace LineSprinklers.Framework
{
    /// <summary>The API which provides access to Line Sprinklers for other mods.</summary>
    public class LineSprinklersApi : ILineSprinklersApi
    {
        /*********
        ** Fields
        *********/
        /// <summary>The relative tile coverage by sprinkler ID.</summary>
        private readonly IDictionary<int, Vector2[]> SprinklerCoverage;


        /*********
        ** Public methods
        *********/
        /// <summary>Get the maximum supported coverage width or height.</summary>
        public int GetMaxGridSize() => 24;

        /// <summary>Construct an instance.</summary>
        /// <param name="sprinklerCoverage">The relative tile coverage by sprinkler ID.</param>
        public LineSprinklersApi(IDictionary<int, Vector2[]> sprinklerCoverage)
        {
            this.SprinklerCoverage = sprinklerCoverage;
        }

        /// <summary>Get the relative tile coverage by supported sprinkler ID. Note that sprinkler IDs may change after a save is loaded due to Json Assets reallocating IDs.</summary>
        public IDictionary<int, Vector2[]> GetSprinklerCoverage()
        {
            return this.SprinklerCoverage;
        }
    }
}
