/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FlameHorizon/ProductionStats
**
*************************************************/

// derived from code by Jesse Plamondon-Willard under MIT license: https://github.com/Pathoschild/StardewMods

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

namespace ProductionStats.Components
{
    internal class SearchTextBox : IDisposable
    {
        /// <summary>The underlying textbox.</summary>
        private readonly TextBox _textbox;

        /// <summary>The rendered textbox's pixel area on-screen.</summary>
        private Rectangle _boundsImpl;

        /// <summary>
        /// The last search text recieved for change detection.
        /// </summary>
        private string _lastText = string.Empty;

        /// <summary>The event raised when the search text changes.</summary>
        public event EventHandler<string>? OnChanged;

        public SearchTextBox(SpriteFont font, Color textColor)
        {
            _textbox = new TextBox(Sprites.Textbox.Sheet, null, font, textColor);
            Bounds = new Rectangle(
                _textbox.X,
                _textbox.Y,
                _textbox.Width,
                _textbox.Height);
        }

        public Rectangle Bounds
        {
            get => _boundsImpl;
            set
            {
                _boundsImpl = value;
                _textbox.X = value.X;
                _textbox.Y = value.Y;
                _textbox.Width = value.Width;
                _textbox.Height = value.Height;
            }
        }

        public string Text => _textbox.Text;

        public bool Selected => _textbox.Selected;

        internal void Draw(SpriteBatch batch)
        {
            NotifyChange();
            _textbox.Draw(batch);
        }

        /// <summary>Detect updated search text and notify listeners.</summary>
        private void NotifyChange()
        {
            if (_textbox.Text != _lastText)
            {
                OnChanged?.Invoke(this, _textbox.Text);
                _lastText = _textbox.Text;
            }
        }

        /// <summary>Release all resources.</summary>
        public void Dispose()
        {
            OnChanged = null;
            _textbox.Selected = false;
        }

        internal void Select() => _textbox.Selected = true;

        internal void Deselect() => _textbox.Selected = false;
    }
}