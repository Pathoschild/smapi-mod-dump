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
using StardewValley.Menus;
using DeluxeJournal.Menus.Components;

using static StardewValley.Menus.ClickableComponent;

namespace DeluxeJournal.Menus
{
    /// <summary>Notes page.</summary>
    public class NotesPage : PageBase
    {
        public readonly ClickableComponent gamepadCursorArea;

        private readonly MultilineTextBox _textBox;

        private int _totalTimeSeconds;
        private int _saveTimeSeconds;
        private bool _dirty;

        public NotesPage(Rectangle bounds, Texture2D tabTexture, ITranslationHelper translation) :
            this("notes", translation.Get("ui.tab.notes"), bounds.X, bounds.Y, bounds.Width, bounds.Height, tabTexture, new Rectangle(32, 0, 16, 16), translation)
        {
        }

        public NotesPage(string name, string title, int x, int y, int width, int height, Texture2D tabTexture, Rectangle tabSourceRect, ITranslationHelper translation) :
            base(name, title, x, y, width, height, tabTexture, tabSourceRect, translation)
        {
            _textBox = new MultilineTextBox(
                new Rectangle(xPositionOnScreen + 30, yPositionOnScreen + 32, width - 60, height - 64),
                null,
                null,
                Game1.dialogueFont,
                Game1.textColor)
            {
                RawText = DeluxeJournalMod.Instance?.GetNotes() ?? ""
            };

            gamepadCursorArea = new ClickableComponent(new Rectangle(_textBox.Bounds.Right - 16, _textBox.Bounds.Bottom - 16, 16, 16), "")
            {
                myID = 0,
                leftNeighborID = CUSTOM_SNAP_BEHAVIOR,
                fullyImmutable = true
            };
        }

        public override void OnHidden()
        {
            _textBox.Selected = false;
        }

        public override bool KeyboardHasFocus()
        {
            return _textBox.Selected;
        }

        public override bool readyToClose()
        {
            if (_dirty)
            {
                _dirty = false;
                Save();
            }

            return base.readyToClose();
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
            switch (b)
            {
                case Buttons.B:
                    _textBox.Selected = false;
                    break;
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            _textBox.ReceiveLeftClick(x, y, playSound);

            if (!Game1.options.SnappyMenus && _textBox.Bounds.Contains(x, y))
            {
                _textBox.Selected = true;
                _textBox.MoveCaretToPoint(x, y);
            }
            else
            {
                _textBox.Selected = false;
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
            if (Game1.options.SnappyMenus && !overrideSnappyMenuCursorMovementBan())
            {
                applyMovementKey(key);
            }

            if (_textBox.Selected)
            {
                _dirty = true;
                _saveTimeSeconds = _totalTimeSeconds + 3;
            }

            if (key == Keys.Escape)
            {
                _textBox.Selected = false;
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
            _textBox.TryHover(x, y);
        }

        public override void update(GameTime time)
        {
            _totalTimeSeconds = time.TotalGameTime.Seconds;
            _textBox.Update(time);

            if (_dirty && _saveTimeSeconds < _totalTimeSeconds)
            {
                _dirty = false;
                Save();
            }
        }

        public override void draw(SpriteBatch b)
        {
            for (int y = _textBox.Font.LineSpacing; y < _textBox.Bounds.Height; y += _textBox.Font.LineSpacing)
            {
                b.Draw(Game1.staminaRect,
                    new Rectangle(_textBox.Bounds.X + 4, _textBox.Bounds.Y + y, _textBox.Bounds.Width - 8, 2),
                    Game1.textShadowColor * 0.5f);
            }

            _textBox.Draw(b);
        }

        private void Save()
        {
            DeluxeJournalMod.Instance?.SaveNotes(_textBox.RawText);
        }
    }
}
