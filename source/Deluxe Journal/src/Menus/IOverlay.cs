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
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using DeluxeJournal.Framework;

namespace DeluxeJournal.Menus
{
    public abstract class IOverlay : IClickableMenu, IDisposable
    {
        /// <summary>Minimum resize width.</summary>
        public const int MinWidth = 100;

        /// <summary>Minumum resize height.</summary>
        public const int MinHeight = 100;

        /// <summary>Size of the resize boxes shown in edit-mode.</summary>
        public const int ResizeBoxSize = 16;

        /// <summary>Background texture box source.</summary>
        public static readonly Rectangle BackgroundSource = new(48, 58, 3, 3);

        /// <summary>Outline texture box source.</summary>
        public static readonly Rectangle OutlineSource = new(48, 61, 3, 3);

        /// <summary>Background texture box color per screen.</summary>
        private static readonly PerScreen<Color> BackgroundColorPerScreen = new(() => Context.ScreenId == 0 ? Color.Black * 0.25f : BackgroundColorPerScreen!.GetValueForScreen(0));

        /// <summary>Background texture box color.</summary>
        public static Color BackgroundColor
        {
            get => BackgroundColorPerScreen.Value;
            set => BackgroundColorPerScreen.Value = value;
        }

        /// <summary>Background color opacity value between <c>0f</c> and <c>1f</c>.</summary>
        public static float BackgroundOpacity => BackgroundColor.A / 255f;

        private Point _oldWindowSize;
        private int _width;
        private int _height;

        /// <summary>Registered page ID value assigned by the <see cref="PageRegistry"/> (this value is set immediately AFTER construction).</summary>
        public string PageId { get; set; } = string.Empty;

        /// <summary>Overlay bounds snapped to the screen edge if within the <see cref="SnapDistance"/>.</summary>
        public Rectangle EdgeSnappedBounds { get; private set; }

        /// <summary><see cref="Rectangle"/> wrapper for the <see cref="IClickableMenu"/> bounds.</summary>
        public virtual Rectangle Bounds
        {
            get => new(xPositionOnScreen, yPositionOnScreen, _width, _height);

            set
            {
                Resize(value.Width, value.Height);
                Move(value.X, value.Y);
            }
        }

        /// <summary>On-screen coordinates of this overlay.</summary>
        public new Point Position
        {
            get => new(xPositionOnScreen, yPositionOnScreen);
            set => Move(value.X, value.Y);
        }

        /// <summary>Size of this overlay.</summary>
        public Point Size
        {
            get => new(_width, _height);
            set => Resize(value.X, value.Y);
        }

        /// <summary>Whether the overlay content should be interactable.</summary>
        public virtual bool IsContentInteractable => IsVisible && !IsEditing && Game1.activeClickableMenu == null;

        /// <summary>Whether this overlay visible on-screen.</summary>
        public virtual bool IsVisible { get; set; }

        /// <summary>Whether the visibility of overlay can be toggled via hotkey.</summary>
        public bool IsVisibilityLocked { get; set; }

        /// <summary>Whether the overlay is currently in edit-mode.</summary>
        public bool IsEditing { get; set; }

        /// <summary>Whether the custom color is optional or is always used.</summary>
        public virtual bool IsColorOptional { get; set; }

        /// <summary>Whether the custom color should be used.</summary>
        public virtual bool IsColorSelected { get; set; }

        /// <summary>Custom color selected by the player.</summary>
        public Color CustomColor { get; set; }

        /// <summary>Distance from the screen edge to start snapping.</summary>
        public int SnapDistance { get; set; } = 4;

        /// <summary>Margin between the screen edge when snapped.</summary>
        public int SnapMargin { get; set; } = -4;

        /// <summary>Unique split-screen ID that this overlay is visible on. Set on instantiation.</summary>
        protected int ScreenId { get; }

        public IOverlay(Rectangle bounds)
            : this(bounds.X, bounds.Y, bounds.Width, bounds.Height)
        {
        }

        public IOverlay(int x, int y, int width, int height)
            : base(x, y, 0, 0, false)
        {
            _width = width;
            _height = height;

            if (Game1.uiMode)
            {
                _oldWindowSize = new(Game1.uiViewport.Width, Game1.uiViewport.Height);
            }
            else
            {
                float uiMultiplier = Game1.options.zoomLevel / Game1.options.uiScale;
                _oldWindowSize = new((int)(Game1.viewport.Width * uiMultiplier), (int)(Game1.viewport.Height * uiMultiplier));
            }

            ScreenId = Context.ScreenId;
        }

        /// <summary>Move the overlay in the specified on-screen coordinates.</summary>
        public virtual void Move(int x, int y)
        {
            xPositionOnScreen = Math.Clamp(x, 0, Game1.uiViewport.Width - _width);
            yPositionOnScreen = Math.Clamp(y, 0, Game1.uiViewport.Height - _height);
            CalculateEdgeSnappedBounds();
        }

        /// <summary>Resize the overlay to the specified dimensions.</summary>
        public virtual void Resize(int width, int height)
        {
            _width = Math.Clamp(width, MinWidth, Game1.uiViewport.Width - ResizeBoxSize);
            _height = Math.Clamp(height, MinHeight, Game1.uiViewport.Height - ResizeBoxSize);
            CalculateEdgeSnappedBounds();
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            Point windowSize = new(Game1.uiViewport.Width, Game1.uiViewport.Height);
            int x = (int)((windowSize.X - _width) * (xPositionOnScreen / (float)(_oldWindowSize.X - _width)));
            int y = (int)((windowSize.Y - _height) * (yPositionOnScreen / (float)(_oldWindowSize.Y - _height)));
            _oldWindowSize = windowSize;

            Resize(_width, _height);
            Move(x, y);
        }

        public override void draw(SpriteBatch b)
        {
            if (IsVisible && Game1.activeClickableMenu is not DeluxeJournalMenu)
            {
                DrawContents(b);
            }
        }

        /// <summary>Draw with edit-mode graphics.</summary>
        public virtual void DrawInEditMode(SpriteBatch b)
        {
            DrawContents(b);
        }

        /// <summary>Draw the overlay contents.</summary>
        public abstract void DrawContents(SpriteBatch b);

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <param name="disposing">Whether this method was invoked from the <see cref="IDisposable.Dispose"/> implementation or the finalizer.</param>
        /// <inheritdoc cref="IDisposable.Dispose"/>
        protected virtual void Dispose(bool disposing)
        {
        }

        /// <summary>Get the overlay bounds snapped to the screen edge.</summary>
        protected virtual void CalculateEdgeSnappedBounds()
        {
            Rectangle bounds = Bounds;

            if (bounds.X <= SnapDistance)
            {
                bounds.X = SnapMargin;
                bounds.Width -= SnapMargin;
            }
            else if (bounds.Right >= Game1.uiViewport.Width - SnapDistance)
            {
                bounds.X = Game1.uiViewport.Width - bounds.Width;
                bounds.Width -= SnapMargin;
            }

            if (bounds.Y <= SnapDistance)
            {
                bounds.Y = SnapMargin;
                bounds.Height -= SnapMargin;
            }
            else if (bounds.Bottom >= Game1.uiViewport.Height - SnapDistance)
            {
                bounds.Y = Game1.uiViewport.Height - bounds.Height;
                bounds.Height -= SnapMargin;
            }

            EdgeSnappedBounds = bounds;
        }
    }
}
