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

using Leclair.Stardew.Common.UI;
using Leclair.Stardew.Common.UI.FlowNode;
using Leclair.Stardew.Common.UI.SimpleLayout;
using Leclair.Stardew.Common.Types;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

using Leclair.Stardew.Almanac.Pages;

namespace Leclair.Stardew.Almanac.Menus {
	public class AlmanacMenu : IClickableMenu {

		// Static Stuff

		public static readonly int REGION_LEFT_PAGE = 1;
		public static readonly int REGION_RIGHT_PAGE = 2;
		public static readonly int REGION_TABS = 3;

		public static readonly Rectangle MAGIC_BG = new Rectangle(288, 352, 15, 15);

		public static readonly Color MAGIC_SHADOW_COLOR = new Color(19, 16, 57);

		public static readonly Rectangle[] LEFT_BUTTON = new Rectangle[] {
			new(0, 256, 64, 64),
			new(208, 352, 16, 16)
		};

		public static readonly Rectangle[] RIGHT_BUTTON = new Rectangle[] {
			new(0, 192, 64, 64),
			new(224, 352, 16, 16)
		};

		public static readonly Rectangle[] UP_ARROW = new Rectangle[] {
			new(64, 64, 64, 64),
			new(176, 352, 16, 16)
		};

		public static readonly Rectangle[] DOWN_ARROW = new Rectangle[] {
			new(0, 64, 64, 64),
			new(192, 352, 16, 16)
		};

		public static readonly Rectangle[] SCROLL_BG = new Rectangle[] {
			new(403, 383, 6, 6),
			new(160, 352, 6, 6)
		};

		public static readonly Rectangle[] SCROLL_THUMB = new Rectangle[] {
			new(435, 463, 6, 10),
			new(160, 358, 6, 10)
		};

		public static readonly Rectangle COVER = new(0, 185, 160, 185);

		public static readonly Rectangle[] OPEN_PAGES = new Rectangle[] {
			new(0, 0, 320, 185),
			new(320, 0, 320, 185)
		};

		public static readonly Rectangle[] CALENDAR = new Rectangle[] {
			new(160, 192, 144, 160),
			new(304, 192, 144, 160)
		};

		public static readonly Rectangle[][] TABS = new Rectangle[][] {
			new Rectangle[] {
				new(448, 192, 16, 16),
				new(448, 208, 16, 16),
				new(448, 224, 16, 16),
				new(448, 240, 16, 16),
			},
			new Rectangle[] {
				new(464, 192, 16, 16),
				new(464, 208, 16, 16),
				new(464, 224, 16, 16),
				new(464, 240, 16, 16),
			},
		};

		// Rendering Stuff

		public readonly Texture2D background;

		// Buttons
		public ClickableTextureComponent btnPageUp;
		public ClickableTextureComponent btnPageDown;

		public ClickableTextureComponent btnPrevious;
		public ClickableTextureComponent btnNext;
		public List<ClickableComponent> calDays;
		public List<ClickableComponent> TabComponents = new();

		public List<ClickableComponent> FlowComponents = new();
		public List<ClickableComponent> PageComponents = new();

		// Current Date
		public WorldDate Date { get; protected set; }
		public int Year => Date.Year;
		public int Season => Date.SeasonIndex;

		// Pages
		private readonly IAlmanacPage[] Pages;

		private int PageIndex = -1;
		private IAlmanacPage CurrentPage => (Pages == null || PageIndex < 0 || PageIndex >= Pages.Length) ? null : Pages[PageIndex];

		private bool IsMagic => CurrentPage?.IsMagic ?? false;

		// Tabs
		private List<Tuple<ClickableComponent, int, int, int>> Tabs = new();

		// Flow Rendering
		private CachedFlow? Flow;
		private int FlowStep = 1;
		private int FlowOffset;
		private int MaxFlowOffset;

		public ClickableTextureComponent FlowScrollBar;
		public Rectangle FlowScrollArea;
		private bool FlowScrolling;

		// Tooltip
		public bool HoverMagic;
		public ISimpleNode HoverNode;
		public string HoverText;

		private Cache<ISimpleNode, string> CachedHoverText;

		// Lookup Anything support.
		public Item HoveredItem = null;

