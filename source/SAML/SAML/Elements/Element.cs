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
using SAML.Menus;
using SAML.Utilities;
using System.Runtime.CompilerServices;

namespace SAML.Elements
{
    public abstract class Element : IElement
    {
        #region Fields
        private string name;
        private int x;
        private int y;
        private int width;
        private int height;
        private HorizontalAlignment horizontalAlignment = HorizontalAlignment.None;
        private VerticalAlignment verticalAlignment = VerticalAlignment.None;
        private Menu owner;
        private Padding margin = Padding.Zero;
        private Padding padding = Padding.Zero;
        private IElement parent;
        private bool visible = true;
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
            get => width;
            set
            {
                width = value;
                invokePropertyChanged();
            }
        }
        public virtual int Height
        {
            get => height;
            set
            {
                height = value;
                invokePropertyChanged();
            }
        }
        public virtual HorizontalAlignment HorizontalAlignment
        {
            get => horizontalAlignment;
            set
            {
                horizontalAlignment = value;
                invokePropertyChanged();
                if (Parent is null)
                    return;
                x = horizontalAlignment switch
                {
                    HorizontalAlignment.Left => Parent.X,
                    HorizontalAlignment.Center => Parent.X + (Parent.Width / 2) - (Width / 2),
                    HorizontalAlignment.Right => Parent.X + Parent.Width - Width,
                    _ => x
                };
                invokePropertyChanged(nameof(X));
            }
        }
        public virtual VerticalAlignment VerticalAlignment
        {
            get => verticalAlignment;
            set
            {
                verticalAlignment = value;
                invokePropertyChanged();
                if (Parent is null)
                    return;
                y = verticalAlignment switch
                {
                    VerticalAlignment.Top => Parent.Y,
                    VerticalAlignment.Center => Parent.Y + (Parent.Height / 2) - (Height / 2),
                    VerticalAlignment.Bottom => Parent.Y + Parent.Height - Height,
                    _ => y
                };
                invokePropertyChanged(nameof(Y));
            }
        }
        /// <summary>
        /// The menu to which this <see cref="Element"/> is assigned
        /// </summary>
        public Menu Owner
        {
            get => owner;
            set
            {
                owner = value;
                invokePropertyChanged();
            }
        }
        /// <summary>
        /// Controls additional spacing for this <see cref="Element"/>
        /// </summary>
        public Padding Margin
        {
            get => margin;
            set
            {
                margin = value;
                invokePropertyChanged();
            }
        }
        /// <summary>
        /// Controls additional sizing for this <see cref="Element"/>
        /// </summary>
        public Padding Padding
        {
            get => padding;
            set
            {
                padding = value;
                invokePropertyChanged();
            }
        }
        /// <summary>
        /// The parent of this <see cref="Element"/>
        /// </summary>
        public IElement Parent
        {
            get => parent;
            set
            {
                parent = value;
                invokePropertyChanged();
            }
        }
        /// <summary>
        /// Controls visibility of this <see cref="Element"/>
        /// </summary>
        /// <remarks>
        /// Elements which are not visible, will not register most interaction logic (e.g. LeftClick / GamePadButton events)
        /// </remarks>
        public bool Visible
        {
            get => visible;
            set
            {
                visible = value;
                invokePropertyChanged();
            }
        }
        /// <summary>
        /// Forces the draw method to ignore margin
        /// </summary>
        /// <remarks>
        /// This is the most natural way I can make items in a <see cref="StackPanel"/> / <see cref="WrapPanel"/> position correctly, believe me, I'm working on it
        /// </remarks>
        public virtual bool ForcePurePosition => false;

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
        /// An event which fires when the size of the game window changes
        /// </summary>
        public event WindowSizeChangedEventHandler? WindowSizeChanged
        {
            add => AddListener(EventIds.WindowSizeChanged, value);
            remove => RemoveListener(EventIds.WindowSizeChanged, value);
        }

