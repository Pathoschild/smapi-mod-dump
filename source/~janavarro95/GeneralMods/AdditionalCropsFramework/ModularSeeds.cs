using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardustCore;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using Newtonsoft.Json.Linq;

namespace AdditionalCropsFramework
{
    ///Make xtra crops data sheet
    ///edit crops png, crops xnb, and objectInformation.XNB and springObjects.png.


    /// <summary>
    /// Stardew ExtraSeeds Class. VERY Broken. Only extend from this class.
    /// </summary>
    /// 
    public class ModularSeeds : CoreObject
    {

        public new Texture2D TextureSheet;
        public string dataFilePath;

        public string cropDataFilePath;
        public string cropTextureFilePath;

        public string cropObjectDataFilePath;
        public string cropObjectTextureFilePath;

        public override string Name
        {
            get
            {
                return this.name;
            }
        }

        public ModularSeeds()
        {
            this.updateDrawPosition();
        }

        public ModularSeeds(int which, string ObjectSpriteSheet, string ObjectDataFile, string AssociatedCropTextureFile, string AssociatedCropDataFile, string AssociatedCropObjectTexture,string AssociatedCropObjectDataFile)
        {
            if (TextureSheet == null)
            {
                TextureSheet = ModCore.ModHelper.Content.Load<Texture2D>(Path.Combine(Utilities.EntensionsFolderName, ObjectSpriteSheet));  //Game1.content.Load<Texture2D>("Revitalize\\CropsNSeeds\\Graphics\\seeds");
                texturePath = ObjectSpriteSheet;
            }
            this.serializationName = this.GetType().ToString();
            Dictionary<int, string> dictionary = ModCore.ModHelper.Content.Load<Dictionary<int, string>>(Path.Combine(Utilities.EntensionsFolderName, ObjectDataFile));//Game1.content.Load<Dictionary<int, string>>("Revitalize\\CropsNSeeds\\Data\\seeds");
            dataFilePath = ObjectDataFile;
            cropDataFilePath = AssociatedCropDataFile;
            cropTextureFilePath = AssociatedCropTextureFile;

           cropObjectDataFilePath = AssociatedCropObjectDataFile;
           cropObjectTextureFilePath = AssociatedCropObjectTexture;


            //Log.AsyncC(which);
            string[] array = dictionary[which].Split(new char[]
            {
                '/'
            });
            this.name = array[0];

            try
            {
                this.description = array[6];
                this.description += "\nGrown in ";
                ModularCrop c = parseCropInfo(which);
                int trackCount = 0;
                foreach (var v in c.seasonsToGrowIn)
                {
                    if (c.seasonsToGrowIn.Count > 1)
                    {
                        trackCount++;
                        if (trackCount != c.seasonsToGrowIn.Count) description += v + ", ";
                        else description += "and " + v;
                    }
                    else description += v;
                }
                this.description += ".\n";
                this.description += "Takes ";
                int totalDaysToGrow = 0;
                foreach (var v in c.phaseDays)
                {
                    totalDaysToGrow += v;
                }
                totalDaysToGrow -= 99999;
                this.description += Convert.ToString(totalDaysToGrow) + " days to mature.\n";

                try
                {
                    this.description += array[7];
                }
                catch (Exception e)
                {
                    e.ToString();
                }
            }
            catch (Exception e)
            {
                e.ToString();
                this.description = "Some seeds! Maybe you should plant them.";
            }
            this.defaultSourceRect = new Rectangle(which * 16 % TextureSheet.Width, which * 16 / TextureSheet.Width * 16, 1, 1);
            if (array[2].Equals("-1"))
            {
            }
            else
            {
                this.defaultSourceRect.Width = Convert.ToInt32(array[2].Split(new char[]
                {
                    ' '
                })[0]);
                this.defaultSourceRect.Height = Convert.ToInt32(array[2].Split(new char[]
                {
                    ' '
                })[1]);
                this.sourceRect = new Rectangle(which * 16 % TextureSheet.Width, which * 16 / TextureSheet.Width * 16, this.defaultSourceRect.Width * 16, this.defaultSourceRect.Height * 16);
                this.defaultSourceRect = this.sourceRect;
            }
            this.defaultBoundingBox = new Rectangle((int)this.TileLocation.X, (int)this.TileLocation.Y, 1, 1);
            if (array[3].Equals("-1"))
            {
            }
            else
            {
                this.defaultBoundingBox.Width = Convert.ToInt32(array[3].Split(new char[]
                {
                    ' '
                })[0]);
                this.defaultBoundingBox.Height = Convert.ToInt32(array[3].Split(new char[]
                {
                    ' '
                })[1]);
                this.boundingBox.Value = new Rectangle((int)this.TileLocation.X * Game1.tileSize, (int)this.TileLocation.Y * Game1.tileSize, this.defaultBoundingBox.Width * Game1.tileSize, this.defaultBoundingBox.Height * Game1.tileSize);
                this.defaultBoundingBox = this.boundingBox.Value;
            }
            this.updateDrawPosition();
            this.rotations = Convert.ToInt32(array[4]);
            this.Price = Convert.ToInt32(array[5]);
            this.ParentSheetIndex = which;

        }

