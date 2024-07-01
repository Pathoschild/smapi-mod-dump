/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace DeluxeJournal.Menus.Components
{
    /// <summary>Encapsulates scrolling logic.</summary>
    public class ScrollComponent : IClickableComponentSupplier
    {
        /// <summary>Raised after the scroll amount is changed.</summary>
        public event Action<ScrollComponent>? OnScroll;

        /// <summary>Scroll bar component ID.</summary>
        public const int ScrollBarId = 12220;

        /// <summary>Up arrow button component ID.</summary>
        public const int UpArrowButtonId = 12221;

        /// <summary>Down arrow button component ID.</summary>
        public const int DownArrowButtonId = 12222;

        private readonly ClickableTextureComponent _scrollBar;
        private readonly ClickableTextureComponent _upArrowButton;
        private readonly ClickableTextureComponent _downArrowButton;
        private readonly Rectangle _scrollBarBounds;
        private readonly Rectangle _contentBounds;

        private Rectangle? _cachedScissorRect;
        private int _contentHeight;
        private int _scrollAmount;
        private bool _scrolling;

        /// <summary>Bounds that the scroll bar may travel within.</summary>
        public Rectangle ScrollBarBounds => _scrollBarBounds;
        
        /// <summary>Content region bounds.</summary>
        public Rectangle ContentBounds => _contentBounds;

        /// <summary>Whether the content exceeds the bounds.</summary>
        public bool CanScroll => ContentHeight > _contentBounds.Height;

        /// <summary>Whether the <see cref="ScrollAmount"/> should clip to the nearest multiple of <see cref="ScrollDistance"/>.</summary>
        public bool ClipToScrollDistance { get; set; }

        /// <summary>Distance to travel on each scroll.</summary>
        public int ScrollDistance { get; set; }

        /// <summary>Height of the content within, or beyond, the <see cref="ContentBounds"/>.</summary>
        public int ContentHeight
        {
            get => _contentHeight;

            set
            {
                _contentHeight = value;
                Refresh();
            }
        }

        /// <summary>Total distance of the overflow content scrolled.</summary>
        public int ScrollAmount
        {
            get
            {
                return _scrollAmount;
            }

            set
            {
                int scrollAmount = _scrollAmount;
                _scrollAmount = ClipToScrollDistance ? (int)Math.Round(value / (double)ScrollDistance) * ScrollDistance : value;
                Refresh();

                if (scrollAmount != _scrollAmount)
                {
                    OnScroll?.Invoke(this);
                }
            }
        }

        public ScrollComponent(Rectangle scrollBarBounds, Rectangle contentBounds, int scrollDistance, bool clipToScrollDistance = false)
        {
            _scrollBarBounds = scrollBarBounds;
            _contentBounds = contentBounds;
            _cachedScissorRect = null;
            _scrollAmount = 0;
            _scrolling = false;

            _scrollBar = new ClickableTextureComponent(
                scrollBarBounds,
                Game1.mouseCursors,
                new Rectangle(435, 463, 6, 10),
                4f)
            {
                myID = ScrollBarId,
                upNeighborID = UpArrowButtonId,
                downNeighborID = DownArrowButtonId,
                upNeighborImmutable = true,
                downNeighborImmutable = true
            };
            
            _upArrowButton = new ClickableTextureComponent(
                new Rectangle(scrollBarBounds.X - 12, scrollBarBounds.Y - 48, 44, 48),
                Game1.mouseCursors,
                new Rectangle(421, 459, 11, 12),
                4f)
            {
                myID = UpArrowButtonId,
                downNeighborID = ScrollBarId,
                downNeighborImmutable = true
            };
            
            _downArrowButton = new ClickableTextureComponent(
                new Rectangle(scrollBarBounds.X - 12, scrollBarBounds.Y + scrollBarBounds.Height + 4, 44, 48),
                Game1.mouseCursors,
                new Rectangle(421, 472, 11, 12),
                4f)
            {
                myID = DownArrowButtonId,
                upNeighborID = ScrollBarId,
                upNeighborImmutable = true
            };

            ScrollDistance = scrollDistance;
            ClipToScrollDistance = clipToScrollDistance;
        }

        public IEnumerable<ClickableComponent> GetClickableComponents()
        {
            yield return _scrollBar;
            yield return _upArrowButton;
            yield return _downArrowButton;
        }

        /// <summary>Get the discrete number of times <see cref="ScrollDistance"/> has been applied to the <see cref="ScrollAmount"/>.</summary>
        public int GetScrollOffset()
        {
            return ScrollAmount / ScrollDistance;
        }

        /// <summary>Get the amount that the <see cref="ContentHeight"/> has overflowed over the <see cref="ContentBounds"/>.</summary>
        public int GetOverflowAmount()
        {
            return Math.Max(ContentHeight - _contentBounds.Height, 0);
        }

        /// <summary>Get the percent scrolled over the overflow region.</summary>
        /// <returns>A value in the range <c>[0f,1f]</c> representing the total amount scrolled.</returns>
        public float GetPercentScrolled()
        {
            float overflow = GetOverflowAmount();
            return (overflow > 0) ? _scrollAmount / overflow : 1f;
        }

        /// <summary>Scroll the <see cref="ScrollDistance"/> in the given direction.</summary>
        /// <param name="direction">
        /// A value greater than zero will scroll up (decrease the <see cref="ScrollAmount"/>);
        /// A value less than zero will scroll down (increase the <see cref="ScrollAmount"/>).
        /// </param>
        /// <param name="playSound">Whether sound should be played while scrolling.</param>
        public void Scroll(int direction, bool playSound = true)
        {
            if (CanScroll && direction != 0)
            {
                if (playSound)
                {
                    Game1.playSound("shiny4");
                }

                ScrollAmount -= Math.Sign(direction) * ScrollDistance;
            }
        }

        /// <summary>Set the <see cref="ScrollAmount"/> based on the mouse position within the scroll bounds.</summary>
        /// <param name="x">Mouse X position.</param>
        /// <param name="y">Mouse Y position.</param>
        /// <param name="playSound">Whether sound should be played while scrolling.</param>
        public void SetScroll(int x, int y, bool playSound = true)
        {
            if (!_scrollBarBounds.Contains(x, y))
            {
                return;
            }

            int oldScrollBarY = _scrollBar.bounds.Y;
            int scrollBarHeight = _scrollBar.bounds.Height;
            float percentage = (y - _scrollBarBounds.Y - scrollBarHeight / 2) / (float)(_scrollBarBounds.Height - scrollBarHeight);
            ScrollAmount = (int)(Math.Clamp(percentage, 0f, 1f) * GetOverflowAmount());

            if (playSound && oldScrollBarY != _scrollBar.bounds.Y)
            {
                Game1.playSound("shiny4");
            }
        }

        /// <summary>Receive a mouse click.</summary>
        /// <param name="x">Mouse X position.</param>
        /// <param name="y">Mouse Y position.</param>
        /// <param name="playSound">Whether sound should be played while scrolling.</param>
        public virtual void ReceiveLeftClick(int x, int y, bool playSound = true)
        {
            if (CanScroll)
            {
                if (_downArrowButton.containsPoint(x, y) && _scrollAmount < GetOverflowAmount())
                {
                    _downArrowButton.scale = _downArrowButton.baseScale;
                    Scroll(-1, false);
                }
                else if (_upArrowButton.containsPoint(x, y) && _scrollAmount > 0)
                {
                    _upArrowButton.scale = _upArrowButton.baseScale;
                    Scroll(1, false);
                }
                else
                {
                    if (_scrollBar.containsPoint(x, y) || _scrollBarBounds.Contains(x, y))
                    {
                        _scrolling = true;
                    }

                    return;
                }

                if (playSound)
                {
                    Game1.playSound("shiny4");
                }
            }
        }

        /// <summary>Update while left click is held.</summary>
        /// <param name="x">Mouse X position.</param>
        /// <param name="y">Mouse Y position.</param>
        /// <param name="playSound">Whether sound should be played while scrolling.</param>
        public virtual void LeftClickHeld(int x, int y, bool playSound = true)
        {
            if (_scrolling)
            {
                SetScroll(_scrollBarBounds.X, y, playSound);
            }
        }

        /// <summary>Update on left click released.</summary>
        /// <param name="x">Mouse X position.</param>
        /// <param name="y">Mouse Y position.</param>
        public virtual void ReleaseLeftClick(int x, int y)
        {
            _scrolling = false;
        }

        /// <summary>Try to perform a hover action given the mouse position.</summary>
        /// <param name="x">Mouse X position.</param>
        /// <param name="y">Mouse Y position.</param>
        public virtual void TryHover(int x, int y)
        {
            if (CanScroll)
            {
                _upArrowButton.tryHover(x, y);
                _downArrowButton.tryHover(x, y);
                _scrollBar.tryHover(x, y);
            }
        }

        /// <summary>Begin the scissor test for the content region.</summary>
        public virtual void BeginScissorTest(SpriteBatch b)
        {
            _cachedScissorRect = b.GraphicsDevice.ScissorRectangle;

            b.End();
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, new RasterizerState()
            {
                ScissorTestEnable = true
            });

            b.GraphicsDevice.ScissorRectangle = _contentBounds;
        }

        /// <summary>End the scissor test for the content region.</summary>
        public virtual void EndScissorTest(SpriteBatch b)
        {
            b.End();

            if (_cachedScissorRect != null)
            {
                b.GraphicsDevice.ScissorRectangle = (Rectangle)_cachedScissorRect;
            }

            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
        }

        /// <summary>Draw the scroll bar.</summary>
        public virtual void DrawScrollBar(SpriteBatch b)
        {
            if (CanScroll)
            {
                IClickableMenu.drawTextureBox(b,
                    Game1.mouseCursors,
                    new(403, 383, 6, 6),
                    _scrollBarBounds.X,
                    _scrollBarBounds.Y,
                    _scrollBarBounds.Width,
                    _scrollBarBounds.Height,
                    Color.White,
                    4f,
                    false);

                Rectangle barBounds = _scrollBar.bounds;
                Rectangle barSource = _scrollBar.sourceRect;
                b.Draw(_scrollBar.texture, new Rectangle(barBounds.X, barBounds.Y, barBounds.Width, 16), new(barSource.X, barSource.Y, barSource.Width, 4), Color.White);
                b.Draw(_scrollBar.texture, new Rectangle(barBounds.X, barBounds.Y + 16, barBounds.Width, barBounds.Height - 32), new(barSource.X, barSource.Y + 4, barSource.Width, 1), Color.White);
                b.Draw(_scrollBar.texture, new Rectangle(barBounds.X, barBounds.Bottom - 16, barBounds.Width, 16), new(barSource.X, barSource.Bottom - 4, barSource.Width, 4), Color.White);

                _upArrowButton.draw(b);
                _downArrowButton.draw(b);
            }
        }

        /// <summary>Refresh the scrolling logic.</summary>
        protected virtual void Refresh()
        {
            if (!CanScroll)
            {
                _scrollAmount = 0;
                return;
            }

            int overflow = GetOverflowAmount();

            if (_scrollAmount < 8)
            {
                _scrollAmount = 0;
            }
            else if (_scrollAmount > overflow - 8)
            {
                _scrollAmount = overflow;
            }

            int scrollBarHeight = _scrollBarBounds.Height * _contentBounds.Height / ContentHeight;
            int offset = (int)((_scrollBarBounds.Height - scrollBarHeight) * GetPercentScrolled());
            _scrollBar.bounds.Y = _scrollBarBounds.Y + offset;
            _scrollBar.bounds.Height = scrollBarHeight;
        }
    }
}
