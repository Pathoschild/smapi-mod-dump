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
using StardewValley.Locations;

namespace BetterIridiumFarmTools
{
    public class IridiumWaterCan : WateringCan, ISaveElement
    {
        private bool inUse;

        public int waterCanMax = 100;

        private int waterLeft = 100;

        public int WaterLeft
        {
            get
            {
                return this.waterLeft;
            }
            set
            {
                this.waterLeft = value;
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
            Chest chest = (Chest)replacement;
            if (chest.isEmpty())
                return;
        }

        public IridiumWaterCan()
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
            return (Item)new IridiumWaterCan();
        }

        private void build()
        {
            this.name = "Good Can";
            this.description = "Waters just the right way";
            this.waterCanMax = 100;
            this.waterLeft = 100;
            this.initialParentTileIndex = 273;
            this.currentParentTileIndex = 273;
            this.indexOfMenuItemView = 296;
            this.upgradeLevel = 4;
            this.instantUse = false;
            this.inUse = false;
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber)
        {
            IridiumTiles.toolDrawInMenu(spriteBatch, location + new Vector2(0.0f, (float)(-Game1.tileSize / 4 + 4)), scaleSize, transparency, layerDepth, drawStackNumber, this);
            if (!drawStackNumber)
                return;
            spriteBatch.Draw(Game1.mouseCursors, location + new Vector2(4f, (float)(Game1.tileSize - 20)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(297, 420, 14, 5)), Color.White * transparency, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth + 0.0001f);
            spriteBatch.Draw(Game1.staminaRect, new Microsoft.Xna.Framework.Rectangle((int)location.X + 8, (int)location.Y + Game1.tileSize - 16, (int)((double)this.waterLeft / (double)this.waterCanMax * 48.0), 8), Color.DodgerBlue * 0.7f * transparency);
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
            //base.DoFunction(location, x, y, power, who);
            power = who.toolPower;
            who.stopJittering();
            List<Vector2> source = IridiumTiles.AFTiles(new Vector2((float)(x / Game1.tileSize), (float)(y / Game1.tileSize)), power, who);
            if (location.doesTileHaveProperty(x / Game1.tileSize, y / Game1.tileSize, "Water", "Back") != null || location.doesTileHaveProperty(x / Game1.tileSize, y / Game1.tileSize, "WaterSource", "Back") != null || location is BuildableGameLocation && (location as BuildableGameLocation).getBuildingAt(source.First<Vector2>()) != null && ((location as BuildableGameLocation).getBuildingAt(source.First<Vector2>()).buildingType.Equals("Well") && (location as BuildableGameLocation).getBuildingAt(source.First<Vector2>()).daysOfConstructionLeft <= 0))
            {
                who.jitterStrength = 0.5f;
                switch (this.upgradeLevel)
                {
                    case 0:
                        this.waterCanMax = 40;
                        break;
                    case 1:
                        this.waterCanMax = 55;
                        break;
                    case 2:
                        this.waterCanMax = 70;
                        break;
                    case 3:
                        this.waterCanMax = 85;
                        break;
                    case 4:
                        this.waterCanMax = 100;
                        break;
                }
                this.waterLeft = this.waterCanMax;
                Game1.playSound("slosh");
                DelayedAction.playSoundAfterDelay("glug", 250);
            }
            else if (this.waterLeft > 0)
            {
                who.Stamina -= (float)(2 * (power + 1)) - (float)who.FarmingLevel * 0.1f;
                int num = 0;
                foreach (Vector2 index in source)
                {

                    //UP function in process

                    if (location.terrainFeatures.ContainsKey(index))
                    {

                        if (location.terrainFeatures.ContainsKey(index) && location.terrainFeatures[index].GetType() == typeof(HoeDirt))
                        {
                            HoeDirt joeDirt = new HoeDirt();
                            if (location.terrainFeatures.ContainsKey(index) && location.terrainFeatures[index].GetType() == typeof(HoeDirt))
                                joeDirt = (HoeDirt)location.terrainFeatures[index];
                            IridiumTiles.perfWaterAction(this, 0, index, (GameLocation)null, joeDirt);
                        }
                        location.terrainFeatures[index].performToolAction((Tool)this, 0, index, (GameLocation)null);
                    }
                   
                   
                    if (location.objects.ContainsKey(index))
                        location.Objects[index].performToolAction((Tool)this);
                    location.performToolAction((Tool)this, (int)index.X, (int)index.Y);
                    location.temporarySprites.Add(new TemporaryAnimatedSprite(13, new Vector2(index.X * (float)Game1.tileSize, index.Y * (float)Game1.tileSize), Color.White, 10, Game1.random.NextDouble() < 0.5, 70f, 0, Game1.tileSize, (float)(((double)index.Y * (double)Game1.tileSize + (double)(Game1.tileSize / 2)) / 10000.0 - 0.00999999977648258), -1, 0)
                    {
                        delayBeforeAnimationStart = 200 + num * 10
                    });
                    ++num;
                }
                this.waterLeft -= power + 1;
                Vector2 vector2 = new Vector2(who.position.X - (float)(Game1.tileSize / 2) - (float)Game1.pixelZoom, who.position.Y - (float)(Game1.tileSize / 4) - (float)Game1.pixelZoom);
                switch (who.facingDirection)
                {
                    case 0:
                        vector2 = Vector2.Zero;
                        break;
                    case 1:
                        vector2.X += (float)(Game1.tileSize * 2 + Game1.pixelZoom * 2);
                        break;
                    case 2:
                        vector2.X += (float)(Game1.tileSize + Game1.pixelZoom * 2);
                        vector2.Y += (float)(Game1.tileSize / 2 + Game1.pixelZoom * 3);
                        break;
                }
                if (vector2.Equals(Vector2.Zero))
                    return;
                for (int index = 0; index < 30; ++index)
                    location.temporarySprites.Add(new TemporaryAnimatedSprite(Game1.staminaRect, new Microsoft.Xna.Framework.Rectangle(0, 0, 1, 1), 999f, 1, 999, vector2 + new Vector2((float)(Game1.random.Next(-3, 0) * Game1.pixelZoom), (float)(Game1.random.Next(2) * Game1.pixelZoom)), false, false, (float)(who.GetBoundingBox().Bottom + Game1.tileSize / 2) / 10000f, 0.04f, Game1.random.NextDouble() < 0.5 ? Color.DeepSkyBlue : Color.LightBlue, (float)Game1.pixelZoom, 0.0f, 0.0f, 0.0f, false)
                    {
                        delayBeforeAnimationStart = index * 15,
                        motion = new Vector2((float)Game1.random.Next(-10, 11) / 100f, 0.5f),
                        acceleration = new Vector2(0.0f, 0.1f)
                    });
            }
            else
            {
                who.doEmote(4);
                Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:WateringCan.cs.14335"));
            }
        }

        public override void draw(SpriteBatch b)
        {
            if (Game1.player.toolPower <= 0 || !Game1.player.canReleaseTool)
                return;
            foreach (Vector2 vector2 in IridiumTiles.AFTiles(Game1.player.GetToolLocation(false) / (float)Game1.tileSize, Game1.player.toolPower, Game1.player))
                b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(new Vector2((float)((int)vector2.X * Game1.tileSize), (float)((int)vector2.Y * Game1.tileSize))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(194, 388, 16, 16)), Color.White, 0.0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 0.01f);
        }

        public override string getDescription()
        {
            string desc = Game1.parseText("Waters just the right way.", Game1.smallFont, Game1.tileSize * 4 + Game1.tileSize / 4);
            return desc;
        }

        protected override string loadDisplayName()
        {
            return this.name;
        }

        protected override string loadDescription()
        {
            return this.getDescription();
        }


        /* IN CLASS------------------------------------------------------*/
    }
}