        public override string getDescription()
        {
            return this.description;
        }

        public override bool performDropDownAction(StardewValley.Farmer who)
        {
            this.resetOnPlayerEntry((who == null) ? Game1.currentLocation : who.currentLocation);
            return false;
        }

        public override void hoverAction()
        {
            base.hoverAction();
            if (!Game1.player.isInventoryFull())
            {
                Game1.mouseCursor = 2;
            }
        }



        public override bool checkForAction(StardewValley.Farmer who, bool justCheckingForActivity = false)
        {
            var mState = Microsoft.Xna.Framework.Input.Mouse.GetState();
            if (mState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                return this.RightClicked(who);
            }
            else
            {
                //Game1.showRedMessage("CRY");
            }

            if (justCheckingForActivity)
            {
                return true;
            }
            return this.clicked(who);
        }

        //DONT USE THIS BASE IT IS TERRIBLE
        public override bool clicked(StardewValley.Farmer who)
        {
            /*
            // Game1.showRedMessage("THIS IS CLICKED!!!");
            //var mState = Microsoft.Xna.Framework.Input.Mouse.GetState();


            if (removable == false) return false;


            Game1.haltAfterCheck = false;
            if (this.Decoration_type == 11 && who.ActiveObject != null && who.ActiveObject != null && this.heldObject == null)
            {
                //    Game1.showRedMessage("Why1?");
                return false;
            }
            if (this.heldObject == null && (who.ActiveObject == null || !(who.ActiveObject is ExtraSeeds)))
            {
                if (Game1.player.currentLocation is FarmHouse)
                {
                    //       
                    Util.addItemToInventoryAndCleanTrackedList(this);
                    removeLights(this.thisLocation);
                    this.lightsOn = false;
                    Game1.playSound("coin");
                    //        this.flaggedForPickUp = true;
                    thisLocation = null;
                    return true;
                }
                else
                {
                    // return true;
                    // this.heldObject = new ExtraSeeds(parentSheetIndex, Vector2.Zero, this.lightColor, this.inventoryMaxSize);
                    Util.addItemToInventoryAndCleanTrackedList(this);
                    removeLights(this.thisLocation);
                    this.lightsOn = false;
                    Game1.playSound("coin");
                    thisLocation = null;
                    return true;

                }
            }
            if (this.heldObject != null && who.addItemToInventoryBool(this.heldObject, false))
            {
                //    Game1.showRedMessage("Why3?");
                // if(this.heldObject!=null) Game1.player.addItemByMenuIfNecessary((Item)this.heldObject);
                Util.addItemToInventoryAndCleanTrackedList(this);
                this.heldObject.performRemoveAction(this.tileLocation, who.currentLocation);
                this.heldObject = null;
                Game1.playSound("coin");
                removeLights(this.thisLocation);
                this.lightsOn = false;
                thisLocation = null;
                return true;
            }

            */

            return false;
        }

