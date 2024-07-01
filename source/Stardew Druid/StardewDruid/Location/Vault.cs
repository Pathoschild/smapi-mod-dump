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
using StardewModdingAPI;
using StardewValley.Buildings;
using StardewValley.Objects;
using System.Runtime.Intrinsics.X86;
using StardewValley.GameData.Locations;
using Microsoft.Xna.Framework.Graphics;
using StardewDruid.Data;
using StardewValley.TerrainFeatures;
using System.Threading;
using xTile;
using StardewDruid.Character;
using static StardewDruid.Cast.Rite;
using StardewDruid.Cast;
using System.Reflection.Emit;

namespace StardewDruid.Location
{
    public class Vault : GameLocation
    {

        public List<Location.WarpTile> warpSets = new();

        public List<Location.LocationTile> locationTiles = new();

        public Dictionary<Vector2, CharacterHandle.characters> dialogueTiles = new();

        public Dictionary<Vector2, int> lightFields = new();

        public Dictionary<Vector2, int> darkFields = new();

        public Dictionary<Vector2, int> crateTiles = new();

        public bool ambientDarkness;

        public Texture2D dungeonTexture;

        public Vault() { }

        public Vault(string Name)
            : base("Maps\\Shed", Name)
        {

        }

        public override void draw(SpriteBatch b)
        {

            base.draw(b);

            foreach (KeyValuePair<Vector2, int> light in lightFields)
            {

                if (Utility.isOnScreen(light.Key, 64 * light.Value))
                {

                    Texture2D texture2D = Game1.sconceLight;

                    Microsoft.Xna.Framework.Vector2 position = new(light.Key.X - (float)Game1.viewport.X, light.Key.Y - (float)Game1.viewport.Y);

                    b.Draw(
                        texture2D,
                        position,
                        texture2D.Bounds,
                        Microsoft.Xna.Framework.Color.LightCoral * 0.75f,
                        0f,
                        new Vector2(texture2D.Bounds.Width / 2, texture2D.Bounds.Height / 2),
                        0.25f * light.Value,
                        SpriteEffects.None,
                        0.9f
                    );

                }

            }

            foreach (KeyValuePair<Vector2, int> crate in crateTiles)
            {

                if(crate.Value < 0)
                {

                    if(crate.Value == -1)
                    {

                        Vector2 crateOpen = crate.Key * 64;

                        if (Utility.isOnScreen(crateOpen, 64))
                        {
                            
                            Microsoft.Xna.Framework.Vector2 position = new(crateOpen.X - (float)Game1.viewport.X, crateOpen.Y - (float)Game1.viewport.Y - 64);

                            b.Draw(
                                Mod.instance.iconData.crateTexture,
                                position,
                                new(64, 0, 32, 64),
                                Color.White,
                                0f,
                                Vector2.Zero,
                                2f,
                                SpriteEffects.None,
                                crateOpen.Y / 10000
                            );
                        
                        }

                    }
                    else
                    {

                        crateTiles[crate.Key] += 1;

                    }

                    continue;

                }

                Vector2 cratePosition = crate.Key * 64;

                if (Utility.isOnScreen(cratePosition, 64))
                {

                    Microsoft.Xna.Framework.Vector2 position = new(cratePosition.X - (float)Game1.viewport.X, cratePosition.Y - (float)Game1.viewport.Y - 64);

                    b.Draw(
                        Mod.instance.iconData.crateTexture,
                        position,
                        new(0,0,32,64),
                        Color.White,
                        0f,
                        Vector2.Zero,
                        2f,
                        SpriteEffects.None,
                        cratePosition.Y / 10000
                    );

                }

            }

        }

