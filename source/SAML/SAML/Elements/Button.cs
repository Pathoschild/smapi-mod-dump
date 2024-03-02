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
using Microsoft.Xna.Framework.Input;
using SAML.Events;
using SAML.Utilities;
using StardewValley;

namespace SAML.Elements
{
    public class Button : Element
    {
        private Texture2D texture;
        private Rectangle sourceRect;
        private Item? item;
        private string? text;
        private DrawEffects textureDrawEffects = new() { Scale = 4f };
        private DrawEffects contentDrawEffects = new() { ZIndex = 1.1f };
        private Color backgroundColor = Color.White;
        private Color color = Color.White;
        private StackDrawType itemStackDrawType = StackDrawType.Both;
        private SpriteFont font = Game1.smallFont;
        private HorizontalAlignment horizontalContentAlignment = HorizontalAlignment.Center;
        private VerticalAlignment verticalContentAlignment = VerticalAlignment.Center;
        private Vector2 itemOffset = Vector2.Zero;
        private Vector2 textOffset = Vector2.Zero;

        private bool isHovered = false;
        private float textureScale;
        private bool isForcedWidth = false;
        private bool isForcedHeight = false;
        private Vector2 itemContentPosition = Vector2.Zero;
        private Vector2 textContentPosition = Vector2.Zero;

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
        /// The texture of this <see cref="Button"/>
        /// </summary>
        public Texture2D Texture
        {
            get => texture;
            set
            {
                texture = value;
                invokePropertyChanged();
            }
        }
        /// <summary>
        /// The source rectangle for the texture of this <see cref="Button"/>
        /// </summary>
        public Rectangle SourceRect
        {
            get => sourceRect;
            set
            {
                sourceRect = value;
                invokePropertyChanged();
            }
        }
        /// <summary>
        /// The Item attached to this <see cref="Button"/>
        /// </summary>
        public Item? Item
        {
            get => item;
            set
            {
                item = value;
                invokePropertyChanged();
            }
        }
        /// <summary>
        /// The text attached to this <see cref="Button"/>
        /// </summary>
        public string? Text
        {
            get => text;
            set
            {
                text = value;
                invokePropertyChanged();
            }
        }
        /// <summary>
        /// Additional context to use when rendering the texture (see <see cref="DrawEffects"/>)
        /// </summary>
        public DrawEffects TextureDrawEffects
        {
            get => textureDrawEffects;
            set
            {
                if (textureDrawEffects is not null)
                    textureDrawEffects.PropertyChanged -= onTextureDrawEffectsChanged;
                textureDrawEffects = value;
                if (textureDrawEffects is not null)
                    textureDrawEffects.PropertyChanged += onTextureDrawEffectsChanged;
                invokePropertyChanged();
            }
        }
        /// <summary>
        /// Additional context to use when rendering the contents (see <see cref="DrawEffects"/>)
        /// </summary>
        public DrawEffects ContentDrawEffects
        {
            get => contentDrawEffects;
            set
            {
                if (contentDrawEffects is not null)
                    contentDrawEffects.PropertyChanged -= onContentDrawEffectsChanged;
                contentDrawEffects = value;
                if (contentDrawEffects is not null)
                    contentDrawEffects.PropertyChanged += onContentDrawEffectsChanged;
                invokePropertyChanged();
            }
        }
        /// <summary>
        /// The color of the texture
        /// </summary>
        public Color BackgroundColor
        {
            get => backgroundColor;
            set
            {
                backgroundColor = value;
                invokePropertyChanged();
            }
        }
        /// <summary>
        /// The color of the content
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
        /// The decoration to apply when drawing the item
        /// </summary>
        public StackDrawType ItemStackDrawType
        {
            get => itemStackDrawType;
            set
            {
                itemStackDrawType = value;
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
        /// Changes the X value of this <see cref="Button"/>'s content based on it's own position
        /// </summary>
        public HorizontalAlignment HorizontalContentAlignment
        {
            get => horizontalContentAlignment;
            set
            {
                horizontalContentAlignment = value;
                invokePropertyChanged();
                updateItemContentPosition();
                updateTextContentPosition();
            }
        }
        /// <summary>
        /// Changes the Y value of this <see cref="Button"/>'s content based on it's own position
        /// </summary>
        public VerticalAlignment VerticalContentAlignment
        {
            get => verticalContentAlignment;
            set
            {
                verticalContentAlignment = value;
                invokePropertyChanged();
                updateItemContentPosition();
                updateTextContentPosition();
            }
        }
        /// <summary>
        /// Additional offset for the item content
        /// </summary>
        public Vector2 ItemOffset
        {
            get => itemOffset;
            set
            {
                itemOffset = value;
                invokePropertyChanged();
            }
        }
        /// <summary>
        /// Additional offset for the text content
        /// </summary>
        public Vector2 TextOffset
        {
            get => textOffset;
            set
            {
                textOffset = value;
                invokePropertyChanged();
            }
        }

        /// <summary>
        /// An event which fires when the left mouse button is pressed over this <see cref="Button"/>
        /// </summary>
        public event MouseButtonEventHandler? LeftClick
        {
            add => AddListener(EventIds.LeftClick, value);
            remove => RemoveListener(EventIds.RightClick, value);
        }
        /// <summary>
        /// An event which fires when the right mouse button is pressed over this <see cref="Button"/>
        /// </summary>
        public event MouseButtonEventHandler? RightClick
        {
            add => AddListener(EventIds.RightClick, value);
            remove => RemoveListener(EventIds.RightClick, value);
        }
        /// <summary>
        /// An event which fires when a gamepad button is pressed over this <see cref="Button"/>
        /// </summary>
        public event GamePadButtonEventHandler? GamePadButtonDown
        {
            add => AddListener(EventIds.GamePadButtonDown, value);
            remove => RemoveListener(EventIds.GamePadButtonDown, value);
        }
        /// <summary>
        /// An event which fires along side <see cref="Element.Update"/> for hover effects
        /// </summary>
        public event MouseButtonEventHandler? Hover
        {
            add => AddListener(EventIds.Hover, value);
            remove => RemoveListener(EventIds.Hover, value);
        }
        /// <summary>
        /// An event which fires when the cursor enters this <see cref="Button"/>
        /// </summary>
        public event MouseButtonEventHandler? MouseEnter
        {
            add => AddListener(EventIds.MouseEnter, value);
            remove => RemoveListener(EventIds.MouseEnter, value);
        }
        /// <summary>
        /// An event which fires when the cursor leaves this <see cref="Button"/>
        /// </summary>
        public event MouseButtonEventHandler? MouseLeave
        {
            add => AddListener(EventIds.MouseLeave, value);
            remove => RemoveListener(EventIds.MouseLeave, value);
        }

        public Button()
        {
            OnPropertyChanged(nameof(SourceRect));
            PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(Width) || 
                    e.PropertyName == nameof(SourceRect) || 
                    e.PropertyName == nameof(Item) ||
                    e.PropertyName == $"{nameof(ContentDrawEffects)}.{nameof(DrawEffects.Scale)}" || 
                    e.PropertyName == nameof(Text) || 
                    e.PropertyName == nameof(Font))
                    HorizontalContentAlignment = horizontalContentAlignment;
                if (e.PropertyName == nameof(Height) ||
                    e.PropertyName == nameof(SourceRect) ||
                    e.PropertyName == nameof(Item) ||
                    e.PropertyName == $"{nameof(ContentDrawEffects)}.{nameof(DrawEffects.Scale)}" ||
                    e.PropertyName == nameof(Text) ||
                    e.PropertyName == nameof(Font))
                    VerticalContentAlignment = verticalContentAlignment;
            };
            LeftClick += (_, e) => OnLeftClick(e.X, e.Y, e.PlaySound);
            RightClick += (_, e) => OnRightClick(e.X, e.Y, e.PlaySound);
            GamePadButtonDown += (_, e) => OnGamePadButtonDown(e.Button);
            Hover += (_, e) => OnHover(e.X, e.Y);
            MouseEnter += (_, e) => OnMouseEnter(e.X, e.Y);
            MouseLeave += (_, e) => OnMouseLeave(e.X, e.Y);
        }