		public AlmanacMenu(int year)
		: base(0, 0, 0, 0, true) {
			background = ModEntry.instance.Helper.Content.Load<Texture2D>("assets/Menu.png");
			Date = new(Game1.Date);

			ModEntry mod = ModEntry.instance;

			// CachedHoverText
			CachedHoverText = new(
				str => string.IsNullOrEmpty(str) ? null : SimpleHelper.Builder().Text(str).GetLayout(),
				() => HoverText
			);

			List<IAlmanacPage> pages = new();
			foreach (var builder in mod.PageBuilders) {
				IAlmanacPage page = builder.Invoke(this, mod);
				if (page != null)
					pages.Add(page);
			}

			Pages = pages.ToArray();

			for (int i = 0; i < Pages.Length; i++) {
				if (Pages[i] is ITab tab && tab.TabVisible) {
					ClickableComponent cmp = new(
						new Rectangle(0, 0, 16, 16),
						(string) null
					) {
						myID = 10 + i,
						region = REGION_TABS,
						leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
						upNeighborID = ClickableComponent.SNAP_AUTOMATIC,
						downNeighborID = ClickableComponent.SNAP_AUTOMATIC,
						rightNeighborID = ClickableComponent.SNAP_AUTOMATIC
					};

					TabComponents.Add(cmp);
					Tabs.Add(new(cmp, i, tab.SortKey, Game1.random.Next(2 * TABS.Length)));
				}
			}

			Tabs.Sort((a, b) => a.Item3.CompareTo(b.Item3));

			FlowScrollBar = new ClickableTextureComponent(
				new Rectangle(0, 0, 44, 48),
				background,
				Rectangle.Empty,
				4f
			);

			btnPageUp = new ClickableTextureComponent(
				new Rectangle(0, 0, 64, 64),
				background,
				Rectangle.Empty,
				3.2f
			) {
				region = REGION_RIGHT_PAGE,
				myID = 1,
				upNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				downNeighborID = 2,
				rightNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				leftNeighborID = ClickableComponent.SNAP_AUTOMATIC
			};

			btnPageDown = new ClickableTextureComponent(
				new Rectangle(0, 0, 64, 64),
				background,
				Rectangle.Empty,
				3.2f
			) {
				region = REGION_RIGHT_PAGE,
				myID = 2,
				upNeighborID = 1,
				downNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				rightNeighborID = ClickableComponent.SNAP_AUTOMATIC
			};

			ChangePage(0);
			Game1.playSound("bigSelect");
		}

		public bool ChangeSeason(int delta) {
			int newSeason = Season + delta;
			if (newSeason < 0)
				newSeason = 0;
			if (newSeason >= WorldDate.MonthsPerYear)
				newSeason = WorldDate.MonthsPerYear - 1;

			if (Season == newSeason)
				return false;

			WorldDate oldDate = Date;
			Date = new(Year, WeatherHelper.GetSeasonName(newSeason), 1);

			foreach (IAlmanacPage page in Pages)
				page?.DateChanged(oldDate, Date);

			//CurrentPage?.DateChanged(oldDate, Date);
			return true;
		}


		public int CurrentTab {
			get {
				for (int i = 0; i < Tabs.Count; i++) {
					int page = Tabs[i].Item2;
					if (page == PageIndex)
						return i;
				}

				return -1;
			}
		}

		public bool PrevPage() {
			int tab = CurrentTab - 1;
			if (tab < 0)
				return false;

			return ChangePage(Tabs[tab].Item2);
		}

		public bool NextPage() {
			int tab = CurrentTab + 1;
			if (tab >= Tabs.Count)
				return false;

			return ChangePage(Tabs[tab].Item2);
		}