        public override void drawWaterTile(SpriteBatch b, int x, int y)
        {

            bool num = y == map.Layers[0].LayerHeight - 1 || !waterTiles[x, y + 1];
            bool flag = y == 0 || !waterTiles[x, y - 1];
            int num2 = 0;
            int num3 = 320;
            b.Draw(dungeonTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, y * 64 - (int)((!flag) ? waterPosition : 0f))), new Microsoft.Xna.Framework.Rectangle(num2 + waterAnimationIndex * 16, num3 + (((x + y) % 2 != 0) ? ((!waterTileFlip) ? 32 : 0) : (waterTileFlip ? 32 : 0)) + (flag ? ((int)waterPosition / 4) : 0), 16, 16 + (flag ? ((int)(0f - waterPosition) / 4) : 0)), waterColor.Value, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.56f);
            if (num)
            {
                b.Draw(dungeonTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, (y + 1) * 64 - (int)waterPosition)), new Microsoft.Xna.Framework.Rectangle(num2 + waterAnimationIndex * 16, num3 + (((x + (y + 1)) % 2 != 0) ? ((!waterTileFlip) ? 32 : 0) : (waterTileFlip ? 32 : 0)), 16, 16 - (int)(16f - waterPosition / 4f) - 1), waterColor.Value, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.56f);
            }
        }

        public override void OnMapLoad(xTile.Map map)
        {
            /*
             
             225
             250
             275
             300
             
             */

            xTile.Dimensions.Size tileSize = map.GetLayer("Back").TileSize;

            xTile.Map newMap = new(map.Id);

            Layer back = new("Back", newMap, new(56, 34), tileSize);

            newMap.AddLayer(back);

            Layer buildings = new("Buildings", newMap, new(56, 34), tileSize);

            newMap.AddLayer(buildings);

            Layer front = new("Front", newMap, new(56, 34), tileSize);

            newMap.AddLayer(front);

            Layer alwaysfront = new("AlwaysFront", newMap, new(56, 34), tileSize);

            newMap.AddLayer(alwaysfront);

            TileSheet outdoor = new(LocationData.druid_vault_name + "_dungeon", newMap, "Maps\\Mines\\volcano_dungeon", new(16, 36), tileSize);

            newMap.AddTileSheet(outdoor); //map.TileSheets[1].ImageSource

            locationTiles = new();

            waterTiles = new(56, 34);

            Dictionary<int, List<List<int>>> codes = new()
            {
                [0] = new() { new() { 0, 1 }, new() { 1, 1 }, new() { 2, 19 }, new() { 3, 1 }, new() { 4, 1 }, new() { 5, 1 }, new() { 6, 1 }, new() { 7, 1 }, new() { 8, 1 }, new() { 9, 1 }, new() { 10, 1 }, new() { 11, 1 }, new() { 12, 1 }, new() { 13, 1 }, new() { 14, 1 }, new() { 15, 1 }, new() { 16, 1 }, new() { 17, 1 }, new() { 18, 3 }, new() { 19, 1 }, new() { 20, 1 }, new() { 21, 1 }, new() { 22, 1 }, new() { 23, 1 }, new() { 24, 17 }, new() { 25, 1 }, new() { 26, 17 }, new() { 27, 1 }, new() { 28, 17 }, new() { 29, 1 }, new() { 30, 1 }, new() { 31, 1 }, new() { 32, 1 }, new() { 33, 1 }, new() { 34, 18 }, new() { 35, 1 }, new() { 36, 1 }, new() { 37, 17 }, new() { 38, 18 }, new() { 39, 1 }, new() { 40, 1 }, new() { 41, 1 }, new() { 42, 1 }, new() { 43, 1 }, new() { 44, 2 }, new() { 45, 1 }, new() { 46, 1 }, new() { 47, 1 }, new() { 48, 1 }, new() { 49, 1 }, new() { 50, 1 }, new() { 51, 1 }, new() { 52, 1 }, new() { 53, 18 }, new() { 54, 1 }, new() { 55, 2 }, },
                [1] = new() { new() { 0, 2 }, new() { 1, 1 }, new() { 2, 1 }, new() { 3, 1 }, new() { 4, 19 }, new() { 5, 1 }, new() { 6, 1 }, new() { 7, 3 }, new() { 8, 1 }, new() { 9, 1 }, new() { 10, 17 }, new() { 11, 17 }, new() { 12, 1 }, new() { 13, 1 }, new() { 14, 3 }, new() { 15, 1 }, new() { 16, 1 }, new() { 17, 1 }, new() { 18, 1 }, new() { 19, 1 }, new() { 20, 1 }, new() { 21, 18 }, new() { 22, 18 }, new() { 23, 1 }, new() { 24, 1 }, new() { 25, 1 }, new() { 26, 1 }, new() { 27, 1 }, new() { 28, 1 }, new() { 29, 1 }, new() { 30, 1 }, new() { 31, 1 }, new() { 32, 1 }, new() { 33, 1 }, new() { 34, 1 }, new() { 35, 1 }, new() { 36, 1 }, new() { 37, 2 }, new() { 38, 1 }, new() { 39, 1 }, new() { 40, 1 }, new() { 41, 1 }, new() { 42, 1 }, new() { 43, 1 }, new() { 44, 1 }, new() { 45, 1 }, new() { 46, 1 }, new() { 47, 2 }, new() { 48, 1 }, new() { 49, 1 }, new() { 50, 1 }, new() { 51, 2 }, new() { 52, 1 }, new() { 53, 18 }, new() { 54, 1 }, new() { 55, 1 }, },
                [2] = new() { new() { 0, 1 }, new() { 1, 1 }, new() { 2, 1 }, new() { 3, 1 }, new() { 4, 1 }, new() { 5, 1 }, new() { 6, 1 }, new() { 7, 1 }, new() { 8, 1 }, new() { 9, 18 }, new() { 10, 1 }, new() { 11, 1 }, new() { 12, 1 }, new() { 13, 1 }, new() { 14, 1 }, new() { 15, 3 }, new() { 16, 1 }, new() { 17, 1 }, new() { 18, 19 }, new() { 19, 1 }, new() { 20, 18 }, new() { 21, 1 }, new() { 22, 3 }, new() { 23, 1 }, new() { 24, 19 }, new() { 25, 2 }, new() { 26, 1 }, new() { 27, 3 }, new() { 28, 3 }, new() { 29, 1 }, new() { 30, 1 }, new() { 31, 1 }, new() { 32, 1 }, new() { 33, 1 }, new() { 34, 18 }, new() { 35, 1 }, new() { 36, 1 }, new() { 37, 1 }, new() { 38, 19 }, new() { 39, 1 }, new() { 40, 18 }, new() { 41, 1 }, new() { 42, 1 }, new() { 43, 1 }, new() { 44, 17 }, new() { 45, 1 }, new() { 46, 1 }, new() { 47, 1 }, new() { 48, 1 }, new() { 49, 17 }, new() { 50, 2 }, new() { 51, 1 }, new() { 52, 1 }, new() { 53, 1 }, new() { 54, 1 }, new() { 55, 1 }, },
                [3] = new() { new() { 0, 1 }, new() { 1, 17 }, new() { 2, 1 }, new() { 3, 2 }, new() { 4, 3 }, new() { 5, 1 }, new() { 6, 1 }, new() { 7, 1 }, new() { 8, 1 }, new() { 9, 1 }, new() { 10, 1 }, new() { 11, 3 }, new() { 12, 1 }, new() { 13, 1 }, new() { 14, 1 }, new() { 15, 1 }, new() { 16, 1 }, new() { 17, 1 }, new() { 18, 1 }, new() { 19, 1 }, new() { 20, 1 }, new() { 21, 1 }, new() { 22, 1 }, new() { 23, 1 }, new() { 24, 1 }, new() { 25, 1 }, new() { 26, 1 }, new() { 27, 1 }, new() { 28, 19 }, new() { 29, 1 }, new() { 30, 1 }, new() { 31, 18 }, new() { 32, 18 }, new() { 33, 17 }, new() { 34, 1 }, new() { 35, 1 }, new() { 36, 1 }, new() { 37, 17 }, new() { 38, 1 }, new() { 39, 1 }, new() { 40, 1 }, new() { 41, 19 }, new() { 42, 1 }, new() { 43, 1 }, new() { 44, 1 }, new() { 45, 1 }, new() { 46, 1 }, new() { 47, 18 }, new() { 48, 1 }, new() { 49, 1 }, new() { 50, 1 }, new() { 51, 1 }, new() { 52, 1 }, new() { 53, 18 }, new() { 54, 18 }, new() { 55, 18 }, },
                [4] = new() { new() { 0, 18 }, new() { 1, 1 }, new() { 2, 2 }, new() { 3, 1 }, new() { 4, 1 }, new() { 5, 1 }, new() { 6, 1 }, new() { 7, 1 }, new() { 8, 1 }, new() { 9, 17 }, new() { 10, 1 }, new() { 11, 1 }, new() { 12, 1 }, new() { 13, 1 }, new() { 14, 1 }, new() { 15, 1 }, new() { 16, 19 }, new() { 17, 3 }, new() { 18, 1 }, new() { 19, 1 }, new() { 20, 1 }, new() { 21, 1 }, new() { 22, 1 }, new() { 23, 1 }, new() { 24, 1 }, new() { 25, 1 }, new() { 26, 1 }, new() { 27, 1 }, new() { 28, 1 }, new() { 29, 19 }, new() { 30, 1 }, new() { 31, 1 }, new() { 32, 19 }, new() { 33, 1 }, new() { 34, 1 }, new() { 35, 1 }, new() { 36, 1 }, new() { 37, 1 }, new() { 38, 1 }, new() { 39, 19 }, new() { 40, 1 }, new() { 41, 1 }, new() { 42, 3 }, new() { 43, 3 }, new() { 44, 1 }, new() { 45, 1 }, new() { 46, 18 }, new() { 47, 1 }, new() { 48, 1 }, new() { 49, 1 }, new() { 50, 3 }, new() { 51, 1 }, new() { 52, 1 }, new() { 53, 17 }, new() { 54, 3 }, new() { 55, 1 }, },
                [5] = new() { new() { 0, 1 }, new() { 1, 1 }, new() { 2, 2 }, new() { 3, 17 }, new() { 4, 1 }, new() { 5, 1 }, new() { 6, 1 }, new() { 7, 1 }, new() { 8, 1 }, new() { 9, 17 }, new() { 10, 1 }, new() { 11, 1 }, new() { 12, 2 }, new() { 13, 18 }, new() { 14, 1 }, new() { 15, 1 }, new() { 16, 17 }, new() { 17, 1 }, new() { 18, 1 }, new() { 19, 3 }, new() { 20, 1 }, new() { 21, 1 }, new() { 22, 1 }, new() { 23, 1 }, new() { 24, 1 }, new() { 25, 1 }, new() { 26, 1 }, new() { 27, 3 }, new() { 28, 1 }, new() { 29, 2 }, new() { 30, 1 }, new() { 31, 1 }, new() { 32, 1 }, new() { 33, 1 }, new() { 34, 1 }, new() { 35, 1 }, new() { 36, 2 }, new() { 37, 3 }, new() { 38, 19 }, new() { 39, 1 }, new() { 40, 1 }, new() { 41, 19 }, new() { 42, 1 }, new() { 43, 1 }, new() { 44, 18 }, new() { 45, 1 }, new() { 46, 17 }, new() { 47, 1 }, new() { 48, 1 }, new() { 49, 17 }, new() { 50, 1 }, new() { 51, 1 }, new() { 52, 3 }, new() { 53, 1 }, new() { 54, 1 }, new() { 55, 1 }, },
                [6] = new() { new() { 0, 3 }, new() { 1, 1 }, new() { 2, 17 }, new() { 3, 1 }, new() { 4, 1 }, new() { 5, 18 }, new() { 6, 1 }, new() { 7, 1 }, new() { 8, 1 }, new() { 9, 1 }, new() { 10, 1 }, new() { 11, 2 }, new() { 12, 1 }, new() { 13, 1 }, new() { 14, 1 }, new() { 15, 1 }, new() { 16, 1 }, new() { 17, 1 }, new() { 18, 1 }, new() { 19, 1 }, new() { 20, 1 }, new() { 21, 1 }, new() { 22, 1 }, new() { 23, 1 }, new() { 24, 1 }, new() { 25, 1 }, new() { 26, 1 }, new() { 27, 17 }, new() { 28, 1 }, new() { 29, 1 }, new() { 30, 1 }, new() { 31, 1 }, new() { 32, 3 }, new() { 33, 3 }, new() { 34, 1 }, new() { 35, 1 }, new() { 36, 1 }, new() { 37, 1 }, new() { 38, 2 }, new() { 39, 1 }, new() { 40, 17 }, new() { 41, 1 }, new() { 42, 1 }, new() { 43, 3 }, new() { 44, 1 }, new() { 45, 1 }, new() { 46, 1 }, new() { 47, 1 }, new() { 48, 19 }, new() { 49, 1 }, new() { 50, 1 }, new() { 51, 19 }, new() { 52, 1 }, new() { 53, 1 }, new() { 54, 1 }, new() { 55, 19 }, },
                [7] = new() { new() { 0, 2 }, new() { 1, 1 }, new() { 2, 1 }, new() { 3, 17 }, new() { 4, 2 }, new() { 5, 18 }, new() { 6, 1 }, new() { 7, 17 }, new() { 8, 1 }, new() { 9, 1 }, new() { 10, 1 }, new() { 11, 1 }, new() { 12, 1 }, new() { 13, 19 }, new() { 14, 3 }, new() { 15, 2 }, new() { 16, 1 }, new() { 17, 3 }, new() { 18, 1 }, new() { 19, 18 }, new() { 20, 1 }, new() { 21, 1 }, new() { 22, 1 }, new() { 23, 1 }, new() { 24, 1 }, new() { 25, 1 }, new() { 26, 18 }, new() { 27, 1 }, new() { 28, 1 }, new() { 29, 1 }, new() { 30, 1 }, new() { 31, 1 }, new() { 32, 1 }, new() { 33, 1 }, new() { 34, 1 }, new() { 35, 1 }, new() { 36, 1 }, new() { 37, 1 }, new() { 38, 1 }, new() { 39, 1 }, new() { 40, 1 }, new() { 41, 1 }, new() { 42, 19 }, new() { 43, 1 }, new() { 44, 2 }, new() { 45, 1 }, new() { 46, 19 }, new() { 47, 1 }, new() { 48, 1 }, new() { 49, 18 }, new() { 50, 1 }, new() { 51, 19 }, new() { 52, 1 }, new() { 53, 1 }, new() { 54, 1 }, new() { 55, 18 }, },
                [8] = new() { new() { 0, 1 }, new() { 1, 1 }, new() { 2, 1 }, new() { 3, 1 }, new() { 4, 18 }, new() { 5, 17 }, new() { 6, 1 }, new() { 7, 1 }, new() { 8, 1 }, new() { 9, 1 }, new() { 10, 1 }, new() { 11, 1 }, new() { 12, 1 }, new() { 13, 17 }, new() { 14, 1 }, new() { 15, 1 }, new() { 16, 1 }, new() { 17, 1 }, new() { 18, 1 }, new() { 19, 1 }, new() { 20, 1 }, new() { 21, 2 }, new() { 22, 2 }, new() { 23, 1 }, new() { 24, 1 }, new() { 25, 1 }, new() { 26, 1 }, new() { 27, 1 }, new() { 28, 1 }, new() { 29, 1 }, new() { 30, 1 }, new() { 31, 1 }, new() { 32, 1 }, new() { 33, 19 }, new() { 34, 1 }, new() { 35, 2 }, new() { 36, 1 }, new() { 37, 1 }, new() { 38, 1 }, new() { 39, 1 }, new() { 40, 1 }, new() { 41, 1 }, new() { 42, 1 }, new() { 43, 3 }, new() { 44, 1 }, new() { 45, 1 }, new() { 46, 1 }, new() { 47, 1 }, new() { 48, 1 }, new() { 49, 18 }, new() { 50, 1 }, new() { 51, 1 }, new() { 52, 1 }, new() { 53, 18 }, new() { 54, 3 }, new() { 55, 1 }, },
                [9] = new() { new() { 0, 1 }, new() { 1, 1 }, new() { 2, 1 }, new() { 3, 1 }, new() { 4, 1 }, new() { 5, 1 }, new() { 6, 1 }, new() { 7, 1 }, new() { 8, 1 }, new() { 9, 1 }, new() { 10, 1 }, new() { 11, 19 }, new() { 12, 3 }, new() { 13, 1 }, new() { 14, 4 }, new() { 15, 4 }, new() { 16, 4 }, new() { 17, 4 }, new() { 18, 4 }, new() { 19, 19 }, new() { 20, 3 }, new() { 21, 1 }, new() { 22, 1 }, new() { 23, 18 }, new() { 24, 1 }, new() { 25, 1 }, new() { 26, 1 }, new() { 27, 1 }, new() { 28, 1 }, new() { 29, 1 }, new() { 30, 1 }, new() { 31, 1 }, new() { 32, 19 }, new() { 33, 1 }, new() { 34, 1 }, new() { 35, 3 }, new() { 36, 1 }, new() { 37, 1 }, new() { 38, 1 }, new() { 39, 3 }, new() { 40, 18 }, new() { 41, 3 }, new() { 42, 1 }, new() { 43, 1 }, new() { 44, 1 }, new() { 45, 1 }, new() { 46, 1 }, new() { 47, 18 }, new() { 48, 1 }, new() { 49, 18 }, new() { 50, 1 }, new() { 51, 18 }, new() { 52, 1 }, new() { 53, 19 }, new() { 54, 1 }, new() { 55, 1 }, },
                [10] = new() { new() { 0, 1 }, new() { 1, 1 }, new() { 2, 1 }, new() { 3, 1 }, new() { 4, 1 }, new() { 5, 1 }, new() { 6, 2 }, new() { 7, 17 }, new() { 8, 19 }, new() { 9, 1 }, new() { 10, 1 }, new() { 11, 1 }, new() { 12, 17 }, new() { 13, 4 }, new() { 14, 4 }, new() { 15, 4 }, new() { 16, 4 }, new() { 17, 4 }, new() { 18, 4 }, new() { 19, 1 }, new() { 20, 17 }, new() { 21, 1 }, new() { 22, 19 }, new() { 23, 1 }, new() { 24, 1 }, new() { 25, 1 }, new() { 26, 1 }, new() { 27, 1 }, new() { 28, 1 }, new() { 29, 3 }, new() { 30, 1 }, new() { 31, 1 }, new() { 32, 1 }, new() { 33, 1 }, new() { 34, 1 }, new() { 35, 1 }, new() { 36, 1 }, new() { 37, 1 }, new() { 38, 1 }, new() { 39, 1 }, new() { 40, 1 }, new() { 41, 1 }, new() { 42, 1 }, new() { 43, 1 }, new() { 44, 18 }, new() { 45, 1 }, new() { 46, 1 }, new() { 47, 19 }, new() { 48, 1 }, new() { 49, 1 }, new() { 50, 17 }, new() { 51, 1 }, new() { 52, 1 }, new() { 53, 1 }, new() { 54, 2 }, new() { 55, 1 }, },
                [11] = new() { new() { 0, 3 }, new() { 1, 1 }, new() { 2, 19 }, new() { 3, 1 }, new() { 4, 1 }, new() { 5, 1 }, new() { 6, 1 }, new() { 7, 2 }, new() { 8, 1 }, new() { 9, 1 }, new() { 10, 1 }, new() { 11, 1 }, new() { 12, 1 }, new() { 13, 4 }, new() { 14, 4 }, new() { 15, 4 }, new() { 16, 4 }, new() { 17, 4 }, new() { 18, 4 }, new() { 19, 1 }, new() { 20, 1 }, new() { 21, 1 }, new() { 22, 1 }, new() { 23, 1 }, new() { 24, 1 }, new() { 25, 1 }, new() { 26, 1 }, new() { 27, 1 }, new() { 28, 1 }, new() { 29, 1 }, new() { 30, 1 }, new() { 31, 1 }, new() { 32, 1 }, new() { 33, 1 }, new() { 34, 1 }, new() { 35, 1 }, new() { 36, 1 }, new() { 37, 1 }, new() { 38, 1 }, new() { 39, 1 }, new() { 40, 1 }, new() { 41, 3 }, new() { 42, 1 }, new() { 43, 1 }, new() { 44, 1 }, new() { 45, 18 }, new() { 46, 1 }, new() { 47, 1 }, new() { 48, 1 }, new() { 49, 3 }, new() { 50, 1 }, new() { 51, 1 }, new() { 52, 1 }, new() { 53, 1 }, new() { 54, 1 }, new() { 55, 1 }, },
                [12] = new() { new() { 0, 2 }, new() { 1, 1 }, new() { 2, 1 }, new() { 3, 1 }, new() { 4, 1 }, new() { 5, 18 }, new() { 6, 1 }, new() { 7, 1 }, new() { 8, 19 }, new() { 9, 1 }, new() { 10, 1 }, new() { 11, 1 }, new() { 12, 17 }, new() { 13, 4 }, new() { 14, 4 }, new() { 15, 4 }, new() { 16, 4 }, new() { 17, 4 }, new() { 18, 1 }, new() { 19, 1 }, new() { 20, 17 }, new() { 21, 1 }, new() { 22, 1 }, new() { 23, 1 }, new() { 24, 1 }, new() { 25, 1 }, new() { 26, 1 }, new() { 27, 18 }, new() { 28, 18 }, new() { 29, 1 }, new() { 30, 1 }, new() { 31, 3 }, new() { 32, 1 }, new() { 33, 1 }, new() { 34, 1 }, new() { 35, 1 }, new() { 36, 1 }, new() { 37, 1 }, new() { 38, 1 }, new() { 39, 1 }, new() { 40, 1 }, new() { 41, 17 }, new() { 42, 1 }, new() { 43, 1 }, new() { 44, 1 }, new() { 45, 1 }, new() { 46, 1 }, new() { 47, 19 }, new() { 48, 1 }, new() { 49, 1 }, new() { 50, 1 }, new() { 51, 1 }, new() { 52, 1 }, new() { 53, 1 }, new() { 54, 1 }, new() { 55, 1 }, },
                [13] = new() { new() { 0, 1 }, new() { 1, 1 }, new() { 2, 1 }, new() { 3, 1 }, new() { 4, 1 }, new() { 5, 1 }, new() { 6, 1 }, new() { 7, 1 }, new() { 8, 1 }, new() { 9, 1 }, new() { 10, 1 }, new() { 11, 17 }, new() { 12, 4 }, new() { 13, 4 }, new() { 14, 4 }, new() { 15, 4 }, new() { 16, 4 }, new() { 17, 1 }, new() { 18, 1 }, new() { 19, 17 }, new() { 20, 1 }, new() { 21, 1 }, new() { 22, 1 }, new() { 23, 1 }, new() { 24, 1 }, new() { 25, 1 }, new() { 26, 1 }, new() { 27, 1 }, new() { 28, 1 }, new() { 29, 18 }, new() { 30, 1 }, new() { 31, 1 }, new() { 32, 1 }, new() { 33, 1 }, new() { 34, 1 }, new() { 35, 1 }, new() { 36, 1 }, new() { 37, 18 }, new() { 38, 19 }, new() { 39, 1 }, new() { 40, 1 }, new() { 41, 1 }, new() { 42, 1 }, new() { 43, 18 }, new() { 44, 2 }, new() { 45, 1 }, new() { 46, 1 }, new() { 47, 2 }, new() { 48, 1 }, new() { 49, 1 }, new() { 50, 1 }, new() { 51, 3 }, new() { 52, 1 }, new() { 53, 1 }, new() { 54, 1 }, new() { 55, 1 }, },
                [14] = new() { new() { 0, 1 }, new() { 1, 2 }, new() { 2, 1 }, new() { 3, 18 }, new() { 4, 2 }, new() { 5, 1 }, new() { 6, 1 }, new() { 7, 3 }, new() { 8, 17 }, new() { 9, 1 }, new() { 10, 1 }, new() { 11, 18 }, new() { 12, 4 }, new() { 13, 4 }, new() { 14, 4 }, new() { 15, 4 }, new() { 16, 17 }, new() { 17, 1 }, new() { 18, 1 }, new() { 19, 18 }, new() { 20, 1 }, new() { 21, 1 }, new() { 22, 1 }, new() { 23, 17 }, new() { 24, 1 }, new() { 25, 18 }, new() { 26, 17 }, new() { 27, 1 }, new() { 28, 1 }, new() { 29, 1 }, new() { 30, 1 }, new() { 31, 1 }, new() { 32, 1 }, new() { 33, 1 }, new() { 34, 1 }, new() { 35, 1 }, new() { 36, 1 }, new() { 37, 1 }, new() { 38, 1 }, new() { 39, 18 }, new() { 40, 1 }, new() { 41, 1 }, new() { 42, 1 }, new() { 43, 1 }, new() { 44, 3 }, new() { 45, 1 }, new() { 46, 1 }, new() { 47, 1 }, new() { 48, 17 }, new() { 49, 18 }, new() { 50, 17 }, new() { 51, 1 }, new() { 52, 1 }, new() { 53, 3 }, new() { 54, 1 }, new() { 55, 1 }, },
                [15] = new() { new() { 0, 1 }, new() { 1, 1 }, new() { 2, 3 }, new() { 3, 1 }, new() { 4, 17 }, new() { 5, 17 }, new() { 6, 1 }, new() { 7, 18 }, new() { 8, 17 }, new() { 9, 1 }, new() { 10, 1 }, new() { 11, 1 }, new() { 12, 4 }, new() { 13, 4 }, new() { 14, 4 }, new() { 15, 18 }, new() { 16, 17 }, new() { 17, 1 }, new() { 18, 1 }, new() { 19, 1 }, new() { 20, 1 }, new() { 21, 1 }, new() { 22, 2 }, new() { 23, 18 }, new() { 24, 1 }, new() { 25, 1 }, new() { 26, 1 }, new() { 27, 18 }, new() { 28, 1 }, new() { 29, 1 }, new() { 30, 1 }, new() { 31, 18 }, new() { 32, 1 }, new() { 33, 17 }, new() { 34, 1 }, new() { 35, 1 }, new() { 36, 1 }, new() { 37, 1 }, new() { 38, 19 }, new() { 39, 3 }, new() { 40, 1 }, new() { 41, 19 }, new() { 42, 1 }, new() { 43, 1 }, new() { 44, 1 }, new() { 45, 1 }, new() { 46, 17 }, new() { 47, 1 }, new() { 48, 1 }, new() { 49, 1 }, new() { 50, 3 }, new() { 51, 1 }, new() { 52, 1 }, new() { 53, 1 }, new() { 54, 1 }, new() { 55, 17 }, },
                [16] = new() { new() { 0, 1 }, new() { 1, 3 }, new() { 2, 17 }, new() { 3, 1 }, new() { 4, 1 }, new() { 5, 17 }, new() { 6, 3 }, new() { 7, 1 }, new() { 8, 1 }, new() { 9, 1 }, new() { 10, 19 }, new() { 11, 4 }, new() { 12, 4 }, new() { 13, 4 }, new() { 14, 4 }, new() { 15, 1 }, new() { 16, 1 }, new() { 17, 1 }, new() { 18, 19 }, new() { 19, 1 }, new() { 20, 1 }, new() { 21, 1 }, new() { 22, 2 }, new() { 23, 1 }, new() { 24, 3 }, new() { 25, 19 }, new() { 26, 18 }, new() { 27, 1 }, new() { 28, 18 }, new() { 29, 1 }, new() { 30, 1 }, new() { 31, 1 }, new() { 32, 1 }, new() { 33, 1 }, new() { 34, 1 }, new() { 35, 18 }, new() { 36, 1 }, new() { 37, 3 }, new() { 38, 1 }, new() { 39, 1 }, new() { 40, 17 }, new() { 41, 1 }, new() { 42, 1 }, new() { 43, 2 }, new() { 44, 1 }, new() { 45, 17 }, new() { 46, 1 }, new() { 47, 1 }, new() { 48, 2 }, new() { 49, 3 }, new() { 50, 1 }, new() { 51, 18 }, new() { 52, 1 }, new() { 53, 18 }, new() { 54, 19 }, new() { 55, 1 }, },
                [17] = new() { new() { 0, 1 }, new() { 1, 1 }, new() { 2, 1 }, new() { 3, 3 }, new() { 4, 1 }, new() { 5, 1 }, new() { 6, 1 }, new() { 7, 1 }, new() { 8, 1 }, new() { 9, 1 }, new() { 10, 3 }, new() { 11, 4 }, new() { 12, 4 }, new() { 13, 4 }, new() { 14, 4 }, new() { 15, 1 }, new() { 16, 1 }, new() { 17, 1 }, new() { 18, 3 }, new() { 19, 1 }, new() { 20, 1 }, new() { 21, 1 }, new() { 22, 1 }, new() { 23, 1 }, new() { 24, 1 }, new() { 25, 1 }, new() { 26, 17 }, new() { 27, 1 }, new() { 28, 1 }, new() { 29, 1 }, new() { 30, 1 }, new() { 31, 19 }, new() { 32, 19 }, new() { 33, 17 }, new() { 34, 18 }, new() { 35, 2 }, new() { 36, 1 }, new() { 37, 1 }, new() { 38, 1 }, new() { 39, 1 }, new() { 40, 19 }, new() { 41, 1 }, new() { 42, 3 }, new() { 43, 1 }, new() { 44, 1 }, new() { 45, 2 }, new() { 46, 1 }, new() { 47, 1 }, new() { 48, 1 }, new() { 49, 19 }, new() { 50, 1 }, new() { 51, 17 }, new() { 52, 1 }, new() { 53, 18 }, new() { 54, 2 }, new() { 55, 1 }, },
                [18] = new() { new() { 0, 2 }, new() { 1, 1 }, new() { 2, 18 }, new() { 3, 1 }, new() { 4, 1 }, new() { 5, 1 }, new() { 6, 1 }, new() { 7, 1 }, new() { 8, 3 }, new() { 9, 19 }, new() { 10, 1 }, new() { 11, 4 }, new() { 12, 4 }, new() { 13, 4 }, new() { 14, 4 }, new() { 15, 1 }, new() { 16, 1 }, new() { 17, 1 }, new() { 18, 1 }, new() { 19, 1 }, new() { 20, 3 }, new() { 21, 1 }, new() { 22, 1 }, new() { 23, 1 }, new() { 24, 1 }, new() { 25, 1 }, new() { 26, 1 }, new() { 27, 1 }, new() { 28, 17 }, new() { 29, 19 }, new() { 30, 1 }, new() { 31, 1 }, new() { 32, 17 }, new() { 33, 19 }, new() { 34, 1 }, new() { 35, 1 }, new() { 36, 1 }, new() { 37, 1 }, new() { 38, 1 }, new() { 39, 1 }, new() { 40, 1 }, new() { 41, 1 }, new() { 42, 18 }, new() { 43, 1 }, new() { 44, 1 }, new() { 45, 3 }, new() { 46, 1 }, new() { 47, 1 }, new() { 48, 1 }, new() { 49, 1 }, new() { 50, 1 }, new() { 51, 1 }, new() { 52, 1 }, new() { 53, 3 }, new() { 54, 1 }, new() { 55, 1 }, },
                [19] = new() { new() { 0, 3 }, new() { 1, 2 }, new() { 2, 18 }, new() { 3, 1 }, new() { 4, 1 }, new() { 5, 1 }, new() { 6, 1 }, new() { 7, 1 }, new() { 8, 1 }, new() { 9, 1 }, new() { 10, 19 }, new() { 11, 4 }, new() { 12, 4 }, new() { 13, 4 }, new() { 14, 4 }, new() { 15, 19 }, new() { 16, 1 }, new() { 17, 1 }, new() { 18, 19 }, new() { 19, 1 }, new() { 20, 17 }, new() { 21, 1 }, new() { 22, 1 }, new() { 23, 1 }, new() { 24, 1 }, new() { 25, 2 }, new() { 26, 19 }, new() { 27, 2 }, new() { 28, 1 }, new() { 29, 1 }, new() { 30, 1 }, new() { 31, 1 }, new() { 32, 1 }, new() { 33, 1 }, new() { 34, 1 }, new() { 35, 1 }, new() { 36, 2 }, new() { 37, 1 }, new() { 38, 1 }, new() { 39, 1 }, new() { 40, 2 }, new() { 41, 1 }, new() { 42, 1 }, new() { 43, 18 }, new() { 44, 1 }, new() { 45, 1 }, new() { 46, 18 }, new() { 47, 1 }, new() { 48, 18 }, new() { 49, 1 }, new() { 50, 1 }, new() { 51, 17 }, new() { 52, 1 }, new() { 53, 1 }, new() { 54, 18 }, new() { 55, 1 }, },
                [20] = new() { new() { 0, 1 }, new() { 1, 1 }, new() { 2, 1 }, new() { 3, 1 }, new() { 4, 3 }, new() { 5, 1 }, new() { 6, 1 }, new() { 7, 1 }, new() { 8, 1 }, new() { 9, 1 }, new() { 10, 1 }, new() { 11, 4 }, new() { 12, 4 }, new() { 13, 4 }, new() { 14, 4 }, new() { 15, 1 }, new() { 16, 1 }, new() { 17, 3 }, new() { 18, 2 }, new() { 19, 17 }, new() { 20, 1 }, new() { 21, 1 }, new() { 22, 1 }, new() { 23, 1 }, new() { 24, 1 }, new() { 25, 1 }, new() { 26, 18 }, new() { 27, 1 }, new() { 28, 1 }, new() { 29, 1 }, new() { 30, 1 }, new() { 31, 19 }, new() { 32, 1 }, new() { 33, 1 }, new() { 34, 1 }, new() { 35, 2 }, new() { 36, 1 }, new() { 37, 1 }, new() { 38, 19 }, new() { 39, 1 }, new() { 40, 1 }, new() { 41, 18 }, new() { 42, 1 }, new() { 43, 1 }, new() { 44, 1 }, new() { 45, 1 }, new() { 46, 1 }, new() { 47, 1 }, new() { 48, 1 }, new() { 49, 19 }, new() { 50, 1 }, new() { 51, 1 }, new() { 52, 1 }, new() { 53, 1 }, new() { 54, 1 }, new() { 55, 19 }, },
                [21] = new() { new() { 0, 1 }, new() { 1, 1 }, new() { 2, 3 }, new() { 3, 1 }, new() { 4, 1 }, new() { 5, 17 }, new() { 6, 1 }, new() { 7, 1 }, new() { 8, 1 }, new() { 9, 2 }, new() { 10, 1 }, new() { 11, 4 }, new() { 12, 4 }, new() { 13, 4 }, new() { 14, 4 }, new() { 15, 1 }, new() { 16, 19 }, new() { 17, 1 }, new() { 18, 1 }, new() { 19, 1 }, new() { 20, 1 }, new() { 21, 1 }, new() { 22, 1 }, new() { 23, 19 }, new() { 24, 1 }, new() { 25, 1 }, new() { 26, 19 }, new() { 27, 1 }, new() { 28, 18 }, new() { 29, 1 }, new() { 30, 1 }, new() { 31, 1 }, new() { 32, 19 }, new() { 33, 1 }, new() { 34, 1 }, new() { 35, 1 }, new() { 36, 1 }, new() { 37, 1 }, new() { 38, 19 }, new() { 39, 1 }, new() { 40, 1 }, new() { 41, 3 }, new() { 42, 1 }, new() { 43, 18 }, new() { 44, 1 }, new() { 45, 1 }, new() { 46, 1 }, new() { 47, 1 }, new() { 48, 1 }, new() { 49, 1 }, new() { 50, 1 }, new() { 51, 1 }, new() { 52, 3 }, new() { 53, 1 }, new() { 54, 3 }, new() { 55, 1 }, },
                [22] = new() { new() { 0, 1 }, new() { 1, 1 }, new() { 2, 1 }, new() { 3, 1 }, new() { 4, 1 }, new() { 5, 1 }, new() { 6, 1 }, new() { 7, 1 }, new() { 8, 19 }, new() { 9, 1 }, new() { 10, 1 }, new() { 11, 4 }, new() { 12, 4 }, new() { 13, 4 }, new() { 14, 4 }, new() { 15, 4 }, new() { 16, 1 }, new() { 17, 1 }, new() { 18, 1 }, new() { 19, 1 }, new() { 20, 1 }, new() { 21, 1 }, new() { 22, 18 }, new() { 23, 1 }, new() { 24, 19 }, new() { 25, 3 }, new() { 26, 1 }, new() { 27, 1 }, new() { 28, 19 }, new() { 29, 1 }, new() { 30, 1 }, new() { 31, 1 }, new() { 32, 1 }, new() { 33, 19 }, new() { 34, 1 }, new() { 35, 1 }, new() { 36, 1 }, new() { 37, 1 }, new() { 38, 3 }, new() { 39, 1 }, new() { 40, 1 }, new() { 41, 17 }, new() { 42, 17 }, new() { 43, 1 }, new() { 44, 1 }, new() { 45, 18 }, new() { 46, 1 }, new() { 47, 19 }, new() { 48, 18 }, new() { 49, 3 }, new() { 50, 1 }, new() { 51, 1 }, new() { 52, 1 }, new() { 53, 1 }, new() { 54, 1 }, new() { 55, 1 }, },
                [23] = new() { new() { 0, 1 }, new() { 1, 1 }, new() { 2, 17 }, new() { 3, 1 }, new() { 4, 1 }, new() { 5, 1 }, new() { 6, 18 }, new() { 7, 1 }, new() { 8, 2 }, new() { 9, 3 }, new() { 10, 1 }, new() { 11, 4 }, new() { 12, 4 }, new() { 13, 4 }, new() { 14, 4 }, new() { 15, 4 }, new() { 16, 4 }, new() { 17, 4 }, new() { 18, 3 }, new() { 19, 1 }, new() { 20, 1 }, new() { 21, 1 }, new() { 22, 1 }, new() { 23, 1 }, new() { 24, 1 }, new() { 25, 1 }, new() { 26, 18 }, new() { 27, 1 }, new() { 28, 1 }, new() { 29, 19 }, new() { 30, 19 }, new() { 31, 1 }, new() { 32, 1 }, new() { 33, 18 }, new() { 34, 1 }, new() { 35, 1 }, new() { 36, 1 }, new() { 37, 1 }, new() { 38, 1 }, new() { 39, 17 }, new() { 40, 1 }, new() { 41, 1 }, new() { 42, 1 }, new() { 43, 1 }, new() { 44, 1 }, new() { 45, 1 }, new() { 46, 1 }, new() { 47, 1 }, new() { 48, 1 }, new() { 49, 1 }, new() { 50, 1 }, new() { 51, 1 }, new() { 52, 1 }, new() { 53, 1 }, new() { 54, 1 }, new() { 55, 18 }, },
                [24] = new() { new() { 0, 1 }, new() { 1, 1 }, new() { 2, 18 }, new() { 3, 3 }, new() { 4, 3 }, new() { 5, 1 }, new() { 6, 1 }, new() { 7, 1 }, new() { 8, 3 }, new() { 9, 1 }, new() { 10, 1 }, new() { 11, 1 }, new() { 12, 4 }, new() { 13, 4 }, new() { 14, 4 }, new() { 15, 4 }, new() { 16, 4 }, new() { 17, 4 }, new() { 18, 2 }, new() { 19, 1 }, new() { 20, 1 }, new() { 21, 1 }, new() { 22, 1 }, new() { 23, 17 }, new() { 24, 1 }, new() { 25, 1 }, new() { 26, 1 }, new() { 27, 1 }, new() { 28, 1 }, new() { 29, 1 }, new() { 30, 1 }, new() { 31, 1 }, new() { 32, 1 }, new() { 33, 3 }, new() { 34, 19 }, new() { 35, 19 }, new() { 36, 1 }, new() { 37, 1 }, new() { 38, 1 }, new() { 39, 1 }, new() { 40, 1 }, new() { 41, 1 }, new() { 42, 1 }, new() { 43, 1 }, new() { 44, 1 }, new() { 45, 1 }, new() { 46, 1 }, new() { 47, 2 }, new() { 48, 1 }, new() { 49, 18 }, new() { 50, 19 }, new() { 51, 3 }, new() { 52, 1 }, new() { 53, 1 }, new() { 54, 1 }, new() { 55, 1 }, },
                [25] = new() { new() { 0, 17 }, new() { 1, 17 }, new() { 2, 1 }, new() { 3, 1 }, new() { 4, 1 }, new() { 5, 1 }, new() { 6, 1 }, new() { 7, 1 }, new() { 8, 1 }, new() { 9, 1 }, new() { 10, 1 }, new() { 11, 1 }, new() { 12, 4 }, new() { 13, 4 }, new() { 14, 4 }, new() { 15, 4 }, new() { 16, 4 }, new() { 17, 4 }, new() { 18, 1 }, new() { 19, 2 }, new() { 20, 1 }, new() { 21, 1 }, new() { 22, 2 }, new() { 23, 1 }, new() { 24, 1 }, new() { 25, 1 }, new() { 26, 18 }, new() { 27, 1 }, new() { 28, 1 }, new() { 29, 1 }, new() { 30, 1 }, new() { 31, 3 }, new() { 32, 1 }, new() { 33, 1 }, new() { 34, 17 }, new() { 35, 2 }, new() { 36, 1 }, new() { 37, 3 }, new() { 38, 1 }, new() { 39, 1 }, new() { 40, 17 }, new() { 41, 2 }, new() { 42, 1 }, new() { 43, 1 }, new() { 44, 1 }, new() { 45, 2 }, new() { 46, 1 }, new() { 47, 1 }, new() { 48, 1 }, new() { 49, 1 }, new() { 50, 1 }, new() { 51, 1 }, new() { 52, 1 }, new() { 53, 17 }, new() { 54, 1 }, new() { 55, 1 }, },
                [26] = new() { new() { 0, 1 }, new() { 1, 1 }, new() { 2, 1 }, new() { 3, 18 }, new() { 4, 1 }, new() { 5, 1 }, new() { 6, 1 }, new() { 7, 1 }, new() { 8, 3 }, new() { 9, 1 }, new() { 10, 1 }, new() { 11, 1 }, new() { 12, 4 }, new() { 13, 4 }, new() { 14, 4 }, new() { 15, 4 }, new() { 16, 4 }, new() { 17, 4 }, new() { 18, 1 }, new() { 19, 17 }, new() { 20, 1 }, new() { 21, 1 }, new() { 22, 1 }, new() { 23, 1 }, new() { 24, 1 }, new() { 25, 1 }, new() { 26, 1 }, new() { 27, 1 }, new() { 28, 1 }, new() { 29, 1 }, new() { 30, 1 }, new() { 31, 1 }, new() { 32, 18 }, new() { 33, 1 }, new() { 34, 1 }, new() { 35, 1 }, new() { 36, 18 }, new() { 37, 1 }, new() { 38, 18 }, new() { 39, 1 }, new() { 40, 1 }, new() { 41, 1 }, new() { 42, 3 }, new() { 43, 1 }, new() { 44, 1 }, new() { 45, 1 }, new() { 46, 1 }, new() { 47, 1 }, new() { 48, 3 }, new() { 49, 1 }, new() { 50, 1 }, new() { 51, 1 }, new() { 52, 1 }, new() { 53, 1 }, new() { 54, 1 }, new() { 55, 1 }, },
                [27] = new() { new() { 0, 1 }, new() { 1, 1 }, new() { 2, 1 }, new() { 3, 2 }, new() { 4, 1 }, new() { 5, 18 }, new() { 6, 1 }, new() { 7, 1 }, new() { 8, 1 }, new() { 9, 1 }, new() { 10, 17 }, new() { 11, 1 }, new() { 12, 1 }, new() { 13, 4 }, new() { 14, 4 }, new() { 15, 4 }, new() { 16, 4 }, new() { 17, 4 }, new() { 18, 4 }, new() { 19, 1 }, new() { 20, 1 }, new() { 21, 1 }, new() { 22, 1 }, new() { 23, 1 }, new() { 24, 1 }, new() { 25, 1 }, new() { 26, 17 }, new() { 27, 17 }, new() { 28, 1 }, new() { 29, 1 }, new() { 30, 1 }, new() { 31, 1 }, new() { 32, 1 }, new() { 33, 1 }, new() { 34, 1 }, new() { 35, 18 }, new() { 36, 1 }, new() { 37, 1 }, new() { 38, 1 }, new() { 39, 1 }, new() { 40, 1 }, new() { 41, 18 }, new() { 42, 1 }, new() { 43, 3 }, new() { 44, 3 }, new() { 45, 1 }, new() { 46, 18 }, new() { 47, 3 }, new() { 48, 3 }, new() { 49, 1 }, new() { 50, 1 }, new() { 51, 1 }, new() { 52, 19 }, new() { 53, 1 }, new() { 54, 1 }, new() { 55, 1 }, },
                [28] = new() { new() { 0, 1 }, new() { 1, 1 }, new() { 2, 1 }, new() { 3, 1 }, new() { 4, 1 }, new() { 5, 18 }, new() { 6, 1 }, new() { 7, 1 }, new() { 8, 1 }, new() { 9, 1 }, new() { 10, 1 }, new() { 11, 1 }, new() { 12, 1 }, new() { 13, 1 }, new() { 14, 4 }, new() { 15, 4 }, new() { 16, 4 }, new() { 17, 4 }, new() { 18, 4 }, new() { 19, 4 }, new() { 20, 1 }, new() { 21, 1 }, new() { 22, 1 }, new() { 23, 17 }, new() { 24, 19 }, new() { 25, 1 }, new() { 26, 17 }, new() { 27, 1 }, new() { 28, 1 }, new() { 29, 19 }, new() { 30, 1 }, new() { 31, 18 }, new() { 32, 1 }, new() { 33, 1 }, new() { 34, 1 }, new() { 35, 1 }, new() { 36, 1 }, new() { 37, 18 }, new() { 38, 1 }, new() { 39, 1 }, new() { 40, 1 }, new() { 41, 1 }, new() { 42, 1 }, new() { 43, 1 }, new() { 44, 18 }, new() { 45, 3 }, new() { 46, 1 }, new() { 47, 1 }, new() { 48, 18 }, new() { 49, 1 }, new() { 50, 19 }, new() { 51, 17 }, new() { 52, 1 }, new() { 53, 17 }, new() { 54, 1 }, new() { 55, 1 }, },
                [29] = new() { new() { 0, 1 }, new() { 1, 1 }, new() { 2, 1 }, new() { 3, 1 }, new() { 4, 1 }, new() { 5, 18 }, new() { 6, 1 }, new() { 7, 1 }, new() { 8, 1 }, new() { 9, 1 }, new() { 10, 2 }, new() { 11, 1 }, new() { 12, 3 }, new() { 13, 1 }, new() { 14, 1 }, new() { 15, 1 }, new() { 16, 4 }, new() { 17, 4 }, new() { 18, 4 }, new() { 19, 4 }, new() { 20, 1 }, new() { 21, 1 }, new() { 22, 1 }, new() { 23, 1 }, new() { 24, 1 }, new() { 25, 1 }, new() { 26, 17 }, new() { 27, 1 }, new() { 28, 1 }, new() { 29, 1 }, new() { 30, 1 }, new() { 31, 1 }, new() { 32, 1 }, new() { 33, 1 }, new() { 34, 17 }, new() { 35, 1 }, new() { 36, 1 }, new() { 37, 1 }, new() { 38, 1 }, new() { 39, 1 }, new() { 40, 17 }, new() { 41, 1 }, new() { 42, 1 }, new() { 43, 1 }, new() { 44, 18 }, new() { 45, 1 }, new() { 46, 1 }, new() { 47, 1 }, new() { 48, 1 }, new() { 49, 1 }, new() { 50, 17 }, new() { 51, 1 }, new() { 52, 1 }, new() { 53, 1 }, new() { 54, 1 }, new() { 55, 1 }, },
                [30] = new() { new() { 0, 1 }, new() { 1, 1 }, new() { 2, 1 }, new() { 3, 1 }, new() { 4, 19 }, new() { 5, 1 }, new() { 6, 1 }, new() { 7, 1 }, new() { 8, 19 }, new() { 9, 2 }, new() { 10, 1 }, new() { 11, 1 }, new() { 12, 2 }, new() { 13, 1 }, new() { 14, 1 }, new() { 15, 17 }, new() { 16, 1 }, new() { 17, 1 }, new() { 18, 19 }, new() { 19, 1 }, new() { 20, 3 }, new() { 21, 1 }, new() { 22, 1 }, new() { 23, 2 }, new() { 24, 1 }, new() { 25, 19 }, new() { 26, 1 }, new() { 27, 1 }, new() { 28, 1 }, new() { 29, 1 }, new() { 30, 3 }, new() { 31, 1 }, new() { 32, 1 }, new() { 33, 1 }, new() { 34, 1 }, new() { 35, 1 }, new() { 36, 1 }, new() { 37, 1 }, new() { 38, 1 }, new() { 39, 1 }, new() { 40, 1 }, new() { 41, 1 }, new() { 42, 1 }, new() { 43, 1 }, new() { 44, 1 }, new() { 45, 1 }, new() { 46, 1 }, new() { 47, 1 }, new() { 48, 1 }, new() { 49, 1 }, new() { 50, 3 }, new() { 51, 1 }, new() { 52, 1 }, new() { 53, 1 }, new() { 54, 1 }, new() { 55, 1 }, },
                [31] = new() { new() { 0, 1 }, new() { 1, 1 }, new() { 2, 1 }, new() { 3, 3 }, new() { 4, 18 }, new() { 5, 1 }, new() { 6, 1 }, new() { 7, 1 }, new() { 8, 1 }, new() { 9, 1 }, new() { 10, 19 }, new() { 11, 1 }, new() { 12, 1 }, new() { 13, 1 }, new() { 14, 1 }, new() { 15, 3 }, new() { 16, 1 }, new() { 17, 1 }, new() { 18, 1 }, new() { 19, 1 }, new() { 20, 1 }, new() { 21, 1 }, new() { 22, 1 }, new() { 23, 1 }, new() { 24, 1 }, new() { 26, 286 }, new() { 27, 286 }, new() { 28, 286 }, new() { 29, 286 }, new() { 31, 1 }, new() { 32, 17 }, new() { 33, 1 }, new() { 34, 1 }, new() { 35, 1 }, new() { 36, 1 }, new() { 37, 1 }, new() { 38, 17 }, new() { 39, 19 }, new() { 40, 19 }, new() { 41, 1 }, new() { 42, 1 }, new() { 43, 1 }, new() { 44, 1 }, new() { 45, 1 }, new() { 46, 1 }, new() { 47, 1 }, new() { 48, 1 }, new() { 49, 1 }, new() { 50, 1 }, new() { 51, 3 }, new() { 52, 1 }, new() { 53, 17 }, new() { 54, 1 }, new() { 55, 1 }, },
                [32] = new() { new() { 0, 2 }, new() { 1, 1 }, new() { 2, 1 }, new() { 3, 1 }, new() { 4, 3 }, new() { 5, 1 }, new() { 6, 2 }, new() { 7, 1 }, new() { 8, 1 }, new() { 9, 1 }, new() { 10, 19 }, new() { 11, 1 }, new() { 12, 1 }, new() { 13, 1 }, new() { 14, 1 }, new() { 15, 1 }, new() { 16, 1 }, new() { 17, 18 }, new() { 18, 1 }, new() { 19, 1 }, new() { 20, 1 }, new() { 21, 1 }, new() { 22, 1 }, new() { 23, 1 }, new() { 24, 1 }, new() { 26, 302 }, new() { 27, 302 }, new() { 28, 302 }, new() { 29, 302 }, new() { 31, 1 }, new() { 32, 1 }, new() { 33, 18 }, new() { 34, 1 }, new() { 35, 18 }, new() { 36, 1 }, new() { 37, 1 }, new() { 38, 1 }, new() { 39, 1 }, new() { 40, 1 }, new() { 41, 1 }, new() { 42, 1 }, new() { 43, 1 }, new() { 44, 1 }, new() { 45, 18 }, new() { 46, 1 }, new() { 47, 1 }, new() { 48, 2 }, new() { 49, 17 }, new() { 50, 1 }, new() { 51, 1 }, new() { 52, 1 }, new() { 53, 1 }, new() { 54, 1 }, new() { 55, 1 }, },
                [33] = new() { new() { 0, 3 }, new() { 1, 1 }, new() { 2, 1 }, new() { 3, 1 }, new() { 4, 19 }, new() { 5, 2 }, new() { 6, 3 }, new() { 7, 19 }, new() { 8, 1 }, new() { 9, 1 }, new() { 10, 1 }, new() { 11, 19 }, new() { 12, 19 }, new() { 13, 1 }, new() { 14, 3 }, new() { 15, 1 }, new() { 16, 1 }, new() { 17, 1 }, new() { 18, 1 }, new() { 19, 1 }, new() { 20, 19 }, new() { 21, 1 }, new() { 22, 1 }, new() { 23, 1 }, new() { 24, 18 }, new() { 31, 1 }, new() { 32, 2 }, new() { 33, 1 }, new() { 34, 1 }, new() { 35, 2 }, new() { 36, 1 }, new() { 37, 1 }, new() { 38, 17 }, new() { 39, 1 }, new() { 40, 19 }, new() { 41, 17 }, new() { 42, 1 }, new() { 43, 19 }, new() { 44, 1 }, new() { 45, 1 }, new() { 46, 1 }, new() { 47, 1 }, new() { 48, 1 }, new() { 49, 1 }, new() { 50, 1 }, new() { 51, 1 }, new() { 52, 18 }, new() { 53, 1 }, new() { 54, 19 }, new() { 55, 1 }, },

            };
            foreach (KeyValuePair<int, List<List<int>>> code in codes)
            {

                foreach (List<int> array in code.Value)
                {

                    back.Tiles[array[0], code.Key] = new StaticTile(back, outdoor, BlendMode.Alpha, array[1]);

                    if (array[1] == 4)
                    {
                        
                        waterTiles[array[0], code.Key] = true;

                        back.Tiles[array[0], code.Key].TileIndexProperties.Add("Water", new(true));

                    }
                    else
                    {
                        back.Tiles[array[0], code.Key].TileIndexProperties.Add("Type", "Stone");

                    }

                }

            }

            codes = new()
            {
                [0] = new() { new() { 0, 76 }, new() { 1, 76 }, new() { 2, 76 }, new() { 3, 76 }, new() { 4, 76 }, new() { 5, 76 }, new() { 6, 76 }, new() { 7, 76 }, new() { 8, 76 }, new() { 9, 76 }, new() { 10, 76 }, new() { 11, 76 }, new() { 12, 76 }, new() { 13, 76 }, new() { 14, 76 }, new() { 15, 76 }, new() { 16, 76 }, new() { 17, 76 }, new() { 18, 76 }, new() { 19, 76 }, new() { 20, 76 }, new() { 21, 76 }, new() { 22, 76 }, new() { 23, 76 }, new() { 24, 76 }, new() { 25, 76 }, new() { 26, 76 }, new() { 27, 76 }, new() { 28, 76 }, new() { 29, 76 }, new() { 30, 76 }, new() { 31, 76 }, new() { 32, 76 }, new() { 33, 76 }, new() { 34, 76 }, new() { 35, 76 }, new() { 36, 76 }, new() { 37, 76 }, new() { 38, 76 }, new() { 39, 76 }, new() { 40, 76 }, new() { 41, 76 }, new() { 42, 76 }, new() { 43, 76 }, new() { 44, 76 }, new() { 45, 76 }, new() { 46, 76 }, new() { 47, 76 }, new() { 48, 76 }, new() { 49, 76 }, new() { 50, 76 }, new() { 51, 76 }, new() { 52, 76 }, new() { 53, 76 }, new() { 54, 76 }, new() { 55, 76 }, },
                [1] = new() { new() { 0, 76 }, new() { 1, 76 }, new() { 2, 76 }, new() { 3, 76 }, new() { 4, 76 }, new() { 5, 76 }, new() { 6, 76 }, new() { 7, 76 }, new() { 8, 76 }, new() { 9, 76 }, new() { 10, 76 }, new() { 11, 76 }, new() { 12, 76 }, new() { 13, 76 }, new() { 14, 76 }, new() { 15, 76 }, new() { 16, 76 }, new() { 17, 76 }, new() { 18, 76 }, new() { 19, 76 }, new() { 20, 76 }, new() { 21, 76 }, new() { 22, 76 }, new() { 23, 76 }, new() { 24, 76 }, new() { 25, 76 }, new() { 26, 76 }, new() { 27, 76 }, new() { 28, 76 }, new() { 29, 76 }, new() { 30, 76 }, new() { 31, 76 }, new() { 32, 76 }, new() { 33, 76 }, new() { 34, 76 }, new() { 35, 76 }, new() { 36, 76 }, new() { 37, 76 }, new() { 38, 76 }, new() { 39, 76 }, new() { 40, 76 }, new() { 41, 76 }, new() { 42, 76 }, new() { 43, 76 }, new() { 44, 76 }, new() { 45, 76 }, new() { 46, 76 }, new() { 47, 76 }, new() { 48, 76 }, new() { 49, 76 }, new() { 50, 76 }, new() { 51, 76 }, new() { 52, 76 }, new() { 53, 76 }, new() { 54, 76 }, new() { 55, 76 }, },
                [2] = new() { new() { 0, 76 }, new() { 1, 76 }, new() { 2, 76 }, new() { 3, 76 }, new() { 4, 76 }, new() { 5, 76 }, new() { 6, 76 }, new() { 7, 76 }, new() { 8, 76 }, new() { 9, 76 }, new() { 10, 76 }, new() { 11, 76 }, new() { 12, 76 }, new() { 13, 76 }, new() { 14, 76 }, new() { 15, 76 }, new() { 16, 76 }, new() { 17, 76 }, new() { 18, 76 }, new() { 19, 76 }, new() { 20, 76 }, new() { 21, 76 }, new() { 22, 76 }, new() { 23, 76 }, new() { 24, 76 }, new() { 25, 76 }, new() { 26, 76 }, new() { 27, 76 }, new() { 28, 76 }, new() { 29, 76 }, new() { 30, 76 }, new() { 31, 76 }, new() { 32, 76 }, new() { 33, 76 }, new() { 34, 76 }, new() { 35, 76 }, new() { 36, 76 }, new() { 37, 76 }, new() { 38, 76 }, new() { 39, 76 }, new() { 40, 76 }, new() { 41, 76 }, new() { 42, 76 }, new() { 43, 76 }, new() { 44, 76 }, new() { 45, 76 }, new() { 46, 76 }, new() { 47, 76 }, new() { 48, 76 }, new() { 49, 76 }, new() { 50, 76 }, new() { 51, 76 }, new() { 52, 76 }, new() { 53, 76 }, new() { 54, 76 }, new() { 55, 76 }, },
                [3] = new() { new() { 0, 76 }, new() { 1, 76 }, new() { 2, 76 }, new() { 3, 76 }, new() { 4, 76 }, new() { 5, 76 }, new() { 6, 76 }, new() { 7, 76 }, new() { 8, 76 }, new() { 9, 76 }, new() { 10, 76 }, new() { 11, 76 }, new() { 12, 76 }, new() { 13, 76 }, new() { 14, 76 }, new() { 15, 76 }, new() { 16, 76 }, new() { 17, 76 }, new() { 18, 76 }, new() { 19, 76 }, new() { 20, 76 }, new() { 21, 76 }, new() { 22, 76 }, new() { 23, 76 }, new() { 24, 76 }, new() { 25, 76 }, new() { 26, 76 }, new() { 27, 76 }, new() { 28, 76 }, new() { 29, 76 }, new() { 30, 76 }, new() { 31, 76 }, new() { 32, 76 }, new() { 33, 76 }, new() { 34, 76 }, new() { 35, 76 }, new() { 36, 76 }, new() { 37, 76 }, new() { 38, 76 }, new() { 39, 76 }, new() { 40, 76 }, new() { 41, 76 }, new() { 42, 76 }, new() { 43, 76 }, new() { 44, 76 }, new() { 45, 76 }, new() { 46, 76 }, new() { 47, 76 }, new() { 48, 76 }, new() { 49, 76 }, new() { 50, 76 }, new() { 51, 76 }, new() { 52, 76 }, new() { 53, 76 }, new() { 54, 76 }, new() { 55, 76 }, },
                [4] = new() { new() { 0, 76 }, new() { 1, 76 }, new() { 2, 76 }, new() { 3, 76 }, new() { 4, 76 }, new() { 5, 76 }, new() { 6, 76 }, new() { 7, 76 }, new() { 8, 76 }, new() { 9, 76 }, new() { 10, 76 }, new() { 11, 76 }, new() { 12, 76 }, new() { 13, 76 }, new() { 14, 76 }, new() { 15, 76 }, new() { 16, 76 }, new() { 17, 76 }, new() { 18, 76 }, new() { 19, 76 }, new() { 20, 76 }, new() { 21, 76 }, new() { 22, 149 }, new() { 23, 148 }, new() { 24, 151 }, new() { 25, 148 }, new() { 26, 148 }, new() { 27, 150 }, new() { 28, 148 }, new() { 29, 148 }, new() { 30, 148 }, new() { 31, 148 }, new() { 32, 150 }, new() { 33, 150 }, new() { 34, 148 }, new() { 35, 148 }, new() { 36, 148 }, new() { 37, 76 }, new() { 38, 76 }, new() { 39, 76 }, new() { 40, 76 }, new() { 41, 76 }, new() { 42, 76 }, new() { 43, 76 }, new() { 44, 76 }, new() { 45, 76 }, new() { 46, 76 }, new() { 47, 76 }, new() { 48, 76 }, new() { 49, 76 }, new() { 50, 76 }, new() { 51, 76 }, new() { 52, 76 }, new() { 53, 76 }, new() { 54, 76 }, new() { 55, 76 }, },
                [5] = new() { new() { 0, 76 }, new() { 1, 76 }, new() { 2, 76 }, new() { 3, 76 }, new() { 4, 76 }, new() { 5, 76 }, new() { 6, 76 }, new() { 7, 76 }, new() { 8, 76 }, new() { 9, 76 }, new() { 10, 76 }, new() { 11, 76 }, new() { 12, 76 }, new() { 13, 76 }, new() { 14, 149 }, new() { 15, 149 }, new() { 16, 148 }, new() { 17, 148 }, new() { 18, 148 }, new() { 19, 148 }, new() { 20, 148 }, new() { 21, 164 }, new() { 22, 165 }, new() { 23, 164 }, new() { 24, 167 }, new() { 25, 164 }, new() { 26, 164 }, new() { 27, 166 }, new() { 28, 164 }, new() { 29, 164 }, new() { 30, 164 }, new() { 31, 164 }, new() { 32, 166 }, new() { 33, 166 }, new() { 34, 164 }, new() { 35, 164 }, new() { 36, 164 }, new() { 37, 164 }, new() { 38, 76 }, new() { 39, 76 }, new() { 40, 76 }, new() { 41, 76 }, new() { 42, 76 }, new() { 43, 76 }, new() { 44, 76 }, new() { 45, 76 }, new() { 46, 76 }, new() { 47, 76 }, new() { 48, 76 }, new() { 49, 76 }, new() { 50, 76 }, new() { 51, 76 }, new() { 52, 76 }, new() { 53, 76 }, new() { 54, 76 }, new() { 55, 76 }, },
                [6] = new() { new() { 0, 76 }, new() { 1, 76 }, new() { 2, 76 }, new() { 3, 76 }, new() { 4, 76 }, new() { 5, 76 }, new() { 6, 76 }, new() { 7, 76 }, new() { 8, 76 }, new() { 9, 76 }, new() { 10, 76 }, new() { 11, 76 }, new() { 12, 76 }, new() { 13, 164 }, new() { 14, 165 }, new() { 15, 165 }, new() { 16, 164 }, new() { 17, 164 }, new() { 18, 164 }, new() { 19, 164 }, new() { 20, 164 }, new() { 21, 180 }, new() { 22, 181 }, new() { 23, 180 }, new() { 24, 183 }, new() { 25, 180 }, new() { 26, 180 }, new() { 27, 182 }, new() { 28, 180 }, new() { 29, 180 }, new() { 30, 180 }, new() { 31, 180 }, new() { 32, 182 }, new() { 33, 182 }, new() { 34, 180 }, new() { 35, 180 }, new() { 36, 180 }, new() { 37, 180 }, new() { 38, 164 }, new() { 39, 76 }, new() { 40, 76 }, new() { 41, 76 }, new() { 42, 76 }, new() { 43, 76 }, new() { 44, 76 }, new() { 45, 76 }, new() { 46, 76 }, new() { 47, 76 }, new() { 48, 76 }, new() { 49, 76 }, new() { 50, 76 }, new() { 51, 76 }, new() { 52, 76 }, new() { 53, 76 }, new() { 54, 76 }, new() { 55, 76 }, },
                [7] = new() { new() { 0, 76 }, new() { 1, 76 }, new() { 2, 76 }, new() { 3, 76 }, new() { 4, 76 }, new() { 5, 76 }, new() { 6, 76 }, new() { 7, 76 }, new() { 8, 76 }, new() { 9, 76 }, new() { 10, 76 }, new() { 11, 76 }, new() { 12, 164 }, new() { 13, 180 }, new() { 14, 181 }, new() { 15, 181 }, new() { 16, 180 }, new() { 17, 180 }, new() { 18, 180 }, new() { 19, 180 }, new() { 20, 180 }, new() { 21, 196 }, new() { 22, 197 }, new() { 23, 196 }, new() { 24, 199 }, new() { 25, 196 }, new() { 26, 196 }, new() { 27, 198 }, new() { 28, 196 }, new() { 29, 196 }, new() { 30, 196 }, new() { 31, 196 }, new() { 32, 198 }, new() { 33, 198 }, new() { 34, 196 }, new() { 35, 196 }, new() { 36, 196 }, new() { 37, 196 }, new() { 38, 180 }, new() { 39, 164 }, new() { 40, 76 }, new() { 41, 76 }, new() { 42, 76 }, new() { 43, 76 }, new() { 44, 76 }, new() { 45, 76 }, new() { 46, 76 }, new() { 47, 76 }, new() { 48, 76 }, new() { 49, 76 }, new() { 50, 76 }, new() { 51, 76 }, new() { 52, 76 }, new() { 53, 76 }, new() { 54, 76 }, new() { 55, 76 }, },
                [8] = new() { new() { 0, 76 }, new() { 1, 76 }, new() { 2, 76 }, new() { 3, 76 }, new() { 4, 76 }, new() { 5, 76 }, new() { 6, 76 }, new() { 7, 76 }, new() { 8, 76 }, new() { 9, 76 }, new() { 10, 76 }, new() { 11, 76 }, new() { 12, 180 }, new() { 13, 196 }, new() { 14, 197 }, new() { 15, 197 }, new() { 16, 196 }, new() { 17, 196 }, new() { 18, 196 }, new() { 19, 196 }, new() { 20, 196 }, new() { 21, 202 }, new() { 37, 192 }, new() { 38, 196 }, new() { 39, 180 }, new() { 40, 164 }, new() { 41, 76 }, new() { 42, 76 }, new() { 43, 76 }, new() { 44, 76 }, new() { 45, 76 }, new() { 46, 76 }, new() { 47, 76 }, new() { 48, 76 }, new() { 49, 76 }, new() { 50, 76 }, new() { 51, 76 }, new() { 52, 76 }, new() { 53, 76 }, new() { 54, 76 }, new() { 55, 76 }, },
                [9] = new() { new() { 0, 76 }, new() { 1, 76 }, new() { 2, 76 }, new() { 3, 76 }, new() { 4, 76 }, new() { 5, 76 }, new() { 6, 76 }, new() { 7, 76 }, new() { 8, 76 }, new() { 9, 76 }, new() { 10, 76 }, new() { 11, 76 }, new() { 12, 196 }, new() { 13, 200 }, new() { 14, 257 }, new() { 15, 258 }, new() { 16, 258 }, new() { 17, 258 }, new() { 18, 259 }, new() { 38, 194 }, new() { 39, 196 }, new() { 40, 180 }, new() { 41, 76 }, new() { 42, 76 }, new() { 43, 76 }, new() { 44, 76 }, new() { 45, 76 }, new() { 46, 76 }, new() { 47, 76 }, new() { 48, 76 }, new() { 49, 76 }, new() { 50, 76 }, new() { 51, 76 }, new() { 52, 76 }, new() { 53, 76 }, new() { 54, 76 }, new() { 55, 76 }, },
                [10] = new() { new() { 0, 76 }, new() { 1, 76 }, new() { 2, 76 }, new() { 3, 76 }, new() { 4, 76 }, new() { 5, 76 }, new() { 6, 76 }, new() { 7, 76 }, new() { 8, 76 }, new() { 9, 76 }, new() { 10, 76 }, new() { 11, 164 }, new() { 12, 169 }, new() { 13, 257 }, new() { 14, 308 }, new() { 15, 274 }, new() { 16, 274 }, new() { 17, 274 }, new() { 18, 275 }, new() { 39, 194 }, new() { 40, 196 }, new() { 41, 150 }, new() { 42, 150 }, new() { 43, 76 }, new() { 44, 76 }, new() { 45, 76 }, new() { 46, 76 }, new() { 47, 76 }, new() { 48, 76 }, new() { 49, 76 }, new() { 50, 76 }, new() { 51, 76 }, new() { 52, 76 }, new() { 53, 76 }, new() { 54, 76 }, new() { 55, 76 }, },
                [11] = new() { new() { 0, 76 }, new() { 1, 76 }, new() { 2, 76 }, new() { 3, 76 }, new() { 4, 76 }, new() { 5, 76 }, new() { 6, 76 }, new() { 7, 76 }, new() { 8, 76 }, new() { 9, 76 }, new() { 10, 76 }, new() { 11, 180 }, new() { 12, 185 }, new() { 13, 273 }, new() { 14, 274 }, new() { 15, 274 }, new() { 16, 274 }, new() { 17, 274 }, new() { 18, 291 }, new() { 40, 163 }, new() { 41, 166 }, new() { 42, 166 }, new() { 43, 164 }, new() { 44, 76 }, new() { 45, 76 }, new() { 46, 76 }, new() { 47, 76 }, new() { 48, 76 }, new() { 49, 76 }, new() { 50, 76 }, new() { 51, 76 }, new() { 52, 76 }, new() { 53, 76 }, new() { 54, 76 }, new() { 55, 76 }, },
                [12] = new() { new() { 0, 76 }, new() { 1, 76 }, new() { 2, 76 }, new() { 3, 76 }, new() { 4, 76 }, new() { 5, 76 }, new() { 6, 76 }, new() { 7, 76 }, new() { 8, 76 }, new() { 9, 76 }, new() { 10, 76 }, new() { 11, 196 }, new() { 12, 200 }, new() { 13, 273 }, new() { 14, 274 }, new() { 15, 274 }, new() { 16, 274 }, new() { 17, 291 }, new() { 40, 176 }, new() { 41, 182 }, new() { 42, 182 }, new() { 43, 180 }, new() { 44, 164 }, new() { 45, 150 }, new() { 46, 76 }, new() { 47, 76 }, new() { 48, 76 }, new() { 49, 76 }, new() { 50, 76 }, new() { 51, 76 }, new() { 52, 76 }, new() { 53, 76 }, new() { 54, 76 }, new() { 55, 76 }, },
                [13] = new() { new() { 0, 76 }, new() { 1, 76 }, new() { 2, 76 }, new() { 3, 76 }, new() { 4, 76 }, new() { 5, 76 }, new() { 6, 76 }, new() { 7, 76 }, new() { 8, 76 }, new() { 9, 76 }, new() { 10, 164 }, new() { 11, 171 }, new() { 12, 257 }, new() { 13, 308 }, new() { 14, 274 }, new() { 15, 274 }, new() { 16, 291 }, new() { 40, 194 }, new() { 41, 198 }, new() { 42, 198 }, new() { 43, 196 }, new() { 44, 180 }, new() { 45, 166 }, new() { 46, 164 }, new() { 47, 76 }, new() { 48, 76 }, new() { 49, 76 }, new() { 50, 76 }, new() { 51, 76 }, new() { 52, 76 }, new() { 53, 76 }, new() { 54, 76 }, new() { 55, 76 }, },
                [14] = new() { new() { 0, 76 }, new() { 1, 76 }, new() { 2, 76 }, new() { 3, 76 }, new() { 4, 76 }, new() { 5, 76 }, new() { 6, 76 }, new() { 7, 76 }, new() { 8, 76 }, new() { 9, 76 }, new() { 10, 180 }, new() { 11, 185 }, new() { 12, 273 }, new() { 13, 274 }, new() { 14, 274 }, new() { 15, 291 }, new() { 43, 193 }, new() { 44, 196 }, new() { 45, 182 }, new() { 46, 180 }, new() { 47, 76 }, new() { 48, 76 }, new() { 49, 76 }, new() { 50, 76 }, new() { 51, 76 }, new() { 52, 76 }, new() { 53, 76 }, new() { 54, 76 }, new() { 55, 76 }, },
                [15] = new() { new() { 0, 76 }, new() { 1, 76 }, new() { 2, 76 }, new() { 3, 76 }, new() { 4, 76 }, new() { 5, 76 }, new() { 6, 76 }, new() { 7, 76 }, new() { 8, 76 }, new() { 9, 76 }, new() { 10, 196 }, new() { 11, 200 }, new() { 12, 273 }, new() { 13, 274 }, new() { 14, 275 }, new() { 44, 195 }, new() { 45, 198 }, new() { 46, 196 }, new() { 47, 76 }, new() { 48, 76 }, new() { 49, 76 }, new() { 50, 76 }, new() { 51, 76 }, new() { 52, 76 }, new() { 53, 76 }, new() { 54, 76 }, new() { 55, 76 }, },
                [16] = new() { new() { 0, 76 }, new() { 1, 76 }, new() { 2, 76 }, new() { 3, 76 }, new() { 4, 76 }, new() { 5, 76 }, new() { 6, 76 }, new() { 7, 76 }, new() { 8, 76 }, new() { 9, 76 }, new() { 10, 139 }, new() { 11, 257 }, new() { 12, 308 }, new() { 13, 274 }, new() { 14, 275 }, new() { 46, 96 }, new() { 47, 76 }, new() { 48, 76 }, new() { 49, 76 }, new() { 50, 76 }, new() { 51, 76 }, new() { 52, 76 }, new() { 53, 76 }, new() { 54, 76 }, new() { 55, 76 }, },
                [17] = new() { new() { 0, 76 }, new() { 1, 76 }, new() { 2, 76 }, new() { 3, 76 }, new() { 4, 76 }, new() { 5, 76 }, new() { 6, 76 }, new() { 7, 76 }, new() { 8, 76 }, new() { 9, 76 }, new() { 10, 107 }, new() { 11, 273 }, new() { 12, 274 }, new() { 13, 274 }, new() { 14, 275 }, new() { 46, 80 }, new() { 47, 76 }, new() { 48, 76 }, new() { 49, 76 }, new() { 50, 76 }, new() { 51, 76 }, new() { 52, 76 }, new() { 53, 76 }, new() { 54, 76 }, new() { 55, 76 }, },
                [18] = new() { new() { 0, 76 }, new() { 1, 76 }, new() { 2, 76 }, new() { 3, 76 }, new() { 4, 76 }, new() { 5, 76 }, new() { 6, 76 }, new() { 7, 76 }, new() { 8, 76 }, new() { 9, 76 }, new() { 10, 107 }, new() { 11, 273 }, new() { 12, 274 }, new() { 13, 274 }, new() { 14, 275 }, new() { 39, 76 }, new() { 40, 76 }, new() { 41, 76 }, new() { 42, 139 }, new() { 46, 96 }, new() { 47, 76 }, new() { 48, 76 }, new() { 49, 76 }, new() { 50, 76 }, new() { 51, 76 }, new() { 52, 76 }, new() { 53, 76 }, new() { 54, 76 }, new() { 55, 76 }, },
                [19] = new() { new() { 0, 76 }, new() { 1, 76 }, new() { 2, 76 }, new() { 3, 76 }, new() { 4, 76 }, new() { 5, 76 }, new() { 6, 76 }, new() { 7, 76 }, new() { 8, 76 }, new() { 9, 76 }, new() { 10, 107 }, new() { 11, 273 }, new() { 12, 274 }, new() { 13, 274 }, new() { 14, 275 }, new() { 37, 145 }, new() { 38, 76 }, new() { 39, 76 }, new() { 40, 76 }, new() { 41, 149 }, new() { 42, 153 }, new() { 43, 50 }, new() { 46, 112 }, new() { 47, 76 }, new() { 48, 76 }, new() { 49, 76 }, new() { 50, 76 }, new() { 51, 76 }, new() { 52, 76 }, new() { 53, 76 }, new() { 54, 76 }, new() { 55, 76 }, },
                [20] = new() { new() { 0, 76 }, new() { 1, 76 }, new() { 2, 76 }, new() { 3, 76 }, new() { 4, 76 }, new() { 5, 76 }, new() { 6, 76 }, new() { 7, 76 }, new() { 8, 76 }, new() { 9, 76 }, new() { 10, 91 }, new() { 11, 273 }, new() { 12, 274 }, new() { 13, 274 }, new() { 14, 275 }, new() { 36, 48 }, new() { 37, 162 }, new() { 38, 164 }, new() { 39, 148 }, new() { 40, 164 }, new() { 41, 165 }, new() { 42, 168 }, new() { 43, 49 }, new() { 46, 96 }, new() { 47, 76 }, new() { 48, 76 }, new() { 49, 76 }, new() { 50, 76 }, new() { 51, 76 }, new() { 52, 76 }, new() { 53, 76 }, new() { 54, 76 }, new() { 55, 76 }, },
                [21] = new() { new() { 0, 76 }, new() { 1, 76 }, new() { 2, 76 }, new() { 3, 76 }, new() { 4, 76 }, new() { 5, 76 }, new() { 6, 76 }, new() { 7, 76 }, new() { 8, 76 }, new() { 9, 76 }, new() { 10, 123 }, new() { 11, 273 }, new() { 12, 274 }, new() { 13, 274 }, new() { 14, 275 }, new() { 36, 48 }, new() { 37, 178 }, new() { 38, 180 }, new() { 39, 164 }, new() { 40, 180 }, new() { 41, 181 }, new() { 42, 187 }, new() { 43, 48 }, new() { 46, 112 }, new() { 47, 76 }, new() { 48, 76 }, new() { 49, 76 }, new() { 50, 76 }, new() { 51, 76 }, new() { 52, 76 }, new() { 53, 76 }, new() { 54, 76 }, new() { 55, 76 }, },
                [22] = new() { new() { 0, 76 }, new() { 1, 76 }, new() { 2, 76 }, new() { 3, 76 }, new() { 4, 76 }, new() { 5, 76 }, new() { 6, 76 }, new() { 7, 76 }, new() { 8, 76 }, new() { 9, 76 }, new() { 10, 123 }, new() { 11, 273 }, new() { 12, 274 }, new() { 13, 292 }, new() { 14, 258 }, new() { 15, 259 }, new() { 36, 49 }, new() { 37, 195 }, new() { 38, 196 }, new() { 39, 180 }, new() { 40, 196 }, new() { 41, 197 }, new() { 42, 200 }, new() { 46, 112 }, new() { 47, 76 }, new() { 48, 76 }, new() { 49, 76 }, new() { 50, 76 }, new() { 51, 76 }, new() { 52, 76 }, new() { 53, 76 }, new() { 54, 76 }, new() { 55, 76 }, },
                [23] = new() { new() { 0, 76 }, new() { 1, 76 }, new() { 2, 76 }, new() { 3, 76 }, new() { 4, 76 }, new() { 5, 76 }, new() { 6, 76 }, new() { 7, 76 }, new() { 8, 76 }, new() { 9, 76 }, new() { 10, 91 }, new() { 11, 289 }, new() { 12, 274 }, new() { 13, 274 }, new() { 14, 274 }, new() { 15, 292 }, new() { 16, 258 }, new() { 17, 259 }, new() { 38, 193 }, new() { 39, 196 }, new() { 40, 202 }, new() { 46, 96 }, new() { 47, 76 }, new() { 48, 76 }, new() { 49, 76 }, new() { 50, 76 }, new() { 51, 76 }, new() { 52, 76 }, new() { 53, 76 }, new() { 54, 76 }, new() { 55, 76 }, },
                [24] = new() { new() { 0, 76 }, new() { 1, 76 }, new() { 2, 76 }, new() { 3, 76 }, new() { 4, 76 }, new() { 5, 76 }, new() { 6, 76 }, new() { 7, 76 }, new() { 8, 76 }, new() { 9, 76 }, new() { 10, 76 }, new() { 11, 139 }, new() { 12, 273 }, new() { 13, 274 }, new() { 14, 274 }, new() { 15, 274 }, new() { 16, 274 }, new() { 17, 275 }, new() { 46, 112 }, new() { 47, 76 }, new() { 48, 76 }, new() { 49, 76 }, new() { 50, 76 }, new() { 51, 76 }, new() { 52, 76 }, new() { 53, 76 }, new() { 54, 76 }, new() { 55, 76 }, },
                [25] = new() { new() { 0, 76 }, new() { 1, 76 }, new() { 2, 76 }, new() { 3, 76 }, new() { 4, 76 }, new() { 5, 76 }, new() { 6, 76 }, new() { 7, 76 }, new() { 8, 76 }, new() { 9, 76 }, new() { 10, 76 }, new() { 11, 91 }, new() { 12, 273 }, new() { 13, 274 }, new() { 14, 274 }, new() { 15, 274 }, new() { 16, 274 }, new() { 17, 275 }, new() { 46, 112 }, new() { 47, 76 }, new() { 48, 76 }, new() { 49, 76 }, new() { 50, 76 }, new() { 51, 76 }, new() { 52, 76 }, new() { 53, 76 }, new() { 54, 76 }, new() { 55, 76 }, },
                [26] = new() { new() { 0, 76 }, new() { 1, 76 }, new() { 2, 76 }, new() { 3, 76 }, new() { 4, 76 }, new() { 5, 76 }, new() { 6, 76 }, new() { 7, 76 }, new() { 8, 76 }, new() { 9, 76 }, new() { 10, 76 }, new() { 11, 91 }, new() { 12, 289 }, new() { 13, 274 }, new() { 14, 274 }, new() { 15, 274 }, new() { 16, 274 }, new() { 17, 275 }, new() { 46, 76 }, new() { 47, 76 }, new() { 48, 76 }, new() { 49, 76 }, new() { 50, 76 }, new() { 51, 76 }, new() { 52, 76 }, new() { 53, 76 }, new() { 54, 76 }, new() { 55, 76 }, },
                [27] = new() { new() { 0, 76 }, new() { 1, 76 }, new() { 2, 76 }, new() { 3, 76 }, new() { 4, 76 }, new() { 5, 76 }, new() { 6, 76 }, new() { 7, 76 }, new() { 8, 76 }, new() { 9, 76 }, new() { 10, 76 }, new() { 11, 76 }, new() { 13, 289 }, new() { 14, 274 }, new() { 15, 274 }, new() { 16, 274 }, new() { 17, 292 }, new() { 18, 259 }, new() { 45, 76 }, new() { 46, 76 }, new() { 47, 76 }, new() { 48, 76 }, new() { 49, 76 }, new() { 50, 76 }, new() { 51, 76 }, new() { 52, 76 }, new() { 53, 76 }, new() { 54, 76 }, new() { 55, 76 }, },
                [28] = new() { new() { 0, 76 }, new() { 1, 76 }, new() { 2, 76 }, new() { 3, 76 }, new() { 4, 76 }, new() { 5, 76 }, new() { 6, 76 }, new() { 7, 76 }, new() { 8, 76 }, new() { 9, 76 }, new() { 10, 76 }, new() { 11, 76 }, new() { 12, 76 }, new() { 14, 289 }, new() { 15, 290 }, new() { 16, 274 }, new() { 17, 274 }, new() { 18, 292 }, new() { 19, 259 }, new() { 44, 76 }, new() { 45, 76 }, new() { 46, 76 }, new() { 47, 76 }, new() { 48, 76 }, new() { 49, 76 }, new() { 50, 76 }, new() { 51, 76 }, new() { 52, 76 }, new() { 53, 76 }, new() { 54, 76 }, new() { 55, 76 }, },
                [29] = new() { new() { 0, 76 }, new() { 1, 76 }, new() { 2, 76 }, new() { 3, 76 }, new() { 4, 76 }, new() { 5, 76 }, new() { 6, 76 }, new() { 7, 76 }, new() { 8, 76 }, new() { 9, 76 }, new() { 10, 76 }, new() { 11, 76 }, new() { 12, 76 }, new() { 13, 76 }, new() { 16, 289 }, new() { 17, 290 }, new() { 18, 290 }, new() { 19, 291 }, new() { 43, 76 }, new() { 44, 76 }, new() { 45, 76 }, new() { 46, 76 }, new() { 47, 76 }, new() { 48, 76 }, new() { 49, 76 }, new() { 50, 76 }, new() { 51, 76 }, new() { 52, 76 }, new() { 53, 76 }, new() { 54, 76 }, new() { 55, 76 }, },
                [30] = new() { new() { 0, 76 }, new() { 1, 76 }, new() { 2, 76 }, new() { 3, 76 }, new() { 4, 76 }, new() { 5, 76 }, new() { 6, 76 }, new() { 7, 76 }, new() { 8, 76 }, new() { 9, 76 }, new() { 10, 76 }, new() { 11, 76 }, new() { 12, 76 }, new() { 13, 76 }, new() { 14, 76 }, new() { 15, 153 }, new() { 42, 76 }, new() { 43, 76 }, new() { 44, 76 }, new() { 45, 76 }, new() { 46, 76 }, new() { 47, 76 }, new() { 48, 76 }, new() { 49, 76 }, new() { 50, 76 }, new() { 51, 76 }, new() { 52, 76 }, new() { 53, 76 }, new() { 54, 76 }, new() { 55, 76 }, },
                [31] = new() { new() { 0, 76 }, new() { 1, 76 }, new() { 2, 76 }, new() { 3, 76 }, new() { 4, 76 }, new() { 5, 76 }, new() { 6, 76 }, new() { 7, 76 }, new() { 8, 76 }, new() { 9, 76 }, new() { 10, 76 }, new() { 11, 76 }, new() { 12, 76 }, new() { 13, 76 }, new() { 14, 76 }, new() { 15, 76 }, new() { 16, 76 }, new() { 17, 76 }, new() { 18, 76 }, new() { 19, 76 }, new() { 20, 76 }, new() { 21, 76 }, new() { 22, 76 }, new() { 23, 76 }, new() { 24, 76 }, new() { 25, 285 }, new() { 30, 287 }, new() { 31, 76 }, new() { 32, 76 }, new() { 33, 76 }, new() { 34, 76 }, new() { 35, 76 }, new() { 36, 76 }, new() { 37, 76 }, new() { 38, 76 }, new() { 39, 76 }, new() { 40, 76 }, new() { 41, 76 }, new() { 42, 76 }, new() { 43, 76 }, new() { 44, 76 }, new() { 45, 76 }, new() { 46, 76 }, new() { 47, 76 }, new() { 48, 76 }, new() { 49, 76 }, new() { 50, 76 }, new() { 51, 76 }, new() { 52, 76 }, new() { 53, 76 }, new() { 54, 76 }, new() { 55, 76 }, },
                [32] = new() { new() { 0, 76 }, new() { 1, 76 }, new() { 2, 76 }, new() { 3, 76 }, new() { 4, 76 }, new() { 5, 76 }, new() { 6, 76 }, new() { 7, 76 }, new() { 8, 76 }, new() { 9, 76 }, new() { 10, 76 }, new() { 11, 76 }, new() { 12, 76 }, new() { 13, 76 }, new() { 14, 76 }, new() { 15, 76 }, new() { 16, 76 }, new() { 17, 76 }, new() { 18, 76 }, new() { 19, 76 }, new() { 20, 76 }, new() { 21, 76 }, new() { 22, 76 }, new() { 23, 76 }, new() { 24, 76 }, new() { 25, 301 }, new() { 30, 303 }, new() { 31, 76 }, new() { 32, 76 }, new() { 33, 76 }, new() { 34, 76 }, new() { 35, 76 }, new() { 36, 76 }, new() { 37, 76 }, new() { 38, 76 }, new() { 39, 76 }, new() { 40, 76 }, new() { 41, 76 }, new() { 42, 76 }, new() { 43, 76 }, new() { 44, 76 }, new() { 45, 76 }, new() { 46, 76 }, new() { 47, 76 }, new() { 48, 76 }, new() { 49, 76 }, new() { 50, 76 }, new() { 51, 76 }, new() { 52, 76 }, new() { 53, 76 }, new() { 54, 76 }, new() { 55, 76 }, },
                [33] = new() { new() { 0, 76 }, new() { 1, 76 }, new() { 2, 76 }, new() { 3, 76 }, new() { 4, 76 }, new() { 5, 76 }, new() { 6, 76 }, new() { 7, 76 }, new() { 8, 76 }, new() { 9, 76 }, new() { 10, 76 }, new() { 11, 76 }, new() { 12, 76 }, new() { 13, 76 }, new() { 14, 76 }, new() { 15, 76 }, new() { 16, 76 }, new() { 17, 76 }, new() { 18, 76 }, new() { 19, 76 }, new() { 20, 76 }, new() { 21, 76 }, new() { 22, 76 }, new() { 23, 76 }, new() { 24, 76 }, new() { 25, 317 }, new() { 30, 319 }, new() { 31, 76 }, new() { 32, 76 }, new() { 33, 76 }, new() { 34, 76 }, new() { 35, 76 }, new() { 36, 76 }, new() { 37, 76 }, new() { 38, 76 }, new() { 39, 76 }, new() { 40, 76 }, new() { 41, 76 }, new() { 42, 76 }, new() { 43, 76 }, new() { 44, 76 }, new() { 45, 76 }, new() { 46, 76 }, new() { 47, 76 }, new() { 48, 76 }, new() { 49, 76 }, new() { 50, 76 }, new() { 51, 76 }, new() { 52, 76 }, new() { 53, 76 }, new() { 54, 76 }, new() { 55, 76 }, },

            };

            foreach (KeyValuePair<int, List<List<int>>> code in codes)
            {

                foreach (List<int> array in code.Value)
                {

                    buildings.Tiles[array[0], code.Key] = new StaticTile(buildings, outdoor, BlendMode.Alpha, array[1]);

                }

            }

            codes = new()
            {
                [5] = new() { new() { 21, 155 }, new() { 37, 145 }, },
                [6] = new() { new() { 13, 153 }, new() { 21, 168 }, new() { 37, 163 }, new() { 38, 146 }, },
                [7] = new() { new() { 12, 139 }, new() { 13, 168 }, new() { 21, 184 }, new() { 37, 179 }, new() { 38, 162 }, new() { 39, 146 }, },
                [8] = new() { new() { 12, 139 }, new() { 13, 186 }, new() { 38, 178 }, new() { 39, 162 }, new() { 40, 96 }, },
                [9] = new() { new() { 12, 152 }, new() { 39, 177 }, new() { 40, 80 }, },
                [10] = new() { new() { 11, 107 }, new() { 40, 147 }, },
                [11] = new() { new() { 11, 123 }, new() { 43, 146 }, },
                [12] = new() { new() { 11, 153 }, new() { 43, 160 }, new() { 44, 144 }, },
                [13] = new() { new() { 10, 107 }, new() { 43, 179 }, new() { 44, 161 }, new() { 46, 128 }, },
                [14] = new() { new() { 10, 91 }, new() { 44, 176 }, new() { 46, 128 }, },
                [15] = new() { new() { 10, 91 }, new() { 46, 112 }, },
                [17] = new() { new() { 39, 66 }, new() { 40, 68 }, new() { 41, 70 }, new() { 42, 75 }, },
                [18] = new() { new() { 37, 65 }, new() { 38, 70 }, new() { 39, 109 }, new() { 43, 34 }, },
                [19] = new() { new() { 36, 32 }, new() { 43, 33 }, },
                [20] = new() { new() { 36, 32 }, new() { 38, 146 }, new() { 40, 152 }, new() { 43, 32 }, },
                [21] = new() { new() { 36, 33 }, new() { 38, 162 }, new() { 40, 168 }, },
                [22] = new() { new() { 38, 179 }, new() { 40, 187 }, },
                [24] = new() { new() { 10, 108 }, new() { 11, 73 }, },
                [26] = new() { new() { 45, 66 }, new() { 46, 109 }, },
                [27] = new() { new() { 11, 108 }, new() { 12, 73 }, new() { 44, 67 }, new() { 45, 109 }, },
                [28] = new() { new() { 12, 108 }, new() { 13, 73 }, new() { 43, 66 }, new() { 44, 109 }, },
                [29] = new() { new() { 13, 108 }, new() { 14, 75 }, new() { 42, 65 }, new() { 43, 109 }, },
                [30] = new() { new() { 14, 108 }, new() { 15, 71 }, new() { 16, 70 }, new() { 17, 69 }, new() { 18, 70 }, new() { 19, 70 }, new() { 20, 70 }, new() { 21, 70 }, new() { 22, 70 }, new() { 23, 69 }, new() { 24, 71 }, new() { 25, 269 }, new() { 30, 271 }, new() { 31, 71 }, new() { 32, 73 }, new() { 33, 70 }, new() { 34, 69 }, new() { 35, 70 }, new() { 36, 70 }, new() { 37, 70 }, new() { 38, 70 }, new() { 39, 70 }, new() { 40, 69 }, new() { 41, 71 }, new() { 42, 109 }, },
                [33] = new() { new() { 26, 318 }, new() { 27, 318 }, new() { 28, 318 }, new() { 29, 318 }, },

            };

            foreach (KeyValuePair<int, List<List<int>>> code in codes)
            {

                foreach (List<int> array in code.Value)
                {

                    front.Tiles[array[0], code.Key] = new StaticTile(front, outdoor, BlendMode.Alpha, array[1]);

                }

            }

            codes = new()
            {

                [4] = new() { new() { 21, 92 }, new() { 37, 93 }, },
                [5] = new() { new() { 13, 92 }, new() { 38, 93 }, },
                [6] = new() { new() { 12, 92 }, new() { 39, 93 }, },
                [7] = new() { new() { 40, 93 }, },

                [9] = new() { new() { 11, 92 }, },
                [10] = new() { new() { 43, 93 }, },
                [11] = new() { new() { 44, 93 }, },
                [12] = new() { new() { 10, 92 }, new() { 46, 93 }, },

                [19] = new() { new() { 38, 93 }, new() { 40, 92 }, },

                [26] = new() { new() { 11, 92 }, },

                [31] = new() { new() { 19, 92 }, },



            };

            foreach (KeyValuePair<int, List<List<int>>> code in codes)
            {

                foreach (List<int> array in code.Value)
                {

                    alwaysfront.Tiles[array[0], code.Key] = new StaticTile(alwaysfront, outdoor, BlendMode.Alpha, array[1]);

                }

            }
            
            codes = new()
            {

                [9] = new() { new() { 24, 1 }, new() { 29, 1 }, new() { 34, 1 }, },
                [10] = new() { new() { 20, 1 }, },
                [11] = new() { new() { 15, 1 }, },
                [12] = new() { new() { 37, 1 }, },
                [15] = new() { new() { 40, 1 }, },
                [17] = new() { new() { 13, 1 }, },
                [22] = new() { new() { 12, 1 }, },
                [25] = new() { new() { 42, 1 }, },
                [26] = new() { new() { 15, 1 }, new() { 37, 1 }, },
                [28] = new() { new() { 21, 1 }, },

            };

            lightFields = new();

            foreach (KeyValuePair<int, List<List<int>>> code in codes)
            {

                foreach (List<int> array in code.Value)
                {

                    lightFields.Add(new Vector2(array[0], code.Key) * 64 + new Vector2(0, 32),4+Mod.instance.randomIndex.Next(3));

                }

            }

            this.map = newMap;

            dungeonTexture = Game1.temporaryContent.Load<Texture2D>(outdoor.ImageSource);

        }

        public override bool CanItemBePlacedHere(Vector2 tile, bool itemIsPassable = false, CollisionMask collisionMask = CollisionMask.All, CollisionMask ignorePassables = ~CollisionMask.Objects, bool useFarmerTile = false, bool ignorePassablesExactly = false)
        {

            return false;

        }

        public override bool isActionableTile(int xTile, int yTile, Farmer who)
        {

            Vector2 actionTile = new(xTile, yTile);

            if (dialogueTiles.ContainsKey(actionTile))
            {

                return true;

            }

            if (crateTiles.ContainsKey(actionTile))
            {
                
                if (crateTiles[actionTile] >= 0)
                {

                    return true;

                }

            }

            return base.isActionableTile(xTile, yTile, who);

        }

        public override bool checkAction(xTile.Dimensions.Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
        {

            Vector2 actionTile = new(tileLocation.X, tileLocation.Y);

            if (dialogueTiles.ContainsKey(actionTile) && Mod.instance.activeEvent.Count == 0)
            {

                CharacterHandle.characters characterType = dialogueTiles[actionTile];

                if (!Mod.instance.dialogue.ContainsKey(characterType))
                {

                    Mod.instance.dialogue[characterType] = new(characterType);

                }

                Mod.instance.dialogue[characterType].DialogueApproach();

                return true;

            }

            if (crateTiles.ContainsKey(actionTile))
            {
                if (crateTiles[actionTile] < 0)
                {

                    return false;

                }

                SpellHandle crate = new(this, new(0,crateTiles[actionTile]), actionTile * 64);

                crate.type = SpellHandle.spells.crate;

                crate.counter = 60;

                crate.Update();

                Mod.instance.spellRegister.Add(crate);

                crateTiles[actionTile] = -210;

                return true;

            }

            return base.checkAction(tileLocation, viewport, who);

        }

        public override void updateWarps()
        {
            //warps.Clear();

            if (warpSets.Count > 0)
            {

                warps.Clear();

                foreach (WarpTile warpSet in warpSets)
                {

                    warps.Add(new Warp(warpSet.enterX, warpSet.enterY, warpSet.location, warpSet.exitX, warpSet.exitY, flipFarmer: false));

                }

                return;

            }

            warpSets.Add(new WarpTile(26, 32, "Mine", 17, 5));

            warpSets.Add(new WarpTile(27, 32, "Mine", 17, 5));

            warpSets.Add(new WarpTile(28, 32, "Mine", 17, 5));

            warpSets.Add(new WarpTile(29, 32, "Mine", 17, 5));

            warps.Add(new Warp(26, 32, "Mine", 17, 6, flipFarmer: false));

            warps.Add(new Warp(27, 32, "Mine", 17, 6, flipFarmer: false));

            warps.Add(new Warp(28, 32, "Mine", 17, 6, flipFarmer: false));

            warps.Add(new Warp(29, 32, "Mine", 17, 6, flipFarmer: false));

        }

        public void AddCrateTile(int x, int y, int id)
        {

            crateTiles.Add(new(x, y), id);

            map.Layers[1].Tiles[x,y] = new StaticTile(map.Layers[1], map.Layers[0].Tiles[x,y].TileSheet, BlendMode.Alpha, map.Layers[0].Tiles[x, y].TileIndex);

        }

    }

}
