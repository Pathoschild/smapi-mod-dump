
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using StardustCore;
using StardustCore.Animations;
using StardustCore.UIUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace AdditionalCropsFramework
{
    /// <summary>
    ///  Revitalize ModularCropObject Class. This is a core class and should only be extended upon.
    /// </summary>
    /// 
    public class ModularCropObject : CoreObject
    {

        public new bool flipped;

        public string cropType;

        public string dataFileName;

        public override string Name
        {
            get
            {
                return this.name;
            }

        }

        public ModularCropObject()
        {
            this.updateDrawPosition();
        }

        public ModularCropObject(bool f)
        {
            //does nothng
        }

        public ModularCropObject(IModHelper helper,int which, int initalStack, string ObjectTextureSheetName, string DataFileName)
        {
            this.serializationName = this.GetType().ToString();
           // if (File.Exists(ObjectTextureSheetName)) Log.AsyncC("YAY");
            this.TileLocation = Vector2.Zero;
          
            this.Stack = initalStack;
            try
            {
                 this.TextureSheet = new Texture2DExtended(helper, ObjectTextureSheetName);
                //TextureSheet = ModCore.ModHelper.Content.Load<Texture2D>(Path.Combine(Utilities.EntensionsFolderName, ObjectTextureSheetName));  //Game1.content.Load<Texture2D>("TileSheets\\furniture");
            }
            catch(Exception err)
            {
                err.ToString();
              //  Log.AsyncM(err);
            }
                texturePath = ObjectTextureSheetName;
            this.dataFileName = DataFileName;
            this.CanBeSetDown = false;
          //  Log.AsyncG(Path.Combine(Utilities.EntensionsFolderName,DataFileName));
          //  Log.AsyncC(which);
            Dictionary<int, string> dictionary = new Dictionary<int, string>();

        
            try
            {
                dictionary = ModCore.ModHelper.Content.Load<Dictionary<int, string>>(Path.Combine(Utilities.EntensionsFolderName, DataFileName));
            }
            catch(Exception err)
            {
                err.ToString();
               // Log.AsyncC(err);
            }


            string[] array = dictionary[which].Split(new char[]
            {
                '/'
            });
            this.name = array[0];
            this.Price = Convert.ToInt32(array[1]);
            this.Edibility =Convert.ToInt32(array[2]);
            string[] array2 = array[3].Split(' ');
            this.cropType = array2[0];
            this.Category =Convert.ToInt32(array2[1]);
            this.Type = this.cropType;
            this.displayName = this.name;
            
            this.description = array[5];

            string[] dArray = this.description.Split(' ');
            string newDes = "";
            int MaxWords = 7;
            int currentCount = 0;
            foreach (var v in dArray)
            {
                if (currentCount == MaxWords)
                {
                    currentCount = 0;
                    newDes += "\n";
                }
                newDes += v+" ";
                currentCount++;
            }
            this.description = newDes;

            this.defaultSourceRect = new Rectangle(which * 16 % TextureSheet.texture.Width, which * 16 / TextureSheet.texture.Width * 16, 16, 16);
            this.sourceRect = this.defaultSourceRect;
            
     
        
            this.defaultBoundingBox = new Rectangle(0, 0, 16, 16);
            this.boundingBox.Value = this.defaultBoundingBox;

            this.updateDrawPosition();
            this.ParentSheetIndex = which;

            try
            {
                this.animationManager = new StardustCore.Animations.AnimationManager(this.TextureSheet, new StardustCore.Animations.Animation(this.defaultSourceRect, -1), AnimationManager.parseAnimationsFromXNB(array[3]), "Default");
                this.animationManager.setAnimation("Default", 0);
                //Log.AsyncC("Using animation manager");
            }
            catch (Exception errr)
            {
                errr.ToString();
                this.animationManager = new StardustCore.Animations.AnimationManager(this.TextureSheet, new StardustCore.Animations.Animation(this.defaultSourceRect, -1));
            }
        }

        public override string getDescription()
        {
            return this.description;
        }

        /*
        public override bool performDropDownAction(StardewValley.Farmer who)
        {
            this.resetOnPlayerEntry((who == null) ? Game1.currentLocation : who.currentLocation);
            return false;
        }
        */
        public override void hoverAction()
        {
            base.hoverAction();
            if (!Game1.player.isInventoryFull())
            {
                Game1.mouseCursor = 2;
            }
        }

        public override bool canBeGivenAsGift()
        {
            return true;
        }
        
        public override bool canBePlacedHere(GameLocation l, Vector2 tile)
        {
            return false;         
        }

        public override void updateDrawPosition()
        {
            this.drawPosition = new Vector2((float)this.boundingBox.X, (float)(this.boundingBox.Y - (this.sourceRect.Height * Game1.pixelZoom - this.boundingBox.Height)));
        }

        public override int getTilesWide()
        {
            return this.boundingBox.Width / Game1.tileSize;
        }

        public override int getTilesHigh()
        {
            return this.boundingBox.Height / Game1.tileSize;
        }


        public override bool placementAction(GameLocation location, int x, int y, StardewValley.Farmer who = null)
        {
            return false;          
        }

        public override bool isPlaceable()
        {
            return true;
        }

        public override Rectangle getBoundingBox(Vector2 tileLocation)
        {
            return this.boundingBox.Value;
        }

        public override int salePrice()
        {
            return this.Price;
        }

        public override int maximumStackSize()
        {
            return 999;
        }

        public override int getStack()
        {
            return this.Stack;
        }


        private float getScaleSize()
        {
            int num = this.sourceRect.Width / 16;
            int num2 = this.sourceRect.Height / 16;
            if (num >= 5)
            {
                return 0.75f;
            }
            if (num2 >= 3)
            {
                return 1f;
            }
            if (num <= 2)
            {
                return 2f;
            }
            if (num <= 4)
            {
                return 1f;
            }
            return 0.1f;
        }

        public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, StardewValley.Farmer f)
        {
            if (f.ActiveObject.bigCraftable.Value)
            {
                spriteBatch.Draw(Game1.bigCraftableSpriteSheet, objectPosition, new Microsoft.Xna.Framework.Rectangle?(StardewValley.Object.getSourceRectForBigCraftable(f.ActiveObject.ParentSheetIndex)), Color.White, 0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, Math.Max(0f, (float)(f.getStandingY() + 2) / 10000f));
                return;
            }
            spriteBatch.Draw(Game1.objectSpriteSheet, objectPosition, new Microsoft.Xna.Framework.Rectangle?(GameLocation.getSourceRectForObject(f.ActiveObject.ParentSheetIndex)), Color.White, 0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, Math.Max(0f, (float)(f.getStandingY() + 2) / 10000f));
            if (f.ActiveObject != null && f.ActiveObject.Name.Contains("="))
            {
                spriteBatch.Draw(Game1.objectSpriteSheet, objectPosition + new Vector2((float)(Game1.tileSize / 2), (float)(Game1.tileSize / 2)), new Microsoft.Xna.Framework.Rectangle?(GameLocation.getSourceRectForObject(f.ActiveObject.ParentSheetIndex)), Color.White, 0f, new Vector2((float)(Game1.tileSize / 2), (float)(Game1.tileSize / 2)), (float)Game1.pixelZoom + Math.Abs(Game1.starCropShimmerPause) / 8f, SpriteEffects.None, Math.Max(0f, (float)(f.getStandingY() + 2) / 10000f));
                if (Math.Abs(Game1.starCropShimmerPause) <= 0.05f && Game1.random.NextDouble() < 0.97)
                {
                    return;
                }
                Game1.starCropShimmerPause += 0.04f;
                if (Game1.starCropShimmerPause >= 0.8f)
                {
                    Game1.starCropShimmerPause = -0.8f;
                }
            }
            //base.drawWhenHeld(spriteBatch, objectPosition, f);
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber, Color c, bool drawWithShadows)
        {

            //spriteBatch.Draw(TextureSheet, location + new Vector2((float)(Game1.tileSize), (float)(Game1.tileSize)), new Rectangle?(this.sourceRect), Color.White * transparency, 0f, new Vector2((float)(this.sourceRect.Width / 2), (float)(this.sourceRect.Height)), 1f * 3, SpriteEffects.None, layerDepth);
            spriteBatch.Draw(TextureSheet.texture, location + new Vector2((float)(Game1.tileSize / 2), (float)(Game1.tileSize / 2)), new Rectangle?(this.defaultSourceRect), Color.White * transparency, 0f, new Vector2((float)(this.defaultSourceRect.Width / 2), (float)(this.defaultSourceRect.Height / 2)), 1f * this.getScaleSize() * scaleSize * 1.5f, SpriteEffects.None, layerDepth);

            if (drawStackNumber && this.maximumStackSize() > 1 && ((double)scaleSize > 0.3 && this.Stack != int.MaxValue) && this.Stack > 1)
                Utility.drawTinyDigits(this.Stack, spriteBatch, location + new Vector2((float)(Game1.tileSize - Utility.getWidthOfTinyDigitString(this.Stack, 3f * scaleSize)) + 3f * scaleSize, (float)((double)Game1.tileSize - 18.0 * (double)scaleSize + 2.0)), 3f * scaleSize, 1f, Color.White);
            if (drawStackNumber && this.Quality > 0)
            {
                float num = this.Quality < 4 ? 0.0f : (float)((Math.Cos((double)Game1.currentGameTime.TotalGameTime.Milliseconds * Math.PI / 512.0) + 1.0) * 0.0500000007450581);
                spriteBatch.Draw(Game1.mouseCursors, location + new Vector2(12f, (float)(Game1.tileSize - 12) + num), new Microsoft.Xna.Framework.Rectangle?(this.Quality < 4 ? new Microsoft.Xna.Framework.Rectangle(338 + (this.Quality - 1) * 8, 400, 8, 8) : new Microsoft.Xna.Framework.Rectangle(346, 392, 8, 8)), Color.White * transparency, 0.0f, new Vector2(4f, 4f), (float)(3.0 * (double)scaleSize * (1.0 + (double)num)), SpriteEffects.None, layerDepth);
            }
        }

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {
            if (x == -1)
            {
                spriteBatch.Draw(TextureSheet.texture, Game1.GlobalToLocal(Game1.viewport, this.drawPosition), new Rectangle?(this.sourceRect), Color.White * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (this.Decoration_type == 12) ? 0f : ((float)(this.boundingBox.Bottom - 8) / 10000f));
            }
            else
            {
                spriteBatch.Draw(TextureSheet.texture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * Game1.tileSize), (float)(y * Game1.tileSize - (this.sourceRect.Height * Game1.pixelZoom - this.boundingBox.Height)))), new Rectangle?(this.sourceRect), Color.White * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (this.Decoration_type == 12) ? 0f : ((float)(this.boundingBox.Bottom - 8) / 10000f));
            }
            if (this.heldObject.Value != null)
            {
                if (this.heldObject.Value is ModularCropObject)
                {
                    (this.heldObject.Value as ModularCropObject).drawAtNonTileSpot(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(this.boundingBox.Center.X - Game1.tileSize / 2), (float)(this.boundingBox.Center.Y - (this.heldObject.Value as ModularCropObject).sourceRect.Height * Game1.pixelZoom - Game1.tileSize / 4))), (float)(this.boundingBox.Bottom - 7) / 10000f, alpha);
                    return;
                }
                spriteBatch.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(this.boundingBox.Center.X - Game1.tileSize / 2), (float)(this.boundingBox.Center.Y - Game1.tileSize * 4 / 3))) + new Vector2((float)(Game1.tileSize / 2), (float)(Game1.tileSize * 5 / 6)), new Rectangle?(Game1.shadowTexture.Bounds), Color.White * alpha, 0f, new Vector2((float)Game1.shadowTexture.Bounds.Center.X, (float)Game1.shadowTexture.Bounds.Center.Y), 4f, SpriteEffects.None, (float)this.boundingBox.Bottom / 10000f);
                spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(this.boundingBox.Center.X - Game1.tileSize / 2), (float)(this.boundingBox.Center.Y - Game1.tileSize * 4 / 3))), new Rectangle?(GameLocation.getSourceRectForObject(this.heldObject.Value.ParentSheetIndex)), Color.White * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, (float)(this.boundingBox.Bottom + 1) / 10000f);
            }
        }

        public override void drawAtNonTileSpot(SpriteBatch spriteBatch, Vector2 location, float layerDepth, float alpha = 1f)
        {
            spriteBatch.Draw(TextureSheet.texture, location, new Rectangle?(this.sourceRect), Color.White * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth);
        }

        public override Item getOne(IModHelper helper)
        {
            ModularCropObject ModularCropObject = new ModularCropObject(helper,this.ParentSheetIndex,this.Stack,this.texturePath,this.dataFileName);

            ModularCropObject.drawPosition = this.drawPosition;
            ModularCropObject.defaultBoundingBox = this.defaultBoundingBox;
            ModularCropObject.boundingBox.Value = this.boundingBox.Value;
            ModularCropObject.currentRotation = this.currentRotation - 1;
            ModularCropObject.rotations = this.rotations;
            //rotate();

            return ModularCropObject;
        }

        public virtual void resetTexture(IModHelper helper)
        {
            TextureSheet = new Texture2DExtended(helper, TextureSheet.path);
        }

        public override string getCategoryName()
        {
            if (this.cropType != "") return ("Modded Crop:"+this.cropType);
            return "Modded Crop";
            //return "Modded Crop";
        }

        public override Color getCategoryColor()
        {
            return Color.DarkBlue;
            
        }


        public static new void Serialize(Item I)
        {
        
            StardustCore.ModCore.SerializationManager.WriteToJsonFile(Path.Combine(StardustCore.ModCore.SerializationManager.playerInventoryPath, I.Name + ".json"), (ModularCropObject)I);
        }

        public static void Serialize(Item I,string s)
        {

            StardustCore.ModCore.SerializationManager.WriteToJsonFile(Path.Combine(s, I.Name + ".json"), (ModularCropObject)I);
        }

        public static ModularCropObject ParseIntoInventory(string s)
        {
            // PlanterBox p = new PlanterBox();
            // return p;

            var crop = StardustCore.ModCore.ModHelper.ReadJsonFile<ModularCropObject>(s);
            return crop;

        }

        public static void SerializeFromWorld(Item I)
        {
            //  ModCore.serilaizationManager.WriteToJsonFile(Path.Combine(ModCore.serilaizationManager.objectsInWorldPath, (c as CoreObject).thisLocation.name, c.Name + ".json"), (PlanterBox)c);
            StardustCore.ModCore.SerializationManager.WriteToJsonFile(Path.Combine(StardustCore.ModCore.SerializationManager.objectsInWorldPath, I.Name + ".json"), (ModularCropObject)I);
        }
    }
}