        public override bool RightClicked(StardewValley.Farmer who)
        {


            // Game1.showRedMessage("THIS IS CLICKED!!!");
            //var mState = Microsoft.Xna.Framework.Input.Mouse.GetState();
            /*

            Game1.haltAfterCheck = false;
            if (this.Decoration_type == 11 && who.ActiveObject != null && who.ActiveObject != null && this.heldObject == null)
            {
                //    Game1.showRedMessage("Why1?");
                return false;
            }
            if (this.heldObject == null && (who.ActiveObject == null || !(who.ActiveObject is ExtraSeeds)))
            {
                if (Game1.player.currentLocation is FarmHouse)
                {
                    //       
                    Game1.player.addItemByMenuIfNecessary(this);
                    removeLights(this.thisLocation);
                    this.lightsOn = false;
                    Game1.playSound("coin");
                    //        this.flaggedForPickUp = true;
                    return true;
                }
                else
                {
                    // return true;
                    // this.heldObject = new ExtraSeeds(parentSheetIndex, Vector2.Zero, this.lightColor, this.inventoryMaxSize);
                    Game1.player.addItemByMenuIfNecessary(this);
                    removeLights(this.thisLocation);
                    this.lightsOn = false;
                    Game1.playSound("coin");
                    return true;

                }
            }
            if (this.heldObject != null && who.addItemToInventoryBool(this.heldObject, false))
            {
                //    Game1.showRedMessage("Why3?");
                // if(this.heldObject!=null) Game1.player.addItemByMenuIfNecessary((Item)this.heldObject);
                Util.addItemToInventoryElseDrop(this);
                this.heldObject.performRemoveAction(this.tileLocation, who.currentLocation);
                this.heldObject = null;
                Game1.playSound("coin");
                removeLights(this.thisLocation);
                this.lightsOn = false;
                return true;
            }

            */

            return false;
        }

        public override void DayUpdate(GameLocation location)
        {
            base.DayUpdate(location);
            this.lightGlowAdded = false;
            if (!Game1.isDarkOut() || (Game1.newDay && !Game1.isRaining))
            {
                this.removeLights(location);
                return;
            }
            // this.addLights(thisLocation, lightColor);
            this.addLights(thisLocation, lightColor);
        }

        public override bool performObjectDropInAction(Item dropIn, bool probe, StardewValley.Farmer who)
        {
            return false;
        }


        public override bool minutesElapsed(int minutes, GameLocation environment)
        {
            return false;
        }

        public override void performRemoveAction(Vector2 tileLocation, GameLocation environment)
        {
            
        }

        public override bool canBeGivenAsGift()
        {
            return false;
        }

        public override bool canBePlacedHere(GameLocation l, Vector2 tile)
        {
            return false;
        }

        public override void updateDrawPosition()
        {
            this.drawPosition = new Vector2((float)this.boundingBox.X, (float)(this.boundingBox.Y - (this.sourceRect.Height * Game1.pixelZoom - this.boundingBox.Height)));
        }

        public override void drawPlacementBounds(SpriteBatch spriteBatch, GameLocation location)
        {

        }


        public override bool placementAction(GameLocation location, int x, int y, StardewValley.Farmer who = null)
        {
            return true;
        }

        public override bool isPlaceable()
        {
            return false;
        }

