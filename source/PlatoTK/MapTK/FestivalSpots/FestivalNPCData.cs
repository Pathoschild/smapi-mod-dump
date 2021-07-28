/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapTK.FestivalSpots
{
    internal class FestivalPlacementData
    {
        public string Festival { get; set; } = "summer11";
        public string Phase { get; set; } = "All";
        public int X { get; set; } = -1;
        public int Y { get; set; } = -1;
        public int Facing { get; set; } = 3;

    }

    internal class FestivalNPCData
    {
        public string NPC { get; set; } = "";
        public FestivalPlacementData[] Placements { get; set; } = new FestivalPlacementData[0];
    }
}
