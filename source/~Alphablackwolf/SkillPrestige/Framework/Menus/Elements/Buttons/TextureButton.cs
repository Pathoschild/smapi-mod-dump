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
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace SkillPrestige.Framework.Menus.Elements.Buttons
{
    /// <summary>Represents a button that has nothing drawn on top of it's background texture.</summary>
    internal sealed class TextureButton : Button
    {
        /*********
        ** Fields
        *********/
        private readonly ClickCallback OnClick;

        protected override string HoverText { get; }
        protected override string Text => string.Empty;


        /*********
        ** Accessors
        *********/
        public delegate void ClickCallback();

        public Rectangle SourceRectangle;


        /*********
        ** Public methods
        *********/
        public TextureButton(Rectangle bounds, Texture2D buttonTexture, Rectangle sourceRectangle, ClickCallback onClickCallback, string hoverText = "")
        {
            this.Bounds = bounds;
            this.ButtonTexture = buttonTexture;
            this.HoverText = hoverText;
            this.SourceRectangle = sourceRectangle;
            this.ClickableTextureComponent = new ClickableTextureComponent(string.Empty, this.Bounds, string.Empty, this.HoverText, this.ButtonTexture, sourceRectangle, 1f);
            this.OnClick = onClickCallback;
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="e">The event data.</param>
        /// <param name="isClick">Whether the button press is a click.</param>
        public override void OnButtonPressed(ButtonPressedEventArgs e, bool isClick)
        {
            base.OnButtonPressed(e, isClick);

            if (isClick && this.IsHovered)
            {
                Game1.playSound("bigSelect");
                this.OnClick.Invoke();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            this.ClickableTextureComponent.draw(spriteBatch);
        }

        public void Draw(SpriteBatch spriteBatch, Color color)
        {
            var location = new Vector2(this.ClickableTextureComponent.bounds.X, this.ClickableTextureComponent.bounds.Y);
            spriteBatch.Draw(this.ClickableTextureComponent.texture, location, this.ClickableTextureComponent.sourceRect, color, 0.0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.4f);
        }


        /*********
        ** Protected methods
        *********/
        /// <summary>Raised when the player begins hovering over the button.</summary>
        protected override void OnMouseHovered()
        {
            base.OnMouseHovered();

            Game1.playSound("smallSelect");
        }
    }
}
