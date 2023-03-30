/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using System;
using System.Linq;
using Circuit.VirtualProperties;

namespace Circuit.Events
{
    public class BountifulHarvest : EventBase
    {
        public static int[] HarvestIndices { get; } = new int[]
        {
            24, 188, 190, 192, 248, 271, 830, 250, 252, 400,
            254, 256, 258, 260, 262, 264, 266, 268, 832, 433,
            90, 270, 304, 398, 300, 272, 274, 276, 278, 280,
            282, 284, 454, 591, 597, 376, 593, 421, 595, 417,
            16, 396, 404, 412, 771, 889, 402, 18, 418
        };

        public BountifulHarvest(EventType eventType) : base(eventType) { }

        public override string GetDisplayName()
        {
            return "Bountiful Harvest";
        }

        public override string GetChatWarningMessage()
        {
            return "The harvest is looking like it will be good...";
        }

        public override string GetChatStartMessage()
        {
            return "The harvest is bountiful!";
        }

        public override string GetDescription()
        {
            return "Harvested crops automatically increase in quality by 1.";
        }
    }
}
