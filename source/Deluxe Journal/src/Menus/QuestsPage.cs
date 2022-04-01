/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewValley.Quests;
using DeluxeJournal.Menus.Components;

using static StardewValley.Menus.ClickableComponent;

namespace DeluxeJournal.Menus
{
    /// <summary>Ported vanilla QuestLog to an IPage.</summary>
    public class QuestsPage : PageBase
    {
        public readonly List<ClickableComponent> questButtons;
        public readonly ClickableTextureComponent forwardButton;
        public readonly ClickableTextureComponent backButton;
        public readonly ClickableTextureComponent rewardBox;
        public readonly ClickableTextureComponent cancelQuestButton;
        public readonly ScrollComponent scrollComponent;

        protected IQuest? shownQuest;

        private int _currentPage;
        private int _questOnPage;
        private List<List<IQuest>> _pages;
        private List<string> _objectiveText;

        public QuestsPage(Rectangle bounds, Texture2D tabTexture, ITranslationHelper translation) :
            this("quests", translation.Get("ui.tab.quests"), bounds.X, bounds.Y, bounds.Width, bounds.Height, tabTexture, new Rectangle(0, 0, 16, 16), translation)
        {
        }

        public QuestsPage(string name, string title, int x, int y, int width, int height, Texture2D tabTexture, Rectangle tabSourceRect, ITranslationHelper translation) :
            base(name, title, x, y, width, height, tabTexture, tabSourceRect, translation)
        {
            questButtons = new List<ClickableComponent>();
            _pages = new List<List<IQuest>>();
            _objectiveText = new List<string>();

            Game1.dayTimeMoneyBox.DismissQuestPing();
            PaginateQuests();

            Rectangle bounds = new Rectangle(xPositionOnScreen + 16, 0, width - 32, (height - 32) / 6 + 4);

            for (int i = 0; i < 6; ++i)
            {
                bounds.Y = yPositionOnScreen + 16 + i * ((height - 32) / 6);
                questButtons.Add(new ClickableComponent(bounds, i.ToString())
                {
                    myID = i,
                    upNeighborID = i - 1,
                    downNeighborID = CUSTOM_SNAP_BEHAVIOR,
                    rightNeighborID = CUSTOM_SNAP_BEHAVIOR,
                    leftNeighborID = CUSTOM_SNAP_BEHAVIOR,
                    fullyImmutable = true
                });
            }

            forwardButton = new ClickableTextureComponent(
                new Rectangle(xPositionOnScreen + width + 16, yPositionOnScreen + height - 48, 48, 44),
                Game1.mouseCursors,
                new Rectangle(365, 495, 12, 11),
                4f)
            {
                myID = 101
            };

            backButton = new ClickableTextureComponent(
                new Rectangle(xPositionOnScreen - 128, yPositionOnScreen + 8, 48, 44),
                Game1.mouseCursors,
                new Rectangle(352, 495, 12, 11),
                4f)
            {
                myID = 102,
                rightNeighborID = CUSTOM_SNAP_BEHAVIOR,
                downNeighborID = CUSTOM_SNAP_BEHAVIOR,
                downNeighborImmutable = true
            };

            rewardBox = new ClickableTextureComponent(
                new Rectangle(xPositionOnScreen + width / 2 - 80, yPositionOnScreen + height - 128, 96, 96),
                Game1.mouseCursors,
                new Rectangle(293, 360, 24, 24),
                4f, drawShadow: true)
            {
                myID = 103
            };

            cancelQuestButton = new ClickableTextureComponent(
                new Rectangle(xPositionOnScreen + 4, yPositionOnScreen + height + 4, 48, 48),
                Game1.mouseCursors,
                new Rectangle(322, 498, 12, 12),
                4f, drawShadow: true)
            {
                myID = 104
            };

            Rectangle scrollBarBounds = new Rectangle(x + width + 16, y + 148, 24, height - 216);
            Rectangle scrollContentBounds = default;
            scrollContentBounds.X = x + 32;
            scrollContentBounds.Y = y + 96;
            scrollContentBounds.Width = width - 64;
            scrollContentBounds.Height = y + height - 32 - scrollContentBounds.Y;
            scrollContentBounds = Utility.ConstrainScissorRectToScreen(scrollContentBounds);

            scrollComponent = new ScrollComponent(scrollBarBounds, scrollContentBounds, 32, false);

            exitFunction = () => Game1.activeClickableMenu?.exitThisMenu();
        }

