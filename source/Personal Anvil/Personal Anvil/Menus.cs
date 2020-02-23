using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

namespace StardewValley.Menus
{
    public class WorkbenchGeodeMenu : MenuWithInventory
    {
        private List<TemporaryAnimatedSprite> fluffSprites = new List<TemporaryAnimatedSprite>();
        public const int region_geodeSpot = 998;
        public ClickableComponent geodeSpot;
        public AnimatedSprite clint;
        public TemporaryAnimatedSprite geodeDestructionAnimation;
        public TemporaryAnimatedSprite sparkle;
        public int geodeAnimationTimer;
        public int yPositionOfGem;
        public int alertTimer;
        public float delayBeforeShowArtifactTimer;
        public Object geodeTreasure;

        private readonly IContentHelper Content;

        public WorkbenchGeodeMenu(IContentHelper content)
            : base(null, true, true, 12, 132, 0)
        {
            Content = content;
            if (yPositionOnScreen == borderWidth + spaceToClearTopBorder)
                movePosition(0, -spaceToClearTopBorder);
            inventory.highlightMethod = new InventoryMenu.highlightThisItem(highlightGeodes);
            geodeSpot = new ClickableComponent(new Rectangle(xPositionOnScreen + spaceToClearSideBorder + borderWidth / 2, yPositionOnScreen + spaceToClearTopBorder + 4, 560, 308), "")
            {
                myID = 998,
                downNeighborID = 0
            };
            clint = new AnimatedSprite(Content.GetActualAssetKey("assets/Empty.png"), 8, 32, 48);
            if (inventory.inventory != null && inventory.inventory.Count >= 12)
            {
                for (int index = 0; index < 12; ++index)
                {
                    if (inventory.inventory[index] != null)
                        inventory.inventory[index].upNeighborID = 998;
                }
            }
            if (trashCan != null)
                trashCan.myID = 106;
            if (okButton != null)
                okButton.leftNeighborID = 11;
            if (!Game1.options.SnappyMenus)
                return;
            populateClickableComponentList();
            snapToDefaultClickableComponent();
        }

        public override void snapToDefaultClickableComponent()
        {
            currentlySnappedComponent = getComponentWithID(0);
            snapCursorToCurrentSnappedComponent();
        }

        public override bool readyToClose()
        {
            if (base.readyToClose() && geodeAnimationTimer <= 0)
                return heldItem == null;
            return false;
        }

        public bool highlightGeodes(Item i)
        {
            if (heldItem != null)
                return true;
            switch (i.ParentSheetIndex)
            {
                case 275:
                case 535:
                case 536:
                case 537:
                case 749:
                    return true;
                default:
                    return false;
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = false)
        {
            base.receiveLeftClick(x, y, false);
            if (!geodeSpot.containsPoint(x, y))
                return;
            if (heldItem != null && (heldItem.Name.Contains("Geode") || heldItem.ParentSheetIndex == 275) && (geodeAnimationTimer <= 0))
            {
                if (Game1.player.freeSpotsInInventory() > 1 || Game1.player.freeSpotsInInventory() == 1 && heldItem.Stack == 1)
                {
                    geodeSpot.item = heldItem.getOne();
                    --heldItem.Stack;
                    if (heldItem.Stack <= 0)
                        heldItem = null;
                    geodeAnimationTimer = 800;
                    Game1.playSound("stoneStep");
                    clint.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
                    {
                        new FarmerSprite.AnimationFrame(8, 90),
                        new FarmerSprite.AnimationFrame(9, 60),
                        new FarmerSprite.AnimationFrame(10, 24),
                        new FarmerSprite.AnimationFrame(11, 60),
                        new FarmerSprite.AnimationFrame(12, 30),
                        new FarmerSprite.AnimationFrame(8, 90)
                    });
                    clint.loop = false;
                }
                else
                {
                    descriptionText = Game1.content.LoadString("Strings\\UI:GeodeMenu_InventoryFull");
                    wiggleWordsTimer = 500;
                    alertTimer = 800;
                }
            }
            else
            {
                if (Game1.player.Money >= 0)
                    return;
                wiggleWordsTimer = 500;
                Game1.dayTimeMoneyBox.moneyShakeTimer = 1000;
            }
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            base.receiveRightClick(x, y, true);
        }

        public override void performHoverAction(int x, int y)
        {
            if (alertTimer > 0)
                return;
            base.performHoverAction(x, y);
            if (!descriptionText.Equals(""))
                return;
            else
                descriptionText = "Place geodes or artifact troves in the box on the left to break them!";
        }

        public override void emergencyShutDown()
        {
            base.emergencyShutDown();
            if (heldItem == null)
                return;
            Game1.player.addItemToInventoryBool(heldItem, false);
        }