		public bool ChangePage(int index) {
			if (index >= Pages.Length)
				index = Pages.Length - 1;
			if (index < 0)
				index = 0;

			if (PageIndex == index)
				return false;

			CurrentPage?.Deactivate();
			PageComponents = null;
			SetFlow(null);
			PageIndex = index;

			// Apply the size of the new page type.
			height = 185 * 4;

			switch (CurrentPage?.Type) {
				case PageType.Cover:
					width = 160 * 4;
					break;
				case PageType.Blank:
				case PageType.Calendar:
				default:
					width = 320 * 4;
					break;
			}

			CurrentPage?.Activate();
			Recenter();

			// Update component textures, source rects, and scales.
			bool is_magic = IsMagic;

			if (btnPageDown != null) {
				btnPageDown.texture = is_magic ? background : Game1.mouseCursors;
				btnPageDown.scale = btnPageDown.baseScale = is_magic ? 3.2f : 0.8f;
				btnPageDown.sourceRect = DOWN_ARROW[is_magic ? 1 : 0];
			}

			if (btnPageUp != null) {
				btnPageUp.texture = is_magic ? background : Game1.mouseCursors;
				btnPageUp.scale = btnPageUp.baseScale = is_magic ? 3.2f : 0.8f;
				btnPageUp.sourceRect = UP_ARROW[is_magic ? 1 : 0];
			}

			if (btnPrevious != null) {
				btnPrevious.texture = is_magic ? background : Game1.mouseCursors;
				btnPrevious.scale = btnPrevious.baseScale = is_magic ? 3.2f : 0.8f;
				btnPrevious.sourceRect = LEFT_BUTTON[is_magic ? 1 : 0];
			}

			if (btnNext != null) {
				btnNext.texture = is_magic ? background : Game1.mouseCursors;
				btnNext.scale = btnNext.baseScale = is_magic ? 3.2f : 0.8f;
				btnNext.sourceRect = RIGHT_BUTTON[is_magic ? 1 : 0];
			}

			if (FlowScrollBar != null) {
				FlowScrollBar.texture = is_magic ? background : Game1.mouseCursors;
				FlowScrollBar.sourceRect = SCROLL_THUMB[is_magic ? 1 : 0];
			}

			return true;
		}

		public void Recenter() {
			Vector2 pos = Utility.getTopLeftPositionForCenteringOnScreen(
				width,
				height
			);

			xPositionOnScreen = (int) pos.X;
			yPositionOnScreen = (int) pos.Y;

			var view = Game1.uiViewport;
			if (view.Width >= 1280 && view.Width <= 1376) {
				xPositionOnScreen -= (1376 - view.Width) / 2;
			}

			int yOffset = 0;

			if (view.Height >= 720 && view.Height <= 752) {
				yOffset = (752 - view.Height) / 2;
				yPositionOnScreen -= yOffset;
			}

			if (upperRightCloseButton != null)
				upperRightCloseButton.bounds = new Rectangle(
					xPositionOnScreen + width - 40,
					yPositionOnScreen - 20 + yOffset * 3,
					upperRightCloseButton.bounds.Width,
					upperRightCloseButton.bounds.Height
				);

			btnPageUp.bounds = new Rectangle(
					xPositionOnScreen + width - 64 - 16,
					yPositionOnScreen + 60 + (IsMagic ? 32 : 0),
					64, 64
				);

			btnPageDown.bounds = new Rectangle(
					xPositionOnScreen + width - 64 - 16,
					yPositionOnScreen + height - 64 - 60 + (IsMagic ? 32 : 0),
					64,
					64
				);

			FlowScrollArea = new Rectangle(
				xPositionOnScreen + width - 66,
				yPositionOnScreen + 124 + (IsMagic ? 32 : 0),
				24,
				height - 256
			);

			UpdateTabs();
			UpdateCalendarComponents();
			CurrentPage?.UpdateComponents();
			PageComponents = CurrentPage?.GetComponents();
			UpdateFlowComponents(false);
			populateClickableComponentList();
		}



		public void UpdateTabs() {
			int offsetY = 60;

			var view = Game1.uiViewport;
			if (view.Height >= 720 && view.Height <= 752) {
				offsetY += 3 * (752 - view.Height) / 2;
			}

			foreach (var entry in Tabs) {
				ClickableComponent cmp = entry.Item1;
				if (Pages[entry.Item2] is not ITab tab || cmp == null)
					continue;

				cmp.bounds = new Rectangle(
					xPositionOnScreen + width - 8,
					yPositionOnScreen + offsetY,
					64, 64
				);

				offsetY += 68;
			}
		}

