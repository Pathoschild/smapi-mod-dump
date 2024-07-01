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

using DeluxeJournal.Framework;
using DeluxeJournal.Menus.Components;
using DeluxeJournal.Task;

using static StardewValley.Menus.ClickableComponent;

namespace DeluxeJournal.Menus
{
    /// <summary>TasksPage child menu for adding a new task.</summary>
    public class AddTaskMenu : IClickableMenu
    {
        public readonly ClickableTextureComponent optionsButton;
        public readonly ClickableTextureComponent closeTipButton;
        public readonly ClickableTextureComponent cancelButton;
        public readonly ClickableTextureComponent okButton;
        public readonly ClickableTextureComponent smartOkButton;

        public readonly ClickableComponent textBoxCC;

        private readonly SideScrollingTextBox _textBox;
        private readonly SmartIconComponent _smartIcons;

        private readonly ITranslationHelper _translation;
        private readonly Config _config;
        private readonly TaskParser _taskParser;
        private string _previousText;
        private string _hoverText;

        public AddTaskMenu(ITranslationHelper translation) : base(0, 0, 612, 64)
        {
            if (DeluxeJournalMod.Config is not Config config)
            {
                throw new InvalidOperationException("AddTaskMenu created before mod entry.");
            }

            xPositionOnScreen = (Game1.uiViewport.Width / 2) - (width / 2);
            yPositionOnScreen = (Game1.uiViewport.Height / 2) - (height / 2);

            _translation = translation;
            _config = config;
            _taskParser = new TaskParser(translation);
            _previousText = string.Empty;
            _hoverText = string.Empty;

            optionsButton = new ClickableTextureComponent(
                new Rectangle(xPositionOnScreen - 88, yPositionOnScreen - 6, 64, 64),
                DeluxeJournalMod.UiTexture,
                new Rectangle(16, 80, 16, 16),
                4f)
            {
                myID = 100,
                downNeighborID = SNAP_AUTOMATIC,
                rightNeighborID = SNAP_AUTOMATIC
            };

            closeTipButton = new ClickableTextureComponent(
                new Rectangle(xPositionOnScreen + 444, yPositionOnScreen + height + 18, 24, 24),
                Game1.mouseCursors,
                new Rectangle(337, 494, 12, 12),
                2f)
            {
                myID = 101,
                upNeighborID = 0,
                downNeighborID = SNAP_AUTOMATIC,
                rightNeighborID = SNAP_AUTOMATIC,
                leftNeighborID = SNAP_AUTOMATIC,
                visible = false
            };

            cancelButton = new ClickableTextureComponent(
                new Rectangle(xPositionOnScreen + width + 32, yPositionOnScreen - 6, 64, 64),
                DeluxeJournalMod.UiTexture,
                new Rectangle(0, 80, 16, 16),
                4f)
            {
                myID = 102,
                downNeighborID = SNAP_AUTOMATIC,
                leftNeighborID = 0
            };

            okButton = new ClickableTextureComponent(
                new Rectangle(xPositionOnScreen + width - 36, yPositionOnScreen + height + 18, 64, 64),
                DeluxeJournalMod.UiTexture,
                new Rectangle(32, 80, 16, 16),
                4f)
            {
                myID = 103,
                upNeighborID = 0,
                rightNeighborID = 102,
                leftNeighborID = SNAP_AUTOMATIC
            };

            smartOkButton = new ClickableTextureComponent(
                new Rectangle(xPositionOnScreen + width - 108, okButton.bounds.Y, 64, 64),
                DeluxeJournalMod.UiTexture,
                new Rectangle(48, 80, 16, 16),
                4f)
            {
                myID = 104,
                upNeighborID = 0,
                rightNeighborID = SNAP_AUTOMATIC,
                leftNeighborID = SNAP_AUTOMATIC
            };

            _smartIcons = new SmartIconComponent(new Rectangle(xPositionOnScreen, okButton.bounds.Y, 56, 56), _taskParser, 200, 0, -1)
            {
                Visible = false
            };

            _textBox = new SideScrollingTextBox(null, null, Game1.dialogueFont, Game1.textColor)
            {
                X = xPositionOnScreen,
                Y = yPositionOnScreen,
                Width = width,
                Height = 192,
                Selected = true
            };

            textBoxCC = new ClickableComponent(new Rectangle(_textBox.X, _textBox.Y, _textBox.Width, 64), "")
            {
                myID = 0,
                downNeighborID = SNAP_AUTOMATIC,
                rightNeighborID = SNAP_AUTOMATIC,
                leftNeighborID = SNAP_AUTOMATIC
            };

            Game1.playSound("shwip");

            exitFunction = OnExit;
        }

