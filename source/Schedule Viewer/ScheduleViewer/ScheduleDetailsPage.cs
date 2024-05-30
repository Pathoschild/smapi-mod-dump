/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/BinaryLip/ScheduleViewer
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Characters;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScheduleViewer
{
    public class ScheduleDetailsPage : IClickableMenu
    {
        private int currentIndex;

        private SocialPage.SocialEntry socialEntry;

        private string hoverText = "";

        /// <summary>Key in the line number, Value is Rectangle containing the bounds of the hover text and the hover text string"</summary>
        protected readonly List<Tuple<Rectangle, string>> hoverTextOptions = new();

        private readonly List<Schedule.NPCSchedule> schedules;

        private int _characterSpriteRandomInt;

        public bool scrolling;

        protected string _printedName = "";

        public ClickableTextureComponent nextCharacterButton;

        public ClickableTextureComponent previousCharacterButton;

        public ClickableTextureComponent upArrow;

        public ClickableTextureComponent downArrow;

        protected ClickableTextureComponent scrollBar;

        protected Rectangle scrollBarRunner;

        protected int scrollPosition;

        protected int scrollStep = 36;

        protected int scrollSize;

        protected int maxLines;

        protected int currentLineNum;

        public Texture2D letterTexture;

        public Rectangle _contentRectangle;

        protected Rectangle characterSpriteBox;

        protected Vector2 _characterEntrancePosition = new Vector2(0f, 0f);

        protected AnimatedSprite _animatedSprite;

        protected float _hiddenEmoteTimer = -1f;

        protected int _currentDirection;

        protected float _directionChangeTimer;

        protected Vector2 _characterNamePosition;

        protected Vector2 _heartDisplayPosition;

        protected Vector2 _giftHeaderDisplayPosition;

        protected Vector2 _giftDisplayPosition;

        protected Vector2 _locationHeaderDisplayPosition;

        protected Vector2 _locationDisplayPosition;

        protected Vector2 _pageHeaderDisplayPosition;

        protected Vector2 _characterSpriteDrawPosition;

        protected Rectangle _characterStatusDisplayBox;

        protected List<ClickableTextureComponent> _clickableTextureComponents = new();

        public readonly Rectangle emptyHeartSourceRect = new(218, 428, 7, 6);

        public readonly Rectangle filledHeartSourceRect = new(211, 428, 7, 6);

        /// <summary>LookupAnything</summary>
        public NPC hoveredNpc;


        public ScheduleDetailsPage(int index, List<Schedule.NPCSchedule> allSchedules)
        : base((int)Utility.getTopLeftPositionForCenteringOnScreen(1280, 720).X, (int)Utility.getTopLeftPositionForCenteringOnScreen(1280, 720).Y, 1280, 720, showUpperRightCloseButton: true)
        {
            this.currentIndex = index;
            this.schedules = allSchedules;
            this._printedName = "";
            this._characterEntrancePosition = new Vector2(0f, 4f);
            this.letterTexture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\letterBG");

            // init buttons
            this.upArrow = new ClickableTextureComponent(new Rectangle(0, 0, 44, 48), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), 4f)
            {
                myID = 105,
                upNeighborID = 102,
                upNeighborImmutable = true,
                downNeighborID = 106,
                downNeighborImmutable = true,
                leftNeighborID = -99998,
                leftNeighborImmutable = true
            };
            this._clickableTextureComponents.Add(this.upArrow);
            this.downArrow = new ClickableTextureComponent(new Rectangle(0, 0, 44, 48), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), 4f)
            {
                myID = 106,
                upNeighborID = 105,
                upNeighborImmutable = true,
                leftNeighborID = -99998,
                leftNeighborImmutable = true
            };
            this._clickableTextureComponents.Add(this.downArrow);
            this.scrollBar = new ClickableTextureComponent(new Rectangle(0, 0, 24, 40), Game1.mouseCursors, new Rectangle(435, 463, 6, 10), 4f);
            this.previousCharacterButton = new ClickableTextureComponent(new Rectangle(base.xPositionOnScreen + 32, base.yPositionOnScreen + base.height - 32 - 64, 48, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f)
            {
                myID = 0,
                name = "Previous Char",
                upNeighborID = -99998,
                downNeighborID = -99998,
                leftNeighborID = -99998,
                rightNeighborID = -99998,
                region = 500
            };
            this._clickableTextureComponents.Add(this.previousCharacterButton);
            this.nextCharacterButton = new ClickableTextureComponent(new Rectangle(base.xPositionOnScreen + base.width - 32 - 48, base.yPositionOnScreen + base.height - 32 - 64, 48, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f)
            {
                myID = 0,
                name = "Next Char",
                upNeighborID = -99998,
                downNeighborID = -99998,
                leftNeighborID = -99998,
                rightNeighborID = -99998,
                region = 500
            };
            this._clickableTextureComponents.Add(this.nextCharacterButton);

            this.SetCharacter(index);
        }

        #region override methods
        public override void applyMovementKey(int direction)
        {
            // TODO - have each line be a snapable component for better gamepad support
            base.applyMovementKey(direction);
            // ConstrainSelectionToView
            //if (!Game1.options.snappyMenus)
            //{
            //    return;
            //}
            //if (base.currentlySnappedComponent != null && base.currentlySnappedComponent.region == 502 && !this._contentRectangle.Contains(base.currentlySnappedComponent.bounds))
            //{
            //    if (base.currentlySnappedComponent.bounds.Bottom > this._contentRectangle.Bottom)
            //    {
            //        int scroll2 = (int)Math.Ceiling(((double)base.currentlySnappedComponent.bounds.Bottom - (double)this._contentRectangle.Bottom) / (double)this.scrollStep) * this.scrollStep;
            //        //this.Scroll(scroll2);
            //    }
            //    else if (base.currentlySnappedComponent.bounds.Top < this._contentRectangle.Top)
            //    {
            //        int scroll = (int)Math.Floor(((double)base.currentlySnappedComponent.bounds.Top - (double)this._contentRectangle.Top) / (double)this.scrollStep) * this.scrollStep;
            //        //this.Scroll(scroll);
            //    }
            //}
            //if (this.scrollPosition <= this.scrollStep)
            //{
            //    this.scrollPosition = 0;
            //    this.UpdateScroll();
            //}
        }
        public override void draw(SpriteBatch b)
        {
            Schedule.NPCSchedule schedule = this.schedules[this.currentIndex];
            var (entries, currentLocation, isOnSchedule, displayName, npc) = schedule;
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);
            b.Draw(this.letterTexture, new Vector2(base.xPositionOnScreen + base.width / 2, base.yPositionOnScreen + base.height / 2), new Rectangle(0, 0, 320, 180), Color.White, 0f, new Vector2(160f, 90f), 4f, SpriteEffects.None, 0.86f);
            Game1.DrawBox(this._characterStatusDisplayBox.X, this._characterStatusDisplayBox.Y, this._characterStatusDisplayBox.Width, this._characterStatusDisplayBox.Height);

            // sprite
            b.Draw((Game1.timeOfDay >= 1900) ? Game1.nightbg : Game1.daybg, this._characterSpriteDrawPosition, Color.White);
            Vector2 character_position_offset = new Vector2(0f, (32 - this._animatedSprite.SpriteHeight) * 4);
            character_position_offset += this._characterEntrancePosition * 4f;
            this._animatedSprite.draw(b, new Vector2(this._characterSpriteDrawPosition.X + 32f + character_position_offset.X, this._characterSpriteDrawPosition.Y + 32f + character_position_offset.Y), 0.8f);

            // name label
            if (this._printedName.Length < displayName.Length)
            {
                SpriteText.drawStringWithScrollCenteredAt(b, "", (int)this._characterNamePosition.X, (int)this._characterNamePosition.Y, this._printedName);
            }
            else
            {
                SpriteText.drawStringWithScrollCenteredAt(b, displayName, (int)this._characterNamePosition.X, (int)this._characterNamePosition.Y);
            }

            // friendship section
            if (ShowFriendshipSection(out string giftedTodayText))
            {
                // hearts
                int maxHearts = Math.Max(10, Utility.GetMaximumHeartsForCharacter(npc));
                float heart_draw_start_x = this._heartDisplayPosition.X - (float)(Math.Min(10, maxHearts) * 32 / 2);
                float heart_draw_offset_y = maxHearts > 10 ? -16f : 0f;
                for (int i = 0; i < maxHearts; i++)
                {
                    bool isGreyedOut = socialEntry.IsDatable && !socialEntry.IsDatingCurrentPlayer() && !socialEntry.IsMarriedToCurrentPlayer() && i >= 8;
                    Rectangle heartSourceRect = isGreyedOut || i < socialEntry.HeartLevel ? filledHeartSourceRect : emptyHeartSourceRect;
                    if (i < 10)
                    {
                        b.Draw(Game1.mouseCursors, new Vector2(heart_draw_start_x + (float)(i * 32), this._heartDisplayPosition.Y + heart_draw_offset_y), heartSourceRect, isGreyedOut ? (Color.Black * 0.35f) : Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.88f);
                    }
                    else
                    {
                        b.Draw(Game1.mouseCursors, new Vector2(heart_draw_start_x + (float)((i - 10) * 32), this._heartDisplayPosition.Y + heart_draw_offset_y + 32f), heartSourceRect, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.88f);
                    }
                }

                // Gifted today
                SpriteText.drawStringHorizontallyCenteredAt(b, ModEntry.ModHelper.Translation.Get("schedule_details_page.gifted_today.header"), (int)this._giftHeaderDisplayPosition.X, (int)this._giftHeaderDisplayPosition.Y);
                Utility.drawBoldText(b, giftedTodayText, Game1.dialogueFont, new Vector2((0f - Game1.dialogueFont.MeasureString(giftedTodayText).X) / 2f + this._giftDisplayPosition.X, this._giftDisplayPosition.Y), Game1.textColor);
            }

            // Current Location
            SpriteText.drawStringHorizontallyCenteredAt(b, ModEntry.ModHelper.Translation.Get("schedule_details_page.current_location"), (int)this._locationHeaderDisplayPosition.X, (int)this._locationHeaderDisplayPosition.Y);
            int maxWidth = this._characterStatusDisplayBox.Width - 8;
            string[] words = currentLocation.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            StringBuilder line1 = new(), line2 = new();
            foreach (string word in words)
            {
                if (line2.Length == 0 && Game1.dialogueFont.MeasureString($"{line1}{word}").X < maxWidth)
                {
                    line1.Append(word);
                    line1.Append(' ');
                }
                else if (Game1.dialogueFont.MeasureString($"{line2}{word}").X < maxWidth)
                {
                    line2.Append(word);
                    line2.Append(' ');
                }
                else
                {
                    break;
                }
            }
            string line1Text = line1.ToString().Trim();
            string line2Text = line2.ToString().Trim();
            Vector2 line1Size = Game1.dialogueFont.MeasureString(line1Text);
            Utility.drawBoldText(b, line1Text, Game1.dialogueFont, new Vector2((0f - line1Size.X) / 2f + this._locationDisplayPosition.X, this._locationDisplayPosition.Y), Game1.textColor);
            Utility.drawBoldText(b, line2Text, Game1.dialogueFont, new Vector2((0f - Game1.dialogueFont.MeasureString(line2Text).X) / 2f + this._locationDisplayPosition.X, this._locationDisplayPosition.Y + line1Size.Y), Game1.textColor);

            SpriteText.drawStringWithScrollCenteredAt(b, ModEntry.ModHelper.Translation.Get("schedule_details_page.header"), (int)this._pageHeaderDisplayPosition.X, (int)this._pageHeaderDisplayPosition.Y);
            b.End();
            Rectangle cached_scissor_rect = b.GraphicsDevice.ScissorRectangle;
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, Utility.ScissorEnabled);
            b.GraphicsDevice.ScissorRectangle = this._contentRectangle;

            this.hoverTextOptions.Clear();
            SpriteFont font = ModEntry.Config.UseLargerFontForScheduleDetails ? Game1.dialogueFont : Game1.smallFont;
            float lineHeight = font.MeasureString("W").Y;
            int x = this._contentRectangle.X;
            int y = this._contentRectangle.Y;
            if (isOnSchedule)
            {
                float yOffset = 0;
                int activeEntryIndex = 0;
                Dictionary<int, Schedule.ScheduleEntry> lines = new();
                for (int i = 0; i < entries.Count; i++)
                {
                    if (entries[i].Time <= Game1.timeOfDay)
                    {
                        activeEntryIndex = i;
                    }
                    if (i >= this.currentLineNum && i < this.currentLineNum + this.maxLines)
                    {
                        lines.Add(i, entries[i]);
                    }
                }
                foreach (var line in lines)
                {
                    Schedule.ScheduleEntry entry = line.Value;
                    string entryString = entry?.ToString();
                    int stringWidth = 0;
                    if (!string.IsNullOrEmpty(entryString))
                    {
                        stringWidth = (int)font.MeasureString(entryString).X;
                        this.hoverTextOptions.Add(Tuple.Create(new Rectangle(x, y + (int)yOffset, stringWidth + 2, (int)lineHeight), entry?.HoverText));
                    }
                    if (entry != null)
                    {
                        if (line.Key == activeEntryIndex)
                        {
                            Utility.drawBoldText(b, entryString, font, new Vector2(x, y + yOffset), Game1.textColor);
                        }
                        else
                        {
                            b.DrawString(font, entryString, new Vector2(x, y + yOffset), Game1.textColor);
                        }
                        // draw inaccesible icon
                        if (!line.Value.CanAccess)
                        {
                            if (ModEntry.Config.UseLargerFontForScheduleDetails)
                            {
                                b.Draw(Game1.mouseCursors, new Vector2(x + stringWidth + 6, y + yOffset + 12), new Rectangle(218, 428, 7, 6), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
                            }
                            else
                            {
                                b.Draw(Game1.mouseCursors, new Vector2(x + stringWidth + 4, y + yOffset + 7), new Rectangle(218, 428, 7, 6), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 1f);
                            }
                        }
                        yOffset += lineHeight;
                    }
                }
            }
            else
            {
                b.DrawString(font, ModEntry.ModHelper.Translation.Get(entries == null ? "not_following_schedule_today" : "ignoring_schedule_today"), new Vector2(x, y), Game1.textColor);
                Utility.drawBoldText(b, currentLocation, font, new Vector2(x, y + lineHeight), Game1.textColor);
            }

            b.End();
            b.GraphicsDevice.ScissorRectangle = cached_scissor_rect;
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
            if (this.NeedsScrollBar())
            {
                IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), this.scrollBarRunner.X, this.scrollBarRunner.Y, this.scrollBarRunner.Width, this.scrollBarRunner.Height, Color.White, 4f, drawShadow: false);
                this.scrollBar.draw(b);
            }
            foreach (ClickableTextureComponent clickableTextureComponent in this._clickableTextureComponents)
            {
                clickableTextureComponent.draw(b);
            }

            if (!this.hoverText.Equals(""))
            {
                IClickableMenu.drawHoverText(b, this.hoverText, Game1.smallFont);
            }
            base.draw(b);
            base.drawMouse(b, ignore_transparency: true);
        }
        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);
            Game1.activeClickableMenu = new ScheduleDetailsPage(this.currentIndex, this.schedules);
        }
        public override bool IsAutomaticSnapValid(int direction, ClickableComponent a, ClickableComponent b)
        {
            if (direction == 2 && a.region == 501 && b.region == 500)
            {
                return false;
            }
            return base.IsAutomaticSnapValid(direction, a, b);
        }
        public override void leftClickHeld(int x, int y)
        {
            if (GameMenu.forcePreventClose)
            {
                return;
            }
            base.leftClickHeld(x, y);
            if (this.scrolling)
            {
                int numOfLines = GetNumOfEntries();
                int y2 = this.scrollBar.bounds.Y;
                this.scrollBar.bounds.Y = Math.Min(base.yPositionOnScreen + base.height - 64 - 12 - this.scrollBar.bounds.Height, Math.Max(y, base.yPositionOnScreen + this.upArrow.bounds.Height + 20));
                float percentage = (float)(y - this.scrollBarRunner.Y) / (float)this.scrollBarRunner.Height;
                this.currentLineNum = Math.Min(numOfLines - this.maxLines, Math.Max(0, (int)((float)numOfLines * percentage)));
                this.SetScrollBarToCurrentIndex();
                if (y2 != this.scrollBar.bounds.Y)
                {
                    Game1.playSound("shiny4");
                }
            }
        }
        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            this.upArrow.tryHover(x, y);
            this.downArrow.tryHover(x, y);
            this.nextCharacterButton.tryHover(x, y, 0.6f);
            this.previousCharacterButton.tryHover(x, y, 0.6f);
            if (!ModEntry.Config.DisableHover)
            {
                string newHoverText = "";
                foreach (var hoverTextOption in this.hoverTextOptions)
                {
                    if (hoverTextOption != null && hoverTextOption.Item1.Contains(x, y))
                    {
                        newHoverText = hoverTextOption.Item2;
                        break;
                    }
                }
                this.hoverText = newHoverText;
            }
        }
        public override void receiveGamePadButton(Buttons b)
        {
            base.receiveGamePadButton(b);
            switch (b)
            {
                case Buttons.RightShoulder:
                    this.ChangeCharacter(1);
                    break;
                case Buttons.LeftShoulder:
                    this.ChangeCharacter(-1);
                    break;
                case Buttons.Back:
                    this.PlayHiddenEmote();
                    break;
            }
        }
        public override void receiveKeyPress(Keys key)
        {
            if (key != 0)
            {
                if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && this.readyToClose())
                {
                    base.exitThisMenu();
                }
                else if (Game1.options.snappyMenus && Game1.options.gamepadControls && !this.overrideSnappyMenuCursorMovementBan())
                {
                    base.applyMovementKey(key);
                }
            }
        }
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (this.scrollBar.containsPoint(x, y))
            {
                this.scrolling = true;
            }
            else if (this.scrollBarRunner.Contains(x, y))
            {
                this.scrolling = true;
                this.leftClickHeld(x, y);
                this.releaseLeftClick(x, y);
            }
            if (base.upperRightCloseButton != null && this.readyToClose() && base.upperRightCloseButton.containsPoint(x, y))
            {
                base.exitThisMenu();
                return;
            }
            if (Game1.activeClickableMenu == null && Game1.currentMinigame == null)
            {
                return;
            }
            if (this.previousCharacterButton.containsPoint(x, y))
            {
                this.ChangeCharacter(-1);
                return;
            }
            if (this.nextCharacterButton.containsPoint(x, y))
            {
                this.ChangeCharacter(1);
                return;
            }
            if (this.downArrow.containsPoint(x, y))
            {
                DownArrowPressed();
                Game1.playSound("shwip");
                return;
            }
            if (this.upArrow.containsPoint(x, y))
            {
                this.UpArrowPressed();
                Game1.playSound("shwip");
                return;
            }
            if (this.characterSpriteBox.Contains(x, y))
            {
                this.PlayHiddenEmote();
            }
        }
        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            this.receiveLeftClick(x, y, playSound);
        }
        public override void receiveScrollWheelAction(int direction)
        {
            base.receiveScrollWheelAction(direction);
            if (direction > 0 && this.currentLineNum > 0)
            {
                this.UpArrowPressed();
                Game1.playSound("shiny4");
            }
            else if (direction < 0 && this.currentLineNum < Math.Max(0, GetNumOfEntries() - this.maxLines))
            {
                this.DownArrowPressed();
                Game1.playSound("shiny4");
            }
        }
        public override void releaseLeftClick(int x, int y)
        {
            base.releaseLeftClick(x, y);
            this.scrolling = false;
        }
        public override void snapToDefaultClickableComponent()
        {
            base.currentlySnappedComponent = this.previousCharacterButton;
            this.snapCursorToCurrentSnappedComponent();
        }
        public override void update(GameTime time)
        {
            base.update(time);
            NPC npc = this.GetCharacter();
            if (npc != null && npc.displayName != null && this._printedName.Length < npc.displayName.Length)
            {
                this._printedName += npc?.displayName[this._printedName.Length];
            }
            if (this._characterEntrancePosition.X != 0f)
            {
                this._characterEntrancePosition.X -= (float)Math.Sign(this._characterEntrancePosition.X) * 0.25f;
            }
            if (this._characterEntrancePosition.Y != 0f)
            {
                this._characterEntrancePosition.Y -= (float)Math.Sign(this._characterEntrancePosition.Y) * 0.25f;
            }
            if (this._animatedSprite == null)
            {
                return;
            }
            if (this._hiddenEmoteTimer > 0f)
            {
                this._hiddenEmoteTimer -= time.ElapsedGameTime.Milliseconds;
                if (this._hiddenEmoteTimer <= 0f)
                {
                    this._hiddenEmoteTimer = -1f;
                    this._currentDirection = 2;
                    this._directionChangeTimer = 2000f;
                    if (npc != null && npc.Name == "Leo")
                    {
                        npc.Sprite.AnimateDown(time);
                    }
                }
            }
            else if (this._directionChangeTimer > 0f)
            {
                this._directionChangeTimer -= time.ElapsedGameTime.Milliseconds;
                if (this._directionChangeTimer <= 0f)
                {
                    this._directionChangeTimer = 2000f;
                    this._currentDirection = (this._currentDirection + 1) % 4;
                }
            }
            if (this._characterEntrancePosition != Vector2.Zero)
            {
                if (this._characterEntrancePosition.X < 0f)
                {
                    this._animatedSprite.AnimateRight(time, 2);
                }
                else if (this._characterEntrancePosition.X > 0f)
                {
                    this._animatedSprite.AnimateLeft(time, 2);
                }
                else if (this._characterEntrancePosition.Y > 0f)
                {
                    this._animatedSprite.AnimateUp(time, 2);
                }
                else if (this._characterEntrancePosition.Y < 0f)
                {
                    this._animatedSprite.AnimateDown(time, 2);
                }
            }
            else if (this._hiddenEmoteTimer > 0f)
            {
                switch (npc.Name)
                {
                    case "Abigail":
                        this._animatedSprite.Animate(time, 16, 4, 200f);
                        break;
                    case "Penny":
                        this._animatedSprite.Animate(time, 18, 2, 1000f);
                        break;
                    case "Maru":
                        this._animatedSprite.Animate(time, 16, 8, 150f);
                        break;
                    case "Leah":
                        this._animatedSprite.Animate(time, 16, 4, 200f);
                        break;
                    case "Haley":
                        this._animatedSprite.Animate(time, 26, 1, 200f);
                        break;
                    case "Emily":
                        this._animatedSprite.Animate(time, 16 + this._characterSpriteRandomInt * 2, 2, 300f);
                        break;
                    case "Sam":
                        this._animatedSprite.Animate(time, 20, 2, 300f);
                        break;
                    case "Sebastian":
                        this._animatedSprite.Animate(time, 16, 8, 180f);
                        break;
                    case "Shane":
                        this._animatedSprite.Animate(time, 28, 2, 500f);
                        break;
                    case "Elliott":
                        this._animatedSprite.Animate(time, 33, 2, 800f);
                        break;
                    case "Harvey":
                        this._animatedSprite.Animate(time, 20, 2, 800f);
                        break;
                    case "Alex":
                        this._animatedSprite.Animate(time, 16, 8, 170f);
                        break;
                    case "Lewis":
                        this._animatedSprite.Animate(time, 24, 1, 170f);
                        break;
                    case "Wizard":
                        this._animatedSprite.Animate(time, 16, 1, 170f);
                        break;
                    case "Willy":
                        this._animatedSprite.Animate(time, 28, 4, 200f);
                        break;
                    case "Vincent":
                        this._animatedSprite.Animate(time, 18, 2, 600f);
                        break;
                    case "Robin":
                        this._animatedSprite.Animate(time, 32, 2, 120f);
                        break;
                    case "Marnie":
                        this._animatedSprite.Animate(time, 28, 4, 120f);
                        break;
                    case "Linus":
                        this._animatedSprite.Animate(time, 22, 1, 200f);
                        break;
                    case "Kent":
                        this._animatedSprite.Animate(time, 16, 1, 200f);
                        break;
                    case "Jodi":
                        this._animatedSprite.Animate(time, 16, 2, 200f);
                        break;
                    case "Jas":
                        this._animatedSprite.Animate(time, 16, 4, 100f);
                        break;
                    case "Gus":
                        this._animatedSprite.Animate(time, 18, 3, 200f);
                        break;
                    case "George":
                        this._animatedSprite.Animate(time, 16, 4, 400f);
                        break;
                    case "Demetrius":
                        this._animatedSprite.Animate(time, 30, 2, 200f);
                        break;
                    case "Caroline":
                        this._animatedSprite.Animate(time, 19, 1, 200f);
                        break;
                    case "Pierre":
                        this._animatedSprite.Animate(time, 23, 1, 200f);
                        break;
                    case "Krobus":
                        this._animatedSprite.Animate(time, 20, 4, 200f);
                        break;
                    case "Evelyn":
                        this._animatedSprite.Animate(time, 20, 1, 200f);
                        break;
                    case "Clint":
                        this._animatedSprite.Animate(time, 39, 1, 200f);
                        break;
                    case "Dwarf":
                        this._animatedSprite.Animate(time, 16, 4, 100f);
                        break;
                    case "Pam":
                        this._animatedSprite.Animate(time, 28, 2, 200f);
                        break;
                    case "Sandy":
                        this._animatedSprite.Animate(time, 16, 2, 200f);
                        break;
                    case "Leo":
                        this._animatedSprite.Animate(time, 17, 1, 200f);
                        break;
                    default:
                        this._animatedSprite.AnimateDown(time, 2);
                        break;
                }
            }
            else
            {
                switch (this._currentDirection)
                {
                    case 0:
                        this._animatedSprite.AnimateUp(time, 2);
                        break;
                    case 2:
                        this._animatedSprite.AnimateDown(time, 2);
                        break;
                    case 3:
                        this._animatedSprite.AnimateLeft(time, 2);
                        break;
                    case 1:
                        this._animatedSprite.AnimateRight(time, 2);
                        break;
                }
            }
        }
        #endregion

        public bool ShowFriendshipSection(out string giftText)
        {
            giftText = null;
            NPC npc = this.GetCharacter();
            if (!npc.CanSocialize)
            {
                return false;
            }
            if (socialEntry.Friendship == null)
            {
                giftText = Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_No");
                return true;
            }
            bool alreadyGiftedToday = socialEntry.Friendship.GiftsToday == 1;
            if (alreadyGiftedToday)
            {
                giftText = Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_Yes");
                return true;
            }
            bool ignoreWeeklyGiftLimit = npc.isBirthday() || npc is Child || socialEntry.IsMarriedToCurrentPlayer();
            if (ignoreWeeklyGiftLimit)
            {
                giftText = Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_No");
                return true;
            }
            if (socialEntry.Friendship.GiftsThisWeek >= NPC.maxGiftsPerWeek)
            {
                giftText = ModEntry.ModHelper.Translation.Get("schedule_details_page.gifted_today.weekly_limit");
                return true;
            }
            giftText = Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_No");
            return true;
        }

        public void ChangeCharacter(int offset)
        {
            int numOfNPCs = schedules.Count;
            int newIndex = this.currentIndex + offset;
            if (this.currentIndex + offset < 0)
            {
                newIndex = numOfNPCs - 1;
            }
            else if (this.currentIndex + offset >= numOfNPCs)
            {
                newIndex = 0;
            }
            this.SetCharacter(newIndex);
            Game1.playSound("smallSelect");
            this._printedName = "";
            this._characterEntrancePosition = new Vector2(Math.Sign(offset) * -4, 0f);
            if (Game1.options.SnappyMenus && (base.currentlySnappedComponent == null || !base.currentlySnappedComponent.visible))
            {
                this.snapToDefaultClickableComponent();
            }
        }

        private void DownArrowPressed()
        {
            this.downArrow.scale = 3.5f;
            if (this.currentLineNum < GetNumOfEntries() - this.maxLines)
            {
                this.currentLineNum++;
                this.SetScrollBarToCurrentIndex();
            }
        }

        public NPC GetCharacter() => this.schedules[this.currentIndex]?.NPC;

        public int GetCurrentIndex() => this.currentIndex;

        public int GetNumOfEntries() => this.schedules[this.currentIndex]?.Entries?.Count ?? 0;

        public bool NeedsScrollBar() => GetNumOfEntries() > this.maxLines;

        public void PlayHiddenEmote()
        {
            string name = this.GetCharacter()?.Name;
            if (Game1.player.getFriendshipHeartLevelForNPC(name) >= 4)
            {
                this._currentDirection = 2;
                this._characterSpriteRandomInt = Game1.random.Next(4);
                if (name == "Leo")
                {
                    if (this._hiddenEmoteTimer <= 0f)
                    {
                        Game1.playSound("parrot_squawk");
                        this._hiddenEmoteTimer = 300f;
                    }
                }
                else
                {
                    Game1.playSound("drumkit6");
                    this._hiddenEmoteTimer = 4000f;
                }
            }
            else
            {
                this._currentDirection = 2;
                this._directionChangeTimer = 5000f;
                Game1.playSound("Cowboy_Footstep");
            }
        }

        private void SetCharacter(int newIndex)
        {
            this.currentIndex = newIndex;
            NPC npc = this.schedules[newIndex].NPC;
            this.hoveredNpc = npc;
            this._animatedSprite = npc.Sprite.Clone();
            this._animatedSprite.tempSpriteHeight = -1;
            this._animatedSprite.faceDirection(2);
            this._directionChangeTimer = 2000f;
            this._currentDirection = 2;
            this._hiddenEmoteTimer = -1f;
            Game1.player.friendshipData.TryGetValue(npc?.Name, out Friendship friendship);
            this.socialEntry = new(npc, friendship, npc.GetData());
            this.SetupLayout();
            base.populateClickableComponentList();
            if (Game1.options.snappyMenus && Game1.options.gamepadControls && (base.currentlySnappedComponent == null || !base.allClickableComponents.Contains(base.currentlySnappedComponent)))
            {
                this.snapToDefaultClickableComponent();
            }
        }

        private void SetScrollBarToCurrentIndex()
        {
            int numOfLines = GetNumOfEntries();
            this.scrollBar.bounds.Y = this.scrollBarRunner.Height / Math.Max(1, numOfLines - this.maxLines + 1) * this.currentLineNum + this.upArrow.bounds.Bottom + 16;
            if (this.currentLineNum == numOfLines - this.maxLines)
            {
                this.scrollBar.bounds.Y = this.downArrow.bounds.Y - this.scrollBar.bounds.Height - 16;
            }
        }

        public void SetupLayout()
        {
            int x = base.xPositionOnScreen + 64 - 12;
            int y = base.yPositionOnScreen + IClickableMenu.borderWidth;
            Rectangle left_pane_rectangle = new Rectangle(x, y, 400, 720 - IClickableMenu.borderWidth * 2);
            Rectangle content_rectangle = new Rectangle(x, y, 1204, 720 - IClickableMenu.borderWidth * 2);
            content_rectangle.X += left_pane_rectangle.Width;
            content_rectangle.Width -= left_pane_rectangle.Width;
            this._characterStatusDisplayBox = new Rectangle(left_pane_rectangle.X, left_pane_rectangle.Y, left_pane_rectangle.Width, left_pane_rectangle.Height);
            left_pane_rectangle.Y += 24;
            left_pane_rectangle.Height -= 24;
            this._characterSpriteDrawPosition = new Vector2(left_pane_rectangle.X + (left_pane_rectangle.Width - Game1.nightbg.Width) / 2, left_pane_rectangle.Y);
            this.characterSpriteBox = new Rectangle(base.xPositionOnScreen + 64 - 12 + (400 - Game1.nightbg.Width) / 2, base.yPositionOnScreen + IClickableMenu.borderWidth, Game1.nightbg.Width, Game1.nightbg.Height);
            this.previousCharacterButton.bounds.X = (int)this._characterSpriteDrawPosition.X - 64 - this.previousCharacterButton.bounds.Width / 2;
            this.previousCharacterButton.bounds.Y = (int)this._characterSpriteDrawPosition.Y + Game1.nightbg.Height / 2 - this.previousCharacterButton.bounds.Height / 2;
            this.nextCharacterButton.bounds.X = (int)this._characterSpriteDrawPosition.X + Game1.nightbg.Width + 64 - this.nextCharacterButton.bounds.Width / 2;
            this.nextCharacterButton.bounds.Y = (int)this._characterSpriteDrawPosition.Y + Game1.nightbg.Height / 2 - this.nextCharacterButton.bounds.Height / 2;
            left_pane_rectangle.Y += Game1.daybg.Height + 24;
            left_pane_rectangle.Height -= Game1.daybg.Height + 24;
            this._characterNamePosition = new Vector2(left_pane_rectangle.Center.X, left_pane_rectangle.Top);
            left_pane_rectangle.Y += 96;
            left_pane_rectangle.Height -= 96;
            this._heartDisplayPosition = new Vector2(left_pane_rectangle.Center.X, left_pane_rectangle.Top);
            left_pane_rectangle.Y += 56;
            left_pane_rectangle.Height -= 48;
            this._giftHeaderDisplayPosition = new Vector2(left_pane_rectangle.Center.X, left_pane_rectangle.Top);
            left_pane_rectangle.Y += 48;
            left_pane_rectangle.Height -= 48;
            this._giftDisplayPosition = new Vector2(left_pane_rectangle.Center.X, left_pane_rectangle.Top);
            left_pane_rectangle.Y += 48;
            left_pane_rectangle.Height -= 64;
            this._locationHeaderDisplayPosition = new Vector2(left_pane_rectangle.Center.X, left_pane_rectangle.Top);
            left_pane_rectangle.Y += 48;
            left_pane_rectangle.Height -= 48;
            this._locationDisplayPosition = new Vector2(left_pane_rectangle.Center.X, left_pane_rectangle.Top);
            left_pane_rectangle.Y += 64;
            left_pane_rectangle.Height -= 64;
            content_rectangle.Height -= 56;
            content_rectangle.Y -= 8;
            this._pageHeaderDisplayPosition = new Vector2(content_rectangle.Center.X, content_rectangle.Top);
            content_rectangle.Width -= 92;
            content_rectangle.X += 48;
            content_rectangle.Y += 64;
            content_rectangle.Y += 16;
            this._contentRectangle = content_rectangle;
            int scroll_inset = 84;
            this.scrollBarRunner = new Rectangle(content_rectangle.Right + 12, content_rectangle.Top + scroll_inset, this.scrollBar.bounds.Width, content_rectangle.Height - scroll_inset * 2);
            this.downArrow.bounds.Y = this.scrollBarRunner.Bottom + 16;
            this.downArrow.bounds.X = this.scrollBarRunner.Center.X - this.downArrow.bounds.Width / 2;
            this.upArrow.bounds.Y = this.scrollBarRunner.Top - 16 - this.upArrow.bounds.Height;
            this.upArrow.bounds.X = this.scrollBarRunner.Center.X - this.upArrow.bounds.Width / 2;
            float lineHeight = ModEntry.Config.UseLargerFontForScheduleDetails ? Game1.dialogueFont.MeasureString("W").Y : Game1.smallFont.MeasureString("W").Y;
            this.maxLines = (int)Math.Floor(this._contentRectangle.Height / lineHeight);
            this.currentLineNum = 0;
            float draw_y = lineHeight * GetNumOfEntries();
            this.scrollSize = (int)draw_y - this._contentRectangle.Height;
            bool needsScrollBar = this.NeedsScrollBar();
            this.upArrow.visible = needsScrollBar;
            this.downArrow.visible = needsScrollBar;
            this.UpdateScroll();
        }

        private void UpArrowPressed()
        {
            this.upArrow.scale = 3.5f;
            if (this.currentLineNum > 0)
            {
                this.currentLineNum--;
                this.SetScrollBarToCurrentIndex();
            }
        }

        public virtual void UpdateScroll()
        {
            this.scrollPosition = Utility.Clamp(this.scrollPosition, 0, this.scrollSize);
            if (this.scrollSize > 0)
            {
                this.scrollBar.bounds.X = this.scrollBarRunner.Center.X - this.scrollBar.bounds.Width / 2;
                this.scrollBar.bounds.Y = (int)Utility.Lerp(this.scrollBarRunner.Top, this.scrollBarRunner.Bottom - this.scrollBar.bounds.Height, (float)this.scrollPosition / (float)this.scrollSize);
                if (Game1.options.SnappyMenus)
                {
                    this.snapCursorToCurrentSnappedComponent();
                }
            }
        }

    }
}
