/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fishnets.Data
{
    [Obsolete("This seemed good at first, but saving both bait and offset to the location is actually pretty stupid, I'm not saving offset anymore and bait will now be stored to the relevant object")]
    public record ModData
    {
        public Vector2 Offset { get; set; }

        public string BaitId { get; set; } = "";

        public int BaitQuality { get; set; } = 0;

        public ModData() { }

        public ModData(Vector2 offset) => Offset = offset;

        public ModData(ModData old)
        {
            Offset = old.Offset;
            BaitId = old.BaitId;
            BaitQuality = old.BaitQuality;
        }

        public ModData SetBait(Object? bait)
        {
            BaitId = bait?.QualifiedItemId ?? "";
            BaitQuality = bait?.Quality ?? 0;
            return this;
        }
    }
}
