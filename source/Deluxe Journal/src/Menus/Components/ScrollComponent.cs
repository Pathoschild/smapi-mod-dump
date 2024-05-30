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
    public class ScrollComponent
    {
        public readonly ClickableTextureComponent upArrowButton;
        public readonly ClickableTextureComponent downArrowButton;
        public readonly ClickableTextureComponent scrollBar;

        private Rectangle _scrollBarBounds;
        private Rectangle _contentBounds;
        private Rectangle? _cachedScissorRect;
        private int _scrollAmount;
        private bool _scrolling;

        public Rectangle ScrollBarBounds => _scrollBarBounds;
        
        public Rectangle ContentBounds => _contentBounds;

        public bool ClipToScrollDistance { get; set; }

        public int ScrollDistance { get; set; }

        public int ContentHeight { get; set; }

        public int ScrollAmount
        {
            get
            {
                return _scrollAmount;
            }

            set
            {
                _scrollAmount = value;
                SetScrollFromAmount();
            }
        }

        public ScrollComponent(Rectangle scrollBarBounds, Rectangle contentBounds, int scrollDistance, bool clipToScrollDistance = false)
        {
            _scrollBarBounds = scrollBarBounds;
            _contentBounds = contentBounds;
            _cachedScissorRect = null;
            _scrollAmount = 0;
            _scrolling = false;

            ScrollDistance = scrollDistance;
            ClipToScrollDistance = clipToScrollDistance;

            Rectangle bounds = new Rectangle(_scrollBarBounds.X, _scrollBarBounds.Y, _scrollBarBounds.Width, 40);
            scrollBar = new ClickableTextureComponent(bounds, Game1.mouseCursors, new Rectangle(435, 463, 6, 10), 4f);

            bounds = new Rectangle(_scrollBarBounds.X - 12, _scrollBarBounds.Y - 52, 44, 48);
            upArrowButton = new ClickableTextureComponent(bounds, Game1.mouseCursors, new Rectangle(421, 459, 11, 12), 4f);

            bounds = new Rectangle(_scrollBarBounds.X - 12, _scrollBarBounds.Y + _scrollBarBounds.Height + 4, 44, 48);
            downArrowButton = new ClickableTextureComponent(bounds, Game1.mouseCursors, new Rectangle(421, 472, 11, 12), 4f);
        }

        public int GetScrollOffset()
        {
            return ScrollAmount / ScrollDistance;
        }

        public int GetOverflowAmount()
        {
            return Math.Max(ContentHeight - _contentBounds.Height, 0);
        }

        public float GetPercentScrolled()
        {
            float overflow = GetOverflowAmount();
            return (overflow > 0) ? _scrollAmount / overflow : 1f;
        }

        public bool CanScroll()
        {
            return ContentHeight > _contentBounds.Height;
        }

        public void Refresh()
        {
            SetScrollFromAmount();
        }

        public void Scroll(int direction, bool playSound = true)
        {
            if (CanScroll())
            {
                if (playSound)
                {
                    Game1.playSound("shiny4");
                }

                ScrollAmount -= Math.Sign(direction) * ScrollDistance;
            }
        }

        public void SetScrollFromY(int y, bool playSound = true)
        {
            int oldScrollBarY = scrollBar.bounds.Y;
            float percentage = (y - _scrollBarBounds.Y) / (float)(_scrollBarBounds.Height - scrollBar.bounds.Height);
            float scrollAmount = Utility.Clamp(percentage, 0, 1f) * (ContentHeight - _contentBounds.Height);

            if (ClipToScrollDistance)
            {
                ScrollAmount = (int)(scrollAmount / ScrollDistance) * ScrollDistance;
            }
            else
            {
                ScrollAmount = (int)scrollAmount;
            }

            if (playSound && oldScrollBarY != scrollBar.bounds.Y)
            {
                Game1.playSound("shiny4");
            }
        }

        private void SetScrollFromAmount()
        {
            int overflow = GetOverflowAmount();

            if (!CanScroll())
            {
                _scrollAmount = 0;
                return;
            }
            else if (_scrollAmount < 8)
            {
                _scrollAmount = 0;
            }
            else if (_scrollAmount > overflow - 8)
            {
                _scrollAmount = overflow;
            }

            int offset = (int)((_scrollBarBounds.Height - scrollBar.bounds.Height) * GetPercentScrolled());
            scrollBar.bounds.Y = _scrollBarBounds.Y + offset;
        }

        public virtual void ReceiveLeftClick(int x, int y, bool playSound = true)
        {
            if (CanScroll())
            {
                if (downArrowButton.containsPoint(x, y) && _scrollAmount < GetOverflowAmount())
                {
                    downArrowButton.scale = downArrowButton.baseScale;
                    Scroll(-1, false);
                }
                else if (upArrowButton.containsPoint(x, y) && _scrollAmount > 0)
                {
                    upArrowButton.scale = upArrowButton.baseScale;
                    Scroll(1, false);
                }
                else
                {
                    if (scrollBar.containsPoint(x, y) || _scrollBarBounds.Contains(x, y))
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

        public virtual void LeftClickHeld(int x, int y, bool playSound = true)
        {
            if (_scrolling)
            {
                SetScrollFromY(y, playSound);
            }
        }

        public virtual void ReleaseLeftClick(int x, int y)
        {
            _scrolling = false;
        }

        public virtual void TryHover(int x, int y)
        {
            if (CanScroll())
            {
                upArrowButton.tryHover(x, y);
                downArrowButton.tryHover(x, y);
                scrollBar.tryHover(x, y);
            }
        }

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

        public virtual void EndScissorTest(SpriteBatch b)
        {
            b.End();

            if (_cachedScissorRect != null)
            {
                b.GraphicsDevice.ScissorRectangle = (Rectangle)_cachedScissorRect;
            }

            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
        }

        public virtual void DrawScrollBar(SpriteBatch b)
        {
            if (CanScroll())
            {
                upArrowButton.draw(b);
                downArrowButton.draw(b);
                scrollBar.draw(b);
            }
        }
    }
}