        private void PaginateQuests()
        {
            int page;

            _pages = new List<List<IQuest>>();

            for (int j = Game1.player.team.specialOrders.Count - 1; j >= 0; j--)
            {
                page = j / 6;

                while (_pages.Count <= page)
                {
                    _pages.Add(new List<IQuest>());
                }

                if (!Game1.player.team.specialOrders[j].IsHidden())
                {
                    _pages[page].Add(Game1.player.team.specialOrders[j]);
                }
            }

            for (int i = Game1.player.questLog.Count - 1; i >= 0; i--)
            {
                if (Game1.player.questLog[i] == null || Game1.player.questLog[i].destroy.Get())
                {
                    Game1.player.questLog.RemoveAt(i);
                }
                else if (Game1.player.questLog[i] == null || !Game1.player.questLog[i].IsHidden())
                {
                    page = Math.Max(0, Game1.player.visibleQuestCount - 1 - i) / 6;

                    while (_pages.Count <= page)
                    {
                        _pages.Add(new List<IQuest>());
                    }

                    _pages[page].Add(Game1.player.questLog[i]);
                }
            }

            if (_pages.Count == 0)
            {
                _pages.Add(new List<IQuest>());
            }

            _currentPage = Math.Min(Math.Max(_currentPage, 0), _pages.Count - 1);
            _questOnPage = -1;
        }

        public bool HasReward()
        {
            return shownQuest != null && shownQuest.HasReward();
        }

        public bool HasMoneyReward()
        {
            return shownQuest != null && shownQuest.HasMoneyReward();
        }

        public void ExitQuestPage(bool playSound = true)
        {
            if (shownQuest != null && shownQuest.OnLeaveQuestPage())
            {
                _pages[_currentPage].RemoveAt(_questOnPage);
            }

            _questOnPage = -1;
            PaginateQuests();

            if (playSound)
            {
                Game1.playSound("shwip");
            }

            if (Game1.options.SnappyMenus)
            {
                snapToDefaultClickableComponent();
            }
        }

        private void PreviousPage()
        {
            _currentPage--;
            Game1.playSound("shwip");

            if (Game1.options.SnappyMenus && _currentPage == 0)
            {
                snapToDefaultClickableComponent();
            }
        }

        private void NextPage()
        {
            _currentPage++;
            Game1.playSound("shwip");

            if (Game1.options.SnappyMenus && _currentPage == _pages.Count - 1)
            {
                snapToDefaultClickableComponent();
            }
        }

        public override void OnVisible()
        {
            if (_questOnPage != -1)
            {
                ExitQuestPage(false);
            }
        }

        public override bool ChildHasFocus()
        {
            return _questOnPage != -1;
        }

        protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
        {
            if (_questOnPage == -1)
            {
                switch (direction)
                {
                    case Game1.down:
                        if (oldID == backButton.myID)
                        {
                            currentlySnappedComponent = getComponentWithID(IPage.TabRegion + 1);
                        }
                        else if (oldID < 5 && _pages[_currentPage].Count - 1 > oldID)
                        {
                            currentlySnappedComponent = getComponentWithID(oldID + 1);
                        }
                        break;
                    case Game1.right:
                        if (oldID == backButton.myID)
                        {
                            currentlySnappedComponent = getComponentWithID(0);
                        }
                        else if (_currentPage < _pages.Count - 1)
                        {
                            currentlySnappedComponent = forwardButton;
                            currentlySnappedComponent.leftNeighborID = oldID;
                        }
                        break;
                    case Game1.left:
                        if (_currentPage > 0)
                        {
                            currentlySnappedComponent = backButton;
                            currentlySnappedComponent.rightNeighborID = oldID;
                        }
                        else if (oldID < 6)
                        {
                            SnapToActiveTabComponent();
                        }
                        break;
                }
            }
            else if (oldID == backButton.myID && direction == 2)
            {
                if (HasMoneyReward())
                {
                    currentlySnappedComponent = rewardBox;
                }
                else if (shownQuest != null && shownQuest.CanBeCancelled())
                {
                    currentlySnappedComponent = cancelQuestButton;
                }

                currentlySnappedComponent.upNeighborID = oldID;
            }

            snapCursorToCurrentSnappedComponent();
        }

