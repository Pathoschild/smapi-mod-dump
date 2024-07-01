/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/Personal-Anvil
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace PersonalAnvil
{
    public class WorkbenchGeodeMenu : MenuWithInventory
    {
        public const int region_geodeSpot = 998;
        public ClickableComponent geodeSpot;
        private AnimatedSprite clint;
        private TemporaryAnimatedSprite geodeDestructionAnimation;
        private TemporaryAnimatedSprite sparkle;
        public int geodeAnimationTimer;
        private int yPositionOfGem;
        private int alertTimer;
        private float delayBeforeShowArtifactTimer;
        private Item geodeTreasure;
        private Item geodeTreasureOverride;
        private bool waitingForServerResponse;
        private readonly IModHelper content;
        private List<TemporaryAnimatedSprite> fluffSprites = new();

        public WorkbenchGeodeMenu(IModHelper content) : base(null, true, true, 12, 132)
        {
            this.content = content;
            if (yPositionOnScreen == borderWidth + spaceToClearTopBorder)
                movePosition(0, -spaceToClearTopBorder);
            inventory.highlightMethod = highlightGeodes;
            geodeSpot = new ClickableComponent(
                new Rectangle(xPositionOnScreen + spaceToClearSideBorder + borderWidth / 2,
                    yPositionOnScreen + spaceToClearTopBorder + 4, 560, 308), "")
            { myID = 998, downNeighborID = 0 };
            clint = new AnimatedSprite(content.ModContent.GetInternalAssetName("assets/empty.png").ToString(), 8, 32, 48);
            if (inventory.inventory != null && inventory.inventory.Count >= 12)
                for (var index = 0; index < 12; ++index)
                    if (inventory.inventory[index] != null)
                        inventory.inventory[index].upNeighborID = 998;

            if (trashCan != null) trashCan.myID = 106;
            if (okButton != null) okButton.leftNeighborID = 11;
            if (Game1.options.SnappyMenus)
            {
                populateClickableComponentList();
                snapToDefaultClickableComponent();
            }
        }

        public override void snapToDefaultClickableComponent()
        {
            currentlySnappedComponent = getComponentWithID(0);
            snapCursorToCurrentSnappedComponent();
        }

        public override bool readyToClose()
        {
            return base.readyToClose() && geodeAnimationTimer <= 0 && heldItem == null &&
                   !waitingForServerResponse;
        }


        public bool highlightGeodes(Item i)
        {
            return heldItem != null || Utility.IsGeode(i);
        }

        private void startGeodeCrack()
        {
            geodeSpot.item = heldItem.getOne();
            --heldItem.Stack;
            if (heldItem.Stack <= 0)
                heldItem = null;

            geodeAnimationTimer = 300;
            Game1.playSound("stoneStep");
            clint.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>
            {
                new(8, 24),
                new(9, 24),
                new(10, 24),
                new(11, 24),
                new(12, 24),
                new(8, 24)
            });
            clint.loop = false;
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (waitingForServerResponse)
                return;
            base.receiveLeftClick(x, y);
            if (!geodeSpot.containsPoint(x, y))
                return;
            if (heldItem == null || !Utility.IsGeode(heldItem) ||
                geodeAnimationTimer > 0) return;
            if (Game1.player.freeSpotsInInventory() > 1 ||
                (Game1.player.freeSpotsInInventory() == 1 && heldItem.Stack == 1))
            {
                if (heldItem.QualifiedItemId == "(O)791" && !Game1.netWorldState.Value.GoldenCoconutCracked)
                {
                    waitingForServerResponse = true;
                    Game1.player.team.goldenCoconutMutex.RequestLock(() =>
                    {
                        waitingForServerResponse = false;
                        geodeTreasureOverride = ItemRegistry.Create("(O)73");
                        startGeodeCrack();
                    }, () =>
                    {
                        waitingForServerResponse = false;
                        startGeodeCrack();
                    });
                }
                else
                {
                    startGeodeCrack();
                }
            }
            else
            {
                descriptionText = Game1.content.LoadString("Strings\\UI:GeodeMenu_InventoryFull");
                wiggleWordsTimer = 500;
                alertTimer = 1500;
            }
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            base.receiveRightClick(x, y);
        }

        public override void performHoverAction(int x, int y)
        {
            if (alertTimer > 0) return;
            base.performHoverAction(x, y);
            if (!descriptionText.Equals("")) return;
            descriptionText =
                "Place geodes or artifact troves in the box on the left to break them! Keep the button pressed to break them continuously.";
        }

        public override void emergencyShutDown()
        {
            base.emergencyShutDown();
            if (heldItem == null) return;
            Game1.player.addItemToInventoryBool(heldItem);
        }

        public override void update(GameTime time)
        {
            base.update(time);
            for (var index = fluffSprites.Count - 1; index >= 0; --index)
                if (fluffSprites[index].update(time))
                    fluffSprites.RemoveAt(index);
            if (alertTimer > 0)
                alertTimer -= time.ElapsedGameTime.Milliseconds;
            if (geodeAnimationTimer <= 0)
                return;
            Game1.MusicDuckTimer = 1500f;
            geodeAnimationTimer -= time.ElapsedGameTime.Milliseconds;
            if (geodeAnimationTimer <= 0)
            {
                geodeDestructionAnimation = null;
                geodeSpot.item = null;
                if (geodeTreasure?.QualifiedItemId == "(O)73")
                {
                    Game1.netWorldState.Value.GoldenCoconutCracked = true;
                }
                Game1.player.addItemToInventoryBool(geodeTreasure);
                geodeTreasure = null;
                yPositionOfGem = 0;
                fluffSprites.Clear();
                delayBeforeShowArtifactTimer = 0f;
                return;
            }
            else
            {
                int currentFrame = clint.currentFrame;
                clint.animateOnce(time);
                if (clint.currentFrame == 11 && currentFrame != 11)
                {
                    if (geodeSpot.item?.QualifiedItemId == "(O)275" || geodeSpot.item?.QualifiedItemId == "(O)MysteryBox" || geodeSpot.item?.QualifiedItemId == "(O)GoldenMysteryBox")
                    {
                        Game1.playSound("hammer");
                        Game1.playSound("woodWhack");
                    }
                    else
                    {
                        Game1.playSound("hammer");
                        Game1.playSound("stoneCrack");
                    }

                    ++Game1.stats.GeodesCracked;
                    if (geodeSpot.item?.QualifiedItemId == "(O)MysteryBox" || geodeSpot.item?.QualifiedItemId == "(O)GoldenMysteryBox")
                    {
                        Game1.stats.Increment("MysteryBoxesOpened");
                    }
                    var geodeDestructionYOffset = 448;
                    if (geodeSpot.item != null)
                    {
                        string qualifiedItemId = geodeSpot.item.QualifiedItemId;
                        if (!(qualifiedItemId == "(O)536"))
                        {
                            if (qualifiedItemId == "(O)537")
                            {
                                geodeDestructionYOffset += 128;
                            }
                        }
                        else
                        {
                            geodeDestructionYOffset += 64;
                        }
                        geodeDestructionAnimation = new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, geodeDestructionYOffset, 64, 64), 100f, 8, 0, new Vector2(geodeSpot.bounds.X + 392 - 32, geodeSpot.bounds.Y + 192 - 32), flicker: false, flipped: false);
                        if (geodeSpot.item?.QualifiedItemId == "(O)275")
                        {
                            geodeDestructionAnimation = new TemporaryAnimatedSprite
                            {
                                texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites//temporary_sprites_1"),
                                sourceRect = new Rectangle(388, 123, 18, 21),
                                sourceRectStartingPos = new Vector2(388f, 123f),
                                animationLength = 6,
                                position = new Vector2(geodeSpot.bounds.X + 380 - 32, geodeSpot.bounds.Y + 192 - 32),
                                holdLastFrame = true,
                                interval = 100f,
                                id = 777,
                                scale = 4f
                            };
                            for (int j = 0; j < 6; j++)
                            {
                                fluffSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(372, 1956, 10, 10), new Vector2(geodeSpot.bounds.X + 392 - 32 + Game1.random.Next(21), geodeSpot.bounds.Y + 192 - 16), flipped: false, 0.002f, new Color(255, 222, 198))
                                {
                                    alphaFade = 0.02f,
                                    motion = new Vector2((float)Game1.random.Next(-20, 21) / 10f, (float)Game1.random.Next(5, 20) / 10f),
                                    interval = 99999f,
                                    layerDepth = 0.9f,
                                    scale = 3f,
                                    scaleChange = 0.01f,
                                    rotationChange = (float)Game1.random.Next(-5, 6) * (float)Math.PI / 256f,
                                    delayBeforeAnimationStart = j * 20
                                });
                                fluffSprites.Add(new TemporaryAnimatedSprite
                                {
                                    texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites//temporary_sprites_1"),
                                    sourceRect = new Rectangle(499, 132, 5, 5),
                                    sourceRectStartingPos = new Vector2(499f, 132f),
                                    motion = new Vector2((float)Game1.random.Next(-30, 31) / 10f, Game1.random.Next(-7, -4)),
                                    acceleration = new Vector2(0f, 0.25f),
                                    totalNumberOfLoops = 1,
                                    interval = 1000f,
                                    alphaFade = 0.015f,
                                    animationLength = 1,
                                    layerDepth = 1f,
                                    scale = 4f,
                                    rotationChange = (float)Game1.random.Next(-5, 6) * (float)Math.PI / 256f,
                                    delayBeforeAnimationStart = j * 10,
                                    position = new Vector2(geodeSpot.bounds.X + 392 - 32 + Game1.random.Next(21), geodeSpot.bounds.Y + 192 - 16)
                                });
                                delayBeforeShowArtifactTimer = 500f;
                            }
                        }
                        else if (geodeSpot.item?.QualifiedItemId == "(O)MysteryBox" || geodeSpot.item?.QualifiedItemId == "(O)GoldenMysteryBox")
                        {
                            geodeDestructionAnimation = new TemporaryAnimatedSprite
                            {
                                texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\Cursors_1_6"),
                                sourceRect = new Rectangle((geodeSpot.item?.QualifiedItemId == "(O)GoldenMysteryBox") ? 256 : 0, 27, 24, 24),
                                sourceRectStartingPos = new Vector2((geodeSpot.item?.QualifiedItemId == "(O)GoldenMysteryBox") ? 256 : 0, 27f),
                                animationLength = 8,
                                position = new Vector2(geodeSpot.bounds.X + 380 - 48, geodeSpot.bounds.Y + 192 - 48),
                                holdLastFrame = true,
                                interval = 100f,
                                id = 777,
                                scale = 4f
                            };
                            for (int i = 0; i < 6; i++)
                            {
                                fluffSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(372, 1956, 10, 10), new Vector2(geodeSpot.bounds.X + 392 - 48 + Game1.random.Next(32), geodeSpot.bounds.Y + 192 - 24), flipped: false, 0.002f, new Color(255, 222, 198))
                                {
                                    alphaFade = 0.02f,
                                    motion = new Vector2((float)Game1.random.Next(-20, 21) / 10f, (float)Game1.random.Next(5, 20) / 10f),
                                    interval = 99999f,
                                    layerDepth = 0.9f,
                                    scale = 3f,
                                    scaleChange = 0.01f,
                                    rotationChange = (float)Game1.random.Next(-5, 6) * (float)Math.PI / 256f,
                                    delayBeforeAnimationStart = i * 20
                                });
                                int which = Game1.random.Next(3);
                                fluffSprites.Add(new TemporaryAnimatedSprite
                                {
                                    texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\Cursors_1_6"),
                                    sourceRect = new Rectangle(((geodeSpot.item?.QualifiedItemId == "(O)GoldenMysteryBox") ? 15 : 0) + which * 5, 52, 5, 5),
                                    sourceRectStartingPos = new Vector2(which * 5, 75f),
                                    motion = new Vector2((float)Game1.random.Next(-30, 31) / 10f, Game1.random.Next(-7, -4)),
                                    acceleration = new Vector2(0f, 0.25f),
                                    totalNumberOfLoops = 1,
                                    interval = 1000f,
                                    alphaFade = 0.015f,
                                    animationLength = 1,
                                    layerDepth = 1f,
                                    scale = 4f,
                                    rotationChange = (float)Game1.random.Next(-5, 6) * (float)Math.PI / 256f,
                                    delayBeforeAnimationStart = i * 10,
                                    position = new Vector2(geodeSpot.bounds.X + 392 - 48 + Game1.random.Next(32), geodeSpot.bounds.Y + 192 - 24)
                                });
                                delayBeforeShowArtifactTimer = 500f;
                            }
                        }
                        if (geodeTreasureOverride != null)
                        {
                            geodeTreasure = geodeTreasureOverride;
                            geodeTreasureOverride = null;
                        }
                        else
                        {
                            geodeTreasure = Utility.getTreasureFromGeode(geodeSpot.item);
                        }
                        if (!(geodeSpot.item.QualifiedItemId == "(O)275") && (!(geodeTreasure is StardewValley.Object mineral) || !(mineral.Type == "Minerals")) && geodeTreasure is StardewValley.Object artifact && artifact.Type == "Arch" && !Game1.player.hasOrWillReceiveMail("artifactFound"))
                        {
                            geodeTreasure = ItemRegistry.Create("(O)390", 5);
                        }
                    }
                }

                if (geodeDestructionAnimation != null &&
                    ((geodeDestructionAnimation.id != 777f &&
                      geodeDestructionAnimation.currentParentTileIndex < 7) ||
                     (geodeDestructionAnimation.id == 777f &&
                      geodeDestructionAnimation.currentParentTileIndex < 5)))
                {
                    geodeDestructionAnimation.update(time);
                    if (delayBeforeShowArtifactTimer > 0f)
                    {
                        delayBeforeShowArtifactTimer -= (float)time.ElapsedGameTime.TotalMilliseconds;
                        if (delayBeforeShowArtifactTimer <= 0f)
                        {
                            fluffSprites.Add(geodeDestructionAnimation);
                            fluffSprites.Reverse();
                            geodeDestructionAnimation = new TemporaryAnimatedSprite
                            {
                                interval = 100f,
                                animationLength = 6,
                                alpha = 0.001f,
                                id = 777
                            };
                        }
                    }
                    else
                    {
                        if (geodeDestructionAnimation.currentParentTileIndex < 3)
                            --yPositionOfGem;
                        --yPositionOfGem;
                        if (geodeDestructionAnimation.currentParentTileIndex == 7 || (geodeDestructionAnimation.id == 777 && geodeDestructionAnimation.currentParentTileIndex == 5))
                        {
                            if (!(geodeTreasure is StardewValley.Object treasure) || treasure.Price > 75 || geodeSpot.item?.QualifiedItemId == "(O)MysteryBox" || geodeSpot.item?.QualifiedItemId == "(O)GoldenMysteryBox")
                            {
                                sparkle = new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 640, 64, 64), 100f, 8, 0, new Vector2(geodeSpot.bounds.X + ((geodeSpot.item.itemId.Value == "MysteryBox") ? 94 : 98) * 4 - 32, geodeSpot.bounds.Y + 192 + yPositionOfGem - 32), flicker: false, flipped: false);
                                Game1.playSound("discoverMineral");
                            }
                            else
                            {
                                Game1.playSound("newArtifact");
                            }
                        }
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
            geodeSpot = new ClickableComponent(new Rectangle(base.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth / 2, base.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 4, 560, 308), "Anvil");
            int yPositionForInventory = base.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth + 192 - 16 + 128 + 4;
            base.inventory = new InventoryMenu(base.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth / 2 + 12, yPositionForInventory, playerInventory: false, null, base.inventory.highlightMethod);
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);
            draw(b);
            Game1.dayTimeMoneyBox.drawMoneyBox(b);
            b.Draw(content.ModContent.Load<Texture2D>("assets/bgpatch.png"), new Vector2(geodeSpot.bounds.X, geodeSpot.bounds.Y),
                new Rectangle(0, 0, 140, 78), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
            if (geodeSpot.item != null)
            {
                if (geodeDestructionAnimation == null)
                {
                    Vector2 offset = Vector2.Zero;
                    if (geodeSpot.item.QualifiedItemId == "(O)275")
                    {
                        offset = new Vector2(-2f, 2f);
                    }
                    else if (geodeSpot.item.QualifiedItemId == "(O)MysteryBox" || geodeSpot.item?.QualifiedItemId == "(O)GoldenMysteryBox")
                    {
                        offset = new Vector2(-7f, 4f);
                    }
                    _ = geodeSpot.item.QualifiedItemId == "(O)275";
                    geodeSpot.item.drawInMenu(b, new Vector2(geodeSpot.bounds.X + 360, geodeSpot.bounds.Y + 160) + offset, 1f);
                }
                else
                {
                    geodeDestructionAnimation.draw(b, localPosition: true);
                }
                foreach (TemporaryAnimatedSprite fluffSprite in fluffSprites)
                {
                    fluffSprite.draw(b, localPosition: true);
                }
                if (geodeTreasure != null && delayBeforeShowArtifactTimer <= 0f)
                {
                    geodeTreasure.drawInMenu(b, new Vector2(geodeSpot.bounds.X + (geodeSpot.item.QualifiedItemId.Contains("MysteryBox") ? 86 : 90) * 4, geodeSpot.bounds.Y + 160 + yPositionOfGem), 1f);
                }
                sparkle?.draw(b, localPosition: true);
            }
            clint.draw(b,
                new Vector2(geodeSpot.bounds.X + 384, geodeSpot.bounds.Y + 64), 0.877f);
            if (!hoverText.Equals(""))
                drawHoverText(b, hoverText, Game1.smallFont);
            if (heldItem != null)
                heldItem.drawInMenu(b,
                    new Vector2(Game1.getOldMouseX() + 8, Game1.getOldMouseY() + 8), 1f);
            if (Game1.options.hardwareCursor)
                return;
            drawMouse(b);
        }
    }
}