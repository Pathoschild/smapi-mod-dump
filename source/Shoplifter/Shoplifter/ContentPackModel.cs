/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TheMightyAmondee/Shoplifter
**
*************************************************/

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shoplifter
{
    public class ContentPackModel
    {
        public string UniqueShopId { get; set; }
        public string ShopName { get; set; }
        public ShopCounterLocation CounterLocation { get; set; } = null;
        public List<string> ShopKeepers { get; set; } = new List<string>();
        public Dictionary<string, string> CaughtDialogue { get; set; } = null;
        public ShopliftableConditions OpenConditions { get; set; } = new ShopliftableConditions();
        public int MaxStockQuantity { get; set; } = 1;
        public int MaxStackPerItem { get; set; } = 1;
        public bool Bannable { get; set; } = false;      
        public string ContentModelPath { get; set; } // Content packs should not add this property themselves, this is determined by the mod
    }

    public class ShopliftableConditions
    {
        //public int OpenTime { get; set; } = -1;
        //public int CloseTime { get; set; } = -1;
        public List<string> Weather { get;set; } = null;
        //public List<int> DayOfSeason { get; set; } = null;
        //public List<string> Season { get; set; } = null;
        //public List<string> EventsSeen { get; set; } = null;
        public List<string> GameStateQueries { get; set; } = null;
        //public Dictionary<string,int> FriendshipLevels { get; set; } = null;
        public List<ShopKeeperConditions> ShopKeeperRange { get; set; } = null;

    }

    public class ShopKeeperConditions
    {
        public string Name { get; set; }
        public int TileX { get; set; }
        public int TileY { get; set; }
        public int Width { get; set; } = 1;
        public int Height { get; set; } = 1;
    }

    public class ShopCounterLocation
    {
        public string LocationName { get; set; }
        public bool NeedsShopProperty { get; set; } = true;
        public int TileX { get; set; } = -1;
        public int TileY { get; set; } = -1;

    }
}
