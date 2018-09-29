using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Cobalt.Framework
{
    /// <summary>The API which provides access to Cobalt for other mods.</summary>
    public class CobaltApi : ICobaltApi
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Get the cobalt sprinkler's object ID.</summary>
        public int GetBarId()
        {
            return CobaltBarItem.INDEX;
        }

        /// <summary>Get the cobalt sprinkler's object ID.</summary>
        public int GetSprinklerId()
        {
            return CobaltSprinklerObject.INDEX;
        }

        /// <summary>Get the cobalt sprinkler coverage.</summary>
        /// <param name="origin">The tile position containing the sprinkler.</param>
        public IEnumerable<Vector2> GetSprinklerCoverage(Vector2 origin)
        {
            return CobaltSprinklerObject.GetCoverage(Vector2.Zero);
        }
    }
}
