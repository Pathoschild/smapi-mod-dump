using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardustCore.UIUtilities;

namespace StardustCore.Objects
{
    public class MultiTileComponent : CoreObject
    {
        //Pass in different function pointers that return bool to check if this default code will run. If not 
        public MultiTileObject containerObject;
        
        public MultiTileComponent()
        {
            //this.TextureSheet = new Texture2DExtended();
            this.NetFields.AddField(new NetCode.NetTexture2DExtended(this.getExtendedTexture()));
        }

        public MultiTileComponent(CoreObject part)
        {
            this.name = part.name;
            this.description = part.description;
            this.TextureSheet = part.getExtendedTexture();
            if (part.animationManager != null)
            {
                this.animationManager = part.animationManager;
            }
            this.defaultBoundingBox = new Rectangle(0, 0, 16, 16);
            this.boundingBox.Value = new Rectangle((int)0 * Game1.tileSize, (int)0* Game1.tileSize, 1 * Game1.tileSize, 1 * Game1.tileSize);

            
            this.NetFields.AddField(new NetCode.NetTexture2DExtended(this.getExtendedTexture()));

            this.InitializeBasics(0, Vector2.Zero);
        }

        public MultiTileComponent(int which,String name, String description, Texture2DExtended texture)
        {
            this.name = name;
            this.displayName = name;
            this.description = description;
            this.TextureSheet = texture;
            this.defaultBoundingBox = new Rectangle(0, 0, 16, 16);
            this.boundingBox.Value = new Rectangle((int)0 * Game1.tileSize, (int)0 * Game1.tileSize, 1 * Game1.tileSize, 1 * Game1.tileSize);
            this.defaultSourceRect.Width = 16;
            this.defaultSourceRect.Height = 16;
            this.sourceRect = new Rectangle((which * 16) % TextureSheet.getTexture().Width, (which * 16) / TextureSheet.getTexture().Width * 16, this.defaultSourceRect.Width * 1, this.defaultSourceRect.Height * 1);
            this.defaultSourceRect = this.sourceRect;
            this.serializationName = this.GetType().ToString();
            this.ParentSheetIndex = which;

            this.animationManager = new Animations.AnimationManager(texture, new Animations.Animation(this.defaultSourceRect), false);

            this.NetFields.AddField(new NetCode.NetTexture2DExtended(this.getExtendedTexture()));

            this.InitializeBasics(0, Vector2.Zero);
        }

        public MultiTileComponent(int which,String name, String description, Animations.AnimationManager animationManager)
        {
            this.name = name;
            this.displayName = name;
            this.description = description;
            this.animationManager = animationManager;
            this.TextureSheet = animationManager.getExtendedTexture();
            this.defaultBoundingBox = new Rectangle(0, 0, 16, 16);
            this.boundingBox.Value = new Rectangle((int)0 * Game1.tileSize, (int)0 * Game1.tileSize, 1 * Game1.tileSize, 1 * Game1.tileSize);
            this.defaultSourceRect.Width = 16;
            this.defaultSourceRect.Height = 16;
            this.sourceRect = new Rectangle(which * 16 % TextureSheet.getTexture().Width, which * 16 / TextureSheet.getTexture().Width * 16, this.defaultSourceRect.Width * 16, this.defaultSourceRect.Height * 16);
            this.defaultSourceRect = this.sourceRect;
            this.serializationName = this.GetType().ToString();
            this.ParentSheetIndex = which;

            this.NetFields.AddField(new NetCode.NetTexture2DExtended(this.getExtendedTexture()));

            this.InitializeBasics(0,Vector2.Zero);
        }

        public override void InitializeBasics(int InvMaxSize, Vector2 tile)
        {
            this.inventory = new List<Item>();
            this.inventoryMaxSize = InvMaxSize;
            this.TileLocation = tile;
            lightsOn = false;

            lightColor = Color.Black;

            base.initNetFields();
            this.NetFields.AddField(new NetCode.Objects.NetMultiTileComponent(this));
        }

        public override bool clicked(Farmer who)
        {
            //Check if shift click to interact with object.
            containerObject.RemoveAllObjects();
            return true;
        }

        public override Item getOne()
        {
            if (this.animationManager != null)
            {
                var obj = new MultiTileComponent(this.ParentSheetIndex,this.name, this.description, this.animationManager);
                obj.containerObject = this.containerObject;
                return obj;
            }
            else
            {
                var obj = new MultiTileComponent(this.ParentSheetIndex,this.name, this.description, this.TextureSheet);
                obj.containerObject = this.containerObject;
                return obj;
            }
        }

        public override bool RightClicked(Farmer who)
        {
            return true;
        }

        public override bool placementAction(GameLocation location, int x, int y, Farmer who = null)
        {
            Point point = new Point(x / Game1.tileSize, y / Game1.tileSize);
            this.position = new Vector2(point.X, point.Y);
            this.TileLocation = new Vector2((float)point.X, (float)point.Y);
            this.boundingBox.Value = new Rectangle((int)TileLocation.X * Game1.tileSize, (int)TileLocation.Y * Game1.tileSize, 1 * Game1.tileSize, 1 * Game1.tileSize);

            foreach(Farmer farmer in Game1.getAllFarmers())
            {
                if (location == farmer.currentLocation)
                {
                    if (farmer.GetBoundingBox().Intersects(this.boundingBox.Value))
                    {
                        Game1.showRedMessage("Can't place on top of a person.");
                        bool result = false;
                        return result;
                    }
                }
            }


            this.updateDrawPosition();

            bool f = Utilities.placementAction(this, location, x, y,StardustCore.ModCore.SerializationManager ,who);
            this.thisLocation = Game1.player.currentLocation;
            return f;
            //  Game1.showRedMessage("Can only be placed in House");
            //  return false;
        }

