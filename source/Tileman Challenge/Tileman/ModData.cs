/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spicykai/StardewValley-Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tileman
{
    class ModData

    {
        public bool ToPlaceTiles { get; set; }
        public bool DoCollision { get; set; }  
        
        public bool AllowPlayerPlacement { get; set; }
        public bool ToggleOverlay { get; set; }

        public double TilePrice { get; set; }
        public double TilePriceRaise { get; set; }

        public int CavernsExtra { get; set; }
        public int DifficultyMode { get; set; } 

        public int PurchaseCount { get; set; }

    }
}
