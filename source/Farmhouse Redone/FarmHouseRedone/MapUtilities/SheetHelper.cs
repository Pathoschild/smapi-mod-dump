/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mjSurber/FarmHouseRedone
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile;
using xTile.Tiles;
using StardewValley;
using Microsoft.Xna.Framework;

namespace FarmHouseRedone.MapUtilities
{
    public static class SheetHelper
    {
        public static Dictionary<TileSheet, TileSheet> getEquivalentSheets(GameLocation location, Map subMap)
        {
            Dictionary<TileSheet, TileSheet> equivalentSheets = new Dictionary<TileSheet, TileSheet>();
            foreach (TileSheet sheet in subMap.TileSheets)
            {
                foreach (TileSheet locSheet in location.map.TileSheets)
                {
                    if (cleanImageSource(locSheet.ImageSource).Equals(cleanImageSource(sheet.ImageSource)))
                    {
                        Logger.Log("Equivalent sheets: " + sheet.ImageSource + " and " + locSheet.ImageSource);
                        equivalentSheets[sheet] = locSheet;
                        break;
                    }
                    else
                    {
                        Logger.Log(cleanImageSource(locSheet.ImageSource) + " was not the same as " + cleanImageSource(sheet.ImageSource));
                    }
                }
                if (equivalentSheets.ContainsKey(sheet))
                    continue;
                else
                {
                    Logger.Log("No equivalent sheet found for " + sheet.ImageSource + "!  Using default (" + location.map.TileSheets[0].ImageSource + ")", StardewModdingAPI.LogLevel.Warn);
                    equivalentSheets[sheet] = location.map.TileSheets[0];
                }
            }
            return equivalentSheets;
        }

        public static string cleanImageSource(string source)
        {
            string[] seasons = { "spring", "summer", "fall", "winter" };
            if (source.Contains("\\"))
            {
                string[] path = source.Split('\\');
                source = path.Last();
            }
            if (source.Contains("/"))
            {
                string[] path = source.Split('/');
                source = path.Last();
            }
            if (source.Contains("_") && seasons.Contains(source.Split('_')[0].ToLower()))
            {
                source = source.Remove(0, (source.Split('_')[0]).Length + 1);
            }
            Logger.Log("Cleaned as " + source);
            return source;
        }

        public static int getTileSheet(Map map, string sourceToMatch)
        {
            for(int sheetIndex = 0; sheetIndex < map.TileSheets.Count; sheetIndex++)
            {
                TileSheet sheet = map.TileSheets[sheetIndex];
                if (sheet.ImageSource.Contains(sourceToMatch))
                    return sheetIndex;
            }
            Logger.Log("Couldn't find any tilesheet that matches the source \"" + sourceToMatch + "\"!  Using index 0.", StardewModdingAPI.LogLevel.Warn);
            return 0;
        }

        public static bool isTileOnSheet(Map map, Tile tile, int tileSheetToMatch)
        {
            return tile != null && tile.TileSheet.Equals((object)map.TileSheets[tileSheetToMatch]);
        }

        public static bool isTileOnSheet(Map map, string layer, int x, int y, int tileSheetToMatch)
        {
            return map.GetLayer(layer).Tiles[x, y] != null && map.GetLayer(layer).Tiles[x, y].TileSheet.Equals((object)map.TileSheets[tileSheetToMatch]);
        }

        public static bool isTileOnSheet(Map map, string layer, int x, int y, int tileSheetToMatch, Rectangle region)
        {
            return isTileOnSheet(map, layer, x, y, tileSheetToMatch) && isIndexInRect(map.GetLayer(layer).Tiles[x, y].TileIndex, map.TileSheets[tileSheetToMatch], region);
        }

        public static bool isIndexInRect(int index, TileSheet sheet, Rectangle region)
        {
            int x = index % sheet.SheetSize.Width;
            int y = (index / sheet.SheetSize.Width);

            return region.Contains(x, y);
        }
    }
}
