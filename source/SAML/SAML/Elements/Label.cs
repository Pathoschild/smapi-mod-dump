/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/SAML
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SAML.Events;
using SAML.Utilities;
using StardewValley;

namespace SAML.Elements
{
    public class Label : Element
    {
        #region Fields
        private string text = "";
        private SpriteFont font = Game1.smallFont;
        private Color color = Game1.textColor;
        private DrawEffects contentDrawEffects = new();

        private bool isForcedWidth = false;
        private bool isForcedHeight = false;
        #endregion

        public override int Width
        {
            get => base.Width;
            set
            {
                isForcedWidth = true;
                base.Width = value;
            }
        }
        public override int Height
        {
            get => base.Height;
            set
            {
                isForcedHeight = true;
                base.Height = value;
            }
        }
        /// <summary>
        /// The text which this <see cref="Label"/> displays
        /// </summary>
        public string Text
        {
            get => text;
            set
            {
                text = value ?? "";
                invokePropertyChanged();
            }
        }
        /// <summary>
        /// The font in which the text will be drawn
        /// </summary>
        public SpriteFont Font
        {
            get => font;
            set
            {
                font = value;
                invokePropertyChanged();
            }
        }
        /// <summary>
        /// The color in which the text will be drawn
        /// </summary>
        public Color Color
        {
            get => color;
            set
            {
                color = value;
                invokePropertyChanged();
            }
        }
        /// <summary>
        /// Additional context to use when rendering the text (see <see cref="DrawEffects"/>)
        /// </summary>
        public DrawEffects ContentDrawEffects
        {
            get => contentDrawEffects;
            set
            {
                if (contentDrawEffects is not null)
                    contentDrawEffects.PropertyChanged -= onDrawEffectsChanged;
                contentDrawEffects = value;
                invokePropertyChanged();
                if (contentDrawEffects is not null)
                    contentDrawEffects.PropertyChanged += onDrawEffectsChanged;
            }
        }

        public override void Draw(SpriteBatch b) => b.DrawString(Font, Text, GetPosition(!(Parent is not null and Element e && e.ForcePurePosition)), Color, ContentDrawEffects.Rotation, ContentDrawEffects.Origin, ContentDrawEffects.Scale, ContentDrawEffects.SpriteEffects, ContentDrawEffects.ZIndex);

        public override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == nameof(Text) || propertyName == nameof(Font) || propertyName == $"{nameof(ContentDrawEffects)}.{nameof(DrawEffects.Scale)}")
            {
                if (!isForcedWidth)
                    base.Width = (int)(Font.MeasureString(Text).X * ContentDrawEffects.Scale);
                if (!isForcedHeight)
                    base.Height = (int)(Font.MeasureString(Text).Y * ContentDrawEffects.Scale);
            }
        }

        private void onDrawEffectsChanged(object sender, PropertyChangedEventArgs e) => invokePropertyChanged($"{nameof(ContentDrawEffects)}.{e.PropertyName}");
    }
}