        /// <summary>
        /// The deault listener for the <see cref="LeftClick"/> event
        /// </summary>
        /// <param name="x">The horizontal position of the mouse on screen at the time of the event</param>
        /// <param name="y">The vertical position of the mouse on screen at the time of the event</param>
        /// <param name="playSound">Whether this function should use sounds or not</param>
        public virtual void OnLeftClick(int x, int y, bool playSound = true) { }

        /// <summary>
        /// The deault listener for the <see cref="RightClick"/> event
        /// </summary>
        /// <param name="x">The horizontal position of the mouse on screen at the time of the event</param>
        /// <param name="y">The vertical position of the mouse on screen at the time of the event</param>
        /// <param name="playSound">Whether this function should use sounds or not</param>
        public virtual void OnRightClick(int x, int y, bool playSound = true) { }

        /// <summary>
        /// The default listener for the <see cref="GamePadButtonDown"/> event
        /// </summary>
        /// <param name="b">The buttons that were pressed at the time of the event</param>
        public virtual void OnGamePadButtonDown(Buttons b) { }

        /// <summary>
        /// The default listener for the <see cref="Hover"/> event
        /// </summary>
        /// <param name="x">The horizontal position of the mouse on screen at the time of the event</param>
        /// <param name="y">The vertical position of the mouse on screen at the time of the event</param>
        public virtual void OnHover(int x, int y)
        {
            if (isHovered)
                textureScale = Math.Min(textureScale + 0.04f, TextureDrawEffects.Scale + TextureDrawEffects.HoverScaleIncrease);
            else
                textureScale = Math.Max(textureScale - 0.04f, TextureDrawEffects.Scale);
        }

