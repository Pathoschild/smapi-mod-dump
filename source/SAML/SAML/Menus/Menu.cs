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
using SAML.Elements;
using SAML.Events;
using SAML.Utilities;
using SAML.Utilities.Collections;
using StardewValley;
using StardewValley.Menus;
using System.Runtime.CompilerServices;

namespace SAML.Menus
{
    public class Menu : IClickableMenu, IElement, IDisposable
    {
        #region Fields
        private string name;
        private HorizontalAlignment horizontalAlignment = HorizontalAlignment.Center;
        private VerticalAlignment verticalAlignment = VerticalAlignment.Center;
        private string? hoverText;
        private Item? hoverItem;
        private Item? heldItem;
        private IFocusable? focused;
        private ObservableList<Element> elements = [];
        private Button closeButton;

        private int x;
        private int y;
        private int _width;
        private int _height;
        #endregion

        public string Name
        {
            get => name;
            set
            {
                name = value;
                invokePropertyChanged();
            }
        }
        public virtual int X
        {
            get => x;
            set
            {
                x = value;
                horizontalAlignment = HorizontalAlignment.None;
                invokePropertyChanged();
                invokePropertyChanged(nameof(HorizontalAlignment));
            }
        }
        public virtual int Y
        {
            get => y;
            set
            {
                y = value;
                verticalAlignment = VerticalAlignment.None;
                invokePropertyChanged();
                invokePropertyChanged(nameof(VerticalAlignment));
            }
        }
        public virtual int Width
        {
            get => _width;
            set
            {
                _width = value;
                invokePropertyChanged();
            }
        }
        public virtual int Height
        {
            get => _height;
            set
            {
                _height = value;
                invokePropertyChanged();
            }
        }
        public virtual HorizontalAlignment HorizontalAlignment
        {
            get => horizontalAlignment;
            set
            {
                horizontalAlignment = value;
                x = horizontalAlignment switch
                {
                    HorizontalAlignment.Left => 0,
                    HorizontalAlignment.Center => (Game1.viewport.Width / 2) - (Width / 2),
                    HorizontalAlignment.Right => Game1.viewport.Width - Width,
                    _ => x
                };
                invokePropertyChanged();
                invokePropertyChanged(nameof(X));
            }
        }
        public virtual VerticalAlignment VerticalAlignment
        {
            get => verticalAlignment;
            set
            {
                verticalAlignment = value;
                y = verticalAlignment switch
                {
                    VerticalAlignment.Top => 0,
                    VerticalAlignment.Center => (Game1.viewport.Height / 2) - (Height / 2),
                    VerticalAlignment.Bottom => Game1.viewport.Height - Height,
                    _ => y
                };
                invokePropertyChanged();
                invokePropertyChanged(nameof(Y));
            }
        }
        /// <summary>
        /// The text to display while hovering
        /// </summary>
        public string? HoverText
        {
            get => hoverText;
            set
            {
                hoverText = value;
                invokePropertyChanged();
            }
        }
        /// <summary>
        /// The item of which to display information while hovering
        /// </summary>
        public Item? HoverItem
        {
            get => hoverItem;
            set
            {
                hoverItem = value;
                invokePropertyChanged();
            }
        }
        /// <summary>
        /// The elements currently under the cursor
        /// </summary>
        public ObservableList<Element> HoveredElements { get; } = [];
        /// <summary>
        /// The item attached to the cursor
        /// </summary>
        public Item? HeldItem
        {
            get => heldItem;
            set
            {
                heldItem = value;
                invokePropertyChanged();
            }
        }
        /// <summary>
        /// The focused element
        /// </summary>
        public IFocusable? Focused
        {
            get => focused;
            set
            {
                if (focused is not null)
                    Fire(EventIds.LostFocus, new FocusChangedEventArgs(value), this, new[] { focused });
                focused = value;
                invokePropertyChanged();
                if (focused is not null)
                    Fire(EventIds.GotFocus, new FocusChangedEventArgs(value), this, new[] { focused });
            }
        }
        /// <summary>
        /// The elements attached to this <see cref="Menu"/>
        /// </summary>
        public ObservableList<Element> Elements
        {
            get => elements;
            set
            {
                if (elements is not null)
                    elements.CollectionChanged -= OnElementsChanged;
                elements = value;
                invokePropertyChanged();
                if (elements is not null)
                    elements.CollectionChanged += OnElementsChanged;
            }
        }
        /// <summary>
        /// The default close button
        /// </summary>
        public Button CloseButton
        {
            get => closeButton;
            set
            {
                closeButton = value;
                invokePropertyChanged();
            }
        }


