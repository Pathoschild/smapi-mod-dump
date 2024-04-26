/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods-aedenthorn
**
*************************************************/

using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace DynamicMapTiles
{
    public class DynamicTileInfo
    {
        public List<string> locations;
        public List<string> layers;
        public List<string> tileSheets;
        public List<string> tileSheetPaths;
        public List<int> indexes;
        public List<Rectangle> rectangles;
        public Dictionary<string, string> properties;
    }
}