        public override void snapToDefaultClickableComponent()
        {
            currentlySnappedComponent = (_questOnPage == -1) ? getComponentWithID(0) : backButton;
            snapCursorToCurrentSnappedComponent();
        }

        public override void receiveGamePadButton(Buttons b)
        {
            if (b == Buttons.RightTrigger && _questOnPage == -1 && _currentPage < _pages.Count - 1)
            {
                NextPage();
            }
            else if (b == Buttons.LeftTrigger && _questOnPage == -1 && _currentPage > 0)
            {
                PreviousPage();
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (Game1.activeClickableMenu == null)
            {
                return;
            }

            if (_questOnPage == -1)
            {
                for (int i = 0; i < questButtons.Count; ++i)
                {
                    if (_pages.Count > 0 & _pages[_currentPage].Count > i && questButtons[i].containsPoint(x, y))
                    {
                        Game1.playSound("smallSelect");

                        _questOnPage = i;
                        shownQuest = _pages[_currentPage][i];
                        _objectiveText = shownQuest.GetObjectiveDescriptions();
                        shownQuest.MarkAsViewed();

                        scrollComponent.ScrollAmount = 0;

                        if (Game1.options.SnappyMenus)
                        {
                            currentlySnappedComponent = backButton;
                            currentlySnappedComponent.rightNeighborID = CUSTOM_SNAP_BEHAVIOR;
                            snapCursorToCurrentSnappedComponent();
                        }

                        return;
                    }
                }

                if (_currentPage > 0 && backButton.containsPoint(x, y))
                {
                    PreviousPage();
                }
                else if (_currentPage < _pages.Count - 1 && forwardButton.containsPoint(x, y))
                {
                    NextPage();
                }
                else
                {
                    Game1.activeClickableMenu?.exitThisMenu();
                }

                return;
            }
            else if (shownQuest != null)
            {
                if (shownQuest.ShouldDisplayAsComplete() && shownQuest.HasMoneyReward() && rewardBox.containsPoint(x, y))
                {
                    Game1.player.Money += shownQuest.GetMoneyReward();
                    Game1.playSound("purchaseRepeat");
                    shownQuest.OnMoneyRewardClaimed();
                }
                else if (shownQuest is Quest quest && !quest.completed.Get() && quest.canBeCancelled.Get() && cancelQuestButton.containsPoint(x, y))
                {
                    quest.accepted.Value = false;

                    if (quest.dailyQuest.Get() && quest.dayQuestAccepted.Get() == Game1.Date.TotalDays)
                    {
                        Game1.player.acceptedDailyQuest.Set(newValue: false);
                    }

                    Game1.player.questLog.Remove(quest);
                    Game1.playSound("trashcan");
                    _pages[_currentPage].RemoveAt(_questOnPage);
                    _questOnPage = -1;

                    if (Game1.options.SnappyMenus && _currentPage == 0)
                    {
                        currentlySnappedComponent = getComponentWithID(0);
                        snapCursorToCurrentSnappedComponent();
                    }
                }
                else if (!scrollComponent.CanScroll() || backButton.containsPoint(x, y))
                {
                    ExitQuestPage();
                }
            }

            scrollComponent.ReceiveLeftClick(x, y, playSound);
        }

        public override void leftClickHeld(int x, int y)
        {
            if (!GameMenu.forcePreventClose)
            {
                scrollComponent.LeftClickHeld(x, y);
            }
        }

        public override void releaseLeftClick(int x, int y)
        {
            if (!GameMenu.forcePreventClose)
            {
                scrollComponent.ReleaseLeftClick(x, y);
            }
        }

        public override void receiveScrollWheelAction(int direction)
        {
            scrollComponent.Scroll(direction);
        }

        public override void receiveKeyPress(Keys key)
        {
            if (_questOnPage != -1 && Game1.isAnyGamePadButtonBeingPressed() &&
                Game1.options.doesInputListContain(Game1.options.menuButton, key))
            {
                ExitQuestPage();
            }
            else
            {
                base.receiveKeyPress(key);
            }
        }

        public override void applyMovementKey(int direction)
        {
            base.applyMovementKey(direction);

            switch (direction)
            {
                case Game1.up:
                    scrollComponent.Scroll(1);
                    break;
                case Game1.down:
                    scrollComponent.Scroll(-1);
                    break;
            }
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);

            if (_questOnPage == -1)
            {
                for (int i = 0; i < questButtons.Count; ++i)
                {
                    if (_pages.Count > 0 && _pages[0].Count > i && questButtons[i].containsPoint(x, y) &&
                        !questButtons[i].containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()))
                    {
                        Game1.playSound("Cowboy_gunshot");
                    }
                }
            }
            else if ((shownQuest?.CanBeCancelled() ?? false) && cancelQuestButton.containsPoint(x, y))
            {
                HoverText = Game1.content.LoadString("Strings\\StringsFromCSFiles:QuestLog.cs.11364");
            }