        /// <summary>
        /// The shared <see cref="Utilities.EventManager"/> used by the <see cref="Menu"/> and it's <see cref="Element"/>'s while the menu is active
        /// </summary>
        protected EventManager EventManager => ModEntry.EventManager.Value;

        public event PropertyChangedEventHandler? PropertyChanged;
        /// <summary>
        /// An event which fires before this element begins drawing
        /// </summary>
        public event DrawEventHandler? BeforeDraw;
        /// <summary>
        /// An event which fires after this element is drawn
        /// </summary>
        public event DrawEventHandler? AfterDraw;
        /// <summary>
        /// An event which fires once per tick (~60 times / second)
        /// </summary>
        public event UpdateEventHandler? Update
        {
            add => AddListener(EventIds.Update, value);
            remove => RemoveListener(EventIds.Update, value);
        }
        /// <summary>
        /// An event which fires when the left mouse button is pressed
        /// </summary>
        public event MouseButtonEventHandler? LeftClick
        {
            add => AddListener(EventIds.LeftClick, value);
            remove => RemoveListener(EventIds.LeftClick, value);
        }
        /// <summary>
        /// An event which fires when the right mouse button is pressed
        /// </summary>
        public event MouseButtonEventHandler? RightClick
        {
            add => AddListener(EventIds.RightClick, value);
            remove => RemoveListener(EventIds.RightClick, value);
        }
        /// <summary>
        /// An event which fires when a gamepad button is pressed
        /// </summary>
        public event GamePadButtonEventHandler? GamePadButtonDown
        {
            add => AddListener(EventIds.GamePadButtonDown, value);
            remove => RemoveListener(EventIds.GamePadButtonDown, value);
        }
        /// <summary>
        /// An event which fires along side <see cref="Update"/> for hover effects
        /// </summary>
        public event MouseButtonEventHandler? Hover
        {
            add => AddListener(EventIds.Hover, value);
            remove => RemoveListener(EventIds.Hover, value);
        }
        /// <summary>
        /// An event which fires when a key is pressed
        /// </summary>
        public event KeyboardEventHandler? KeyDown
        {
            add => AddListener(EventIds.KeyDown, value);
            remove => RemoveListener(EventIds.KeyDown, value);
        }
        /// <summary>
        /// An event which fires when the size of the game window changes
        /// </summary>
        public event WindowSizeChangedEventHandler? WindowSizeChanged
        {
            add => AddListener(EventIds.WindowSizeChanged, value);
            remove => RemoveListener(EventIds.WindowSizeChanged, value);
        }
        /// <summary>
        /// An event which fires when the mousewheel is scrolled
        /// </summary>
        public event ScrollWheelEventHandler? Scrolled
        {
            add => AddListener(EventIds.Scrolled, value);
            remove => RemoveListener(EventIds.Scrolled, value);
        }

        public Menu()
        {
            elements.CollectionChanged += OnElementsChanged;
            PropertyChanged += (_, e) => OnPropertyChanged(e.PropertyName);
            BeforeDraw += (_, e) => OnBeforeDraw(e.Batch);
            AfterDraw += (_, e) => OnAfterDraw(e.Batch);
            closeButton = new() 
            { 
                HorizontalAlignment = HorizontalAlignment.Right, 
                VerticalAlignment = VerticalAlignment.Top,
                Texture = Game1.mouseCursors,
                SourceRect = new(337, 494, 12, 12),
                Margin = new(32, 0, -32, 0)
            };
            Elements.Add(closeButton);
        }

