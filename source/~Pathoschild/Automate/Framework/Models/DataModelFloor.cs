/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

#nullable disable

namespace Pathoschild.Stardew.Automate.Framework.Models
{
    /// <summary>A floor to support by name in configuration.</summary>
    internal class DataModelFloor
    {
        /// <summary>The English name for the floor item.</summary>
        public string Name { get; set; }

        /// <summary>The item's unique ID.</summary>
        public int ItemId { get; set; }
    }
}
