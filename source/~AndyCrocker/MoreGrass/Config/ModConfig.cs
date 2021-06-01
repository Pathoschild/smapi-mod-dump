/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using System.Collections.Generic;

namespace MoreGrass.Config
{
    /// <summary>The mod configuration.</summary>
    public class ModConfig
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The locations that More Grass is allowed to retexture grass in.</summary>
        public List<string> LocationsWhiteList { get; set; } = new List<string>();

        /// <summary>The locations that More Grass isn't allowed to retexture grass is.</summary>
        public List<string> LocationsBlackList { get; set; } = new List<string>() { "mine", "skullcave", "s:undergroundmine" };

        /// <summary>The percent of default grass coverage.</summary>
        /// <remarks>This is converage that's *explicitly* set to be default, this doesn't take into account when default grass is grouped in with custom grass.<br/>For example, if 6 grass sprites are loaded (3 custom, 3 default) with a 50% <see cref="PercentConverageOfDefaultGrass"/> then you can expect to see about 75% default grass, because half of the loaded grass sprites are default on top of the explicit 50%.</remarks>
        public int PercentConverageOfDefaultGrass { get; set; } = 0;

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
