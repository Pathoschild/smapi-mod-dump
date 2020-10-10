/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

namespace ContentPatcher.Framework.ConfigModels
{
    /// <summary>The input settings for a tile position field in <see cref="PatchConfig"/>.</summary>
    internal class PatchPositionConfig
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The X position.</summary>
        public string X { get; set; }

        /// <summary>The Y position.</summary>
        public string Y { get; set; }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        public PatchPositionConfig() { }

        /// <summary>Construct an instance.</summary>
        /// <param name="other">The other instance to copy.</param>
        public PatchPositionConfig(PatchRectangleConfig other)
        {
            this.X = other.X;
            this.Y = other.Y;
        }
    }
}
