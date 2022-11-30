/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using System.Collections.Generic;
using xTile;

namespace StardewOpenWorld
{
    public class Biome
    {
        public Dictionary<Point, ObjData> terrainFeatures = new Dictionary<Point, ObjData>();
        public Dictionary<Point, ObjData> objects = new Dictionary<Point, ObjData>();
        public Dictionary<Point, ObjData> back = new Dictionary<Point, ObjData>();
        public Dictionary<Point, ObjData> buildings = new Dictionary<Point, ObjData>();
        public Dictionary<Point, ObjData> front = new Dictionary<Point, ObjData>();
        public Dictionary<Point, ObjData> alwaysFront = new Dictionary<Point, ObjData>();
    }

    public class TileData
    {
        public string tileSheet;
        public int index;
    }

    public class ObjData
    {
        public string type;
        public Dictionary<string, object> fields;
        public Dictionary<string, object> properties;
    }
}