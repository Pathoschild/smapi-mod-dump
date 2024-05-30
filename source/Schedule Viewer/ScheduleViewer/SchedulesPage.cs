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
using StardewModdingAPI;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Menus;
using StardewValley.Quests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ScheduleViewer
{
    public class SchedulesPage : IClickableMenu
    {
        public const int spriteSize = 64, partitionSize = 8, rowHeight = 112, slotsOnPage = 5;

        private string hoverText = "";

        /// <summary>Key in the format "{slot}-{line#}, Value is Rectangle containing the bounds of the hover text and the hover text string"</summary>
        private readonly Dictionary<string, Tuple<Rectangle, string>> entryHoverText = new();

        // TODO
        //private readonly (Rectangle bounds, string text)[] scheduleHoverText = new (Rectangle bounds, string text)[slotsOnPage * 3];

        private readonly (Rectangle bounds, string text)[] questHoverText = new (Rectangle bounds, string text)[slotsOnPage];

        private readonly ClickableTextureComponent upButton;

        private readonly ClickableTextureComponent downButton;

        private readonly ClickableTextureComponent scrollBar;

        private Rectangle scrollBarRunner;

        private bool scrolling;

        /// <summary>Index of the NPC that's at the top of the menu</summary>
        private int slotPosition;

        public readonly List<ClickableTextureComponent> characterSlots = new();

        private readonly List<Schedule.NPCSchedule> schedules = new();

        private readonly List<ClickableTextureComponent> sprites = new();

        public readonly List<ClickableTextureComponent> questIcons = new();

        private readonly Texture2D icons = ModEntry.ModHelper.ModContent.Load<Texture2D>("assets/Icons.png");

        public Friendship emptyFriendship = new();

        /// <summary>LookupAnything</summary>
        public NPC hoveredNpc;


        public SchedulesPage(int initialSlotPosition = 0)
            : base(Game1.uiViewport.Width / 2 - (800 + IClickableMenu.borderWidth * 2) / 2, Game1.uiViewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2, 800 + 36 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2, showUpperRightCloseButton: true)
        {
            // filter npcs
            IEnumerable<KeyValuePair<string, Schedule.NPCSchedule>> filteredSchedules = Schedule.GetSchedules(ModEntry.Config.OnlyShowSocializableNPCs, ModEntry.Config.OnlyShowMetNPCs);
            // sort npcs
            filteredSchedules = ModEntry.Config.NPCSortOrder switch
            {
                ModConfig.SortType.AlphabeticalDescending => filteredSchedules.OrderByDescending(x => x.Value.DisplayName),
                ModConfig.SortType.HeartsAscending => filteredSchedules.OrderBy(x => Game1.player.getFriendshipLevelForNPC(x.Key)).ThenBy(x => x.Value.DisplayName),
                ModConfig.SortType.HeartsDescending => filteredSchedules.OrderByDescending(x => Game1.player.getFriendshipLevelForNPC(x.Key)).ThenBy(x => x.Value.DisplayName),
                _ => filteredSchedules.OrderBy(x => x.Value.DisplayName),
            };

            // get NPCs with quests
            Dictionary<string, string> npcsWithQuests = GetNPCsWithQuests();

            // map schedules into slots
            int itemIndex = 0;
            int lastQuestIndex = -1;
            Rectangle spriteBounds = new(base.xPositionOnScreen + IClickableMenu.borderWidth + 4, base.yPositionOnScreen + IClickableMenu.borderWidth + spriteSize / 2, 260, spriteSize);
            Regex nameRegex = new($"-{ModEntry.ModHelper.ModRegistry.ModID}-\\d");
            foreach (var item in filteredSchedules)
            {
                // if not host then need to get sprite info
                if (item.Value.NPC == null)
                {
                    // clear out extra count details from name (see Schedule.cs:241)
                    NPC npc = Game1.getCharacterFromName(nameRegex.Replace(item.Key, ""));
                    item.Value.NPC = npc;
                }
                // update CanAccess variable
                Schedule.UpdateScheduleEntriesCanAccess(item.Value);

                this.schedules.Add(item.Value);
                this.sprites.Add(new ClickableTextureComponent($"{item.Key}-sprite", spriteBounds.Clone(), null, "", item.Value.NPC?.Sprite.Texture ?? icons, item.Value.NPC?.getMugShotSourceRect() ?? new Rectangle(0, 0, 16, 24), 4f));
                ClickableTextureComponent characterSlot = new(new Rectangle(base.xPositionOnScreen + IClickableMenu.borderWidth, 0, base.width - IClickableMenu.borderWidth * 2, rowHeight), null, new Rectangle(0, 0, 0, 0), 4f)
                {
                    myID = itemIndex,
                    downNeighborID = itemIndex + 1,
                    upNeighborID = itemIndex - 1,
                };

                if (npcsWithQuests.TryGetValue(item.Key, out string questHoverText))
                {
                    int id = itemIndex + 999;
                    characterSlot.rightNeighborID = id;
                    ClickableTextureComponent questIcon = new($"{item.Key}-questIcon", new Rectangle(spriteBounds.Right - 30, spriteBounds.Y + 28, 20, 44), null, questHoverText, Game1.mouseCursors, new Rectangle(401, 492, 7, 18), 2f)
                    {
                        myID = id,
                        leftNeighborID = itemIndex,
                    };
                    if (lastQuestIndex != -1)
                    {
                        questIcon.upNeighborID = lastQuestIndex;
                        this.questIcons[lastQuestIndex].downNeighborID = id;
                    }
                    this.questIcons.Add(questIcon);
                    lastQuestIndex = itemIndex;
                }
                else
                {
                    this.questIcons.Add(null);
                }
                characterSlots.Add(characterSlot);

                itemIndex++;
            }

            // set up hover text
            for (int i = 0; i < this.questHoverText.Length; i++)
            {
                this.questHoverText[i] = (new Rectangle(spriteBounds.Right - 38, this.RowPosition(i) + 56, 30, 42), string.Empty);
            }

            base.initializeUpperRightCloseButton();

            // init scroll section
            this.upButton = new ClickableTextureComponent(new Rectangle(base.xPositionOnScreen + base.width + 16, base.yPositionOnScreen + 64, 44, 48), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), 4f);
            this.downButton = new ClickableTextureComponent(new Rectangle(base.xPositionOnScreen + base.width + 16, base.yPositionOnScreen + base.height - 64, 44, 48), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), 4f);
            this.scrollBar = new ClickableTextureComponent(new Rectangle(this.upButton.bounds.X + 12, this.upButton.bounds.Y + this.upButton.bounds.Height + 4, 24, 40), Game1.mouseCursors, new Rectangle(435, 463, 6, 10), 4f);
            this.scrollBarRunner = new Rectangle(this.scrollBar.bounds.X, this.upButton.bounds.Y + this.upButton.bounds.Height + 4, this.scrollBar.bounds.Width, base.height - 128 - this.upButton.bounds.Height - 8);
            // init slot position but don't overflow
            this.slotPosition = Math.Max(0, Math.Min(initialSlotPosition, this.sprites.Count - slotsOnPage));
            // set the scoll bar postion and sets yPos for characterSlots and sprites
            this.SetScrollBarToCurrentIndex();
        }

        #region override methods
        public override void applyMovementKey(int direction)
        {
            base.applyMovementKey(direction);
            if (base.currentlySnappedComponent != null)
            {
                int index = -1;
                if (this.characterSlots.Contains(base.currentlySnappedComponent))
                {
                    index = this.characterSlots.IndexOf(base.currentlySnappedComponent as ClickableTextureComponent);
                }
                else if (this.questIcons.Contains(base.currentlySnappedComponent))
                {
                    index = this.questIcons.IndexOf(base.currentlySnappedComponent as ClickableTextureComponent);
                }
                if (index != -1)
                {
                    if (index < this.slotPosition)
                    {
                        this.slotPosition = index;
                    }
                    else if (index >= this.slotPosition + slotsOnPage)
                    {
                        this.slotPosition = index - slotsOnPage + 1;
                    }
                    this.SetScrollBarToCurrentIndex();
                    if (Game1.options.snappyMenus && Game1.options.gamepadControls)
                    {
                        this.snapCursorToCurrentSnappedComponent();
                    }
                }
            }
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);
            Game1.drawDialogueBox(base.xPositionOnScreen, base.yPositionOnScreen, base.width, base.height, false, true);

            b.End();
            b.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, Utility.ScissorEnabled);
            base.drawHorizontalPartition(b, base.yPositionOnScreen + IClickableMenu.borderWidth + 128 + 4, small: true);
            base.drawHorizontalPartition(b, base.yPositionOnScreen + IClickableMenu.borderWidth + 192 + 32 + 20, small: true);
            base.drawHorizontalPartition(b, base.yPositionOnScreen + IClickableMenu.borderWidth + 320 + 36, small: true);
            base.drawHorizontalPartition(b, base.yPositionOnScreen + IClickableMenu.borderWidth + 384 + 32 + 52, small: true);

            // draw slots
            for (int i = this.slotPosition; i < this.slotPosition + slotsOnPage; i++)
            {
                if (i < this.sprites.Count)
                {
                    this.DrawNPCSlot(b, i);
                }
            }

            Rectangle origClip = b.GraphicsDevice.ScissorRectangle;
            Rectangle newClip = origClip;
            newClip.Y = 0;
            newClip.Height -= newClip.Y;
            if (newClip.Height > 0)
            {
                b.GraphicsDevice.ScissorRectangle = newClip;
                try
                {
                    base.drawVerticalPartition(b, base.xPositionOnScreen + 256 + 12, small: true);
                }
                finally
                {
                    b.GraphicsDevice.ScissorRectangle = origClip;
                }
            }

            this.upButton.draw(b);
            this.downButton.draw(b);
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), this.scrollBarRunner.X, this.scrollBarRunner.Y, this.scrollBarRunner.Width, this.scrollBarRunner.Height, Color.White, 4f);
            this.scrollBar.draw(b);
            if (!this.hoverText.Equals(""))
            {
                IClickableMenu.drawHoverText(b, this.hoverText, Game1.smallFont);
            }
            // draw close button
            base.draw(b);
            // draw cursor
            base.drawMouse(b);

            b.End();
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);
            Game1.activeClickableMenu = new SchedulesPage(this.slotPosition);
        }

        public override void leftClickHeld(int x, int y)
        {
            base.leftClickHeld(x, y);
            if (this.scrolling)
            {
                int y2 = this.scrollBar.bounds.Y;
                this.scrollBar.bounds.Y = Math.Min(base.yPositionOnScreen + base.height - 64 - 12 - this.scrollBar.bounds.Height, Math.Max(y, base.yPositionOnScreen + this.upButton.bounds.Height + 20));
                float percentage = (float)(y - this.scrollBarRunner.Y) / (float)this.scrollBarRunner.Height;
                this.slotPosition = Math.Min(this.sprites.Count - slotsOnPage, Math.Max(0, (int)((float)this.sprites.Count * percentage)));
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
            this.upButton.tryHover(x, y);
            this.downButton.tryHover(x, y);
            if (!ModEntry.Config.DisableHover)
            {
                string newHoverText = "";
                foreach (var hoverTextOption in this.entryHoverText)
                {
                    if (hoverTextOption.Value != null && hoverTextOption.Value.Item1.Contains(x, y))
                    {
                        newHoverText = hoverTextOption.Value.Item2;
                        break;
                    }
                }
                this.hoverText = newHoverText;
            }
            foreach (var (bounds, text) in this.questHoverText)
            {
                if (bounds.Contains(x, y))
                {
                    this.hoverText = text;
                    break;
                }
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            if (this.upButton.containsPoint(x, y) && this.slotPosition > 0)
            {
                this.UpArrowPressed();
                Game1.playSound("shwip");
                return;
            }
            if (this.downButton.containsPoint(x, y) && this.slotPosition < this.sprites.Count - slotsOnPage)
            {
                this.DownArrowPressed();
                Game1.playSound("shwip");
                return;
            }
            if (this.scrollBar.containsPoint(x, y))
            {
                this.scrolling = true;
                return;
            }
            if (!this.downButton.containsPoint(x, y) && x > base.xPositionOnScreen + base.width && x < base.xPositionOnScreen + base.width + 128 && y > base.yPositionOnScreen && y < base.yPositionOnScreen + base.height)
            {
                this.scrolling = true;
                this.leftClickHeld(x, y);
                this.releaseLeftClick(x, y);
                return;
            }
            // Show the NPC's full schedule in a new window
            for (int i = 0; i < this.characterSlots.Count; i++)
            {
                if (i < this.slotPosition || i >= this.slotPosition + slotsOnPage || !this.characterSlots[i].bounds.Contains(x, y))
                {
                    continue;
                }

                if (schedules[i].NPC != null)
                {
                    Game1.playSound("bigSelect");
                    ScheduleDetailsPage menu = new(i, schedules);
                    menu.exitFunction = delegate
                    {
                        Game1.activeClickableMenu = new SchedulesPage(menu.GetCurrentIndex());
                    };
                    Game1.activeClickableMenu = menu;
                    if (Game1.options.SnappyMenus)
                    {
                        menu.snapToDefaultClickableComponent();
                    }
                    return;
                }
                break;
            }
            this.slotPosition = Math.Max(0, Math.Min(this.sprites.Count - slotsOnPage, this.slotPosition));
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
        }

        public override void receiveScrollWheelAction(int direction)
        {
            base.receiveScrollWheelAction(direction);
            if (direction > 0 && this.slotPosition > 0)
            {
                this.UpArrowPressed();
                this.ConstrainSelectionToVisibleSlots();
                Game1.playSound("shiny4");
            }
            else if (direction < 0 && this.slotPosition < Math.Max(0, this.sprites.Count - slotsOnPage))
            {
                this.DownArrowPressed();
                this.ConstrainSelectionToVisibleSlots();
                Game1.playSound("shiny4");
            }
        }

        public override void releaseLeftClick(int x, int y)
        {
            base.releaseLeftClick(x, y);
            this.scrolling = false;
        }

        public override void snapCursorToCurrentSnappedComponent()
        {
            if (base.currentlySnappedComponent != null && this.characterSlots.Contains(base.currentlySnappedComponent))
            {
                Game1.setMousePosition(base.currentlySnappedComponent.bounds.Left + 64, base.currentlySnappedComponent.bounds.Center.Y);
            }
            else
            {
                base.snapCursorToCurrentSnappedComponent();
            }
        }

        public override void snapToDefaultClickableComponent()
        {
            if (this.slotPosition < this.characterSlots.Count)
            {
                base.currentlySnappedComponent = this.characterSlots[this.slotPosition];
            }
            this.snapCursorToCurrentSnappedComponent();
        }
        #endregion

        /// <returns>A dictionary of NPC names and their active quests.</returns>
        public static Dictionary<string, string> GetNPCsWithQuests()
        {
            Dictionary<string, string> npcsWithQuests = new();
            foreach (var quest in Game1.player.questLog)
            {
                List<string> names = new();
                switch (quest)
                {
                    case FishingQuest q:
                        names.Add(q.target.Value);
                        break;
                    case ItemDeliveryQuest q:
                        names.Add(q.target.Value);
                        break;
                    case LostItemQuest q:
                        names.Add(q.npcName.Value);
                        break;
                    case ResourceCollectionQuest q:
                        names.Add(q.target.Value);
                        break;
                    //case SecretLostItemQuest q:
                    //    names.Add(q.npcName.Value);
                    //    break;
                    case SlayMonsterQuest q:
                        names.Add(q.target.Value);
                        break;
                    case SocializeQuest q:
                        names.AddRange(q.whoToGreet);
                        break;
                }
                foreach (var name in names)
                {
                    if (npcsWithQuests.ContainsKey(name))
                    {
                        npcsWithQuests[name] = npcsWithQuests[name] + Environment.NewLine + quest.questTitle;
                    }
                    else
                    {
                        npcsWithQuests.Add(name, quest.questTitle);
                    }
                }
            }
            return npcsWithQuests;
        }

        #region private methods
        private void ConstrainSelectionToVisibleSlots()
        {
            if (this.characterSlots.Contains(base.currentlySnappedComponent))
            {
                int index = this.characterSlots.IndexOf(base.currentlySnappedComponent as ClickableTextureComponent);
                if (index < this.slotPosition)
                {
                    index = this.slotPosition;
                }
                else if (index >= this.slotPosition + slotsOnPage)
                {
                    index = this.slotPosition + slotsOnPage - 1;
                }
                base.currentlySnappedComponent = this.characterSlots[index];
                if (Game1.options.snappyMenus && Game1.options.gamepadControls)
                {
                    this.snapCursorToCurrentSnappedComponent();
                }
            }
        }

        private void DownArrowPressed()
        {
            this.slotPosition++;
            this.downButton.scale = 3.5f;
            this.SetScrollBarToCurrentIndex();
        }

        private void DrawNPCSlot(SpriteBatch b, int i)
        {
            ClickableTextureComponent sprite = this.sprites[i];
            Rectangle characterSlotBounds = this.characterSlots[i].bounds;
            var (entries, currentLocation, isOnSchedule, displayName, npc) = schedules[i];
            // highlight which NPC the mouse is over
            if (characterSlotBounds.Contains(Game1.getMouseX(), Game1.getMouseY()))
            {
                b.Draw(Game1.staminaRect, new Rectangle(base.xPositionOnScreen + IClickableMenu.borderWidth - 4, sprite.bounds.Y - 4, characterSlotBounds.Width, characterSlotBounds.Height - 12), Color.White * 0.25f);
                this.hoveredNpc = npc;
            }
            // draw sprite
            sprite.draw(b);

            // draw name
            float lineHeight = Game1.smallFont.MeasureString("W").Y;
            float russianOffsetY = ((LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ru || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko) ? ((0f - lineHeight) / 2f) : 0f);
            b.DrawString(Game1.dialogueFont, displayName, new Vector2((float)(base.xPositionOnScreen + IClickableMenu.borderWidth * 3 / 2 + 64 - 20 + 96) - Game1.dialogueFont.MeasureString(displayName).X / 2f, (float)(sprite.bounds.Y + 48) + russianOffsetY - 20), Game1.textColor);

            // draw quest icon
            this.questIcons[i]?.draw(b);

            int x = sprite.bounds.Right + partitionSize;
            int y = sprite.bounds.Y - 4;
            int slot = i - this.slotPosition;

            if (isOnSchedule && entries != null)
            {
                float yOffset = 0;
                int activeEntryIndex = 0;
                for (int j = 0; j < entries.Count; j++)
                {
                    if (entries[j].Time <= Game1.timeOfDay)
                    {
                        activeEntryIndex = j;
                    }
                }

                Dictionary<int, Schedule.ScheduleEntry> lines = new();
                int line1Index = activeEntryIndex == 0 ? activeEntryIndex : activeEntryIndex - 1;
                lines.Add(line1Index, entries.ElementAtOrDefault(line1Index));
                lines.Add(line1Index + 1, entries.ElementAtOrDefault(line1Index + 1));
                lines.Add(line1Index + 2, entries.ElementAtOrDefault(line1Index + 2));
                foreach (var line in lines)
                {
                    string entryString = line.Value?.ToString();
                    int stringWidth = string.IsNullOrEmpty(entryString) ? 0 : (int)Game1.smallFont.MeasureString(entryString).X + 2;
                    string key = $"{slot}-{line.Key - line1Index}";
                    this.entryHoverText[key] = string.IsNullOrEmpty(entryString) ? null : Tuple.Create(new Rectangle(x, y + (int)yOffset, stringWidth, (int)lineHeight), line.Value.HoverText);

                    if (line.Value != null)
                    {
                        if (line.Key == activeEntryIndex)
                        {
                            Utility.drawBoldText(b, entryString, Game1.smallFont, new Vector2(x, y + yOffset), Game1.textColor);
                        }
                        else
                        {
                            b.DrawString(Game1.smallFont, entryString, new Vector2(x, y + yOffset), Game1.textColor);
                        }
                        // draw inaccesible icon
                        if (!line.Value.CanAccess)
                        {
                            b.Draw(Game1.mouseCursors, new Vector2(x + stringWidth + 4, y + yOffset + 5), new Rectangle(218, 428, 7, 6), Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 1f);
                        }
                        yOffset += lineHeight;
                    }
                }
            }
            else
            {
                b.DrawString(Game1.smallFont, ModEntry.ModHelper.Translation.Get(entries == null ? "not_following_schedule_today" : "ignoring_schedule_today"), new Vector2(x, y), Game1.textColor);
                Utility.drawBoldText(b, currentLocation, Game1.smallFont, new Vector2(x, y + lineHeight), Game1.textColor);
                // clear hover text options
                this.entryHoverText[$"{slot}-0"] = null;
                this.entryHoverText[$"{slot}-1"] = null;
                this.entryHoverText[$"{slot}-2"] = null;
            }
        }

        private int RowPosition(int i)
        {
            int j = i - this.slotPosition;
            return base.yPositionOnScreen + IClickableMenu.borderWidth + j * rowHeight;
        }

        private void SetScrollBarToCurrentIndex()
        {
            int numOfSlots = this.sprites.Count;
            if (numOfSlots > 0)
            {
                this.scrollBar.bounds.Y = this.scrollBarRunner.Height / Math.Max(1, numOfSlots - slotsOnPage + 1) * this.slotPosition + this.upButton.bounds.Bottom + 4;
                if (this.slotPosition == numOfSlots - slotsOnPage)
                {
                    this.scrollBar.bounds.Y = this.downButton.bounds.Y - this.scrollBar.bounds.Height - 4;
                }
            }
            this.UpdateSlots();
        }

        private void UpArrowPressed()
        {
            this.slotPosition--;
            this.downButton.scale = 3.5f;
            this.SetScrollBarToCurrentIndex();
        }

        private void UpdateSlots()
        {
            // update y position of all characterSlots
            int index = 0;
            foreach (var slot in characterSlots)
            {
                slot.bounds.Y = this.RowPosition(index - 1) + 164;
                index++;
            }
            // update y position for visible sprites
            for (int i = this.slotPosition; i < this.slotPosition + slotsOnPage; i++)
            {
                // adding additional logging
                try
                {
                    int newSpriteY = this.RowPosition(i) + 64;
                    ClickableTextureComponent sprite = this.sprites.ElementAtOrDefault(i);
                    if (sprite != null)
                    {
                        sprite.bounds.Y = newSpriteY;
                    }
                    ClickableTextureComponent questIcon = this.questIcons.ElementAtOrDefault(i);
                    if (questIcon != null)
                    {
                        questIcon.bounds.Y = newSpriteY - 8;
                    }
                    this.questHoverText[i - this.slotPosition].text = questIcon?.hoverText ?? string.Empty;
                }
                catch
                {
                    ModEntry.Console.Log($"Error in the UpdateSlots method. Couldn't update the position of the character sprite at {i} out of {this.sprites.Count} total sprites. Current slotPosition: {this.slotPosition}", LogLevel.Error);
                }
            }
            base.populateClickableComponentList();
        }
        #endregion
    }
}
