/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/CJBok/SDV-Mods
**
*************************************************/

namespace CJBCheatsMenu.Framework.ContentModels
{
    /// <summary>The data for a section which groups warps in the UI.</summary>
    internal class WarpSectionContentModel
    {
        /// <summary>A unique string ID for this warp section.</summary>
        public string Id { get; set; } = "";

        /// <summary>The translated display name to show in the UI.</summary>
        public string DisplayName { get; set; } = "";

        /// <summary>The relative order in which to list it in the warp menu (default 0).</summary>
        public int Order { get; set; }
    }
}
