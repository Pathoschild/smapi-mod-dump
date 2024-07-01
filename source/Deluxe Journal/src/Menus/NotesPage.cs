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
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using DeluxeJournal.Menus.Components;

using static StardewValley.Menus.ClickableComponent;

namespace DeluxeJournal.Menus
{
    /// <summary>Notes page.</summary>
    public class NotesPage : PageBase
    {
        private static readonly PerScreen<int> ScrollAmountPerScreen = new();

        public readonly ClickableComponent gamepadCursorArea;

        private readonly MultilineTextBox _textBox;
        private readonly NotesOverlay? _overlay;
        private readonly ButtonComponent _trashButton;
        private readonly ButtonComponent _promptYesButton;
        private readonly ButtonComponent _promptNoButton;

        private int _totalTimeSeconds;
        private int _saveTimeSeconds;
        private bool _dirty;
        private bool _promptActive;
        private int _promptChoice;

        public NotesPage(string name, Rectangle bounds, Texture2D tabTexture, ITranslationHelper translation, string rawText)
            : this(name, translation.Get("ui.tab.notes"), bounds.X, bounds.Y, bounds.Width, bounds.Height, tabTexture, new Rectangle(32, 0, 16, 16), rawText)
        {
        }

        public NotesPage(string name, string title, int x, int y, int width, int height, Texture2D tabTexture, Rectangle tabSourceRect, string rawText)
            : base(name, title, x, y, width, height, tabTexture, tabSourceRect)
        {
            _overlay = Game1.onScreenMenus.Where(menu => menu is NotesOverlay).FirstOrDefault() as NotesOverlay;

            _textBox = new MultilineTextBox(
                new Rectangle(xPositionOnScreen + 32, yPositionOnScreen + 32, width - 64, height - 64),
                null,
                null,
                Game1.dialogueFont,
                Game1.textColor)
            {
                RawText = rawText
            };

            _textBox.ScrollComponent.ScrollAmount = ScrollAmountPerScreen.Value;
            _textBox.ScrollComponent.OnScroll += (self) => ScrollAmountPerScreen.Value = self.ScrollAmount;

            gamepadCursorArea = new ClickableComponent(new Rectangle(_textBox.Bounds.Right - 16, _textBox.Bounds.Bottom - 16, 16, 16), "")
            {
                myID = 0,
                leftNeighborID = CUSTOM_SNAP_BEHAVIOR,
                fullyImmutable = true
            };

            string promptYes = Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_Yes");
            string promptNo = Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_No");
            int promptButtonWidth = Math.Max(SpriteText.getWidthOfString(promptYes), SpriteText.getWidthOfString(promptNo)) + 64;
            int promptButtonHeight = Math.Max(SpriteText.getHeightOfString(promptYes), SpriteText.getHeightOfString(promptNo)) + 12;

            _trashButton = new ButtonComponent(
                new(x + width - 20, y + 44, 48, 48),
                DeluxeJournalMod.UiTexture!,
                new(66, 82, 12, 12),
                4f)
            {
                SoundCueName = "trashcanlid",
                OnClick = delegate
                {
                    _promptActive = true;
                    _promptChoice = 0;
                }
            };

            _promptYesButton = new ButtonComponent(
                new(x + (width - promptButtonWidth) / 2, y + (height - promptButtonHeight) / 2, promptButtonWidth, promptButtonHeight),
                DeluxeJournalMod.UiTexture!,
                new(87, 55, 9, 9),
                4f)
            {
                hoverText = promptYes,
                Value = 1,
                SoundCueName = "trashcan",
                OnClick = delegate
                {
                    _textBox.RawText = string.Empty;
                    _promptActive = false;
                    Save(true);
                }
            };

            _promptNoButton = new ButtonComponent(
                new(x + (width - promptButtonWidth) / 2, y + (height + promptButtonHeight) / 2, promptButtonWidth, promptButtonHeight),
                DeluxeJournalMod.UiTexture!,
                new(87, 55, 9, 9),
                4f)
            {
                hoverText = promptNo,
                Value = 2,
                SoundCueName = "breathout",
                OnClick = (_, _) => _promptActive = false
            };

            exitFunction = () => Save();
        }

        public override void OnHidden()
        {
            Save();
            _promptActive = false;
            _textBox.Selected = false;
        }

        public override bool KeyboardHasFocus()
        {
            return _textBox.Selected;
        }

        public override bool isWithinBounds(int x, int y)
        {
            return x >= xPositionOnScreen && x < xPositionOnScreen + width + 48 && y >= yPositionOnScreen && y < yPositionOnScreen + height;
        }

        protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
        {
            if (oldID == gamepadCursorArea.myID)
            {
                SnapToActiveTabComponent();
            }
        }

        public override void snapToDefaultClickableComponent()
        {
            currentlySnappedComponent = gamepadCursorArea;
            snapCursorToCurrentSnappedComponent();
        }

