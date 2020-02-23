using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using StardewModdingAPI;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.Dimensions;
using System.IO;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework.Input;
using SubterranianOverhaul.Crops;

namespace SubterranianOverhaul
{
    class CaveCarrotSeed : StardewValley.Object
    {
        public const String NAME = "Ancient Carrot Seed";
        public const String DISPLAY_NAME = "Ancient Carrot Seed";
        public const int QUALITY = 0;
        public const int PRICE = 10;
        public const int CATEGORY = 74;
        public const int EDIBILITY = -300;
        public const string DESCRIPTION = "An ancient carrot seed that can only grow in the dark, moist soil of living caves.";

        private static int itemIndex = -1;

        public static void setIndex()
        {
            if (CaveCarrotSeed.itemIndex == -1)
            {
                CaveCarrotSeed.itemIndex = IndexManager.getUnusedObjectIndex();
            }
        }

        public static int getIndex()
        {
            if(CaveCarrotSeed.itemIndex == -1)
            {
                CaveCarrotSeed.setIndex();
            }

            return CaveCarrotSeed.itemIndex;
        }

        public CaveCarrotSeed(Vector2 tileLocation) : base(tileLocation, itemIndex, false)
        {
            this.Name = VoidshroomSpore.NAME;
            this.DisplayName = VoidshroomSpore.DISPLAY_NAME;
            this.Quality = VoidshroomSpore.QUALITY;
            this.Price = VoidshroomSpore.PRICE;
            this.Category = VoidshroomSpore.CATEGORY;
        }

        public CaveCarrotSeed() : this(Vector2.Zero)
        {
            
        }        

        public static string getObjectData()
        {
            return string.Format("{0}/{1}/{2}/Seeds -{3}/{4}/{5}", CaveCarrotSeed.NAME, CaveCarrotSeed.PRICE, CaveCarrotSeed.EDIBILITY, CaveCarrotSeed.CATEGORY, CaveCarrotSeed.NAME, CaveCarrotSeed.DESCRIPTION);
        }

        public override string getDescription()
        {
            return CaveCarrotSeed.DESCRIPTION;
        }

        public static bool IsValidLocation(GameLocation location)
        {
            return location.Name.Equals("Mine") || location.Name.Contains("Cave");
        }

        internal static bool AttemptPlanting(Vector2 grabTile, GameLocation location, Farmer who = null)
        {
            if (CaveCarrotSeed.canPlaceHere(location, grabTile))
            {   
                try
                {
                    location.terrainFeatures.Remove(grabTile);
                    int X = (int)grabTile.X;
                    int Y = (int)grabTile.Y;
                    int seedIndex = CaveCarrotSeed.getIndex();
                    bool isReallyGreenhouse = location.isGreenhouse.Value;
                    location.isGreenhouse.Value = true;
                    HoeDirt dirtPatch = new HoeDirt(0, location);
                    location.terrainFeatures.Add(grabTile, (TerrainFeature)dirtPatch);
                    bool planted2 = dirtPatch.plant(seedIndex, X, Y, Game1.player, false, location);
                    location.isGreenhouse.Value = isReallyGreenhouse;
                    return true;
                } catch
                {   
                    return false;
                }
                
                
            } else
            {
                if(!IsValidLocation(location))
                {
                    Game1.showRedMessage("This plant would not thrive here.");
                }
                return false;
            }
        }

        public static bool canPlaceHere(GameLocation location, Vector2 tile, bool ignoreValidLocation = false)
        {
            if (ignoreValidLocation || IsValidLocation(location))
            {
                Vector2 index1 = tile;
                bool occupied = location.isTileOccupiedForPlacement(index1);
                bool flag = location.terrainFeatures.ContainsKey(index1) && location.terrainFeatures[index1] is HoeDirt && (location.terrainFeatures[index1] as HoeDirt).crop == null;
                string str = location.doesTileHaveProperty((int)index1.X, (int)index1.Y, "NoSpawn", "Back");
                if (occupied || !flag && (location.objects.ContainsKey(index1) || location.terrainFeatures.ContainsKey(index1) || str != null && (str.Equals("Tree") || str.Equals("All") || str.Equals("True"))))
                {   
                    return false;
                }
                if (str != null && (str.Equals("Tree") || str.Equals("All") || str.Equals("True")))
                    return false;
                if (flag || !location.isTileOccupied(index1, "") && location.doesTileHaveProperty((int)index1.X, (int)index1.Y, "Water", "Back") == null)
                {   
                    return true;
                }
            }

            return false;
        }

        public static void drawPlacementBounds(StardewValley.Object obj, SpriteBatch spriteBatch, GameLocation location)
        {   
            int x = Game1.getOldMouseX() + Game1.viewport.X;
            int y = Game1.getOldMouseY() + Game1.viewport.Y;
            if ((double)Game1.mouseCursorTransparency == 0.0)
            {
                x = (int)Game1.player.GetGrabTile().X * 64;
                y = (int)Game1.player.GetGrabTile().Y * 64;
            }
            if (Game1.player.GetGrabTile().Equals(Game1.player.getTileLocation()) && (double)Game1.mouseCursorTransparency == 0.0)
            {
                Vector2 translatedVector2 = Utility.getTranslatedVector2(Game1.player.GetGrabTile(), Game1.player.FacingDirection, 1f);
                x = (int)translatedVector2.X * 64;
                y = (int)translatedVector2.Y * 64;
            }
            
            bool flag = VoidshroomSpore.canPlaceHere(location, new Vector2(x /64 ,y / 64));

            spriteBatch.Draw(Game1.mouseCursors, new Vector2((float)(x / 64 * 64 - Game1.viewport.X), (float)(y / 64 * 64 - Game1.viewport.Y)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(flag ? 194 : 210, 388, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.01f);
        }
    }
}
