/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using Newtonsoft.Json;
using PyTK.CustomElementHandler;
using Revitalize.Framework.Energy;
using Revitalize.Framework.Utilities;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;
using StardustCore.UIUtilities;
using xTile.ObjectModel;

namespace Revitalize.Framework.Objects.Items.Tools
{
    public class Chainsaw:AxeExtended,IEnergyInterface
    {

        private Texture2D energyTexture;
        [JsonIgnore]
        public EnergyManager EnergyManager
        {
            get => this.info.EnergyManager;
            set
            {
                this.info.EnergyManager = value;
                this.info.requiresUpdate = true;
            }
        }
        public Chainsaw()
        {

        }

        public Chainsaw(BasicItemInformation ItemInfo, int UpgradeLevel, Texture2DExtended WorkingTexture)
        {
            this.info = ItemInfo;
            this.upgradeLevel.Value = UpgradeLevel;
            this.guid = Guid.NewGuid();
            this.workingTexture = WorkingTexture;
            this.updateInfo();
        }


        public override void draw(SpriteBatch b)
        {
            if (this.lastUser == null || this.lastUser.toolPower <= 0 || !this.lastUser.canReleaseTool)
                return;
            this.updateInfo();
            foreach (Vector2 vector2 in this.tilesAffected(this.lastUser.GetToolLocation(false) / 64f, this.lastUser.toolPower, this.lastUser))
                this.info.animationManager.draw(b, Game1.GlobalToLocal(new Vector2((float)((int)vector2.X * 64), (float)((int)vector2.Y * 64))), Color.White, 4f, SpriteEffects.None, 0.01f);
        }

        public override void drawAttachments(SpriteBatch b, int x, int y)
        {
            this.updateInfo();
            //base.drawAttachments(b, x, y);
            //this.info.animationManager.draw(b,)


        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            this.updateInfo();
            this.info.animationManager.draw(spriteBatch, location, color * transparency, 4f * scaleSize, SpriteEffects.None, layerDepth);
            //base.drawInMenu(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, color, drawShadow);
            if (this.energyTexture == null)
            {
                this.initializeEnergyTexture();
            }
            spriteBatch.Draw(this.energyTexture, new Rectangle((int)location.X + 8, (int)location.Y + Game1.tileSize / 2, (int)((Game1.tileSize - 16) * this.EnergyManager.energyPercentRemaining), (int)16), new Rectangle(0, 0, 1, 1), EnergyUtilities.GetEnergyRemainingColor(this.EnergyManager), 0f, Vector2.Zero, SpriteEffects.None, layerDepth);
        }

        private void initializeEnergyTexture()
        {
            this.energyTexture = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            Color[] color = new Color[1];
            color[0] = new Color(255, 255, 255);
            this.energyTexture.SetData(color, 0, 1);
        }

        public override bool beginUsing(GameLocation location, int x, int y, Farmer who)
        {
            this.updateInfo();
            Revitalize.Framework.Hacks.ColorChanger.SwapAxeTextures(this.workingTexture.texture);
            this.Update(who.FacingDirection, 0, who);
            who.EndUsingTool();
            return true;
        }
        public override void endUsing(GameLocation location, Farmer who)
        {
            if (this.EnergyManager.hasEnoughEnergy(this.getEnergyConsumptionRate()) == false)
            {
                Game1.toolAnimationDone(who);
                who.canReleaseTool = false;
                who.UsingTool = false;
                who.canMove = true;
                return;
            }

            who.stopJittering();
            who.canReleaseTool = false;
            int num = (double)who.Stamina <= 0.0 ? 2 : 1;
            if (Game1.isAnyGamePadButtonBeingPressed() || !who.IsLocalPlayer)
                who.lastClick = who.GetToolLocation(false);
            else
            {
                //who.FarmerSprite.nextOffset = 0;
                switch (who.FacingDirection)
                {
                    case 0:
                        ((FarmerSprite)who.Sprite).animateOnce(176, 60f * (float)num, 8);
                        break;
                    case 1:
                        ((FarmerSprite)who.Sprite).animateOnce(168, 60f * (float)num, 8);
                        break;
                    case 2:
                        ((FarmerSprite)who.Sprite).animateOnce(160, 60f * (float)num, 8);
                        break;
                    case 3:
                        ((FarmerSprite)who.Sprite).animateOnce(184, 60f * (float)num, 8);
                        break;
                }

            }
        }

        private void baseDoFunction(GameLocation location, int x, int y, int power, Farmer who)
        {
            this.lastUser = who;
            Game1.recentMultiplayerRandom = new Random((int)(short)Game1.random.Next((int)short.MinValue, 32768));
            ToolFactory.getIndexFromTool(this);
            if (who.FarmerSprite.currentAnimationIndex <= 0)
                return;
            MeleeWeapon.timedHitTimer = 500;

        }

