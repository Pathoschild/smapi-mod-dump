/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;

namespace Pathoschild.Stardew.DataLayers.Framework
{
    /// <summary>An entry to display in the overlay legend.</summary>
    internal class LegendEntry
    {
        /*********
        ** Accessors
        *********/
        /// <summary>A unique identifier for the legend entry.</summary>
        public string Id { get; }

        /// <summary>The entry name.</summary>
        public string Name { get; }

        /// <summary>The tile color.</summary>
        public Color Color { get; }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="id">A unique identifier for the legend entry.</param>
        /// <param name="name">The entry name.</param>
        /// <param name="color">The tile color.</param>
        public LegendEntry(string id, string name, Color color)
        {
            this.Id = id;
            this.Name = name;
            this.Color = color;
        }

        /// <summary>Construct an instance.</summary>
        /// <param name="id">The unique identifier and translation key the legend entry.</param>
        /// <param name="color">The tile color.</param>
        public LegendEntry(string id, Color color)
            : this(id, I18n.GetByKey(id), color) { }
    }
}
