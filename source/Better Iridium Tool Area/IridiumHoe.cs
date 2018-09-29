using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PyTK.CustomElementHandler;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using xTile.Dimensions;



namespace BetterIridiumFarmTools
{
    public class IridiumHoe : Hoe, ISaveElement
    {
        internal static Texture2D texture;

        private bool inUse;

        public override int UpgradeLevel
        {
            get
            {
                return this.UpgradeLevel;
            }
            set
            {
                this.upgradeLevel = value;
                this.setNewTileIndexForUpgradeLevel();
            }
        }

        public Dictionary<string, string> getAdditionalSaveData()
        {
            return new Dictionary<string, string>()
            {
                {
                    "name",
                    this.name
                }
            };
        }

        public object getReplacement()
        {
            Chest chest = new Chest(true);
            return (object)chest;
        }

        public void rebuild(Dictionary<string, string> additionalSaveData, object replacement)
        {
            this.build();
           
        }

        public IridiumHoe()
        {
            this.build();
        }

        public override string Name
        {
            get
            {
                return this.name;
            }
        }

        public override bool canBeTrashed()
        {
            return false;
        }

        

        public override bool actionWhenPurchased()
        {
            return true;
        }

        public override Item getOne()
        {
            return (Item)new IridiumHoe();
        }

        private void build()
        {
            this.name = "Good Hoe";
            this.description = "Hoes just the right way";
            //77 for blank
            this.initialParentTileIndex = 21;
            this.currentParentTileIndex = 21;
            this.indexOfMenuItemView = 47;
            //Check here for failure
            this.upgradeLevel = 4;
            this.instantUse = false;
            this.inUse = false;
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber)
        {
            this.upgradeLevel = 4;
            base.drawInMenu(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber);
        }

        public override bool onRelease(GameLocation location, int x, int y, StardewValley.Farmer who)
        {
            this.inUse = false;
            return base.onRelease(location, x, y, who);
        }

        public override bool beginUsing(GameLocation location, int x, int y, StardewValley.Farmer who)
        {
            this.inUse = true;
            return base.beginUsing(location, x, y, who);
        }

