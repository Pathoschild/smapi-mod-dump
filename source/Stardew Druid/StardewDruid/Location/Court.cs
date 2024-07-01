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
    public class Court : GameLocation
    {

        public List<Location.WarpTile> warpSets = new();

        public List<Location.LocationTile> locationTiles = new();

        public Dictionary<Vector2, CharacterHandle.characters> dialogueTiles = new();

        public Dictionary<Vector2, int> lightFields = new();

        public Dictionary<Vector2, int> darkFields = new();

        public bool ambientDarkness;

        public Texture2D dungeonTexture;

        public Texture2D waterfallTexture;

        public Court() { }

        public Court(string Name)
            : base("Maps\\Shed", Name)
        {

        }

        public override void draw(SpriteBatch b)
        {

            base.draw(b);

            Dictionary<Vector2, int> occupied = new();

            foreach (Farmer character in farmers)
            {

                Vector2 check = ModUtility.PositionToTile(character.Position);

                for (int i = 0; i < 2; i++)
                {

                    List<Vector2> around = ModUtility.GetTilesWithinRadius(this, check, i);

                    foreach (Vector2 tile in around)
                    {

                        occupied[tile] = (int)check.Y + 1;

                    }

                }

            }

            foreach (LocationTile tile in locationTiles)
            {

                if (Utility.isOnScreen(tile.position, 64))
                {

                    float opacity = 1f;

                    if (occupied.ContainsKey(tile.tile))
                    {

                        if (occupied[tile.tile] < (int)tile.tile.Y + tile.offset)
                        {

                            opacity = 0.75f;

                        }

                    }

                    Microsoft.Xna.Framework.Vector2 position = new(tile.position.X - (float)Game1.viewport.X, tile.position.Y - (float)Game1.viewport.Y);

                    b.Draw(Mod.instance.iconData.sheetTextures[IconData.tilesheets.court], position, tile.rectangle, Microsoft.Xna.Framework.Color.White * opacity, 0f, Vector2.Zero, 4, tile.flip ? (SpriteEffects)1 : 0, tile.layer + (tile.offset * 0.0064f));

                    if (tile.shadow)
                    {

                        b.Draw(Mod.instance.iconData.sheetTextures[IconData.tilesheets.court], position + new Vector2(2, 11), tile.rectangle, Microsoft.Xna.Framework.Color.Black * 0.35f, 0f, Vector2.Zero, 4, tile.flip ? (SpriteEffects)1 : 0, tile.layer - 0.001f);

                    }

                }

            }

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
                        Microsoft.Xna.Framework.Color.LightGoldenrodYellow * 0.5f,
                        0f,
                        new Vector2(texture2D.Bounds.Width / 2, texture2D.Bounds.Height / 2),
                        0.25f * light.Value,
                        SpriteEffects.None,
                        0.9f
                    );

                }

            }


            Microsoft.Xna.Framework.Vector2 waterfallposition = new((19 *64) - (float)Game1.viewport.X,-32-(float)Game1.viewport.Y);

            int waterFallframe = (int)(Game1.currentGameTime.TotalGameTime.TotalMilliseconds % (17 * 200) / 200);

            int x = waterFallframe % 6;

            int y = (int)(waterFallframe / 6);

            Rectangle waterfallRect = new Rectangle(x * 96, 64 + (y * 112), 96, 112);

            Rectangle waterfallTop = new(waterfallRect.X, waterfallRect.Y + 32, 96, 32);

            Rectangle waterfallBottom = new(waterfallRect.X, waterfallRect.Y + 32, 96, 80);

            b.Draw(
                waterfallTexture,
                waterfallposition,
                waterfallTop,
                Color.White,
                0f,
                Vector2.Zero,
                4f,
                SpriteEffects.None,
                999f
            );

            b.Draw(
                waterfallTexture,
                waterfallposition + new Vector2(0,128),
                waterfallTop,
                Color.White,
                0f,
                Vector2.Zero,
                4f,
                SpriteEffects.None,
                999f
            );

            b.Draw(
                waterfallTexture,
                waterfallposition + new Vector2(0, 256),
                waterfallBottom,
                Color.White,
                0f,
                Vector2.Zero,
                4f,
                SpriteEffects.None,
                999f
            );

        }

        protected override void _updateAmbientLighting()
        {

            Game1.ambientLight = new(64, 64, 32);

            //base._updateAmbientLighting();

        }

        public override void drawWaterTile(SpriteBatch b, int x, int y)
        {
            base.drawWaterTile(b, x, y);  
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

            TileSheet dungeon = new(LocationData.druid_court_name + "_dungeon", newMap, "Maps\\Mines\\mine", new(16, 18), tileSize);

            newMap.AddTileSheet(dungeon); //map.TileSheets[1].ImageSource

            TileSheet dangerous = new(LocationData.druid_court_name + "_dangerous", newMap, "Maps\\Mines\\mine_frost_dangerous", new(16, 18), tileSize);

            newMap.AddTileSheet(dangerous); //map.TileSheets[1].ImageSource

            locationTiles = new();

            waterTiles = new(56, 34);

            Dictionary<int, List<List<int>>> codes = new()
            {
                [0] = new() { },
                [1] = new() { },
                [2] = new() { },
                [3] = new() { new() { 15, 218 }, new() { 16, 218 }, new() { 17, 218 }, new() { 18, 218 }, new() { 19, 218 }, new() { 20, 218 }, new() { 21, 218 }, new() { 22, 218 }, new() { 23, 218 }, new() { 24, 218 }, new() { 25, 218 }, new() { 26, 218 }, new() { 27, 218 }, new() { 28, 218 }, new() { 29, 218 }, new() { 30, 218 }, new() { 31, 218 }, new() { 32, 218 }, new() { 33, 218 }, new() { 34, 218 }, },
                [4] = new() { new() { 11, 218 }, new() { 12, 218 }, new() { 13, 218 }, new() { 14, 218 }, new() { 15, 201 }, new() { 16, 202 }, new() { 17, 188 }, new() { 18, 218 }, new() { 19, 234 }, new() { 20, 234 }, new() { 21, 234 }, new() { 22, 234 }, new() { 23, 234 }, new() { 24, 234 }, new() { 25, 234 }, new() { 26, 234 }, new() { 27, 234 }, new() { 28, 234 }, new() { 29, 234 }, new() { 30, 234 }, new() { 31, 234 }, new() { 32, 185 }, new() { 33, 218 }, new() { 34, 218 }, new() { 35, 218 }, new() { 36, 218 }, new() { 37, 187 }, new() { 38, 202 }, new() { 39, 202 }, new() { 40, 202 }, },
                [5] = new() { new() { 11, 138 }, new() { 12, 201 }, new() { 13, 202 }, new() { 14, 202 }, new() { 15, 188 }, new() { 16, 218 }, new() { 17, 218 }, new() { 18, 186 }, new() { 19, 276 }, new() { 20, 276 }, new() { 21, 276 }, new() { 22, 276 }, new() { 23, 276 }, new() { 24, 276 }, new() { 25, 138 }, new() { 26, 138 }, new() { 27, 138 }, new() { 28, 138 }, new() { 29, 138 }, new() { 30, 138 }, new() { 31, 138 }, new() { 32, 233 }, new() { 33, 234 }, new() { 34, 234 }, new() { 35, 234 }, new() { 36, 185 }, new() { 37, 218 }, new() { 38, 218 }, new() { 39, 218 }, new() { 40, 218 }, new() { 41, 187 }, new() { 42, 202 }, new() { 43, 203 }, },
                [6] = new() { new() { 8, 218 }, new() { 9, 218 }, new() { 10, 218 }, new() { 11, 201 }, new() { 12, 188 }, new() { 13, 218 }, new() { 14, 218 }, new() { 15, 186 }, new() { 16, 234 }, new() { 17, 234 }, new() { 18, 276 }, new() { 19, 276 }, new() { 20, 276 }, new() { 21, 276 }, new() { 22, 276 }, new() { 23, 276 }, new() { 24, 276 }, new() { 25, 276 }, new() { 26, 276 }, new() { 27, 138 }, new() { 28, 138 }, new() { 29, 138 }, new() { 30, 138 }, new() { 31, 138 }, new() { 32, 138 }, new() { 33, 138 }, new() { 34, 138 }, new() { 35, 138 }, new() { 36, 233 }, new() { 37, 234 }, new() { 38, 234 }, new() { 39, 185 }, new() { 40, 218 }, new() { 41, 218 }, new() { 42, 218 }, new() { 43, 187 }, },
                [7] = new() { new() { 7, 218 }, new() { 8, 188 }, new() { 9, 187 }, new() { 10, 202 }, new() { 11, 188 }, new() { 12, 218 }, new() { 13, 186 }, new() { 14, 234 }, new() { 15, 234 }, new() { 16, 276 }, new() { 17, 276 }, new() { 18, 276 }, new() { 19, 276 }, new() { 20, 276 }, new() { 21, 276 }, new() { 22, 276 }, new() { 23, 276 }, new() { 24, 276 }, new() { 25, 276 }, new() { 26, 276 }, new() { 27, 276 }, new() { 28, 138 }, new() { 29, 138 }, new() { 30, 138 }, new() { 31, 138 }, new() { 32, 138 }, new() { 33, 138 }, new() { 34, 138 }, new() { 35, 138 }, new() { 36, 138 }, new() { 37, 138 }, new() { 38, 140 }, new() { 39, 233 }, new() { 40, 234 }, new() { 41, 185 }, new() { 42, 218 }, new() { 43, 218 }, new() { 44, 187 }, new() { 45, 202 }, new() { 46, 203 }, },
                [8] = new() { new() { 7, 218 }, new() { 8, 218 }, new() { 9, 218 }, new() { 10, 218 }, new() { 11, 186 }, new() { 12, 234 }, new() { 13, 235 }, new() { 14, 276 }, new() { 15, 276 }, new() { 16, 276 }, new() { 17, 276 }, new() { 18, 276 }, new() { 19, 276 }, new() { 20, 276 }, new() { 21, 276 }, new() { 22, 276 }, new() { 23, 276 }, new() { 24, 276 }, new() { 25, 276 }, new() { 26, 276 }, new() { 27, 276 }, new() { 28, 138 }, new() { 29, 138 }, new() { 30, 138 }, new() { 31, 138 }, new() { 32, 138 }, new() { 33, 138 }, new() { 34, 138 }, new() { 35, 138 }, new() { 36, 138 }, new() { 37, 138 }, new() { 38, 138 }, new() { 39, 138 }, new() { 40, 138 }, new() { 41, 233 }, new() { 42, 234 }, new() { 43, 185 }, new() { 44, 218 }, new() { 45, 218 }, new() { 46, 187 }, new() { 47, 202 }, new() { 48, 202 }, },
                [9] = new() { new() { 7, 218 }, new() { 8, 186 }, new() { 9, 234 }, new() { 10, 234 }, new() { 11, 235 }, new() { 12, 276 }, new() { 13, 276 }, new() { 14, 276 }, new() { 15, 276 }, new() { 16, 276 }, new() { 17, 276 }, new() { 18, 276 }, new() { 19, 276 }, new() { 20, 276 }, new() { 21, 276 }, new() { 22, 276 }, new() { 23, 276 }, new() { 24, 276 }, new() { 25, 276 }, new() { 26, 276 }, new() { 27, 276 }, new() { 28, 138 }, new() { 29, 138 }, new() { 30, 138 }, new() { 31, 138 }, new() { 32, 138 }, new() { 33, 138 }, new() { 34, 138 }, new() { 35, 138 }, new() { 36, 138 }, new() { 37, 138 }, new() { 38, 138 }, new() { 39, 138 }, new() { 40, 138 }, new() { 41, 138 }, new() { 42, 138 }, new() { 43, 233 }, new() { 44, 234 }, new() { 45, 185 }, new() { 46, 218 }, new() { 47, 218 }, new() { 48, 218 }, },
                [10] = new() { new() { 6, 218 }, new() { 7, 218 }, new() { 8, 219 }, new() { 9, 138 }, new() { 10, 276 }, new() { 11, 276 }, new() { 12, 276 }, new() { 13, 276 }, new() { 14, 276 }, new() { 15, 276 }, new() { 16, 276 }, new() { 17, 276 }, new() { 18, 276 }, new() { 19, 276 }, new() { 20, 276 }, new() { 21, 276 }, new() { 22, 276 }, new() { 23, 276 }, new() { 24, 276 }, new() { 25, 276 }, new() { 26, 276 }, new() { 27, 138 }, new() { 28, 138 }, new() { 29, 138 }, new() { 30, 138 }, new() { 31, 138 }, new() { 32, 138 }, new() { 33, 138 }, new() { 34, 138 }, new() { 35, 138 }, new() { 36, 138 }, new() { 37, 138 }, new() { 38, 138 }, new() { 39, 138 }, new() { 40, 138 }, new() { 41, 138 }, new() { 42, 138 }, new() { 43, 138 }, new() { 44, 138 }, new() { 45, 233 }, new() { 46, 234 }, new() { 47, 185 }, new() { 48, 218 }, },
                [11] = new() { new() { 6, 218 }, new() { 7, 186 }, new() { 8, 235 }, new() { 9, 138 }, new() { 10, 276 }, new() { 11, 276 }, new() { 12, 276 }, new() { 13, 276 }, new() { 14, 276 }, new() { 15, 276 }, new() { 16, 276 }, new() { 17, 276 }, new() { 18, 276 }, new() { 19, 276 }, new() { 20, 276 }, new() { 21, 276 }, new() { 22, 276 }, new() { 23, 276 }, new() { 24, 276 }, new() { 25, 276 }, new() { 26, 138 }, new() { 27, 138 }, new() { 28, 138 }, new() { 29, 138 }, new() { 30, 138 }, new() { 31, 138 }, new() { 32, 138 }, new() { 33, 138 }, new() { 34, 138 }, new() { 35, 138 }, new() { 36, 138 }, new() { 37, 138 }, new() { 38, 138 }, new() { 39, 138 }, new() { 40, 138 }, new() { 41, 138 }, new() { 42, 138 }, new() { 43, 233 }, new() { 44, 138 }, new() { 45, 138 }, new() { 46, 138 }, new() { 47, 217 }, new() { 48, 218 }, new() { 49, 219 }, },
                [12] = new() { new() { 6, 218 }, new() { 7, 219 }, new() { 8, 138 }, new() { 9, 138 }, new() { 10, 276 }, new() { 11, 276 }, new() { 12, 276 }, new() { 13, 276 }, new() { 14, 276 }, new() { 15, 138 }, new() { 16, 138 }, new() { 17, 138 }, new() { 18, 276 }, new() { 19, 276 }, new() { 20, 276 }, new() { 21, 276 }, new() { 22, 276 }, new() { 23, 276 }, new() { 24, 276 }, new() { 25, 276 }, new() { 26, 138 }, new() { 27, 138 }, new() { 28, 138 }, new() { 29, 138 }, new() { 30, 138 }, new() { 31, 166 }, new() { 32, 167 }, new() { 33, 167 }, new() { 34, 167 }, new() { 35, 167 }, new() { 36, 168 }, new() { 37, 154 }, new() { 38, 166 }, new() { 39, 167 }, new() { 40, 168 }, new() { 41, 138 }, new() { 42, 138 }, new() { 43, 138 }, new() { 44, 138 }, new() { 45, 138 }, new() { 46, 138 }, new() { 47, 217 }, new() { 48, 218 }, new() { 49, 219 }, },
                [13] = new() { new() { 6, 218 }, new() { 7, 219 }, new() { 8, 138 }, new() { 9, 138 }, new() { 10, 276 }, new() { 11, 276 }, new() { 12, 276 }, new() { 13, 276 }, new() { 14, 138 }, new() { 15, 138 }, new() { 16, 138 }, new() { 17, 138 }, new() { 18, 138 }, new() { 19, 138 }, new() { 20, 276 }, new() { 21, 276 }, new() { 22, 276 }, new() { 23, 276 }, new() { 24, 138 }, new() { 25, 138 }, new() { 26, 138 }, new() { 27, 138 }, new() { 28, 138 }, new() { 29, 166 }, new() { 30, 167 }, new() { 31, 152 }, new() { 32, 183 }, new() { 33, 183 }, new() { 34, 183 }, new() { 35, 183 }, new() { 36, 151 }, new() { 37, 168 }, new() { 38, 182 }, new() { 39, 183 }, new() { 40, 184 }, new() { 41, 138 }, new() { 42, 138 }, new() { 43, 233 }, new() { 44, 138 }, new() { 45, 138 }, new() { 46, 138 }, new() { 47, 217 }, new() { 48, 218 }, new() { 49, 219 }, },
                [14] = new() { new() { 6, 218 }, new() { 7, 219 }, new() { 8, 138 }, new() { 9, 276 }, new() { 10, 276 }, new() { 11, 276 }, new() { 12, 276 }, new() { 13, 276 }, new() { 14, 138 }, new() { 15, 138 }, new() { 16, 138 }, new() { 17, 138 }, new() { 18, 138 }, new() { 19, 138 }, new() { 20, 138 }, new() { 21, 138 }, new() { 22, 138 }, new() { 23, 138 }, new() { 24, 138 }, new() { 25, 138 }, new() { 26, 138 }, new() { 27, 138 }, new() { 28, 166 }, new() { 29, 152 }, new() { 30, 165 }, new() { 31, 183 }, new() { 32, 165 }, new() { 33, 165 }, new() { 34, 183 }, new() { 35, 183 }, new() { 36, 183 }, new() { 37, 151 }, new() { 38, 152 }, new() { 39, 183 }, new() { 40, 151 }, new() { 41, 167 }, new() { 42, 168 }, new() { 43, 153 }, new() { 44, 138 }, new() { 45, 138 }, new() { 46, 138 }, new() { 47, 233 }, new() { 48, 185 }, new() { 49, 187 }, },
                [15] = new() { new() { 5, 218 }, new() { 6, 186 }, new() { 7, 235 }, new() { 8, 138 }, new() { 9, 276 }, new() { 10, 276 }, new() { 11, 276 }, new() { 12, 276 }, new() { 13, 276 }, new() { 14, 138 }, new() { 15, 138 }, new() { 16, 138 }, new() { 17, 138 }, new() { 18, 138 }, new() { 19, 138 }, new() { 20, 138 }, new() { 21, 138 }, new() { 22, 138 }, new() { 23, 138 }, new() { 24, 138 }, new() { 25, 166 }, new() { 26, 167 }, new() { 27, 167 }, new() { 28, 152 }, new() { 29, 183 }, new() { 30, 183 }, new() { 31, 183 }, new() { 32, 165 }, new() { 33, 165 }, new() { 34, 183 }, new() { 35, 165 }, new() { 36, 165 }, new() { 37, 165 }, new() { 38, 183 }, new() { 39, 183 }, new() { 40, 150 }, new() { 41, 149 }, new() { 42, 151 }, new() { 43, 168 }, new() { 44, 138 }, new() { 45, 138 }, new() { 46, 138 }, new() { 47, 138 }, new() { 48, 217 }, new() { 49, 218 }, new() { 50, 219 }, },
                [16] = new() { new() { 5, 218 }, new() { 6, 219 }, new() { 7, 138 }, new() { 8, 138 }, new() { 9, 276 }, new() { 10, 276 }, new() { 11, 276 }, new() { 12, 276 }, new() { 13, 138 }, new() { 14, 138 }, new() { 15, 138 }, new() { 16, 138 }, new() { 17, 138 }, new() { 18, 138 }, new() { 19, 138 }, new() { 20, 138 }, new() { 21, 166 }, new() { 22, 167 }, new() { 23, 167 }, new() { 24, 167 }, new() { 25, 152 }, new() { 26, 165 }, new() { 27, 183 }, new() { 28, 183 }, new() { 29, 183 }, new() { 30, 183 }, new() { 31, 183 }, new() { 32, 165 }, new() { 33, 165 }, new() { 34, 183 }, new() { 35, 165 }, new() { 36, 165 }, new() { 37, 165 }, new() { 38, 183 }, new() { 39, 183 }, new() { 40, 151 }, new() { 41, 152 }, new() { 42, 183 }, new() { 43, 151 }, new() { 44, 167 }, new() { 45, 168 }, new() { 46, 138 }, new() { 47, 138 }, new() { 48, 217 }, new() { 49, 218 }, new() { 50, 219 }, },
                [17] = new() { new() { 4, 218 }, new() { 5, 218 }, new() { 6, 235 }, new() { 7, 138 }, new() { 8, 276 }, new() { 9, 276 }, new() { 10, 276 }, new() { 11, 276 }, new() { 12, 276 }, new() { 13, 138 }, new() { 14, 138 }, new() { 15, 138 }, new() { 16, 138 }, new() { 17, 138 }, new() { 18, 138 }, new() { 19, 138 }, new() { 20, 166 }, new() { 21, 152 }, new() { 22, 183 }, new() { 23, 183 }, new() { 24, 183 }, new() { 25, 183 }, new() { 26, 183 }, new() { 27, 183 }, new() { 28, 183 }, new() { 29, 183 }, new() { 30, 183 }, new() { 31, 165 }, new() { 32, 183 }, new() { 33, 183 }, new() { 34, 183 }, new() { 35, 165 }, new() { 36, 183 }, new() { 37, 165 }, new() { 38, 183 }, new() { 39, 165 }, new() { 40, 183 }, new() { 41, 183 }, new() { 42, 183 }, new() { 43, 183 }, new() { 44, 183 }, new() { 45, 151 }, new() { 46, 168 }, new() { 47, 138 }, new() { 48, 217 }, new() { 49, 218 }, new() { 50, 187 }, },
                [18] = new() { new() { 4, 138 }, new() { 5, 138 }, new() { 6, 138 }, new() { 7, 138 }, new() { 8, 276 }, new() { 9, 276 }, new() { 10, 276 }, new() { 11, 276 }, new() { 12, 276 }, new() { 13, 138 }, new() { 14, 138 }, new() { 15, 138 }, new() { 16, 138 }, new() { 17, 138 }, new() { 18, 138 }, new() { 19, 166 }, new() { 20, 152 }, new() { 21, 183 }, new() { 22, 165 }, new() { 23, 181 }, new() { 24, 165 }, new() { 25, 165 }, new() { 26, 165 }, new() { 27, 165 }, new() { 28, 165 }, new() { 29, 165 }, new() { 30, 165 }, new() { 31, 165 }, new() { 32, 165 }, new() { 33, 165 }, new() { 34, 165 }, new() { 35, 165 }, new() { 36, 165 }, new() { 37, 165 }, new() { 38, 165 }, new() { 39, 165 }, new() { 40, 165 }, new() { 41, 165 }, new() { 42, 165 }, new() { 43, 183 }, new() { 44, 183 }, new() { 45, 183 }, new() { 46, 184 }, new() { 47, 138 }, new() { 48, 217 }, new() { 49, 217 }, new() { 50, 218 }, },
                [19] = new() { new() { 5, 138 }, new() { 6, 169 }, new() { 7, 138 }, new() { 8, 276 }, new() { 9, 276 }, new() { 10, 276 }, new() { 11, 276 }, new() { 12, 276 }, new() { 13, 138 }, new() { 14, 138 }, new() { 15, 138 }, new() { 16, 138 }, new() { 17, 138 }, new() { 18, 138 }, new() { 19, 182 }, new() { 20, 183 }, new() { 21, 183 }, new() { 22, 183 }, new() { 23, 183 }, new() { 24, 183 }, new() { 25, 183 }, new() { 26, 181 }, new() { 27, 165 }, new() { 28, 183 }, new() { 29, 165 }, new() { 30, 165 }, new() { 31, 165 }, new() { 32, 165 }, new() { 33, 183 }, new() { 34, 165 }, new() { 35, 165 }, new() { 36, 183 }, new() { 37, 165 }, new() { 38, 183 }, new() { 39, 165 }, new() { 40, 183 }, new() { 41, 183 }, new() { 42, 165 }, new() { 43, 183 }, new() { 44, 165 }, new() { 45, 183 }, new() { 46, 184 }, new() { 47, 153 }, new() { 48, 233 }, new() { 49, 185 }, new() { 50, 218 }, },
                [20] = new() { new() { 5, 138 }, new() { 6, 138 }, new() { 7, 138 }, new() { 8, 276 }, new() { 9, 276 }, new() { 10, 276 }, new() { 11, 276 }, new() { 12, 138 }, new() { 13, 138 }, new() { 14, 138 }, new() { 15, 138 }, new() { 16, 138 }, new() { 17, 138 }, new() { 18, 138 }, new() { 19, 198 }, new() { 20, 149 }, new() { 21, 183 }, new() { 22, 183 }, new() { 23, 183 }, new() { 24, 150 }, new() { 25, 149 }, new() { 26, 183 }, new() { 27, 183 }, new() { 28, 183 }, new() { 29, 183 }, new() { 30, 183 }, new() { 31, 165 }, new() { 32, 165 }, new() { 33, 183 }, new() { 34, 165 }, new() { 35, 183 }, new() { 36, 165 }, new() { 37, 183 }, new() { 38, 165 }, new() { 39, 183 }, new() { 40, 165 }, new() { 41, 165 }, new() { 42, 165 }, new() { 43, 165 }, new() { 44, 165 }, new() { 45, 183 }, new() { 46, 151 }, new() { 47, 167 }, new() { 48, 168 }, new() { 49, 217 }, new() { 50, 218 }, },
                [21] = new() { new() { 6, 138 }, new() { 7, 138 }, new() { 8, 276 }, new() { 9, 276 }, new() { 10, 276 }, new() { 11, 276 }, new() { 12, 138 }, new() { 13, 138 }, new() { 14, 138 }, new() { 15, 138 }, new() { 16, 138 }, new() { 17, 138 }, new() { 18, 138 }, new() { 19, 138 }, new() { 20, 182 }, new() { 21, 183 }, new() { 22, 183 }, new() { 23, 183 }, new() { 24, 151 }, new() { 25, 152 }, new() { 26, 183 }, new() { 27, 183 }, new() { 28, 165 }, new() { 29, 165 }, new() { 30, 165 }, new() { 31, 183 }, new() { 32, 183 }, new() { 33, 165 }, new() { 34, 165 }, new() { 35, 183 }, new() { 36, 183 }, new() { 37, 183 }, new() { 38, 183 }, new() { 39, 183 }, new() { 40, 165 }, new() { 41, 165 }, new() { 42, 165 }, new() { 43, 165 }, new() { 44, 165 }, new() { 45, 183 }, new() { 46, 183 }, new() { 47, 183 }, new() { 48, 184 }, new() { 49, 188 }, },
                [22] = new() { new() { 6, 138 }, new() { 7, 138 }, new() { 8, 276 }, new() { 9, 276 }, new() { 10, 276 }, new() { 11, 276 }, new() { 12, 138 }, new() { 13, 138 }, new() { 14, 138 }, new() { 15, 138 }, new() { 16, 138 }, new() { 17, 138 }, new() { 18, 138 }, new() { 19, 138 }, new() { 20, 198 }, new() { 21, 149 }, new() { 22, 183 }, new() { 23, 183 }, new() { 24, 183 }, new() { 25, 183 }, new() { 26, 183 }, new() { 27, 183 }, new() { 28, 165 }, new() { 29, 165 }, new() { 30, 165 }, new() { 31, 183 }, new() { 32, 183 }, new() { 33, 183 }, new() { 34, 165 }, new() { 35, 165 }, new() { 36, 165 }, new() { 37, 183 }, new() { 38, 183 }, new() { 39, 183 }, new() { 40, 183 }, new() { 41, 165 }, new() { 42, 165 }, new() { 43, 165 }, new() { 44, 165 }, new() { 45, 183 }, new() { 46, 150 }, new() { 47, 199 }, new() { 48, 200 }, new() { 49, 218 }, },
                [23] = new() { new() { 7, 138 }, new() { 8, 276 }, new() { 9, 276 }, new() { 10, 276 }, new() { 11, 276 }, new() { 12, 138 }, new() { 13, 138 }, new() { 14, 138 }, new() { 15, 138 }, new() { 16, 138 }, new() { 17, 138 }, new() { 18, 138 }, new() { 19, 138 }, new() { 20, 138 }, new() { 21, 182 }, new() { 22, 183 }, new() { 23, 183 }, new() { 24, 183 }, new() { 25, 165 }, new() { 26, 183 }, new() { 27, 165 }, new() { 28, 165 }, new() { 29, 183 }, new() { 30, 183 }, new() { 31, 165 }, new() { 32, 165 }, new() { 33, 165 }, new() { 34, 183 }, new() { 35, 165 }, new() { 36, 165 }, new() { 37, 181 }, new() { 38, 183 }, new() { 39, 150 }, new() { 40, 149 }, new() { 41, 183 }, new() { 42, 165 }, new() { 43, 183 }, new() { 44, 181 }, new() { 45, 150 }, new() { 46, 200 }, new() { 47, 201 }, new() { 48, 188 }, },
                [24] = new() { new() { 7, 138 }, new() { 8, 276 }, new() { 9, 276 }, new() { 10, 276 }, new() { 11, 276 }, new() { 12, 138 }, new() { 13, 138 }, new() { 14, 138 }, new() { 15, 138 }, new() { 16, 138 }, new() { 17, 138 }, new() { 18, 138 }, new() { 19, 138 }, new() { 20, 138 }, new() { 21, 198 }, new() { 22, 199 }, new() { 23, 149 }, new() { 24, 183 }, new() { 25, 165 }, new() { 26, 165 }, new() { 27, 165 }, new() { 28, 165 }, new() { 29, 183 }, new() { 30, 183 }, new() { 31, 165 }, new() { 32, 165 }, new() { 33, 165 }, new() { 34, 183 }, new() { 35, 165 }, new() { 36, 165 }, new() { 37, 183 }, new() { 38, 183 }, new() { 39, 151 }, new() { 40, 152 }, new() { 41, 183 }, new() { 42, 183 }, new() { 43, 183 }, new() { 44, 183 }, new() { 45, 184 }, new() { 46, 169 }, new() { 47, 217 }, new() { 48, 218 }, },
                [25] = new() { new() { 8, 276 }, new() { 9, 276 }, new() { 10, 276 }, new() { 11, 276 }, new() { 12, 138 }, new() { 13, 138 }, new() { 14, 138 }, new() { 15, 138 }, new() { 16, 138 }, new() { 17, 138 }, new() { 18, 138 }, new() { 19, 138 }, new() { 20, 138 }, new() { 21, 138 }, new() { 22, 138 }, new() { 23, 198 }, new() { 24, 199 }, new() { 25, 149 }, new() { 26, 183 }, new() { 27, 183 }, new() { 28, 183 }, new() { 29, 183 }, new() { 30, 183 }, new() { 31, 183 }, new() { 32, 183 }, new() { 33, 183 }, new() { 34, 183 }, new() { 35, 183 }, new() { 36, 181 }, new() { 37, 165 }, new() { 38, 183 }, new() { 39, 183 }, new() { 40, 183 }, new() { 41, 183 }, new() { 42, 150 }, new() { 43, 199 }, new() { 44, 199 }, new() { 45, 200 }, new() { 46, 201 }, new() { 47, 188 }, },
                [26] = new() { new() { 8, 276 }, new() { 9, 276 }, new() { 10, 276 }, new() { 11, 276 }, new() { 12, 276 }, new() { 13, 138 }, new() { 14, 138 }, new() { 15, 138 }, new() { 16, 138 }, new() { 17, 138 }, new() { 18, 138 }, new() { 19, 138 }, new() { 20, 138 }, new() { 21, 138 }, new() { 22, 138 }, new() { 23, 138 }, new() { 24, 138 }, new() { 25, 198 }, new() { 26, 149 }, new() { 27, 183 }, new() { 28, 183 }, new() { 29, 183 }, new() { 30, 183 }, new() { 31, 183 }, new() { 32, 183 }, new() { 33, 183 }, new() { 34, 183 }, new() { 35, 183 }, new() { 36, 183 }, new() { 37, 183 }, new() { 38, 183 }, new() { 39, 183 }, new() { 40, 183 }, new() { 41, 183 }, new() { 42, 184 }, new() { 43, 153 }, new() { 44, 139 }, new() { 45, 138 }, new() { 46, 217 }, new() { 47, 218 }, },
                [27] = new() { new() { 10, 276 }, new() { 11, 276 }, new() { 12, 276 }, new() { 13, 276 }, new() { 14, 138 }, new() { 15, 138 }, new() { 16, 138 }, new() { 17, 138 }, new() { 18, 138 }, new() { 19, 138 }, new() { 20, 138 }, new() { 21, 138 }, new() { 22, 138 }, new() { 23, 138 }, new() { 24, 33 }, new() { 25, 35 }, new() { 26, 198 }, new() { 27, 149 }, new() { 28, 183 }, new() { 29, 183 }, new() { 30, 150 }, new() { 31, 199 }, new() { 32, 199 }, new() { 33, 199 }, new() { 34, 199 }, new() { 35, 199 }, new() { 36, 149 }, new() { 37, 183 }, new() { 38, 150 }, new() { 39, 199 }, new() { 40, 199 }, new() { 41, 199 }, new() { 42, 200 }, new() { 43, 138 }, new() { 44, 138 }, new() { 45, 138 }, },
                [28] = new() { new() { 12, 276 }, new() { 13, 276 }, new() { 14, 170 }, new() { 15, 171 }, new() { 16, 138 }, new() { 17, 138 }, new() { 18, 139 }, new() { 19, 169 }, new() { 20, 138 }, new() { 21, 138 }, new() { 22, 138 }, new() { 23, 138 }, new() { 24, 138 }, new() { 25, 138 }, new() { 26, 138 }, new() { 27, 182 }, new() { 28, 183 }, new() { 29, 183 }, new() { 30, 184 }, new() { 31, 138 }, new() { 32, 138 }, new() { 33, 34 }, new() { 34, 35 }, new() { 35, 140 }, new() { 36, 198 }, new() { 37, 199 }, new() { 38, 200 }, new() { 39, 138 }, new() { 40, 153 }, new() { 41, 201 }, new() { 42, 202 }, new() { 43, 203 }, new() { 44, 138 }, new() { 45, 138 }, },
                [29] = new() { new() { 14, 138 }, new() { 15, 138 }, new() { 16, 138 }, new() { 17, 138 }, new() { 18, 138 }, new() { 19, 138 }, new() { 20, 138 }, new() { 21, 138 }, new() { 22, 138 }, new() { 23, 138 }, new() { 24, 138 }, new() { 25, 138 }, new() { 26, 138 }, new() { 27, 182 }, new() { 28, 183 }, new() { 29, 183 }, new() { 30, 184 }, new() { 31, 138 }, new() { 32, 138 }, new() { 33, 138 }, new() { 34, 138 }, new() { 35, 138 }, new() { 36, 138 }, new() { 37, 201 }, new() { 38, 202 }, new() { 39, 203 }, new() { 40, 138 }, new() { 41, 217 }, new() { 42, 218 }, },
                [30] = new() { new() { 17, 218 }, new() { 18, 218 }, new() { 19, 218 }, new() { 20, 218 }, new() { 21, 218 }, new() { 22, 218 }, new() { 23, 170 }, new() { 24, 171 }, new() { 25, 218 }, new() { 26, 138 }, new() { 27, 182 }, new() { 28, 183 }, new() { 29, 183 }, new() { 30, 184 }, new() { 31, 138 }, new() { 32, 201 }, new() { 33, 202 }, new() { 34, 218 }, new() { 35, 218 }, new() { 36, 218 }, new() { 37, 218 }, new() { 38, 218 }, },
                [31] = new() { new() { 25, 218 }, new() { 26, 138 }, new() { 27, 182 }, new() { 28, 183 }, new() { 29, 183 }, new() { 30, 184 }, new() { 31, 138 }, new() { 32, 218 }, },
                [32] = new() { new() { 25, 218 }, new() { 26, 138 }, new() { 27, 182 }, new() { 28, 183 }, new() { 29, 183 }, new() { 30, 184 }, new() { 31, 138 }, new() { 32, 218 }, },
                [33] = new() { new() { 25, 218 }, new() { 26, 138 }, new() { 27, 182 }, new() { 28, 183 }, new() { 29, 183 }, new() { 30, 184 }, new() { 31, 138 }, new() { 32, 218 }, },



            };

            foreach (KeyValuePair<int, List<List<int>>> code in codes)
            {

                foreach (List<int> array in code.Value)
                {

                    back.Tiles[array[0], code.Key] = new StaticTile(back, dangerous, BlendMode.Alpha, array[1]);

                    if (array[1] == 276)
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
                [0] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 77 }, new() { 7, 77 }, new() { 8, 77 }, new() { 9, 77 }, new() { 10, 77 }, new() { 11, 77 }, new() { 12, 77 }, new() { 13, 77 }, new() { 14, 77 }, new() { 15, 77 }, new() { 16, 77 }, new() { 17, 77 }, new() { 18, 77 }, new() { 19, 73 }, new() { 20, 73 }, new() { 21, 74 }, new() { 22, 73 }, new() { 23, 74 }, new() { 24, 73 }, new() { 25, 74 }, new() { 26, 73 }, new() { 27, 85 }, new() { 28, 4 }, new() { 29, 5 }, new() { 30, 6 }, new() { 31, 93 }, new() { 32, 94 }, new() { 34, 77 }, new() { 35, 77 }, new() { 36, 77 }, new() { 37, 77 }, new() { 38, 77 }, new() { 39, 77 }, new() { 40, 77 }, new() { 41, 77 }, new() { 42, 77 }, new() { 43, 77 }, new() { 44, 77 }, new() { 45, 77 }, new() { 46, 77 }, new() { 47, 77 }, new() { 48, 77 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [1] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 77 }, new() { 7, 77 }, new() { 8, 77 }, new() { 9, 77 }, new() { 10, 77 }, new() { 11, 77 }, new() { 12, 77 }, new() { 13, 77 }, new() { 14, 77 }, new() { 15, 77 }, new() { 16, 77 }, new() { 17, 69 }, new() { 18, 70 }, new() { 19, 126 }, new() { 20, 126 }, new() { 21, 126 }, new() { 22, 126 }, new() { 23, 126 }, new() { 24, 126 }, new() { 25, 126 }, new() { 26, 65 }, new() { 27, 66 }, new() { 28, 20 }, new() { 29, 21 }, new() { 30, 22 }, new() { 31, 109 }, new() { 32, 110 }, new() { 33, 74 }, new() { 34, 74 }, new() { 35, 74 }, new() { 36, 74 }, new() { 37, 93 }, new() { 38, 94 }, new() { 39, 77 }, new() { 40, 77 }, new() { 41, 77 }, new() { 42, 77 }, new() { 43, 77 }, new() { 44, 77 }, new() { 45, 77 }, new() { 46, 77 }, new() { 47, 77 }, new() { 48, 77 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [2] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 77 }, new() { 7, 77 }, new() { 8, 77 }, new() { 9, 77 }, new() { 10, 77 }, new() { 11, 77 }, new() { 12, 77 }, new() { 13, 77 }, new() { 14, 69 }, new() { 15, 70 }, new() { 16, 7 }, new() { 17, 85 }, new() { 18, 86 }, new() { 19, 142 }, new() { 20, 142 }, new() { 21, 142 }, new() { 22, 142 }, new() { 23, 142 }, new() { 24, 142 }, new() { 25, 142 }, new() { 26, 81 }, new() { 27, 82 }, new() { 28, 36 }, new() { 29, 37 }, new() { 30, 38 }, new() { 31, 125 }, new() { 32, 126 }, new() { 33, 126 }, new() { 34, 65 }, new() { 35, 66 }, new() { 36, 126 }, new() { 37, 109 }, new() { 38, 110 }, new() { 39, 110 }, new() { 40, 93 }, new() { 41, 94 }, new() { 42, 77 }, new() { 43, 77 }, new() { 44, 77 }, new() { 45, 77 }, new() { 46, 77 }, new() { 47, 77 }, new() { 48, 77 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [3] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 77 }, new() { 7, 77 }, new() { 8, 77 }, new() { 9, 77 }, new() { 10, 77 }, new() { 11, 77 }, new() { 12, 69 }, new() { 13, 70 }, new() { 14, 85 }, new() { 15, 86 }, new() { 16, 23 }, new() { 17, 98 }, new() { 18, 102 }, new() { 19, 158 }, new() { 20, 158 }, new() { 21, 158 }, new() { 22, 158 }, new() { 23, 158 }, new() { 24, 158 }, new() { 25, 158 }, new() { 26, 121 }, new() { 27, 133 }, new() { 28, 52 }, new() { 29, 53 }, new() { 30, 54 }, new() { 31, 141 }, new() { 32, 142 }, new() { 33, 142 }, new() { 34, 81 }, new() { 35, 82 }, new() { 36, 142 }, new() { 37, 125 }, new() { 38, 126 }, new() { 39, 126 }, new() { 40, 109 }, new() { 41, 110 }, new() { 42, 93 }, new() { 43, 94 }, new() { 44, 77 }, new() { 45, 77 }, new() { 46, 77 }, new() { 47, 77 }, new() { 48, 77 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [4] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 77 }, new() { 7, 68 }, new() { 8, 93 }, new() { 9, 94 }, new() { 10, 69 }, new() { 11, 70 }, new() { 12, 85 }, new() { 13, 86 }, new() { 14, 101 }, new() { 15, 102 }, new() { 16, 39 }, new() { 17, 114 }, new() { 18, 118 }, new() { 31, 157 }, new() { 32, 158 }, new() { 33, 158 }, new() { 34, 158 }, new() { 35, 122 }, new() { 36, 158 }, new() { 37, 141 }, new() { 38, 142 }, new() { 39, 142 }, new() { 40, 125 }, new() { 41, 126 }, new() { 42, 109 }, new() { 43, 110 }, new() { 44, 93 }, new() { 45, 94 }, new() { 46, 77 }, new() { 47, 77 }, new() { 48, 77 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [5] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 77 }, new() { 7, 84 }, new() { 8, 109 }, new() { 9, 110 }, new() { 10, 85 }, new() { 11, 86 }, new() { 12, 101 }, new() { 13, 97 }, new() { 14, 117 }, new() { 15, 118 }, new() { 16, 55 }, new() { 17, 133 }, new() { 18, 134 }, new() { 37, 157 }, new() { 38, 158 }, new() { 39, 158 }, new() { 40, 141 }, new() { 41, 142 }, new() { 42, 113 }, new() { 43, 126 }, new() { 44, 109 }, new() { 45, 110 }, new() { 46, 93 }, new() { 47, 94 }, new() { 48, 77 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [6] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 77 }, new() { 7, 100 }, new() { 8, 125 }, new() { 9, 126 }, new() { 10, 98 }, new() { 11, 102 }, new() { 12, 117 }, new() { 13, 118 }, new() { 14, 133 }, new() { 15, 134 }, new() { 40, 157 }, new() { 41, 158 }, new() { 42, 141 }, new() { 43, 142 }, new() { 44, 125 }, new() { 45, 126 }, new() { 46, 109 }, new() { 47, 110 }, new() { 48, 111 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [7] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 77 }, new() { 7, 116 }, new() { 8, 141 }, new() { 9, 142 }, new() { 10, 114 }, new() { 11, 118 }, new() { 12, 133 }, new() { 13, 134 }, new() { 42, 157 }, new() { 43, 158 }, new() { 44, 141 }, new() { 45, 142 }, new() { 46, 125 }, new() { 47, 126 }, new() { 48, 127 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [8] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 68 }, new() { 7, 71 }, new() { 8, 157 }, new() { 9, 158 }, new() { 10, 133 }, new() { 11, 134 }, new() { 44, 157 }, new() { 45, 158 }, new() { 46, 141 }, new() { 47, 142 }, new() { 48, 143 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [9] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 84 }, new() { 7, 97 }, new() { 46, 157 }, new() { 47, 158 }, new() { 48, 159 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [10] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 100 }, new() { 7, 103 }, new() { 48, 72 }, new() { 49, 111 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [11] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 116 }, new() { 7, 119 }, new() { 48, 113 }, new() { 49, 127 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [12] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 68 }, new() { 6, 71 }, new() { 48, 104 }, new() { 49, 143 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [13] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 84 }, new() { 6, 87 }, new() { 48, 120 }, new() { 49, 159 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [14] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 100 }, new() { 6, 103 }, new() { 49, 72 }, new() { 50, 111 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [15] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 116 }, new() { 6, 119 }, new() { 49, 88 }, new() { 50, 127 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [16] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 68 }, new() { 5, 71 }, new() { 49, 104 }, new() { 50, 143 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [17] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 84 }, new() { 5, 87 }, new() { 49, 120 }, new() { 50, 159 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [18] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 100 }, new() { 5, 103 }, new() { 50, 175 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [19] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 116 }, new() { 5, 119 }, new() { 50, 191 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [20] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 50, 206 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [21] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 232 }, new() { 49, 236 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [22] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 77 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [23] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 77 }, new() { 7, 232 }, new() { 48, 236 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [24] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 77 }, new() { 7, 77 }, new() { 48, 77 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [25] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 77 }, new() { 7, 77 }, new() { 8, 77 }, new() { 47, 236 }, new() { 48, 77 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [26] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 77 }, new() { 7, 77 }, new() { 8, 77 }, new() { 9, 77 }, new() { 47, 77 }, new() { 48, 77 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [27] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 77 }, new() { 7, 77 }, new() { 8, 77 }, new() { 9, 77 }, new() { 10, 77 }, new() { 46, 77 }, new() { 47, 77 }, new() { 48, 77 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [28] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 77 }, new() { 7, 77 }, new() { 8, 77 }, new() { 9, 77 }, new() { 10, 77 }, new() { 11, 77 }, new() { 12, 77 }, new() { 45, 77 }, new() { 46, 77 }, new() { 47, 77 }, new() { 48, 77 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [29] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 77 }, new() { 7, 77 }, new() { 8, 77 }, new() { 9, 77 }, new() { 10, 77 }, new() { 11, 77 }, new() { 12, 77 }, new() { 13, 77 }, new() { 43, 77 }, new() { 44, 77 }, new() { 45, 77 }, new() { 46, 77 }, new() { 47, 77 }, new() { 48, 77 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [30] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 77 }, new() { 7, 77 }, new() { 8, 77 }, new() { 9, 77 }, new() { 10, 77 }, new() { 11, 77 }, new() { 12, 77 }, new() { 13, 77 }, new() { 14, 77 }, new() { 15, 77 }, new() { 16, 77 }, new() { 39, 77 }, new() { 40, 77 }, new() { 41, 77 }, new() { 42, 77 }, new() { 43, 77 }, new() { 44, 77 }, new() { 45, 77 }, new() { 46, 77 }, new() { 47, 77 }, new() { 48, 77 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [31] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 77 }, new() { 7, 77 }, new() { 8, 77 }, new() { 9, 77 }, new() { 10, 77 }, new() { 11, 77 }, new() { 12, 77 }, new() { 13, 77 }, new() { 14, 77 }, new() { 15, 77 }, new() { 16, 77 }, new() { 17, 77 }, new() { 18, 77 }, new() { 19, 77 }, new() { 20, 77 }, new() { 21, 77 }, new() { 22, 77 }, new() { 23, 77 }, new() { 24, 77 }, new() { 33, 77 }, new() { 34, 77 }, new() { 35, 77 }, new() { 36, 77 }, new() { 37, 77 }, new() { 38, 77 }, new() { 39, 77 }, new() { 40, 77 }, new() { 41, 77 }, new() { 42, 77 }, new() { 43, 77 }, new() { 44, 77 }, new() { 45, 77 }, new() { 46, 77 }, new() { 47, 77 }, new() { 48, 77 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [32] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 77 }, new() { 7, 77 }, new() { 8, 77 }, new() { 9, 77 }, new() { 10, 77 }, new() { 11, 77 }, new() { 12, 77 }, new() { 13, 77 }, new() { 14, 77 }, new() { 15, 77 }, new() { 16, 77 }, new() { 17, 77 }, new() { 18, 77 }, new() { 19, 77 }, new() { 20, 77 }, new() { 21, 77 }, new() { 22, 77 }, new() { 23, 77 }, new() { 24, 77 }, new() { 33, 77 }, new() { 34, 77 }, new() { 35, 77 }, new() { 36, 77 }, new() { 37, 77 }, new() { 38, 77 }, new() { 39, 77 }, new() { 40, 77 }, new() { 41, 77 }, new() { 42, 77 }, new() { 43, 77 }, new() { 44, 77 }, new() { 45, 77 }, new() { 46, 77 }, new() { 47, 77 }, new() { 48, 77 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [33] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 77 }, new() { 7, 77 }, new() { 8, 77 }, new() { 9, 77 }, new() { 10, 77 }, new() { 11, 77 }, new() { 12, 77 }, new() { 13, 77 }, new() { 14, 77 }, new() { 15, 77 }, new() { 16, 77 }, new() { 17, 77 }, new() { 18, 77 }, new() { 19, 77 }, new() { 20, 77 }, new() { 21, 77 }, new() { 22, 77 }, new() { 23, 77 }, new() { 24, 77 }, new() { 33, 77 }, new() { 34, 77 }, new() { 35, 77 }, new() { 36, 77 }, new() { 37, 77 }, new() { 38, 77 }, new() { 39, 77 }, new() { 40, 77 }, new() { 41, 77 }, new() { 42, 77 }, new() { 43, 77 }, new() { 44, 77 }, new() { 45, 77 }, new() { 46, 77 }, new() { 47, 77 }, new() { 48, 77 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },

            };

            foreach (KeyValuePair<int, List<List<int>>> code in codes)
            {

                foreach (List<int> array in code.Value)
                {

                    buildings.Tiles[array[0], code.Key] = new StaticTile(buildings, dungeon, BlendMode.Alpha, array[1]);

                }

            }


            codes = new()
            {
                [0] = new() { },
                [1] = new() { },
                [2] = new() { },
                [3] = new() { },
                [4] = new() { },
                [5] = new() { new() { 19, 243 }, new() { 20, 244 }, new() { 21, 244 }, new() { 22, 244 }, new() { 23, 244 }, new() { 24, 245 }, },
                [6] = new() { new() { 18, 243 }, new() { 19, 249 }, new() { 24, 250 }, new() { 25, 244 }, new() { 26, 245 }, },
                [7] = new() { new() { 16, 243 }, new() { 17, 244 }, new() { 18, 249 }, new() { 26, 250 }, new() { 27, 245 }, },
                [8] = new() { new() { 14, 243 }, new() { 15, 244 }, new() { 16, 249 }, new() { 27, 261 }, },
                [9] = new() { new() { 12, 243 }, new() { 13, 244 }, new() { 14, 249 }, new() { 26, 267 }, new() { 27, 280 }, },
                [10] = new() { new() { 10, 243 }, new() { 11, 244 }, new() { 12, 249 }, new() { 25, 267 }, new() { 26, 280 }, },
                [11] = new() { new() { 10, 259 }, new() { 14, 267 }, new() { 15, 279 }, new() { 16, 279 }, new() { 17, 279 }, new() { 18, 251 }, new() { 25, 261 }, },
                [12] = new() { new() { 10, 259 }, new() { 13, 267 }, new() { 14, 280 }, new() { 18, 278 }, new() { 19, 279 }, new() { 20, 251 }, new() { 23, 267 }, new() { 24, 279 }, new() { 25, 280 }, },
                [13] = new() { new() { 10, 259 }, new() { 13, 261 }, new() { 20, 278 }, new() { 21, 279 }, new() { 22, 279 }, new() { 23, 280 }, },
                [14] = new() { new() { 9, 243 }, new() { 10, 249 }, new() { 13, 261 }, },
                [15] = new() { new() { 9, 259 }, new() { 12, 267 }, new() { 13, 280 }, },
                [16] = new() { new() { 9, 259 }, new() { 12, 261 }, },
                [17] = new() { new() { 8, 243 }, new() { 9, 249 }, new() { 12, 261 }, },
                [18] = new() { new() { 8, 259 }, new() { 12, 261 }, },
                [19] = new() { new() { 8, 259 }, new() { 11, 267 }, new() { 12, 280 }, },
                [20] = new() { new() { 8, 259 }, new() { 11, 261 }, },
                [21] = new() { new() { 8, 259 }, new() { 11, 261 }, },
                [22] = new() { new() { 8, 259 }, new() { 11, 261 }, },
                [23] = new() { new() { 8, 259 }, new() { 11, 261 }, },
                [24] = new() { new() { 8, 259 }, new() { 11, 261 }, },
                [25] = new() { new() { 8, 259 }, new() { 11, 261 }, },
                [26] = new() { new() { 8, 259 }, new() { 11, 250 }, new() { 12, 245 }, },
                [27] = new() { new() { 12, 250 }, new() { 13, 245 }, },
                [28] = new() { new() { 13, 261 }, },
                [29] = new() { },
                [30] = new() { },
                [31] = new() { },
                [32] = new() { },
                [33] = new() { },

            };

            foreach (KeyValuePair<int, List<List<int>>> code in codes)
            {

                foreach (List<int> array in code.Value)
                {

                    buildings.Tiles[array[0], code.Key] = new StaticTile(buildings, dangerous, BlendMode.Alpha, array[1]);

                }

            }

            codes = new()
            {
                [0] = new() { new() { 24, 31 }, new() { 25, 47 }, new() { 33, 61 }, },
                [1] = new() { new() { 10, 41 }, new() { 11, 42 }, new() { 12, 9 }, new() { 13, 10 }, new() { 14, 29 }, new() { 15, 25 }, new() { 16, 26 }, new() { 28, 51 }, new() { 39, 25 }, new() { 40, 26 }, new() { 41, 43 }, new() { 42, 44 }, new() { 43, 41 }, new() { 44, 42 }, },
                [2] = new() { new() { 9, 63 }, new() { 10, 29 }, new() { 11, 77 }, new() { 12, 77 }, new() { 13, 57 }, new() { 14, 58 }, new() { 29, 49 }, new() { 41, 59 }, new() { 42, 60 }, new() { 43, 29 }, new() { 44, 9 }, new() { 45, 10 }, new() { 46, 63 }, new() { 47, 41 }, new() { 48, 42 }, },
                [3] = new() { new() { 7, 29 }, new() { 8, 25 }, new() { 9, 26 }, new() { 10, 77 }, new() { 11, 61 }, new() { 12, 12 }, new() { 43, 57 }, new() { 44, 58 }, new() { 45, 61 }, new() { 46, 9 }, new() { 47, 10 }, },
                [4] = new() { new() { 5, 9 }, new() { 6, 10 }, new() { 10, 58 }, new() { 46, 25 }, new() { 47, 26 }, new() { 48, 63 }, new() { 49, 41 }, new() { 50, 42 }, },
                [5] = new() { new() { 6, 61 }, new() { 48, 57 }, new() { 49, 43 }, new() { 50, 44 }, },
                [6] = new() { new() { 5, 29 }, new() { 6, 77 }, new() { 49, 59 }, new() { 50, 60 }, },
                [7] = new() { new() { 5, 57 }, new() { 6, 58 }, new() { 49, 29 }, new() { 50, 41 }, new() { 51, 42 }, },
                [8] = new() { new() { 5, 29 }, new() { 49, 57 }, new() { 50, 58 }, new() { 51, 63 }, },
                [9] = new() { new() { 4, 63 }, new() { 5, 58 }, new() { 49, 77 }, new() { 50, 43 }, new() { 51, 44 }, },
                [10] = new() { new() { 4, 10 }, new() { 5, 61 }, new() { 50, 59 }, new() { 51, 60 }, },
                [11] = new() { new() { 4, 25 }, new() { 5, 26 }, new() { 50, 44 }, new() { 51, 29 }, },
                [12] = new() { new() { 4, 29 }, new() { 50, 25 }, new() { 51, 26 }, },
                [13] = new() { new() { 4, 58 }, new() { 50, 77 }, new() { 51, 57 }, new() { 52, 63 }, },
                [14] = new() { new() { 4, 77 }, new() { 51, 77 }, new() { 52, 29 }, },
                [15] = new() { new() { 4, 12 }, new() { 51, 77 }, new() { 52, 61 }, },
                [16] = new() { new() { 51, 15 }, new() { 52, 63 }, },
                [17] = new() { new() { 51, 77 }, new() { 52, 29 }, },
                [18] = new() { new() { 4, 196 }, new() { 5, 216 }, new() { 51, 15 }, new() { 52, 77 }, },
                [19] = new() { new() { 4, 46 }, new() { 5, 232 }, new() { 50, 191 }, new() { 51, 29 }, new() { 52, 63 }, },
                [20] = new() { new() { 4, 62 }, new() { 5, 213 }, new() { 6, 216 }, new() { 49, 220 }, new() { 50, 206 }, new() { 51, 44 }, new() { 52, 29 }, },
                [21] = new() { new() { 4, 45 }, new() { 5, 46 }, new() { 6, 232 }, new() { 49, 236 }, new() { 50, 77 }, new() { 51, 29 }, },
                [22] = new() { new() { 4, 44 }, new() { 5, 62 }, new() { 6, 213 }, new() { 7, 216 }, new() { 48, 220 }, new() { 49, 221 }, new() { 50, 25 }, new() { 51, 26 }, },
                [23] = new() { new() { 5, 25 }, new() { 6, 26 }, new() { 7, 232 }, new() { 48, 236 }, new() { 49, 77 }, new() { 50, 29 }, new() { 51, 63 }, },
                [24] = new() { new() { 4, 9 }, new() { 5, 10 }, new() { 6, 77 }, new() { 7, 213 }, new() { 8, 197 }, new() { 47, 220 }, new() { 48, 221 }, new() { 49, 15 }, new() { 50, 9 }, new() { 51, 10 }, },
                [25] = new() { new() { 6, 45 }, new() { 7, 46 }, new() { 8, 213 }, new() { 9, 197 }, new() { 47, 236 }, new() { 48, 77 }, new() { 49, 29 }, },
                [26] = new() { new() { 6, 61 }, new() { 7, 62 }, new() { 8, 29 }, new() { 9, 213 }, new() { 10, 197 }, new() { 46, 205 }, new() { 47, 221 }, new() { 48, 77 }, new() { 49, 63 }, },
                [27] = new() { new() { 7, 29 }, new() { 8, 25 }, new() { 9, 26 }, new() { 10, 213 }, new() { 11, 214 }, new() { 12, 197 }, new() { 45, 205 }, new() { 46, 221 }, new() { 47, 25 }, new() { 48, 26 }, },
                [28] = new() { new() { 10, 44 }, new() { 11, 77 }, new() { 12, 213 }, new() { 13, 215 }, new() { 14, 197 }, new() { 42, 205 }, new() { 43, 214 }, new() { 44, 215 }, new() { 45, 221 }, new() { 46, 61 }, new() { 47, 29 }, new() { 48, 9 }, new() { 49, 10 }, },
                [29] = new() { new() { 9, 9 }, new() { 10, 10 }, new() { 11, 61 }, new() { 12, 77 }, new() { 13, 77 }, new() { 14, 213 }, new() { 15, 215 }, new() { 16, 215 }, new() { 17, 215 }, new() { 18, 197 }, new() { 19, 230 }, new() { 20, 231 }, new() { 21, 205 }, new() { 22, 214 }, new() { 23, 215 }, new() { 24, 197 }, new() { 36, 230 }, new() { 37, 231 }, new() { 38, 205 }, new() { 39, 214 }, new() { 40, 215 }, new() { 41, 214 }, new() { 42, 221 }, new() { 43, 77 }, new() { 44, 77 }, new() { 45, 77 }, new() { 46, 9 }, new() { 47, 10 }, new() { 48, 63 }, },
                [30] = new() { new() { 11, 63 }, new() { 12, 29 }, new() { 13, 25 }, new() { 14, 26 }, new() { 15, 77 }, new() { 16, 77 }, new() { 17, 77 }, new() { 18, 213 }, new() { 19, 221 }, new() { 20, 213 }, new() { 21, 221 }, new() { 22, 25 }, new() { 23, 26 }, new() { 24, 213 }, new() { 25, 215 }, new() { 26, 197 }, new() { 32, 205 }, new() { 33, 214 }, new() { 34, 215 }, new() { 35, 215 }, new() { 36, 221 }, new() { 37, 213 }, new() { 38, 221 }, new() { 39, 77 }, new() { 40, 61 }, new() { 41, 77 }, new() { 42, 25 }, new() { 43, 26 }, new() { 44, 77 }, new() { 45, 29 }, },
                [31] = new() { new() { 11, 9 }, new() { 12, 10 }, new() { 13, 77 }, new() { 14, 63 }, new() { 15, 29 }, new() { 16, 63 }, new() { 17, 61 }, new() { 18, 63 }, new() { 19, 25 }, new() { 20, 26 }, new() { 21, 29 }, new() { 22, 77 }, new() { 23, 63 }, new() { 24, 26 }, new() { 25, 77 }, new() { 26, 213 }, new() { 27, 215 }, new() { 28, 214 }, new() { 29, 215 }, new() { 30, 214 }, new() { 31, 215 }, new() { 32, 221 }, new() { 33, 77 }, new() { 34, 77 }, new() { 35, 61 }, new() { 36, 29 }, new() { 37, 9 }, new() { 38, 10 }, new() { 39, 63 }, new() { 40, 77 }, new() { 41, 77 }, new() { 42, 9 }, new() { 43, 10 }, },
                [32] = new() { new() { 25, 25 }, new() { 26, 26 }, new() { 27, 77 }, new() { 28, 25 }, new() { 29, 26 }, new() { 30, 63 }, new() { 31, 63 }, new() { 32, 25 }, new() { 33, 26 }, new() { 34, 77 }, new() { 35, 77 }, },
                [33] = new() { new() { 25, 77 }, new() { 26, 63 }, new() { 27, 29 }, new() { 28, 77 }, new() { 29, 63 }, new() { 30, 77 }, new() { 31, 77 }, new() { 32, 9 }, new() { 33, 10 }, },

            };

            foreach (KeyValuePair<int, List<List<int>>> code in codes)
            {

                foreach (List<int> array in code.Value)
                {

                    front.Tiles[array[0], code.Key] = new StaticTile(front, dungeon, BlendMode.Alpha, array[1]);

                }

            }

            codes = new()
            {

                [3] = new() { new() { 32, 4 }, new() { 33, 5 }, new() { 34, 6 }, new() { 35, 7 }, },
                [4] = new() { new() { 32, 20 }, new() { 33, 21 }, new() { 34, 22 }, new() { 35, 23 }, new() { 37, 8 }, new() { 38, 9 }, new() { 39, 10 }, new() { 40, 11 }, },
                [5] = new() { new() { 28, 1 }, new() { 29, 2 }, new() { 30, 3 }, new() { 32, 36 }, new() { 33, 37 }, new() { 34, 38 }, new() { 35, 39 }, new() { 37, 24 }, new() { 38, 25 }, new() { 39, 26 }, new() { 40, 27 }, },
                [6] = new() { new() { 27, 16 }, new() { 28, 17 }, new() { 29, 18 }, new() { 30, 19 }, new() { 32, 52 }, new() { 33, 53 }, new() { 34, 54 }, new() { 35, 55 }, new() { 37, 40 }, new() { 38, 41 }, new() { 39, 42 }, new() { 40, 43 }, new() { 42, 12 }, new() { 43, 13 }, new() { 44, 14 }, new() { 45, 15 }, },
                [7] = new() { new() { 27, 32 }, new() { 28, 33 }, new() { 29, 34 }, new() { 30, 35 }, new() { 32, 68 }, new() { 33, 69 }, new() { 34, 70 }, new() { 35, 71 }, new() { 37, 56 }, new() { 38, 57 }, new() { 39, 58 }, new() { 40, 59 }, new() { 42, 28 }, new() { 43, 29 }, new() { 44, 30 }, new() { 45, 31 }, },
                [8] = new() { new() { 27, 48 }, new() { 28, 49 }, new() { 29, 50 }, new() { 30, 51 }, new() { 32, 84 }, new() { 33, 85 }, new() { 34, 86 }, new() { 35, 87 }, new() { 37, 72 }, new() { 38, 73 }, new() { 39, 74 }, new() { 40, 75 }, new() { 42, 44 }, new() { 43, 45 }, new() { 44, 46 }, new() { 45, 47 }, },
                [9] = new() { new() { 27, 64 }, new() { 28, 65 }, new() { 29, 66 }, new() { 30, 67 }, new() { 32, 100 }, new() { 33, 101 }, new() { 34, 102 }, new() { 35, 103 }, new() { 37, 88 }, new() { 38, 89 }, new() { 39, 90 }, new() { 40, 91 }, new() { 42, 60 }, new() { 43, 61 }, new() { 44, 62 }, new() { 45, 63 }, },
                [10] = new() { new() { 27, 80 }, new() { 28, 81 }, new() { 29, 82 }, new() { 30, 83 }, new() { 32, 116 }, new() { 33, 117 }, new() { 34, 118 }, new() { 35, 119 }, new() { 37, 104 }, new() { 38, 105 }, new() { 39, 106 }, new() { 40, 107 }, new() { 42, 76 }, new() { 43, 77 }, new() { 44, 78 }, new() { 45, 79 }, },
                [11] = new() { new() { 27, 96 }, new() { 28, 97 }, new() { 29, 98 }, new() { 30, 99 }, new() { 37, 120 }, new() { 38, 121 }, new() { 39, 122 }, new() { 40, 123 }, new() { 42, 92 }, new() { 43, 93 }, new() { 44, 94 }, new() { 45, 95 }, },
                [12] = new() { new() { 27, 112 }, new() { 28, 113 }, new() { 29, 114 }, new() { 30, 115 }, new() { 42, 108 }, new() { 43, 109 }, new() { 44, 110 }, new() { 45, 111 }, },
                [13] = new() { new() { 42, 124 }, new() { 43, 125 }, new() { 44, 126 }, new() { 45, 127 }, },

            };

            foreach (KeyValuePair<int, List<List<int>>> code in codes)
            {

                foreach (List<int> array in code.Value)
                {

                    Vector2 codeVector = new(array[1] % 16, (int)(array[1] / 16));

                    int offset = 8 - (int)codeVector.Y;

                    LocationTile lTile = new(array[0], code.Key, (int)codeVector.X, (int)codeVector.Y, offset, offset == 1, IconData.tilesheets.court);

                    if (offset == 1)
                    {

                        buildings.Tiles[array[0], code.Key] = new StaticTile(buildings, dangerous, BlendMode.Alpha, back.Tiles[array[0], code.Key].TileIndex);

                    }

                    locationTiles.Add(lTile);

                }

            }

            codes = new()
            {

                [2] = new() { new() { 27, 1 }, },
                [3] = new() { new() { 35, 1 }, },

                [5] = new() { new() { 13, 1 }, new() { 42, 1 }, },

                [9] = new() { new() { 7, 1 }, },

                [11] = new() { new() { 48, 1 }, },

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

            dungeonTexture = Game1.temporaryContent.Load<Texture2D>(dungeon.ImageSource);

            waterfallTexture = Game1.temporaryContent.Load<Texture2D>("Maps\\spring_Waterfalls");

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

            Town town = Game1.getLocationFromName("Town") as Town;

            town.warps.Add(new Warp(98, 4, LocationData.druid_court_name, 28, 29, flipFarmer: false));

            town.warps.Add(new Warp(98, 5, LocationData.druid_court_name, 28, 29, flipFarmer: false));

            warpSets.Add(new WarpTile(27, 32, "Town", 98, 8));

            warpSets.Add(new WarpTile(28, 32, "Town", 98, 8));

            warpSets.Add(new WarpTile(29, 32, "Town", 98, 8));
            
            warpSets.Add(new WarpTile(30, 32, "Town", 98, 8));

            warps.Add(new Warp(27, 32, "Town", 98, 8, flipFarmer: false));

            warps.Add(new Warp(28, 32, "Town", 98, 8, flipFarmer: false));

            warps.Add(new Warp(29, 32, "Town", 98, 8, flipFarmer: false));

            warps.Add(new Warp(30, 32, "Town", 98, 8, flipFarmer: false));

        }

    }

}
