/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Menus;

using Leclair.Stardew.Common.UI.FlowNode;

namespace Leclair.Stardew.Common.UI
{

	public enum ScrollVisibility {
		Hidden,
		Visible,
		Auto
	};

    public class ScrollableFlow {

#if DEBUG
		private static readonly Color[] DEBUG_COLORS = new Color[] {
			Color.Blue,
			Color.Purple,
			Color.Gray,
			Color.Gold,
			Color.Fuchsia,
			Color.Orange
		};
#endif

		public static readonly Rectangle SCROLL_AREA_BACKGROUND = new(403, 383, 6, 6);

		public readonly IClickableMenu ParentMenu;

		public readonly int Region;

		private int lastID;

		private int _x;
		private int _y;
		private int _width;
		private int _height;

		public Rectangle Bounds { get; private set; }

		private int _scrollTopMargin = 0;
		private int _scrollBottomMargin = 0;

		private SpriteFont _defaultFont;
		private ScrollVisibility showScrollbar;

		public Texture2D ScrollAreaTexture;
		public Rectangle ScrollAreaSource;

		public ClickableTextureComponent btnPageUp { get; private set; }
		public ClickableTextureComponent btnPageDown { get; private set; }

		public ClickableTextureComponent ScrollBar { get;private set; }
		public Rectangle ScrollArea { get; private set; }

		public List<ClickableComponent> DynamicComponents { get; private set; }

		private CachedFlow? Flow;

		private float ScrollStep = 1f;
		private float ScrollOffset = 0f;
		private float ScrollMax = 0f;

		private bool MiddleScrolling = false;
		private float MiddleStart = -1f;

		private bool Scrolling = false;
		private float ScrollingOffset = 0f;

		public ScrollableFlow(IClickableMenu parent, int x, int y, int width, int height, ScrollVisibility showScrollbar = ScrollVisibility.Auto, int region = 5000, int firstID = 100) {
			ParentMenu = parent;
			Region = region;

			ScrollAreaTexture = Game1.mouseCursors;
			ScrollAreaSource = SCROLL_AREA_BACKGROUND;

			this.showScrollbar = showScrollbar;

			_x = x;
			_y = y;
			_width = width;
			_height = height;

			// Create components
			ScrollBar = new ClickableTextureComponent(
				new Rectangle(0, 0, 44, 48),
				Game1.mouseCursors,
				new Rectangle(435, 463, 6, 10),
				4f
			);

			btnPageUp = new ClickableTextureComponent(
				new Rectangle(0, 0, 64, 64),
				Game1.mouseCursors,
				new Rectangle(64, 64, 64, 64),
				0.8f
			) {
				region = region,
				myID = firstID,
				upNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				downNeighborID = firstID + 1,
				leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				rightNeighborID = ClickableComponent.SNAP_AUTOMATIC
			};

			btnPageDown = new ClickableTextureComponent(
				new Rectangle(0, 0, 64, 64),
				Game1.mouseCursors,
				new Rectangle(0, 64, 64, 64),
				0.8f
			) {
				region = region,
				myID = firstID + 1,
				upNeighborID = firstID,
				downNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				rightNeighborID = ClickableComponent.SNAP_AUTOMATIC
			};

			lastID = firstID + 1;

			DynamicComponents = new();

			UpdateLayout();
		}

		#region Properties

		public int X {
			get => _x;
			set {
				if (_x != value) {
					_x = value;
					UpdateLayout();
				}
			}
		}

		public int Y {
			get => _y;
			set {
				if (_y != value) {
					_y = value;
					UpdateLayout();
				}
			}
		}

		public int Width {
			get => _width;
			set {
				if (_width != value) {
					_width = value;
					UpdateLayout();
				}
			}
		}

		public int Height {
			get => _height;
			set {
				if (_height != value) {
					_height = value;
					UpdateLayout();
				}
			}
		}

