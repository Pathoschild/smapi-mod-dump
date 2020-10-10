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
using Microsoft.Xna.Framework;
using xTile;
using xTile.Layers;
using xTile.Display;
using xTile.Tiles;
using xTile.ObjectModel;
using StardewValley;

namespace FarmHouseRedone
{
    public class Patio
    {
        public Vector2 offset;
        public Map patioMap;

        //For testing purposes
        //Final should allow for selectors
        public string whichSpouse;

        public Patio(Vector2 offset, Map patioMap, string whichSpouse)
        {
            this.offset = offset;
            this.patioMap = patioMap;
            this.whichSpouse = whichSpouse;
        }

        public void pasteMap(GameLocation location, int pasteX, int pasteY)
        {
            Dictionary<TileSheet, TileSheet> equivalentSheets = MapUtilities.SheetHelper.getEquivalentSheets(location, patioMap);
            Vector2 mapSize = getMapSize(patioMap);
            for(int x = 0; x < mapSize.X; x++)
            {
                for(int y = 0; y < mapSize.Y; y++)
                {
                    MapUtilities.MapMerger.pasteTile(location.map, patioMap, x, y, pasteX + x, pasteY + y, equivalentSheets);
                }
            }
        }

        private Vector2 getMapSize(Map map)
        {
            return new Vector2(map.Layers[0].LayerWidth, map.Layers[0].LayerHeight);
        }

        public List<FarmerSprite.AnimationFrame> getAnimation()
        {
            Logger.Log("Getting patio animation for Patio_" + whichSpouse + "...");
            List<FarmerSprite.AnimationFrame> outFrames = new List<FarmerSprite.AnimationFrame>();
            if(patioMap == null || !patioMap.Properties.ContainsKey("Animation"))
            {
                return outFrames;
            }
            string[] framesProperty = Utility.cleanup(patioMap.Properties["Animation"]).Split(' ');
            for(int frame = 0; frame < framesProperty.Length - 1; frame += 2)
            {
                try
                {
                    int frameIndex = Convert.ToInt32(framesProperty[frame]);
                    int frameDuration = Convert.ToInt32(framesProperty[frame + 1]);
                    outFrames.Add(new FarmerSprite.AnimationFrame(frameIndex, frameDuration));
                }
                catch (FormatException)
                {
                    Logger.Log("Animation frame formatting incorrect!  Incorrect frame: index=" + framesProperty[frame] + ", duration=" + framesProperty[frame + 1] + ".\nFull animation string: " + Utility.cleanup(patioMap.Properties["Animation"]));
                }
            }
            Logger.Log("Added " + outFrames.Count + " frames");
            return outFrames;
        }
    }
}
