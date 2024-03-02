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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAML.Elements
{
    public class CheckBox : Element
    {
        private Texture2D texture = Game1.mouseCursors;
        private Rectangle sourceRect = new(227, 425, 9, 9);
        private Color backgroundColor = Color.White;
        private bool isChecked = false;
        private DrawEffects textureDrawEffects = new() { Scale = 4f };

        private bool isDefaultTexture = true;
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
        /// The texture of this <see cref="CheckBox"/>
        /// </summary>
        public Texture2D Texture
        {
            get => texture;
            set
            {
                texture = value;
                isDefaultTexture = false;
                invokePropertyChanged();
            }
        }
        /// <summary>
        /// The source rectangle for the texture of this <see cref="CheckBox"/>
        /// </summary>
        public Rectangle SourceRect
        {
            get => sourceRect;
            set
            {
                sourceRect = value;
                isDefaultTexture = false;
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
        /// The value of this <see cref="CheckBox"/>
        /// </summary>
        public bool IsChecked
        {
            get => isChecked;
            set
            {
                isChecked = value;
                onToggle();
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
        /// An event which fires when the left mouse button is pressed over this <see cref="CheckBox"/>
        /// </summary>
        public event MouseButtonEventHandler? LeftClick
        {
            add => AddListener(EventIds.LeftClick, value);
            remove => RemoveListener(EventIds.LeftClick, value);
        }
        /// <summary>
        /// An event which fires when the right mouse button is pressed over this <see cref="CheckBox"/>
        /// </summary>
        public event MouseButtonEventHandler? RightClick
        {
            add => AddListener(EventIds.RightClick, value);
            remove => RemoveListener(EventIds.RightClick, value);
        }
        /// <summary>
        /// An event which fires when a gamepad button is pressed over this <see cref="CheckBox"/>
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
        /// An event which fires when the cursor enters this <see cref="CheckBox"/>
        /// </summary>
        public event MouseButtonEventHandler? MouseEnter
        {
            add => AddListener(EventIds.MouseEnter, value);
            remove => RemoveListener(EventIds.MouseEnter, value);
        }
        /// <summary>
        /// An event which fires when the cursor leaves this <see cref="CheckBox"/>
        /// </summary>
        public event MouseButtonEventHandler? MouseLeave
        {
            add => AddListener(EventIds.MouseLeave, value);
            remove => RemoveListener(EventIds.MouseLeave, value);
        }
        /// <summary>
        /// An event which fires when the value of this <see cref="CheckBox"/> changes
        /// </summary>
        public event ToggleEventHandler? Toggle;
        /// <summary>
        /// An event which fires when the value of this <see cref="CheckBox"/> changes to true
        /// </summary>
        public event ToggleEventHandler? Checked;
        /// <summary>
        /// An event which fires when the value of this <see cref="CheckBox"/> changes to false
        /// </summary>
        public event ToggleEventHandler? UnChecked;

        public CheckBox()
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
        public virtual void OnLeftClick(int x, int y, bool playSound = true) => IsChecked = !IsChecked;

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
            if (b == Buttons.A)
                IsChecked = !IsChecked;
        }

        /// <summary>
        /// The default listener for the <see cref="Hover"/> event
        /// </summary>
        /// <param name="x">The horizontal position of the mouse on screen at the time of the event</param>
        /// <param name="y">The vertical position of the mouse on screen at the time of the event</param>
        public virtual void OnHover(int x, int y) { }

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

        /// <summary>
        /// The default listener for the <see cref="Toggle"/> event
        /// </summary>
        public virtual void OnToggle()
        {
            if (!isDefaultTexture)
                return;
            if (!IsChecked)
                sourceRect = new(227, 425, 9, 9);
            else
                sourceRect = new(236, 425, 9, 9);
        }

        public override void Draw(SpriteBatch b)
        {
            bool forcePure = Parent is not null and Element e && e.ForcePurePosition;
            b.Draw(Texture, GetPosition(!forcePure), SourceRect, BackgroundColor, TextureDrawEffects.Rotation, TextureDrawEffects.Origin, TextureDrawEffects.Scale, TextureDrawEffects.SpriteEffects, TextureDrawEffects.ZIndex);
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

        private void onToggle()
        {
            OnToggle();
            Toggle?.Invoke(this, new(isChecked));
            if (isChecked)
                Checked?.Invoke(this, new(isChecked));
            else
                UnChecked?.Invoke(this, new(isChecked));
        }

        private void onTextureDrawEffectsChanged(object sender, PropertyChangedEventArgs e) => invokePropertyChanged($"{nameof(TextureDrawEffects)}.{e.PropertyName}");
    }
}
