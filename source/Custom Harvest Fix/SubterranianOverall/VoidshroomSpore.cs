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

namespace SubterranianOverhaul
{
    class VoidshroomSpore : StardewValley.Object
    {   
        private static int itemIndex = -1;

        public static void setIndex()
        {
            if (VoidshroomSpore.itemIndex == -1)
            {
                VoidshroomSpore.itemIndex = IndexManager.getUnusedObjectIndex();
            }
        }

        public static int getIndex()
        {
            if(VoidshroomSpore.itemIndex == -1)
            {
                VoidshroomSpore.setIndex();
            }

            return VoidshroomSpore.itemIndex;
        }

        public VoidshroomSpore(Vector2 tileLocation) : base(tileLocation, itemIndex, false)
        {
            this.Name = "Voidshroom Spore";
            this.DisplayName = "Voidshroom Spore";
            this.Quality = 0;
            this.Price = 10;
            this.Category = 74;
        }

        public VoidshroomSpore() : this(Vector2.Zero)
        {
            
        }        

        public string getObjectData()
        {
            return string.Format("{0}/{1}/{2}/Basic {3}/{4}/{5}", this.Name, this.Price, this.Edibility, (int)this.Category, this.Name, this.getDescription());
        }

        public override string getDescription()
        {
            return "The spore of a giant Voidshroom Tree.  These giant fungi only thrive in darkness.";
        }

        public static bool IsValidLocation(GameLocation location)
        {
            return location.Name.Equals("Mine") || location.Name.Contains("Cave");
        }

        internal static bool AttemptPlanting(Vector2 grabTile, GameLocation location, Farmer who = null)
        {
            if (VoidshroomSpore.canPlaceHere(location, grabTile))
            {
                location.terrainFeatures.Remove(grabTile);
                location.terrainFeatures.Add(grabTile, (TerrainFeature)new VoidshroomTree(0));
                location.playSound("dirtyHit");
                return true;
            } else
            {
                if(!IsValidLocation(location))
                {
                    Game1.showRedMessage("These seeds can only thrive in darkness.");
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