        /// <summary>
        /// Use <see cref="Draw(SpriteBatch)"/> instead
        /// </summary>
        public sealed override void draw(SpriteBatch b)
        {
            BeforeDraw?.Invoke(this, new(b));
            Draw(b);
            foreach (var el in Elements)
                el.draw(b); //Call internal draw, which in turn will call public draw
            AfterDraw?.Invoke(this, new(b));
        }
        /// <summary>
        /// Use <see cref="OnUpdate(GameTime)"/> instead
        /// </summary>
        public sealed override void update(GameTime time)
        {
            OnUpdate(time);
            foreach (var el in Elements)
                el.update(time); //Call internal update, which in turn will call public update
            Fire(EventIds.Update, new UpdateEventArgs(time), this);
        }
        /// <summary>
        /// Use <see cref="OnLeftClick(int, int, bool)"/> instead
        /// </summary>
        public sealed override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            OnLeftClick(x, y, playSound);
            Fire(EventIds.LeftClick, new MouseButtonEventArgs(x, y, playSound), this, new[] { (IElement?)FindElementUnderMouse(x, y, Elements), this });
        }
        /// <summary>
        /// Use <see cref="OnRightClick(int, int, bool)"/> instead
        /// </summary>
        public sealed override void receiveRightClick(int x, int y, bool playSound = true)
        {
            OnRightClick(x, y, playSound);
            Fire(EventIds.RightClick, new MouseButtonEventArgs(x, y, playSound), this, new[] { (IElement?)FindElementUnderMouse(x, y, Elements), this });
        }
        /// <summary>
        /// Use <see cref="OnGamePadButtonDown(Buttons)"/> instead
        /// </summary>
        public sealed override void receiveGamePadButton(Buttons b)
        {
            OnGamePadButtonDown(b);
            Fire(EventIds.GamePadButtonDown, new GamePadButtonEventArgs(b), this, new[] { (IElement?)FindElementUnderMouse(Game1.getMouseX(), Game1.getMouseY(), Elements), this });
        }
        /// <summary>
        /// Use <see cref="OnHover(int, int)"/> instead
        /// </summary>
        public sealed override void performHoverAction(int x, int y)
        {
            OnHover(x, y);
            foreach (var el in Elements)
            {
                bool flag1 = el.tryHover(x, y);
                bool flag2 = HoveredElements.Contains(el);
                if (flag1 && !flag2)
                {
                    HoveredElements.Add(el);
                    Fire(EventIds.MouseEnter, new MouseButtonEventArgs(x, y), this, new[] { el });
                }
                if (!flag1 && flag2)
                {
                    HoveredElements.Remove(el);
                    Fire(EventIds.MouseLeave, new MouseButtonEventArgs(x, y), this, new[] { el });
                }
                if (el is Container c)
                    c.performHoverAction(x, y);
            }
            Fire(EventIds.Hover, new MouseButtonEventArgs(x, y), this);
        }
        /// <summary>
        /// Use <see cref="OnKeyDown(Keys)"/> instead
        /// </summary>
        public sealed override void receiveKeyPress(Keys key)
        {
            OnKeyDown(key);
            Fire(EventIds.KeyDown, new KeyboardEventArgs(key), this, Focused is not null ? new[] { Focused } : null);
        }
        /// <summary>
        /// Use <see cref="OnGameWindowSizeChanged(Rectangle, Rectangle)"/> instead
        /// </summary>
        public sealed override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            OnGameWindowSizeChanged(oldBounds, newBounds);
            foreach (var el in Elements)
                el.windowSizeChanged(oldBounds, newBounds); //Call internal windowSizeChanged, which in turn will call public windowSizeChanged
            Fire(EventIds.WindowSizeChanged, new WindowSizeChangedEventArgs(oldBounds, newBounds), this);
        }
        /// <summary>
        /// Use <see cref="OnScrolled(ScrollDirection)"/> instead
        /// </summary>
        public sealed override void receiveScrollWheelAction(int direction)
        {
            ScrollDirection scrollDirection = ScrollDirection.Up;
            if (direction < 0)
                scrollDirection = ScrollDirection.Down;
            OnScrolled(scrollDirection);
            Fire(EventIds.Scrolled, new ScrollWheelEventArgs(scrollDirection), this);
        }

        /// <summary>
        /// The default listener for the <see cref="Update"/> event
        /// </summary>
        /// <param name="gameTime">Contains information about the elapsed and total game time</param>
        public virtual void OnUpdate(GameTime gameTime) { }

        /// <summary>
        /// The deault listener for the <see cref="LeftClick"/> event
        /// </summary>
        /// <param name="x">The horizontal position of the mouse on screen at the time of the event</param>
        /// <param name="y">The vertical position of the mouse on screen at the time of the event</param>
        /// <param name="playSound">Whether this function should use sounds or not</param>
        public virtual void OnLeftClick(int x, int y, bool playSound = true)
        {
            if (Focused is not null && !(Focused as Element)!.GetBounds(true, true).Contains(x, y))
                Focused.UnFocus();
            if (CloseButton?.GetBounds(true, true).Contains(x, y) ?? false)
                Close();
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
            int x = Game1.getMouseX(), y = Game1.getMouseY();
            switch (b)
            {
                case Buttons.B:
                    if (Focused is null)
                        exitThisMenu();
                    else
                        Focused.UnFocus();
                    break;
                case Buttons.A:
                    if (Focused is not null && !(Focused as Element)!.GetBounds(true, true).Contains(x, y))
                        Focused.UnFocus();
                    if (CloseButton?.GetBounds(true, true).Contains(x, y) ?? false)
                        Close();
                    break;
            }
        }

        /// <summary>
        /// The default listener for the <see cref="Hover"/> event
        /// </summary>
        /// <param name="x">The horizontal position of the mouse on screen at the time of the event</param>
        /// <param name="y">The vertical position of the mouse on screen at the time of the event</param>
        public virtual void OnHover(int x, int y) { }

        /// <summary>
        /// The default listener for the <see cref="KeyDown"/> event
        /// </summary>
        /// <param name="key">The key that was pressed</param>
        public virtual void OnKeyDown(Keys key)
        {
            if (key == Keys.Escape)
            {
                if (Focused is null)
                    Close();
                else
                    Focused.UnFocus();
            }
        }

        /// <summary>
        /// The default listener for the <see cref="WindowSizeChanged"/> event
        /// </summary>
        /// <param name="oldBounds">The bounds of the window before it was resized</param>
        /// <param name="newBounds">The bounds of the window after it was resized</param>
        public virtual void OnGameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            HorizontalAlignment = horizontalAlignment;
            VerticalAlignment = verticalAlignment;
        }

        /// <summary>
        /// The default listener for the <see cref="Scrolled"/> event
        /// </summary>
        /// <param name="direction">The direction in which the mouse wheel was scrolled</param>
        public virtual void OnScrolled(ScrollDirection direction) { }

        public virtual void OnPropertyChanged(string propertyName)
        {
            if (propertyName == nameof(Width) || propertyName == nameof(Height) || propertyName == nameof(X) || propertyName == nameof(Y))
            {
                closeButton.HorizontalAlignment = closeButton.HorizontalAlignment;
                closeButton.VerticalAlignment = closeButton.VerticalAlignment;
            }
        }

        /// <summary>
        /// The default listener for the <see cref="BeforeDraw"/> event
        /// </summary>
        /// <param name="b">Framework helper class to draw text and sprites to the screen</param>
        public virtual void OnBeforeDraw(SpriteBatch b)
        {
            DrawBackground(b);
        }

        /// <summary>
        /// The default listener for the <see cref="AfterDraw"/> event
        /// </summary>
        /// <param name="b">Framework helper class to draw text and sprites to the screen</param>
        public virtual void OnAfterDraw(SpriteBatch b)
        {
            if (!Game1.options.hardwareCursor)
                DrawMouse(b);
        }

        public virtual void Draw(SpriteBatch b) { }

        /// <summary>
        /// Draw either the hovered item tooltip, or the hovertext if it's null
        /// </summary>
        public virtual void DrawToolTip(SpriteBatch b)
        {
            if (HoverItem is not null)
            {
                drawToolTip(b, HoverItem.getDescription(), HoverItem.DisplayName, HoverItem, true);
                return;
            }
            if (HoverText is not null)
            {
                drawHoverText(b, HoverText, Game1.dialogueFont);
            }
        }

        /// <summary>
        /// Draw the mouse
        /// </summary>
        /// <param name="transparency">The cursor transparency</param>
        /// <param name="withItem">Whether to draw the <see cref="HeldItem"/> or not</param>
        public virtual void DrawMouse(SpriteBatch b, float transparency = 1f, bool withItem = true)
        {
            if (withItem)
                HeldItem?.DrawInMenu(b, new(Game1.getMouseX() + 8, Game1.getMouseY() + 8), 1f, 1f, 0.0f, Vector2.Zero, SpriteEffects.None, 1f, StackDrawType.Both, Color.White, false);
            Game1.mouseCursorTransparency = transparency;
            drawMouse(b);
        }

        public virtual void DrawBackground(SpriteBatch b, float transparency = .5f)
        {
            if (Game1.options.showMenuBackground)
                drawBackground(b);
            else
                b.Draw(Game1.staminaRect, new Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height), Color.Black * transparency);
        }

        /// <summary>
        /// Exit this menu and drop the <see cref="HeldItem"/> if necessary
        /// </summary>
        public virtual void Close()
        {
            if (HeldItem != null)
                Game1.createItemDebris(HeldItem, Game1.player.getStandingPosition(), Game1.player.FacingDirection);
            Game1.activeClickableMenu = null;
        }

        /// <summary>
        /// Invoke the <see cref="PropertyChanged"/> event
        /// </summary>
        /// <param name="propertyName">(Optional) The name of the property that has changed</param>
        /// <remarks>
        /// The name of the property is infered and does not need to be manually added if this call is made in the setter (see examples above)
        /// </remarks>
        protected void invokePropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));

        /// <summary>
        /// Add a listener to a specified event
        /// </summary>
        /// <param name="eventId">The id of the event</param>
        /// <param name="handler">The listener to attach</param>
        protected void AddListener(EventIds eventId, Delegate? handler) => EventManager.AddListener(this, eventId, handler);

        /// <summary>
        /// Remove a listener from a specified event
        /// </summary>
        /// <param name="eventId">The id of the event</param>
        /// <param name="handler">The listener to remove</param>
        protected void RemoveListener(EventIds eventId, Delegate? handler) => EventManager.RemoveListener(this, eventId, handler);

        /// <summary>
        /// Invoke a specified event
        /// </summary>
        /// <param name="eventId">The id of the event</param>
        /// <param name="args">The event arguments</param>
        /// <param name="sender">(Optional) The event sender</param>
        /// <param name="targets">(Optional) A collection of elements for which to send the event</param>
        protected void Fire<T>(EventIds eventId, T args, object? sender = null, IEnumerable<IElement?>? targets = null) where T : EventArgs => EventManager.Fire(eventId, args, sender, targets);
        
        /// <summary>
        /// The default listener for changes to the <see cref="Elements"/> of this <see cref="Container"/>
        /// </summary>
        protected virtual void OnElementsChanged(object sender, CollectionChangedEventArgs e)
        {
            if (e.Added is not null and IEnumerable<Element> added)
            {
                foreach (var el in added)
                {
                    el.Parent = this;
                    el.Owner = this;
                }
            }
            if (e.Removed is not null and IEnumerable<Element> removed)
            {
                foreach (var el in removed)
                {
                    el.Parent = null;
                    el.Owner = null;
                }
            }
        }

        /// <summary>
        /// Try to find the deepest element under the cursor
        /// </summary>
        /// <param name="x">The horizontal position of the cursor</param>
        /// <param name="y">The vertical position of the cursor</param>
        /// <param name="elements">The starting list of elements to search</param>
        /// <param name="includeMargin">Whether or not to use margin for bounds detection (used to prevent weird recursion bug for containers)</param>
        /// <returns>The deepest element under the cursor, or null if no element is under the cursor</returns>
        protected virtual Element? FindElementUnderMouse(int x, int y, IEnumerable<Element> elements, bool includeMargin = true)
        {
            foreach (var el in elements)
            {
                if (el.GetBounds(includeMargin, true).Contains(x, y) && el.Visible)
                {
                    if (el is Container c)
                    {
                        if (FindElementUnderMouse(x, y, c.Elements, false) is not null and Element e)
                            return e;
                        return c;
                    }
                    return el;
                }
            }
            return null;
        }

        /// <summary>
        /// [Internal] Fire the text changed event when the <see cref="GameWindow.TextInput"/> is received
        /// </summary>
        /// <param name="e">The arguments from the original event</param>
        internal void TextInputReceived(TextInputEventArgs e) => Fire(EventIds.TextInput, new TextChangeEventArgs(e.Character), this);

        void IDisposable.Dispose()
        {
            EventManager.Clear();
            GC.SuppressFinalize(this);
        }
    }
}
