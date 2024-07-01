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

namespace StardewDruid.Location
{
    public class Chapel : GameLocation
    {

        protected Texture2D _dayParallaxTexture;

        protected Texture2D _nightParallaxTexture;

        public List<Location.WarpTile> warpSets = new();

        public List<Location.LocationTile> locationTiles = new();

        public Dictionary<Vector2, CharacterHandle.characters> dialogueTiles = new();

        public Dictionary<Vector2, int> lightFields = new();

        public Dictionary<Vector2, int> darkFields = new();

        public bool ambientDarkness;

        public Chapel() { }

        public Chapel(string Name)
            : base("Maps\\Shed", Name) 
        {

        }

        public override void draw(SpriteBatch b)
        {
            
            IsOutdoors = false;

            ignoreOutdoorLighting.Set(false);

            base.draw(b);

            Dictionary<Vector2,int> occupied = new();

            foreach(Farmer character in farmers)
            {

                Vector2 check = ModUtility.PositionToTile(character.Position);

                for (int i = 0; i < 2; i++)
                {

                    List<Vector2> around = ModUtility.GetTilesWithinRadius(this, check, i);

                    foreach(Vector2 tile in around)
                    {
                        
                        occupied[tile] = (int)check.Y+1;

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
                        
                        if (occupied[tile.tile] < (int)tile.tile.Y+tile.offset)
                        {
                        
                            opacity = 0.75f;
                        
                        }

                    }

                    Microsoft.Xna.Framework.Vector2 position = new(tile.position.X - (float)Game1.viewport.X, tile.position.Y - (float)Game1.viewport.Y);

                    b.Draw(Mod.instance.iconData.sheetTextures[tile.tilesheet], position, tile.rectangle, Microsoft.Xna.Framework.Color.White * opacity, 0f, Vector2.Zero, 4, tile.flip ? (SpriteEffects)1 : 0, tile.layer+ (tile.offset*0.0064f));

                    if (tile.shadow)
                    {
                        
                        b.Draw(Mod.instance.iconData.sheetTextures[tile.tilesheet], position + new Vector2(1,8), tile.rectangle, Microsoft.Xna.Framework.Color.Black *0.35f, 0f, Vector2.Zero, 4, tile.flip ? (SpriteEffects)1 : 0, tile.layer - 0.001f);

                    }

                }

            }

            float ambience = 0.4f;

            foreach (KeyValuePair<Vector2,int> light in darkFields)
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
                        Microsoft.Xna.Framework.Color.LightGoldenrodYellow * 0.75f,
                        0f,
                        new Vector2(texture2D.Bounds.Width / 2, texture2D.Bounds.Height / 2),
                        0.25f*light.Value,
                        SpriteEffects.None,
                        0.9f
                    );

                }

            }

        }

        public override void drawBackground(SpriteBatch b)
        {
            base.drawBackground(b);
            DrawParallaxHorizon(b);

        }

        public virtual void DrawParallaxHorizon(SpriteBatch b, bool horizontal_parallax = true)
        {
            float num = 4f;
            if (_dayParallaxTexture == null || _dayParallaxTexture.IsDisposed)
            {
                _dayParallaxTexture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\Cloudy_Ocean_BG");
            }

            if (_nightParallaxTexture == null || _dayParallaxTexture.IsDisposed)
            {
                _nightParallaxTexture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\Cloudy_Ocean_BG_Night");
            }

            float num2 = (float)_dayParallaxTexture.Width * num - (float)map.DisplayWidth;
            float t = 0f;
            int num3 = -640;
            int y = (int)((float)Game1.viewport.Y * 0.2f + (float)num3);
            if (horizontal_parallax)
            {
                if (map.DisplayWidth - Game1.viewport.Width < 0)
                {
                    t = 0.5f;
                }
                else if (map.DisplayWidth - Game1.viewport.Width > 0)
                {
                    t = (float)Game1.viewport.X / (float)(map.DisplayWidth - Game1.viewport.Width) / 3;
                }
            }
            else
            {
                t = 0.5f;
            }

            if (Game1.game1.takingMapScreenshot)
            {
                y = num3;
                t = 0.5f;
            }

            float num4 = 0.25f;
            t = Utility.Lerp(0.5f + num4, 0.5f - num4, t);
            float value = (float)Utility.ConvertTimeToMinutes(Game1.timeOfDay + (int)((float)Game1.gameTimeInterval / (float)Game1.realMilliSecondsPerGameMinute % 10f) - Game1.getStartingToGetDarkTime(this)) / (float)Utility.ConvertTimeToMinutes(Game1.getTrulyDarkTime(this) - Game1.getStartingToGetDarkTime(this));
            value = Utility.Clamp(value, 0f, 1f);
            b.Draw(Game1.staminaRect, Game1.GlobalToLocal(Game1.viewport, new Microsoft.Xna.Framework.Rectangle(0, 0, map.DisplayWidth, map.DisplayHeight)), new Color(1, 122, 217, 255));
            b.Draw(Game1.staminaRect, Game1.GlobalToLocal(Game1.viewport, new Microsoft.Xna.Framework.Rectangle(0, 0, map.DisplayWidth, map.DisplayHeight)), new Color(0, 7, 63, 255) * value);
            Microsoft.Xna.Framework.Rectangle globalPosition = new Microsoft.Xna.Framework.Rectangle((int)((0f - num2) * t), y, (int)((float)_dayParallaxTexture.Width * num), (int)((float)_dayParallaxTexture.Height * num));
            Microsoft.Xna.Framework.Rectangle value2 = new Microsoft.Xna.Framework.Rectangle(0, 0, _dayParallaxTexture.Width, _dayParallaxTexture.Height);
            int num5 = 0;
            if (globalPosition.X < num5)
            {
                int num6 = num5 - globalPosition.X;
                globalPosition.X += num6;
                globalPosition.Width -= num6;
                value2.X += (int)((float)num6 / num);
                value2.Width -= (int)((float)num6 / num);
            }

            int displayWidth = map.DisplayWidth;
            if (globalPosition.X + globalPosition.Width > displayWidth)
            {
                int num7 = globalPosition.X + globalPosition.Width - displayWidth;
                globalPosition.Width -= num7;
                value2.Width -= (int)((float)num7 / num);
            }

            if (value2.Width > 0 && globalPosition.Width > 0)
            {
                Microsoft.Xna.Framework.Color rain = Game1.isRaining ? Color.LightSkyBlue : Color.White;
                b.Draw(_dayParallaxTexture, Game1.GlobalToLocal(Game1.viewport, globalPosition), value2, rain, 0f, Vector2.Zero, SpriteEffects.None, 0f);
                b.Draw(_nightParallaxTexture, Game1.GlobalToLocal(Game1.viewport, globalPosition), value2, Color.White * value, 0f, Vector2.Zero, SpriteEffects.None, 0f);
            }
        }

        public override void OnMapLoad(xTile.Map map)
        {

            /*
             
            60 0

            85
            89

            7
            11

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

            TileSheet chapelsheet = new(
                newMap,
                IconData.chapel_assetName,
                new(
                    Mod.instance.iconData.sheetTextures[IconData.tilesheets.chapel].Width / 16,
                    Mod.instance.iconData.sheetTextures[IconData.tilesheets.chapel].Height / 16
                ),
                new(16, 16)
            );

            newMap.AddTileSheet(chapelsheet);

            newMap.LoadTileSheets(Game1.mapDisplayDevice);

            locationTiles = new();

            Dictionary<int, List<List<int>>> codes = new()
            {
                [0] = new() { },
                [1] = new() { },
                [2] = new() { },
                [3] = new() { },
                [4] = new() { },
                [5] = new() { },
                [6] = new() { },
                [7] = new() { },
                [8] = new() { },
                [9] = new() { },
                [10] = new() { },
                [11] = new() { },
                [12] = new() { new() { 19, 3 }, new() { 20, 3 }, new() { 21, 3 }, new() { 22, 3 }, new() { 23, 3 }, new() { 24, 3 }, new() { 25, 3 }, new() { 26, 3 }, new() { 27, 3 }, new() { 28, 3 }, new() { 29, 3 }, new() { 30, 3 }, new() { 31, 3 }, new() { 32, 3 }, new() { 33, 3 }, new() { 34, 3 }, new() { 35, 3 }, new() { 36, 3 }, },
                [13] = new() { new() { 18, 3 }, new() { 19, 3 }, new() { 20, 17 }, new() { 21, 18 }, new() { 22, 17 }, new() { 23, 18 }, new() { 24, 17 }, new() { 25, 18 }, new() { 26, 17 }, new() { 27, 18 }, new() { 28, 17 }, new() { 29, 18 }, new() { 30, 17 }, new() { 31, 18 }, new() { 32, 17 }, new() { 33, 18 }, new() { 34, 17 }, new() { 35, 18 }, new() { 36, 3 }, new() { 37, 3 }, },
                [14] = new() { new() { 17, 3 }, new() { 18, 3 }, new() { 19, 3 }, new() { 20, 31 }, new() { 21, 32 }, new() { 22, 31 }, new() { 23, 32 }, new() { 24, 31 }, new() { 25, 32 }, new() { 26, 31 }, new() { 27, 32 }, new() { 28, 31 }, new() { 29, 32 }, new() { 30, 31 }, new() { 31, 32 }, new() { 32, 31 }, new() { 33, 32 }, new() { 34, 31 }, new() { 35, 32 }, new() { 36, 3 }, new() { 37, 3 }, new() { 38, 3 }, },
                [15] = new() { new() { 16, 3 }, new() { 17, 3 }, new() { 18, 17 }, new() { 19, 18 }, new() { 20, 17 }, new() { 21, 18 }, new() { 22, 17 }, new() { 23, 18 }, new() { 24, 17 }, new() { 25, 18 }, new() { 26, 17 }, new() { 27, 18 }, new() { 28, 17 }, new() { 29, 18 }, new() { 30, 17 }, new() { 31, 18 }, new() { 32, 17 }, new() { 33, 18 }, new() { 34, 17 }, new() { 35, 18 }, new() { 36, 17 }, new() { 37, 18 }, new() { 38, 3 }, new() { 39, 3 }, },
                [16] = new() { new() { 15, 3 }, new() { 16, 3 }, new() { 17, 3 }, new() { 18, 31 }, new() { 19, 32 }, new() { 20, 31 }, new() { 21, 32 }, new() { 22, 31 }, new() { 23, 60 }, new() { 24, 59 }, new() { 25, 60 }, new() { 26, 59 }, new() { 27, 60 }, new() { 28, 59 }, new() { 29, 60 }, new() { 30, 59 }, new() { 31, 60 }, new() { 32, 59 }, new() { 33, 32 }, new() { 34, 31 }, new() { 35, 32 }, new() { 36, 31 }, new() { 37, 32 }, new() { 38, 3 }, new() { 39, 3 }, new() { 40, 3 }, },
                [17] = new() { new() { 14, 3 }, new() { 15, 3 }, new() { 16, 17 }, new() { 17, 18 }, new() { 18, 17 }, new() { 19, 18 }, new() { 20, 17 }, new() { 21, 18 }, new() { 22, 17 }, new() { 23, 46 }, new() { 24, 45 }, new() { 25, 46 }, new() { 26, 45 }, new() { 27, 46 }, new() { 28, 45 }, new() { 29, 46 }, new() { 30, 45 }, new() { 31, 46 }, new() { 32, 45 }, new() { 33, 18 }, new() { 34, 17 }, new() { 35, 18 }, new() { 36, 17 }, new() { 37, 18 }, new() { 38, 17 }, new() { 39, 18 }, new() { 40, 3 }, new() { 41, 3 }, },
                [18] = new() { new() { 13, 3 }, new() { 14, 3 }, new() { 15, 3 }, new() { 16, 31 }, new() { 17, 32 }, new() { 18, 31 }, new() { 19, 32 }, new() { 20, 31 }, new() { 21, 60 }, new() { 22, 59 }, new() { 23, 60 }, new() { 24, 59 }, new() { 25, 60 }, new() { 26, 59 }, new() { 27, 60 }, new() { 28, 59 }, new() { 29, 60 }, new() { 30, 59 }, new() { 31, 60 }, new() { 32, 59 }, new() { 33, 60 }, new() { 34, 59 }, new() { 35, 32 }, new() { 36, 31 }, new() { 37, 32 }, new() { 38, 31 }, new() { 39, 32 }, new() { 40, 3 }, new() { 41, 3 }, new() { 42, 3 }, },
                [19] = new() { new() { 13, 3 }, new() { 14, 17 }, new() { 15, 18 }, new() { 16, 17 }, new() { 17, 18 }, new() { 18, 17 }, new() { 19, 18 }, new() { 20, 17 }, new() { 21, 46 }, new() { 22, 45 }, new() { 23, 46 }, new() { 24, 45 }, new() { 25, 46 }, new() { 26, 45 }, new() { 27, 46 }, new() { 28, 45 }, new() { 29, 46 }, new() { 30, 45 }, new() { 31, 46 }, new() { 32, 45 }, new() { 33, 46 }, new() { 34, 45 }, new() { 35, 18 }, new() { 36, 17 }, new() { 37, 18 }, new() { 38, 17 }, new() { 39, 18 }, new() { 40, 17 }, new() { 41, 18 }, new() { 42, 3 }, },
                [20] = new() { new() { 13, 3 }, new() { 14, 31 }, new() { 15, 32 }, new() { 16, 31 }, new() { 17, 32 }, new() { 18, 31 }, new() { 19, 32 }, new() { 20, 31 }, new() { 21, 60 }, new() { 22, 59 }, new() { 23, 60 }, new() { 24, 59 }, new() { 25, 60 }, new() { 26, 59 }, new() { 27, 60 }, new() { 28, 59 }, new() { 29, 60 }, new() { 30, 59 }, new() { 31, 60 }, new() { 32, 59 }, new() { 33, 60 }, new() { 34, 59 }, new() { 35, 32 }, new() { 36, 31 }, new() { 37, 32 }, new() { 38, 31 }, new() { 39, 32 }, new() { 40, 31 }, new() { 41, 32 }, new() { 42, 3 }, },
                [21] = new() { new() { 13, 3 }, new() { 14, 17 }, new() { 15, 18 }, new() { 16, 17 }, new() { 17, 18 }, new() { 18, 17 }, new() { 19, 18 }, new() { 20, 17 }, new() { 21, 46 }, new() { 22, 45 }, new() { 23, 46 }, new() { 24, 45 }, new() { 25, 46 }, new() { 26, 45 }, new() { 27, 46 }, new() { 28, 45 }, new() { 29, 46 }, new() { 30, 45 }, new() { 31, 46 }, new() { 32, 45 }, new() { 33, 46 }, new() { 34, 45 }, new() { 35, 18 }, new() { 36, 17 }, new() { 37, 18 }, new() { 38, 17 }, new() { 39, 18 }, new() { 40, 17 }, new() { 41, 18 }, new() { 42, 3 }, },
                [22] = new() { new() { 13, 3 }, new() { 14, 31 }, new() { 15, 32 }, new() { 16, 31 }, new() { 17, 32 }, new() { 18, 31 }, new() { 19, 32 }, new() { 20, 31 }, new() { 21, 60 }, new() { 22, 59 }, new() { 23, 60 }, new() { 24, 59 }, new() { 25, 60 }, new() { 26, 59 }, new() { 27, 60 }, new() { 28, 59 }, new() { 29, 60 }, new() { 30, 59 }, new() { 31, 60 }, new() { 32, 59 }, new() { 33, 60 }, new() { 34, 59 }, new() { 35, 32 }, new() { 36, 31 }, new() { 37, 32 }, new() { 38, 31 }, new() { 39, 32 }, new() { 40, 31 }, new() { 41, 32 }, new() { 42, 3 }, },
                [23] = new() { new() { 14, 3 }, new() { 15, 3 }, new() { 16, 17 }, new() { 17, 18 }, new() { 18, 17 }, new() { 19, 18 }, new() { 20, 17 }, new() { 21, 46 }, new() { 22, 45 }, new() { 23, 46 }, new() { 24, 45 }, new() { 25, 46 }, new() { 26, 45 }, new() { 27, 46 }, new() { 28, 45 }, new() { 29, 46 }, new() { 30, 45 }, new() { 31, 46 }, new() { 32, 45 }, new() { 33, 46 }, new() { 34, 45 }, new() { 35, 18 }, new() { 36, 17 }, new() { 37, 18 }, new() { 38, 17 }, new() { 39, 18 }, new() { 40, 3 }, new() { 41, 3 }, },
                [24] = new() { new() { 15, 3 }, new() { 16, 31 }, new() { 17, 32 }, new() { 18, 31 }, new() { 19, 32 }, new() { 20, 31 }, new() { 21, 32 }, new() { 22, 31 }, new() { 23, 32 }, new() { 24, 31 }, new() { 25, 60 }, new() { 26, 59 }, new() { 27, 60 }, new() { 28, 59 }, new() { 29, 60 }, new() { 30, 59 }, new() { 31, 32 }, new() { 32, 31 }, new() { 33, 32 }, new() { 34, 31 }, new() { 35, 32 }, new() { 36, 31 }, new() { 37, 32 }, new() { 38, 31 }, new() { 39, 32 }, new() { 40, 3 }, },
                [25] = new() { new() { 16, 3 }, new() { 17, 3 }, new() { 18, 17 }, new() { 19, 18 }, new() { 20, 17 }, new() { 21, 18 }, new() { 22, 17 }, new() { 23, 18 }, new() { 24, 17 }, new() { 25, 46 }, new() { 26, 45 }, new() { 27, 46 }, new() { 28, 45 }, new() { 29, 46 }, new() { 30, 45 }, new() { 31, 18 }, new() { 32, 17 }, new() { 33, 18 }, new() { 34, 17 }, new() { 35, 18 }, new() { 36, 17 }, new() { 37, 18 }, new() { 38, 3 }, new() { 39, 3 }, },
                [26] = new() { new() { 17, 3 }, new() { 18, 31 }, new() { 19, 32 }, new() { 20, 31 }, new() { 21, 32 }, new() { 22, 31 }, new() { 23, 32 }, new() { 24, 31 }, new() { 25, 32 }, new() { 26, 31 }, new() { 27, 32 }, new() { 28, 31 }, new() { 29, 32 }, new() { 30, 31 }, new() { 31, 32 }, new() { 32, 31 }, new() { 33, 32 }, new() { 34, 31 }, new() { 35, 32 }, new() { 36, 31 }, new() { 37, 32 }, new() { 38, 3 }, },
                [27] = new() { new() { 18, 3 }, new() { 19, 3 }, new() { 20, 17 }, new() { 21, 18 }, new() { 22, 17 }, new() { 23, 18 }, new() { 24, 17 }, new() { 25, 18 }, new() { 26, 17 }, new() { 27, 18 }, new() { 28, 17 }, new() { 29, 18 }, new() { 30, 17 }, new() { 31, 18 }, new() { 32, 17 }, new() { 33, 18 }, new() { 34, 17 }, new() { 35, 18 }, new() { 36, 3 }, new() { 37, 3 }, },
                [28] = new() { new() { 19, 3 }, new() { 20, 31 }, new() { 21, 32 }, new() { 22, 31 }, new() { 23, 32 }, new() { 24, 31 }, new() { 25, 32 }, new() { 26, 31 }, new() { 27, 32 }, new() { 28, 31 }, new() { 29, 32 }, new() { 30, 31 }, new() { 31, 32 }, new() { 32, 31 }, new() { 33, 32 }, new() { 34, 31 }, new() { 35, 32 }, new() { 36, 3 }, },
                [29] = new() { new() { 20, 3 }, new() { 21, 3 }, new() { 22, 3 }, new() { 23, 3 }, new() { 24, 3 }, new() { 25, 3 }, new() { 26, 3 }, new() { 27, 3 }, new() { 28, 3 }, new() { 29, 3 }, new() { 30, 3 }, new() { 31, 3 }, new() { 32, 3 }, new() { 33, 3 }, new() { 34, 3 }, new() { 35, 3 }, },
                [30] = new() { new() { 25, 4 }, new() { 26, 4 }, new() { 27, 4 }, new() { 28, 4 }, new() { 29, 4 }, new() { 30, 4 }, },
                [31] = new() { new() { 25, 4 }, new() { 26, 4 }, new() { 27, 4 }, new() { 28, 4 }, new() { 29, 4 }, new() { 30, 4 }, },
                [32] = new() { new() { 25, 4 }, new() { 26, 4 }, new() { 27, 4 }, new() { 28, 4 }, new() { 29, 4 }, new() { 30, 4 }, },
                [33] = new() { new() { 25, 4 }, new() { 26, 4 }, new() { 27, 4 }, new() { 28, 4 }, new() { 29, 4 }, new() { 30, 4 }, },



            };

            foreach (KeyValuePair<int, List<List<int>>> code in codes)
            {

                foreach (List<int> array in code.Value)
                {

                    if (array[1] == 19)
                    {

                        continue;

                    }

                    back.Tiles[array[0], code.Key] = new StaticTile(back, chapelsheet, BlendMode.Alpha, array[1]);

                    back.Tiles[array[0], code.Key].TileIndexProperties.Add("Type", "Stone");

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
                [8] = new() { new() { 19, 5 }, new() { 20, 7 }, new() { 35, 91 }, new() { 36, 89 }, },
                [9] = new() { new() { 18, 6 }, new() { 19, 19 }, new() { 20, 21 }, new() { 35, 105 }, new() { 36, 103 }, new() { 37, 90 }, },
                [10] = new() { new() { 17, 5 }, new() { 18, 20 }, new() { 19, 33 }, new() { 20, 35 }, new() { 35, 119 }, new() { 36, 117 }, new() { 37, 104 }, new() { 38, 89 }, },
                [11] = new() { new() { 16, 6 }, new() { 17, 19 }, new() { 18, 34 }, new() { 19, 47 }, new() { 20, 49 }, new() { 21, 77 }, new() { 22, 77 }, new() { 23, 77 }, new() { 24, 77 }, new() { 25, 77 }, new() { 26, 77 }, new() { 27, 77 }, new() { 28, 77 }, new() { 29, 77 }, new() { 30, 77 }, new() { 31, 77 }, new() { 32, 77 }, new() { 33, 77 }, new() { 34, 77 }, new() { 35, 133 }, new() { 36, 131 }, new() { 37, 118 }, new() { 38, 103 }, new() { 39, 90 }, },
                [12] = new() { new() { 15, 5 }, new() { 16, 20 }, new() { 17, 33 }, new() { 18, 48 }, new() { 19, 61 }, new() { 20, 63 }, new() { 35, 147 }, new() { 36, 145 }, new() { 37, 132 }, new() { 38, 117 }, new() { 39, 104 }, new() { 40, 89 }, },
                [13] = new() { new() { 14, 6 }, new() { 15, 19 }, new() { 16, 34 }, new() { 17, 47 }, new() { 18, 62 }, new() { 19, 75 }, new() { 36, 159 }, new() { 37, 146 }, new() { 38, 132 }, new() { 39, 118 }, new() { 40, 103 }, new() { 41, 90 }, },
                [14] = new() { new() { 13, 5 }, new() { 14, 20 }, new() { 15, 33 }, new() { 16, 48 }, new() { 17, 61 }, new() { 18, 76 }, new() { 37, 160 }, new() { 38, 145 }, new() { 39, 132 }, new() { 40, 117 }, new() { 41, 104 }, new() { 42, 89 }, },
                [15] = new() { new() { 13, 19 }, new() { 14, 34 }, new() { 15, 47 }, new() { 16, 62 }, new() { 17, 75 }, new() { 38, 159 }, new() { 39, 146 }, new() { 40, 131 }, new() { 41, 118 }, new() { 42, 103 }, },
                [16] = new() { new() { 13, 33 }, new() { 14, 48 }, new() { 15, 61 }, new() { 16, 76 }, new() { 39, 160 }, new() { 40, 145 }, new() { 41, 132 }, new() { 42, 117 }, },
                [17] = new() { new() { 13, 47 }, new() { 14, 62 }, new() { 15, 75 }, new() { 40, 159 }, new() { 41, 146 }, new() { 42, 131 }, },
                [18] = new() { new() { 13, 61 }, new() { 14, 76 }, new() { 41, 160 }, new() { 42, 145 }, },
                [19] = new() { new() { 12, -1 }, new() { 13, 75 }, new() { 42, 159 }, new() { 43, -1 }, },
                [20] = new() { new() { 12, -1 }, new() { 43, -1 }, },
                [21] = new() { new() { 12, -1 }, new() { 43, -1 }, },
                [22] = new() { new() { 12, -1 }, new() { 43, -1 }, },
                [23] = new() { new() { 12, -1 }, new() { 43, -1 }, },
                [24] = new() { new() { 12, -1 }, new() { 13, -1 }, new() { 42, -1 }, new() { 43, -1 }, },
                [25] = new() { new() { 13, -1 }, new() { 14, -1 }, new() { 41, -1 }, new() { 42, -1 }, },
                [26] = new() { new() { 14, -1 }, new() { 15, -1 }, new() { 40, -1 }, new() { 41, -1 }, },
                [27] = new() { new() { 15, -1 }, new() { 16, -1 }, new() { 39, -1 }, new() { 40, -1 }, },
                [28] = new() { new() { 16, -1 }, new() { 17, -1 }, new() { 38, -1 }, new() { 39, -1 }, },
                [29] = new() { new() { 17, -1 }, new() { 18, -1 }, new() { 37, -1 }, new() { 38, -1 }, },
                [30] = new() { new() { 18, -1 }, new() { 19, -1 }, new() { 20, -1 }, new() { 21, -1 }, new() { 22, -1 }, new() { 23, -1 }, new() { 24, -1 }, new() { 25, -1 }, new() { 30, -1 }, new() { 31, -1 }, new() { 32, -1 }, new() { 33, -1 }, new() { 34, -1 }, new() { 35, -1 }, new() { 36, -1 }, new() { 37, -1 }, },
                [31] = new() { new() { 25, -1 }, new() { 30, -1 }, },
                [32] = new() { new() { 25, -1 }, new() { 30, -1 }, },
                [33] = new() { new() { 25, -1 }, new() { 30, -1 }, },


            };

            foreach (KeyValuePair<int, List<List<int>>> code in codes)
            {

                foreach (List<int> array in code.Value)
                {

                    buildings.Tiles[array[0], code.Key] = new StaticTile(back, chapelsheet, BlendMode.Alpha, array[1]);

                }

            }

            codes = new()
            {
                [0] = new() { new() { 0, 11 }, new() { 1, 11 }, new() { 2, 11 }, new() { 3, 11 }, new() { 4, 11 }, new() { 5, 11 }, new() { 6, 11 }, new() { 7, 22 }, new() { 51, 134 }, new() { 52, 120 }, new() { 53, 11 }, new() { 54, 11 }, new() { 55, 11 }, },
                [1] = new() { new() { 0, 11 }, new() { 1, 11 }, new() { 2, 11 }, new() { 3, 11 }, new() { 4, 11 }, new() { 5, 11 }, new() { 6, 11 }, new() { 7, 22 }, new() { 50, 134 }, new() { 51, 120 }, new() { 52, 11 }, new() { 53, 11 }, new() { 54, 11 }, new() { 55, 11 }, },
                [2] = new() { new() { 0, 11 }, new() { 1, 11 }, new() { 2, 11 }, new() { 3, 11 }, new() { 4, 11 }, new() { 5, 11 }, new() { 6, 11 }, new() { 7, 51 }, new() { 8, 23 }, new() { 9, 37 }, new() { 49, 134 }, new() { 50, 120 }, new() { 51, 11 }, new() { 52, 11 }, new() { 53, 11 }, new() { 54, 11 }, new() { 55, 11 }, },
                [3] = new() { new() { 0, 11 }, new() { 1, 11 }, new() { 2, 11 }, new() { 3, 11 }, new() { 4, 11 }, new() { 5, 11 }, new() { 6, 11 }, new() { 7, 11 }, new() { 8, 11 }, new() { 9, 22 }, new() { 46, 121 }, new() { 47, 107 }, new() { 48, 162 }, new() { 49, 148 }, new() { 50, 11 }, new() { 51, 11 }, new() { 52, 11 }, new() { 53, 11 }, new() { 54, 11 }, new() { 55, 11 }, },
                [4] = new() { new() { 0, 11 }, new() { 1, 11 }, new() { 2, 11 }, new() { 3, 11 }, new() { 4, 11 }, new() { 5, 11 }, new() { 6, 11 }, new() { 7, 11 }, new() { 8, 11 }, new() { 9, 36 }, new() { 10, 50 }, new() { 46, 106 }, new() { 47, 11 }, new() { 48, 11 }, new() { 49, 11 }, new() { 50, 11 }, new() { 51, 11 }, new() { 52, 11 }, new() { 53, 11 }, new() { 54, 11 }, new() { 55, 11 }, },
                [5] = new() { new() { 0, 11 }, new() { 1, 11 }, new() { 2, 11 }, new() { 3, 11 }, new() { 4, 11 }, new() { 5, 11 }, new() { 6, 11 }, new() { 7, 11 }, new() { 8, 11 }, new() { 9, 11 }, new() { 10, 64 }, new() { 11, 78 }, new() { 12, 9 }, new() { 13, 23 }, new() { 14, 78 }, new() { 15, 23 }, new() { 16, 37 }, new() { 38, 121 }, new() { 39, 107 }, new() { 40, 93 }, new() { 41, 162 }, new() { 42, 107 }, new() { 43, 93 }, new() { 44, 162 }, new() { 45, 93 }, new() { 46, 135 }, new() { 47, 11 }, new() { 48, 11 }, new() { 49, 11 }, new() { 50, 11 }, new() { 51, 11 }, new() { 52, 11 }, new() { 53, 11 }, new() { 54, 11 }, new() { 55, 11 }, },
                [6] = new() { new() { 0, 11 }, new() { 1, 11 }, new() { 2, 11 }, new() { 3, 11 }, new() { 4, 11 }, new() { 5, 11 }, new() { 6, 11 }, new() { 7, 11 }, new() { 8, 11 }, new() { 9, 11 }, new() { 10, 11 }, new() { 11, 11 }, new() { 12, 11 }, new() { 13, 11 }, new() { 14, 11 }, new() { 15, 11 }, new() { 16, 22 }, new() { 38, 106 }, new() { 39, 11 }, new() { 40, 11 }, new() { 41, 11 }, new() { 42, 11 }, new() { 43, 11 }, new() { 44, 11 }, new() { 45, 11 }, new() { 46, 11 }, new() { 47, 11 }, new() { 48, 11 }, new() { 49, 11 }, new() { 50, 11 }, new() { 51, 11 }, new() { 52, 11 }, new() { 53, 11 }, new() { 54, 11 }, new() { 55, 11 }, },
                [7] = new() { new() { 0, 11 }, new() { 1, 11 }, new() { 2, 11 }, new() { 3, 11 }, new() { 4, 11 }, new() { 5, 11 }, new() { 6, 11 }, new() { 7, 11 }, new() { 8, 11 }, new() { 9, 11 }, new() { 10, 11 }, new() { 11, 11 }, new() { 12, 11 }, new() { 13, 11 }, new() { 14, 11 }, new() { 15, 11 }, new() { 16, 51 }, new() { 17, 9 }, new() { 18, 23 }, new() { 19, 78 }, new() { 20, 65 }, new() { 35, 149 }, new() { 36, 162 }, new() { 37, 107 }, new() { 38, 135 }, new() { 39, 11 }, new() { 40, 11 }, new() { 41, 11 }, new() { 42, 11 }, new() { 43, 11 }, new() { 44, 11 }, new() { 45, 11 }, new() { 46, 11 }, new() { 47, 11 }, new() { 48, 11 }, new() { 49, 11 }, new() { 50, 11 }, new() { 51, 11 }, new() { 52, 11 }, new() { 53, 11 }, new() { 54, 11 }, new() { 55, 11 }, },
                [8] = new() { new() { 0, 11 }, new() { 1, 11 }, new() { 2, 11 }, new() { 3, 11 }, new() { 4, 11 }, new() { 5, 11 }, new() { 6, 11 }, new() { 7, 11 }, new() { 8, 11 }, new() { 9, 11 }, new() { 10, 11 }, new() { 11, 11 }, new() { 12, 11 }, new() { 13, 11 }, new() { 14, 11 }, new() { 15, 11 }, new() { 16, 11 }, new() { 17, 11 }, new() { 18, 11 }, new() { 19, 10 }, new() { 20, 80 }, new() { 35, 164 }, new() { 36, 94 }, new() { 37, 11 }, new() { 38, 11 }, new() { 39, 11 }, new() { 40, 11 }, new() { 41, 11 }, new() { 42, 11 }, new() { 43, 11 }, new() { 44, 11 }, new() { 45, 11 }, new() { 46, 11 }, new() { 47, 11 }, new() { 48, 11 }, new() { 49, 11 }, new() { 50, 11 }, new() { 51, 11 }, new() { 52, 11 }, new() { 53, 11 }, new() { 54, 11 }, new() { 55, 11 }, },
                [9] = new() { new() { 0, 11 }, new() { 1, 11 }, new() { 2, 11 }, new() { 3, 11 }, new() { 4, 11 }, new() { 5, 11 }, new() { 6, 11 }, new() { 7, 11 }, new() { 8, 11 }, new() { 9, 11 }, new() { 10, 11 }, new() { 11, 11 }, new() { 12, 11 }, new() { 13, 11 }, new() { 14, 11 }, new() { 15, 11 }, new() { 16, 11 }, new() { 17, 11 }, new() { 18, 10 }, new() { 19, 79 }, new() { 36, 163 }, new() { 37, 94 }, new() { 38, 11 }, new() { 39, 11 }, new() { 40, 11 }, new() { 41, 11 }, new() { 42, 11 }, new() { 43, 11 }, new() { 44, 11 }, new() { 45, 11 }, new() { 46, 11 }, new() { 47, 11 }, new() { 48, 11 }, new() { 49, 11 }, new() { 50, 11 }, new() { 51, 11 }, new() { 52, 11 }, new() { 53, 11 }, new() { 54, 11 }, new() { 55, 11 }, },
                [10] = new() { new() { 0, 11 }, new() { 1, 11 }, new() { 2, 11 }, new() { 3, 11 }, new() { 4, 11 }, new() { 5, 11 }, new() { 6, 11 }, new() { 7, 11 }, new() { 8, 11 }, new() { 9, 11 }, new() { 10, 11 }, new() { 11, 11 }, new() { 12, 11 }, new() { 13, 11 }, new() { 14, 11 }, new() { 15, 11 }, new() { 16, 11 }, new() { 17, 10 }, new() { 18, 79 }, new() { 37, 163 }, new() { 38, 94 }, new() { 39, 11 }, new() { 40, 11 }, new() { 41, 11 }, new() { 42, 11 }, new() { 43, 11 }, new() { 44, 11 }, new() { 45, 11 }, new() { 46, 11 }, new() { 47, 11 }, new() { 48, 11 }, new() { 49, 11 }, new() { 50, 11 }, new() { 51, 11 }, new() { 52, 11 }, new() { 53, 11 }, new() { 54, 11 }, new() { 55, 11 }, },
                [11] = new() { new() { 0, 11 }, new() { 1, 11 }, new() { 2, 11 }, new() { 3, 11 }, new() { 4, 11 }, new() { 5, 11 }, new() { 6, 11 }, new() { 7, 11 }, new() { 8, 11 }, new() { 9, 11 }, new() { 10, 11 }, new() { 11, 11 }, new() { 12, 11 }, new() { 13, 11 }, new() { 14, 11 }, new() { 15, 11 }, new() { 16, 10 }, new() { 17, 79 }, new() { 38, 163 }, new() { 39, 94 }, new() { 40, 11 }, new() { 41, 11 }, new() { 42, 11 }, new() { 43, 11 }, new() { 44, 11 }, new() { 45, 11 }, new() { 46, 11 }, new() { 47, 11 }, new() { 48, 11 }, new() { 49, 11 }, new() { 50, 11 }, new() { 51, 11 }, new() { 52, 11 }, new() { 53, 11 }, new() { 54, 11 }, new() { 55, 11 }, },
                [12] = new() { new() { 0, 11 }, new() { 1, 11 }, new() { 2, 11 }, new() { 3, 11 }, new() { 4, 11 }, new() { 5, 11 }, new() { 6, 11 }, new() { 7, 11 }, new() { 8, 11 }, new() { 9, 11 }, new() { 10, 11 }, new() { 11, 11 }, new() { 12, 11 }, new() { 13, 11 }, new() { 14, 11 }, new() { 15, 10 }, new() { 16, 79 }, new() { 39, 163 }, new() { 40, 94 }, new() { 41, 11 }, new() { 42, 11 }, new() { 43, 11 }, new() { 44, 11 }, new() { 45, 11 }, new() { 46, 11 }, new() { 47, 11 }, new() { 48, 11 }, new() { 49, 11 }, new() { 50, 11 }, new() { 51, 11 }, new() { 52, 11 }, new() { 53, 11 }, new() { 54, 11 }, new() { 55, 11 }, },
                [13] = new() { new() { 0, 11 }, new() { 1, 11 }, new() { 2, 11 }, new() { 3, 11 }, new() { 4, 11 }, new() { 5, 11 }, new() { 6, 11 }, new() { 7, 11 }, new() { 8, 11 }, new() { 9, 11 }, new() { 10, 11 }, new() { 11, 11 }, new() { 12, 11 }, new() { 13, 11 }, new() { 14, 10 }, new() { 15, 79 }, new() { 40, 163 }, new() { 41, 94 }, new() { 42, 11 }, new() { 43, 11 }, new() { 44, 11 }, new() { 45, 11 }, new() { 46, 11 }, new() { 47, 11 }, new() { 48, 11 }, new() { 49, 11 }, new() { 50, 11 }, new() { 51, 11 }, new() { 52, 11 }, new() { 53, 11 }, new() { 54, 11 }, new() { 55, 11 }, },
                [14] = new() { new() { 0, 11 }, new() { 1, 11 }, new() { 2, 11 }, new() { 3, 11 }, new() { 4, 11 }, new() { 5, 11 }, new() { 6, 11 }, new() { 7, 11 }, new() { 8, 11 }, new() { 9, 11 }, new() { 10, 11 }, new() { 11, 11 }, new() { 12, 11 }, new() { 13, 24 }, new() { 14, 79 }, new() { 41, 163 }, new() { 42, 108 }, new() { 43, 11 }, new() { 44, 11 }, new() { 45, 11 }, new() { 46, 11 }, new() { 47, 11 }, new() { 48, 11 }, new() { 49, 11 }, new() { 50, 11 }, new() { 51, 11 }, new() { 52, 11 }, new() { 53, 11 }, new() { 54, 11 }, new() { 55, 11 }, },
                [15] = new() { new() { 0, 11 }, new() { 1, 11 }, new() { 2, 11 }, new() { 3, 11 }, new() { 4, 11 }, new() { 5, 11 }, new() { 6, 11 }, new() { 7, 11 }, new() { 8, 11 }, new() { 9, 11 }, new() { 10, 11 }, new() { 11, 11 }, new() { 12, 11 }, new() { 13, 38 }, new() { 42, 122 }, new() { 43, 11 }, new() { 44, 11 }, new() { 45, 11 }, new() { 46, 11 }, new() { 47, 11 }, new() { 48, 11 }, new() { 49, 11 }, new() { 50, 11 }, new() { 51, 11 }, new() { 52, 11 }, new() { 53, 11 }, new() { 54, 11 }, new() { 55, 11 }, },
                [16] = new() { new() { 0, 11 }, new() { 1, 11 }, new() { 2, 11 }, new() { 3, 11 }, new() { 4, 11 }, new() { 5, 11 }, new() { 6, 11 }, new() { 7, 11 }, new() { 8, 11 }, new() { 9, 11 }, new() { 10, 11 }, new() { 11, 11 }, new() { 12, 11 }, new() { 13, 52 }, new() { 42, 136 }, new() { 43, 11 }, new() { 44, 11 }, new() { 45, 11 }, new() { 46, 11 }, new() { 47, 11 }, new() { 48, 11 }, new() { 49, 11 }, new() { 50, 11 }, new() { 51, 11 }, new() { 52, 11 }, new() { 53, 11 }, new() { 54, 11 }, new() { 55, 11 }, },
                [17] = new() { new() { 0, 11 }, new() { 1, 11 }, new() { 2, 11 }, new() { 3, 11 }, new() { 4, 11 }, new() { 5, 11 }, new() { 6, 11 }, new() { 7, 11 }, new() { 8, 11 }, new() { 9, 11 }, new() { 10, 11 }, new() { 11, 11 }, new() { 12, 11 }, new() { 13, 38 }, new() { 42, 122 }, new() { 43, 11 }, new() { 44, 11 }, new() { 45, 11 }, new() { 46, 11 }, new() { 47, 11 }, new() { 48, 11 }, new() { 49, 11 }, new() { 50, 11 }, new() { 51, 11 }, new() { 52, 11 }, new() { 53, 11 }, new() { 54, 11 }, new() { 55, 11 }, },
                [18] = new() { new() { 0, 11 }, new() { 1, 11 }, new() { 2, 11 }, new() { 3, 11 }, new() { 4, 11 }, new() { 5, 11 }, new() { 6, 11 }, new() { 7, 11 }, new() { 8, 11 }, new() { 9, 11 }, new() { 10, 11 }, new() { 11, 11 }, new() { 12, 11 }, new() { 13, 52 }, new() { 42, 136 }, new() { 43, 11 }, new() { 44, 11 }, new() { 45, 11 }, new() { 46, 11 }, new() { 47, 11 }, new() { 48, 11 }, new() { 49, 11 }, new() { 50, 11 }, new() { 51, 11 }, new() { 52, 11 }, new() { 53, 11 }, new() { 54, 11 }, new() { 55, 11 }, },
                [19] = new() { new() { 0, 11 }, new() { 1, 11 }, new() { 2, 11 }, new() { 3, 11 }, new() { 4, 11 }, new() { 5, 11 }, new() { 6, 11 }, new() { 7, 11 }, new() { 8, 11 }, new() { 9, 11 }, new() { 10, 11 }, new() { 11, 11 }, new() { 12, 11 }, new() { 13, 38 }, new() { 42, 122 }, new() { 43, 11 }, new() { 44, 11 }, new() { 45, 11 }, new() { 46, 11 }, new() { 47, 11 }, new() { 48, 11 }, new() { 49, 11 }, new() { 50, 11 }, new() { 51, 11 }, new() { 52, 11 }, new() { 53, 11 }, new() { 54, 11 }, new() { 55, 11 }, },
                [20] = new() { new() { 0, 11 }, new() { 1, 11 }, new() { 2, 11 }, new() { 3, 11 }, new() { 4, 11 }, new() { 5, 11 }, new() { 6, 11 }, new() { 7, 11 }, new() { 8, 11 }, new() { 9, 11 }, new() { 10, 11 }, new() { 11, 11 }, new() { 12, 11 }, new() { 13, 52 }, new() { 42, 136 }, new() { 43, 11 }, new() { 44, 11 }, new() { 45, 11 }, new() { 46, 11 }, new() { 47, 11 }, new() { 48, 11 }, new() { 49, 11 }, new() { 50, 11 }, new() { 51, 11 }, new() { 52, 11 }, new() { 53, 11 }, new() { 54, 11 }, new() { 55, 11 }, },
                [21] = new() { new() { 0, 11 }, new() { 1, 11 }, new() { 2, 11 }, new() { 3, 11 }, new() { 4, 11 }, new() { 5, 11 }, new() { 6, 11 }, new() { 7, 11 }, new() { 8, 11 }, new() { 9, 11 }, new() { 10, 11 }, new() { 11, 11 }, new() { 12, 11 }, new() { 13, 38 }, new() { 42, 122 }, new() { 43, 11 }, new() { 44, 11 }, new() { 45, 11 }, new() { 46, 11 }, new() { 47, 11 }, new() { 48, 11 }, new() { 49, 11 }, new() { 50, 11 }, new() { 51, 11 }, new() { 52, 11 }, new() { 53, 11 }, new() { 54, 11 }, new() { 55, 11 }, },
                [22] = new() { new() { 0, 11 }, new() { 1, 11 }, new() { 2, 11 }, new() { 3, 11 }, new() { 4, 11 }, new() { 5, 11 }, new() { 6, 11 }, new() { 7, 11 }, new() { 8, 11 }, new() { 9, 11 }, new() { 10, 11 }, new() { 11, 11 }, new() { 12, 11 }, new() { 13, 36 }, new() { 14, 50 }, new() { 41, 134 }, new() { 42, 120 }, new() { 43, 11 }, new() { 44, 11 }, new() { 45, 11 }, new() { 46, 11 }, new() { 47, 11 }, new() { 48, 11 }, new() { 49, 11 }, new() { 50, 11 }, new() { 51, 11 }, new() { 52, 11 }, new() { 53, 11 }, new() { 54, 11 }, new() { 55, 11 }, },
                [23] = new() { new() { 0, 11 }, new() { 1, 11 }, new() { 2, 11 }, new() { 3, 11 }, new() { 4, 11 }, new() { 5, 11 }, new() { 6, 11 }, new() { 7, 11 }, new() { 8, 11 }, new() { 9, 11 }, new() { 10, 11 }, new() { 11, 11 }, new() { 12, 11 }, new() { 13, 11 }, new() { 14, 66 }, new() { 15, 50 }, new() { 40, 134 }, new() { 41, 150 }, new() { 42, 11 }, new() { 43, 11 }, new() { 44, 11 }, new() { 45, 11 }, new() { 46, 11 }, new() { 47, 11 }, new() { 48, 11 }, new() { 49, 11 }, new() { 50, 11 }, new() { 51, 11 }, new() { 52, 11 }, new() { 53, 11 }, new() { 54, 11 }, new() { 55, 11 }, },
                [24] = new() { new() { 0, 11 }, new() { 1, 11 }, new() { 2, 11 }, new() { 3, 11 }, new() { 4, 11 }, new() { 5, 11 }, new() { 6, 11 }, new() { 7, 11 }, new() { 8, 11 }, new() { 9, 11 }, new() { 10, 11 }, new() { 11, 11 }, new() { 12, 11 }, new() { 13, 11 }, new() { 14, 11 }, new() { 15, 66 }, new() { 16, 50 }, new() { 39, 134 }, new() { 40, 150 }, new() { 41, 11 }, new() { 42, 11 }, new() { 43, 11 }, new() { 44, 11 }, new() { 45, 11 }, new() { 46, 11 }, new() { 47, 11 }, new() { 48, 11 }, new() { 49, 11 }, new() { 50, 11 }, new() { 51, 11 }, new() { 52, 11 }, new() { 53, 11 }, new() { 54, 11 }, new() { 55, 11 }, },
                [25] = new() { new() { 0, 11 }, new() { 1, 11 }, new() { 2, 11 }, new() { 3, 11 }, new() { 4, 11 }, new() { 5, 11 }, new() { 6, 11 }, new() { 7, 11 }, new() { 8, 11 }, new() { 9, 11 }, new() { 10, 11 }, new() { 11, 11 }, new() { 12, 11 }, new() { 13, 11 }, new() { 14, 11 }, new() { 15, 11 }, new() { 16, 66 }, new() { 17, 50 }, new() { 38, 134 }, new() { 39, 150 }, new() { 40, 11 }, new() { 41, 11 }, new() { 42, 11 }, new() { 43, 11 }, new() { 44, 11 }, new() { 45, 11 }, new() { 46, 11 }, new() { 47, 11 }, new() { 48, 11 }, new() { 49, 11 }, new() { 50, 11 }, new() { 51, 11 }, new() { 52, 11 }, new() { 53, 11 }, new() { 54, 11 }, new() { 55, 11 }, },
                [26] = new() { new() { 0, 11 }, new() { 1, 11 }, new() { 2, 11 }, new() { 3, 11 }, new() { 4, 11 }, new() { 5, 11 }, new() { 6, 11 }, new() { 7, 11 }, new() { 8, 11 }, new() { 9, 11 }, new() { 10, 11 }, new() { 11, 11 }, new() { 12, 11 }, new() { 13, 11 }, new() { 14, 11 }, new() { 15, 11 }, new() { 16, 11 }, new() { 17, 66 }, new() { 18, 50 }, new() { 37, 134 }, new() { 38, 150 }, new() { 39, 11 }, new() { 40, 11 }, new() { 41, 11 }, new() { 42, 11 }, new() { 43, 11 }, new() { 44, 11 }, new() { 45, 11 }, new() { 46, 11 }, new() { 47, 11 }, new() { 48, 11 }, new() { 49, 11 }, new() { 50, 11 }, new() { 51, 11 }, new() { 52, 11 }, new() { 53, 11 }, new() { 54, 11 }, new() { 55, 11 }, },
                [27] = new() { new() { 0, 11 }, new() { 1, 11 }, new() { 2, 11 }, new() { 3, 11 }, new() { 4, 11 }, new() { 5, 11 }, new() { 6, 11 }, new() { 7, 11 }, new() { 8, 11 }, new() { 9, 11 }, new() { 10, 11 }, new() { 11, 11 }, new() { 12, 11 }, new() { 13, 11 }, new() { 14, 11 }, new() { 15, 11 }, new() { 16, 11 }, new() { 17, 11 }, new() { 18, 66 }, new() { 19, 50 }, new() { 36, 134 }, new() { 37, 150 }, new() { 38, 11 }, new() { 39, 11 }, new() { 40, 11 }, new() { 41, 11 }, new() { 42, 11 }, new() { 43, 11 }, new() { 44, 11 }, new() { 45, 11 }, new() { 46, 11 }, new() { 47, 11 }, new() { 48, 11 }, new() { 49, 11 }, new() { 50, 11 }, new() { 51, 11 }, new() { 52, 11 }, new() { 53, 11 }, new() { 54, 11 }, new() { 55, 11 }, },
                [28] = new() { new() { 0, 11 }, new() { 1, 11 }, new() { 2, 11 }, new() { 3, 11 }, new() { 4, 11 }, new() { 5, 11 }, new() { 6, 11 }, new() { 7, 11 }, new() { 8, 11 }, new() { 9, 11 }, new() { 10, 11 }, new() { 11, 11 }, new() { 12, 11 }, new() { 13, 11 }, new() { 14, 11 }, new() { 15, 11 }, new() { 16, 11 }, new() { 17, 11 }, new() { 18, 11 }, new() { 19, 66 }, new() { 20, 50 }, new() { 35, 134 }, new() { 36, 150 }, new() { 37, 11 }, new() { 38, 11 }, new() { 39, 11 }, new() { 40, 11 }, new() { 41, 11 }, new() { 42, 11 }, new() { 43, 11 }, new() { 44, 11 }, new() { 45, 11 }, new() { 46, 11 }, new() { 47, 11 }, new() { 48, 11 }, new() { 49, 11 }, new() { 50, 11 }, new() { 51, 11 }, new() { 52, 11 }, new() { 53, 11 }, new() { 54, 11 }, new() { 55, 11 }, },
                [29] = new() { new() { 0, 11 }, new() { 1, 11 }, new() { 2, 11 }, new() { 3, 11 }, new() { 4, 11 }, new() { 5, 11 }, new() { 6, 11 }, new() { 7, 11 }, new() { 8, 11 }, new() { 9, 11 }, new() { 10, 11 }, new() { 11, 11 }, new() { 12, 11 }, new() { 13, 11 }, new() { 14, 11 }, new() { 15, 11 }, new() { 16, 11 }, new() { 17, 11 }, new() { 18, 11 }, new() { 19, 11 }, new() { 20, 64 }, new() { 21, 78 }, new() { 22, 23 }, new() { 23, 78 }, new() { 24, 23 }, new() { 25, 37 }, new() { 30, 121 }, new() { 31, 162 }, new() { 32, 107 }, new() { 33, 162 }, new() { 34, 107 }, new() { 35, 148 }, new() { 36, 11 }, new() { 37, 11 }, new() { 38, 11 }, new() { 39, 11 }, new() { 40, 11 }, new() { 41, 11 }, new() { 42, 11 }, new() { 43, 11 }, new() { 44, 11 }, new() { 45, 11 }, new() { 46, 11 }, new() { 47, 11 }, new() { 48, 11 }, new() { 49, 11 }, new() { 50, 11 }, new() { 51, 11 }, new() { 52, 11 }, new() { 53, 11 }, new() { 54, 11 }, new() { 55, 11 }, },
                [30] = new() { new() { 0, 11 }, new() { 1, 11 }, new() { 2, 11 }, new() { 3, 11 }, new() { 4, 11 }, new() { 5, 11 }, new() { 6, 11 }, new() { 7, 11 }, new() { 8, 11 }, new() { 9, 11 }, new() { 10, 11 }, new() { 11, 11 }, new() { 12, 11 }, new() { 13, 11 }, new() { 14, 11 }, new() { 15, 11 }, new() { 16, 11 }, new() { 17, 11 }, new() { 18, 11 }, new() { 19, 11 }, new() { 20, 11 }, new() { 21, 11 }, new() { 22, 11 }, new() { 23, 11 }, new() { 24, 11 }, new() { 25, 38 }, new() { 30, 122 }, new() { 31, 11 }, new() { 32, 11 }, new() { 33, 11 }, new() { 34, 11 }, new() { 35, 11 }, new() { 36, 11 }, new() { 37, 11 }, new() { 38, 11 }, new() { 39, 11 }, new() { 40, 11 }, new() { 41, 11 }, new() { 42, 11 }, new() { 43, 11 }, new() { 44, 11 }, new() { 45, 11 }, new() { 46, 11 }, new() { 47, 11 }, new() { 48, 11 }, new() { 49, 11 }, new() { 50, 11 }, new() { 51, 11 }, new() { 52, 11 }, new() { 53, 11 }, new() { 54, 11 }, new() { 55, 11 }, },
                [31] = new() { new() { 0, 11 }, new() { 1, 11 }, new() { 2, 11 }, new() { 3, 11 }, new() { 4, 11 }, new() { 5, 11 }, new() { 6, 11 }, new() { 7, 11 }, new() { 8, 11 }, new() { 9, 11 }, new() { 10, 11 }, new() { 11, 11 }, new() { 12, 11 }, new() { 13, 11 }, new() { 14, 11 }, new() { 15, 11 }, new() { 16, 11 }, new() { 17, 11 }, new() { 18, 11 }, new() { 19, 11 }, new() { 20, 11 }, new() { 21, 11 }, new() { 22, 11 }, new() { 23, 11 }, new() { 24, 11 }, new() { 25, 52 }, new() { 30, 136 }, new() { 31, 11 }, new() { 32, 11 }, new() { 33, 11 }, new() { 34, 11 }, new() { 35, 11 }, new() { 36, 11 }, new() { 37, 11 }, new() { 38, 11 }, new() { 39, 11 }, new() { 40, 11 }, new() { 41, 11 }, new() { 42, 11 }, new() { 43, 11 }, new() { 44, 11 }, new() { 45, 11 }, new() { 46, 11 }, new() { 47, 11 }, new() { 48, 11 }, new() { 49, 11 }, new() { 50, 11 }, new() { 51, 11 }, new() { 52, 11 }, new() { 53, 11 }, new() { 54, 11 }, new() { 55, 11 }, },
                [32] = new() { new() { 0, 11 }, new() { 1, 11 }, new() { 2, 11 }, new() { 3, 11 }, new() { 4, 11 }, new() { 5, 11 }, new() { 6, 11 }, new() { 7, 11 }, new() { 8, 11 }, new() { 9, 11 }, new() { 10, 11 }, new() { 11, 11 }, new() { 12, 11 }, new() { 13, 11 }, new() { 14, 11 }, new() { 15, 11 }, new() { 16, 11 }, new() { 17, 11 }, new() { 18, 11 }, new() { 19, 11 }, new() { 20, 11 }, new() { 21, 11 }, new() { 22, 11 }, new() { 23, 11 }, new() { 24, 11 }, new() { 25, 38 }, new() { 30, 122 }, new() { 31, 11 }, new() { 32, 11 }, new() { 33, 11 }, new() { 34, 11 }, new() { 35, 11 }, new() { 36, 11 }, new() { 37, 11 }, new() { 38, 11 }, new() { 39, 11 }, new() { 40, 11 }, new() { 41, 11 }, new() { 42, 11 }, new() { 43, 11 }, new() { 44, 11 }, new() { 45, 11 }, new() { 46, 11 }, new() { 47, 11 }, new() { 48, 11 }, new() { 49, 11 }, new() { 50, 11 }, new() { 51, 11 }, new() { 52, 11 }, new() { 53, 11 }, new() { 54, 11 }, new() { 55, 11 }, },
                [33] = new() { new() { 0, 11 }, new() { 1, 11 }, new() { 2, 11 }, new() { 3, 11 }, new() { 4, 11 }, new() { 5, 11 }, new() { 6, 11 }, new() { 7, 11 }, new() { 8, 11 }, new() { 9, 11 }, new() { 10, 11 }, new() { 11, 11 }, new() { 12, 11 }, new() { 13, 11 }, new() { 14, 11 }, new() { 15, 11 }, new() { 16, 11 }, new() { 17, 11 }, new() { 18, 11 }, new() { 19, 11 }, new() { 20, 11 }, new() { 21, 11 }, new() { 22, 11 }, new() { 23, 11 }, new() { 24, 11 }, new() { 25, 52 }, new() { 30, 136 }, new() { 31, 11 }, new() { 32, 11 }, new() { 33, 11 }, new() { 34, 11 }, new() { 35, 11 }, new() { 36, 11 }, new() { 37, 11 }, new() { 38, 11 }, new() { 39, 11 }, new() { 40, 11 }, new() { 41, 11 }, new() { 42, 11 }, new() { 43, 11 }, new() { 44, 11 }, new() { 45, 11 }, new() { 46, 11 }, new() { 47, 11 }, new() { 48, 11 }, new() { 49, 11 }, new() { 50, 11 }, new() { 51, 11 }, new() { 52, 11 }, new() { 53, 11 }, new() { 54, 11 }, new() { 55, 11 }, },

            };

            foreach (KeyValuePair<int, List<List<int>>> code in codes)
            {

                foreach (List<int> array in code.Value)
                {

                    front.Tiles[array[0], code.Key] = new StaticTile(back, chapelsheet, BlendMode.Alpha, array[1]);

                }

            }

            Dictionary<int,Vector2> codeVectors = new()
            {
                [101] = new(12,4),

                [102] = new(13,4),

                [103] = new(0, 3),

                [104] = new(1, 3),

                [105] = new(0, 9),

                [106] = new(1, 9),

                [201] = new(12, 3),

                [202] = new(13, 3),

                [203] = new(0, 2),

                [204] = new(1, 2),

                [205] = new(0, 8),

                [206] = new(1, 8),

                [301] = new(12, 2),

                [302] = new(13, 2),

                [401] = new(12, 1),

                [402] = new(13, 1),

                [501] = new(12, 0),

                [502] = new(13, 0),

            };

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
                [8] = new() { },
                [9] = new() { },
                [10] = new() { },
                [11] = new() { },
                [12] = new() { new() { 20, 501 }, new() { 21, 502 }, new() { 34, 501 }, new() { 35, 502 }, },
                [13] = new() { new() { 20, 401 }, new() { 21, 402 }, new() { 34, 401 }, new() { 35, 402 }, },
                [14] = new() { new() { 20, 301 }, new() { 21, 302 }, new() { 26, 203 }, new() { 27, 204 }, new() { 28, 205 }, new() { 29, 206 }, new() { 34, 301 }, new() { 35, 302 }, },
                [15] = new() { new() { 20, 201 }, new() { 21, 202 }, new() { 26, 103 }, new() { 27, 104 }, new() { 28, 105 }, new() { 29, 106 }, new() { 34, 201 }, new() { 35, 202 }, },
                [16] = new() { new() { 20, 101 }, new() { 21, 102 }, new() { 34, 101 }, new() { 35, 102 }, },
                [17] = new() { },
                [18] = new() { new() { 18, 501 }, new() { 19, 502 }, new() { 36, 501 }, new() { 37, 502 }, },
                [19] = new() { new() { 18, 401 }, new() { 19, 402 }, new() { 36, 401 }, new() { 37, 402 }, },
                [20] = new() { new() { 18, 301 }, new() { 19, 302 }, new() { 36, 301 }, new() { 37, 302 }, },
                [21] = new() { new() { 18, 201 }, new() { 19, 202 }, new() { 36, 201 }, new() { 37, 202 }, },
                [22] = new() { new() { 18, 101 }, new() { 19, 102 }, new() { 22, 501 }, new() { 23, 502 }, new() { 32, 501 }, new() { 33, 502 }, new() { 36, 101 }, new() { 37, 102 }, },
                [23] = new() { new() { 22, 401 }, new() { 23, 402 }, new() { 32, 401 }, new() { 33, 402 }, },
                [24] = new() { new() { 22, 301 }, new() { 23, 302 }, new() { 32, 301 }, new() { 33, 302 }, },
                [25] = new() { new() { 22, 201 }, new() { 23, 202 }, new() { 32, 201 }, new() { 33, 202 }, },
                [26] = new() { new() { 22, 101 }, new() { 23, 102 }, new() { 32, 101 }, new() { 33, 102 }, },
                [27] = new() { },
                [28] = new() { },
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
                    
                    int offset = (int)(array[1] / 100);
                    
                    LocationTile lTile = new(array[0], code.Key, (int)codeVectors[array[1]].X, (int)codeVectors[array[1]].Y, offset, offset == 1, IconData.tilesheets.chapel);

                    if(offset == 1)
                    {

                        buildings.Tiles[array[0], code.Key] = new StaticTile(back, chapelsheet, BlendMode.Alpha, back.Tiles[array[0], code.Key].TileIndex);

                    }

                    locationTiles.Add(lTile);

                }

            }

            lightFields = new()
            {

                [new Vector2(21, 14) * 64 + new Vector2(0, 32)] = 3,
                [new Vector2(26, 14) * 64 + new Vector2(32, 32)] = 2,
                [new Vector2(29, 14) * 64 + new Vector2(32, 32)] = 2,
                [new Vector2(35, 14) * 64 + new Vector2(0, 32)] = 3,
                [new Vector2(19, 20) * 64 + new Vector2(0, 32)] = 3,
                [new Vector2(37, 20) * 64 + new Vector2(0, 32)] = 3,
                [new Vector2(23, 24) * 64 + new Vector2(0, 32)] = 3,
                [new Vector2(33, 24) * 64 + new Vector2(0, 32)] = 3,

            };

            darkFields = new()
            {

                [new Vector2(25, 21) * 64 + new Vector2(0, 32)] = 4,
                [new Vector2(30, 21) * 64 + new Vector2(0, 32)] = 4,
                [new Vector2(27, 32) * 64 + new Vector2(0, 32)] = 5,
                [new Vector2(15, 15) * 64 + new Vector2(0, 32)] = 5,
                [new Vector2(12, 21) * 64 + new Vector2(0, 32)] = 5,
                [new Vector2(15, 26) * 64 + new Vector2(0, 32)] = 5,
                [new Vector2(39, 15) * 64 + new Vector2(0, 32)] = 5,
                [new Vector2(42, 21) * 64 + new Vector2(0, 32)] = 5,
                [new Vector2(39, 26) * 64 + new Vector2(0, 32)] = 5,

            };

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

            warpSets.Add(new WarpTile(26, 32, "Mine", 17, 5));

            warpSets.Add(new WarpTile(27, 32, "Mine", 17, 5));

            warpSets.Add(new WarpTile(28, 32, "Mine", 17, 5));

            warpSets.Add(new WarpTile(29, 32, "Mine", 17, 5));

            warps.Add(new Warp(26, 32, "Mine", 17, 6, flipFarmer: false));

            warps.Add(new Warp(27, 32, "Mine", 17, 6, flipFarmer: false));

            warps.Add(new Warp(28, 32, "Mine", 17, 6, flipFarmer: false));

            warps.Add(new Warp(29, 32, "Mine", 17, 6, flipFarmer: false));

        }

    }

}
