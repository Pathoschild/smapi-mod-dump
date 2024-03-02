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
    public class TextBox : Element, IFocusable
    {
        private string text = "";
        private SpriteFont font = Game1.smallFont;
        private Color color = Game1.textColor;
        private Color backgroundColor = Color.White;
        private Texture2D texture = Game1.content.Load<Texture2D>("LooseSprites\\textBox");
        private Rectangle sourceRect = new(0, 0, 192, 48);
        private int maxLength = int.MaxValue;
        private bool numberBox = false;
        private DrawEffects textureDrawEffects = new();
        private DrawEffects contentDrawEffects = new() { ZIndex = 1.1f };
        private int caretIndex = 0;
        private bool caretFlicker = true;
        private int flickerTimer = 60;
        private Vector2 scale = Vector2.One;
        private bool forceTextFit = true;

        private bool isFocused = false;
        private int timer = 0;
        private bool caretToggle = true;
        private bool isForcedWidth = false;
        private bool isForcedHeight = false;
        private readonly DummyKeyboardSubscriber keyboardSubscriber = new();

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
        /// The text value of this <see cref="TextBox"/>
        /// </summary>
        public string Text
        {
            get => text;
            set
            {
                if (tryUpdateContent(value))
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
        /// The texture of this <see cref="TextBox"/>
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
        /// The source rectangle of the texture of this <see cref="TextBox"/>
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
        /// The maximum length of text allowed in this <see cref="TextBox"/>
        /// </summary>
        public int MaxLength
        {
            get => maxLength;
            set
            {
                maxLength = value;
                invokePropertyChanged();
            }
        }
        /// <summary>
        /// Whether this <see cref="TextBox"/> only accepts numbers or not
        /// </summary>
        public bool NumberBox
        {
            get => numberBox;
            set
            {
                numberBox = value;
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
        /// The numerical value of this <see cref="TextBox"/> if it is a number box
        /// </summary>
        public int NumberValue
        {
            get
            {
                if (!NumberBox || !int.TryParse(Text, out int result))
                    return -1;
                return result;
            }
        }
        /// <summary>
        /// The index of the caret, controlls at which index text will be inserted / removed
        /// </summary>
        public int CaretIndex
        {
            get => caretIndex;
            set
            {
                caretIndex = value;
                invokePropertyChanged();
            }
        }
        /// <summary>
        /// Whether the caret should flicker or not
        /// </summary>
        public bool CaretFlicker
        {
            get => caretFlicker;
            set
            {
                caretFlicker = value;
                invokePropertyChanged();
            }
        }
        /// <summary>
        /// The delay for caret flicker
        /// </summary>
        public int FlickerTimer
        {
            get => flickerTimer;
            set
            {
                flickerTimer = value;
                invokePropertyChanged();
            }
        }
        /// <summary>
        /// The vertical and horizontal scale of the texture
        /// </summary>
        /// <remarks>
        /// Replaces TextureDrawEffects.Scale for this element
        /// </remarks>
        public Vector2 Scale
        {
            get => scale;
            set
            {
                scale = value;
                invokePropertyChanged();
            }
        }
        /// <summary>
        /// Toggles if the text in this <see cref="TextBox"/> has to fit inside the texture bounds
        /// </summary>
        public bool ForceTextFit
        {
            get => forceTextFit;
            set
            {
                forceTextFit = value;
                invokePropertyChanged();
            }
        }

        public event FocusChangedEventHandler? GotFocus
        {
            add => AddListener(EventIds.GotFocus, value);
            remove => RemoveListener(EventIds.GotFocus, value);
        }
        public event FocusChangedEventHandler? LostFocus
        {
            add => AddListener(EventIds.LostFocus, value);
            remove => RemoveListener(EventIds.LostFocus, value);
        }
        /// <summary>
        /// An event which fires when the left mouse button is pressed over this <see cref="TextBox"/>
        /// </summary>
        public event MouseButtonEventHandler? LeftClick
        {
            add => AddListener(EventIds.LeftClick, value);
            remove => RemoveListener(EventIds.RightClick, value);
        }
        /// <summary>
        /// An event which fires when the right mouse button is pressed over this <see cref="TextBox"/>
        /// </summary>
        public event MouseButtonEventHandler? RightClick
        {
            add => AddListener(EventIds.RightClick, value);
            remove => RemoveListener(EventIds.RightClick, value);
        }
        /// <summary>
        /// An event which fires when a gamepad button is pressed over this <see cref="TextBox"/>
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
            remove => RemoveListener(EventIds.RightClick, value);
        }
        /// <summary>
        /// An event which fires when this <see cref="TextBox"/> receives character input
        /// </summary>
        public event TextChangeEventHandler? TextInput
        {
            add => AddListener(EventIds.TextInput, value);
            remove => RemoveListener(EventIds.TextInput, value);
        }
        /// <summary>
        /// An event which fires when a key is pressed while this <see cref="TextBox"/> is focused
        /// </summary>
        public event KeyboardEventHandler? KeyDown
        {
            add => AddListener(EventIds.KeyDown, value);
            remove => RemoveListener(EventIds.KeyDown, value);
        }
        /// <summary>
        /// An event which fires when the cursor enters this <see cref="TextBox"/>
        /// </summary>
        public event MouseButtonEventHandler? MouseEnter
        {
            add => AddListener(EventIds.MouseEnter, value);
            remove => RemoveListener(EventIds.MouseEnter, value);
        }
        /// <summary>
        /// An event which fires when the cursor leaves this <see cref="TextBox"/>
        /// </summary>
        public event MouseButtonEventHandler? MouseLeave
        {
            add => AddListener(EventIds.MouseLeave, value);
            remove => RemoveListener(EventIds.MouseLeave, value);
        }

        public TextBox()
        {
            OnPropertyChanged(nameof(SourceRect));
            LeftClick += (_, e) => OnLeftClick(e.X, e.Y, e.PlaySound);
            RightClick += (_, e) => OnRightClick(e.X, e.Y, e.PlaySound);
            GamePadButtonDown += (_, e) => OnGamePadButtonDown(e.Button);
            Hover += (_, e) => OnHover(e.X, e.Y);
            TextInput += (_, e) => OnTextInput(e.Char);
            KeyDown += (_, e) => OnKeyDown(e.Key);
        }

        /// <summary>
        /// The deault listener for the <see cref="LeftClick"/> event
        /// </summary>
        /// <param name="x">The horizontal position of the mouse on screen at the time of the event</param>
        /// <param name="y">The vertical position of the mouse on screen at the time of the event</param>
        /// <param name="playSound">Whether this function should use sounds or not</param>
        public virtual void OnLeftClick(int x, int y, bool playSound = true)
        {
            if (!isFocused)
                Focus();
        }

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
        public virtual void OnGamePadButtonDown(Buttons b)
        {
            if (b == Buttons.A && !isFocused)
                Focus();
        }

        /// <summary>
        /// The default listener for the <see cref="Hover"/> event
        /// </summary>
        /// <param name="x">The horizontal position of the mouse on screen at the time of the event</param>
        /// <param name="y">The vertical position of the mouse on screen at the time of the event</param>
        public virtual void OnHover(int x, int y) { }

        /// <summary>
        /// The default listener for the <see cref="TextInput"/> event
        /// </summary>
        /// <param name="c">The character that was input</param>
        /// <remarks>
        /// Doesn't automatically filter out control characters
        /// </remarks>
        public virtual void OnTextInput(char c)
        {
            if (!char.IsControl(c))
            {
                Text = Text.Insert(CaretIndex, $"{c}");
                CaretIndex++;
            }
        }

        /// <summary>
        /// The default listener for the <see cref="KeyDown"/> event
        /// </summary>
        /// <param name="key">The key that was pressed</param>
        public virtual void OnKeyDown(Keys key)
        {
            if (!isFocused)
                return;
            switch (key)
            {
                case Keys.Escape:
                    UnFocus();
                    caretToggle = true;
                    timer = flickerTimer;
                    break;
                case Keys.Back:
                    if (Text.Length > 0 && CaretIndex > 0)
                    {
                        Text = Text.Remove(CaretIndex - 1, 1);
                        CaretIndex--;
                        caretToggle = true;
                        timer = flickerTimer;
                    }
                    break;
                case Keys.Delete:
                    if (Text.Length > 0 && CaretIndex < Text.Length)
                    {
                        Text = Text.Remove(CaretIndex, 1);
                        caretToggle = true;
                        timer = flickerTimer;
                    }
                    break;
                case Keys.Left:
                    if (CaretIndex > 0)
                    {
                        CaretIndex--;
                        caretToggle = true;
                        timer = flickerTimer;
                    }
                    break;
                case Keys.Right:
                    if (CaretIndex < Text.Length)
                    {
                        CaretIndex++;
                        caretToggle = true;
                        timer = flickerTimer;
                    }
                    break;
                case Keys.Up:
                    CaretIndex = 0;
                    caretToggle = true;
                    timer = flickerTimer;
                    break;
                case Keys.Down:
                    CaretIndex = Text.Length;
                    caretToggle = true;
                    timer = flickerTimer;
                    break;

            }
        }

        /// <summary>
        /// The default listener for the <see cref="MouseEnter"/> event
        /// </summary>
        /// <param name="x">The horizontal position of the mouse on screen at the time of the event</param>
        /// <param name="y">The vertical position of the mouse on screen at the time of the event</param>
        public virtual void OnMouseEnter(int x, int y) { }

        /// <summary>
        /// The default listener for the <see cref="MouseLeave"/> event
        /// </summary>
        /// <param name="x">The horizontal position of the mouse on screen at the time of the event</param>
        /// <param name="y">The vertical position of the mouse on screen at the time of the event</param>
        public virtual void OnMouseLeave(int x, int y) { }

        public virtual void Focus()
        {
            Owner.Focused = this;
            isFocused = true;
            CaretIndex = Text.Length;
            Game1.keyboardDispatcher.Subscriber = keyboardSubscriber;
        }

        public virtual void UnFocus()
        {
            Owner.Focused = null;
            isFocused = false;
            Game1.keyboardDispatcher.Subscriber = null;
        }

        public override void OnUpdate(GameTime time)
        {
            if (!CaretFlicker || !isFocused)
                return;
            timer--;
            if (timer < 0)
            {
                timer = FlickerTimer;
                caretToggle = !caretToggle;
            }
        }

        public override void Draw(SpriteBatch b)
        {
            bool forcePure = Parent is not null and Element e && e.ForcePurePosition;
            b.Draw(Texture, GetPosition(!forcePure), SourceRect, BackgroundColor, TextureDrawEffects.Rotation, TextureDrawEffects.Origin, Scale, TextureDrawEffects.SpriteEffects, TextureDrawEffects.ZIndex);
            b.DrawString(Font, Text, GetPosition(!forcePure) + new Vector2(12f), Color, ContentDrawEffects.Rotation, ContentDrawEffects.Origin, ContentDrawEffects.Scale, ContentDrawEffects.SpriteEffects, ContentDrawEffects.ZIndex);
            if (isFocused && caretToggle)
            {
                Vector2 position = GetPosition(!forcePure);
                Vector2 textSize = CaretIndex > 0 ? Font.MeasureString(Text[..(CaretIndex)]) : Vector2.Zero;
                b.Draw(Game1.staminaRect, new Rectangle((int)(position.X + textSize.X) + 12, (int)position.Y + 6, 4, (int)textSize.Y), new Rectangle(0, 0, 1, 1), Color, ContentDrawEffects.Rotation, ContentDrawEffects.Origin, ContentDrawEffects.SpriteEffects, ContentDrawEffects.ZIndex);
            }
        }

        public override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == nameof(SourceRect) || propertyName == nameof(Scale))
            {
                if (!isForcedWidth)
                    base.Width = (int)(SourceRect.Width * Scale.X);
                if (!isForcedHeight)
                    base.Height = (int)(SourceRect.Height * Scale.Y);
            }
            switch (propertyName)
            {
                case nameof(ForceTextFit):
                    if (forceTextFit)
                    {
                        string text = Text;
                        while (Font.MeasureString(text).X > (Width - 16))
                            text = text.Remove(text.Length - 1);
                        Text = text;
                        CaretIndex = Text.Length;
                    }
                    break;
                case nameof(MaxLength):
                    if (Text.Length > MaxLength)
                        Text = Text[..MaxLength];
                    break;
                case nameof(NumberBox):
                    if (NumberBox && !int.TryParse(Text, out _))
                        Text = "";
                    break;
            }
        }

        private bool tryUpdateContent(string newText)
        {
            newText ??= "";
            if (ForceTextFit && Font.MeasureString(newText).X > (Width - 16))
            {
                CaretIndex--;
                return false;
            }
            if (string.IsNullOrEmpty(newText))
            {
                text = newText ?? "";
                return true;
            }
            if (newText.Length > MaxLength || (NumberBox && !int.TryParse(newText, out _)))
                return false;
            text = newText;
            return true;
        }

        private void onTextureDrawEffectsChanged(object sender, PropertyChangedEventArgs e) => invokePropertyChanged($"{nameof(TextureDrawEffects)}.{e.PropertyName}");

        private void onContentDrawEffectsChanged(object sender, PropertyChangedEventArgs e) => invokePropertyChanged($"{nameof(ContentDrawEffects)}.{e.PropertyName}");
    }
}
