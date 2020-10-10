/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/CJBok/SDV-Mods
**
*************************************************/

using StardewModdingAPI;

namespace CJBItemSpawner.Framework.Models
{
    /// <summary>The mod settings.</summary>
    internal class ModConfig
    {
        /// <summary>The key which opens the item spawner menu.</summary>
        public SButton ShowMenuKey { get; set; } = SButton.I;

        /// <summary>Whether to show items which may cause bugs or crashes when spawned.</summary>
        public bool AllowProblematicItems { get; set; } = false;
    }
}
