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

namespace SubterranianOverhaul
{
    class VoidshroomSpore : StardewValley.Object
    {
        private static Texture2D texture;


        public Texture2D Texture
        {
            get {
                if(VoidshroomSpore.texture == null)
                {
                    loadTexture();
                }
                return VoidshroomSpore.texture;
            }
        }

        public VoidshroomSpore() : base()
        {
            this.Name = "VoidshroomSpore";
            this.DisplayName = "Voidshroom Spore";
            this.Quality = 0;
            this.Price = 10;
            this.Category = 74;
            loadTexture();
        }

        public VoidshroomSpore(Vector2 tileLocation) : this()
        {
            this.TileLocation = tileLocation;
        }

        public override bool placementAction(GameLocation location, int x, int y, Farmer who = null)
        {
            //First verify we're in a suitable location for plant (the mine or the cave)
            if(location.Name.Equals("Mine") || location.Name.Contains("Cave"))
            {
                Vector2 index1 = new Vector2((float)(x / 64), (float)(y / 64));

                bool flag = location.terrainFeatures.ContainsKey(index1) && location.terrainFeatures[index1] is HoeDirt && (location.terrainFeatures[index1] as HoeDirt).crop == null;
                string str = location.doesTileHaveProperty((int)index1.X, (int)index1.Y, "NoSpawn", "Back");
                if (!flag && (location.objects.ContainsKey(index1) || location.terrainFeatures.ContainsKey(index1) || !(location is Farm) && !location.IsGreenhouse || str != null && (str.Equals("Tree") || str.Equals("All") || str.Equals("True"))))
                {
                    Game1.showRedMessage(Game1.content.LoadString("Strings\\Voidshroom:CantPlantHere"));
                    return false;
                }
                if (str != null && (str.Equals("Tree") || str.Equals("All") || str.Equals("True")))
                    return false;
                if (flag || location.isTileLocationOpen(new Location(x * 64, y * 64)) && !location.isTileOccupied(new Vector2((float)x, (float)y), "") && location.doesTileHaveProperty(x, y, "Water", "Back") == null)
                {   
                    location.terrainFeatures.Remove(index1);
                    location.terrainFeatures.Add(index1, (TerrainFeature)new VoidshroomTree(0));
                    location.playSound("dirtyHit");
                    return true;
                }
            } else
            {
                Game1.showRedMessage(Game1.content.LoadString("Strings\\Voidshroom:CantPlantHere"));
                return false;
            }

            return true;
        }

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {
            if (!Game1.eventUp || Game1.CurrentEvent != null && !Game1.CurrentEvent.isTileWalkedOn(x, y))
            {   
                Microsoft.Xna.Framework.Rectangle boundingBox;
                if (this.Fragility != 2)
                {
                    SpriteBatch spriteBatch1 = spriteBatch;
                    Texture2D shadowTexture = Game1.shadowTexture;
                    Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * 64 + 32), (float)(y * 64 + 51 + 4)));
                    Microsoft.Xna.Framework.Rectangle? sourceRectangle = new Microsoft.Xna.Framework.Rectangle?(Game1.shadowTexture.Bounds);
                    Color color = Color.White * alpha;
                    Vector2 origin = new Vector2((float)Game1.shadowTexture.Bounds.Center.X, (float)Game1.shadowTexture.Bounds.Center.Y);
                    boundingBox = this.getBoundingBox(new Vector2((float)x, (float)y));
                    double num = (double)boundingBox.Bottom / 15000.0;
                    spriteBatch1.Draw(shadowTexture, local, sourceRectangle, color, 0.0f, origin, 4f, SpriteEffects.None, (float)num);
                }
                SpriteBatch spriteBatch2 = spriteBatch;
                Microsoft.Xna.Framework.Rectangle? sourceRectangle1 = new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, 16, 16));
                Vector2 local1 = Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * 64 + 32 + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0)), (float)(y * 64 + 32 + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0))));
                Color color1 = Color.White * alpha;
                Vector2 origin1 = new Vector2(8f, 8f);
                Vector2 scale1 = this.scale;
                int num6;
                if (!this.isPassable())
                {
                    boundingBox = this.getBoundingBox(new Vector2((float)x, (float)y));
                    num6 = boundingBox.Bottom;
                }
                else
                {
                    boundingBox = this.getBoundingBox(new Vector2((float)x, (float)y));
                    num6 = boundingBox.Top;
                }
                double num7 = (double)num6 / 10000.0;
                //spriteBatch2.Draw(objectSpriteSheet, local1, sourceRectangle1, color1, 0.0f, origin1, (float)num4, (SpriteEffects)num5, (float)num7);
                spriteBatch2.Draw(this.Texture, local1, sourceRectangle1, color1,0.0f,origin1,0f,(SpriteEffects)0,(float)num7);
            }
        }

        public override void draw(
          SpriteBatch spriteBatch,
          int xNonTile,
          int yNonTile,
          float layerDepth,
          float alpha = 1f)
        {
            if (Game1.eventUp && Game1.CurrentEvent.isTileWalkedOn(xNonTile / 64, yNonTile / 64))
                return;
            if (this.Fragility != 2)
            spriteBatch.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(xNonTile + 32), (float)(yNonTile + 51 + 4))), new Microsoft.Xna.Framework.Rectangle?(Game1.shadowTexture.Bounds), Color.White * alpha, 0.0f, new Vector2((float)Game1.shadowTexture.Bounds.Center.X, (float)Game1.shadowTexture.Bounds.Center.Y), 4f, SpriteEffects.None, layerDepth - 1E-06f);
            SpriteBatch spriteBatch1 = spriteBatch;
            Texture2D objectSpriteSheet = Game1.objectSpriteSheet;
            Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(xNonTile + 32 + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0)), (float)(yNonTile + 32 + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0))));
            Microsoft.Xna.Framework.Rectangle sRect = new Microsoft.Xna.Framework.Rectangle(0, 0, 16, 16);
            Microsoft.Xna.Framework.Rectangle? sourceRectangle = new Microsoft.Xna.Framework.Rectangle?(sRect);
            Color color = Color.White * alpha;
            Vector2 origin = new Vector2(8f, 8f);
            Vector2 scale = this.scale;
            double num1 = (double)this.scale.Y > 1.0 ? (double)this.getScale().Y : 4.0;
            int num2 = this.Flipped ? 1 : 0;
            double num3 = (double)layerDepth;
            spriteBatch1.Draw(this.Texture, local, sourceRectangle, color, 0.0f, origin, (float)num1, (SpriteEffects)num2, (float)num3);
            
        }

        public override bool performToolAction(Tool t, GameLocation location)
        {
            return base.performToolAction(t, location);
        }

        private static void loadTexture()
        {
            VoidshroomSpore.texture = VoidshroomSpore.texture ?? ModEntry.GetHelper().Content.Load<Texture2D>(Path.Combine("assets", "voidshroom_spore.png"), ContentSource.ModFolder);
        }
    }
}