		public void UpdateCalendarComponents() {
			if (CurrentPage?.Type != PageType.Calendar) {
				calDays = null;
				btnNext = null;
				btnPrevious = null;
				return;
			}

			if (btnPrevious == null)
				btnPrevious = new ClickableTextureComponent(
					Rectangle.Empty,
					IsMagic ? background : Game1.mouseCursors,
					LEFT_BUTTON[IsMagic ? 1 : 0],
					IsMagic ? 3.2f : 0.8f
				) {
					region = REGION_LEFT_PAGE,
					myID = 88,
					rightNeighborID = 89,
					upNeighborID = ClickableComponent.SNAP_AUTOMATIC,
					leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
					downNeighborID = ClickableComponent.SNAP_AUTOMATIC
				};

			if (btnNext == null)
				btnNext = new ClickableTextureComponent(
					Rectangle.Empty,
					IsMagic ? background : Game1.mouseCursors,
					RIGHT_BUTTON[IsMagic ? 1 : 0],
					IsMagic ? 3.2f : 0.8f
				) {
					region = REGION_LEFT_PAGE,
					myID = 89,
					leftNeighborID = 88,
					upNeighborID = ClickableComponent.SNAP_AUTOMATIC,
					rightNeighborID = ClickableComponent.SNAP_AUTOMATIC,
					downNeighborID = ClickableComponent.SNAP_AUTOMATIC
				};

			if (calDays == null) {
				calDays = new();
				for (int day = 1; day <= WorldDate.DaysPerMonth; day++)
					calDays.Add(new ClickableComponent(
						Rectangle.Empty, Convert.ToString(day)
					) {
						region = REGION_LEFT_PAGE,
						myID = 100 + day,
						leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
						rightNeighborID = ClickableComponent.SNAP_AUTOMATIC,
						upNeighborID = ClickableComponent.SNAP_AUTOMATIC,
						downNeighborID = ClickableComponent.SNAP_AUTOMATIC
					});
			}

			int col = 0;
			int row = 0;

			for (int day = 1; day <= WorldDate.DaysPerMonth; day++) {
				calDays[day - 1].bounds = new Rectangle(
						xPositionOnScreen + 64 + 76 * col,
						yPositionOnScreen + 120 + 52 + 128 * row,
						72, 124
					);

				col++;
				if (col >= 7) {
					col = 0;
					row++;
				}
			}

			btnPrevious.bounds = new Rectangle(
				xPositionOnScreen + 60,
				yPositionOnScreen + 34,
				64, 64
			);

			btnNext.bounds = new Rectangle(
				xPositionOnScreen + (width / 2) - 44 - 48,
				yPositionOnScreen + 34,
				64, 64
			);
		}

		public void SetFlow(IEnumerable<IFlowNode> nodes) {
			SetFlow(nodes, 1);
		}

		public void SetFlow(IEnumerable<IFlowNode> nodes, int step) {
			FlowStep = step;

			if (nodes == null) {
				Flow = null;
				FlowOffset = MaxFlowOffset = 0;
				FlowScrollBar.visible = false;
				btnPageDown.visible = false;
				btnPageUp.visible = false;
				UpdateFlowComponents();
				return;
			}

			CachedFlow flow = FlowHelper.CalculateFlow(
				nodes,
				width / 2 - 80 - 64
			);
			Flow = flow;

			// Starting at the end of the flow, determine how many
			// lines we have to skip to view the end.

			float remaining = height - 64;
			int oldMax = MaxFlowOffset;
			MaxFlowOffset = 0;

			for (int i = flow.Lines.Length - 1; i >= 0; i--) {
				remaining -= flow.Lines[i].Height;
				if (remaining <= 0) {
					MaxFlowOffset = i + 1;
					MaxFlowOffset += MaxFlowOffset % FlowStep;
					break;
				}
			}

			btnPageDown.visible = MaxFlowOffset > 0;
			btnPageUp.visible = MaxFlowOffset > 0;
			FlowScrollBar.visible = MaxFlowOffset > 0;

			if (oldMax == 0)
				FlowOffset = 0;
			else {
				if (FlowOffset < 0)
					FlowOffset = 0;
				if (FlowOffset > MaxFlowOffset)
					FlowOffset = MaxFlowOffset;
			}

			UpdateFlowComponents();
		}

		public void UpdateFlowComponents(bool callbacks = true) {

			if (MaxFlowOffset > 0) {
				FlowScrollBar.bounds.X = FlowScrollArea.X;

				int height = FlowScrollArea.Height - FlowScrollBar.bounds.Height + 8;
				float progress = FlowOffset / (float) Math.Max(1, MaxFlowOffset);

				FlowScrollBar.bounds.Y = FlowScrollArea.Y + (int) Math.Floor(height * progress);
			}

			if (Flow.HasValue)
				FlowHelper.UpdateComponentsForFlow(
					Flow.Value,
					FlowComponents,
					xPositionOnScreen + (width / 2) + 40,
					yPositionOnScreen + 40,
					lineOffset: FlowOffset,
					maxHeight: height - 120,
					onCreate: callbacks ? cmp => {
						cmp.region = REGION_RIGHT_PAGE;
						allClickableComponents.Add(cmp);
					}
				: null,
					onDestroy: callbacks ? cmp => allClickableComponents.Remove(cmp) : null
				);
			else {
				foreach (ClickableComponent cmp in FlowComponents)
					allClickableComponents.Remove(cmp);
				FlowComponents.Clear();
			}
		}

