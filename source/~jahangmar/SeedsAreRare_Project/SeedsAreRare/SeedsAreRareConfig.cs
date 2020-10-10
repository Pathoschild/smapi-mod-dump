/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jahangmar/StardewValleyMods
**
*************************************************/

using System;
namespace SeedsAreRare
{
    public class SeedsAreRareConfig     
    {
        public bool exclude_rare_seed { get; set; } = false;
        public bool exclude_traveling_merchant { get; set; } = false;
        public bool exclude_oasis { get; set; } = false;
        public bool exclude_pierre { get; set; } = false;
        public bool exclude_night_market { get; set; } = false;
        public bool exclude_egg_festival { get; set; } = false;
    }
}
