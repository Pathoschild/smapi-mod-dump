/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/GreenhouseGatherers
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseGatherers.GreenhouseGatherers.Models
{
    public class HarvestStatueData
    {
        public string GameLocation { get; set; }
        public Vector2 Tile { get; set; }

        public HarvestStatueData(string gameLocation, Vector2 tile)
        {
            this.GameLocation = gameLocation;
            this.Tile = tile;
        }
    }
}