		public SpriteFont DefaultFont {
			get => _defaultFont;
			set {
				if (value != _defaultFont) {
					_defaultFont = value;
					UpdateFlow();
				}
			}
		}

		public ScrollVisibility ShowScrollbar {
			get => showScrollbar;
			set {
				if (showScrollbar != value) {
					showScrollbar = value;
					UpdateLayout();
				}
			}
		}

		public int FlowWidth => _width - (showScrollbar == ScrollVisibility.Hidden ? 0 : 80);

		public int Position => (int) ScrollOffset;

		public bool HasValue => Flow.HasValue;

		#endregion

		#region Updating Components

		public bool IsScrollVisible {
			get {
				if (showScrollbar == ScrollVisibility.Hidden)
					return false;
				if (showScrollbar == ScrollVisibility.Visible)
					return true;
				return ScrollMax > 0;
			}
		}

		public bool Reposition(int? x, int? y, int? width, int? height, int? scrollTopMargin, int? scrollBottomMargin) {
			int newX = x ?? _x;
			int newY = y ?? _y;
			int newWidth = width ?? _width;
			int newHeight = height ?? _height;
			int newTopMargin = scrollTopMargin ?? _scrollTopMargin;
			int newBottomMargin = scrollBottomMargin ?? _scrollBottomMargin;

			if (
				newX == _x &&
				newY == _y &&
				newWidth == _width &&
				newHeight == _height &&
				newTopMargin == _scrollTopMargin &&
				newBottomMargin == _scrollBottomMargin
			)
				return false;

			_x = newX;
			_y = newY;
			_width = newWidth;
			_height = newHeight;
			_scrollTopMargin = newTopMargin;
			_scrollBottomMargin = newBottomMargin;

			UpdateLayout();

			return true;
		}

		private void UpdateLayout() {

			Bounds = new(_x, _y, _width, _height);

			btnPageUp.bounds = new(
				_x + _width - 64,
				_y + _scrollTopMargin,
				64, 64
			);

			ScrollArea = new Rectangle(
				_x + _width - 50,
				_y + 64 + _scrollTopMargin,
				24,
				_height - 64 - 80 - _scrollTopMargin - _scrollBottomMargin
			);

			btnPageDown.bounds = new(
				_x + _width - 64,
				_y + _height - 64 - _scrollBottomMargin,
				64, 64
			);

			UpdateFlow();
		}

		private void UpdateFlow(bool recalculate = true) {
			if (! Flow.HasValue) {
				ScrollMax = 0;
				ScrollOffset = 0;
				ScrollBar.visible = btnPageUp.visible = btnPageDown.visible = IsScrollVisible;
				UpdateComponents();
				return;
			}

			CachedFlow flow = recalculate ? FlowHelper.CalculateFlow(
				Flow.Value,
				maxWidth: FlowWidth,
				defaultFont: _defaultFont
			) : Flow.Value;

			Flow = flow;

			ScrollMax = Math.Max(0, flow.Height - _height);

			float height = Math.Min(1, _height / flow.Height) * ScrollArea.Height;
			if (height < 24)
				height = 24;

			ScrollBar.bounds.Height = (int) height;

			/*ScrollMax = FlowHelper.GetMaximumScrollOffset(
				flow,
				_height,
				ScrollStep
			);*/

			ScrollBar.visible = btnPageUp.visible = btnPageDown.visible = IsScrollVisible;

			if (ScrollOffset < 0)
				ScrollOffset = 0;
			if (ScrollOffset > ScrollMax)
				ScrollOffset = ScrollMax;

			UpdateComponents();
		}

