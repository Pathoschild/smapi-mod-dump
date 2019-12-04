using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BetterSprinklers
{
    /// <summary>The API which provides access to Better Sprinklers for other mods.</summary>
    public interface IBetterSprinklersApi
    {
        /// <summary>Get the maximum sprinkler coverage supported by this mod (in tiles wide or high).</summary>
        int GetMaxGridSize();

        /// <summary>Get the relative tile coverage by supported sprinkler ID.</summary>
        IDictionary<int, Vector2[]> GetSprinklerCoverage();
    }
}