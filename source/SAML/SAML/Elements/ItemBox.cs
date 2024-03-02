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
    public class ItemBox : Element
    {
        private Item? item;
        private Color color = Color.White;
        private Texture2D texture = Game1.menuTexture;
        private Rectangle sourcRect = new(128, 128, 64, 64);
        private Color backgroundColor = Color.White;
        private StackDrawType itemStackDrawType = StackDrawType.Both;
        private bool isGrayedOut = false;
        private DrawEffects textureDrawEffects = new();
        private DrawEffects contentDrawEffects = new() { ZIndex = 1.1f, HoverScaleIncrease = 0.1f };
        private bool canTake = true;
        private bool canPlace = true;

        private float contentScale;
        private bool isHovered = false;
        private bool isForcedWidth = false;
        private bool isForcedHeight = false;

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
        /// The Item attached to this <see cref="ItemBox"/>
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
        /// The color of the item
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
        /// The texture of this <see cref="ItemBox"/>
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
        /// The source rectangle for the texture of this <see cref="ItemBox"/>
        /// </summary>
        public Rectangle SourceRect
        {
            get => sourcRect;
            set
            {
                sourcRect = value;
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
        /// Determines whether this <see cref="ItemBox"/> is unlocked and useable or not
        /// </summary>
        public bool IsGrayedOut
        {
            get => isGrayedOut;
            set
            {
                isGrayedOut = value;
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
        /// Determines whether this <see cref="ItemBox"/> allows it's Item to be taken out or not
        /// </summary>
        public bool CanTake
        {
            get => canTake;
            set
            {
                canTake = value;
                invokePropertyChanged();
            }
        }
        /// <summary>
        /// Determines whether this <see cref="ItemBox"/> allows items to be placed inside or not
        /// </summary>
        public bool CanPlace
        {
            get => canPlace;
            set
            {
                canPlace = value;
                invokePropertyChanged();
            }
        }

        /// <summary>
        /// An event which fires when the left mouse button is pressed over this <see cref="ItemBox"/>
        /// </summary>
        public event MouseButtonEventHandler? LeftClick
        {
            add => AddListener(EventIds.LeftClick, value);
            remove => RemoveListener(EventIds.LeftClick, value);
        }
        /// <summary>
        /// An event which fires when the right mouse button is pressed over this <see cref="ItemBox"/>
        /// </summary>
        public event MouseButtonEventHandler? RightClick
        {
            add => AddListener(EventIds.RightClick, value);
            remove => RemoveListener(EventIds.RightClick, value);
        }
        /// <summary>
        /// An event which fires when a gamepad button is pressed over this <see cref="ItemBox"/>
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
        /// An event which fires when the cursor enters this <see cref="ItemBox"/>
        /// </summary>
        public event MouseButtonEventHandler? MouseEnter
        {
            add => AddListener(EventIds.MouseEnter, value);
            remove => RemoveListener(EventIds.MouseEnter, value);
        }
        /// <summary>
        /// An event which fires when the cursor leaves this <see cref="ItemBox"/>
        /// </summary>
        public event MouseButtonEventHandler? MouseLeave
        {
            add => AddListener(EventIds.MouseLeave, value);
            remove => RemoveListener(EventIds.MouseLeave, value);
        }

        public ItemBox()
        {
            OnPropertyChanged(nameof(SourceRect));
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
        public virtual void OnLeftClick(int x, int y, bool playSound = true)
        {
            if (Owner.HeldItem is null)
                Owner.HeldItem = TakeItem(Item?.Stack ?? -1);
            else
                Owner.HeldItem = TryPlaceItem(Owner.HeldItem, Owner.HeldItem?.Stack ?? -1);
        }

        /// <summary>
        /// The deault listener for the <see cref="RightClick"/> event
        /// </summary>
        /// <param name="x">The horizontal position of the mouse on screen at the time of the event</param>
        /// <param name="y">The vertical position of the mouse on screen at the time of the event</param>
        /// <param name="playSound">Whether this function should use sounds or not</param>
        public virtual void OnRightClick(int x, int y, bool playSound = true)
        {
            if (Owner.HeldItem is null)
                Owner.HeldItem = TakeItem();
            else if (Owner.HeldItem.canStackWith(Item) && CanTake)
            {
                Owner.HeldItem.addToStack(TakeItem());
            }
            else
                Owner.HeldItem = TryPlaceItem(Owner.HeldItem);
        }

        /// <summary>
        /// The default listener for the <see cref="GamePadButtonDown"/> event
        /// </summary>
        /// <param name="b">The buttons that were pressed at the time of the event</param>
        public virtual void OnGamePadButtonDown(Buttons b)
        {
            //I'm choosing A as left click and X as right click, don't agree? override it
            if (Owner.HeldItem is null)
            {
                if (b == Buttons.A)
                    Owner.HeldItem = TakeItem(Item?.Stack ?? -1);
                if (b == Buttons.X)
                    Owner.HeldItem = TakeItem();
            }
            else
            {
                if (b == Buttons.A || b == Buttons.X)
                    Owner.HeldItem = TryPlaceItem(Owner.HeldItem);
            }
        }

        /// <summary>
        /// The default listener for the <see cref="Hover"/> event
        /// </summary>
        /// <param name="x">The horizontal position of the mouse on screen at the time of the event</param>
        /// <param name="y">The vertical position of the mouse on screen at the time of the event</param>
        public virtual void OnHover(int x, int y)
        {
            if (isHovered)
                contentScale = Math.Min(contentScale + 0.04f, ContentDrawEffects.Scale + ContentDrawEffects.HoverScaleIncrease);
            else
                contentScale = Math.Max(contentScale - 0.04f, ContentDrawEffects.Scale);
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
        }

        /// <summary>
        /// Try to take the item from this <see cref="ItemBox"/>
        /// </summary>
        /// <param name="stack">How much of this item to try to take (-1 for auto determine)</param>
        /// <returns>A stack of the requested item, or null if it could not be retrieved</returns>
        public virtual Item? TakeItem(int stack = -1)
        {
            if (Item is null || IsGrayedOut || !CanTake)
                return null;

            Item? copy = getStackOfItem(Item, stack)!;

            Item.Stack -= copy.Stack;
            if (Item.Stack <= 0)
                Item = null;
            return copy;
        }

        /// <summary>
        /// Try to place an item inside this <see cref="ItemBox"/>
        /// </summary>
        /// <param name="item">The item to try to place</param>
        /// <param name="stack">The amount of the item to try to place (-1 for auto determine)</param>
        /// <returns>null or the old item if the item was placed, the item - the amount that was placed, or the item itself if it couldn't be placed</returns>
        public virtual Item? TryPlaceItem(Item item, int stack = -1)
        {
            if (IsGrayedOut || !CanPlace)
                return item;

            Item? toPlace = getStackOfItem(item, stack);
            if (toPlace == null) 
                return null;

            if (Item is null)
            {
                Item = toPlace;
                if ((item.Stack -= toPlace.Stack) <= 0)
                    return null;
                return item;
            }
            if (Item.canStackWith(item))
            {
                int remainder = Item.addToStack(toPlace);
                item.Stack -= toPlace.Stack - remainder;
                if (item.Stack <= 0)
                    return null;
                return item;
            }
            if (!CanTake)
                return item;
            Item old = Item;
            Item = item;
            return old;
        }

        /// <summary>
        /// Remove the item inside this <see cref="ItemBox"/>
        /// </summary>
        /// <returns>The item which was inside, or null if no item was inside or the box is grayed out</returns>
        public virtual Item? RemoveItem()
        {
            if (IsGrayedOut)
                return null;
            Item old = Item;
            Item = null;
            return old;
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

        public override void Draw(SpriteBatch b)
        {
            bool forcePure = Parent is not null and Element e && e.ForcePurePosition;
            b.Draw(Texture, GetPosition(!forcePure), SourceRect, BackgroundColor, TextureDrawEffects.Rotation, TextureDrawEffects.Origin, TextureDrawEffects.Scale, TextureDrawEffects.SpriteEffects, TextureDrawEffects.ZIndex);
            item.DrawInMenu(b, GetPosition(!forcePure), contentScale, 1f, ContentDrawEffects.Rotation, ContentDrawEffects.Origin, ContentDrawEffects.SpriteEffects, ContentDrawEffects.ZIndex, ItemStackDrawType, Color, true);
        }

        private void onTextureDrawEffectsChanged(object sender, PropertyChangedEventArgs e) => invokePropertyChanged($"{nameof(TextureDrawEffects)}.{e.PropertyName}");

        private void onContentDrawEffectsChanged(object sender, PropertyChangedEventArgs e) => invokePropertyChanged($"{nameof(ContentDrawEffects)}.{e.PropertyName}");

        private Item? getStackOfItem(Item item, int stack = -1)
        {
            if (item == null) 
                return null;

            if (stack == -1)
            {
                stack = 1;
                var kbState = Game1.GetKeyboardState();
                if (kbState.IsKeyDown(Keys.LeftShift) || kbState.IsKeyDown(Keys.RightShift))
                    stack = 5;
                if (kbState.IsKeyDown(Keys.LeftControl) || kbState.IsKeyDown(Keys.RightControl))
                    stack = 25;
            }

            if (stack > item.Stack)
                stack = item.Stack;

            Item copy = item.getOne();
            copy.Stack = stack;
            return copy;
        }
    }
}