		private void UpdateComponents() {
			if (ScrollMax > 0) {
				ScrollBar.bounds.X = ScrollArea.X;

				int height = ScrollArea.Height - ScrollBar.bounds.Height;
				float progress = ScrollOffset / (float) Math.Max(1, ScrollMax);

				ScrollBar.bounds.Y = ScrollArea.Y + (int) Math.Floor(height * progress);
			}

			if (Flow.HasValue) {
				FlowHelper.UpdateComponentsForFlow(
					Flow.Value,
					DynamicComponents,
					_x,
					_y,
					scrollOffset: ScrollOffset,
					maxHeight: _height,
					onCreate: cmp => {
						cmp.region = Region;
						ParentMenu?.allClickableComponents?.Add(cmp);
					},
					onDestroy: cmp => {
						ParentMenu?.allClickableComponents?.Remove(cmp);
					},
					startID: lastID + 1
				);

			} else {
				if (ParentMenu?.allClickableComponents != null)
					foreach (var cmp in DynamicComponents)
						ParentMenu.allClickableComponents.Remove(cmp);

				DynamicComponents.Clear();
			}
		}

		#endregion

		#region Flow Management

		public void Set(IEnumerable<IFlowNode> nodes) {
			Set(nodes, 4, ScrollOffset);
		}

		public void Set(IEnumerable<IFlowNode> nodes, int step) {
			Set(nodes, step, ScrollOffset);
		}

		public void Set(IEnumerable<IFlowNode> nodes, float step) {
			Set(nodes, step, ScrollOffset);
		}

		public void Set(IEnumerable<IFlowNode> nodes, int step, float scroll) {
			Vector2 size = (DefaultFont ?? Game1.smallFont).MeasureString(@"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789 ");
			Set(nodes, size.Y * step, scroll);
		}

		public void Set(IEnumerable<IFlowNode> nodes, float step, float scroll) {
			if (nodes == null) {
				Flow = null;

				ScrollStep = step;
				ScrollOffset = 0;

				UpdateFlow(false);
				return;
			}

			Tuple<IFlowNode, float> closest = null;
			if (scroll == -1 && Flow.HasValue)
				closest = FlowHelper.GetClosestUniqueNode(Flow.Value, 1f, ScrollOffset, _height);

			Flow = FlowHelper.CalculateFlow(
				nodes,
				maxWidth: FlowWidth,
				defaultFont: _defaultFont
			);

			if (scroll == -1) {
				if (closest != null) {
					float pos = FlowHelper.GetScrollOffsetForUniqueNode(
						Flow.Value,
						closest.Item1.UniqueId
					);
					if (pos >= 0)
						scroll = pos - closest.Item2;
					else
						scroll = 0;
				} else
					scroll = 0;
			}

			ScrollStep = step;
			ScrollOffset = scroll;

			UpdateFlow(false);
		}

		#endregion

		#region Scrolling

		public bool Scroll(int steps) {
			return ScrollTo(ScrollOffset + (steps * ScrollStep));
		}

		public bool ScrollPage(int pages) {
			if (!Flow.HasValue)
				return false;

			float page = _height * .75f; // Flow.Value.Lines.Length - ScrollMax;
			return ScrollTo(ScrollOffset + (pages * page));
		}

		public bool ScrollToStart() {
			return ScrollTo(0);
		}

		public bool ScrollToEnd() {
			return ScrollTo(ScrollMax, false);
		}

		public bool ScrollTo(float offset, bool steplock = true) {
			if (!Flow.HasValue)
				return false;

			if (steplock)
				offset -= offset % ScrollStep;

			if (offset < 0)
				offset = 0;
			if (offset > ScrollMax)
				offset = ScrollMax;

			if (offset == ScrollOffset)
				return false;

			ScrollOffset = offset;
			UpdateComponents();
			return true;
		}

		public bool ScrollTo(IFlowNode node, float offset = 0) {
			if (!Flow.HasValue || node == null)
				return false;

			float height = 0;

			for(int i = 0; i < Flow.Value.Lines.Length; i++) {
				CachedFlowLine line = Flow.Value.Lines[i];
				foreach(IFlowNodeSlice slice in line.Slices) {
					if (slice.Node.Equals(node))
						return ScrollTo(height + offset);
				}

				height += line.Height;
			}

			return false;
		}

		#endregion

		#region Events

