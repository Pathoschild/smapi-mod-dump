/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

namespace DeluxeJournal.Framework.Data
{
    internal class BuildingIconData
    {
        /// <summary>Index of the building icon in the sprite sheet.</summary>
        public int SpriteIndex { get; set; }

        /// <summary>Building upgrade tier.</summary>
        /// <remarks>
        /// 0 = none,
        /// 1 = the base variant,
        /// 2 = the "Big" variant,
        /// 3 = the "Deluxe" variant.
        /// </remarks>
        public int Tier { get; set; }
    }
}
