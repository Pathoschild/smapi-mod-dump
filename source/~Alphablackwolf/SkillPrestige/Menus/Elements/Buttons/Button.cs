using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkillPrestige.InputHandling;
using SkillPrestige.Logging;
using StardewValley;
using StardewValley.Menus;

namespace SkillPrestige.Menus.Elements.Buttons
{
    /// <summary>
    /// Represnets a button in Stardew Valley.
    /// </summary>
    public abstract class Button
    {
        /// <summary>
        /// The texture to draw for the button.
        /// </summary>
        protected virtual Texture2D ButtonTexture
        {
            get
            {
                return _buttonTexture ?? DefaultButtonTexture;
            }
            set { _buttonTexture = value; }
        }

        private Texture2D _buttonTexture;

        /// <summary>
        /// The default texture to use for a button background if none is provided.
        /// </summary>
        public static Texture2D DefaultButtonTexture { private get; set; }

        public Rectangle Bounds
        {
            get
            {
                return _bounds;
            }
            set
            {
                _bounds = value;
                ClickableTextureComponent = new ClickableTextureComponent(string.Empty, _bounds, string.Empty, HoverText,
                            ButtonTexture, new Rectangle(0, 0, 0, 0), 1f);
            }
        }
        private Rectangle _bounds;

        private bool IsHovered { get; set; }
        protected SpriteFont TitleTextFont { get; set; }
        protected abstract string HoverText { get; }
        protected abstract string Text { get; }

        protected virtual void OnMouseHover()
        {
            IsHovered = true;
        }

        // ReSharper disable once VirtualMemberNeverOverridden.Global
        protected virtual void OnMouseLeave()
        {
            IsHovered = false;
        }

        protected abstract void OnMouseClick();
        
        /// <summary>
        /// The Stardew Valley component used to draw clickable items. 
        /// Certain items are handled better by the original game using the clickable texture component, 
        /// but not all; the features in the component are not all used by this mod.
        /// </summary>
        protected ClickableTextureComponent ClickableTextureComponent;

        // ReSharper disable once UnusedMemberInSuper.Global
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(ButtonTexture, Bounds, Color.White);
            DrawTitleText(spriteBatch);
        }

        public void DrawHoverText(SpriteBatch spriteBatch)
        {
            if (IsHovered) IClickableMenu.drawHoverText(spriteBatch, HoverText, Game1.smallFont);
        }

        protected void DrawTitleText(SpriteBatch spriteBatch, Vector2? locationRelativeToButton = null)
        {
            var textLocation = locationRelativeToButton;
            if (locationRelativeToButton == null)
            {
                var textSize = TitleTextFont.MeasureString(Text);
                var buttonXCenter = Bounds.X + Bounds.Width / 2;
                var buttonYCenter = Bounds.Y + Bounds.Height / 2;
                var textX = buttonXCenter - textSize.X / 2f;
                var textY = buttonYCenter - textSize.Y / 2f + 3f;
                textLocation = new Vector2(textX, textY);
            }
            else
            {
                textLocation += new Vector2(Bounds.X, Bounds.Y);
            }

            spriteBatch.DrawString(TitleTextFont, Text ?? string.Empty, textLocation.Value, Game1.textColor);
        }

        internal void CheckForMouseHover(MouseMoveEventArguments arguments)
        {
            if (!ClickableTextureComponent.containsPoint(arguments.LastPoint.X, arguments.LastPoint.Y)
                && ClickableTextureComponent.containsPoint(arguments.CurrentPoint.X, arguments.CurrentPoint.Y))
            {
                Logger.LogVerbose($"{Text ?? HoverText} button has focus.");
                OnMouseHover();
            }
            else if (ClickableTextureComponent.containsPoint(arguments.LastPoint.X, arguments.LastPoint.Y)
              && !ClickableTextureComponent.containsPoint(arguments.CurrentPoint.X, arguments.CurrentPoint.Y))
            {
                Logger.LogVerbose($"{Text ?? HoverText} button lost focus.");
                OnMouseLeave();
            }
        }

        internal void CheckForMouseClick(MouseClickEventArguments arguments)
        {
            if (!ClickableTextureComponent.containsPoint(arguments.ClickPoint.X, arguments.ClickPoint.Y) ||
                !ClickableTextureComponent.containsPoint(arguments.ReleasePoint.X, arguments.ReleasePoint.Y)) return;
            Logger.LogVerbose($"Mouse click of button {Text ?? HoverText} detected.");
            OnMouseClick();
        }
    }
}