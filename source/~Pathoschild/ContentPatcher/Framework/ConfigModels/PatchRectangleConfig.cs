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
    /// <summary>The input settings for a Rectangle field in <see cref="PatchConfig"/>.</summary>
    internal class PatchRectangleConfig
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The X position of the rectangle.</summary>
        public string X { get; set; }

        /// <summary>The Y position of the rectangle.</summary>
        public string Y { get; set; }

        /// <summary>The width of the rectangle.</summary>
        public string Width { get; set; }

        /// <summary>The height of the rectangle.</summary>
        public string Height { get; set; }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        public PatchRectangleConfig() { }

        /// <summary>Construct an instance.</summary>
        /// <param name="other">The other instance to copy.</param>
        public PatchRectangleConfig(PatchRectangleConfig other)
        {
            this.X = other.X;
            this.Y = other.Y;
            this.Width = other.Width;
            this.Height = other.Height;
        }
    }
}
