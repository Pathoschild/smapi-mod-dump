using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace LineSprinklers
{
    /// <summary>The API which provides access to Line Sprinklers for other mods.</summary>
    public interface ILineSprinklersApi
    {
        /// <summary>Get the maximum supported coverage width or height.</summary>
        int GetMaxGridSize();

        /// <summary>Get the relative tile coverage by supported sprinkler ID. Note that sprinkler IDs may change after a save is loaded due to Json Assets reallocating IDs.</summary>
        IDictionary<int, Vector2[]> GetSprinklerCoverage();
    }
}
