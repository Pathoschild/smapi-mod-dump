/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using DeluxeJournal.Util;

using static StardewValley.Menus.ClickableComponent;

namespace DeluxeJournal.Menus
{
    /// <summary>IPage QuestLog wrapper.</summary>
    public class QuestLogPage : PageBase
    {
        private readonly ClickableTextureComponent _backButton;

        private readonly FieldInfo _currentPageField;
        private readonly FieldInfo _questPageField;
        private readonly Rectangle _eraseScrollRect;
        private readonly string _titlePlaceholder;

        private QuestLog? _questLog;

        public QuestLog? QuestLog
        {
            get
            {
                return _questLog;
            }

            set
            {
                if ((_questLog = value) != null)
                {
                    _questLog.backButton = _backButton;
                    _questLog.exitFunction = delegate
                    {
                        _questLog.exitFunction?.Invoke();
                        ExitJournalMenu(false);
                    };
                }
            }
        }

        private int CurrentPage => (int)(_currentPageField.GetValue(QuestLog) ?? 0);

        private int QuestPage => (int)(_questPageField.GetValue(QuestLog) ?? -1);

        public QuestLogPage(string name, Rectangle bounds, Texture2D tabTexture, ITranslationHelper translation)
            : this(name, translation.Get("ui.tab.quests"), bounds.X, bounds.Y, bounds.Width, bounds.Height, tabTexture, new Rectangle(0, 0, 16, 16))
        {
        }

        public QuestLogPage(string name, string title, int x, int y, int width, int height, Texture2D tabTexture, Rectangle tabSourceRect)
            : base(name, title, x, y, width, height, tabTexture, tabSourceRect)
        {
            _currentPageField = ReflectionHelper.TryGetField<QuestLog>("currentPage", BindingFlags.Instance | BindingFlags.NonPublic);
            _questPageField = ReflectionHelper.TryGetField<QuestLog>("questPage", BindingFlags.Instance | BindingFlags.NonPublic);
            _titlePlaceholder = Game1.content.LoadString("Strings\\StringsFromCSFiles:QuestLog.cs.11373");

            if (Game1.dialogueFont.MeasureString(title).X > Game1.dialogueFont.MeasureString(_titlePlaceholder).X)
            {
                _titlePlaceholder = title;
            }

            int scrollWidth = SpriteText.getWidthOfString(_titlePlaceholder) + 96;
            _eraseScrollRect = new Rectangle(xPositionOnScreen + (width - scrollWidth - 8) / 2, yPositionOnScreen - 72, scrollWidth, 72);

            _backButton = new ClickableTextureComponent(
                new Rectangle(xPositionOnScreen - 128, yPositionOnScreen + 8, 48, 44),
                Game1.mouseCursors,
                new Rectangle(352, 495, 12, 11),
                4f)
            {
                myID = QuestLog.region_backButton,
                rightNeighborID = CUSTOM_SNAP_BEHAVIOR,
                downNeighborID = CUSTOM_SNAP_BEHAVIOR,
                downNeighborImmutable = true
            };
        }

        public void PopulateQuestLogClickableComponentsList()
        {
            if (QuestLog != null)
            {
                QuestLog.populateClickableComponentList();
                QuestLog.allClickableComponents.AddRange(allClickableComponents);
            }
        }

        public override void OnVisible()
        {
            PopulateQuestLogClickableComponentsList();
        }

        public override void SnapToActiveTabComponent()
        {
            base.SnapToActiveTabComponent();

            if (QuestLog != null)
            {
                QuestLog.currentlySnappedComponent = currentlySnappedComponent;
            }
        }

        public override void snapToDefaultClickableComponent()
        {
            QuestLog?.snapToDefaultClickableComponent();
        }

        public override void receiveGamePadButton(Buttons b)
        {
            QuestLog?.receiveGamePadButton(b);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            QuestLog?.receiveLeftClick(x, y, playSound);
        }

        public override void leftClickHeld(int x, int y)
        {
            QuestLog?.leftClickHeld(x, y);
        }

        public override void releaseLeftClick(int x, int y)
        {
            QuestLog?.releaseLeftClick(x, y);
        }

        public override void receiveScrollWheelAction(int direction)
        {
            QuestLog?.receiveScrollWheelAction(direction);
        }

        public override void receiveKeyPress(Keys key)
        {
            if (QuestLog != null)
            {
                if (Game1.options.doesInputListContain(Game1.options.journalButton, key) && readyToClose())
                {
                    exitThisMenuNoSound();
                }

                currentlySnappedComponent = null;
                applyMovementKey(key);
                QuestLog.receiveKeyPress(key);

                if (currentlySnappedComponent != null)
                {
                    QuestLog.currentlySnappedComponent = currentlySnappedComponent;
                    QuestLog.snapCursorToCurrentSnappedComponent();
                }
            }
        }

        public override void applyMovementKey(int direction)
        {
            if (QuestLog != null)
            {
                ClickableComponent? old = QuestLog.currentlySnappedComponent;

                if (old != null && QuestPage == -1)
                {
                    switch (direction)
                    {
                        case Game1.down:
                            if (old.myID == QuestLog.region_backButton)
                            {
                                currentlySnappedComponent = QuestLog.getComponentWithID(TabRegion + 1);
                            }
                            break;
                        case Game1.left:
                            if (CurrentPage == 0 && old.myID < QuestLog.questsPerPage)
                            {
                                SnapToActiveTabComponent();
                            }
                            else if (CurrentPage > 0 && old.myID >= TabRegion)
                            {
                                currentlySnappedComponent = QuestLog.getComponentWithID(QuestLog.region_backButton);
                            }
                            break;
                    }
                }
            }
        }

        public override void performHoverAction(int x, int y)
        {
            QuestLog?.performHoverAction(x, y);
        }

        public override void update(GameTime time)
        {
            QuestLog?.update(time);
        }

        public override void draw(SpriteBatch b)
        {
            QuestLog?.draw(b);

            b.End();
            b.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.PointClamp);
            b.Draw(Game1.fadeToBlackRect, _eraseScrollRect, Color.Black * 0.75f);

            b.End();
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
            SpriteText.drawStringWithScrollCenteredAt(b, Title, xPositionOnScreen + width / 2, yPositionOnScreen - 64, _titlePlaceholder);
        }
    }
}