        public override void receiveGamePadButton(Buttons b)
        {
            if (_promptActive)
            {
                _promptNoButton.SimulateLeftClick();
            }

            switch (b)
            {
                case Buttons.B:
                    _textBox.Selected = false;
                    break;
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (_promptActive)
            {
                if (_promptNoButton.containsPoint(x, y))
                {
                    _promptNoButton.ReceiveLeftClick(x, y, playSound);
                }
                else if (_promptYesButton.containsPoint(x, y))
                {
                    _promptYesButton.ReceiveLeftClick(x, y, playSound);
                }
                else if (!isWithinBounds(x, y))
                {
                    ExitJournalMenu(playSound);
                }

                return;
            }

            _textBox.ReceiveLeftClick(x, y, playSound);

            if (_trashButton.containsPoint(x, y))
            {
                _textBox.Selected = false;
                _trashButton.ReceiveLeftClick(x, y, playSound);
            }
            else if (!Game1.options.SnappyMenus && _textBox.Bounds.Contains(x, y))
            {
                _textBox.Selected = true;
                _textBox.MoveCaretToPoint(x, y);
            }
            else if (_textBox.Selected)
            {
                _textBox.Selected = false;
            }
            else if (!isWithinBounds(x, y))
            {
                ExitJournalMenu(playSound);
            }
        }

        public override void leftClickHeld(int x, int y)
        {
            if (!GameMenu.forcePreventClose)
            {
                base.leftClickHeld(x, y);
                _textBox.LeftClickHeld(x, y);
            }
        }

        public override void releaseLeftClick(int x, int y)
        {
            if (!GameMenu.forcePreventClose)
            {
                base.releaseLeftClick(x, y);
                _textBox.ReleaseLeftClick(x, y);
            }
        }

        public override void receiveScrollWheelAction(int direction)
        {
            _textBox.ReceiveScrollWheelAction(direction);
        }

        public override void receiveKeyPress(Keys key)
        {
            if (_promptActive)
            {
                if (key == Keys.Escape)
                {
                    _promptNoButton.SimulateLeftClick();
                }

                return;
            }

            if (_textBox.Selected)
            {
                if (Game1.options.SnappyMenus && !overrideSnappyMenuCursorMovementBan())
                {
                    applyMovementKey(key);
                }

                _dirty = true;
                _saveTimeSeconds = _totalTimeSeconds + 3;
            }
            else
            {
                base.receiveKeyPress(key);
            }

            if (key == Keys.Escape)
            {
                _textBox.Selected = false;
                ExitJournalMenu();
            }
        }

        public override void applyMovementKey(int direction)
        {
            base.applyMovementKey(direction);

            if (Game1.options.SnappyMenus && gamepadCursorArea.containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()))
            {
                switch (direction)
                {
                    case Game1.up:
                        _textBox.ReceiveScrollWheelAction(1);
                        break;
                    case Game1.down:
                        _textBox.ReceiveScrollWheelAction(-1);
                        break;
                }
            }
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);

            if (_promptActive)
            {
                int promptChoice = 0;

                if (_promptYesButton.containsPoint(x, y))
                {
                    promptChoice = _promptYesButton.Value;
                }
                else if (_promptNoButton.containsPoint(x, y))
                {
                    promptChoice = _promptNoButton.Value;
                }

                if (promptChoice != _promptChoice)
                {
                    _promptChoice = promptChoice;
                    Game1.playSound("Cowboy_gunshot");
                }
            }

            _textBox.TryHover(x, y);
            _trashButton.tryHover(x, y, 0.5f);
        }

        public override void update(GameTime time)
        {
            _totalTimeSeconds = time.TotalGameTime.Seconds;
            _textBox.Update(time);

            if (_dirty && _saveTimeSeconds < _totalTimeSeconds)
            {
                Save();
            }
        }

        public override void draw(SpriteBatch b)
        {
            if (!_promptActive)
            {
                for (int y = _textBox.Font.LineSpacing; y < _textBox.Bounds.Height; y += _textBox.Font.LineSpacing)
                {
                    b.Draw(Game1.staminaRect,
                        new Rectangle(_textBox.Bounds.X + 4, _textBox.Bounds.Y + y, _textBox.Bounds.Width - 8, 2),
                        Game1.textShadowColor * 0.5f);
                }

                _textBox.Draw(b);
            }
            else
            {
                Point textCenter = new(xPositionOnScreen + width / 2, yPositionOnScreen + (height - _promptYesButton.bounds.Height * 3) / 2);
                SpriteText.drawStringHorizontallyCenteredAt(b, Game1.content.LoadString("Strings\\UI:AnimalQuery_ConfirmSell"), textCenter.X, textCenter.Y);
                SpriteText.drawStringHorizontallyCenteredAt(b, _promptYesButton.hoverText, textCenter.X, _promptYesButton.bounds.Y + 8, alpha: _promptChoice == _promptYesButton.Value ? 1f : 0.6f);
                SpriteText.drawStringHorizontallyCenteredAt(b, _promptNoButton.hoverText, textCenter.X, _promptNoButton.bounds.Y + 8, alpha: _promptChoice == _promptNoButton.Value ? 1f : 0.6f);

                if (_promptChoice > 0)
                {
                    ButtonComponent choiceButton = _promptChoice == _promptYesButton.Value ? _promptYesButton : _promptNoButton;

                    drawTextureBox(b,
                        choiceButton.texture,
                        choiceButton.sourceRect,
                        choiceButton.bounds.X,
                        choiceButton.bounds.Y,
                        choiceButton.bounds.Width,
                        choiceButton.bounds.Height,
                        Color.White,
                        choiceButton.scale,
                        false);
                }
            }

            _trashButton.draw(b);
        }

        private void Save(bool forced = false)
        {
            if (forced || _dirty)
            {
                string rawText = _textBox.RawText;

                DeluxeJournalMod.Instance?.SaveNotes(rawText);
                _overlay?.UpdateText(rawText);
                _dirty = false;
            }
        }
    }
}