        public override void update(GameTime time)
        {
            base.update(time);
            for (int index = fluffSprites.Count - 1; index >= 0; --index)
            {
                if (fluffSprites[index].update(time))
                    fluffSprites.RemoveAt(index);
            }
            if (alertTimer > 0)
                alertTimer -= time.ElapsedGameTime.Milliseconds;
            if (geodeAnimationTimer <= 0)
                return;
            geodeAnimationTimer -= time.ElapsedGameTime.Milliseconds;
            if (geodeAnimationTimer <= 0)
            {
                geodeDestructionAnimation = null;
                geodeSpot.item = null;
                Game1.player.addItemToInventoryBool(geodeTreasure, false);
                geodeTreasure = null;
                yPositionOfGem = 0;
                fluffSprites.Clear();
                delayBeforeShowArtifactTimer = 0.0f;
            }
            else
            {
                int currentFrame = clint.currentFrame;
                clint.animateOnce(time);
                if (clint.currentFrame == 11 && currentFrame != 11)
                {
                    if (geodeSpot.item != null && geodeSpot.item.ParentSheetIndex == 275)
                    {
                        Game1.playSound("hammer");
                        Game1.playSound("woodWhack");
                    }
                    else
                    {
                        Game1.playSound("hammer");
                        Game1.playSound("stoneCrack");
                    }
                    Game1.player.gainExperience(Farmer.miningSkill, 3);
                    ++Game1.stats.GeodesCracked;
                    int y = 448;
                    if (geodeSpot.item != null)
                    {
                        switch ((geodeSpot.item as Object).ParentSheetIndex)
                        {
                            case 536:
                                y += 64;
                                break;
                            case 537:
                                y += 128;
                                break;
                        }
                        geodeDestructionAnimation = new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, y, 64, 64), 100f, 8, 0, new Vector2(geodeSpot.bounds.X + 285 - 32, geodeSpot.bounds.Y + 150 - 32), false, false);
                        if (geodeSpot.item != null && geodeSpot.item.ParentSheetIndex == 275)
                        {
                            geodeDestructionAnimation = new TemporaryAnimatedSprite()
                            {
                                texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites//temporary_sprites_1"),
                                sourceRect = new Rectangle(388, 123, 18, 21),
                                sourceRectStartingPos = new Vector2(388f, 123f),
                                animationLength = 6,
                                position = new Vector2(geodeSpot.bounds.X + 273 - 32, geodeSpot.bounds.Y + 150 - 32),
                                holdLastFrame = true,
                                interval = 100f,
                                id = 777f,
                                scale = 4f
                            };
                            for (int index = 0; index < 6; ++index)
                            {
                                fluffSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(372, 1956, 10, 10), new Vector2(geodeSpot.bounds.X + 285 - 32 + Game1.random.Next(21), geodeSpot.bounds.Y + 150 - 16), false, 1f / 500f, new Color(byte.MaxValue, 222, 198))
                                {
                                    alphaFade = 0.02f,
                                    motion = new Vector2(Game1.random.Next(-20, 21) / 10f, Game1.random.Next(5, 20) / 10f),
                                    interval = 99999f,
                                    layerDepth = 0.9f,
                                    scale = 3f,
                                    scaleChange = 0.01f,
                                    rotationChange = (float)(Game1.random.Next(-5, 6) * 3.14159274101257 / 256.0),
                                    delayBeforeAnimationStart = index * 20
                                });
                                fluffSprites.Add(new TemporaryAnimatedSprite()
                                {
                                    texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites//temporary_sprites_1"),
                                    sourceRect = new Rectangle(499, 132, 5, 5),
                                    sourceRectStartingPos = new Vector2(499f, 132f),
                                    motion = new Vector2(Game1.random.Next(-30, 31) / 10f, Game1.random.Next(-7, -4)),
                                    acceleration = new Vector2(0.0f, 0.25f),
                                    totalNumberOfLoops = 1,
                                    interval = 1000f,
                                    alphaFade = 0.015f,
                                    animationLength = 1,
                                    layerDepth = 1f,
                                    scale = 4f,
                                    rotationChange = (float)(Game1.random.Next(-5, 6) * 3.14159274101257 / 256.0),
                                    delayBeforeAnimationStart = index * 10,
                                    position = new Vector2(geodeSpot.bounds.X + 285 - 32 + Game1.random.Next(21), geodeSpot.bounds.Y + 150 - 16)
                                });
                                delayBeforeShowArtifactTimer = 100f;
                            }
                        }
                        geodeTreasure = Utility.getTreasureFromGeode(geodeSpot.item);
                        if (geodeSpot.item.ParentSheetIndex == 275)
                            Game1.player.foundArtifact(geodeTreasure.ParentSheetIndex, 1);
                        else if (geodeTreasure.Type.Contains("Mineral"))
                            Game1.player.foundMineral(geodeTreasure.ParentSheetIndex);
                        else if (geodeTreasure.Type.Contains("Arch") && !Game1.player.hasOrWillReceiveMail("artifactFound"))
                            geodeTreasure = new Object(390, 5, false, -1, 0);
                    }
                }
                if (geodeDestructionAnimation != null && (geodeDestructionAnimation.id != 777.0 && geodeDestructionAnimation.currentParentTileIndex < 7 || geodeDestructionAnimation.id == 777.0 && geodeDestructionAnimation.currentParentTileIndex < 5))
                {
                    geodeDestructionAnimation.update(time);
                    if (delayBeforeShowArtifactTimer > 0.0)
                    {
                        delayBeforeShowArtifactTimer -= (float)time.ElapsedGameTime.TotalMilliseconds;
                        if (delayBeforeShowArtifactTimer <= 0.0)
                        {
                            fluffSprites.Add(geodeDestructionAnimation);
                            fluffSprites.Reverse();
                            geodeDestructionAnimation = new TemporaryAnimatedSprite()
                            {
                                interval = 50f,
                                animationLength = 2,
                                alpha = 1f / 1000f,
                                id = 777f
                            };
                        }
                    }
                    else
                    {
                        if (geodeDestructionAnimation.currentParentTileIndex < 3)
                            --yPositionOfGem;
                        --yPositionOfGem;
                        if ((geodeDestructionAnimation.currentParentTileIndex == 7 || geodeDestructionAnimation.id == 777.0 && geodeDestructionAnimation.currentParentTileIndex == 5) && geodeTreasure.Price > 75)
                        {
                            sparkle = new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 640, 64, 64), 100f, 8, 0, new Vector2(geodeSpot.bounds.X + 285 - 32, geodeSpot.bounds.Y + 150 + yPositionOfGem - 32), false, false);
                            Game1.playSound("discoverMineral");
                        }
                        else if ((geodeDestructionAnimation.currentParentTileIndex == 7 || geodeDestructionAnimation.id == 777.0 && geodeDestructionAnimation.currentParentTileIndex == 5) && geodeTreasure.Price <= 75)
                            Game1.playSound("newArtifact");
                    }
                }
                if (sparkle == null || !sparkle.update(time))
                    return;
                sparkle = null;
            }
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);
            geodeSpot = new ClickableComponent(new Rectangle(xPositionOnScreen + spaceToClearSideBorder + borderWidth / 2, yPositionOnScreen + spaceToClearTopBorder + 4, 560, 308), "Anvil");
            int yPosition = yPositionOnScreen + spaceToClearTopBorder + borderWidth + 192 - 16 + 128 + 4;
            inventory = new InventoryMenu(xPositionOnScreen + spaceToClearSideBorder + borderWidth / 2 + 12, yPosition, false, null, inventory.highlightMethod, -1, 3, 0, 0, true);
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);
            draw(b, true, true, -1, -1, -1);
            Game1.dayTimeMoneyBox.drawMoneyBox(b, -1, -1);
            b.Draw(Content.Load<Texture2D>("assets/bgpatch.png"), new Vector2(geodeSpot.bounds.X, geodeSpot.bounds.Y), new Rectangle?(new Rectangle(0, 0, 140, 78)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
            if (geodeSpot.item != null)
            {
                if (geodeDestructionAnimation == null)
                    geodeSpot.item.drawInMenu(b, new Vector2(geodeSpot.bounds.X + 253 + (geodeSpot.item.ParentSheetIndex == 275 ? -8 : 0), geodeSpot.bounds.Y + 118 + (geodeSpot.item.ParentSheetIndex == 275 ? 8 : 0)), 1f);
                else
                    geodeDestructionAnimation.draw(b, true, 0, 0, 1f);
                foreach (TemporaryAnimatedSprite fluffSprite in fluffSprites)
                    fluffSprite.draw(b, true, 0, 0, 1f);
                if (geodeTreasure != null && (double)delayBeforeShowArtifactTimer <= 0.0)
                    geodeTreasure.drawInMenu(b, new Vector2(geodeSpot.bounds.X + 253, geodeSpot.bounds.Y + 118 + yPositionOfGem), 1f);
                if (sparkle != null)
                    sparkle.draw(b, true, 0, 0, 1f);
            }
            clint.draw(b, new Vector2(geodeSpot.bounds.X + 384, geodeSpot.bounds.Y + 64), 0.877f);
            if (!hoverText.Equals(""))
                drawHoverText(b, hoverText, Game1.smallFont, 0, 0, -1, null, -1, null, null, 0, -1, -1, -1, -1, 1f, null, null);
            if (heldItem != null)
                heldItem.drawInMenu(b, new Vector2(Game1.getOldMouseX() + 8, Game1.getOldMouseY() + 8), 1f);
            if (Game1.options.hardwareCursor)
                return;
            drawMouse(b);
        }
    }
}