		public bool LeftClickHeld(int x, int y, bool playSound = true) {
			if (!Scrolling)
				return false;

			int half = ScrollBar.bounds.Height / 2;
			int minY = ScrollArea.Y + half;
			int height = ScrollArea.Height - ScrollBar.bounds.Height;

			float progress = (float) (y - ScrollingOffset - minY) / height;

			ScrollTo((float) Math.Floor(progress * ScrollMax), false);
			return true;
		}

		public void ReleaseLeftClick(int x, int y) {
			Scrolling = false;
		}

		public bool ReceiveLeftClick(int x, int y, bool playSound = true) {
			if (ScrollBar.visible && ScrollArea.Contains(x, y)) {

				if (ScrollBar.containsPoint(x, y)) {
					float middle = ScrollBar.bounds.Y + (ScrollBar.bounds.Height / 2f);
					ScrollingOffset = y - middle;

				} else {
					ScrollingOffset = 0;
				}

				//ScrollingOffset = 0; // y - ScrollBar.bounds.Y;
				Scrolling = true;
				return true;
			}

			if (btnPageUp.visible && btnPageUp.containsPoint(x, y)) {
				btnPageUp.scale = btnPageUp.baseScale;
				if (Scroll(-1) && playSound)
					Game1.playSound("shiny4");

				return true;
			}

			if (btnPageDown.visible && btnPageDown.containsPoint(x, y)) {
				btnPageDown.scale = btnPageDown.baseScale;
				if (Scroll(1) && playSound)
					Game1.playSound("shiny4");

				return true;
			}

			if (x < _x || x > (_width + _x) || y < _y || y > (_height + _y))
				return false;

			if (Flow.HasValue) {
				int fx = x - _x;
				int fy = y - _y;

				if (FlowHelper.ClickFlow(Flow.Value, fx, fy, scrollOffset: ScrollOffset, maxHeight: _height))
					return true;
			}

			return false;
		}

		public bool ReceiveRightClick(int x, int y, bool playSound = true) {
			if (x < _x || x > (_width + _x) || y < _y || y > (_height + _y))
				return false;

			if (Flow.HasValue) {
				int fx = x - _y;
				int fy = y - _y;

				if (FlowHelper.RightClickFlow(Flow.Value, fx, fy, scrollOffset: ScrollOffset, maxHeight: _height))
					return true;
			}

			return false;
		}


		public object GetExtraAt(int x, int y) {
			if (Flow.HasValue) {
				int fx = x - _x;
				int fy = y - _y;

				return FlowHelper.GetExtra(Flow.Value, fx, fy, scrollOffset: ScrollOffset, maxHeight: _height);
			}

			return null;
		}


		public bool PerformHover(int x, int y) {
			btnPageUp.tryHover(x, ScrollOffset > 0 ? y : -1);
			btnPageDown.tryHover(x, ScrollOffset < ScrollMax ? y : -1);

			if (x < _x || x > (_width + _x) || y < _y || y > (_height + _y))
				return false;

			if (Flow.HasValue) {
				int fx = x - _x;
				int fy = y - _y;

				FlowHelper.HoverFlow(
					Flow.Value,
					fx, fy,
					scrollOffset: ScrollOffset,
					maxHeight: _height
				);
			}

			return false;
		}

		public bool IsMiddleScrolling() {
			return MiddleScrolling;
		}

		public bool PerformMiddleScroll(int x, int y) {
			bool outside = x < _x || x > (_width + _x) || y < _y || y > (_height + _y);

			if (Game1.oldMouseState.MiddleButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed) {
				if (!MiddleScrolling && (outside || ScrollMax <= 0)) {
					/* nothing~ */
					return false;

				} else if (!MiddleScrolling) {
					MiddleScrolling = true;
					MiddleStart = y;
				} else {
					float offset = y - MiddleStart;
					ScrollTo(ScrollOffset + offset, false);
				}

				return true;

			} else if (MiddleScrolling)
				MiddleScrolling = false;

			return false;
		}