        /// <summary>
        /// The default listener for the <see cref="MouseEnter"/> event
        /// </summary>
        /// <param name="x">The horizontal position of the mouse on screen at the time of the event</param>
        /// <param name="y">The vertical position of the mouse on screen at the time of the event</param>
        public virtual void OnMouseEnter(int x, int y)
        {
            isHovered = true;
            if (Item is not null)
                Owner.HoverItem = Item;
            if (!string.IsNullOrEmpty(Text))
                Owner.HoverText = Text;
        }

        /// <summary>
        /// The default listener for the <see cref="MouseLeave"/> event
        /// </summary>
        /// <param name="x">The horizontal position of the mouse on screen at the time of the event</param>
        /// <param name="y">The vertical position of the mouse on screen at the time of the event</param>
        public virtual void OnMouseLeave(int x, int y)
        {
            isHovered = false;
            Owner.HoverItem = null;
            Owner.HoverText = null;
        }

        public override void Draw(SpriteBatch b)
        {
            bool forcePure = Parent is not null and Element e && e.ForcePurePosition;
            b.Draw(Texture, GetPosition(!forcePure), SourceRect, BackgroundColor, TextureDrawEffects.Rotation, TextureDrawEffects.Origin, textureScale, TextureDrawEffects.SpriteEffects, TextureDrawEffects.ZIndex);
            if (Item is null && Text is null)
                return;
            Vector2 textContentOffset = Vector2.Zero;
            if (Item is not null)
            {
                textContentOffset = new(0, measureSizeForItemTexture().Y * 4f);
                item.DrawInMenu(b, GetPosition(!forcePure) + itemContentPosition + itemOffset, ContentDrawEffects.Scale, 1f, ContentDrawEffects.Rotation, ContentDrawEffects.Origin, ContentDrawEffects.SpriteEffects, ContentDrawEffects.ZIndex, ItemStackDrawType, Color, true);
            }
            if (Text is not null)
                b.DrawString(Font, Text, GetPosition(!forcePure) + textContentPosition + textContentOffset + textOffset, Color, ContentDrawEffects.Rotation, ContentDrawEffects.Origin, ContentDrawEffects.Scale, ContentDrawEffects.SpriteEffects, ContentDrawEffects.ZIndex);
        }

        public override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == nameof(SourceRect) || propertyName == $"{nameof(TextureDrawEffects)}.{nameof(DrawEffects.Scale)}")
            {
                if (!isForcedWidth)
                    base.Width = (int)(SourceRect.Width * TextureDrawEffects.Scale);
                if (!isForcedHeight)
                    base.Height = (int)(SourceRect.Height * TextureDrawEffects.Scale);
            }
        }

        private void onTextureDrawEffectsChanged(object sender, PropertyChangedEventArgs e) => invokePropertyChanged($"{nameof(TextureDrawEffects)}.{e.PropertyName}");

        private void onContentDrawEffectsChanged(object sender, PropertyChangedEventArgs e) => invokePropertyChanged($"{nameof(ContentDrawEffects)}.{e.PropertyName}");

        protected Vector2 measureSizeForItemTexture(bool scaled = true)
        {
            if (item is null)
                return Vector2.Zero;
            Rectangle sourceRectItem = ItemRegistry.GetDataOrErrorItem(item.QualifiedItemId).GetSourceRect();
            Vector2 size = new(sourceRectItem.Width, sourceRectItem.Height);
            if (scaled)
                size *= contentDrawEffects.Scale;
            return size;
        }

        private void updateItemContentPosition()
        {
            if (item is null)
                return;
            Vector2 size = measureSizeForItemTexture() * 4f; //Render size for items is x4
            float y = 0, x = 0;
            y = verticalContentAlignment switch
            {
                VerticalAlignment.Top => 0,
                VerticalAlignment.Center => (Height / 2) - (size.Y / 2),
                VerticalAlignment.Bottom => Height - size.Y,
                _ => itemContentPosition.Y
            };
            x = horizontalContentAlignment switch
            {
                HorizontalAlignment.Left => 0,
                HorizontalAlignment.Center => (Width / 2) - (size.X / 2),
                HorizontalAlignment.Right => Width - size.X,
                _ => itemContentPosition.X
            };
            itemContentPosition = new(x, y);
        }

        private void updateTextContentPosition()
        {
            if (string.IsNullOrEmpty(text))
                return;
            Vector2 size = Font.MeasureString(text);
            float y = 0, x = 0;
            y = verticalContentAlignment switch
            {
                VerticalAlignment.Top => 0,
                VerticalAlignment.Center => (Height / 2) - (size.Y / 2),
                VerticalAlignment.Bottom => Height - size.Y,
                _ => textContentPosition.Y
            };
            x = horizontalContentAlignment switch
            {
                HorizontalAlignment.Left => 0,
                HorizontalAlignment.Center => (Width / 2) - (size.X / 2),
                HorizontalAlignment.Right => Width - size.X,
                _ => textContentPosition.X
            };
            textContentPosition = new(x, y);
        }
    }
}
