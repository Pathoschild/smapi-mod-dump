/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/dantheman999301/StardewMods
**
*************************************************/

namespace VariableGrass.Framework
{
    /// <summary>The configuration settings.</summary>
    internal class ModConfig
    {
        /// <summary>The minimum grow iterations per day.</summary>
        public int MinIterations { get; set; }

        /// <summary>The maximum grow iterations per day.</summary>
        public int MaxIterations { get; set; } = 2;
    }
}
