/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System;

namespace TehPers.FishingOverhaul.Api
{
    /// <summary>
    /// Type of water that fish can be caught in. Each location handles these values differently.
    /// </summary>
    [Flags]
    public enum WaterTypes
    {
        /// <summary>
        /// River water tiles.
        /// </summary>
        River = 0b1,

        /// <summary>
        /// Pond or ocean water tiles. They use the same ID.
        /// </summary>
        PondOrOcean = 0b10,

        /// <summary>
        /// Freshwater tiles.
        /// </summary>
        Freshwater = 0b100,

        /// <summary>
        /// No water tiles.
        /// </summary>
        None = 0,

        /// <summary>
        /// All water tiles.
        /// </summary>
        All = WaterTypes.River | WaterTypes.PondOrOcean | WaterTypes.Freshwater
    }
}