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

namespace CustomWallpaperFramework
{
    public class WallPaperTileData
    {
        public string id;
        public Vector2 startTile = new Vector2(-1, -1);
        public List<Vector2> backTiles = new List<Vector2>();
        public List<Vector2> buildingTiles = new List<Vector2>();
    }
}