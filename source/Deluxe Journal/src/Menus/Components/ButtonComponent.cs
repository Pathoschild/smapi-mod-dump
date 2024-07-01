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
using StardewValley;
using StardewValley.Menus;

namespace DeluxeJournal.Menus.Components
{
    public class ButtonComponent : ButtonComponent<int>
    {
        public ButtonComponent(string name, string label, string hoverText, Rectangle bounds, int value, Texture2D texture, Rectangle source, float scale, bool drawShadow = false)
            : base(name, label, hoverText, bounds, value, texture, source, scale, drawShadow)
        {
        }

        public ButtonComponent(string name, Rectangle bounds, int value, Texture2D texture, Rectangle source, float scale, bool drawShadow = false)
            : this(name, string.Empty, string.Empty, bounds, value, texture, source, scale, drawShadow)
        {
        }

        public ButtonComponent(Rectangle bounds, Texture2D texture, Rectangle source, float scale, bool drawShadow = false)
            : this(string.Empty, string.Empty, string.Empty, bounds, default, texture, source, scale, drawShadow)
        {
        }
    }

    public class ButtonComponent<T> : ClickableTextureComponent
    {
        /// <summary>The data value represented by this button.</summary>
        public virtual T? Value { get; set; }

        /// <summary>Whether this button is selected/toggled.</summary>
        public virtual bool Selected { get; set; }

        /// <summary>Determines shadow darkness. Must be a value between <c>0f</c> and <c>1f</c>.</summary>
        public float ShadowIntensity { get; set; } = 0.35f;

        /// <summary>Sound cue to be played on left click.</summary>
        public string SoundCueName { get; set; } = "smallSelect";

        /// <summary>Function that takes this button instance as a parameter and returns the pitch to be used while playing <see cref="SoundCueName"/>.</summary>
        public Func<ButtonComponent<T>, int>? SoundPitch { get; set; }

        /// <summary>Callback that takes this button instance and a <see cref="Point"/> mouse position as parameters and fires on <see cref="ReceiveLeftClick"/>.</summary>
        public Action<ButtonComponent<T>, Point>? OnClick {  get; set; }

        public ButtonComponent(string name, string label, string hoverText, Rectangle bounds, T? value, Texture2D texture, Rectangle source, float scale, bool drawShadow = false)
            : base(name, bounds, label, hoverText, texture, source, scale, drawShadow)
        {
            Value = value;
        }

        public ButtonComponent(string name, Rectangle bounds, T? value, Texture2D texture, Rectangle source, float scale, bool drawShadow = false)
            : this(name, string.Empty, string.Empty, bounds, value, texture, source, scale, drawShadow)
        {
        }

        public ButtonComponent(Rectangle bounds, Texture2D texture, Rectangle source, float scale, bool drawShadow = false)
            : this(string.Empty, string.Empty, string.Empty, bounds, default, texture, source, scale, drawShadow)
        {
        }

        /// <summary>Toggle <see cref="Selected"/> and return the new state.</summary>
        public virtual bool Toggle()
        {
            return Selected = !Selected;
        }

        /// <summary>Receive a mouse click.</summary>
        /// <param name="x">Mouse X position.</param>
        /// <param name="y">Mouse Y position.</param>
        /// <param name="playSound">Allow a sound to be played.</param>
        public virtual void ReceiveLeftClick(int x, int y, bool playSound = true)
        {
            if (visible)
            {
                if (playSound && !string.IsNullOrEmpty(SoundCueName))
                {
                    Game1.playSound(SoundCueName, SoundPitch?.Invoke(this));
                }

                OnClick?.Invoke(this, new(x, y));
            }
        }

        /// <summary>Simulate a mouse click on the center of this button.</summary>
        /// <param name="playSound">Allow a sound to be played.</param>
        public virtual void SimulateLeftClick(bool playSound = true)
        {
            Point center = bounds.Center;
            ReceiveLeftClick(center.X, center.Y, playSound);
        }

        public override void draw(SpriteBatch b, Color color, float layerDepth, int frameOffset = 0, int xOffset = 0, int yOffset = 0)
        {
            if (!visible)
            {
                return;
            }

            if (texture != null)
            {
                Vector2 position = new(bounds.X + xOffset + sourceRect.Width / 2f * baseScale, bounds.Y + yOffset + sourceRect.Height / 2f * baseScale);
                Vector2 origin = new(sourceRect.Width / 2f, sourceRect.Height / 2f);
                Rectangle source = frameOffset == 0 ? sourceRect : new(sourceRect.X + sourceRect.Width * frameOffset, sourceRect.Y, sourceRect.Width, sourceRect.Height);

                if (drawShadow)
                {
                    Utility.drawWithShadow(b, texture, position, source, color, 0f, origin, scale, layerDepth: layerDepth, shadowIntensity: ShadowIntensity);
                }
                else
                {
                    b.Draw(texture, position, source, color, 0f, origin, scale, SpriteEffects.None, layerDepth);
                }
            }

            if (!string.IsNullOrEmpty(label))
            {
                Vector2 position = new(bounds.X + xOffset + bounds.Width, bounds.Y + yOffset + (bounds.Height - Game1.smallFont.MeasureString(label).Y) / 2f);

                if (drawLabelWithShadow)
                {
                    Utility.drawTextWithShadow(b, label, Game1.smallFont, position, Game1.textColor, shadowIntensity: ShadowIntensity);
                }
                else
                {
                    b.DrawString(Game1.smallFont, label, position, Game1.textColor);
                }
            }
        }
    }
}
