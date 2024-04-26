/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Alphablackwolf/SkillPrestige
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkillPrestige.Framework.InputHandling;
using SkillPrestige.Logging;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
// ReSharper disable InconsistentNaming

namespace SkillPrestige.Framework.Menus.Elements.Buttons
{
    /// <summary>Represents a button in Stardew Valley.</summary>
    internal abstract class Button : IInputHandler
    {
        /*********
        ** Fields
        *********/
        private Texture2D _buttonTexture;
        private Rectangle _bounds;

        /// <summary>The texture to draw for the button.</summary>
        protected virtual Texture2D ButtonTexture
        {
            get => this._buttonTexture ?? DefaultButtonTexture;
            init => this._buttonTexture = value;
        }

        protected bool IsHovered { get; private set; }
        protected SpriteFont TitleTextFont { get; init; }
        protected abstract string HoverText { get; }
        protected abstract string Text { get; }

        /// <summary>The Stardew Valley component used to draw clickable items.  Certain items are handled better by the original game using the clickable texture component, but not all; the features in the component are not all used by this mod.</summary>
        protected ClickableTextureComponent ClickableTextureComponent;


        /*********
        ** Accessors
        *********/
        /// <summary>The default texture to use for a button background if none is provided.</summary>
        public static Texture2D DefaultButtonTexture { private get; set; }

        public Rectangle Bounds
        {
            get => this._bounds;
            init
            {
                this._bounds = value;
                this.ClickableTextureComponent = new ClickableTextureComponent(string.Empty, this._bounds, string.Empty, this.HoverText, this.ButtonTexture, new Rectangle(0, 0, 0, 0), 1f);
            }
        }


        /*********
        ** Public methods
        *********/
        // ReSharper disable once UnusedMemberInSuper.Global
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.ButtonTexture, this.Bounds, Color.White);
            this.DrawTitleText(spriteBatch);
        }

        public void DrawHoverText(SpriteBatch spriteBatch)
        {
            if (this.IsHovered)
                IClickableMenu.drawHoverText(spriteBatch, this.HoverText, Game1.smallFont);
        }

        /// <summary>Raised after the player moves the in-game cursor.</summary>
        /// <param name="e">The event data.</param>
        public virtual void OnCursorMoved(CursorMovedEventArgs e)
        {
            var newMousePosition = Game1.getMousePosition(true);
            int oldMouseX = Game1.getOldMouseX(true);
            int oldMouseY = Game1.getOldMouseY(true);
            var oldMousePosition = new Point(oldMouseX, oldMouseY);
            this.IsHovered = this.ContainsPoint(newMousePosition);
            // ReSharper disable once InvertIf - easier to read intent this way
            if (this.IsHovered && !this.ContainsPoint(oldMousePosition))
            {
                Logger.LogVerbose($"{this.Text ?? this.HoverText} button has focus.");
                this.OnMouseHovered();
            }
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="e">The event data.</param>
        /// <param name="isClick">Whether the button press is a click.</param>
        public virtual void OnButtonPressed(ButtonPressedEventArgs e, bool isClick) { }



        /*********
        ** Protected methods
        *********/
        protected void DrawTitleText(SpriteBatch spriteBatch, Vector2? locationRelativeToButton = null)
        {
            var textLocation = locationRelativeToButton;
            if (locationRelativeToButton == null)
            {
                var textSize = this.TitleTextFont.MeasureString(this.Text);
                int buttonXCenter = this.Bounds.X + this.Bounds.Width / 2;
                int buttonYCenter = this.Bounds.Y + this.Bounds.Height / 2;
                float textX = buttonXCenter - textSize.X / 2f;
                float textY = buttonYCenter - textSize.Y / 2f + 3f;
                textLocation = new Vector2(textX, textY);
            }
            else
                textLocation += new Vector2(this.Bounds.X, this.Bounds.Y);

            spriteBatch.DrawString(this.TitleTextFont, this.Text ?? string.Empty, textLocation.Value, Game1.textColor);
        }

        /// <summary>Raised when the player begins hovering over the button.</summary>
        protected virtual void OnMouseHovered() { }

        /// <summary>Get whether the cursor position is over the button.</summary>
        /// <param name="position">The cursor position.</param>
        // ReSharper disable once MemberCanBePrivate.Global
        protected bool ContainsPoint(Point position)
        {
            return this.ClickableTextureComponent.containsPoint(position.X, position.Y);
        }
    }
}
