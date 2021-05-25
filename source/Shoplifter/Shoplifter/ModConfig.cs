/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TheMightyAmondee/Shoplifter
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shoplifter
{
    public class ModConfig
    {
        public uint MaxShopliftsPerDay { get; set; } = 1;
        public uint MaxShopliftsPerStore { get; set; } = 1;
        public uint MaxFine { get; set; } = 1000;
        public uint FriendshipPenalty { get; set; } = 500;
        public uint DaysBannedFor { get; set; } = 3;
        public uint CatchesBeforeBan { get; set; } = 3;
    }
}