		#endregion

		#region Drawing

		public void DrawMiddleScroll(SpriteBatch batch) {
			if (MiddleScrolling) {
				batch.Draw(
					Game1.mouseCursors,
					new Vector2(Game1.getOldMouseX(), MiddleStart - 32),
					new Rectangle(257, 284, 16, 16),
					Color.White * 0.5f,
					(float) Math.PI * .25f,
					Vector2.Zero,
					3f,
					SpriteEffects.None,
					1f
				);
			}
		}

		public void Draw(SpriteBatch batch, Color? defaultColor = null, Color? defaultShadowColor = null) {

#if DEBUG
			if (Game1.oldKBState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.F3)) {
				batch.Draw(
					Game1.uncoloredMenuTexture,
					new Rectangle(_x, _y, _width, _height),
					new Rectangle(16, 272, 28, 28),
					Color.Pink * 0.5f
				);

			}
#endif

			if (IsScrollVisible) {
				if (ScrollOffset > 0)
					btnPageUp.draw(batch);
				else
					btnPageUp.draw(batch, Color.Black * 0.35f, 0.89f);

				if (ScrollOffset < ScrollMax)
					btnPageDown.draw(batch);
				else
					btnPageDown.draw(batch, Color.Black * 0.35f, 0.89f);

				RenderHelper.DrawBox(
					batch,
					texture: ScrollAreaTexture,
					sourceRect: ScrollAreaSource,
					x: ScrollArea.X,
					y: ScrollArea.Y,
					width: ScrollArea.Width,
					height: ScrollArea.Height,
					color: Color.White,
					scale: 4f,
					drawShadow: false
				);

				bool scrolling = Scrolling || MiddleScrolling;

				RenderHelper.DrawBox(
					batch,
					texture: ScrollBar.texture,
					sourceRect: ScrollBar.sourceRect,
					x: ScrollBar.bounds.X + (scrolling ? -2 : 0),
					y: ScrollBar.bounds.Y + (scrolling ? -2 : 0),
					width: ScrollArea.Width + (scrolling ? 4 : 0),
					height: ScrollBar.bounds.Height + (scrolling ? 4 : 0),
					color: Color.White,
					topSlice: 3,
					bottomSlice: 3,
					leftSlice: 2,
					rightSlice: 2,
					scale: 4f,
					drawShadow: false
				);
			}

			if (!Flow.HasValue)
				return;

			RenderHelper.WithScissor(batch, SpriteSortMode.Deferred, new Rectangle(_x, _y, _width, _height), () => {
				FlowHelper.RenderFlow(
					batch,
					Flow.Value,
					new Vector2(_x, _y),
					defaultColor,
					defaultShadowColor: defaultShadowColor,
					scrollOffset: ScrollOffset,
					maxHeight: _height
				);
			});

#if DEBUG
			if (Game1.oldKBState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.F3)) {

				int d_idx = 0;

				foreach (var cmp in DynamicComponents) {
					if (!cmp.visible)
						continue;

					batch.Draw(
						Game1.uncoloredMenuTexture,
						cmp.bounds,
						new Rectangle(16, 272, 28, 28),
						DEBUG_COLORS[d_idx] * 0.5f
					);

					d_idx = (d_idx + 1) % DEBUG_COLORS.Length;
				}

				batch.Draw(
					Game1.uncoloredMenuTexture,
					btnPageUp.bounds,
					new Rectangle(16, 272, 28, 28),
					Color.Red * 0.5f
				);

				batch.Draw(
					Game1.uncoloredMenuTexture,
					ScrollArea,
					new Rectangle(16, 272, 28, 28),
					Color.Orange * 0.5f
				);

				batch.Draw(
					Game1.uncoloredMenuTexture,
					btnPageDown.bounds,
					new Rectangle(16, 272, 28, 28),
					Color.Red * 0.5f
				);
			}
#endif
		}

		#endregion

	}
}
