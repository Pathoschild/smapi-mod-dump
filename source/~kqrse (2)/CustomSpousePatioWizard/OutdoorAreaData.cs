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
using StardewValley;
using System;
using System.Collections.Generic;
using xTile.Dimensions;

namespace CustomSpousePatioWizard
{
    public class OutdoorAreaData
    {
        public Dictionary<string, OutdoorArea> areas = new Dictionary<string, OutdoorArea>();
        public Dictionary<string, TileSheetInfo> tileSheetsToAdd = new Dictionary<string, TileSheetInfo>();
    }

    public class TileSheetInfo
    {
        public string path;
        public string realPath;
        public int width;
        public int height;
        public int tileWidth;
        public int tileHeight;
    }

    public class OutdoorArea
    {
        public bool useDefaultTiles = false;
        public Point location = new Point(-1,-1);
        public List<SpecialTile> specialTiles = new List<SpecialTile>();
        public Point npcOffset = new Point(-1,-1);
        public string npcAnimation = null;
        public string useTilesOf = null;

        public Point GetLocation()
        {
            if (location.X != -1)
                return location;
            return Utility.Vector2ToPoint(Game1.getFarm().GetSpouseOutdoorAreaCorner());
        }
        public Point NpcPos(string name)
        {
            if (npcOffset.X != -1)
                return new Point(location.X + npcOffset.X, location.Y + +npcOffset.Y);
            if (ModEntry.customSpousePatioApi.GetCurrentSpouseAreas().ContainsKey(name))
                return new Point(location.X + ModEntry.customSpousePatioApi.GetDefaultSpouseOffsets()[name].X, location.Y + ModEntry.customSpousePatioApi.GetDefaultSpouseOffsets()[name].Y);
            return new Point(location.X + 2, location.Y + 4);
        }
    }
    public class SpecialTile
    {
        public Point location;
        public string layer;
        public int tileIndex;
        public string tilesheet;
    }
}