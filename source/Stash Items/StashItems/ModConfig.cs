/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zack-hill/stardew-valley-stash-items
**
*************************************************/

using StardewModdingAPI;

namespace StashItems
{
    internal class ModConfig
    {
        public int Radius { get; set; } = 5;
        public SButton StashHotKey { get; set; } = SButton.R;
    }
}