		#region Events

		protected override bool _ShouldAutoSnapPrioritizeAlignedElements() {
			return true;
		}


		public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds) {
			base.gameWindowSizeChanged(oldBounds, newBounds);
			Recenter();
		}

		public override void receiveGamePadButton(Buttons b) {
			base.receiveGamePadButton(b);

			if (CurrentPage?.ReceiveGamePadButton(b) ?? false)
				return;

			if (b.Equals(Buttons.LeftTrigger)) {
				if (PrevPage())
					Game1.playSound("smallSelect");
			} else if (b.Equals(Buttons.RightTrigger)) {
				if (NextPage())
					Game1.playSound("smallSelect");
			} else if (b.Equals(Buttons.LeftShoulder)) {
				if (ChangeSeason(-1))
					Game1.playSound("shwip");
			} else if (b.Equals(Buttons.RightShoulder)) {
				if (ChangeSeason(1))
					Game1.playSound("shwip");
			}
		}

		public bool ScrollFlow(int direction) {
			int old = FlowOffset;
			FlowOffset += direction < 0 ? -FlowStep : FlowStep;
			if (FlowOffset < 0)
				FlowOffset = 0;
			if (FlowOffset > MaxFlowOffset)
				FlowOffset = MaxFlowOffset;

			if (old != FlowOffset)
				UpdateFlowComponents();

			return old != FlowOffset;
		}

		public bool ScrollFlow(IFlowNode node) {
			if (!Flow.HasValue || node == null)
				return false;

			int old = FlowOffset;
			bool found = false;

			// Find the node in the flow.
			for (int i = 0; i < Flow.Value.Lines.Length; i++) {
				CachedFlowLine line = Flow.Value.Lines[i];
				foreach (IFlowNodeSlice slice in line.Slices) {
					if (slice.Node.Equals(node)) {
						FlowOffset = i;
						found = true;
						break;
					}
				}
				if (found)
					break;
			}

			FlowOffset -= FlowOffset % FlowStep;
			if (FlowOffset < 0)
				FlowOffset = 0;
			if (FlowOffset > MaxFlowOffset)
				FlowOffset = MaxFlowOffset;

			if (old != FlowOffset) {
				UpdateFlowComponents();
				return true;
			}

			return false;
		}

		public override void receiveScrollWheelAction(int direction) {
			base.receiveScrollWheelAction(direction);

			int x = Game1.getOldMouseX();
			int y = Game1.getOldMouseY();

			if (CurrentPage?.ReceiveScroll(x, y, direction) ?? false)
				return;

			bool left = (x - xPositionOnScreen) < (width / 2);

			if (CurrentPage?.Type == PageType.Calendar && left && !Game1.options.gamepadControls) {
				if (ChangeSeason(direction > 0 ? -1 : 1))
					Game1.playSound("shwip");

				return;
			}

			if (Flow.HasValue) {
				if (ScrollFlow(direction > 0 ? -1 : 1))
					Game1.playSound("shiny4");

				return;
			}
		}

		public override void receiveKeyPress(Keys key) {
			base.receiveKeyPress(key);

			if (CurrentPage?.ReceiveKeyPress(key) ?? false)
				return;
		}

		public override void leftClickHeld(int x, int y) {
			base.leftClickHeld(x, y);

			if (FlowScrolling) {
				int oldY = FlowScrollBar.bounds.Y;
				int half = FlowScrollBar.bounds.Height / 2;

				int minY = FlowScrollArea.Y + half;
				int height = FlowScrollArea.Height - FlowScrollBar.bounds.Height;
				int maxY = minY + height;

				float progress = (float) (y - minY) / height;

				int OldOffset = FlowOffset;
				FlowOffset = (int) Math.Floor(progress * MaxFlowOffset);
				if (FlowOffset < 0)
					FlowOffset = 0;
				if (FlowOffset > MaxFlowOffset)
					FlowOffset = MaxFlowOffset;

				if (OldOffset == FlowOffset)
					return;

				UpdateFlowComponents();
				Game1.playSound("shiny4");
			}
		}

