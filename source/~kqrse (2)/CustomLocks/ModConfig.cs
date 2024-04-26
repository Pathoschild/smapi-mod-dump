/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods-aedenthorn
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomLocks
{
    public class ModConfig
    {
        public bool Enabled { get; set; } = true;
        public bool AllowSeedShopWed { get; set; } = true;
        public bool AllowOutsideTime { get; set; } = true;
        public bool AllowStrangerHomeEntry { get; set; } = false;
        public bool AllowStrangerRoomEntry { get; set; } = false;
        public bool AllowAdventureGuildEntry { get; set; } = false;
        public bool IgnoreEvents { get; set; } = false;
    }
}