        public override void performRemoveAction(Vector2 tileLocation, GameLocation environment)
        {
            // Game1.showRedMessage("Why3?");
            try
            {
                this.heldObject.Value.performRemoveAction(this.TileLocation, this.thisLocation);
                this.heldObject.Value = null;
            }
            catch(Exception err)
            {
                
            }
            StardustCore.ModCore.SerializationManager.trackedObjectList.Remove(this);
            this.thisLocation.removeObject(this.tileLocation, false);
            //this.thisLocation.objects.Remove(this.TileLocation);
            this.thisLocation = null;
            this.locationsName = "";
            base.performRemoveAction(tileLocation, environment);
        }

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {
            if (x == -1)
            {
                spriteBatch.Draw(this.TextureSheet.getTexture(), Game1.GlobalToLocal(Game1.viewport, this.drawPosition), new Rectangle?(this.sourceRect), Color.White * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (this.Decoration_type == 12) ? 0f : ((float)(this.boundingBox.Bottom - 8) / 10000f));
            }
            else
            {
                //The actual planter box being drawn.
                if (animationManager == null)
                {
                    if (this.TextureSheet == null)
                    {
                        ModCore.ModMonitor.Log("Tex Extended is null???");
                       
                    }

                    spriteBatch.Draw(this.TextureSheet.getTexture(), Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * Game1.tileSize), y * Game1.tileSize)), new Rectangle?(this.sourceRect), Color.White * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
                    // Log.AsyncG("ANIMATION IS NULL?!?!?!?!");
                }

                else
                {
                    //Log.AsyncC("Animation Manager is working!");
                    this.animationManager.draw(spriteBatch, this.animationManager.getTexture(), Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * Game1.tileSize), y * Game1.tileSize)), new Rectangle?(this.animationManager.currentAnimation.sourceRectangle), Color.White * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
                    try
                    {
                        this.animationManager.tickAnimation();
                        // Log.AsyncC("Tick animation");
                    }
                    catch (Exception err)
                    {
                        ModCore.ModMonitor.Log(err.ToString());
                    }
                }

                // spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)((double)tileLocation.X * (double)Game1.tileSize + (((double)tileLocation.X * 11.0 + (double)tileLocation.Y * 7.0) % 10.0 - 5.0)) + (float)(Game1.tileSize / 2), (float)((double)tileLocation.Y * (double)Game1.tileSize + (((double)tileLocation.Y * 11.0 + (double)tileLocation.X * 7.0) % 10.0 - 5.0)) + (float)(Game1.tileSize / 2))), new Rectangle?(new Rectangle((int)((double)tileLocation.X * 51.0 + (double)tileLocation.Y * 77.0) % 3 * 16, 128 + this.whichForageCrop * 16, 16, 16)), Color.White, 0.0f, new Vector2(8f, 8f), (float)Game1.pixelZoom, SpriteEffects.None, (float)(((double)tileLocation.Y * (double)Game1.tileSize + (double)(Game1.tileSize / 2) + (((double)tileLocation.Y * 11.0 + (double)tileLocation.X * 7.0) % 10.0 - 5.0)) / 10000.0));



            }
        }

        public override void draw(SpriteBatch spriteBatch, int xNonTile, int yNonTile, float layerDepth, float alpha = 1f)
        {

                if (Game1.eventUp && Game1.CurrentEvent.isTileWalkedOn(xNonTile / 64, yNonTile / 64))
                    return;
                if ((int)(this.ParentSheetIndex) != 590 && (int)(this.Fragility) != 2)
                    spriteBatch.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(xNonTile + 32), (float)(yNonTile + 51 + 4))), new Microsoft.Xna.Framework.Rectangle?(Game1.shadowTexture.Bounds), Color.White * alpha, 0.0f, new Vector2((float)Game1.shadowTexture.Bounds.Center.X, (float)Game1.shadowTexture.Bounds.Center.Y), 4f, SpriteEffects.None, layerDepth - 1E-06f);
                SpriteBatch spriteBatch1 = spriteBatch;
                Texture2D objectSpriteSheet = Game1.objectSpriteSheet;
                Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(xNonTile + 32 + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0)), (float)(yNonTile + 32 + (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0))));
                Microsoft.Xna.Framework.Rectangle? sourceRectangle = new Microsoft.Xna.Framework.Rectangle?(GameLocation.getSourceRectForObject(this.ParentSheetIndex));
                Color color = Color.White * alpha;
                double num1 = 0.0;
                Vector2 origin = new Vector2(8f, 8f);
                Vector2 scale = this.scale;
                double num2 = (double)this.scale.Y > 1.0 ? (double)this.getScale().Y : 4.0;
                int num3 = (bool)(this.flipped) ? 1 : 0;
                double num4 = (double)layerDepth;
                spriteBatch1.Draw(TextureSheet.getTexture(), local, sourceRectangle, color, (float)num1, origin, (float)num2, (SpriteEffects)num3, (float)num4);
            
        }



    }
}
