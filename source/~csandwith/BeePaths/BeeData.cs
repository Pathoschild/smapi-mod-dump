/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace BeePaths
{
    public class HiveData
    {
        public Vector2 hiveTile;
        public Vector2 cropTile;
        public List<BeeData> bees = new();
    }
    public class BeeData
    {
        public Vector2 pos;
        public Vector2 startPos;
        public Vector2 endPos;
        public Vector2 startTile;
        public Vector2 endTile;
        public float speed;
    }
}