            backButton.tryHover(x, y, 0.2f);
            forwardButton.tryHover(x, y, 0.2f);
            cancelQuestButton.tryHover(x, y, 0.2f);
            scrollComponent.TryHover(x, y);
        }

        public override void update(GameTime time)
        {
            base.update(time);

            if (_questOnPage != -1 && HasReward())
            {
                rewardBox.scale = rewardBox.baseScale + Game1.dialogueButtonScale / 20f;
            }
        }

        public override void draw(SpriteBatch b)
        {
            if (_questOnPage == -1)
            {
                Rectangle textureRect = new Rectangle(384, 396, 15, 15);

                for (int i = 0; i < questButtons.Count; ++i)
                {
                    if (_pages.Count > 0 && _pages[_currentPage].Count > i)
                    {
                        ClickableComponent questButton = questButtons[i];
                        IQuest quest = _pages[_currentPage][i];

                        drawTextureBox(b,
                            Game1.mouseCursors,
                            textureRect,
                            questButton.bounds.X,
                            questButton.bounds.Y,
                            questButton.bounds.Width,
                            questButton.bounds.Height,
                            questButton.containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()) ? Color.Wheat : Color.White,
                            4f,
                            drawShadow: false);

                        if (quest.ShouldDisplayAsNew() || quest.ShouldDisplayAsComplete())
                        {
                            Utility.drawWithShadow(b,
                                Game1.mouseCursors,
                                new Vector2(questButton.bounds.X + 68, questButton.bounds.Y + 44),
                                new Rectangle(quest.ShouldDisplayAsComplete() ? 341 : 317, 410, 23, 9),
                                Color.White,
                                0f,
                                new Vector2(11f, 4f),
                                4f + Game1.dialogueButtonScale * 10f / 250f,
                                flipped: false, 0.99f);
                        }
                        else
                        {
                            Utility.drawWithShadow(b,
                                Game1.mouseCursors,
                                new Vector2(questButton.bounds.X + 32, questButton.bounds.Y + 28),
                                quest.IsTimedQuest() ? new Rectangle(410, 501, 9, 9) : new Rectangle(395 + (quest.IsTimedQuest() ? 3 : 0), 497, 3, 8),
                                Color.White,
                                0f,
                                Vector2.Zero,
                                4f,
                                flipped: false, 0.99f);
                        }

                        SpriteText.drawString(b, quest.GetName(), questButton.bounds.X + 128 + 4, questButton.bounds.Y + 20);
                    }
                }
            }
            else if (shownQuest != null)
            {
                string daysLeftText = Game1.content.LoadString("Strings\\StringsFromCSFiles:QuestLog.cs.11374", shownQuest.GetDaysLeft());
                string finalDayText = Game1.content.LoadString("Strings\\StringsFromCSFiles:Quest_FinalDay");
                string rewardText = Game1.content.LoadString("Strings\\StringsFromCSFiles:QuestLog.cs.11376");
                string moneyText = Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", shownQuest.GetMoneyReward());
                string description = Game1.parseText(shownQuest.GetDescription(), Game1.dialogueFont, width - 128);
                Vector2 descriptionSize = Game1.dialogueFont.MeasureString(description);
                int questTimeOffset = 0;

                if (shownQuest.IsTimedQuest() && shownQuest.GetDaysLeft() > 0)
                {
                    questTimeOffset = Math.Max(32, SpriteText.getWidthOfString(shownQuest.GetName()) / 3) - 32;
                }

                SpriteText.drawStringHorizontallyCenteredAt(b, shownQuest.GetName(), xPositionOnScreen + width / 2 + questTimeOffset, yPositionOnScreen + 32);

                if (shownQuest.IsTimedQuest() && shownQuest.GetDaysLeft() > 0)
                {
                    Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2(xPositionOnScreen + 32, yPositionOnScreen + 48 - 8), new Rectangle(410, 501, 9, 9), Color.White, 0f, Vector2.Zero, 4f, flipped: false, 0.99f);
                    Utility.drawTextWithShadow(b,
                        Game1.parseText((shownQuest.GetDaysLeft() > 1) ? daysLeftText : finalDayText, Game1.dialogueFont, width - 128),
                        Game1.dialogueFont,
                        new Vector2(xPositionOnScreen + 80, yPositionOnScreen + 48 - 8),
                        Game1.textColor);
                }

