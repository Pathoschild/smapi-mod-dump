/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/HandyHeadphones
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewValley.Quests;

namespace HandyHeadphones.UI
{
    public class MusicMenu : IClickableMenu
    {
        // Constants
        private const int songsPerPage = 6;
        private const int region_forwardButton = 101;
        private const int region_backButton = 102;

        // UI related
        private List<List<string>> pages;
        private List<string> songs = new List<string>();
        private int maxSongWidth = (int)Game1.dialogueFont.MeasureString("Summer (The Sun Can Bend An Orange Sky)").X;

        public List<ClickableComponent> songButtons;
        public ClickableTextureComponent forwardButton;
        public ClickableTextureComponent backButton;
        protected float _contentHeight;
        private string hoverText = "";

        // Logic related
        private int currentPage;

        public delegate void actionOnChoosingListOption(string s);
        private actionOnChoosingListOption chooseAction;

        public MusicMenu(List<string> songs, actionOnChoosingListOption chooseAction, bool isJukebox = false, string default_selection = null) : base(0, 0, 0, 0, showUpperRightCloseButton: true)
        {
            // Filter songs using base game logic
            ChooseFromListMenu.FilterJukeboxTracks(songs);

            this.songs = songs;
            this.chooseAction = chooseAction;

            Game1.playSound("bigSelect");
            this.PaginateSongs();
            base.width = Game1.uiViewport.Width / 2 < maxSongWidth ? Game1.uiViewport.Width / 4 + maxSongWidth : Game1.uiViewport.Width / 2;
            base.height = 576;

            if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.fr)
            {
                base.height += 64;
            }
            Vector2 topLeft = Utility.getTopLeftPositionForCenteringOnScreen(base.width, base.height);
            base.xPositionOnScreen = (int)topLeft.X;
            base.yPositionOnScreen = (int)topLeft.Y + 32;

            // Create the buttons used for teleporting and renaming
            this.songButtons = new List<ClickableComponent>();

            for (int i = 0; i < songsPerPage; i++)
            {
                this.songButtons.Add(new ClickableComponent(new Rectangle(base.xPositionOnScreen + 16, base.yPositionOnScreen + 16 + i * ((base.height - 32) / songsPerPage), base.width - 32, (base.height - 32) / songsPerPage + 4), string.Concat(i))
                {
                    myID = i,
                    downNeighborID = -7777,
                    upNeighborID = ((i > 0) ? (i - 1) : (-1)),
                    rightNeighborID = i + 103,
                    leftNeighborID = -7777,
                    fullyImmutable = true
                });
            }

