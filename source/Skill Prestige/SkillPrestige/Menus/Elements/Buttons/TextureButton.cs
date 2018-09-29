using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace SkillPrestige.Menus.Elements.Buttons
{
    /// <summary>
    /// Represents a button that has nothing drawn on top of it's background texture.
    /// </summary>
    public sealed class TextureButton : Button
    {
        public TextureButton(Rectangle bounds, Texture2D buttonTexture, Rectangle sourceRectangle, ClickCallback onClickCallback, string hoverText = "")
        {
            Bounds = bounds;
            ButtonTexture = buttonTexture;
            HoverText = hoverText;
            SourceRectangle = sourceRectangle;
            ClickableTextureComponent = new ClickableTextureComponent(string.Empty, Bounds, string.Empty, HoverText,
                            ButtonTexture, sourceRectangle, 1f);
            _onClick = onClickCallback;
        }

        public delegate void ClickCallback();

        private readonly ClickCallback _onClick;

        public Rectangle SourceRectangle;

        protected override string HoverText { get; }

        protected override string Text => string.Empty;

        protected override void OnMouseHover()
        {
            base.OnMouseHover();
            Game1.playSound("smallSelect");
        }

        protected override void OnMouseClick()
        {
            Game1.playSound("bigSelect");
            _onClick.Invoke();
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            ClickableTextureComponent.draw(spriteBatch);
        }

        public void Draw(SpriteBatch spriteBatch, Color color)
        {
            var location = new Vector2(ClickableTextureComponent.bounds.X, ClickableTextureComponent.bounds.Y);
            spriteBatch.Draw(ClickableTextureComponent.texture, location, ClickableTextureComponent.sourceRect, color, 0.0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.4f);
        }
    }
}