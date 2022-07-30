/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/Personal-Anvil
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace PersonalAnvil
{
    public sealed class WorkbenchGeodeMenu : MenuWithInventory
    {
        public const int region_geodeSpot = 998;
        private readonly AnimatedSprite clint;
        private readonly IModContentHelper content;
        private readonly List<TemporaryAnimatedSprite> fluffSprites = new List<TemporaryAnimatedSprite>();
        private int alertTimer;
        private float delayBeforeShowArtifactTimer;
        public int geodeAnimationTimer;
        private TemporaryAnimatedSprite geodeDestructionAnimation;
        public ClickableComponent geodeSpot;
        private Item geodeTreasure;
        private Item geodeTreasureOverride;
        private TemporaryAnimatedSprite sparkle;
        private bool waitingForServerResponse;
        private int yPositionOfGem;

        public WorkbenchGeodeMenu(IModContentHelper content) : base(null, true, true, 12, 132)
        {
            this.content = content;
            if (yPositionOnScreen == borderWidth + spaceToClearTopBorder)
                movePosition(0, -spaceToClearTopBorder);
            inventory.highlightMethod = highlightGeodes;
            geodeSpot = new ClickableComponent(
                new Rectangle(xPositionOnScreen + spaceToClearSideBorder + borderWidth / 2,
                    yPositionOnScreen + spaceToClearTopBorder + 4, 560, 308), "") { myID = 998, downNeighborID = 0 };
            clint = new AnimatedSprite(content.GetInternalAssetName("assets/Empty.png").ToString(), 8, 32, 48);
            if (inventory.inventory != null && inventory.inventory.Count >= 12)
                for (var index = 0; index < 12; ++index)
                    if (inventory.inventory[index] != null)
                        inventory.inventory[index].upNeighborID = 998;

            if (trashCan != null) trashCan.myID = 106;
            if (okButton != null) okButton.leftNeighborID = 11;
            if (!Game1.options.SnappyMenus) return;
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
                new FarmerSprite.AnimationFrame(8, 24),
                new FarmerSprite.AnimationFrame(9, 24),
                new FarmerSprite.AnimationFrame(10, 24),
                new FarmerSprite.AnimationFrame(11, 24),
                new FarmerSprite.AnimationFrame(12, 24),
                new FarmerSprite.AnimationFrame(8, 24)
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
            if (heldItem == null || !Utility.IsGeode(heldItem) || Game1.player.Money < 25 ||
                geodeAnimationTimer > 0) return;
            if (Game1.player.freeSpotsInInventory() > 1 ||
                (Game1.player.freeSpotsInInventory() == 1 && heldItem.Stack == 1))
            {
                if (heldItem.QualifiedItemID == "(O)791" && !Game1.netWorldState.Value.GoldenCoconutCracked.Value)
                {
                    waitingForServerResponse = true;
                    Game1.player.team.goldenCoconutMutex.RequestLock(() =>
                    {
                        waitingForServerResponse = false;
                        geodeTreasureOverride = new Object(73, 1);
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
            Game1.changeMusicTrack("none");
            geodeAnimationTimer -= time.ElapsedGameTime.Milliseconds;
            if (geodeAnimationTimer <= 0)
            {
                geodeDestructionAnimation = null;
                geodeSpot.item = null;
                if (geodeTreasure != null && geodeTreasure.QualifiedItemID == "(O)128")
                    Game1.netWorldState.Value.GoldenCoconutCracked.Value = true;
                Game1.player.addItemToInventoryBool(geodeTreasure);
                geodeTreasure = null;
                yPositionOfGem = 0;
                fluffSprites.Clear();
                delayBeforeShowArtifactTimer = 0.0f;
            }
            else
            {
                var currentFrame = clint.currentFrame;
                clint.animateOnce(time);
                if (clint.currentFrame == 11 && currentFrame != 11)
                {
                    if (geodeSpot.item != null && geodeSpot.item.QualifiedItemID == "(O)275")
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
                    var y = 448;
                    if (geodeSpot.item != null)
                    {
                        var qualifiedItemId = geodeSpot.item.QualifiedItemID;
                        if (!(qualifiedItemId == "(O)536"))
                        {
                            if (qualifiedItemId == "(O)537")
                                y += 128;
                        }
                        else
                        {
                            y += 64;
                        }

                        geodeDestructionAnimation = new TemporaryAnimatedSprite("TileSheets\\animations",
                            new Rectangle(0, y, 64, 64), 100f, 8, 0,
                            new Vector2(geodeSpot.bounds.X + 392 - 32, geodeSpot.bounds.Y + 192 - 32), false, false);
                        if (geodeSpot.item?.QualifiedItemID == "(O)275")
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
                                id = 777f,
                                scale = 4f
                            };
                            for (var index = 0; index < 6; ++index)
                            {
                                fluffSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors",
                                    new Rectangle(372, 1956, 10, 10),
                                    new Vector2(geodeSpot.bounds.X + 392 - 32 + Game1.random.Next(21),
                                        geodeSpot.bounds.Y + 192 - 16), false, 1f / 500f,
                                    new Color(byte.MaxValue, 222, 198))
                                {
                                    alphaFade = 0.02f,
                                    motion = new Vector2(Game1.random.Next(-20, 21) / 10f,
                                        Game1.random.Next(5, 20) / 10f),
                                    interval = 99999f,
                                    layerDepth = 0.9f,
                                    scale = 3f,
                                    scaleChange = 0.01f,
                                    rotationChange = (float)(Game1.random.Next(-5, 6) * 3.1415927410125732 / 256.0),
                                    delayBeforeAnimationStart = index * 20
                                });
                                fluffSprites.Add(new TemporaryAnimatedSprite
                                {
                                    texture = Game1.temporaryContent.Load<Texture2D>(
                                        "LooseSprites//temporary_sprites_1"),
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
                                    rotationChange = (float)(Game1.random.Next(-5, 6) * 3.1415927410125732 / 256.0),
                                    delayBeforeAnimationStart = index * 10,
                                    position = new Vector2(geodeSpot.bounds.X + 392 - 32 + Game1.random.Next(21),
                                        geodeSpot.bounds.Y + 192 - 16)
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

                        if (geodeSpot.item?.QualifiedItemID != "(O)275" &&
                            (!(geodeTreasure is Object) || !(geodeTreasure as Object).Type.Contains("Mineral")) &&
                            geodeTreasure is Object && (geodeTreasure as Object).Type.Contains("Arch") &&
                            !Game1.player.hasOrWillReceiveMail("artifactFound"))
                            geodeTreasure = new Object(390, 5);
                    }
                }

                if (geodeDestructionAnimation != null &&
                    ((geodeDestructionAnimation.id != 777.0 && geodeDestructionAnimation.currentParentTileIndex < 7) ||
                     (geodeDestructionAnimation.id == 777.0 && geodeDestructionAnimation.currentParentTileIndex < 5)))
                {
                    geodeDestructionAnimation.update(time);
                    if (delayBeforeShowArtifactTimer > 0.0)
                    {
                        delayBeforeShowArtifactTimer -= (float)time.ElapsedGameTime.TotalMilliseconds;
                        if (delayBeforeShowArtifactTimer <= 0.0)
                        {
                            fluffSprites.Add(geodeDestructionAnimation);
                            fluffSprites.Reverse();
                            geodeDestructionAnimation = new TemporaryAnimatedSprite
                            {
                                interval = 100f,
                                animationLength = 6,
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
                        if ((geodeDestructionAnimation.currentParentTileIndex == 7 ||
                             (geodeDestructionAnimation.id == 777.0 &&
                              geodeDestructionAnimation.currentParentTileIndex == 5)) && (!(geodeTreasure is Object) ||
                                (geodeTreasure as Object).Price > 75))
                        {
                            sparkle = new TemporaryAnimatedSprite("TileSheets\\animations",
                                new Rectangle(0, 640, 64, 64), 100f, 8, 0,
                                new Vector2(geodeSpot.bounds.X + 392 - 32,
                                    geodeSpot.bounds.Y + 192 + yPositionOfGem - 32), false, false);
                            Game1.playSound("discoverMineral");
                        }
                        else if ((geodeDestructionAnimation.currentParentTileIndex == 7 ||
                                  (geodeDestructionAnimation.id == 777.0 &&
                                   geodeDestructionAnimation.currentParentTileIndex == 5)) && geodeTreasure is Object &&
                                 (geodeTreasure as Object).Price <= 75)
                        {
                            Game1.playSound("newArtifact");
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
            geodeSpot = new ClickableComponent(
                new Rectangle(
                    xPositionOnScreen + spaceToClearSideBorder + borderWidth / 2,
                    yPositionOnScreen + spaceToClearTopBorder + 4, 560, 308), "Anvil");
            var yPosition = yPositionOnScreen + spaceToClearTopBorder + borderWidth +
                192 - 16 + 128 + 4;
            inventory =
                new InventoryMenu(
                    xPositionOnScreen + spaceToClearSideBorder + borderWidth / 2 +
                    12, yPosition, false, highlightMethod: inventory.highlightMethod);
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);
            draw(b);
            Game1.dayTimeMoneyBox.drawMoneyBox(b);
            b.Draw(content.Load<Texture2D>("assets/bgpatch.png"), new Vector2(geodeSpot.bounds.X, geodeSpot.bounds.Y),
                new Rectangle(0, 0, 140, 78), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
            if (geodeSpot.item != null)
            {
                if (geodeDestructionAnimation == null)
                {
                    var flag = geodeSpot.item.QualifiedItemID == "(O)275";
                    geodeSpot.item.drawInMenu(b,
                        new Vector2(geodeSpot.bounds.X + 360 + (flag ? -8 : 0),
                            geodeSpot.bounds.Y + 160 + (flag ? 8 : 0)), 1f);
                }
                else
                {
                    geodeDestructionAnimation.draw(b, true);
                }

                foreach (var fluffSprite in fluffSprites)
                    fluffSprite.draw(b, true);
                if (geodeTreasure != null && delayBeforeShowArtifactTimer <= 0.0)
                    geodeTreasure.drawInMenu(b,
                        new Vector2(geodeSpot.bounds.X + 360,
                            geodeSpot.bounds.Y + 160 + yPositionOfGem), 1f);
                if (sparkle != null)
                    sparkle.draw(b, true);
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