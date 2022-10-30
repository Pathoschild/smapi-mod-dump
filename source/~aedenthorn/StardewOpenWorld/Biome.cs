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
        public bool unique;
        public Point location = new Point(-1, -1);
        public Dictionary<Point, ObjData> terrainFeatures = new Dictionary<Point, ObjData>();
        public Dictionary<Point, ObjData> objects = new Dictionary<Point, ObjData>();
        public string mapPath;
    }

    public class ObjData
    {
        public string type;
        public Dictionary<string, object> parameters;
        public Dictionary<string, object> fields;
        public Dictionary<string, object> properties;
    }
}