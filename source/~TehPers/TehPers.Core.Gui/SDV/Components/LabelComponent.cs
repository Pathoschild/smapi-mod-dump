using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using TehPers.Core.Gui.Base.Components;
using TehPers.Core.Gui.Base.Units;
using TehPers.Core.Gui.SDV.Units;
using TehPers.Core.Helpers.Static;

namespace TehPers.Core.Gui.SDV.Components {
    public sealed class LabelComponent : ResizableGuiComponent {
        private SpriteFont _font = Game1.smallFont;
        private Vector2 _charSize = Game1.smallFont.MeasureString("X");
        private ResponsiveVector2<GuiInfo> _size = GuiVectors.Zero;
        private Vector2? _scale = Vector2.One;
        private Vector2 _textSize = Vector2.Zero;
        private string _text = null;

        public LabelComponent() { }

        public LabelComponent(IGuiComponent parent) : base(parent) { }

        /// <summary>The text to display.</summary>
        public string Text { get => this._text; set => this.SetText(value); }

        /// <summary>The color of this label's text.</summary>
        public Color Color { get; set; } = Game1.textColor;

        /// <summary>The color of this label's text's shadow, or <c>null</c> for no shadow.</summary>
        public Color? ShadowColor { get; set; } = null;

        /// <summary>The font to use when rendering this text.</summary>
        public SpriteFont Font { get => this._font; set => this.SetFont(value); }

        /// <summary>The size of this label relative to the size of the font used, or <c>null</c> if <see cref="Size"/> is being used directly instead.</summary>
        public Vector2? Scale { get => this._scale; set => this.SetScale(value); }

        /// <inheritdoc />
        public override ResponsiveVector2<GuiInfo> Size { get => this._size; set => this.SetSize(value); }

        /// <inheritdoc />
        protected override void DrawSelf(SpriteBatch batch, ResolvedVector2 resolvedLocation, ResolvedVector2 resolvedSize) {
            if (this.Text == null) {
                return;
            }

            // Draw the string
            Vector2 scale = this.Scale ?? new Vector2(resolvedSize.X / this._textSize.X, resolvedSize.Y / this._textSize.Y);
            Vector2 position = new Vector2(resolvedLocation.X, resolvedLocation.Y);
            float depth = this.GetGlobalDepth(0.1f);
            Color? shadowColor = this.ShadowColor;
            if (shadowColor.HasValue) {
                // With shadow
                float shadowDepth = this.GetGlobalDepth(0.05f);
                batch.DrawStringWithShadow(this._font, this.Text, position, this.Color, shadowColor.Value, scale, depth, shadowDepth);
            } else {
                // Normally
                batch.DrawString(this._font, this.Text, position, this.Color, 0, Vector2.Zero, scale, SpriteEffects.None, depth);
            }
        }

        private void SetScale(Vector2? scale) {
            this._scale = scale;

            if (this._scale.HasValue) {
                this.RecalculateSize(false);
            }
        }

        private void SetSize(ResponsiveVector2<GuiInfo> size) {
            this._scale = null;
            this._size = size;
        }

        private void SetText(string text) {
            this._text = text;
            this.RecalculateSize(true);
        }

        private void SetFont(SpriteFont font) {
            this._font = font;
            this.RecalculateSize(true);
        }

        private void RecalculateSize(bool calculateTextSize) {
            if (calculateTextSize) {
                this._textSize = this._font.MeasureString(this._text);
            }

            if (this._scale.HasValue) {
                this._size = new ResponsiveVector2<GuiInfo>(new PixelUnits(this._scale.Value.X * this._textSize.X), new PixelUnits(this._scale.Value.Y * this._textSize.Y));
            }
        }
    }
}