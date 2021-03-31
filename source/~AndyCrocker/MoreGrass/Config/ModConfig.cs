/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

namespace MoreGrass.Config
{
    /// <summary>The mod configuration.</summary>
    public class ModConfig
    {
        /*********
        ** Accessors
        *********/
        /// <summary>Whether animals can eat grass.</summary>
        /// <remarks>This is mainly here so players can keep grass for aesthetic reasons, instead of animals eating it all.</remarks>
        public bool CanAnimalsEatGrass { get; set; } = true;

        /// <summary>Whether grass can live in spring.</summary>
        public bool CanGrassLiveInSpring { get; set; } = true;

        /// <summary>Whether grass can live in summer.</summary>
        public bool CanGrassLiveInSummer { get; set; } = true;

        /// <summary>Whether grass can live in fall.</summary>
        public bool CanGrassLiveInFall { get; set; } = true;

        /// <summary>Whether grass can live in winter.</summary>
        public bool CanGrassLiveInWinter { get; set; } = true;

        /// <summary>Whether grass can spread in spring.</summary>
        public bool CanGrassGrowInSpring { get; set; } = true;

        /// <summary>Whether grass can spread in summer.</summary>
        public bool CanGrassGrowInSummer { get; set; } = true;

        /// <summary>Whether grass can spread in fall.</summary>
        public bool CanGrassGrowInFall { get; set; } = true;

        /// <summary>Whether grass can spread in winter.</summary>
        public bool CanGrassGrowInWinter { get; set; } = false;
    }
}
