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

namespace Revitalize.Framework.Objects.Items.Tools
{
    public class MiningDrill : PickaxeExtended, IEnergyInterface
    {
        private int boulderTileX;
        private int boulderTileY;
        private int hitsToBoulder;
        private Texture2D energyTexture;

        public MiningDrill()
        {

        }

        public MiningDrill(BasicItemInformation ItemInfo, int UpgradeLevel, Texture2DExtended WorkingTexture)
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
            spriteBatch.Draw(this.energyTexture, new Rectangle((int)location.X + 8, (int)location.Y + Game1.tileSize / 2, (int)((Game1.tileSize - 16) * this.GetEnergyManager().energyPercentRemaining), (int)16), new Rectangle(0, 0, 1, 1), EnergyUtilities.GetEnergyRemainingColor(this.GetEnergyManager()), 0f, Vector2.Zero, SpriteEffects.None, layerDepth);
        }

        public override bool beginUsing(GameLocation location, int x, int y, Farmer who)
        {
            this.updateInfo();
            Revitalize.Framework.Hacks.ColorChanger.SwapPickaxeTextures(this.workingTexture.texture);
            this.Update(who.FacingDirection, 0, who);
            who.EndUsingTool();
            return true;
        }

        public override void endUsing(GameLocation location, Farmer who)
        {
            if (this.GetEnergyManager().hasEnoughEnergy(this.getEnergyConsumptionRate()) == false)
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

        private void initializeEnergyTexture()
        {
            this.energyTexture = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            Color[] color = new Color[1];
            color[0] = new Color(255, 255, 255);
            this.energyTexture.SetData(color, 0, 1);
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
            //base.DoFunction(location, x, y, power, who);
            if (this.GetEnergyManager().hasEnoughEnergy(this.getEnergyConsumptionRate()) == true)
            {
            }
            else
            {
                Game1.showRedMessage("Out of energy!");
                return;
            }
            this.baseDoFunction(location, x, y, power, who);
            power = who.toolPower;
            //who.Stamina -= (float)(2 * (power + 1)) - (float)who.MiningLevel * 0.1f;
            //Drain energy here;
            this.GetEnergyManager().consumeEnergy(this.getEnergyConsumptionRate());
            //Double check to prevent animation from happening with even no power


            Utility.clampToTile(new Vector2((float)x, (float)y));
            int num1 = x / 64;
            int num2 = y / 64;
            Vector2 index = new Vector2((float)num1, (float)num2);
            if (location.performToolAction((Tool)this, num1, num2))
                return;
            StardewValley.Object @object = (StardewValley.Object)null;
            location.Objects.TryGetValue(index, out @object);
            if (@object == null)
            {
                if (who.FacingDirection == 0 || who.FacingDirection == 2)
                {
                    num1 = (x - 8) / 64;
                    location.Objects.TryGetValue(new Vector2((float)num1, (float)num2), out @object);
                    if (@object == null)
                    {
                        num1 = (x + 8) / 64;
                        location.Objects.TryGetValue(new Vector2((float)num1, (float)num2), out @object);
                    }
                }
                else
                {
                    num2 = (y + 8) / 64;
                    location.Objects.TryGetValue(new Vector2((float)num1, (float)num2), out @object);
                    if (@object == null)
                    {
                        num2 = (y - 8) / 64;
                        location.Objects.TryGetValue(new Vector2((float)num1, (float)num2), out @object);
                    }
                }
                x = num1 * 64;
                y = num2 * 64;
                if (location.terrainFeatures.ContainsKey(index) && location.terrainFeatures[index].performToolAction((Tool)this, 0, index, location))
                    location.terrainFeatures.Remove(index);
            }
            index = new Vector2((float)num1, (float)num2);
            if (@object != null)
            {
                if (@object.Name.Equals("Stone"))
                {
                    location.playSound("hammer");
                    if (@object.MinutesUntilReady > 0)
                    {
                        int num3 = Math.Max(1, this.UpgradeLevel + 1);
                        @object.MinutesUntilReady -= num3;
                        @object.shakeTimer = 200;
                        if (@object.MinutesUntilReady > 0)
                        {
                            Game1.createRadialDebris(Game1.currentLocation, 14, num1, num2, Game1.random.Next(2, 5), false, -1, false, -1);
                            return;
                        }
                    }
                    if (@object.ParentSheetIndex < 200 && !Game1.objectInformation.ContainsKey(@object.ParentSheetIndex + 1))
                        MultiplayerUtilities.GetMultiplayer().broadcastSprites(location, new TemporaryAnimatedSprite(@object.ParentSheetIndex + 1, 300f, 1, 2, new Vector2((float)(x - x % 64), (float)(y - y % 64)), true, @object.Flipped)
                        {
                            alphaFade = 0.01f
                        });
                    else
                        MultiplayerUtilities.GetMultiplayer().broadcastSprites(location, new TemporaryAnimatedSprite(47, new Vector2((float)(num1 * 64), (float)(num2 * 64)), Color.Gray, 10, false, 80f, 0, -1, -1f, -1, 0));
                    Game1.createRadialDebris(location, 14, num1, num2, Game1.random.Next(2, 5), false, -1, false, -1);
                    MultiplayerUtilities.GetMultiplayer().broadcastSprites(location, new TemporaryAnimatedSprite(46, new Vector2((float)(num1 * 64), (float)(num2 * 64)), Color.White, 10, false, 80f, 0, -1, -1f, -1, 0)
                    {
                        motion = new Vector2(0.0f, -0.6f),
                        acceleration = new Vector2(0.0f, 1f / 500f),
                        alphaFade = 0.015f
                    });
                    if (!location.Name.StartsWith("UndergroundMine"))
                    {
                        if (@object.ParentSheetIndex == 343 || @object.ParentSheetIndex == 450)
                        {
                            Random random = new Random((int)Game1.stats.DaysPlayed + (int)Game1.uniqueIDForThisGame / 2 + num1 * 2000 + num2);
                            if (random.NextDouble() < 0.035 && Game1.stats.DaysPlayed > 1U)
                                Game1.createObjectDebris(535 + (Game1.stats.DaysPlayed <= 60U || random.NextDouble() >= 0.2 ? (Game1.stats.DaysPlayed <= 120U || random.NextDouble() >= 0.2 ? 0 : 2) : 1), num1, num2, this.getLastFarmerToUse().UniqueMultiplayerID);
                            if (random.NextDouble() < 0.035 * (who.professions.Contains(21) ? 2.0 : 1.0) && Game1.stats.DaysPlayed > 1U)
                                Game1.createObjectDebris(382, num1, num2, this.getLastFarmerToUse().UniqueMultiplayerID);
                            if (random.NextDouble() < 0.01 && Game1.stats.DaysPlayed > 1U)
                                Game1.createObjectDebris(390, num1, num2, this.getLastFarmerToUse().UniqueMultiplayerID);
                        }
                        Revitalize.ModCore.ModHelper.Reflection.GetMethod(location,"breakStone").Invoke(@object.ParentSheetIndex, num1, num2, who, new Random((int)Game1.stats.DaysPlayed + (int)Game1.uniqueIDForThisGame / 2 + num1 * 4000 + num2));
                    }
                    else
                        Game1.mine.checkStoneForItems(@object.ParentSheetIndex, num1, num2, who);
                    if (@object.MinutesUntilReady > 0)
                        return;
                    location.Objects.Remove(new Vector2((float)num1, (float)num2));
                    location.playSound("stoneCrack");
                    ++Game1.stats.RocksCrushed;
                }
                else if (@object.Name.Contains("Boulder"))
                {
                    location.playSound("hammer");
                    if (this.UpgradeLevel < 2)
                    {
                        Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Pickaxe.cs.14194")));
                    }
                    else
                    {
                        if (num1 == this.boulderTileX && num2 == this.boulderTileY)
                        {
                            this.hitsToBoulder += power + 1;
                            @object.shakeTimer = 190;
                        }
                        else
                        {
                            this.hitsToBoulder = 0;
                            this.boulderTileX = num1;
                            this.boulderTileY = num2;
                        }
                        if (this.hitsToBoulder < 4)
                            return;
                        location.removeObject(index, false);
                        MultiplayerUtilities.GetMultiplayer().broadcastSprites(location, new TemporaryAnimatedSprite(5, new Vector2((float)(64.0 * (double)index.X - 32.0), (float)(64.0 * ((double)index.Y - 1.0))), Color.Gray, 8, Game1.random.NextDouble() < 0.5, 50f, 0, -1, -1f, -1, 0)
                        {
                            delayBeforeAnimationStart = 0
                        });
                        MultiplayerUtilities.GetMultiplayer().broadcastSprites(location, new TemporaryAnimatedSprite(5, new Vector2((float)(64.0 * (double)index.X + 32.0), (float)(64.0 * ((double)index.Y - 1.0))), Color.Gray, 8, Game1.random.NextDouble() < 0.5, 50f, 0, -1, -1f, -1, 0)
                        {
                            delayBeforeAnimationStart = 200
                        });
                        MultiplayerUtilities.GetMultiplayer().broadcastSprites(location, new TemporaryAnimatedSprite(5, new Vector2(64f * index.X, (float)(64.0 * ((double)index.Y - 1.0) - 32.0)), Color.Gray, 8, Game1.random.NextDouble() < 0.5, 50f, 0, -1, -1f, -1, 0)
                        {
                            delayBeforeAnimationStart = 400
                        });
                        MultiplayerUtilities.GetMultiplayer().broadcastSprites(location, new TemporaryAnimatedSprite(5, new Vector2(64f * index.X, (float)(64.0 * (double)index.Y - 32.0)), Color.Gray, 8, Game1.random.NextDouble() < 0.5, 50f, 0, -1, -1f, -1, 0)
                        {
                            delayBeforeAnimationStart = 600
                        });
                        MultiplayerUtilities.GetMultiplayer().broadcastSprites(location, new TemporaryAnimatedSprite(25, new Vector2(64f * index.X, 64f * index.Y), Color.White, 8, Game1.random.NextDouble() < 0.5, 50f, 0, -1, -1f, 128, 0));
                        MultiplayerUtilities.GetMultiplayer().broadcastSprites(location, new TemporaryAnimatedSprite(25, new Vector2((float)(64.0 * (double)index.X + 32.0), 64f * index.Y), Color.White, 8, Game1.random.NextDouble() < 0.5, 50f, 0, -1, -1f, 128, 0)
                        {
                            delayBeforeAnimationStart = 250
                        });
                        MultiplayerUtilities.GetMultiplayer().broadcastSprites(location, new TemporaryAnimatedSprite(25, new Vector2((float)(64.0 * (double)index.X - 32.0), 64f * index.Y), Color.White, 8, Game1.random.NextDouble() < 0.5, 50f, 0, -1, -1f, 128, 0)
                        {
                            delayBeforeAnimationStart = 500
                        });
                        location.playSound("boulderBreak");
                        ++Game1.stats.BouldersCracked;
                    }
                }
                else
                {
                    if (!@object.performToolAction((Tool)this, location))
                        return;
                    @object.performRemoveAction(index, location);
                    if (@object.Type.Equals((object)"Crafting") && @object.Fragility != 2)
                    {
                        NetCollection<Debris> debris1 = Game1.currentLocation.debris;
                        int objectIndex = @object.bigCraftable.Value ? -@object.ParentSheetIndex : @object.ParentSheetIndex;
                        Vector2 toolLocation = who.GetToolLocation(false);
                        Rectangle boundingBox = who.GetBoundingBox();
                        double x1 = (double)boundingBox.Center.X;
                        boundingBox = who.GetBoundingBox();
                        double y1 = (double)boundingBox.Center.Y;
                        Vector2 playerPosition = new Vector2((float)x1, (float)y1);
                        Debris debris2 = new Debris(objectIndex, toolLocation, playerPosition);
                        debris1.Add(debris2);
                    }
                    Game1.currentLocation.Objects.Remove(index);
                }
            }
            else
            {
                location.playSound("woodyHit");
                if (location.doesTileHaveProperty(num1, num2, "Diggable", "Back") == null)
                    return;
                MultiplayerUtilities.GetMultiplayer().broadcastSprites(location, new TemporaryAnimatedSprite(12, new Vector2((float)(num1 * 64), (float)(num2 * 64)), Color.White, 8, false, 80f, 0, -1, -1f, -1, 0)
                {
                    alphaFade = 0.015f
                });
            }
        }


        public override void actionWhenStopBeingHeld(Farmer who)
        {
            Revitalize.Framework.Hacks.ColorChanger.ResetPickaxeTexture();
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
            b.Append(this.GetEnergyManager().remainingEnergy);
            b.Append("/");
            b.Append(this.GetEnergyManager().maxEnergy);
            b.Append(System.Environment.NewLine);
            b.Append(this.info.description);
            return b.ToString();
        }

        public override Item getOne()
        {
            return new MiningDrill(this.info.Copy(), this.UpgradeLevel, this.workingTexture.Copy());
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

        public override ICustomObject recreate(Dictionary<string, string> additionalSaveData, object replacement)
        {
            MiningDrill p = Revitalize.ModCore.Serializer.DeserializeGUID<MiningDrill>(additionalSaveData["GUID"]);
            return p;
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

        public override bool canBeTrashed()
        {
            return true;
        }
    }
}