        private void OnExit()
        {
            _textBox.Selected = false;
            _hoverText = string.Empty;

            if (Game1.options.SnappyMenus)
            {
                Game1.activeClickableMenu?.snapToDefaultClickableComponent();
            }
        }

        public override void populateClickableComponentList()
        {
            base.populateClickableComponentList();
            allClickableComponents.AddRange(_smartIcons.GetClickableComponents());
        }

        public override void snapToDefaultClickableComponent()
        {
            currentlySnappedComponent = textBoxCC;
            snapCursorToCurrentSnappedComponent();
        }

        public override bool IsAutomaticSnapValid(int direction, ClickableComponent a, ClickableComponent b)
        {
            return base.IsAutomaticSnapValid(direction, a, b) && (b != smartOkButton || _taskParser.MatchFound());
        }

        public override void receiveGamePadButton(Buttons b)
        {
            switch (b)
            {
                case Buttons.B:
                    currentlySnappedComponent = cancelButton;
                    snapCursorToCurrentSnappedComponent();
                    break;
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (cancelButton.containsPoint(x, y))
            {
                exitThisMenu(playSound);
            }
            else if (okButton.containsPoint(x, y))
            {
                if (_textBox.Text.Length > 0)
                {
                    AddTaskAndExit(false);
                }
            }
            else if (smartOkButton.containsPoint(x, y))
            {
                if (_textBox.Text.Length > 0 && _taskParser.MatchFound())
                {
                    AddTaskAndExit(true);
                }
            }
            else if (optionsButton.containsPoint(x, y))
            {
                SetChildMenu(new TaskOptionsMenu(_previousText, _taskParser, _translation));
            }
            else if (closeTipButton.containsPoint(x, y))
            {
                _config.ShowSmartAddTip = false;
            }
            else if (textBoxCC.containsPoint(x, y))
            {
                _textBox.SelectMe();

                // HACK: TextBox.Update() will not show the gamepad keyboard immediately after opening
                //  this menu due to Game1.lastCursorMotionWasMouse being true, regardless of the fact
                //  that snapToDefaultClickableComponent() should invalidate this flag. As a workaround,
                //  manually show the keyboard without checking mouse state.
                if (Game1.options.SnappyMenus)
                {
                    Game1.showTextEntry(_textBox);
                }
            }
        }

        public override void receiveKeyPress(Keys key)
        {
            if (Game1.options.SnappyMenus)
            {
                applyMovementKey(key);
            }
            else if (!_textBox.Selected)
            {
                base.receiveKeyPress(key);
            }

            switch (key)
            {
                case Keys.Enter:
                    AddTaskAndExit(_config.EnableDefaultSmartAdd);
                    break;
                case Keys.Escape:
                    exitThisMenu();
                    break;
            }
        }

        public override void performHoverAction(int x, int y)
        {
            _hoverText = string.Empty;

            if (optionsButton.containsPoint(x, y))
            {
                _hoverText = _translation.Get("ui.tasks.new.optionsbutton.hover");
            }
            else if (cancelButton.containsPoint(x, y))
            {
                _hoverText = _translation.Get("ui.tasks.new.cancelbutton.hover");
            }
            else if (_taskParser.MatchFound())
            {
                if (smartOkButton.containsPoint(x, y))
                {
                    _hoverText = _translation.Get("ui.tasks.new.smartokbutton.hover");
                }
                else if (_smartIcons.TryGetHoverText(x, y, _translation, out string hoverText))
                {
                    _hoverText = hoverText;
                }
            }

            optionsButton.tryHover(x, y);
            cancelButton.tryHover(x, y, 0.2f);
            closeTipButton.tryHover(x, y, 0.2f);

            if (_textBox.Text.Length > 0)
            {
                okButton.tryHover(x, y);

                if (_taskParser.MatchFound())
                {
                    smartOkButton.tryHover(x, y);
                }
                else
                {
                    smartOkButton.scale = smartOkButton.baseScale;
                }
            }
            else
            {
                okButton.scale = okButton.baseScale;
            }
        }

        public override void update(GameTime time)
        {
            string text = _textBox.Text;

            if (_previousText != text)
            {
                _previousText = text;
                _taskParser.Parse(text);
            }

            if (!_textBox.Selected && !Game1.options.SnappyMenus)
            {
                _textBox.SelectMe();
            }
        }

        public override void draw(SpriteBatch b)
        {
            if (GetChildMenu() != null)
            {
                return;
            }

            string title = _translation.Get("ui.tasks.new");
            Point iconLocation = new(_smartIcons.Location.X, okButton.bounds.Y);

            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            SpriteText.drawStringWithScrollCenteredAt(b, title, xPositionOnScreen + width / 2, yPositionOnScreen - 96);

            _textBox.Draw(b);

            if (closeTipButton.visible = _config.ShowSmartAddTip)
            {
                string text = Game1.parseText(_translation.Get("ui.tasks.new.smarttip"), Game1.smallFont, 396);
                int extraLineSpacing = Math.Max(0, text.Count(c => c == '\n') - 1) * Game1.smallFont.LineSpacing;

                DrawInfoBox(b, text, new Rectangle(_textBox.X - 32, _textBox.Y + 34, 544, 140 + extraLineSpacing));
                iconLocation.Y += extraLineSpacing + 80;

                if (new Rectangle(_textBox.X, _textBox.Y + 72, 480, 76).Contains(Game1.getOldMouseX(), Game1.getOldMouseY()))
                {
                    closeTipButton.draw(b);
                }
            }

            optionsButton.draw(b);
            cancelButton.draw(b);
            okButton.draw(b, (_textBox.Text.Length > 0) ? Color.White : Color.Gray * 0.8f, 0.88f);
            smartOkButton.draw(b, (_textBox.Text.Length > 0 && _taskParser.MatchFound()) ? Color.White : Color.Gray * 0.8f, 0.88f);

            _smartIcons.Location = iconLocation;
            _smartIcons.Draw(b, Color.White);

            if (_hoverText.Length > 0)
            {
                drawHoverText(b, _hoverText, Game1.dialogueFont);
            }
        }

        private void DrawInfoBox(SpriteBatch b, string text, Rectangle bounds)
        {
            Texture2D texture = Game1.menuTexture;

            // Left
            b.Draw(texture, new Vector2(bounds.X, bounds.Y), Game1.getSourceRectForStandardTileSheet(texture, 4), Color.White);
            b.Draw(texture, new Rectangle(bounds.X, bounds.Y + 64, 64, bounds.Height - 128), Game1.getSourceRectForStandardTileSheet(texture, 8), Color.White);
            b.Draw(texture, new Vector2(bounds.X, bounds.Bottom - 64), Game1.getSourceRectForStandardTileSheet(texture, 12), Color.White);

            // Right
            b.Draw(texture, new Vector2(bounds.Right - 64, bounds.Bottom - 64), Game1.getSourceRectForStandardTileSheet(texture, 15), Color.White);
            b.Draw(texture, new Rectangle(bounds.Right - 64, bounds.Y + 36, 64, bounds.Height - 100), Game1.getSourceRectForStandardTileSheet(texture, 11), Color.White);

            // Middle
            b.Draw(texture, new Rectangle(bounds.X + 64, bounds.Y, bounds.Width - 96, 64), Game1.getSourceRectForStandardTileSheet(texture, 6), Color.White);
            b.Draw(texture, new Rectangle(bounds.X + 64, bounds.Bottom - 64, bounds.Width - 128, 64), Game1.getSourceRectForStandardTileSheet(texture, 14), Color.White);
            b.Draw(texture, new Rectangle(bounds.X + 36, bounds.Y + 44, bounds.Width - 76, bounds.Height - 80), Game1.getSourceRectForStandardTileSheet(texture, 9), Color.White);

            Utility.drawWithShadow(b,
                ChatBox.emojiTexture,
                new Vector2(_textBox.X + 12, _textBox.Y + 84),
                new Rectangle(54, 90, 9, 9),
                Color.White,
                0,
                Vector2.Zero,
                2f,
                horizontalShadowOffset: -2,
                verticalShadowOffset: 2);

            Utility.drawTextWithShadow(b, text, Game1.smallFont, new Vector2(_textBox.X + 38, _textBox.Y + 80), Color.Maroon);
        }

        private void AddTaskAndExit(bool smart = true)
        {
            string name = _textBox.Text.Trim();

            if (name.Length > 0)
            {
                AddTask(name, smart);
            }

            Game1.playSound("bigSelect");
            exitThisMenuNoSound();
        }

        private void AddTask(string name, bool smart = true)
        {
            if (GetParentMenu() is TasksPage tasksPage)
            {
                tasksPage.AddTask(smart ? _taskParser.GenerateTask(name) : new BasicTask(name));
            }
        }
    }
}