                scrollComponent.BeginScissorTest(b);
                Utility.drawTextWithShadow(b, description, Game1.dialogueFont, new Vector2(xPositionOnScreen + 64, (float)yPositionOnScreen - scrollComponent.ScrollAmount + 96f), Game1.textColor);

                float yPos = (yPositionOnScreen + 96) + descriptionSize.Y + 32f - scrollComponent.ScrollAmount;

                if (shownQuest.ShouldDisplayAsComplete())
                {
                    scrollComponent.EndScissorTest(b);

                    SpriteText.drawString(b, rewardText, xPositionOnScreen + 32 + 4, rewardBox.bounds.Y + 21 + 4);
                    rewardBox.draw(b);

                    if (HasMoneyReward())
                    {
                        b.Draw(Game1.mouseCursors,
                            new Vector2(rewardBox.bounds.X + 16, (float)(rewardBox.bounds.Y + 16) - Game1.dialogueButtonScale / 2f),
                            new Rectangle(280, 410, 16, 16),
                            Color.White,
                            0f,
                            Vector2.Zero,
                            4f,
                            SpriteEffects.None,
                            1f);

                        SpriteText.drawString(b, moneyText, xPositionOnScreen + 448, rewardBox.bounds.Y + 21 + 4);
                    }
                }
                else
                {
                    for (int j = 0; j < _objectiveText.Count; ++j)
                    {
                        SpecialOrder? shownSpecialOrder = shownQuest as SpecialOrder;
                        string parsedText = Game1.parseText(_objectiveText[j], width: width - 192, whichFont: Game1.dialogueFont);
                        Color textColor = Color.DarkBlue;
                        bool displayAsComplete = false;

                        if (shownSpecialOrder != null)
                        {
                            displayAsComplete = shownSpecialOrder.objectives[j].IsComplete();
                        }

                        if (!displayAsComplete)
                        {
                            Utility.drawWithShadow(b,
                                Game1.mouseCursors,
                                new Vector2((float)(xPositionOnScreen + 96) + 8f * Game1.dialogueButtonScale / 10f, yPos),
                                new Rectangle(412, 495, 5, 4),
                                Color.White,
                                (float)Math.PI / 2f,
                                Vector2.Zero);
                        }
                        else
                        {
                            textColor = Game1.unselectedOptionColor;
                        }

                        Utility.drawTextWithShadow(b, parsedText, Game1.dialogueFont, new Vector2(xPositionOnScreen + 128, yPos - 8f), textColor);
                        yPos += Game1.dialogueFont.MeasureString(parsedText).Y;

                        if (shownSpecialOrder != null)
                        {
                            OrderObjective orderObjective = shownSpecialOrder.objectives[j];

                            if (orderObjective.GetMaxCount() > 1 && orderObjective.ShouldShowProgress())
                            {
                                Color dark_bar_color = Color.DarkRed;
                                Color bar_color = Color.Red;

                                if (orderObjective.GetCount() >= orderObjective.GetMaxCount())
                                {
                                    bar_color = Color.LimeGreen;
                                    dark_bar_color = Color.Green;
                                }

                                int inset = 64;
                                int objective_count_draw_width = 160;
                                int notches = 4;
                                Rectangle bar_background_source = new Rectangle(0, 224, 47, 12);
                                Rectangle bar_notch_source = new Rectangle(47, 224, 1, 12);
                                int bar_horizontal_padding = 3;
                                int bar_vertical_padding = 3;
                                int slice_width = 5;
                                string objective_count_text = orderObjective.GetCount() + "/" + orderObjective.GetMaxCount();
                                int max_text_width = (int)Game1.dialogueFont.MeasureString(orderObjective.GetMaxCount() + "/" + orderObjective.GetMaxCount()).X;
                                int count_text_width = (int)Game1.dialogueFont.MeasureString(objective_count_text).X;
                                int text_draw_position = xPositionOnScreen + width - inset - count_text_width;
                                int max_text_draw_position = xPositionOnScreen + width - inset - max_text_width;
                                Utility.drawTextWithShadow(b, objective_count_text, Game1.dialogueFont, new Vector2(text_draw_position, yPos), Color.DarkBlue);
                                Rectangle bar_draw_position = new Rectangle(xPositionOnScreen + inset, (int)yPos, width - inset * 2 - objective_count_draw_width, bar_background_source.Height * 4);
                                
                                if (bar_draw_position.Right > max_text_draw_position - 16)
                                {
                                    int adjustment = bar_draw_position.Right - (max_text_draw_position - 16);
                                    bar_draw_position.Width -= adjustment;
                                }
                                
                                b.Draw(Game1.mouseCursors2, new Rectangle(bar_draw_position.X, bar_draw_position.Y, slice_width * 4, bar_draw_position.Height), new Rectangle(bar_background_source.X, bar_background_source.Y, slice_width, bar_background_source.Height), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
                                b.Draw(Game1.mouseCursors2, new Rectangle(bar_draw_position.X + slice_width * 4, bar_draw_position.Y, bar_draw_position.Width - 2 * slice_width * 4, bar_draw_position.Height), new Rectangle(bar_background_source.X + slice_width, bar_background_source.Y, bar_background_source.Width - 2 * slice_width, bar_background_source.Height), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
                                b.Draw(Game1.mouseCursors2, new Rectangle(bar_draw_position.Right - slice_width * 4, bar_draw_position.Y, slice_width * 4, bar_draw_position.Height), new Rectangle(bar_background_source.Right - slice_width, bar_background_source.Y, slice_width, bar_background_source.Height), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
                                float quest_progress = (float)orderObjective.GetCount() / (float)orderObjective.GetMaxCount();
                                
                                if (orderObjective.GetMaxCount() < notches)
                                {
                                    notches = orderObjective.GetMaxCount();
                                }
                                
                                bar_draw_position.X += 4 * bar_horizontal_padding;
                                bar_draw_position.Width -= 4 * bar_horizontal_padding * 2;
                                
                                for (int k = 1; k < notches; k++)
                                {
                                    b.Draw(Game1.mouseCursors2, new Vector2((float)bar_draw_position.X + (float)bar_draw_position.Width * ((float)k / (float)notches), bar_draw_position.Y), bar_notch_source, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.5f);
                                }
                                
                                bar_draw_position.Y += 4 * bar_vertical_padding;
                                bar_draw_position.Height -= 4 * bar_vertical_padding * 2;
                                Rectangle rect = new Rectangle(bar_draw_position.X, bar_draw_position.Y, (int)((float)bar_draw_position.Width * quest_progress) - 4, bar_draw_position.Height);
                                b.Draw(Game1.staminaRect, rect, null, bar_color, 0f, Vector2.Zero, SpriteEffects.None, (float)rect.Y / 10000f);
                                rect.X = rect.Right;
                                rect.Width = 4;
                                b.Draw(Game1.staminaRect, rect, null, dark_bar_color, 0f, Vector2.Zero, SpriteEffects.None, (float)rect.Y / 10000f);
                                yPos += (float)((bar_background_source.Height + 4) * 4);
                            }
                        }

                        scrollComponent.ContentHeight = (int)(yPos + scrollComponent.ScrollAmount - scrollComponent.ContentBounds.Y);
                    }

                    scrollComponent.EndScissorTest(b);

                    if (shownQuest.CanBeCancelled())
                    {
                        cancelQuestButton.draw(b);
                    }

                    if (scrollComponent.CanScroll())
                    {
                        Rectangle scrollBounds = scrollComponent.ContentBounds;

                        if (scrollComponent.ScrollAmount > 0)
                        {
                            b.Draw(Game1.staminaRect, new Rectangle(scrollBounds.X, scrollBounds.Top, scrollBounds.Width, 4), Color.Black * 0.15f);
                        }
                        else
                        {
                            b.Draw(Game1.staminaRect, new Rectangle(scrollBounds.X, scrollBounds.Bottom - 4, scrollBounds.Width, 4), Color.Black * 0.15f);
                        }
                    }
                }
            }

            scrollComponent.DrawScrollBar(b);

            if (_currentPage > 0 || _questOnPage != -1)
            {
                backButton.draw(b);
            }

            if (_currentPage < _pages.Count - 1 && _questOnPage == -1)
            {
                forwardButton.draw(b);
            }
        }
    }
}
