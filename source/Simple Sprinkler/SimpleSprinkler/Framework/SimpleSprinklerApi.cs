/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ADoby/SimpleSprinkler
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace SimpleSprinkler.Framework
{
    /// <summary>The API which provides access to Simple Sprinkler for other mods.</summary>
    public class SimpleSprinklerApi : ISimplerSprinklerApi
    {
        /*********
        ** Properties
        *********/
        /// <summary>The mod configuration.</summary>
        private readonly SimpleConfig Config;

        /// <summary>Encapsulates the logic for building sprinkler grids.</summary>
        private readonly GridHelper GridHelper;

        /// <summary>The cached sprinkler coverage.</summary>
        private IDictionary<int, Vector2[]> Coverage;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="config">The mod configuration.</param>
        /// <param name="gridHelper">Encapsulates the logic for building sprinkler grids.</param>
        internal SimpleSprinklerApi(SimpleConfig config, GridHelper gridHelper)
        {
            this.Config = config;
            this.GridHelper = gridHelper;
        }

        /// <summary>Get the relative tile coverage for supported sprinkler IDs (additive to the game's default coverage).</summary>
        public IDictionary<int, Vector2[]> GetNewSprinklerCoverage()
        {
            if (this.Coverage == null)
            {
                this.Coverage = new Dictionary<int, Vector2[]>();
                foreach (int sprinklerId in this.Config.Radius.Keys)
                    this.Coverage[sprinklerId] = this.GridHelper.GetGrid(sprinklerId, Vector2.Zero).ToArray();
            }

            return this.Coverage;
        }
    }
}
