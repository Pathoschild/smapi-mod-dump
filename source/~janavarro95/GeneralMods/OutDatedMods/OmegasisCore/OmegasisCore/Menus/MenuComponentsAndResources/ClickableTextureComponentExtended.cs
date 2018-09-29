using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;

namespace OmegasisCore.Menus.MenuComponentsAndResources
{
    public class ClickableTextureComponentExtended : ClickableTextureComponent
    {
        public Texture2D texture;

        public Rectangle sourceRect;

        public float baseScale;

        public string hoverText = "";

        public bool drawShadow;

        public int value;

        public ClickableTextureComponentExtended(string name, Rectangle bounds, string label, string hoverText, Texture2D texture, Rectangle sourceRect, float scale, int value, bool drawShadow = false) : base(name, bounds, label, hoverText, texture, sourceRect, scale, drawShadow)
        {
            this.texture = texture;
            if (sourceRect.Equals(Rectangle.Empty) && texture != null)
            {
                this.sourceRect = texture.Bounds;
            }
            else
            {
                this.sourceRect = sourceRect;
            }
            this.scale = scale;
            this.baseScale = scale;
            this.hoverText = hoverText;
            this.drawShadow = drawShadow;
            this.label = label;
            this.value = value;
        }

        public ClickableTextureComponentExtended(Rectangle bounds, Texture2D texture, Rectangle sourceRect, float scale, int value, bool drawShadow = false) : this("", bounds, "", "", texture, sourceRect, scale, value, drawShadow)
        {
        }

        public Vector2 getVector2()
        {
            return new Vector2((float)this.bounds.X, (float)this.bounds.Y);
        }

        public void tryHover(int x, int y, float maxScaleIncrease = 0.1f)
        {
            if (this.bounds.Contains(x, y))
            {
                this.scale = Math.Min(this.scale + 0.04f, this.baseScale + maxScaleIncrease);
                Game1.SetFreeCursorDrag();
                return;
            }
            this.scale = Math.Max(this.scale - 0.04f, this.baseScale);
        }

        public void draw(SpriteBatch b)
        {
            if (this.visible)
            {
                this.draw(b, Color.White, 0.86f + (float)this.bounds.Y / 20000f);
            }
        }

        public void draw(SpriteBatch b, Color c, float layerDepth)
        {
            if (this.visible)
            {
                if (this.drawShadow)
                {
                    Utility.drawWithShadow(b, this.texture, new Vector2((float)this.bounds.X + (float)(this.sourceRect.Width / 2) * this.baseScale, (float)this.bounds.Y + (float)(this.sourceRect.Height / 2) * this.baseScale), this.sourceRect, c, 0f, new Vector2((float)(this.sourceRect.Width / 2), (float)(this.sourceRect.Height / 2)), this.scale, false, layerDepth, -1, -1, 0.35f);
                }
                else
                {
                    b.Draw(this.texture, new Vector2((float)this.bounds.X + (float)(this.sourceRect.Width / 2) * this.baseScale, (float)this.bounds.Y + (float)(this.sourceRect.Height / 2) * this.baseScale), new Rectangle?(this.sourceRect), c, 0f, new Vector2((float)(this.sourceRect.Width / 2), (float)(this.sourceRect.Height / 2)), this.scale, SpriteEffects.None, layerDepth);
                }
                if (!string.IsNullOrEmpty(this.label))
                {
                    b.DrawString(Game1.smallFont, this.label, new Vector2((float)(this.bounds.X + this.bounds.Width), (float)this.bounds.Y + ((float)(this.bounds.Height / 2) - Game1.smallFont.MeasureString(this.label).Y / 2f)), Game1.textColor);
                }
            }
        }

        public void drawItem(SpriteBatch b, int xOffset = 0, int yOffset = 0)
        {
            if (this.item != null && this.visible)
            {
                this.item.drawInMenu(b, new Vector2((float)(this.bounds.X + xOffset), (float)(this.bounds.Y + yOffset)), this.scale / (float)Game1.pixelZoom);
            }
        }
    }
}