		public override void releaseLeftClick(int x, int y) {
			base.releaseLeftClick(x, y);
			FlowScrolling = false;
		}

		public override void receiveLeftClick(int x, int y, bool playSound = true) {
			base.receiveLeftClick(x, y, playSound);

			if (CurrentPage?.ReceiveLeftClick(x, y, playSound) ?? false)
				return;

			if (calDays != null && CurrentPage is ICalendarPage page) {
				for (int day = 1; day <= WorldDate.DaysPerMonth; day++) {
					ClickableComponent cmp = calDays[day - 1];
					if (cmp.containsPoint(x, y)) {
						WorldDate date = new(Date);
						date.DayOfMonth = day;

						int cx = x - cmp.bounds.X;
						int cy = y - cmp.bounds.Y;

						if (page.ReceiveCellLeftClick(cx, cy, date, cmp.bounds))
							return;

						break;
					}
				}
			}

			if (FlowScrollBar.visible && FlowScrollArea.Contains(x, y)) {// FlowScrollBar.containsPoint(x, y)) {
				FlowScrolling = true;
				return;
			}

			if (Flow.HasValue) {
				int fx = x - (xPositionOnScreen + width / 2 + 40);
				int fy = y - (yPositionOnScreen + 40);

				if (FlowHelper.ClickFlow(Flow.Value, fx, fy, lineOffset: FlowOffset, maxHeight: height - 120))
					return;
			}

			if ((btnPageUp?.containsPoint(x, y) ?? false)) {
				btnPageUp.scale = btnPageUp.baseScale;
				if (ScrollFlow(-1) && playSound)
					Game1.playSound("shiny4");

				return;
			}

			if ((btnPageDown?.containsPoint(x, y) ?? false)) {
				btnPageDown.scale = btnPageDown.baseScale;
				if (ScrollFlow(1) && playSound)
					Game1.playSound("shiny4");

				return;
			}

			// Tabs
			foreach (var entry in Tabs) {
				if (entry.Item1.containsPoint(x, y)) {
					if (ChangePage(entry.Item2) && playSound)
						Game1.playSound("smallSelect");
					return;
				}
			}

			// Time Navigation
			if ((btnPrevious?.containsPoint(x, y) ?? false)) {
				btnPrevious.scale = btnPrevious.baseScale;
				if (ChangeSeason(-1) && playSound)
					Game1.playSound("smallSelect");
				return;
			}

			if ((btnNext?.containsPoint(x, y) ?? false)) {
				btnNext.scale = btnNext.baseScale;
				if (ChangeSeason(1) && playSound)
					Game1.playSound("smallSelect");
				return;
			}
		}

		public override void receiveRightClick(int x, int y, bool playSound = true) {
			base.receiveRightClick(x, y, playSound);

			if (CurrentPage?.ReceiveRightClick(x, y, playSound) ?? false)
				return;

			if (calDays != null && CurrentPage is ICalendarPage page) {
				for (int day = 1; day <= WorldDate.DaysPerMonth; day++) {
					ClickableComponent cmp = calDays[day - 1];
					if (cmp.containsPoint(x, y)) {
						WorldDate date = new(Date);
						date.DayOfMonth = day;

						int cx = x - cmp.bounds.X;
						int cy = y - cmp.bounds.Y;

						if (page.ReceiveCellRightClick(cx, cy, date, cmp.bounds))
							return;

						break;
					}
				}
			}

		}

		public override void performHoverAction(int x, int y) {
			base.performHoverAction(x, y);

			HoveredItem = null;
			HoverMagic = false;
			HoverNode = null;
			HoverText = null;

			btnPrevious?.tryHover(x, Season > 0 ? y : -1);
			btnNext?.tryHover(x, Season < WorldDate.MonthsPerYear - 1 ? y : -1);

			btnPageDown?.tryHover(x, FlowOffset < MaxFlowOffset ? y : -1);
			btnPageUp?.tryHover(x, FlowOffset > 0 ? y : -1);

			foreach (var entry in Tabs) {
				if (entry.Item1.containsPoint(x, y) && Pages[entry.Item2] is ITab tab) {
					HoverNode = tab.TabAdvancedTooltip;
					HoverText = tab.TabSimpleTooltip;
					HoverMagic = tab.TabMagic;
					break;
				}
			}

			if (calDays != null && CurrentPage is ICalendarPage page) {
				for (int day = 1; day <= WorldDate.DaysPerMonth; day++) {
					ClickableComponent cmp = calDays[day - 1];
					if (cmp.containsPoint(x, y)) {
						WorldDate date = new(Date);
						date.DayOfMonth = day;

						page.PerformCellHover(x - cmp.bounds.X, y - cmp.bounds.Y, date, cmp.bounds);

						//HoverText = page.GetCellHoverText(date);
						//HoverNode = page.GetCellHoverNode(date);

						break;
					}
				}
			}

			CurrentPage?.PerformHover(x, y);

			if (Flow.HasValue) {
				int fx = x - (xPositionOnScreen + width / 2 + 40);
				int fy = y - (yPositionOnScreen + 40);

				FlowHelper.HoverFlow(Flow.Value, fx, fy, lineOffset: FlowOffset, maxHeight: height - 120);
			}

		}

