/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.Layers;
using xTile.Tiles;
using Microsoft.Xna.Framework;

namespace StardewDruid.Location
{
    public class Crypt : GameLocation
    {

        public Crypt() { }


        public Crypt(string Name)
            : base("Maps\\Mines\\8", Name) 
        {

        }

        public override void OnMapLoad(xTile.Map map)
        {

            map.AddTileSheet(new("quarry",map, "Maps\\Mines\\mine_quarryshaft", map.TileSheets[0].SheetSize, map.TileSheets[0].TileSize));

            TileSheet quarry = map.GetTileSheet("quarry");

            Layer back = map.GetLayer("Back");

            Layer building = map.GetLayer("Buildings");

            Layer front = map.GetLayer("Front");

            for (int i = 0; i < 50; i++)
            {
                
                for (int j = 0; j < 50; j++)
                {

                    if (back.Tiles[(int)i, (int)j] != null)
                    {
                        
                        if (back.Tiles[(int)i, (int)j].TileIndexProperties.TryGetValue("Type", out var typeValue))
                        {
                            
                            back.Tiles[(int)i, (int)j] = new StaticTile(back, quarry, BlendMode.Alpha, back.Tiles[(int)i, (int)j].TileIndex);
                            
                            back.Tiles[(int)i, (int)j].TileIndexProperties.Add("Type", typeValue);

                        }
                        else
                        {
                            back.Tiles[(int)i, (int)j] = new StaticTile(back, quarry, BlendMode.Alpha, back.Tiles[(int)i, (int)j].TileIndex);
                        }
  
                    }

                    if (building.Tiles[(int)i, (int)j] != null)
                    {

                        building.Tiles[(int)i, (int)j] = new StaticTile(building, quarry, BlendMode.Alpha, building.Tiles[(int)i, (int)j].TileIndex);

                    }

                    if (front.Tiles[(int)i, (int)j] != null)
                    {

                        front.Tiles[(int)i, (int)j] = new StaticTile(front, quarry, BlendMode.Alpha, front.Tiles[(int)i, (int)j].TileIndex);

                    }

                    if ((i - 5) % 10 == 0 && (j - 5) % 10 == 0)
                    {

                        sharedLights[j + i * 999] = new LightSource(4, new Vector2(j, i - 2) * 64f + new Vector2(32f, 0f), 4f, new Color(0, 20, 50), j + i * 999, LightSource.LightContext.None, 0L);

                    }

                }

            }

            int floor = back.Tiles[11, 11].TileIndex;

            for (int i = 14; i < 28; i++)
            {
                for (int j = 8; j < 10; j++)
                {

                    back.Tiles[(int)i, (int)j] = new StaticTile(back, quarry, BlendMode.Alpha, floor);

                    back.Tiles[(int)i, (int)j].TileIndexProperties.Add("Type", "Stone");

                    building.Tiles[(int)i, (int)j] = null;

                    front.Tiles[(int)i, (int)j] = null;

                }

            }

            for (int i = 11; i < 31; i++)
            {
                for (int j = 10; j < 13; j++)
                {

                    back.Tiles[(int)i, (int)j] = new StaticTile(back, quarry, BlendMode.Alpha, floor);

                    back.Tiles[(int)i, (int)j].TileIndexProperties.Add("Type", "Stone");

                    building.Tiles[(int)i, (int)j] = null;

                    front.Tiles[(int)i, (int)j] = null;

                }

            }

            for (int i = 8; i < 31; i++)
            {
                for (int j = 13; j < 18; j++)
                {

                    back.Tiles[(int)i, (int)j] = new StaticTile(back, quarry, BlendMode.Alpha, floor);

                    back.Tiles[(int)i, (int)j].TileIndexProperties.Add("Type", "Stone");

                    building.Tiles[(int)i, (int)j] = null;

                    front.Tiles[(int)i, (int)j] = null;

                }

            }

            for (int i = 8; i < 31; i++)
            {
                for (int j = 18; j < 28; j++)
                {

                    back.Tiles[(int)i, (int)j] = new StaticTile(back, quarry, BlendMode.Alpha, floor);

                    back.Tiles[(int)i, (int)j].TileIndexProperties.Add("Type", "Stone");

                    building.Tiles[(int)i, (int)j] = null;

                    front.Tiles[(int)i, (int)j] = null;

                }

            }

            for (int i = 13; i < 28; i++)
            {
                for (int j = 28; j < 30; j++)
                {

                    back.Tiles[(int)i, (int)j] = new StaticTile(back, quarry, BlendMode.Alpha, floor);

                    back.Tiles[(int)i, (int)j].TileIndexProperties.Add("Type", "Stone");

                    building.Tiles[(int)i, (int)j] = null;

                    front.Tiles[(int)i, (int)j] = null;

                }

            }

        }

        public override void updateWarps()
        {

            warps.Clear();

            warps.Add(new Warp(20,5,"Town",47,88, flipFarmer: false));

        }

    }

}
