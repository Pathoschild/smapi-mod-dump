/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Pathoschild.Stardew.CropsAnytimeAnywhere.Framework
{
    /// <summary>The mod configuration.</summary>
    internal class ModConfig
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The per-location settings.</summary>
        public IDictionary<string, PerLocationConfig> InLocations { get; set; } = new Dictionary<string, PerLocationConfig>
        {
            ["*"] = new PerLocationConfig
            {
                GrowCrops = true,
                GrowCropsOutOfSeason = true
            }
        };


        /*********
        ** Public methods
        *********/
        /// <summary>Normalize the model after it's deserialized.</summary>
        /// <param name="context">The deserialization context.</param>
        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
            this.InLocations ??= new Dictionary<string, PerLocationConfig>();

            foreach (var entry in this.InLocations.Values)
            {
                entry.ForceTillable ??= new ModConfigForceTillable
                {
                    Dirt = true,
                    Grass = true
                };
            }
        }
    }
}