		#endregion

		#region Drawing

		public override void draw(SpriteBatch b) {

			WorldDate date = new(Date);
			int today = Game1.Date.TotalDays;

			// Background

			b.Draw(
				Game1.fadeToBlackRect,
				Game1.graphics.GraphicsDevice.Viewport.Bounds,
				Color.Black * 0.75f
			);

			b.Draw(
				background,
				new Vector2(xPositionOnScreen, yPositionOnScreen),
				CurrentPage?.Type == PageType.Cover ? COVER : OPEN_PAGES[IsMagic ? 1 : 0],
				Color.White,
				0f,
				Vector2.Zero,
				4f,
				SpriteEffects.None,
				1f
			);

			// Page Draw.
			CurrentPage?.Draw(b);

			// Calendar Draw

			if (CurrentPage?.Type == PageType.Calendar) {
				// The Grid
				b.Draw(
					background,
					new Vector2(xPositionOnScreen + 40, yPositionOnScreen + 72),
					CALENDAR[IsMagic ? 1 : 0],
					Color.White,
					0f,
					Vector2.Zero,
					4f,
					SpriteEffects.None,
					1f
				);

				// Title
				SpriteText.drawStringHorizontallyCenteredAt(
					b,
					I18n.Calendar_When(
						season: Utility.getSeasonNameFromNumber(date.SeasonIndex),
						year: date.Year
					),
					xPositionOnScreen + (width / 4),
					yPositionOnScreen + 40,
					color: IsMagic ? 4 : -1
				);

				// Navigation
				if (Season > 0)
					btnPrevious?.draw(b);
				else
					btnPrevious?.draw(b, Color.Black * 0.35f, 0.89f);

				if (Season < WorldDate.MonthsPerYear - 1)
					btnNext?.draw(b);
				else
					btnNext?.draw(b, Color.Black * 0.35f, 0.89f);

				// Headers
				for (int day = 1; day <= 7; day++) {
					int x = xPositionOnScreen + 64 + 76 * (day - 1);
					int y = yPositionOnScreen + 116;
					string text = ModEntry.instance.Helper.Translation.Get($"calendar.day.{day}");

					Vector2 size = Game1.dialogueFont.MeasureString(text);

					b.DrawString(
						Game1.dialogueFont,
						text,
						new Vector2(
							x + (76 - size.X) / 2,
							y
						),
						color: IsMagic ? Color.SkyBlue : Game1.textColor
					);
				}

				// Days
				ICalendarPage page = CurrentPage as ICalendarPage;

				for (int day = 1; day <= WorldDate.DaysPerMonth; day++) {
					date.DayOfMonth = day;
					int ddays = date.TotalDays;

					ClickableComponent cmp = calDays[day - 1];
					if (cmp == null)
						continue;

					if ((page?.ShouldHighlightToday ?? false) && ddays == today) {
						int num = 4 + (int) (2.0 * Game1.dialogueButtonScale / 8.0);
						IClickableMenu.drawTextureBox(
							b,
							Game1.mouseCursors,
							new Rectangle(379, 357, 3, 3),
							cmp.bounds.X - num,
							cmp.bounds.Y - num,
							cmp.bounds.Width + num * 2,
							cmp.bounds.Height + num * 2,
							Color.Blue,
							4f, false
						);
					}

					page?.DrawUnderCell(b, date, cmp.bounds);

					b.DrawString(
						Game1.smallFont,
						Convert.ToString(day),
						new Vector2(cmp.bounds.X, cmp.bounds.Y - 4),
						color: IsMagic ? Color.LightSkyBlue : Game1.textColor
					);

					page?.DrawOverCell(b, date, cmp.bounds);

					if ((page?.ShouldDimPastCells ?? false) && ddays < today)
						b.Draw(Game1.staminaRect, cmp.bounds, (IsMagic ? Color.Black : Color.Gray) * 0.25f);
				}
			}

			for (int i = 0; i < Tabs.Count; i++) {
				ClickableComponent cmp = Tabs[i].Item1;
				int page = Tabs[i].Item2;
				if (cmp != null && Pages[page] is ITab tab) {
					int x = cmp.bounds.X - (page == PageIndex ? 16 : 0);

					bool reflect = false;
					int tsprite = Tabs[i].Item4;
					if (tsprite >= TABS.Length) {
						tsprite -= TABS.Length;
						reflect = true;
					}

					// Tab Background
					b.Draw(
						background,
						new Vector2(x, cmp.bounds.Y),
						TABS[tab.TabMagic ? 1 : 0][tsprite],
						Color.White,
						0f,
						Vector2.Zero,
						4f,
						reflect ? SpriteEffects.FlipVertically : SpriteEffects.None,
						0.0001f
					);

					Texture2D tex = tab.TabTexture;
					if (tex != null) {
						Rectangle source = tab.TabSource ?? tex.Bounds;
						float scale = tab.TabScale ?? 4f;

						float height = source.Height * scale;
						float width = source.Width * scale;

						b.Draw(
							tex,
							new Vector2(
								x + (cmp.bounds.Width - width) / 2,
								cmp.bounds.Y + (cmp.bounds.Height - height) / 2
							),
							source,
							Color.White,
							0f,
							Vector2.Zero,
							scale,
							SpriteEffects.None,
							1f
						);
					}
				}
			}

			// Flow Draw

			if (Flow.HasValue) {
				if (MaxFlowOffset > 0) {
					/*btnPageDown.texture = IsMagic ? background : Game1.mouseCursors;
					btnPageUp.texture = IsMagic ? background : Game1.mouseCursors;
					btnPageDown.sourceRect = DOWN_ARROW[IsMagic ? 1 : 0];
					btnPageUp.sourceRect = UP_ARROW[IsMagic ? 1 : 0];

					btnPageDown.baseScale = IsMagic ? 3.2f : .8f;
					btnPageUp.baseScale = IsMagic ? 3.2f : .8f;

					FlowScrollBar.texture = IsMagic ? background : Game1.mouseCursors;
					FlowScrollBar.sourceRect = SCROLL_THUMB[IsMagic ? 1 : 0];*/

					// Scroll Buttons
					if (FlowOffset > 0)
						btnPageUp.draw(b);
					else
						btnPageUp.draw(b, Color.Black * 0.35f, 0.89f);

					if (FlowOffset < MaxFlowOffset)
						btnPageDown.draw(b);
					else
						btnPageDown.draw(b, Color.Black * 0.35f, 0.89f);

					// Scroll Bar
					IClickableMenu.drawTextureBox(
						b,
						IsMagic ? background : Game1.mouseCursors,
						SCROLL_BG[IsMagic ? 1 : 0],
						FlowScrollArea.X,
						FlowScrollArea.Y,
						FlowScrollArea.Width,
						FlowScrollArea.Height,
						Color.White,
						4f,
						false
					);

					FlowScrollBar.draw(b);
				}

				FlowHelper.RenderFlow(
					b,
					Flow.Value,
					new Vector2(xPositionOnScreen + width / 2 + 40, yPositionOnScreen + 40),
					IsMagic ? Color.White : Game1.textColor,
					defaultShadowColor: IsMagic ? MAGIC_SHADOW_COLOR : null,
					lineOffset: FlowOffset,
					maxHeight: height - 120
				);
			}

			base.draw(b);

			// Mouse
			Game1.mouseCursorTransparency = 1f;
			drawMouse(b);

			// Tooltip
			if (HoverNode != null || ! string.IsNullOrEmpty(HoverText))
				SimpleHelper.DrawHover(
					HoverNode ?? CachedHoverText.Value,
					b,
					Game1.smallFont,
					bgTexture: HoverMagic ? background : null,
					bgSource: HoverMagic ? MAGIC_BG : null,
					bgScale: HoverMagic? 4f : 1f,
					defaultColor: HoverMagic ? Color.White : Game1.textColor,
					defaultShadowColor: HoverMagic ? MAGIC_SHADOW_COLOR : null
				);
		}

		#endregion

	}
}
