/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/MultipleMiniObelisks
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MultipleMiniObelisks.Multiplayer;
using MultipleMiniObelisks.Objects;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewValley.Quests;

namespace MultipleMiniObelisks.UI
{
    public class TeleportMenu : IClickableMenu
    {
        // Constants
        public const int obelisksPerPage = 6;
        public const int region_forwardButton = 101;
        public const int region_backButton = 102;

        // UI related
        private List<List<MiniObelisk>> pages;
        public List<ClickableComponent> teleportDestinationButtons;
        public List<ClickableTextureComponent> renameObeliskButtons;
        public ClickableTextureComponent forwardButton;
        public ClickableTextureComponent backButton;
        protected float _contentHeight;
        private string hoverText = "";

        // Logic related
        private int currentPage;
        private int questPage = -1;

        private List<MiniObelisk> miniObelisks = new List<MiniObelisk>();
        private StardewValley.Object sourceObelisk;

        public TeleportMenu(StardewValley.Object sourceObelisk, List<MiniObelisk> miniObelisks) : base(0, 0, 0, 0, showUpperRightCloseButton: true)
        {
            this.sourceObelisk = sourceObelisk;
            this.miniObelisks = miniObelisks;

            Game1.playSound("bigSelect");
            this.PaginateObelisks();
            base.width = 832;
            base.height = 576;

            if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.fr)
            {
                base.height += 64;
            }
            Vector2 topLeft = Utility.getTopLeftPositionForCenteringOnScreen(base.width, base.height);
            base.xPositionOnScreen = (int)topLeft.X;
            base.yPositionOnScreen = (int)topLeft.Y + 32;

            // Create the buttons used for teleporting and renaming
            this.teleportDestinationButtons = new List<ClickableComponent>();
            this.renameObeliskButtons = new List<ClickableTextureComponent>();

            for (int i = 0; i < 6; i++)
            {
                ClickableComponent teleportButton = new ClickableComponent(new Rectangle(base.xPositionOnScreen + 16, base.yPositionOnScreen + 16 + i * ((base.height - 32) / 6), base.width - 32, (base.height - 32) / 6 + 4), string.Concat(i))
                {
                    myID = i,
                    downNeighborID = -7777,
                    upNeighborID = ((i > 0) ? (i - 1) : (-1)),
                    rightNeighborID = i + 103,
                    leftNeighborID = -7777,
                    fullyImmutable = true
                };
                this.teleportDestinationButtons.Add(teleportButton);

                this.renameObeliskButtons.Add(new ClickableTextureComponent(new Rectangle(teleportButton.bounds.Right - teleportButton.bounds.Width / 8, teleportButton.bounds.Y + teleportButton.bounds.Height / 4, 56, 48), Game1.mouseCursors, new Rectangle(66, 4, 14, 12), 4f)
                {
                    myID = i + 103,
                    downNeighborID = -7777,
                    upNeighborID = ((i > 0) ? (i + 103 - 1) : (-1)),
                    rightNeighborID = -7777,
                    leftNeighborID = i
                });
            }

