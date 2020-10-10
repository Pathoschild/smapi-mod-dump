/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AairTheGreat/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterPanning
{
    public class MapOreConfig
    {
        public string AreaName { get; set; }
        public int numberOfOreSpotsPerDay { get; set; }
        public List<Point> OreSpots { get; set; }
    }
}