        public override int salePrice()
        {
            return this.Price;
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

            spriteBatch.Draw(this.TextureSheet, objectPosition, new Microsoft.Xna.Framework.Rectangle?(GameLocation.getSourceRectForObject(f.ActiveObject.ParentSheetIndex)), Color.White, 0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, Math.Max(0f, (float)(f.getStandingY() + 2) / 10000f));
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
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber, Color c, bool drawWithShadows)
        {
            //spriteBatch.Draw(TextureSheet, location + new Vector2((float)(Game1.tileSize / 2), (float)(Game1.tileSize / 2)), new Rectangle?(this.defaultSourceRect), Color.White * transparency, 0f, new Vector2((float)(this.defaultSourceRect.Width / 2), (float)(this.defaultSourceRect.Height / 2)), 1f * this.getScaleSize() * scaleSize, SpriteEffects.None, layerDepth);
            spriteBatch.Draw(TextureSheet, location + new Vector2((float)(Game1.tileSize / 2), (float)(Game1.tileSize / 2)), new Rectangle?(this.defaultSourceRect), Color.White * transparency, 0f, new Vector2((float)(this.defaultSourceRect.Width / 2), (float)(this.defaultSourceRect.Height / 2)), 1f * this.getScaleSize() * scaleSize * 1.5f, SpriteEffects.None, layerDepth);


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
                spriteBatch.Draw(TextureSheet, Game1.GlobalToLocal(Game1.viewport, this.drawPosition), new Rectangle?(this.sourceRect), Color.White * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (this.Decoration_type == 12) ? 0f : ((float)(this.boundingBox.Bottom - 8) / 10000f));
            }
            else
            {
                spriteBatch.Draw(TextureSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(x * Game1.tileSize), (float)(y * Game1.tileSize - (this.sourceRect.Height * Game1.pixelZoom - this.boundingBox.Height)))), new Rectangle?(this.sourceRect), Color.White * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (this.Decoration_type == 12) ? 0f : ((float)(this.boundingBox.Bottom - 8) / 10000f));
            }
            if (this.heldObject.Value != null)
            {
                if (this.heldObject.Value is ModularSeeds)
                {
                    (this.heldObject.Value as ModularSeeds).drawAtNonTileSpot(spriteBatch, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(this.boundingBox.Center.X - Game1.tileSize / 2), (float)(this.boundingBox.Center.Y - (this.heldObject.Value as ModularSeeds).sourceRect.Height * Game1.pixelZoom - Game1.tileSize / 4))), (float)(this.boundingBox.Bottom - 7) / 10000f, alpha);
                    return;
                }
                spriteBatch.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(this.boundingBox.Center.X - Game1.tileSize / 2), (float)(this.boundingBox.Center.Y - Game1.tileSize * 4 / 3))) + new Vector2((float)(Game1.tileSize / 2), (float)(Game1.tileSize * 5 / 6)), new Rectangle?(Game1.shadowTexture.Bounds), Color.White * alpha, 0f, new Vector2((float)Game1.shadowTexture.Bounds.Center.X, (float)Game1.shadowTexture.Bounds.Center.Y), 4f, SpriteEffects.None, (float)this.boundingBox.Bottom / 10000f);
                spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(this.boundingBox.Center.X - Game1.tileSize / 2), (float)(this.boundingBox.Center.Y - Game1.tileSize * 4 / 3))), new Rectangle?(GameLocation.getSourceRectForObject(this.heldObject.Value.ParentSheetIndex)), Color.White * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, (float)(this.boundingBox.Bottom + 1) / 10000f);
            }
        }

        public override void drawAtNonTileSpot(SpriteBatch spriteBatch, Vector2 location, float layerDepth, float alpha = 1f)
        {
            spriteBatch.Draw(TextureSheet, location, new Rectangle?(this.sourceRect), Color.White * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth);
        }

        public override Item getOne()
        {
            ModularSeeds ExtraSeeds = new ModularSeeds(this.ParentSheetIndex, this.texturePath, this.dataFilePath,this.cropTextureFilePath,this.cropDataFilePath,this.cropObjectTextureFilePath,this.cropObjectDataFilePath);
            /*
            drawPosition = this.drawPosition;
            defaultBoundingBox = this.defaultBoundingBox;
            boundingBox = this.boundingBox;
            currentRotation = this.currentRotation - 1;
            rotations = this.rotations;
            rotate();
            */
            return ExtraSeeds;
        }

        public override string getCategoryName()
        {
            return "Modular Seeds";
            //  return base.getCategoryName();
        }

        public override Color getCategoryColor()
        {
            return Color.Turquoise;
        }


        public override int maximumStackSize()
        {
            return 999;
        }

        public ModularCrop parseCropInfo(int seedIndex)
        {
            ModularCrop c = new ModularCrop();
           // Log.AsyncC(this.cropDataFilePath);
            Dictionary<int, string> dictionary = ModCore.ModHelper.Content.Load<Dictionary<int, string>>(Path.Combine(Utilities.EntensionsFolderName, this.cropDataFilePath)); //Game1.content.Load<Dictionary<int, string>>("Data\\Crops");

            if (dictionary.ContainsKey(seedIndex))
            {
                string[] array = dictionary[seedIndex].Split(new char[]
                {
                    '/'
                });
                string[] array2 = array[0].Split(new char[]
                {
                    ' '
                });
                for (int i = 0; i < array2.Length; i++)
                {
                    c.phaseDays.Add(Convert.ToInt32(array2[i]));
                }
                c.phaseDays.Add(99999);
                string[] array3 = array[1].Split(new char[]
                {
                    ' '
                });
                for (int j = 0; j < array3.Length; j++)
                {
                    c.seasonsToGrowIn.Add(array3[j]);
                }
                c.rowInSpriteSheet = Convert.ToInt32(array[2]);
                c.indexOfHarvest = Convert.ToInt32(array[3]);
                c.regrowAfterHarvest = Convert.ToInt32(array[4]);
                c.harvestMethod = Convert.ToInt32(array[5]);
                string[] array4 = array[6].Split(new char[]
                {
                    ' '
                });
                if (array4.Length != 0 && array4[0].Equals("true"))
                {
                    c.minHarvest = Convert.ToInt32(array4[1]);
                    c.maxHarvest = Convert.ToInt32(array4[2]);
                    c.maxHarvestIncreasePerFarmingLevel = Convert.ToInt32(array4[3]);
                    c.chanceForExtraCrops = Convert.ToDouble(array4[4]);
                }
                c.raisedSeeds = Convert.ToBoolean(array[7]);
                string[] array5 = array[8].Split(new char[]
                {
                    ' '
                });
                if (array5.Length != 0 && array5[0].Equals("true"))
                {
                    List<Color> list = new List<Color>();
                    for (int k = 1; k < array5.Length; k += 3)
                    {
                        list.Add(new Color((int)Convert.ToByte(array5[k]), (int)Convert.ToByte(array5[k + 1]), (int)Convert.ToByte(array5[k + 2])));
                    }
                    Random random = new Random(seedIndex * 1000 + Game1.timeOfDay + Game1.dayOfMonth);
                    c.tintColor = list[random.Next(list.Count)];
                    c.programColored = true;
                }
                c.flip = (Game1.random.NextDouble() < 0.5);
            }
            if (c.rowInSpriteSheet == 23)
            {
                c.whichForageCrop = seedIndex;
            }

            return c;
        }



        public static new void Serialize(Item I)
        {
            StardustCore.ModCore.SerializationManager.WriteToJsonFile(Path.Combine(StardustCore.ModCore.SerializationManager.playerInventoryPath, I.Name + ".json"), (ModularSeeds)I);
        }

        public static void Serialize(Item I,string s)
        {
            StardustCore.ModCore.SerializationManager.WriteToJsonFile(Path.Combine(s, I.Name + ".json"), (ModularSeeds)I);
        }

        public static Item ParseIntoInventory(string s)
        {
            return AdditionalCropsFramework.ModCore.ModHelper.ReadJsonFile<ModularSeeds>(s);
        }

        public static void SerializeFromWorld(Item I)
        {
            //  ModCore.serilaizationManager.WriteToJsonFile(Path.Combine(ModCore.serilaizationManager.objectsInWorldPath, (c as CoreObject).thisLocation.name, c.Name + ".json"), (PlanterBox)c);
            StardustCore.ModCore.SerializationManager.WriteToJsonFile(Path.Combine(StardustCore.ModCore.SerializationManager.objectsInWorldPath, I.Name + ".json"), (ModularSeeds)I);
        }
    }
}