            base.upperRightCloseButton = new ClickableTextureComponent(new Rectangle(base.xPositionOnScreen + base.width - 20, base.yPositionOnScreen - 8, 48, 48), Game1.mouseCursors, new Rectangle(337, 494, 12, 12), 4f);
            this.backButton = new ClickableTextureComponent(new Rectangle(base.xPositionOnScreen - 64, base.yPositionOnScreen + 8, 48, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f)
            {
                myID = 102,
                rightNeighborID = -7777
            };
            this.forwardButton = new ClickableTextureComponent(new Rectangle(base.xPositionOnScreen + base.width + 64 - 48, base.yPositionOnScreen + base.height - 48, 48, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f)
            {
                myID = 101
            };

            if (Game1.options.SnappyMenus)
            {
                base.populateClickableComponentList();
                this.snapToDefaultClickableComponent();
            }
        }

        private void EnsureModDataKeyIsPresent()
        {
            // Not doing miniObelisks.Count - 1 so to avoid "... #0"
            int count = miniObelisks.Count;
            foreach (MiniObelisk obelisk in miniObelisks)
            {
                if (String.IsNullOrEmpty(obelisk.CustomName))
                {
                    obelisk.CustomName = string.Concat("Mini-Obelisk #", count);
                }

                count--;
            }
        }

        public void PaginateObelisks()
        {
            // Ensure all mini-obelisks have the required modData
            EnsureModDataKeyIsPresent();

            int count = miniObelisks.Count - 1;
            this.pages = new List<List<MiniObelisk>>();
            foreach (MiniObelisk obelisk in miniObelisks.OrderBy(o => o.CustomName))
            {
                if (obelisk is null)
                {
                    continue;
                }

                int which2 = miniObelisks.Count - 1 - count;
                while (this.pages.Count <= which2 / 6)
                {
                    this.pages.Add(new List<MiniObelisk>());
                }
                this.pages[which2 / 6].Add(obelisk);

                count--;
            }

            if (this.pages.Count == 0)
            {
                this.pages.Add(new List<MiniObelisk>());
            }
            this.currentPage = Math.Min(Math.Max(this.currentPage, 0), this.pages.Count - 1);
            this.questPage = -1;
        }

        private void AttemptTeleport(Farmer who, MiniObelisk obelisk)
        {
            if (obelisk.Tile == sourceObelisk.TileLocation && obelisk.LocationName == who.currentLocation.NameOrUniqueName)
            {
                Game1.showRedMessage("You're already there!");
                Game1.playSound("bigDeSelect");
                return;
            }

            if (!Context.IsMainPlayer)
            {
                var teleportRequestMessage = new ObeliskTeleportRequestMessage(obelisk, who.UniqueMultiplayerID);
                ModEntry.helper.Multiplayer.SendMessage(teleportRequestMessage, nameof(ObeliskTeleportRequestMessage), modIDs: new[] { ModEntry.manifest.UniqueID });
                return;
            }

            Vector2 target = obelisk.Tile;
            GameLocation obeliskLocation = Game1.getLocationFromName(obelisk.LocationName);
            foreach (Vector2 v in new List<Vector2>
            {
                new Vector2(target.X, target.Y + 1f),
                new Vector2(target.X - 1f, target.Y),
                new Vector2(target.X + 1f, target.Y),
                new Vector2(target.X, target.Y - 1f)
            })
            {
                if (obeliskLocation.isTileLocationTotallyClearAndPlaceableIgnoreFloors(v))
                {
                    for (int i = 0; i < 12; i++)
                    {
                        who.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(354, Game1.random.Next(25, 75), 6, 1, new Vector2(Game1.random.Next((int)who.position.X - 256, (int)who.position.X + 192), Game1.random.Next((int)who.position.Y - 256, (int)who.position.Y + 192)), flicker: false, (Game1.random.NextDouble() < 0.5) ? true : false));
                    }
                    who.currentLocation.playSound("wand");
                    Game1.displayFarmer = false;
                    who.temporarilyInvincible = true;
                    who.temporaryInvincibilityTimer = -2000;
                    who.Halt();
                    who.faceDirection(2);
                    who.CanMove = false;
                    who.freezePause = 2000;
                    Game1.flashAlpha = 1f;
                    DelayedAction.fadeAfterDelay(delegate
                    {
                        Game1.warpFarmer(obeliskLocation.NameOrUniqueName, (int)v.X, (int)v.Y, flip: false);
                        if (!Game1.isStartingToGetDarkOut() && !Game1.isRaining)
                        {
                            Game1.playMorningSong();
                        }
                        else
                        {
                            Game1.changeMusicTrack("none");
                        }
                        Game1.fadeToBlackAlpha = 0.99f;
                        Game1.screenGlow = false;
                        who.temporarilyInvincible = false;
                        who.temporaryInvincibilityTimer = 0;
                        Game1.displayFarmer = true;
                        who.CanMove = true;
                    }, 1000);
                    new Rectangle(who.GetBoundingBox().X, who.GetBoundingBox().Y, 64, 64).Inflate(192, 192);
                    int j = 0;
                    for (int xTile = who.getTileX() + 8; xTile >= who.getTileX() - 8; xTile--)
                    {
                        obeliskLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, new Vector2(xTile, who.getTileY()) * 64f, Color.White, 8, flipped: false, 50f)
                        {
                            layerDepth = 1f,
                            delayBeforeAnimationStart = j * 25,
                            motion = new Vector2(-0.25f, 0f)
                        });
                        j++;
                    }

                    return;
                }
            }

            Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:MiniObelisk_NeedsSpace"));
            return;
        }
        protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
        {
            if (oldID >= 0 && oldID != 101 && oldID != 102)
            {
                switch (direction)
                {
                    case 2:
                        if (oldID < 5 && this.pages[this.currentPage].Count - 1 > oldID)
                        {
                            base.currentlySnappedComponent = base.getComponentWithID(oldID + 1);
                        }
                        else if (oldID > 6 && oldID < 108 && this.pages[this.currentPage].Count - 1 + 103 > oldID)
                        {
                            base.currentlySnappedComponent = base.getComponentWithID(oldID + 1);
                        }
                        break;
                    case 1:
                        if (this.currentPage < this.pages.Count - 1 && oldID > 102)
                        {
                            base.currentlySnappedComponent = base.getComponentWithID(101);
                            base.currentlySnappedComponent.leftNeighborID = oldID;
                        }
                        else if (oldID < 6)
                        {
                            base.currentlySnappedComponent = base.getComponentWithID(oldID + 103);
                            base.currentlySnappedComponent.leftNeighborID = oldID;
                        }
                        break;
                    case 3:
                        if (this.currentPage > 0)
                        {
                            base.currentlySnappedComponent = base.getComponentWithID(102);
                            base.currentlySnappedComponent.rightNeighborID = oldID;
                        }
                        break;
                }
            }
            else if (oldID == 102)
            {
                if (this.questPage != -1)
                {
                    return;
                }
                base.currentlySnappedComponent = base.getComponentWithID(0);
            }
            this.snapCursorToCurrentSnappedComponent();
        }

        public override void snapToDefaultClickableComponent()
        {
            base.currentlySnappedComponent = base.getComponentWithID(0);
            this.snapCursorToCurrentSnappedComponent();
        }

        public override void receiveGamePadButton(Buttons b)
        {
            if (b == Buttons.RightTrigger && this.questPage == -1 && this.currentPage < this.pages.Count - 1)
            {
                this.pageForwardButton();
            }
            else if (b == Buttons.LeftTrigger && this.questPage == -1 && this.currentPage > 0)
            {
                this.pageBackButton();
            }
        }

        public override void receiveScrollWheelAction(int direction)
        {
            base.receiveScrollWheelAction(direction);
            if (direction > 0 && currentPage > 0)
            {
                this.currentPage--;
                Game1.playSound("shiny4");
            }
            else if (direction < 0 && currentPage < pages.Count - 1)
            {
                this.currentPage++;
                Game1.playSound("shiny4");
            }
        }

        public override void performHoverAction(int x, int y)
        {
            this.hoverText = "";
            base.performHoverAction(x, y);
            if (this.questPage == -1)
            {
                for (int i = 0; i < this.teleportDestinationButtons.Count; i++)
                {
                    if (this.pages.Count > 0 && this.pages[0].Count > i && this.renameObeliskButtons[i].containsPoint(x, y))
                    {
                        this.hoverText = "Rename Obelisk";
                    }
                }
            }

            this.forwardButton.tryHover(x, y, 0.2f);
            this.backButton.tryHover(x, y, 0.2f);
        }

        public override void receiveKeyPress(Keys key)
        {
            base.receiveKeyPress(key);
            if (Game1.options.doesInputListContain(Game1.options.cancelButton, key) && this.readyToClose())
            {
                Game1.exitActiveMenu();
                Game1.playSound("bigDeSelect");
            }
        }

        private void pageForwardButton()
        {
            this.currentPage++;
            Game1.playSound("shwip");
            if (Game1.options.SnappyMenus && this.currentPage == this.pages.Count - 1)
            {
                base.currentlySnappedComponent = base.getComponentWithID(0);
                this.snapCursorToCurrentSnappedComponent();
            }
        }

        private void pageBackButton()
        {
            this.currentPage--;
            Game1.playSound("shwip");
            if (Game1.options.SnappyMenus && this.currentPage == 0)
            {
                base.currentlySnappedComponent = base.getComponentWithID(0);
                this.snapCursorToCurrentSnappedComponent();
            }
        }

        public override void releaseLeftClick(int x, int y)
        {
            if (!GameMenu.forcePreventClose)
            {
                base.releaseLeftClick(x, y);
            }
        }

        public override void applyMovementKey(int direction)
        {
            base.applyMovementKey(direction);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = false)
        {
            base.receiveLeftClick(x, y, playSound);
            if (Game1.activeClickableMenu == null)
            {
                return;
            }

            for (int i = 0; i < this.teleportDestinationButtons.Count; i++)
            {
                if (!(this.pages.Count > 0 && this.pages[this.currentPage].Count > i))
                {
                    continue;
                }

                if (this.renameObeliskButtons[i].containsPoint(x, y))
                {
                    this.hoverText = "";
                    Game1.activeClickableMenu = new RenameMenu(this, "Rename the Obelisk", this.pages[this.currentPage][i]);
                }
                else if (this.teleportDestinationButtons[i].containsPoint(x, y))
                {
                    base.exitThisMenu();

                    AttemptTeleport(Game1.player, this.pages[this.currentPage][i]);
                    return;
                }
            }
            if (this.currentPage < this.pages.Count - 1 && this.forwardButton.containsPoint(x, y))
            {
                this.pageForwardButton();
                return;
            }
            if (this.currentPage > 0 && this.backButton.containsPoint(x, y))
            {
                this.pageBackButton();
                return;
            }
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            SpriteText.drawStringWithScrollCenteredAt(b, "Choose a Destination", base.xPositionOnScreen + base.width / 2, base.yPositionOnScreen - 64);
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), base.xPositionOnScreen, base.yPositionOnScreen, base.width, base.height, Color.White, 4f);
            for (int j = 0; j < this.teleportDestinationButtons.Count; j++)
            {
                if (this.pages.Count() > 0 && this.pages[this.currentPage].Count() > j)
                {
                    IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 396, 15, 15), this.teleportDestinationButtons[j].bounds.X, this.teleportDestinationButtons[j].bounds.Y, this.teleportDestinationButtons[j].bounds.Width, this.teleportDestinationButtons[j].bounds.Height, this.teleportDestinationButtons[j].containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()) ? Color.Wheat : Color.White, 4f, drawShadow: false);
                    Utility.drawWithShadow(b, Game1.objectSpriteSheet, new Vector2(this.teleportDestinationButtons[j].bounds.X + 32, this.teleportDestinationButtons[j].bounds.Y + 28), new Rectangle(0, 512, 16, 16), sourceObelisk.TileLocation == this.pages[this.currentPage][j].Tile && Game1.player.currentLocation.NameOrUniqueName == this.pages[this.currentPage][j].LocationName ? Color.White : Color.Gray, 0f, Vector2.Zero, 2f, flipped: false, 0.99f, shadowIntensity: 0f);
                    SpriteText.drawString(b, this.pages[this.currentPage][j].CustomName, this.teleportDestinationButtons[j].bounds.X + 128 + 4, this.teleportDestinationButtons[j].bounds.Y + 20);

                    // Draw the rename button
                    this.renameObeliskButtons[j].draw(b);
                }
            }

            if (this.currentPage < this.pages.Count - 1 && this.questPage == -1)
            {
                this.forwardButton.draw(b);
            }
            if (this.currentPage > 0 || this.questPage != -1)
            {
                this.backButton.draw(b);
            }

            base.draw(b);
            Game1.mouseCursorTransparency = 1f;
            base.drawMouse(b);

            if (this.hoverText.Length > 0)
            {
                IClickableMenu.drawHoverText(b, this.hoverText, Game1.dialogueFont);
            }
        }
    }
}