        public override void DoFunction(GameLocation location, int x, int y, int power, Farmer who)
        {
            if (this.EnergyManager.hasEnoughEnergy(this.getEnergyConsumptionRate()) == true)
            {
            }
            else
            {
                Game1.showRedMessage("Out of energy!");
                return;
            }
            //base.DoFunction(location, x, y, power, who);
            //who.Stamina -= (float)(2 * power) - (float)who.ForagingLevel * 0.1f;
            this.baseDoFunction(location, x, y, power, who);
            this.EnergyManager.consumeEnergy(this.getEnergyConsumptionRate());

            int tileX = x / 64;
            int tileY = y / 64;
            Rectangle rectangle = new Rectangle(tileX * 64, tileY * 64, 64, 64);
            Vector2 index1 = new Vector2((float)tileX, (float)tileY);
            if (location.Map.GetLayer("Buildings").Tiles[tileX, tileY] != null)
            {
                PropertyValue propertyValue = (PropertyValue)null;
                location.Map.GetLayer("Buildings").Tiles[tileX, tileY].TileIndexProperties.TryGetValue("TreeStump", out propertyValue);
                if (propertyValue != null)
                {
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Axe.cs.14023"));
                    return;
                }
            }
            location.performToolAction((Tool)this, tileX, tileY);
            if (location.terrainFeatures.ContainsKey(index1) && location.terrainFeatures[index1].performToolAction((Tool)this, 0, index1, location))
                location.terrainFeatures.Remove(index1);
            Rectangle boundingBox;
            if (location.largeTerrainFeatures != null)
            {
                for (int index2 = location.largeTerrainFeatures.Count - 1; index2 >= 0; --index2)
                {
                    boundingBox = location.largeTerrainFeatures[index2].getBoundingBox();
                    if (boundingBox.Intersects(rectangle) && location.largeTerrainFeatures[index2].performToolAction((Tool)this, 0, index1, location))
                        location.largeTerrainFeatures.RemoveAt(index2);
                }
            }
            Vector2 index3 = new Vector2((float)tileX, (float)tileY);
            if (!location.Objects.ContainsKey(index3) || location.Objects[index3].Type == null || !location.Objects[index3].performToolAction((Tool)this, location))
                return;
            if (location.Objects[index3].Type.Equals((object)"Crafting") && location.Objects[index3].Fragility != 2)
            {
                NetCollection<Debris> debris1 = location.debris;
                int objectIndex = location.Objects[index3].bigCraftable.Value ? -location.Objects[index3].ParentSheetIndex : location.Objects[index3].ParentSheetIndex;
                Vector2 toolLocation = who.GetToolLocation(false);
                boundingBox = who.GetBoundingBox();
                double x1 = (double)boundingBox.Center.X;
                boundingBox = who.GetBoundingBox();
                double y1 = (double)boundingBox.Center.Y;
                Vector2 playerPosition = new Vector2((float)x1, (float)y1);
                Debris debris2 = new Debris(objectIndex, toolLocation, playerPosition);
                debris1.Add(debris2);
            }
            location.Objects[index3].performRemoveAction(index3, location);
            location.Objects.Remove(index3);
        }

        public override void actionWhenStopBeingHeld(Farmer who)
        {
            Revitalize.Framework.Hacks.ColorChanger.ResetAxeTexture();
            base.actionWhenStopBeingHeld(who);
        }

        public override Color getCategoryColor()
        {
            return this.info.categoryColor;
        }

        public override string getCategoryName()
        {
            return this.info.categoryName;
        }

        public override string getDescription()
        {
            StringBuilder b = new StringBuilder();
            b.Append("Energy: ");
            b.Append(this.EnergyManager.remainingEnergy);
            b.Append("/");
            b.Append(this.EnergyManager.maxEnergy);
            b.Append(System.Environment.NewLine);
            b.Append(this.info.description);
            return b.ToString();
        }

        public override Item getOne()
        {
            return new Chainsaw(this.info.Copy(), this.UpgradeLevel, this.workingTexture.Copy());
        }


        private int getEnergyConsumptionRate()
        {
            return this.UpgradeLevel + 1;
        }

        public ref EnergyManager GetEnergyManager()
        {
            return ref this.info.EnergyManager;
        }

        public void SetEnergyManager(ref EnergyManager Manager)
        {
            this.info.EnergyManager = Manager;
        }


        public override ICustomObject recreate(Dictionary<string, string> additionalSaveData, object replacement)
        {
            Chainsaw p = Revitalize.ModCore.Serializer.DeserializeGUID<Chainsaw>(additionalSaveData["GUID"]);
            return p;
        }
        public override Dictionary<string, string> getAdditionalSaveData()
        {
            Dictionary<string, string> serializedInfo = new Dictionary<string, string>();
            serializedInfo.Add("id", this.ItemInfo);
            serializedInfo.Add("ItemInfo", Revitalize.ModCore.Serializer.ToJSONString(this.info));
            serializedInfo.Add("GUID", this.guid.ToString());
            serializedInfo.Add("Level", this.UpgradeLevel.ToString());
            Revitalize.ModCore.Serializer.SerializeGUID(this.guid.ToString(), this);
            return serializedInfo;
        }

        public override object getReplacement()
        {
            Chest c = new Chest(true);
            c.playerChoiceColor.Value = Color.Magenta;
            c.TileLocation = new Vector2(0, 0);
            return c;
        }

        public override void rebuild(Dictionary<string, string> additionalSaveData, object replacement)
        {
            //ModCore.log("REBULD THE PICKAXE!!!!!!!");
            this.info = ModCore.Serializer.DeserializeFromJSONString<BasicItemInformation>(additionalSaveData["ItemInfo"]);
            this.UpgradeLevel = Convert.ToInt32(additionalSaveData["Level"]);
            //this.upgradeLevel.Value = (replacement as Pickaxe).UpgradeLevel;

        }

        public override bool canBeTrashed()
        {
            return true;
        }
    }
}
