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
    public class Tomb : GameLocation
    {

        public List<Location.WarpTile> warpSets = new();

        public List<Location.LocationTile> locationTiles = new();

        public Dictionary<Vector2, CharacterHandle.characters> dialogueTiles = new();

        public Dictionary<Vector2, int> lightFields = new();

        public Dictionary<Vector2, int> darkFields = new();

        public Dictionary<Vector2, int> brazierTiles = new();

        public bool ambientDarkness;

        public Tomb() { }

        public Tomb(string Name)
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
                        Microsoft.Xna.Framework.Color.PeachPuff * 0.75f,
                        0f,
                        new Vector2(texture2D.Bounds.Width / 2, texture2D.Bounds.Height / 2),
                        0.25f * light.Value,
                        SpriteEffects.None,
                        0.9f
                    );

                }

            }

            float ambience = 0.4f;

            foreach (KeyValuePair<Vector2, int> light in darkFields)
            {

                if (Utility.isOnScreen(light.Key, 64 * light.Value))
                {

                    Texture2D texture2D = Game1.lantern;

                    Microsoft.Xna.Framework.Vector2 position = new(light.Key.X - (float)Game1.viewport.X, light.Key.Y - (float)Game1.viewport.Y);

                    b.Draw(
                        texture2D,
                        position,
                        texture2D.Bounds,
                        Microsoft.Xna.Framework.Color.DarkBlue * ambience,
                        0f,
                        new Vector2(texture2D.Bounds.Width / 2, texture2D.Bounds.Height / 2),
                        light.Value,
                        SpriteEffects.None,
                        0.9f
                    );

                }

            }

            int brazierTime = (int)(Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 1000) / 250;

            foreach (KeyValuePair<Vector2, int> brazier in brazierTiles)
            {

                if (Utility.isOnScreen(brazier.Key, 64))
                {

                    Texture2D texture2D = Game1.lantern;

                    int frame = (brazierTime + brazier.Value) % 4;

                    Microsoft.Xna.Framework.Vector2 position = new(brazier.Key.X - (float)Game1.viewport.X, brazier.Key.Y - (float)Game1.viewport.Y);

                    b.Draw(
                        Mod.instance.iconData.sheetTextures[IconData.tilesheets.tomb], 
                        position, 
                        new Rectangle(32 + (frame * 32),0,32,32), 
                        Microsoft.Xna.Framework.Color.White,
                        0f, 
                        Vector2.Zero, 
                        4, 
                        0, 
                        brazier.Key.Y / 10000 + (6 * 0.0064f)
                        );

                }

            }

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

                    b.Draw(Mod.instance.iconData.sheetTextures[IconData.tilesheets.tomb], position, tile.rectangle, Microsoft.Xna.Framework.Color.White * opacity, 0f, Vector2.Zero, 4, tile.flip ? (SpriteEffects)1 : 0, tile.layer + (tile.offset * 0.0064f));

                    if (tile.shadow)
                    {

                        b.Draw(Mod.instance.iconData.sheetTextures[IconData.tilesheets.tomb], position + new Vector2(2, 11), tile.rectangle, Microsoft.Xna.Framework.Color.Black * 0.35f, 0f, Vector2.Zero, 4, tile.flip ? (SpriteEffects)1 : 0, tile.layer - 0.001f);

                    }

                }

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

            Layer back = new("Back", newMap, new(56, 36), tileSize);

            newMap.AddLayer(back);

            Layer buildings = new("Buildings", newMap, new(56, 36), tileSize);

            newMap.AddLayer(buildings);

            Layer front = new("Front", newMap, new(56, 36), tileSize);

            newMap.AddLayer(front);

            Layer alwaysfront = new("AlwaysFront", newMap, new(56, 36), tileSize);

            newMap.AddLayer(alwaysfront);

            TileSheet dungeon = new(LocationData.druid_tomb_name + "_dungeon", newMap, "Maps\\Mines\\mine_desert_dark_dangerous", new(16, 36), tileSize);

            newMap.AddTileSheet(dungeon); //map.TileSheets[1].ImageSource

            locationTiles = new();

            Dictionary<int, List<List<int>>> codes = new()
            {
                [0] = new() { },
                [1] = new() { },
                [2] = new() { },
                [3] = new() { new() { 17, 201 }, new() { 18, 202 }, new() { 19, 188 }, new() { 20, 218 }, new() { 21, 218 }, new() { 22, 218 }, new() { 23, 218 }, new() { 24, 218 }, new() { 25, 218 }, new() { 26, 218 }, new() { 27, 218 }, new() { 28, 218 }, new() { 29, 218 }, new() { 30, 218 }, new() { 31, 218 }, new() { 32, 218 }, new() { 33, 187 }, new() { 34, 202 }, new() { 35, 202 }, new() { 36, 202 }, },
                [4] = new() { new() { 13, 138 }, new() { 14, 201 }, new() { 15, 202 }, new() { 16, 202 }, new() { 17, 188 }, new() { 18, 218 }, new() { 19, 218 }, new() { 20, 186 }, new() { 21, 234 }, new() { 22, 234 }, new() { 23, 234 }, new() { 24, 234 }, new() { 25, 234 }, new() { 26, 234 }, new() { 27, 234 }, new() { 28, 234 }, new() { 29, 234 }, new() { 30, 234 }, new() { 31, 204 }, new() { 32, 185 }, new() { 33, 218 }, new() { 34, 218 }, new() { 35, 218 }, new() { 36, 218 }, new() { 37, 187 }, new() { 38, 202 }, new() { 39, 203 }, },
                [5] = new() { new() { 13, 201 }, new() { 14, 188 }, new() { 15, 218 }, new() { 16, 218 }, new() { 17, 186 }, new() { 18, 234 }, new() { 19, 234 }, new() { 20, 235 }, new() { 21, 138 }, new() { 22, 138 }, new() { 23, 138 }, new() { 24, 138 }, new() { 25, 138 }, new() { 26, 138 }, new() { 27, 138 }, new() { 28, 138 }, new() { 29, 138 }, new() { 30, 138 }, new() { 31, 138 }, new() { 32, 233 }, new() { 33, 234 }, new() { 34, 234 }, new() { 35, 185 }, new() { 36, 218 }, new() { 37, 218 }, new() { 38, 218 }, new() { 39, 187 }, },
                [6] = new() { new() { 9, 188 }, new() { 10, 218 }, new() { 11, 187 }, new() { 12, 202 }, new() { 13, 188 }, new() { 14, 218 }, new() { 15, 186 }, new() { 16, 234 }, new() { 17, 235 }, new() { 18, 138 }, new() { 19, 138 }, new() { 20, 169 }, new() { 21, 138 }, new() { 22, 138 }, new() { 23, 138 }, new() { 24, 138 }, new() { 25, 138 }, new() { 26, 138 }, new() { 27, 138 }, new() { 28, 138 }, new() { 29, 138 }, new() { 30, 138 }, new() { 31, 138 }, new() { 32, 138 }, new() { 33, 138 }, new() { 34, 140 }, new() { 35, 233 }, new() { 36, 234 }, new() { 37, 185 }, new() { 38, 218 }, new() { 39, 218 }, new() { 40, 187 }, new() { 41, 202 }, new() { 42, 203 }, },
                [7] = new() { new() { 9, 218 }, new() { 10, 218 }, new() { 11, 218 }, new() { 12, 218 }, new() { 13, 186 }, new() { 14, 234 }, new() { 15, 235 }, new() { 16, 138 }, new() { 17, 138 }, new() { 18, 166 }, new() { 19, 167 }, new() { 20, 167 }, new() { 21, 167 }, new() { 22, 167 }, new() { 23, 167 }, new() { 24, 167 }, new() { 25, 167 }, new() { 26, 167 }, new() { 27, 167 }, new() { 28, 167 }, new() { 29, 167 }, new() { 30, 167 }, new() { 31, 167 }, new() { 32, 168 }, new() { 33, 154 }, new() { 34, 166 }, new() { 35, 167 }, new() { 36, 168 }, new() { 37, 233 }, new() { 38, 234 }, new() { 39, 185 }, new() { 40, 218 }, new() { 41, 218 }, new() { 42, 187 }, new() { 43, 202 }, new() { 44, 202 }, },
                [8] = new() { new() { 9, 234 }, new() { 10, 234 }, new() { 11, 234 }, new() { 12, 234 }, new() { 13, 235 }, new() { 14, 166 }, new() { 15, 167 }, new() { 16, 168 }, new() { 17, 166 }, new() { 18, 152 }, new() { 19, 183 }, new() { 20, 183 }, new() { 21, 183 }, new() { 22, 183 }, new() { 23, 183 }, new() { 24, 183 }, new() { 25, 183 }, new() { 26, 183 }, new() { 27, 183 }, new() { 28, 183 }, new() { 29, 183 }, new() { 30, 183 }, new() { 31, 183 }, new() { 32, 151 }, new() { 33, 168 }, new() { 34, 182 }, new() { 35, 183 }, new() { 36, 184 }, new() { 37, 138 }, new() { 38, 138 }, new() { 39, 233 }, new() { 40, 234 }, new() { 41, 185 }, new() { 42, 218 }, new() { 43, 218 }, new() { 44, 218 }, },
                [9] = new() { new() { 8, 202 }, new() { 9, 203 }, new() { 10, 138 }, new() { 11, 138 }, new() { 12, 166 }, new() { 13, 167 }, new() { 14, 152 }, new() { 15, 183 }, new() { 16, 151 }, new() { 17, 152 }, new() { 18, 183 }, new() { 19, 183 }, new() { 20, 183 }, new() { 21, 183 }, new() { 22, 183 }, new() { 23, 183 }, new() { 24, 183 }, new() { 25, 183 }, new() { 26, 183 }, new() { 27, 183 }, new() { 28, 183 }, new() { 29, 183 }, new() { 30, 183 }, new() { 31, 183 }, new() { 32, 183 }, new() { 33, 151 }, new() { 34, 152 }, new() { 35, 183 }, new() { 36, 151 }, new() { 37, 167 }, new() { 38, 168 }, new() { 39, 153 }, new() { 40, 138 }, new() { 41, 233 }, new() { 42, 234 }, new() { 43, 185 }, new() { 44, 218 }, },
                [10] = new() { new() { 8, 218 }, new() { 9, 219 }, new() { 10, 169 }, new() { 11, 166 }, new() { 12, 152 }, new() { 13, 183 }, new() { 14, 183 }, new() { 15, 183 }, new() { 16, 183 }, new() { 17, 183 }, new() { 18, 165 }, new() { 19, 165 }, new() { 20, 165 }, new() { 21, 183 }, new() { 22, 183 }, new() { 23, 183 }, new() { 24, 183 }, new() { 25, 183 }, new() { 26, 183 }, new() { 27, 183 }, new() { 28, 183 }, new() { 29, 183 }, new() { 30, 183 }, new() { 31, 165 }, new() { 32, 165 }, new() { 33, 165 }, new() { 34, 183 }, new() { 35, 183 }, new() { 36, 150 }, new() { 37, 149 }, new() { 38, 151 }, new() { 39, 168 }, new() { 40, 138 }, new() { 41, 138 }, new() { 42, 138 }, new() { 43, 217 }, new() { 44, 218 }, new() { 45, 219 }, },
                [11] = new() { new() { 8, 234 }, new() { 9, 235 }, new() { 10, 166 }, new() { 11, 152 }, new() { 12, 183 }, new() { 13, 183 }, new() { 14, 165 }, new() { 15, 165 }, new() { 16, 165 }, new() { 17, 183 }, new() { 18, 165 }, new() { 19, 165 }, new() { 20, 165 }, new() { 21, 183 }, new() { 22, 183 }, new() { 23, 183 }, new() { 24, 183 }, new() { 25, 183 }, new() { 26, 183 }, new() { 27, 183 }, new() { 28, 183 }, new() { 29, 183 }, new() { 30, 183 }, new() { 31, 165 }, new() { 32, 165 }, new() { 33, 165 }, new() { 34, 183 }, new() { 35, 183 }, new() { 36, 151 }, new() { 37, 152 }, new() { 38, 183 }, new() { 39, 151 }, new() { 40, 167 }, new() { 41, 168 }, new() { 42, 138 }, new() { 43, 217 }, new() { 44, 218 }, new() { 45, 219 }, },
                [12] = new() { new() { 8, 138 }, new() { 9, 166 }, new() { 10, 152 }, new() { 11, 183 }, new() { 12, 183 }, new() { 13, 183 }, new() { 14, 165 }, new() { 15, 165 }, new() { 16, 183 }, new() { 17, 183 }, new() { 18, 183 }, new() { 19, 183 }, new() { 20, 165 }, new() { 21, 183 }, new() { 22, 150 }, new() { 23, 199 }, new() { 24, 199 }, new() { 25, 199 }, new() { 26, 199 }, new() { 27, 199 }, new() { 28, 199 }, new() { 29, 199 }, new() { 30, 199 }, new() { 31, 149 }, new() { 32, 183 }, new() { 33, 181 }, new() { 34, 165 }, new() { 35, 183 }, new() { 36, 165 }, new() { 37, 183 }, new() { 38, 183 }, new() { 39, 150 }, new() { 40, 199 }, new() { 41, 200 }, new() { 42, 169 }, new() { 43, 217 }, new() { 44, 218 }, new() { 45, 219 }, },
                [13] = new() { new() { 8, 203 }, new() { 9, 182 }, new() { 10, 183 }, new() { 11, 165 }, new() { 12, 183 }, new() { 13, 165 }, new() { 14, 183 }, new() { 15, 183 }, new() { 16, 183 }, new() { 17, 165 }, new() { 18, 183 }, new() { 19, 165 }, new() { 20, 150 }, new() { 21, 199 }, new() { 22, 200 }, new() { 23, 138 }, new() { 24, 138 }, new() { 25, 138 }, new() { 26, 138 }, new() { 27, 138 }, new() { 28, 138 }, new() { 29, 138 }, new() { 30, 138 }, new() { 31, 198 }, new() { 32, 199 }, new() { 33, 149 }, new() { 34, 165 }, new() { 35, 183 }, new() { 36, 165 }, new() { 37, 183 }, new() { 38, 183 }, new() { 39, 151 }, new() { 40, 167 }, new() { 41, 168 }, new() { 42, 138 }, new() { 43, 233 }, new() { 44, 185 }, new() { 45, 187 }, },
                [14] = new() { new() { 7, 218 }, new() { 8, 219 }, new() { 9, 198 }, new() { 10, 149 }, new() { 11, 183 }, new() { 12, 183 }, new() { 13, 183 }, new() { 14, 183 }, new() { 15, 165 }, new() { 16, 165 }, new() { 17, 165 }, new() { 18, 165 }, new() { 19, 165 }, new() { 20, 184 }, new() { 21, 138 }, new() { 22, 138 }, new() { 23, 138 }, new() { 24, 138 }, new() { 25, 138 }, new() { 26, 138 }, new() { 27, 138 }, new() { 28, 138 }, new() { 29, 138 }, new() { 30, 138 }, new() { 31, 138 }, new() { 32, 138 }, new() { 33, 182 }, new() { 34, 165 }, new() { 35, 183 }, new() { 36, 165 }, new() { 37, 183 }, new() { 38, 183 }, new() { 39, 183 }, new() { 40, 183 }, new() { 41, 151 }, new() { 42, 168 }, new() { 43, 138 }, new() { 44, 217 }, new() { 45, 218 }, new() { 46, 219 }, },
                [15] = new() { new() { 7, 234 }, new() { 8, 235 }, new() { 9, 138 }, new() { 10, 182 }, new() { 11, 183 }, new() { 12, 165 }, new() { 13, 165 }, new() { 14, 165 }, new() { 15, 183 }, new() { 16, 165 }, new() { 17, 165 }, new() { 18, 165 }, new() { 19, 165 }, new() { 20, 184 }, new() { 21, 138 }, new() { 22, 138 }, new() { 23, 138 }, new() { 24, 138 }, new() { 25, 138 }, new() { 26, 138 }, new() { 27, 138 }, new() { 28, 138 }, new() { 29, 138 }, new() { 30, 138 }, new() { 31, 138 }, new() { 32, 138 }, new() { 33, 182 }, new() { 34, 165 }, new() { 35, 165 }, new() { 36, 165 }, new() { 37, 165 }, new() { 38, 165 }, new() { 39, 183 }, new() { 40, 183 }, new() { 41, 183 }, new() { 42, 184 }, new() { 43, 138 }, new() { 44, 217 }, new() { 45, 218 }, new() { 46, 219 }, },
                [16] = new() { new() { 6, 138 }, new() { 7, 138 }, new() { 8, 138 }, new() { 9, 166 }, new() { 10, 152 }, new() { 11, 183 }, new() { 12, 165 }, new() { 13, 181 }, new() { 14, 165 }, new() { 15, 183 }, new() { 16, 165 }, new() { 17, 183 }, new() { 18, 165 }, new() { 19, 183 }, new() { 20, 184 }, new() { 21, 138 }, new() { 22, 138 }, new() { 23, 138 }, new() { 24, 138 }, new() { 25, 138 }, new() { 26, 138 }, new() { 27, 138 }, new() { 28, 138 }, new() { 29, 138 }, new() { 30, 138 }, new() { 31, 138 }, new() { 32, 138 }, new() { 33, 182 }, new() { 34, 165 }, new() { 35, 183 }, new() { 36, 165 }, new() { 37, 183 }, new() { 38, 165 }, new() { 39, 183 }, new() { 40, 165 }, new() { 41, 183 }, new() { 42, 184 }, new() { 43, 153 }, new() { 44, 217 }, new() { 45, 218 }, new() { 46, 187 }, },
                [17] = new() { new() { 6, 202 }, new() { 7, 203 }, new() { 8, 138 }, new() { 9, 182 }, new() { 10, 183 }, new() { 11, 183 }, new() { 12, 183 }, new() { 13, 165 }, new() { 14, 165 }, new() { 15, 183 }, new() { 16, 183 }, new() { 17, 183 }, new() { 18, 183 }, new() { 19, 183 }, new() { 20, 184 }, new() { 21, 138 }, new() { 22, 138 }, new() { 23, 138 }, new() { 24, 138 }, new() { 25, 138 }, new() { 26, 138 }, new() { 27, 138 }, new() { 28, 138 }, new() { 29, 138 }, new() { 30, 138 }, new() { 31, 138 }, new() { 32, 138 }, new() { 33, 182 }, new() { 34, 165 }, new() { 35, 183 }, new() { 36, 165 }, new() { 37, 183 }, new() { 38, 183 }, new() { 39, 165 }, new() { 40, 165 }, new() { 41, 183 }, new() { 42, 151 }, new() { 43, 168 }, new() { 44, 233 }, new() { 45, 185 }, new() { 46, 218 }, },
                [18] = new() { new() { 6, 218 }, new() { 7, 219 }, new() { 8, 138 }, new() { 9, 182 }, new() { 10, 150 }, new() { 11, 149 }, new() { 12, 183 }, new() { 13, 183 }, new() { 14, 183 }, new() { 15, 183 }, new() { 16, 183 }, new() { 17, 183 }, new() { 18, 183 }, new() { 19, 183 }, new() { 20, 184 }, new() { 21, 138 }, new() { 22, 138 }, new() { 23, 138 }, new() { 24, 138 }, new() { 25, 138 }, new() { 26, 138 }, new() { 27, 138 }, new() { 28, 138 }, new() { 29, 138 }, new() { 30, 138 }, new() { 31, 138 }, new() { 32, 138 }, new() { 33, 182 }, new() { 34, 183 }, new() { 35, 183 }, new() { 36, 183 }, new() { 37, 183 }, new() { 38, 183 }, new() { 39, 165 }, new() { 40, 165 }, new() { 41, 183 }, new() { 42, 183 }, new() { 43, 184 }, new() { 44, 140 }, new() { 45, 217 }, new() { 46, 218 }, },
                [19] = new() { new() { 6, 234 }, new() { 7, 235 }, new() { 8, 138 }, new() { 9, 182 }, new() { 10, 184 }, new() { 11, 182 }, new() { 12, 183 }, new() { 13, 183 }, new() { 14, 183 }, new() { 15, 183 }, new() { 16, 183 }, new() { 17, 183 }, new() { 18, 183 }, new() { 19, 183 }, new() { 20, 184 }, new() { 21, 138 }, new() { 22, 138 }, new() { 23, 138 }, new() { 24, 138 }, new() { 25, 138 }, new() { 26, 138 }, new() { 27, 138 }, new() { 28, 138 }, new() { 29, 138 }, new() { 30, 138 }, new() { 31, 138 }, new() { 32, 138 }, new() { 33, 182 }, new() { 34, 183 }, new() { 35, 183 }, new() { 36, 183 }, new() { 37, 183 }, new() { 38, 165 }, new() { 39, 183 }, new() { 40, 183 }, new() { 41, 150 }, new() { 42, 149 }, new() { 43, 184 }, new() { 44, 153 }, new() { 45, 217 }, new() { 46, 218 }, },
                [20] = new() { new() { 6, 138 }, new() { 7, 138 }, new() { 8, 139 }, new() { 9, 182 }, new() { 10, 151 }, new() { 11, 152 }, new() { 12, 183 }, new() { 13, 183 }, new() { 14, 183 }, new() { 15, 165 }, new() { 16, 165 }, new() { 17, 183 }, new() { 18, 165 }, new() { 19, 183 }, new() { 20, 184 }, new() { 21, 138 }, new() { 22, 138 }, new() { 23, 138 }, new() { 24, 138 }, new() { 25, 138 }, new() { 26, 138 }, new() { 27, 138 }, new() { 28, 138 }, new() { 29, 138 }, new() { 30, 138 }, new() { 31, 138 }, new() { 32, 138 }, new() { 33, 182 }, new() { 34, 165 }, new() { 35, 183 }, new() { 36, 165 }, new() { 37, 183 }, new() { 38, 165 }, new() { 39, 183 }, new() { 40, 183 }, new() { 41, 184 }, new() { 42, 182 }, new() { 43, 151 }, new() { 44, 168 }, new() { 45, 217 }, new() { 46, 218 }, },
                [21] = new() { new() { 6, 138 }, new() { 7, 138 }, new() { 8, 138 }, new() { 9, 182 }, new() { 10, 183 }, new() { 11, 183 }, new() { 12, 183 }, new() { 13, 183 }, new() { 14, 183 }, new() { 15, 165 }, new() { 16, 165 }, new() { 17, 183 }, new() { 18, 165 }, new() { 19, 183 }, new() { 20, 184 }, new() { 21, 138 }, new() { 22, 138 }, new() { 23, 138 }, new() { 24, 138 }, new() { 25, 138 }, new() { 26, 138 }, new() { 27, 138 }, new() { 28, 138 }, new() { 29, 138 }, new() { 30, 138 }, new() { 31, 138 }, new() { 32, 138 }, new() { 33, 182 }, new() { 34, 165 }, new() { 35, 183 }, new() { 36, 165 }, new() { 37, 183 }, new() { 38, 183 }, new() { 39, 165 }, new() { 40, 165 }, new() { 41, 151 }, new() { 42, 152 }, new() { 43, 183 }, new() { 44, 184 }, new() { 45, 217 }, new() { 46, 218 }, },
                [22] = new() { new() { 7, 138 }, new() { 8, 169 }, new() { 9, 182 }, new() { 10, 183 }, new() { 11, 183 }, new() { 12, 183 }, new() { 13, 181 }, new() { 14, 183 }, new() { 15, 165 }, new() { 16, 165 }, new() { 17, 183 }, new() { 18, 165 }, new() { 19, 183 }, new() { 20, 151 }, new() { 21, 167 }, new() { 22, 168 }, new() { 23, 138 }, new() { 24, 138 }, new() { 25, 138 }, new() { 26, 138 }, new() { 27, 138 }, new() { 28, 138 }, new() { 29, 138 }, new() { 30, 138 }, new() { 31, 166 }, new() { 32, 167 }, new() { 33, 152 }, new() { 34, 183 }, new() { 35, 165 }, new() { 36, 183 }, new() { 37, 165 }, new() { 38, 165 }, new() { 39, 165 }, new() { 40, 165 }, new() { 41, 183 }, new() { 42, 150 }, new() { 43, 199 }, new() { 44, 200 }, new() { 45, 217 }, new() { 46, 218 }, },
                [23] = new() { new() { 7, 138 }, new() { 8, 138 }, new() { 9, 198 }, new() { 10, 149 }, new() { 11, 183 }, new() { 12, 183 }, new() { 13, 183 }, new() { 14, 183 }, new() { 15, 165 }, new() { 16, 165 }, new() { 17, 165 }, new() { 18, 165 }, new() { 19, 165 }, new() { 20, 165 }, new() { 21, 183 }, new() { 22, 151 }, new() { 23, 167 }, new() { 24, 167 }, new() { 25, 167 }, new() { 26, 167 }, new() { 27, 167 }, new() { 28, 167 }, new() { 29, 167 }, new() { 30, 167 }, new() { 31, 152 }, new() { 32, 183 }, new() { 33, 183 }, new() { 34, 183 }, new() { 35, 165 }, new() { 36, 183 }, new() { 37, 165 }, new() { 38, 165 }, new() { 39, 165 }, new() { 40, 165 }, new() { 41, 183 }, new() { 42, 151 }, new() { 43, 167 }, new() { 44, 168 }, new() { 45, 217 }, new() { 46, 218 }, },
                [24] = new() { new() { 8, 138 }, new() { 9, 138 }, new() { 10, 182 }, new() { 11, 183 }, new() { 12, 183 }, new() { 13, 183 }, new() { 14, 183 }, new() { 15, 165 }, new() { 16, 165 }, new() { 17, 165 }, new() { 18, 165 }, new() { 19, 183 }, new() { 20, 183 }, new() { 21, 165 }, new() { 22, 165 }, new() { 23, 165 }, new() { 24, 165 }, new() { 25, 165 }, new() { 26, 165 }, new() { 27, 165 }, new() { 28, 165 }, new() { 29, 165 }, new() { 30, 165 }, new() { 31, 183 }, new() { 32, 183 }, new() { 33, 183 }, new() { 34, 183 }, new() { 35, 183 }, new() { 36, 165 }, new() { 37, 165 }, new() { 38, 165 }, new() { 39, 165 }, new() { 40, 165 }, new() { 41, 183 }, new() { 42, 183 }, new() { 43, 183 }, new() { 44, 184 }, new() { 45, 188 }, },
                [25] = new() { new() { 8, 138 }, new() { 9, 138 }, new() { 10, 198 }, new() { 11, 149 }, new() { 12, 183 }, new() { 13, 183 }, new() { 14, 183 }, new() { 15, 165 }, new() { 16, 165 }, new() { 17, 165 }, new() { 18, 183 }, new() { 19, 183 }, new() { 20, 165 }, new() { 21, 165 }, new() { 22, 165 }, new() { 23, 165 }, new() { 24, 165 }, new() { 25, 165 }, new() { 26, 165 }, new() { 27, 165 }, new() { 28, 165 }, new() { 29, 165 }, new() { 30, 165 }, new() { 31, 165 }, new() { 32, 165 }, new() { 33, 183 }, new() { 34, 183 }, new() { 35, 183 }, new() { 36, 183 }, new() { 37, 165 }, new() { 38, 165 }, new() { 39, 165 }, new() { 40, 165 }, new() { 41, 183 }, new() { 42, 150 }, new() { 43, 199 }, new() { 44, 200 }, new() { 45, 218 }, },
                [26] = new() { new() { 9, 138 }, new() { 10, 138 }, new() { 11, 182 }, new() { 12, 183 }, new() { 13, 183 }, new() { 14, 183 }, new() { 15, 165 }, new() { 16, 165 }, new() { 17, 165 }, new() { 18, 183 }, new() { 19, 183 }, new() { 20, 183 }, new() { 21, 183 }, new() { 22, 183 }, new() { 23, 183 }, new() { 24, 183 }, new() { 25, 183 }, new() { 26, 183 }, new() { 27, 183 }, new() { 28, 183 }, new() { 29, 183 }, new() { 30, 183 }, new() { 31, 165 }, new() { 32, 165 }, new() { 33, 181 }, new() { 34, 183 }, new() { 35, 165 }, new() { 36, 165 }, new() { 37, 183 }, new() { 38, 165 }, new() { 39, 183 }, new() { 40, 181 }, new() { 41, 150 }, new() { 42, 200 }, new() { 43, 201 }, new() { 44, 188 }, },
                [27] = new() { new() { 9, 138 }, new() { 10, 138 }, new() { 11, 198 }, new() { 12, 199 }, new() { 13, 149 }, new() { 14, 183 }, new() { 15, 183 }, new() { 16, 183 }, new() { 17, 150 }, new() { 18, 149 }, new() { 19, 183 }, new() { 20, 183 }, new() { 21, 183 }, new() { 22, 183 }, new() { 23, 183 }, new() { 24, 183 }, new() { 25, 183 }, new() { 26, 183 }, new() { 27, 183 }, new() { 28, 183 }, new() { 29, 183 }, new() { 30, 183 }, new() { 31, 165 }, new() { 32, 165 }, new() { 33, 183 }, new() { 34, 183 }, new() { 35, 165 }, new() { 36, 165 }, new() { 37, 183 }, new() { 38, 183 }, new() { 39, 183 }, new() { 40, 183 }, new() { 41, 184 }, new() { 42, 169 }, new() { 43, 217 }, new() { 44, 218 }, },
                [28] = new() { new() { 10, 138 }, new() { 11, 138 }, new() { 12, 154 }, new() { 13, 198 }, new() { 14, 199 }, new() { 15, 149 }, new() { 16, 183 }, new() { 17, 151 }, new() { 18, 152 }, new() { 19, 183 }, new() { 20, 183 }, new() { 21, 183 }, new() { 22, 183 }, new() { 23, 183 }, new() { 24, 183 }, new() { 25, 183 }, new() { 26, 183 }, new() { 27, 183 }, new() { 28, 183 }, new() { 29, 183 }, new() { 30, 183 }, new() { 31, 183 }, new() { 32, 181 }, new() { 33, 165 }, new() { 34, 183 }, new() { 35, 183 }, new() { 36, 183 }, new() { 37, 183 }, new() { 38, 150 }, new() { 39, 199 }, new() { 40, 199 }, new() { 41, 200 }, new() { 42, 201 }, new() { 43, 188 }, },
                [29] = new() { new() { 10, 138 }, new() { 11, 138 }, new() { 12, 138 }, new() { 13, 140 }, new() { 14, 138 }, new() { 15, 198 }, new() { 16, 199 }, new() { 17, 149 }, new() { 18, 183 }, new() { 19, 183 }, new() { 20, 183 }, new() { 21, 183 }, new() { 22, 183 }, new() { 23, 183 }, new() { 24, 183 }, new() { 25, 183 }, new() { 26, 183 }, new() { 27, 183 }, new() { 28, 183 }, new() { 29, 183 }, new() { 30, 183 }, new() { 31, 183 }, new() { 32, 183 }, new() { 33, 183 }, new() { 34, 183 }, new() { 35, 183 }, new() { 36, 183 }, new() { 37, 183 }, new() { 38, 184 }, new() { 39, 153 }, new() { 40, 139 }, new() { 41, 138 }, new() { 42, 217 }, new() { 43, 218 }, },
                [30] = new() { new() { 12, 138 }, new() { 13, 138 }, new() { 14, 138 }, new() { 15, 138 }, new() { 16, 138 }, new() { 17, 198 }, new() { 18, 199 }, new() { 19, 199 }, new() { 20, 199 }, new() { 21, 199 }, new() { 22, 199 }, new() { 23, 199 }, new() { 24, 199 }, new() { 25, 199 }, new() { 26, 199 }, new() { 27, 199 }, new() { 28, 199 }, new() { 29, 199 }, new() { 30, 199 }, new() { 31, 199 }, new() { 32, 149 }, new() { 33, 183 }, new() { 34, 150 }, new() { 35, 199 }, new() { 36, 199 }, new() { 37, 199 }, new() { 38, 200 }, new() { 39, 138 }, new() { 40, 138 }, new() { 41, 138 }, },
                [31] = new() { new() { 14, 138 }, new() { 15, 138 }, new() { 16, 138 }, new() { 17, 170 }, new() { 18, 171 }, new() { 19, 138 }, new() { 20, 138 }, new() { 21, 139 }, new() { 22, 139 }, new() { 23, 139 }, new() { 24, 139 }, new() { 25, 139 }, new() { 26, 139 }, new() { 27, 139 }, new() { 28, 139 }, new() { 29, 139 }, new() { 30, 139 }, new() { 31, 140 }, new() { 32, 198 }, new() { 33, 199 }, new() { 34, 200 }, new() { 35, 138 }, new() { 36, 153 }, new() { 37, 201 }, new() { 38, 202 }, new() { 39, 203 }, new() { 40, 138 }, new() { 41, 138 }, },
                [32] = new() { new() { 16, 138 }, new() { 17, 138 }, new() { 18, 138 }, new() { 19, 138 }, new() { 20, 138 }, new() { 21, 138 }, new() { 22, 138 }, new() { 23, 138 }, new() { 24, 138 }, new() { 25, 138 }, new() { 26, 138 }, new() { 27, 138 }, new() { 28, 138 }, new() { 29, 138 }, new() { 30, 138 }, new() { 31, 138 }, new() { 32, 138 }, new() { 33, 201 }, new() { 34, 202 }, new() { 35, 203 }, new() { 36, 138 }, new() { 37, 217 }, new() { 38, 218 }, },
                [33] = new() { new() { 34, 218 }, },
                [34] = new() { },
                [35] = new() { },


            };

            foreach (KeyValuePair<int, List<List<int>>> code in codes)
            {

                foreach (List<int> array in code.Value)
                {

                    back.Tiles[array[0], code.Key] = new StaticTile(back, dungeon, BlendMode.Alpha, array[1]);

                    back.Tiles[array[0], code.Key].TileIndexProperties.Add("Type", "Stone");

                }

            }

            codes = new()
            {
                [0] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 77 }, new() { 7, 77 }, new() { 8, 77 }, new() { 9, 77 }, new() { 10, 77 }, new() { 11, 77 }, new() { 12, 77 }, new() { 13, 77 }, new() { 14, 77 }, new() { 15, 77 }, new() { 16, 77 }, new() { 17, 77 }, new() { 18, 77 }, new() { 19, 69 }, new() { 20, 70 }, new() { 21, 73 }, new() { 22, 73 }, new() { 23, 74 }, new() { 24, 73 }, new() { 25, 73 }, new() { 26, 74 }, new() { 27, 73 }, new() { 28, 74 }, new() { 29, 73 }, new() { 30, 74 }, new() { 31, 67 }, new() { 32, 7 }, new() { 33, 93 }, new() { 34, 94 }, new() { 35, 77 }, new() { 36, 77 }, new() { 37, 77 }, new() { 38, 77 }, new() { 39, 77 }, new() { 40, 77 }, new() { 41, 77 }, new() { 42, 77 }, new() { 43, 77 }, new() { 44, 77 }, new() { 45, 77 }, new() { 46, 77 }, new() { 47, 77 }, new() { 48, 77 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [1] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 77 }, new() { 7, 77 }, new() { 8, 77 }, new() { 9, 77 }, new() { 10, 77 }, new() { 11, 77 }, new() { 12, 77 }, new() { 13, 77 }, new() { 14, 77 }, new() { 15, 77 }, new() { 16, 69 }, new() { 17, 70 }, new() { 18, 7 }, new() { 19, 85 }, new() { 20, 86 }, new() { 21, 89 }, new() { 22, 126 }, new() { 23, 126 }, new() { 24, 89 }, new() { 25, 126 }, new() { 26, 126 }, new() { 27, 126 }, new() { 28, 126 }, new() { 29, 126 }, new() { 30, 126 }, new() { 31, 83 }, new() { 32, 126 }, new() { 33, 109 }, new() { 34, 110 }, new() { 35, 8 }, new() { 36, 93 }, new() { 37, 94 }, new() { 38, 77 }, new() { 39, 77 }, new() { 40, 77 }, new() { 41, 77 }, new() { 42, 77 }, new() { 43, 77 }, new() { 44, 77 }, new() { 45, 77 }, new() { 46, 77 }, new() { 47, 77 }, new() { 48, 77 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [2] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 77 }, new() { 7, 77 }, new() { 8, 77 }, new() { 9, 77 }, new() { 10, 77 }, new() { 11, 77 }, new() { 12, 77 }, new() { 13, 77 }, new() { 14, 69 }, new() { 15, 70 }, new() { 16, 85 }, new() { 17, 86 }, new() { 18, 23 }, new() { 19, 98 }, new() { 20, 102 }, new() { 21, 105 }, new() { 22, 142 }, new() { 23, 142 }, new() { 24, 105 }, new() { 25, 142 }, new() { 26, 142 }, new() { 27, 142 }, new() { 28, 142 }, new() { 29, 142 }, new() { 30, 142 }, new() { 31, 99 }, new() { 32, 142 }, new() { 33, 125 }, new() { 34, 126 }, new() { 35, 126 }, new() { 36, 109 }, new() { 37, 110 }, new() { 38, 93 }, new() { 39, 94 }, new() { 40, 77 }, new() { 41, 77 }, new() { 42, 77 }, new() { 43, 77 }, new() { 44, 77 }, new() { 45, 77 }, new() { 46, 77 }, new() { 47, 77 }, new() { 48, 77 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [3] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 77 }, new() { 7, 77 }, new() { 8, 77 }, new() { 9, 68 }, new() { 10, 93 }, new() { 11, 94 }, new() { 12, 69 }, new() { 13, 70 }, new() { 14, 85 }, new() { 15, 86 }, new() { 16, 101 }, new() { 17, 102 }, new() { 18, 39 }, new() { 19, 114 }, new() { 20, 118 }, new() { 21, 121 }, new() { 22, 158 }, new() { 23, 158 }, new() { 24, 121 }, new() { 25, 158 }, new() { 26, 158 }, new() { 27, 158 }, new() { 28, 158 }, new() { 29, 158 }, new() { 30, 158 }, new() { 31, 115 }, new() { 32, 55 }, new() { 33, 141 }, new() { 34, 142 }, new() { 35, 142 }, new() { 36, 125 }, new() { 37, 126 }, new() { 38, 109 }, new() { 39, 110 }, new() { 40, 93 }, new() { 41, 94 }, new() { 42, 77 }, new() { 43, 77 }, new() { 44, 77 }, new() { 45, 77 }, new() { 46, 77 }, new() { 47, 77 }, new() { 48, 77 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [4] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 77 }, new() { 7, 77 }, new() { 8, 77 }, new() { 9, 84 }, new() { 10, 109 }, new() { 11, 110 }, new() { 12, 85 }, new() { 13, 86 }, new() { 14, 101 }, new() { 15, 102 }, new() { 16, 117 }, new() { 17, 118 }, new() { 18, 55 }, new() { 19, 133 }, new() { 20, 134 }, new() { 33, 157 }, new() { 34, 158 }, new() { 35, 56 }, new() { 36, 141 }, new() { 37, 142 }, new() { 38, 125 }, new() { 39, 126 }, new() { 40, 109 }, new() { 41, 110 }, new() { 42, 93 }, new() { 43, 94 }, new() { 44, 77 }, new() { 45, 77 }, new() { 46, 77 }, new() { 47, 77 }, new() { 48, 77 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [5] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 77 }, new() { 7, 77 }, new() { 8, 77 }, new() { 9, 100 }, new() { 10, 125 }, new() { 11, 126 }, new() { 12, 101 }, new() { 13, 102 }, new() { 14, 117 }, new() { 15, 118 }, new() { 16, 133 }, new() { 17, 134 }, new() { 36, 157 }, new() { 37, 158 }, new() { 38, 141 }, new() { 39, 142 }, new() { 40, 125 }, new() { 41, 126 }, new() { 42, 109 }, new() { 43, 110 }, new() { 44, 111 }, new() { 45, 77 }, new() { 46, 77 }, new() { 47, 77 }, new() { 48, 77 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [6] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 77 }, new() { 7, 77 }, new() { 8, 77 }, new() { 9, 116 }, new() { 10, 141 }, new() { 11, 142 }, new() { 12, 114 }, new() { 13, 118 }, new() { 14, 133 }, new() { 15, 134 }, new() { 38, 157 }, new() { 39, 158 }, new() { 40, 141 }, new() { 41, 142 }, new() { 42, 125 }, new() { 43, 126 }, new() { 44, 127 }, new() { 45, 77 }, new() { 46, 77 }, new() { 47, 77 }, new() { 48, 77 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [7] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 77 }, new() { 7, 77 }, new() { 8, 68 }, new() { 9, 71 }, new() { 10, 157 }, new() { 11, 158 }, new() { 12, 133 }, new() { 13, 134 }, new() { 40, 157 }, new() { 41, 158 }, new() { 42, 141 }, new() { 43, 142 }, new() { 44, 143 }, new() { 45, 77 }, new() { 46, 77 }, new() { 47, 77 }, new() { 48, 77 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [8] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 77 }, new() { 7, 77 }, new() { 8, 84 }, new() { 9, 87 }, new() { 42, 157 }, new() { 43, 158 }, new() { 44, 159 }, new() { 45, 77 }, new() { 46, 77 }, new() { 47, 77 }, new() { 48, 77 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [9] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 77 }, new() { 7, 77 }, new() { 8, 100 }, new() { 9, 103 }, new() { 44, 72 }, new() { 45, 111 }, new() { 46, 77 }, new() { 47, 77 }, new() { 48, 77 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [10] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 77 }, new() { 7, 77 }, new() { 8, 116 }, new() { 9, 119 }, new() { 44, 88 }, new() { 45, 127 }, new() { 46, 77 }, new() { 47, 77 }, new() { 48, 77 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [11] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 77 }, new() { 7, 68 }, new() { 8, 71 }, new() { 44, 104 }, new() { 45, 143 }, new() { 46, 77 }, new() { 47, 77 }, new() { 48, 77 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [12] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 77 }, new() { 7, 84 }, new() { 8, 87 }, new() { 44, 120 }, new() { 45, 159 }, new() { 46, 77 }, new() { 47, 77 }, new() { 48, 77 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [13] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 77 }, new() { 7, 100 }, new() { 8, 103 }, new() { 45, 72 }, new() { 46, 111 }, new() { 47, 77 }, new() { 48, 77 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [14] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 77 }, new() { 7, 116 }, new() { 8, 119 }, new() { 45, 88 }, new() { 46, 127 }, new() { 47, 77 }, new() { 48, 77 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [15] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 68 }, new() { 7, 71 }, new() { 45, 104 }, new() { 46, 143 }, new() { 47, 77 }, new() { 48, 77 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [16] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 84 }, new() { 7, 87 }, new() { 45, 120 }, new() { 46, 159 }, new() { 47, 77 }, new() { 48, 77 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [17] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 100 }, new() { 7, 103 }, new() { 46, 175 }, new() { 47, 77 }, new() { 48, 77 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [18] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 116 }, new() { 7, 119 }, new() { 46, 191 }, new() { 47, 77 }, new() { 48, 77 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [19] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 132 }, new() { 46, 207 }, new() { 47, 77 }, new() { 48, 77 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [20] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 180 }, new() { 46, 223 }, new() { 47, 77 }, new() { 48, 77 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [21] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 77 }, new() { 46, 175 }, new() { 47, 77 }, new() { 48, 77 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [22] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 77 }, new() { 7, 232 }, new() { 46, 191 }, new() { 47, 77 }, new() { 48, 77 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [23] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 77 }, new() { 7, 77 }, new() { 46, 206 }, new() { 47, 77 }, new() { 48, 77 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [24] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 77 }, new() { 7, 77 }, new() { 8, 232 }, new() { 45, 236 }, new() { 46, 77 }, new() { 47, 77 }, new() { 48, 77 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [25] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 77 }, new() { 7, 77 }, new() { 8, 77 }, new() { 45, 77 }, new() { 46, 77 }, new() { 47, 77 }, new() { 48, 77 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [26] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 77 }, new() { 7, 77 }, new() { 8, 77 }, new() { 9, 232 }, new() { 44, 236 }, new() { 45, 77 }, new() { 46, 77 }, new() { 47, 77 }, new() { 48, 77 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [27] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 77 }, new() { 7, 77 }, new() { 8, 77 }, new() { 9, 77 }, new() { 44, 77 }, new() { 45, 77 }, new() { 46, 77 }, new() { 47, 77 }, new() { 48, 77 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [28] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 77 }, new() { 7, 77 }, new() { 8, 77 }, new() { 9, 77 }, new() { 10, 77 }, new() { 43, 236 }, new() { 44, 77 }, new() { 45, 77 }, new() { 46, 77 }, new() { 47, 77 }, new() { 48, 77 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [29] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 77 }, new() { 7, 77 }, new() { 8, 77 }, new() { 9, 77 }, new() { 10, 77 }, new() { 11, 77 }, new() { 43, 77 }, new() { 44, 77 }, new() { 45, 77 }, new() { 46, 77 }, new() { 47, 77 }, new() { 48, 77 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [30] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 77 }, new() { 7, 77 }, new() { 8, 77 }, new() { 9, 77 }, new() { 10, 77 }, new() { 11, 77 }, new() { 12, 77 }, new() { 42, 77 }, new() { 43, 77 }, new() { 44, 77 }, new() { 45, 77 }, new() { 46, 77 }, new() { 47, 77 }, new() { 48, 77 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [31] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 77 }, new() { 7, 77 }, new() { 8, 77 }, new() { 9, 77 }, new() { 10, 77 }, new() { 11, 77 }, new() { 12, 77 }, new() { 13, 77 }, new() { 14, 77 }, new() { 41, 77 }, new() { 42, 77 }, new() { 43, 77 }, new() { 44, 77 }, new() { 45, 77 }, new() { 46, 77 }, new() { 47, 77 }, new() { 48, 77 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [32] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 77 }, new() { 7, 77 }, new() { 8, 77 }, new() { 9, 77 }, new() { 10, 77 }, new() { 11, 77 }, new() { 12, 77 }, new() { 13, 77 }, new() { 14, 77 }, new() { 15, 77 }, new() { 16, 77 }, new() { 38, 77 }, new() { 39, 77 }, new() { 40, 77 }, new() { 41, 77 }, new() { 42, 77 }, new() { 43, 77 }, new() { 44, 77 }, new() { 45, 77 }, new() { 46, 77 }, new() { 47, 77 }, new() { 48, 77 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [33] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 77 }, new() { 7, 77 }, new() { 8, 77 }, new() { 9, 77 }, new() { 10, 77 }, new() { 11, 77 }, new() { 12, 77 }, new() { 13, 77 }, new() { 14, 77 }, new() { 15, 77 }, new() { 16, 77 }, new() { 17, 77 }, new() { 18, 77 }, new() { 19, 77 }, new() { 20, 77 }, new() { 21, 77 }, new() { 22, 77 }, new() { 23, 77 }, new() { 24, 77 }, new() { 29, 77 }, new() { 30, 77 }, new() { 31, 77 }, new() { 32, 77 }, new() { 33, 77 }, new() { 34, 77 }, new() { 35, 77 }, new() { 36, 77 }, new() { 37, 77 }, new() { 38, 77 }, new() { 39, 77 }, new() { 40, 77 }, new() { 41, 77 }, new() { 42, 77 }, new() { 43, 77 }, new() { 44, 77 }, new() { 45, 77 }, new() { 46, 77 }, new() { 47, 77 }, new() { 48, 77 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [34] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 77 }, new() { 7, 77 }, new() { 8, 77 }, new() { 9, 77 }, new() { 10, 77 }, new() { 11, 77 }, new() { 12, 77 }, new() { 13, 77 }, new() { 14, 77 }, new() { 15, 77 }, new() { 16, 77 }, new() { 17, 77 }, new() { 18, 77 }, new() { 19, 77 }, new() { 20, 77 }, new() { 21, 77 }, new() { 22, 77 }, new() { 23, 77 }, new() { 24, 77 }, new() { 25, 77 }, new() { 26, 77 }, new() { 27, 77 }, new() { 28, 77 }, new() { 29, 77 }, new() { 30, 77 }, new() { 31, 77 }, new() { 32, 77 }, new() { 33, 77 }, new() { 34, 77 }, new() { 35, 77 }, new() { 36, 77 }, new() { 37, 77 }, new() { 38, 77 }, new() { 39, 77 }, new() { 40, 77 }, new() { 41, 77 }, new() { 42, 77 }, new() { 43, 77 }, new() { 44, 77 }, new() { 45, 77 }, new() { 46, 77 }, new() { 47, 77 }, new() { 48, 77 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },
                [35] = new() { new() { 0, 77 }, new() { 1, 77 }, new() { 2, 77 }, new() { 3, 77 }, new() { 4, 77 }, new() { 5, 77 }, new() { 6, 77 }, new() { 7, 77 }, new() { 8, 77 }, new() { 9, 77 }, new() { 10, 77 }, new() { 11, 77 }, new() { 12, 77 }, new() { 13, 77 }, new() { 14, 77 }, new() { 15, 77 }, new() { 16, 77 }, new() { 17, 77 }, new() { 18, 77 }, new() { 19, 77 }, new() { 20, 77 }, new() { 21, 77 }, new() { 22, 77 }, new() { 23, 77 }, new() { 24, 77 }, new() { 25, 77 }, new() { 26, 77 }, new() { 27, 77 }, new() { 28, 77 }, new() { 29, 77 }, new() { 30, 77 }, new() { 31, 77 }, new() { 32, 77 }, new() { 33, 77 }, new() { 34, 77 }, new() { 35, 77 }, new() { 36, 77 }, new() { 37, 77 }, new() { 38, 77 }, new() { 39, 77 }, new() { 40, 77 }, new() { 41, 77 }, new() { 42, 77 }, new() { 43, 77 }, new() { 44, 77 }, new() { 45, 77 }, new() { 46, 77 }, new() { 47, 77 }, new() { 48, 77 }, new() { 49, 77 }, new() { 50, 77 }, new() { 51, 77 }, new() { 52, 77 }, new() { 53, 77 }, new() { 54, 77 }, new() { 55, 77 }, },


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
                [0] = new() { new() { 12, 41 }, new() { 13, 42 }, new() { 14, 9 }, new() { 15, 10 }, new() { 16, 29 }, new() { 17, 25 }, new() { 18, 26 }, new() { 35, 25 }, new() { 36, 26 }, new() { 37, 43 }, new() { 38, 44 }, new() { 39, 41 }, new() { 40, 42 }, },
                [1] = new() { new() { 11, 63 }, new() { 12, 29 }, new() { 13, 77 }, new() { 14, 77 }, new() { 15, 57 }, new() { 16, 58 }, new() { 37, 59 }, new() { 38, 60 }, new() { 39, 29 }, new() { 40, 9 }, new() { 41, 10 }, new() { 42, 63 }, new() { 43, 41 }, new() { 44, 42 }, },
                [2] = new() { new() { 9, 29 }, new() { 10, 25 }, new() { 11, 26 }, new() { 12, 77 }, new() { 13, 61 }, new() { 14, 12 }, new() { 39, 57 }, new() { 40, 58 }, new() { 41, 61 }, new() { 42, 9 }, new() { 43, 10 }, },
                [3] = new() { new() { 7, 9 }, new() { 8, 10 }, new() { 12, 58 }, new() { 42, 25 }, new() { 43, 26 }, new() { 44, 63 }, new() { 45, 41 }, new() { 46, 42 }, },
                [4] = new() { new() { 8, 61 }, new() { 44, 57 }, new() { 45, 43 }, new() { 46, 44 }, },
                [5] = new() { new() { 7, 29 }, new() { 8, 77 }, new() { 45, 59 }, new() { 46, 60 }, },
                [6] = new() { new() { 7, 57 }, new() { 8, 58 }, new() { 45, 29 }, new() { 46, 41 }, new() { 47, 42 }, },
                [7] = new() { new() { 7, 29 }, new() { 45, 57 }, new() { 46, 58 }, new() { 47, 63 }, },
                [8] = new() { new() { 6, 63 }, new() { 7, 58 }, new() { 45, 77 }, new() { 46, 43 }, new() { 47, 44 }, },
                [9] = new() { new() { 6, 10 }, new() { 7, 61 }, new() { 46, 59 }, new() { 47, 60 }, },
                [10] = new() { new() { 6, 25 }, new() { 7, 26 }, new() { 46, 44 }, new() { 47, 29 }, },
                [11] = new() { new() { 6, 29 }, new() { 46, 25 }, new() { 47, 26 }, },
                [12] = new() { new() { 6, 58 }, new() { 46, 77 }, new() { 47, 57 }, new() { 48, 63 }, },
                [13] = new() { new() { 6, 77 }, new() { 47, 77 }, new() { 48, 29 }, },
                [14] = new() { new() { 6, 12 }, new() { 47, 77 }, new() { 48, 61 }, },
                [15] = new() { new() { 47, 15 }, new() { 48, 63 }, },
                [16] = new() { new() { 47, 77 }, new() { 48, 29 }, },
                [17] = new() { new() { 47, 57 }, new() { 48, 63 }, },
                [18] = new() { new() { 47, 77 }, new() { 48, 15 }, },
                [19] = new() { new() { 47, 61 }, new() { 48, 29 }, },
                [20] = new() { new() { 6, 180 }, new() { 47, 77 }, new() { 48, 63 }, },
                [21] = new() { new() { 6, 196 }, new() { 7, 216 }, new() { 47, 15 }, new() { 48, 77 }, },
                [22] = new() { new() { 6, 46 }, new() { 7, 232 }, new() { 46, 191 }, new() { 47, 29 }, new() { 48, 63 }, },
                [23] = new() { new() { 6, 62 }, new() { 7, 213 }, new() { 8, 216 }, new() { 45, 220 }, new() { 46, 206 }, new() { 47, 44 }, new() { 48, 29 }, },
                [24] = new() { new() { 6, 45 }, new() { 7, 46 }, new() { 8, 232 }, new() { 45, 236 }, new() { 46, 77 }, new() { 47, 29 }, },
                [25] = new() { new() { 6, 44 }, new() { 7, 62 }, new() { 8, 213 }, new() { 9, 216 }, new() { 44, 220 }, new() { 45, 221 }, new() { 46, 25 }, new() { 47, 26 }, },
                [26] = new() { new() { 7, 25 }, new() { 8, 26 }, new() { 9, 232 }, new() { 44, 236 }, new() { 45, 77 }, new() { 46, 29 }, new() { 47, 63 }, },
                [27] = new() { new() { 6, 9 }, new() { 7, 10 }, new() { 8, 77 }, new() { 9, 213 }, new() { 10, 197 }, new() { 43, 220 }, new() { 44, 221 }, new() { 45, 15 }, new() { 46, 9 }, new() { 47, 10 }, },
                [28] = new() { new() { 8, 45 }, new() { 9, 46 }, new() { 10, 213 }, new() { 11, 197 }, new() { 43, 236 }, new() { 44, 77 }, new() { 45, 29 }, },
                [29] = new() { new() { 8, 61 }, new() { 9, 62 }, new() { 10, 29 }, new() { 11, 213 }, new() { 12, 197 }, new() { 42, 205 }, new() { 43, 221 }, new() { 44, 77 }, new() { 45, 63 }, },
                [30] = new() { new() { 9, 29 }, new() { 10, 25 }, new() { 11, 26 }, new() { 12, 213 }, new() { 13, 214 }, new() { 14, 197 }, new() { 41, 205 }, new() { 42, 221 }, new() { 43, 25 }, new() { 44, 26 }, },
                [31] = new() { new() { 12, 44 }, new() { 13, 77 }, new() { 14, 213 }, new() { 15, 215 }, new() { 16, 197 }, new() { 38, 205 }, new() { 39, 214 }, new() { 40, 215 }, new() { 41, 221 }, new() { 42, 61 }, new() { 43, 29 }, new() { 44, 9 }, new() { 45, 10 }, },
                [32] = new() { new() { 11, 9 }, new() { 12, 10 }, new() { 13, 61 }, new() { 14, 77 }, new() { 15, 77 }, new() { 16, 213 }, new() { 17, 215 }, new() { 18, 215 }, new() { 19, 215 }, new() { 20, 215 }, new() { 21, 215 }, new() { 22, 215 }, new() { 23, 215 }, new() { 24, 215 }, new() { 25, 197 }, new() { 30, 205 }, new() { 31, 214 }, new() { 32, 214 }, new() { 33, 215 }, new() { 34, 214 }, new() { 35, 214 }, new() { 36, 215 }, new() { 37, 214 }, new() { 38, 221 }, new() { 39, 77 }, new() { 40, 77 }, new() { 41, 77 }, new() { 42, 9 }, new() { 43, 10 }, new() { 44, 63 }, },
                [33] = new() { new() { 13, 63 }, new() { 14, 29 }, new() { 15, 25 }, new() { 16, 26 }, new() { 17, 77 }, new() { 18, 77 }, new() { 19, 77 }, new() { 20, 77 }, new() { 21, 77 }, new() { 22, 77 }, new() { 23, 77 }, new() { 24, 77 }, new() { 25, 213 }, new() { 26, 215 }, new() { 27, 214 }, new() { 28, 214 }, new() { 29, 215 }, new() { 30, 221 }, new() { 31, 77 }, new() { 32, 77 }, new() { 33, 61 }, new() { 34, 77 }, new() { 35, 77 }, new() { 36, 61 }, new() { 37, 77 }, new() { 38, 25 }, new() { 39, 26 }, new() { 40, 77 }, new() { 41, 29 }, },
                [34] = new() { new() { 13, 9 }, new() { 14, 10 }, new() { 15, 77 }, new() { 16, 63 }, new() { 17, 29 }, new() { 18, 63 }, new() { 19, 29 }, new() { 20, 63 }, new() { 21, 63 }, new() { 22, 29 }, new() { 23, 63 }, new() { 24, 63 }, new() { 25, 26 }, new() { 26, 77 }, new() { 27, 25 }, new() { 28, 25 }, new() { 29, 63 }, new() { 30, 25 }, new() { 31, 63 }, new() { 32, 63 }, new() { 33, 77 }, new() { 34, 77 }, new() { 35, 63 }, new() { 36, 77 }, new() { 37, 77 }, new() { 38, 9 }, new() { 39, 10 }, },
                [35] = new() { new() { 25, 63 }, new() { 26, 29 }, new() { 27, 77 }, new() { 28, 77 }, new() { 29, 77 }, new() { 30, 9 }, },

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
                [0] = new() { },
                [1] = new() { },
                [2] = new() { },
                [3] = new() { },
                [4] = new() { },
                [5] = new() { },
                [6] = new() { },
                [7] = new() { },
                [8] = new() { new() { 19, 0 }, new() { 20, 1 }, new() { 34, 0 }, new() { 35, 1 }, },
                [9] = new() { new() { 19, 10 }, new() { 20, 11 }, new() { 34, 10 }, new() { 35, 11 }, },
                [10] = new() { new() { 19, 20 }, new() { 20, 21 }, new() { 34, 20 }, new() { 35, 21 }, },
                [11] = new() { new() { 19, 30 }, new() { 20, 31 }, new() { 34, 30 }, new() { 35, 31 }, },
                [12] = new() { },
                [13] = new() { },
                [14] = new() { },
                [15] = new() { new() { 16, 0 }, new() { 17, 1 }, new() { 37, 0 }, new() { 38, 1 }, },
                [16] = new() { new() { 16, 10 }, new() { 17, 11 }, new() { 37, 10 }, new() { 38, 11 }, },
                [17] = new() { new() { 16, 20 }, new() { 17, 21 }, new() { 37, 20 }, new() { 38, 21 }, },
                [18] = new() { new() { 16, 30 }, new() { 17, 31 }, new() { 37, 30 }, new() { 38, 31 }, },
                [19] = new() { },
                [20] = new() { },
                [21] = new() { },
                [22] = new() { new() { 19, 0 }, new() { 20, 1 }, new() { 34, 0 }, new() { 35, 1 }, },
                [23] = new() { new() { 19, 10 }, new() { 20, 11 }, new() { 34, 10 }, new() { 35, 11 }, },
                [24] = new() { new() { 19, 20 }, new() { 20, 21 }, new() { 34, 20 }, new() { 35, 21 }, },
                [25] = new() { new() { 19, 30 }, new() { 20, 31 }, new() { 34, 30 }, new() { 35, 31 }, },
                [26] = new() { },
                [27] = new() { },
                [28] = new() { },
                [29] = new() { },
                [30] = new() { },
                [31] = new() { },
                [32] = new() { },
                [33] = new() { },
                [34] = new() { },
                [35] = new() { },



            };

            foreach (KeyValuePair<int, List<List<int>>> code in codes)
            {

                foreach (List<int> array in code.Value)
                {

                    Vector2 codeVector = new(array[1] % 10, (int)(array[1] / 10));

                    int offset = 4 - (int)codeVector.Y;

                    LocationTile lTile = new(array[0], code.Key, (int)codeVector.X, (int)codeVector.Y, offset, offset == 1, IconData.tilesheets.tomb);

                    if (offset == 1)
                    {

                        buildings.Tiles[array[0], code.Key] = new StaticTile(buildings, dungeon, BlendMode.Alpha, back.Tiles[array[0], code.Key].TileIndex);

                    }

                    locationTiles.Add(lTile);

                }

            }


            codes = new()
            {

                [0] = new() { },
                [1] = new() { },
                [2] = new() { },
                [3] = new() { },
                [4] = new() { new() { 22, 2 }, },
                [5] = new() { },
                [6] = new() { },
                [7] = new() { new() { 15, 2 }, new() { 19, 1 }, new() { 34, 1 }, },
                [8] = new() { new() { 41, 2 }, },
                [9] = new() { },
                [10] = new() { },
                [11] = new() { new() { 12, 2 }, },
                [12] = new() { },
                [13] = new() { },
                [14] = new() { new() { 16, 1 }, new() { 37, 1 }, new() { 43, 2 }, },
                [15] = new() { },
                [16] = new() { },
                [17] = new() { },
                [18] = new() { },
                [19] = new() { new() { 10, 2 }, },
                [20] = new() { new() { 43, 2 }, },
                [21] = new() { new() { 19, 1 }, new() { 34, 1 }, },
                [22] = new() { },
                [23] = new() { },
                [24] = new() { },
                [25] = new() { new() { 12, 2 }, },
                [26] = new() { new() { 41, 2 }, },
                [27] = new() { },
                [28] = new() { },
                [29] = new() { new() { 15, 2 }, },
                [30] = new() { new() { 22, 2 }, new() { 32, 2 }, new() { 37, 2 }, },
                [31] = new() { },
                [32] = new() { },
                [33] = new() { },
                [34] = new() { },
                [35] = new() { },


            };

            foreach (KeyValuePair<int, List<List<int>>> code in codes)
            {

                foreach (List<int> array in code.Value)
                {
                    if (array[1] == 1)
                    {
                        lightFields.Add(new Vector2(array[0], code.Key) * 64 + new Vector2(64, 32), 6);

                        brazierTiles.Add(new Vector2(array[0], code.Key)*64, Mod.instance.randomIndex.Next(4));

                    }
                    else
                    {
                        darkFields.Add(new Vector2(array[0], code.Key) * 64 + new Vector2(0, 32), 4 + Mod.instance.randomIndex.Next(3));

                    }

                }

            }

            this.map = newMap;

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

            warpSets.Add(new WarpTile(26, 34, "SkullCave", 5, 5));

            warpSets.Add(new WarpTile(27, 34, "SkullCave", 5, 5));

            warpSets.Add(new WarpTile(28, 34, "SkullCave", 5, 5));

            warpSets.Add(new WarpTile(29, 34, "SkullCave", 5, 5));

            warpSets.Add(new WarpTile(31, 4, "UndergroundMine145", 1, 1));

            warps.Add(new Warp(26, 34, "SkullCave", 5, 5, flipFarmer: false));

            warps.Add(new Warp(27, 34, "SkullCave", 5, 5, flipFarmer: false));

            warps.Add(new Warp(28, 34, "SkullCave", 5, 5, flipFarmer: false));

            warps.Add(new Warp(29, 34, "SkullCave", 5, 5, flipFarmer: false));

            warps.Add(new Warp(31, 4, "UndergroundMine145", 1, 1, flipFarmer: false));

        }

    }

}