        public override void DoFunction(GameLocation location, int x, int y, int power, StardewValley.Farmer who)
        {
            this.lastUser = who;
            if (location.Name.Equals("UndergroundMine"))
                power = 1;
            who.Stamina -= (float)(2 * power) - (float)who.FarmingLevel * 0.1f;
            power = who.toolPower;
            who.stopJittering();
            Game1.playSound("woodyHit");
            Vector2 vector2 = new Vector2((float)(x / Game1.tileSize), (float)(y / Game1.tileSize));
            List<Vector2> vector2List = IridiumTiles.AFTiles(vector2, power, who);
            foreach (Vector2 index in vector2List)
            {
                index.Equals(vector2);
                if (location.terrainFeatures.ContainsKey(index))
                {
                    //terrainfeatures.performToolAction apparently always returns false
                    if (location.terrainFeatures[index].performToolAction((Tool)this, 0, index, (GameLocation)null))
                        location.terrainFeatures.Remove(index);
                }
                else
                {
                    //Test of object[index].performToolAction
                    StardewValley.Object obj = null;
                    if (location.Objects.ContainsKey(index))
                    {
                        obj = location.Objects[index];
                    }
                    //if (location.objects.ContainsKey(index) && location.Objects[index].performToolAction((Tool)this))


                    if (location.objects.ContainsKey(index) && IridiumTiles.perfHoeAtion(this, obj))
                        {
                        if (location.Objects[index].type.Equals("Crafting") && location.Objects[index].fragility != 2)
                        {
                            List<Debris> debris1 = location.debris;
                            int objectIndex = location.Objects[index].bigCraftable ? -location.Objects[index].ParentSheetIndex : location.Objects[index].ParentSheetIndex;
                            Vector2 toolLocation = who.GetToolLocation(false);
                            Microsoft.Xna.Framework.Rectangle boundingBox = who.GetBoundingBox();
                            double x1 = (double)boundingBox.Center.X;
                            boundingBox = who.GetBoundingBox();
                            double y1 = (double)boundingBox.Center.Y;
                            Vector2 playerPosition = new Vector2((float)x1, (float)y1);
                            Debris debris2 = new Debris(objectIndex, toolLocation, playerPosition);
                            debris1.Add(debris2);
                        }
                        location.Objects[index].performRemoveAction(index, location);
                        location.Objects.Remove(index);
                    }
                    if (location.doesTileHaveProperty((int)index.X, (int)index.Y, "Diggable", "Back") != null)
                    {
                        if (location.Name.Equals("UndergroundMine") && !location.isTileOccupied(index, ""))
                        {
                            if (Game1.mine.mineLevel < 40 || Game1.mine.mineLevel >= 80)
                            {
                                location.terrainFeatures.Add(index, (TerrainFeature)new HoeDirt());
                                Game1.playSound("hoeHit");
                            }
                            else if (Game1.mine.mineLevel < 80)
                            {
                                location.terrainFeatures.Add(index, (TerrainFeature)new HoeDirt());
                                Game1.playSound("hoeHit");
                            }
                            Game1.removeSquareDebrisFromTile((int)index.X, (int)index.Y);
                            location.checkForBuriedItem((int)index.X, (int)index.Y, false, false);
                            location.temporarySprites.Add(new TemporaryAnimatedSprite(12, new Vector2(vector2.X * (float)Game1.tileSize, vector2.Y * (float)Game1.tileSize), Color.White, 8, Game1.random.NextDouble() < 0.5, 50f, 0, -1, -1f, -1, 0));
                            if (vector2List.Count > 2)
                                location.temporarySprites.Add(new TemporaryAnimatedSprite(6, new Vector2(index.X * (float)Game1.tileSize, index.Y * (float)Game1.tileSize), Color.White, 8, Game1.random.NextDouble() < 0.5, Vector2.Distance(vector2, index) * 30f, 0, -1, -1f, -1, 0));
                        }
                        else if (!location.isTileOccupied(index, "") && location.isTilePassable(new Location((int)index.X, (int)index.Y), Game1.viewport))
                        {
                            location.makeHoeDirt(index);
                            Game1.playSound("hoeHit");
                            Game1.removeSquareDebrisFromTile((int)index.X, (int)index.Y);
                            location.temporarySprites.Add(new TemporaryAnimatedSprite(12, new Vector2(index.X * (float)Game1.tileSize, index.Y * (float)Game1.tileSize), Color.White, 8, Game1.random.NextDouble() < 0.5, 50f, 0, -1, -1f, -1, 0));
                            if (vector2List.Count > 2)
                                location.temporarySprites.Add(new TemporaryAnimatedSprite(6, new Vector2(index.X * (float)Game1.tileSize, index.Y * (float)Game1.tileSize), Color.White, 8, Game1.random.NextDouble() < 0.5, Vector2.Distance(vector2, index) * 30f, 0, -1, -1f, -1, 0));
                            location.checkForBuriedItem((int)index.X, (int)index.Y, false, false);
                        }
                        ++Game1.stats.DirtHoed;
                    }
                }
            }
        }

        public override void draw(SpriteBatch b)
        {
            if (Game1.player.toolPower <= 0 || !Game1.player.canReleaseTool)
                return;
            foreach (Vector2 vector2 in IridiumTiles.AFTiles(Game1.player.GetToolLocation(false) / (float)Game1.tileSize, Game1.player.toolPower, Game1.player))
                b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(new Vector2((float)((int)vector2.X * Game1.tileSize), (float)((int)vector2.Y * Game1.tileSize))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(194, 388, 16, 16)), Color.White, 0.0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 0.01f);
        }

        public override void setNewTileIndexForUpgradeLevel()
        {
            int num1 = 21;
            int num2 = num1 + this.upgradeLevel * 7;
            if (this.upgradeLevel > 2)
                num2 += 21;
            this.initialParentTileIndex = num2;
            this.currentParentTileIndex = this.initialParentTileIndex;
            this.indexOfMenuItemView = this.initialParentTileIndex + 21;
        }

        public override string getDescription()
        {
            string desc = Game1.parseText("Hoes just the right way.", Game1.smallFont, Game1.tileSize * 4 + Game1.tileSize / 4);
            return desc;
        }

        protected override string loadDescription()
        {
            return this.getDescription();
        }

        protected override string loadDisplayName()
        {
            return this.name;
        }



    }
}