            base.upperRightCloseButton = new ClickableTextureComponent(new Rectangle(base.xPositionOnScreen + base.width - 20, base.yPositionOnScreen - 8, 48, 48), Game1.mouseCursors, new Rectangle(337, 494, 12, 12), 4f);
            this.backButton = new ClickableTextureComponent(new Rectangle(base.xPositionOnScreen - 64, base.yPositionOnScreen + base.height - 48, 48, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f)
            {
                myID = region_backButton,
                rightNeighborID = -7777
            };
            this.forwardButton = new ClickableTextureComponent(new Rectangle(base.xPositionOnScreen + base.width + 64 - 48, base.yPositionOnScreen + base.height - 48, 48, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f)
            {
                myID = region_forwardButton
            };

            if (Game1.options.SnappyMenus)
            {
                base.populateClickableComponentList();
                this.snapToDefaultClickableComponent();
            }
        }

        public void PaginateSongs()
        {
            int count = songs.Count - 1;
            this.pages = new List<List<string>>();
            foreach (string song in songs)
            {
                int which2 = songs.Count - 1 - count;
                while (this.pages.Count <= which2 / songsPerPage)
                {
                    this.pages.Add(new List<string>());
                }
                this.pages[which2 / songsPerPage].Add(song);

                count--;
            }

            if (this.pages.Count == 0)
            {
                this.pages.Add(new List<string>());
            }
            this.currentPage = Math.Min(Math.Max(this.currentPage, 0), this.pages.Count - 1);
        }

        protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
        {
            if (oldID >= 0 && oldID != region_forwardButton && oldID != region_backButton)
            {
                switch (direction)
                {
                    case 2:
                        if (oldID < songsPerPage - 1 && this.pages[this.currentPage].Count - 1 > oldID)
                        {
                            base.currentlySnappedComponent = base.getComponentWithID(oldID + 1);
                        }
                        break;
                    case 1:
                        if (this.currentPage < this.pages.Count - 1 && oldID > region_backButton)
                        {
                            base.currentlySnappedComponent = base.getComponentWithID(region_forwardButton);
                            base.currentlySnappedComponent.leftNeighborID = oldID;
                        }
                        break;
                    case 3:
                        if (this.currentPage > 0)
                        {
                            base.currentlySnappedComponent = base.getComponentWithID(region_backButton);
                            base.currentlySnappedComponent.rightNeighborID = oldID;
                        }
                        break;
                }
            }
            else if (oldID == region_backButton)
            {
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
            if (b == Buttons.RightTrigger && this.currentPage < this.pages.Count - 1)
            {
                this.pageForwardButton();
            }
            else if (b == Buttons.LeftTrigger && this.currentPage > 0)
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

            for (int i = 0; i < this.songButtons.Count; i++)
            {
                if (!(this.pages.Count > 0 && this.pages[this.currentPage].Count > i))
                {
                    continue;
                }

                if (this.songButtons[i].containsPoint(x, y))
                {
                    base.exitThisMenu();

                    string cueName = this.pages[this.currentPage][i];
                    if (cueName == "random")
                    {
                        cueName = Utility.GetRandom<string>(songs.Where(s => s != "random" && s != "turn_off").ToList());
                    }

                    // Play song
                    this.chooseAction(cueName);

                    string songName = Utility.getSongTitleFromCueName(cueName);
                    if (songName == "Off")
                    {
                        Game1.addHUDMessage(new HUDMessage($"Shutting off headphones", 3));
                    }
                    else
                    {
                        Game1.addHUDMessage(new HUDMessage($"Now playing \"{songName}\"", 2));
                    }

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
            SpriteText.drawStringWithScrollCenteredAt(b, "Pick a Song to Play", base.xPositionOnScreen + base.width / 2, base.yPositionOnScreen - 64);
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), base.xPositionOnScreen, base.yPositionOnScreen, base.width, base.height, Color.White, 4f);
            for (int j = 0; j < this.songButtons.Count; j++)
            {
                if (this.pages.Count() > 0 && this.pages[this.currentPage].Count() > j)
                {
                    string songName = Utility.getSongTitleFromCueName(this.pages[this.currentPage][j]);

                    IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 396, 15, 15), this.songButtons[j].bounds.X, this.songButtons[j].bounds.Y, this.songButtons[j].bounds.Width, this.songButtons[j].bounds.Height, this.songButtons[j].containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()) ? Color.Wheat : Color.White, 4f, drawShadow: false);
                    if (songName == "Off")
                    {
                        Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2(this.songButtons[j].bounds.X + 32, this.songButtons[j].bounds.Y + 28), new Rectangle(137, 384, 9, 9), Color.White, 0f, Vector2.Zero, 4f, flipped: false, 0.99f, shadowIntensity: 0f);
                    }
                    else if (songName == "Randomize")
                    {
                        Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2(this.songButtons[j].bounds.X + 32, this.songButtons[j].bounds.Y + 28), new Rectangle(50, 428, 10, 10), Color.White, 0f, Vector2.Zero, 4f, flipped: false, 0.99f, shadowIntensity: 0f);
                    }
                    else
                    {
                        Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2(this.songButtons[j].bounds.X + 32, this.songButtons[j].bounds.Y + 28), new Rectangle(516, 1916, 7, 10), Color.White, 0f, Vector2.Zero, 4f, flipped: false, 0.99f, shadowIntensity: 0f);
                    }
                    //Utility.drawTextWithShadow(b, songName, Game1.dialogueFont, new Vector2((float)(this.songButtons[j].bounds.X + 128 + 4) - Game1.dialogueFont.MeasureString(songName).X / 2f, this.songButtons[j].bounds.Y + 20), Game1.textColor);
                    Utility.drawTextWithShadow(b, songName, Game1.dialogueFont, new Vector2((float)(base.xPositionOnScreen + base.width / 2) - Game1.dialogueFont.MeasureString(songName).X / 2f, this.songButtons[j].bounds.Y + 20), Game1.textColor);
                    //SpriteText.drawString(b, songName, this.songButtons[j].bounds.X + 128 + 4, this.songButtons[j].bounds.Y + 20);
                }
            }

            if (this.currentPage < this.pages.Count - 1)
            {
                this.forwardButton.draw(b);
            }
            if (this.currentPage > 0)
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