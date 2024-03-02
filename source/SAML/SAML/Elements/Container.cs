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
using SAML.Utilities.Collections;

namespace SAML.Elements
{
    public class Container : Element
    {
        private ObservableList<Element> elements = [];
        private bool autoSize = true;
        private Predicate<Element> filter = x => x.Visible;

        /// <summary>
        /// The elements owned by this <see cref="Container"/>
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
        /// Whether or not this <see cref="Container"/> should adjust it's size based on it's children
        /// </summary>
        public bool AutoSize
        {
            get => autoSize;
            set
            {
                autoSize = value;
                invokePropertyChanged();
            }
        }
        /// <summary>
        /// A filter to apply over the elements to determine if they should be displayed or not
        /// </summary>
        public Predicate<Element> Filter
        {
            get => filter;
            set
            {
                filter = value;
                invokePropertyChanged();
            }
        }
        /// <summary>
        /// A collection of the elements with the <see cref="Filter"/> applied
        /// </summary>
        public IEnumerable<Element> FilteredElements
        {
            get
            {
                foreach (var element in elements)
                    if (Filter.Invoke(element))
                        yield return element;
            }
        }

        public Container() : base()
        {
            elements.CollectionChanged += OnElementsChanged;
        }

        public override void Draw(SpriteBatch b)
        {
            foreach (var element in FilteredElements)
                element.draw(b);
        }

        public override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == nameof(Owner))
                foreach (var el in Elements)
                    el.Owner = Owner;
        }

        /// <summary>
        /// The default listener for changes to the Elements of this <see cref="Container"/>
        /// </summary>
        protected virtual void OnElementsChanged(object sender, CollectionChangedEventArgs e)
        {
            if (e.Added is not null and IEnumerable<Element> added)
            {
                foreach (var el in added)
                {
                    el.Parent = this;
                    el.Owner = Owner;
                    el.PropertyChanged += OnElementPropertyChanged;
                }
            }
            if (e.Removed is not null and IEnumerable<Element> removed)
            {
                foreach (var el in removed)
                {
                    el.Parent = null;
                    el.Owner = null;
                    el.PropertyChanged -= OnElementPropertyChanged;
                }
            }

            if (AutoSize)
            {
                int maxWidth = -1;
                int maxHeight = -1;

                foreach (var el in FilteredElements)
                {
                    Vector2 size = el.GetSize(true);
                    size += new Vector2(el.Margin.Right + el.Margin.Left, el.Margin.Bottom + el.Margin.Top);
                    if (size.X > maxWidth)
                        maxWidth = (int)size.X;
                    if (size.Y > maxHeight) 
                        maxHeight = (int)size.Y;
                }

                Width = maxWidth;
                Height = maxHeight;
            }
        }

        /// <summary>
        /// The default handler for the <see cref="Element.PropertyChanged"/> event for all elements of this <see cref="Container"/>
        /// </summary>
        protected virtual void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //Ignore property changes for x, y, and subsequent alignment to avoid stack overflow calls in stackpanel / wrappanel
            if (e.PropertyName == nameof(X) || e.PropertyName == nameof(Y) || e.PropertyName == nameof(HorizontalAlignment) || e.PropertyName == nameof(VerticalAlignment))
                return;
            invokePropertyChanged(nameof(FilteredElements));
        }

        internal override void windowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.windowSizeChanged(oldBounds, newBounds);
            foreach (var element in Elements)
                element.windowSizeChanged(oldBounds, newBounds);
        }

        internal override void update(GameTime time)
        {
            base.update(time);
            foreach (var element in elements)
                element.update(time);
        }

        /// <summary>
        /// [Internal] perform a new hover loop to check mouse enter / mouse leave conditions for container elements
        /// </summary>
        /// <param name="x">The horizontal position of the cursor</param>
        /// <param name="y">The vertical position of the cursor</param>
        internal void performHoverAction(int x, int y)
        {
            foreach (var el in Elements)
            {
                bool flag1 = el.tryHover(x, y);
                bool flag2 = Owner.HoveredElements.Contains(el);
                if (flag1 && !flag2)
                {
                    Owner.HoveredElements.Add(el);
                    Fire(EventIds.MouseEnter, new MouseButtonEventArgs(x, y), this, new[] { el });
                }
                if (!flag1 && flag2)
                {
                    Owner.HoveredElements.Remove(el);
                    Fire(EventIds.MouseLeave, new MouseButtonEventArgs(x, y), this, new[] { el });
                }
                if (el is Container c)
                    c.performHoverAction(x, y);
            }
        }

        private void Fire<T>(EventIds eventId, T args, object? sender = null, IEnumerable<IElement>? targets = null) where T : EventArgs => EventManager.Fire(eventId, args, sender, targets);
    }
}
