/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

namespace Pathoschild.Stardew.SmallBeachFarm.Framework.Config
{
    /// <summary>The mod configuration.</summary>
    internal class ModConfig
    {
        /// <summary>Whether to add a functional campfire in front of the farmhouse.</summary>
        public bool AddCampfire { get; set; } = true;

        /// <summary>Whether to add ocean islands with extra land area.</summary>
        public bool EnableIslands { get; set; } = false;

        /// <summary>Use the beach's background music (i.e. wave sounds) on the beach farm.</summary>
        public bool UseBeachMusic { get; set; } = false;
    }
}
