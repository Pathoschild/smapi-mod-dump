using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using TehPers.Core.Enums;
using TehPers.Core.Menus.BoxModel;

namespace TehPers.Core.Menus.Elements {
    public class TextElement : Element {
        public string Text { get; set; } = "";
        public Color Color { get; set; } = Game1.textColor;
        public float Rotation { get; set; } = 0;
        public Vector2 Origin { get; set; } = Vector2.Zero;
        public SpriteEffects Effects { get; set; } = SpriteEffects.None;
        public SpriteFont Font { get; set; } = Game1.smallFont;
        public Vector2 Scale { get; set; } = Vector2.One;
        public Alignment HorizontalAlignment { get; set; } = Alignment.LEFT;
        public Alignment VerticalAlignment { get; set; } = Alignment.CENTER;
        public bool Clip { get; set; }

        public void ScaleTo(float height) {
            float resolution = this.Scale.X / this.Scale.Y;
            int baseHeight = this.Font.LineSpacing;
            float yScale = height / baseHeight;
            this.Scale = new Vector2(yScale * resolution, yScale);
        }

        protected override void OnDraw(SpriteBatch batch, Rectangle2I parentBounds) {
            if (this.Clip) {
                this.WithScissorRect(batch, this.Bounds.ToAbsolute(parentBounds).ToRectangle(), b => this.DrawText(b, parentBounds));
            } else {
                this.DrawText(batch, parentBounds);
            }
        }

        private void DrawText(SpriteBatch batch, Rectangle2I parentBounds) {
            float layerDepth = this.GetGlobalDepth(0.5F);
            Rectangle2I bounds = this.Bounds.ToAbsolute(parentBounds);
            Vector2I loc = bounds.Location;
            Vector2 textSize = this.Font.MeasureString(this.Text) * this.Scale;

            // Horizontal alignment
            switch (this.HorizontalAlignment) {
                case Alignment.LEFT:
                    break;
                case Alignment.MIDDLE:
                    loc = loc.Translate((int) ((bounds.Size.X - textSize.X) / 2), 0);
                    break;
                case Alignment.RIGHT:
                    loc = loc.Translate((int) (bounds.Size.X - textSize.X), 0);
                    break;
                default:
                    throw new NotSupportedException($"Unexpected vertical alignment {this.VerticalAlignment}");
            }

            // Vertical alignment
            switch (this.VerticalAlignment) {
                case Alignment.TOP:
                    break;
                case Alignment.CENTER:
                    loc = loc.Translate(0, (int) ((bounds.Size.Y - textSize.Y) / 2));
                    break;
                case Alignment.BOTTOM:
                    loc = loc.Translate(0, (int) (bounds.Size.Y - textSize.Y));
                    break;
                default:
                    throw new NotSupportedException($"Unexpected vertical alignment {this.VerticalAlignment}");
            }

            // Draw text
            batch.DrawString(this.Font, this.Text, loc.ToVector2(), this.Color, this.Rotation, this.Origin, this.Scale, this.Effects, layerDepth);
        }
    }
}
