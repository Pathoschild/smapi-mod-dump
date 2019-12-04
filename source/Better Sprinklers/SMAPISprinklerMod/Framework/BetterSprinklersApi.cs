using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace BetterSprinklers.Framework
{
    /// <summary>The API which provides access to Better Sprinklers for other mods.</summary>
    public class BetterSprinklersApi : IBetterSprinklersApi
    {
        /*********
        ** Fields
        *********/
        /// <summary>The mod configuration.</summary>
        private readonly SprinklerModConfig Config;

        /// <summary>The maximum sprinkler coverage supported by this mod (in tiles wide or high).</summary>
        private readonly int MaxGridSize;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="config">The mod configuration.</param>
        /// <param name="maxGridSize">The maximum sprinkler coverage supported by this mod (in tiles wide or high).</param>
        internal BetterSprinklersApi(SprinklerModConfig config, int maxGridSize)
        {
            this.Config = config;
            this.MaxGridSize = maxGridSize;
        }

        /// <summary>Get the maximum sprinkler coverage supported by this mod (in tiles wide or high).</summary>
        public int GetMaxGridSize()
        {
            return this.MaxGridSize;
        }

        /// <summary>Get the relative tile coverage by supported sprinkler ID.</summary>
        public IDictionary<int, Vector2[]> GetSprinklerCoverage()
        {
            // build tile grids
            IDictionary<int, Vector2[]> coverage = new Dictionary<int, Vector2[]>();
            foreach (KeyValuePair<int, int[,]> shape in this.Config.SprinklerShapes)
                coverage[shape.Key] = GridHelper.GetCoveredTiles(Vector2.Zero, shape.Value).ToArray();
            return coverage;
        }
    }
}