        public Element()
        {
            PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(Parent))
                {
                    VerticalAlignment = verticalAlignment;
                    HorizontalAlignment = horizontalAlignment;
                }
            };
            PropertyChanged += (_, e) => OnPropertyChanged(e.PropertyName);
            BeforeDraw += (_, e) => OnBeforeDraw(e.Batch);
            AfterDraw += (_, e) => OnAfterDraw(e.Batch); 
        }

        /// <summary>
        /// The default listener for the <see cref="BeforeDraw"/> event
        /// </summary>
        /// <param name="b">Framework helper class to draw text and sprites to the screen</param>
        public virtual void OnBeforeDraw(SpriteBatch b) { }

        /// <summary>
        /// The default listener for the <see cref="AfterDraw"/> event
        /// </summary>
        /// <param name="b">Framework helper class to draw text and sprites to the screen</param>
        public virtual void OnAfterDraw(SpriteBatch b) { }

        internal virtual void draw(SpriteBatch b)
        {
            if (!Visible) 
                return;
            BeforeDraw?.Invoke(this, new(b));
            Draw(b);
            AfterDraw?.Invoke(this, new(b));
        }

        public abstract void Draw(SpriteBatch b);

        internal virtual void update(GameTime time) => OnUpdate(time);

        /// <summary>
        /// The default listener for the <see cref="Update"/> event
        /// </summary>
        /// <param name="time">Contains information about the elapsed and total game time</param>
        public virtual void OnUpdate(GameTime time) { }

        /// <summary>
        /// Retrieves the position of this <see cref="Element"/>
        /// </summary>
        /// <param name="includeMargin">Whether to include the applicable margin values or not</param>
        /// <returns>The x and y position of this element</returns>
        public virtual Vector2 GetPosition(bool includeMargin = false)
        {
            int x = X, y = Y;
            if (includeMargin)
            {
                x = x - Margin.Right + Margin.Left;
                y = y - Margin.Bottom + Margin.Top;
            }
            return new(x, y);
        }

        /// <summary>
        /// Retrieves the size of this <see cref="Element"/>
        /// </summary>
        /// <param name="includePadding">Whether to include the applicable margin values or not</param>
        /// <returns>The width and heigh of this element</returns>
        public virtual Vector2 GetSize(bool includePadding = false)
        {
            int width = Width, height = Height;
            if (includePadding)
            {
                width += Padding.Left + Padding.Right;
                height += Padding.Top + Padding.Bottom;
            }
            return new(width, height);
        }

        /// <summary>
        /// Retrieves the size, including the padding and margin of this <see cref="Element"/>
        /// </summary>
        public virtual Vector2 GetSizeForSpacing()
        {
            Vector2 size = GetSize(true);
            size += new Vector2(Margin.Left + Margin.Right, Margin.Top + Margin.Bottom);
            return size;
        }

        /// <summary>
        /// Get the bounds (position and size) of this element
        /// </summary>
        /// <param name="includeMargin">Include the applicable margin in the size or not</param>
        /// <param name="includePadding">Include the applicable margin in the position or not</param>
        /// <returns>The bounds of this element</returns>
        public virtual Rectangle GetBounds(bool includeMargin = false, bool includePadding = false)
        {
            Vector2 position = GetPosition(includeMargin);
            Vector2 size = GetSize(includePadding);
            return new((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
        }

        internal virtual void windowSizeChanged(Rectangle oldBounds, Rectangle newBounds) => OnWindowSizeChanged(oldBounds, newBounds);

        /// <summary>
        /// The default listener for the <see cref="WindowSizeChanged"/> event
        /// </summary>
        /// <param name="oldBounds">The bounds of the window before it was resized</param>
        /// <param name="newBounds">The bounds of the window after it was resized</param>
        public virtual void OnWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            VerticalAlignment = verticalAlignment;
            HorizontalAlignment = horizontalAlignment;
        }

        /// <summary>
        /// The default listener for property changes of this <see cref="Element"/>
        /// </summary>
        /// <param name="propertyName">The name of the property which has changed</param>
        public virtual void OnPropertyChanged(string propertyName)
        {
            Logger.Verbose($"{GetType().Name} - {propertyName}");
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
        /// [Internal] Tests if the mouse is over this element
        /// </summary>
        /// <param name="x">The x position of the mouse</param>
        /// <param name="y">The y position of the mouse</param>
        /// <returns>True if the mouse is over this element, otherwise false</returns>
        internal bool tryHover(int x, int y) => GetBounds(true, true).Contains(x, y);
    }
}
