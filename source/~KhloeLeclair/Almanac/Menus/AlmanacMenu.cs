/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#nullable enable

using System;
using System.Collections.Generic;

using Leclair.Stardew.Common;
using Leclair.Stardew.Common.UI;
using Leclair.Stardew.Common.UI.FlowNode;
using Leclair.Stardew.Common.UI.SimpleLayout;
using Leclair.Stardew.Common.Types;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using StardewValley;
using StardewValley.Menus;

using StardewModdingAPI.Utilities;

using Leclair.Stardew.Almanac.Pages;

namespace Leclair.Stardew.Almanac.Menus;

public class MenuState {
	public string? LastPage;
	public int TabScroll;
	public Dictionary<string, object>? States;
}

public class AlmanacMenu : IClickableMenu {

	public static readonly PerScreen<MenuState> PreviousState = new();

	// Static Stuff

	public static readonly int MAX_TABS = 9;
	public static readonly int VISIBLE_TABS = MAX_TABS - 2;

	public static readonly int REGION_LEFT_PAGE = 1;
	public static readonly int REGION_RIGHT_PAGE = 2;
	public static readonly int REGION_TABS = 3;

	public static readonly Rectangle MAGIC_BG = new(288, 352, 15, 15);
	public static readonly Rectangle DAY_BORDER = new(379, 357, 3, 3);

	public static readonly Color MAGIC_SHADOW_COLOR = new(19, 16, 57);

	public static readonly Rectangle COVER = Sprites.COVER_PAGE;

	public static readonly Rectangle[] OPEN_PAGES = new Rectangle[] {
		Sprites.OPEN_PAGE,
		Sprites.MAGIC_PAGE
	};

	public static readonly Rectangle[] CALENDAR = new Rectangle[] {
		Sprites.CALENDAR,
		Sprites.MAGIC_CALENDAR
	};

	public static readonly Rectangle[][] TABS = new Rectangle[][] {
		Sprites.TABS,
		Sprites.MAGIC_TABS
	};

	// Rendering Stuff

	public Texture2D background;
	public readonly ModEntry Mod;

	// Buttons
	public ClickableTextureComponent? btnPrevious;
	public ClickableTextureComponent? btnNext;
	public List<ClickableComponent>? calDays;
	public List<ClickableComponent> TabComponents = new();

	public List<ClickableComponent>? PageComponents = new();

	// Current Date
	public WorldDate Date { get; protected set; }
	public int Year => Date.Year;
	public int Season => Date.SeasonIndex;
	public int Day => Date.DayOfMonth;

	// Pages
	private readonly IAlmanacPage[] Pages;

	private int PageIndex = -1;
	public IAlmanacPage? CurrentPage => (Pages == null || PageIndex < 0 || PageIndex >= Pages.Length) ? null : Pages[PageIndex];

	public bool IsMagic => CurrentPage?.IsMagic ?? false;
	public Models.Style Style => IsMagic ? Mod.Theme.Magic : Mod.Theme.Standard;

	// Tabs
	public ClickableTextureComponent? btnTabsUp;
	public ClickableTextureComponent? btnTabsDown;
	private int TabScroll = 0;

	private readonly List<Tuple<ClickableComponent, int, int, int>> Tabs = new();

	// Flow Rendering
	private readonly ScrollableFlow LeftFlow;
	private readonly ScrollableFlow RightFlow;

	// Proxy Flow Elements
	public ClickableTextureComponent btnLeftPageUp;
	public ClickableTextureComponent btnLeftPageDown;

	public ClickableTextureComponent btnRightPageUp;
	public ClickableTextureComponent btnRightPageDown;

	public List<ClickableComponent> LeftFlowComponents;
	public List<ClickableComponent> RightFlowComponents;

	// Tooltip
	public bool HoverMagic;
	public ISimpleNode? HoverNode;
	public string? HoverText;

	private readonly Cache<ISimpleNode?, string?> CachedHoverText;

	// Lookup Anything support.
	public Item? HoveredItem = null;

	public AlmanacMenu(ModEntry mod, int year)
	: base(0, 0, 0, 0, true) {
		Mod = mod;
		background = Mod.ThemeManager.Load<Texture2D>("Menu.png");

		if (year == Game1.Date.Year)
			Date = new(Game1.Date);
		else
			Date = new(year, "spring", 1);

		Date.DayOfMonth = 1;

		// CachedHoverText
		CachedHoverText = new(
			str => string.IsNullOrEmpty(str) ? null : SimpleHelper.Builder().FormatText(str).GetLayout(),
			() => HoverText
		);

		List<IAlmanacPage> pages = new();
		foreach (var builder in mod.PageBuilders) {
			IAlmanacPage? page = builder.Invoke(this, mod);
			if (page != null)
				pages.Add(page);
		}

		Pages = pages.ToArray();

		for (int i = 0; i < Pages.Length; i++) {
			if (Pages[i] is ITab tab && tab.TabVisible) {
				ClickableComponent cmp = new(
					new Rectangle(0, 0, 16, 16),
					(string?) null
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

		if (Tabs.Count > MAX_TABS) {
			btnTabsUp = new ClickableTextureComponent(
				new Rectangle(0, 0, 64, 64),
				Game1.mouseCursors,
				Sprites.UP_ARROW,
				0.8f
			) {
				region = REGION_TABS,
				myID = 9,
				upNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				downNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				rightNeighborID = ClickableComponent.SNAP_AUTOMATIC
			};

			btnTabsDown = new ClickableTextureComponent(
				new Rectangle(0, 0, 64, 64),
				Game1.mouseCursors,
				Sprites.DOWN_ARROW,
				0.8f
			) {
				region = REGION_TABS,
				myID = 11 + Pages.Length,
				upNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				downNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				rightNeighborID = ClickableComponent.SNAP_AUTOMATIC
			};
		}

		Tabs.Sort((a, b) => a.Item3.CompareTo(b.Item3));

		// Flow
		LeftFlow = new(
			this,
			xPositionOnScreen, yPositionOnScreen,
			400, 400,
			region: REGION_LEFT_PAGE,
			firstID: 90000
		);
		
		btnLeftPageUp = LeftFlow.btnPageUp;
		btnLeftPageDown = LeftFlow.btnPageDown;
		LeftFlowComponents = LeftFlow.DynamicComponents;

		RightFlow = new(
			this,
			xPositionOnScreen, yPositionOnScreen,
			400, 400,
			region: REGION_RIGHT_PAGE,
			firstID: 100000
		);

		btnRightPageUp = RightFlow.btnPageUp;
		btnRightPageDown = RightFlow.btnPageDown;
		RightFlowComponents = RightFlow.DynamicComponents;

		MenuState state = PreviousState.Value;
		if (state != null && mod.Config.RestoreAlmanacState) {
			TabScroll = PreviousState.Value.TabScroll;

			int idx = 0;
			for(int i = 0; i < Pages.Length; i++) {
				var page = Pages[i];
				string id = page.Id;
				if (id == state.LastPage)
					idx = i;

				if (state.States != null && state.States.TryGetValue(id, out object? pstate))
					page.LoadState(pstate);	
			}

			ChangePage(idx);
		} else
			ChangePage(0);

		Game1.playSound("bigSelect");
	}

	protected override void cleanupBeforeExit() {
		base.cleanupBeforeExit();

		MenuState state = new() {
			LastPage = CurrentPage?.Id,
			TabScroll = TabScroll,
			States = new()
		};

		foreach (var page in Pages) {
			object pstate = page.GetState();
			if (pstate != null)
				state.States[page.Id] = pstate;
		}

		PreviousState.Value = state;
	}

	public bool ChangeSeason(int delta) {
		bool change_season = true;

		int newSeason = Season;

		// Do we have longer months?
		int newDay = Day + (delta * 28);
		if (newDay < 1 || newDay > ModEntry.DaysPerMonth)
			newDay = Day;
		else if (newDay != Day)
			change_season = false;

		if (change_season) {
			newSeason = Season + delta;
			if (newSeason < 0)
				newSeason = 0;
			if (newSeason >= WorldDate.MonthsPerYear)
				newSeason = WorldDate.MonthsPerYear - 1;

			if (Season != newSeason)
				newDay = 1;
		}

		if (Season == newSeason && Day == newDay)
			return false;

		WorldDate oldDate = Date;
		Date = new(Year, WeatherHelper.GetSeasonName(newSeason), newDay);

		foreach (IAlmanacPage page in Pages)
			page?.DateChanged(oldDate, Date);

		//CurrentPage?.DateChanged(oldDate, Date);
		return true;
	}


	public void RefreshPages() {
		foreach (IAlmanacPage page in Pages)
			page.Refresh();
	}

	public void RefreshTheme() {
		background = Mod.ThemeManager.Load<Texture2D>("Menu.png");
		Recenter();
		UpdateScrollComponents();

		foreach (IAlmanacPage page in Pages)
			page.ThemeChanged();
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

	public bool ScrollTabs(int direction) {
		if (Tabs.Count <= MAX_TABS || direction == 0)
			return false;

		int old = TabScroll;
		TabScroll += (direction > 0) ? 1 : -1;
		if (TabScroll < 0)
			TabScroll = 0;
		if (TabScroll > (Tabs.Count - VISIBLE_TABS))
			TabScroll = Tabs.Count - VISIBLE_TABS;

		if (TabScroll == old)
			return false;

		UpdateTabs();
		return true;
	}

	public bool CenterTab(int index) {
		if (Tabs.Count <= MAX_TABS)
			return false;

		int old = TabScroll;
		TabScroll = index - (VISIBLE_TABS / 2);
		if (TabScroll < 0)
			TabScroll = 0;
		if (TabScroll > (Tabs.Count - VISIBLE_TABS))
			TabScroll = Tabs.Count - VISIBLE_TABS;

		if (TabScroll == old)
			return false;

		UpdateTabs();
		return true;
	}

	public bool PrevPage(bool wrap = false) {
		int tab = CurrentTab - 1;
		if (tab < 0) {
			if (wrap)
				tab = Tabs.Count - 1;
			else
				return false;
		}

		return ChangePage(Tabs[tab].Item2, true);
	}

	public bool NextPage(bool wrap = false) {
		int tab = CurrentTab + 1;
		if (tab >= Tabs.Count) {
			if (wrap)
				tab = 0;
			else
				return false;
		}

		return ChangePage(Tabs[tab].Item2, true);
	}

	public bool ChangePage(int index, bool center_tab = false) {
		if (index >= Pages.Length)
			index = Pages.Length - 1;
		if (index < 0)
			index = 0;

		if (PageIndex == index)
			return false;

		CurrentPage?.Deactivate();
		PageComponents = null;
		SetLeftFlow(null);
		SetRightFlow(null);
		PageIndex = index;

		if (center_tab)
			CenterTab(CurrentTab);

		// Apply the size of the new page type.
		height = 185 * 4;

		switch (CurrentPage?.Type) {
			case PageType.Cover:
				width = 160 * 4;
				break;
			case PageType.Blank:
			case PageType.Calendar:
			case PageType.Seasonal:
			default:
				width = 320 * 4;
				break;
		}

		CurrentPage?.Activate();
		Recenter();
		UpdateScrollComponents();

		return true;
	}

	public void UpdateScrollComponents() {
		// Update component textures, source rects, and scales.
		bool is_magic = IsMagic;
		bool custom_scroll = Style.CustomScroll; // ?? is_magic;
		ScrollbarTheme? stheme = custom_scroll ?
			(is_magic ? Sprites.MAGIC_SCROLL : Sprites.STANDARD_SCROLL) : null;

		LeftFlow.ScrollAreaTexture = custom_scroll ? background : Game1.mouseCursors;
		LeftFlow.ScrollAreaSource = stheme?.SCROLL_BG ?? Sprites.SCROLL_BG;

		RightFlow.ScrollAreaTexture = custom_scroll ? background : Game1.mouseCursors;
		RightFlow.ScrollAreaSource = stheme?.SCROLL_BG ?? Sprites.SCROLL_BG;

		if (LeftFlow.ScrollBar != null) {
			LeftFlow.ScrollBar.texture = custom_scroll ? background : Game1.mouseCursors;
			LeftFlow.ScrollBar.sourceRect = stheme?.SCROLL_THUMB ?? Sprites.SCROLL_THUMB;
		}

		if (RightFlow.ScrollBar != null) {
			RightFlow.ScrollBar.texture = custom_scroll ? background : Game1.mouseCursors;
			RightFlow.ScrollBar.sourceRect = stheme?.SCROLL_THUMB ?? Sprites.SCROLL_THUMB;
		}

		if (btnLeftPageDown != null) {
			btnLeftPageDown.texture = custom_scroll ? background : Game1.mouseCursors;
			btnLeftPageDown.scale = btnLeftPageDown.baseScale = custom_scroll ? 3.2f : 0.8f;
			btnLeftPageDown.sourceRect = stheme?.DOWN_ARROW ?? Sprites.DOWN_ARROW;
		}

		if (btnLeftPageUp != null) {
			btnLeftPageUp.texture = custom_scroll ? background : Game1.mouseCursors;
			btnLeftPageUp.scale = btnLeftPageUp.baseScale = custom_scroll ? 3.2f : 0.8f;
			btnLeftPageUp.sourceRect = stheme?.UP_ARROW ?? Sprites.UP_ARROW;
		}

		if (btnRightPageDown != null) {
			btnRightPageDown.texture = custom_scroll ? background : Game1.mouseCursors;
			btnRightPageDown.scale = btnRightPageDown.baseScale = custom_scroll ? 3.2f : 0.8f;
			btnRightPageDown.sourceRect = stheme?.DOWN_ARROW ?? Sprites.DOWN_ARROW;
		}

		if (btnRightPageUp != null) {
			btnRightPageUp.texture = custom_scroll ? background : Game1.mouseCursors;
			btnRightPageUp.scale = btnRightPageUp.baseScale = custom_scroll ? 3.2f : 0.8f;
			btnRightPageUp.sourceRect = stheme?.UP_ARROW ?? Sprites.UP_ARROW;
		}

		if (btnPrevious != null) {
			btnPrevious.texture = custom_scroll ? background : Game1.mouseCursors;
			btnPrevious.scale = btnPrevious.baseScale = custom_scroll ? 3.2f : 0.8f;
			btnPrevious.sourceRect = stheme?.LEFT_ARROW ?? Sprites.LEFT_ARROW;
		}

		if (btnNext != null) {
			btnNext.texture = custom_scroll ? background : Game1.mouseCursors;
			btnNext.scale = btnNext.baseScale = custom_scroll ? 3.2f : 0.8f;
			btnNext.sourceRect = stheme?.RIGHT_ARROW ?? Sprites.RIGHT_ARROW;
		}
	}

	public void Recenter() {
		Vector2 pos = Utility.getTopLeftPositionForCenteringOnScreen(
			width,
			height
		);

		xPositionOnScreen = (int) pos.X;
		yPositionOnScreen = (int) pos.Y;

		var view = Game1.uiViewport;
		if (view.Width >= 1280 && view.Width <= 1376 && width > 1200) {
			xPositionOnScreen -= (1376 - view.Width) / 2 - 8;
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


		int rightMarginTop = 0;
		int rightMarginBottom = 0;
		int rightMarginLeft = 0;
		int rightMarginRight = 0;
		int rightScrollTop = Style.ScrollOffsetTop; // ?? -9999;
		if (rightScrollTop == -9999)
			rightScrollTop = IsMagic ? 32 : 16;
		int rightScrollBottom = Style.ScrollOffsetBottom; // ?? -9999;
		if (rightScrollBottom == -9999)
			rightScrollBottom = IsMagic ? -32 : 0;

		int leftMarginTop = 0;
		int leftMarginBottom = 0;
		int leftMarginLeft = 0;
		int leftMarginRight = 0;
		int leftScrollTop = 0;
		int leftScrollBottom = 0;

		if (CurrentPage is IRightFlowMargins rfm) {
			rightMarginTop = rfm.RightMarginTop;
			rightMarginBottom = rfm.RightMarginBottom;
			rightMarginLeft = rfm.RightMarginLeft;
			rightMarginRight = rfm.RightMarginRight;
			rightScrollTop = rfm.RightScrollMarginTop;
			rightScrollBottom = rfm.RightScrollMarginBottom;
		}

		if (CurrentPage?.Type == PageType.Seasonal || CurrentPage?.Type == PageType.Calendar)
			leftMarginTop = 64;

		if (CurrentPage is ILeftFlowMargins lfm) {
			leftMarginTop = lfm.LeftMarginTop;
			leftMarginLeft = lfm.LeftMarginLeft;
			leftMarginRight = lfm.LeftMarginRight;
			leftMarginBottom = lfm.LeftMarginBottom;
			leftScrollTop = lfm.LeftScrollMarginTop;
			leftScrollBottom = lfm.LeftScrollMarginBottom;
		}

		LeftFlow.Reposition(
			xPositionOnScreen + 32 + 16 + leftMarginLeft,
			yPositionOnScreen + 40 + leftMarginTop,
			width / 2 - 64 + 16 - leftMarginLeft - leftMarginRight,
			height - 100 - leftMarginTop - leftMarginBottom,
			leftScrollTop,
			leftScrollBottom
		);

		RightFlow.Reposition(
			xPositionOnScreen + width / 2 + 32 + rightMarginLeft,
			yPositionOnScreen + 40 + rightMarginTop,
			width / 2 - 64 + 16 - rightMarginLeft - rightMarginRight,
			height - 100 - rightMarginTop - rightMarginBottom,
			rightScrollTop,
			rightScrollBottom
		);

		UpdateTabs();
		UpdateCalendarComponents();
		CurrentPage?.UpdateComponents();
		PageComponents = CurrentPage?.GetComponents();
		populateClickableComponentList();
	}

	public void UpdateTabs() {
		int offsetX = 0;
		int offsetY = 60;

		var view = Game1.uiViewport;
		if (view.Height >= 720 && view.Height <= 752) {
			offsetY += 3 * (752 - view.Height) / 2;
		}

		if (view.Width >= 1280 && view.Width <= 1376 && width > 1200)
			offsetX = -8;

		if (btnTabsUp != null) {
			btnTabsUp.bounds = new Rectangle(
				xPositionOnScreen + width + offsetX,
				yPositionOnScreen + offsetY,
				64, 64
			);

			offsetY += 68;
		};

		for(int i = 0; i < Tabs.Count; i++) {
			var entry = Tabs[i];
			ClickableComponent cmp = entry.Item1;
			if (Pages[entry.Item2] is not ITab || cmp == null)
				continue;

			if (btnTabsUp != null && (i < TabScroll || i >= (TabScroll + VISIBLE_TABS))) {
				cmp.visible = false;
				continue;
			}

			cmp.visible = true;
			cmp.bounds = new Rectangle(
				xPositionOnScreen + width - 8 + offsetX,
				yPositionOnScreen + offsetY,
				64, 64
			);

			offsetY += 68;
		}

		if (btnTabsDown != null)
			btnTabsDown.bounds = new Rectangle(
				xPositionOnScreen + width + offsetX,
				yPositionOnScreen + offsetY,
				64, 64
			);
	}

	public void UpdateCalendarComponents() {
		PageType type = CurrentPage?.Type ?? PageType.Cover;
		if (type != PageType.Calendar && type != PageType.Seasonal) {
			calDays = null;
			btnNext = null;
			btnPrevious = null;
			return;
		}

		bool is_magic = IsMagic;
		bool custom_scroll = Style.CustomScroll; // ?? is_magic;
		ScrollbarTheme? stheme = custom_scroll ?
			(is_magic ? Sprites.MAGIC_SCROLL : Sprites.STANDARD_SCROLL) : null;

		if (btnPrevious == null)
			btnPrevious = new ClickableTextureComponent(
				Rectangle.Empty,
				custom_scroll ? background : Game1.mouseCursors,
				stheme?.LEFT_ARROW ?? Sprites.LEFT_ARROW,
				custom_scroll ? 3.2f : 0.8f
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
				custom_scroll ? background : Game1.mouseCursors,
				stheme?.RIGHT_ARROW ?? Sprites.RIGHT_ARROW,
				custom_scroll ? 3.2f : 0.8f
			) {
				region = REGION_LEFT_PAGE,
				myID = 89,
				leftNeighborID = 88,
				upNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				rightNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				downNeighborID = ClickableComponent.SNAP_AUTOMATIC
			};

		if (type == PageType.Calendar) {
			if (calDays == null) {
				calDays = new();
				// We can only display up to 28 days at a time, so don't use
				// the DaysPerMonth.
				for (int day = 1; day <= 28; day++)
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

			for (int day = 1; day <= 28; day++) {
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
		} else
			calDays = null;

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

		UpdateScrollComponents();
	}

	public int GetLeftFlowScroll() {
		return LeftFlow?.Position ?? 0;
	}

	public int GetRightFlowScroll() {
		return RightFlow?.Position ?? 0;
	}

	public void SetLeftFlow(IEnumerable<IFlowNode>? nodes) {
		LeftFlow.Set(nodes);
	}

	public void SetLeftFlow(IEnumerable<IFlowNode>? nodes, int step) {
		LeftFlow.Set(nodes, step);
	}

	public void SetLeftFlow(IEnumerable<IFlowNode>? nodes, int step, int scroll) {
		LeftFlow.Set(nodes, step, scroll);
	}

	public void SetRightFlow(IEnumerable<IFlowNode>? nodes) {
		RightFlow.Set(nodes);
	}

	public void SetRightFlow(IEnumerable<IFlowNode>? nodes, int step) {
		RightFlow.Set(nodes, step);
	}

	public void SetRightFlow(IEnumerable<IFlowNode>? nodes, int step, int scroll) {
		RightFlow.Set(nodes, step, scroll);
	}

	public bool ScrollLeftFlow(IFlowNode? node, int offset = 0) {
		return LeftFlow.ScrollTo(node, offset);
	}

	public bool ScrollRightFlow(IFlowNode? node, int offset = 0) {
		return RightFlow.ScrollTo(node, offset);
	}

	#region Events

	protected override bool _ShouldAutoSnapPrioritizeAlignedElements() {
		return false; // return true;
	}

	public override void automaticSnapBehavior(int direction, int oldRegion, int oldID) {
		base.automaticSnapBehavior(direction, oldRegion, oldID);
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

	public override void receiveScrollWheelAction(int direction) {
		base.receiveScrollWheelAction(direction);

		int x = Game1.getOldMouseX();
		int y = Game1.getOldMouseY();

		if (x > (xPositionOnScreen + width)) {
			if (ScrollTabs(direction > 0 ? -1 : 1)) {
				Game1.playSound("shiny4");
				if (Game1.options.SnappyMenus)
					snapCursorToCurrentSnappedComponent();
			}

			return;
		}

		if (CurrentPage?.ReceiveScroll(x, y, direction) ?? false)
			return;

		bool left = (x - xPositionOnScreen) < (width / 2);
		PageType type = CurrentPage?.Type ?? PageType.Cover;

		if (left && LeftFlow.HasValue && y >= LeftFlow.Y) {
			if (LeftFlow.Scroll(direction > 0 ? -1 : 1)) {
				Game1.playSound("shiny4");
				if (Game1.options.SnappyMenus)
					snapCursorToCurrentSnappedComponent();
			}

			return;
		}

		if ((type == PageType.Calendar || type == PageType.Seasonal) && left && !Game1.options.gamepadControls) {
			if (ChangeSeason(direction > 0 ? -1 : 1)) {
				Game1.playSound("shwip");
				if (Game1.options.SnappyMenus)
					snapCursorToCurrentSnappedComponent();
			}

			return;
		}

		if (RightFlow.HasValue) {
			//if (ScrollRightFlow(direction > 0 ? -1 : 1)) {
			if (RightFlow.Scroll(direction > 0 ? -1 : 1)) {
				Game1.playSound("shiny4");
				if (Game1.options.SnappyMenus)
					snapCursorToCurrentSnappedComponent();
			}

			return;
		}
	}

	public override void receiveKeyPress(Keys key) {
		base.receiveKeyPress(key);

		if (CurrentPage?.ReceiveKeyPress(key) ?? false)
			return;

		if (key == Keys.F5 && Mod.Config.DebugMode) {
			Mod.Invalidate();
			RefreshPages();
			Game1.playSound("bigSelect");
			return;
		}

		if (key == Keys.Tab) {
			if (
				Game1.oldKBState.IsKeyDown(Keys.LeftShift) ?
					PrevPage(true) :
					NextPage(true)
			)
				Game1.playSound("smallSelect");
		}

		// TODO: Determine which flow is active, between
		// the left and right, if we have both.
		ScrollableFlow? active = RightFlow.HasValue ? RightFlow : null;
		if (LeftFlow.HasValue && active == null)
			active = LeftFlow;

		int x = Game1.getOldMouseX();
		int y = Game1.getOldMouseY();

		if (LeftFlow.HasValue && LeftFlow.Bounds.Contains(x, y))
			active = LeftFlow;

		if (active is null)
			return;

		if (key == Keys.PageDown) {
			if (active.ScrollPage(1))
				Game1.playSound("shiny4");
		}

		if (key == Keys.PageUp) {
			if (active.ScrollPage(-1))
				Game1.playSound("shiny4");
		}

		if (key == Keys.Home) {
			if (active.ScrollToStart())
				Game1.playSound("shiny4");
		}

		if (key == Keys.End) {
			if (active.ScrollToEnd())
				Game1.playSound("shiny4");
		}
	}

	public override void leftClickHeld(int x, int y) {
		base.leftClickHeld(x, y);

		if (LeftFlow.HasValue && LeftFlow.LeftClickHeld(x, y))
			return;

		if (RightFlow.HasValue && RightFlow.LeftClickHeld(x, y))
			return;
	}

	public override void releaseLeftClick(int x, int y) {
		base.releaseLeftClick(x, y);

		LeftFlow.ReleaseLeftClick();
		RightFlow.ReleaseLeftClick();
	}

	public override void receiveLeftClick(int x, int y, bool playSound = true) {
		base.receiveLeftClick(x, y, playSound);

		if (CurrentPage?.ReceiveLeftClick(x, y, playSound) ?? false)
			return;

		if (calDays != null && CurrentPage is ICalendarPage page) {
			for (int day = 1; day <= Math.Min(28, ModEntry.DaysPerMonth); day++) {
				ClickableComponent cmp = calDays[day - 1];
				if (cmp.containsPoint(x, y)) {
					int oday = day + Day - 1;
					if (oday <= ModEntry.DaysPerMonth) {
						WorldDate date = new(Date);
						date.DayOfMonth = oday;

						int cx = x - cmp.bounds.X;
						int cy = y - cmp.bounds.Y;

						if (page.ReceiveCellLeftClick(cx, cy, date, cmp.bounds))
							return;
					}

					break;
				}
			}
		}

		if (LeftFlow.HasValue && LeftFlow.ReceiveLeftClick(x, y, playSound))
			return;

		if (RightFlow.HasValue && RightFlow.ReceiveLeftClick(x, y, playSound))
			return;

		// Tabs
		if (btnTabsDown?.containsPoint(x, y) ?? false) {
			btnTabsDown.scale = btnTabsDown.baseScale;
			if (ScrollTabs(1) && playSound)
				Game1.playSound("shiny4");
		}

		if (btnTabsUp?.containsPoint(x, y) ?? false) {
			btnTabsUp.scale = btnTabsUp.baseScale;
			if (ScrollTabs(-1) && playSound)
				Game1.playSound("shiny4");
		}

		foreach(var entry in Tabs) {
			if (entry.Item1.visible && entry.Item1.containsPoint(x, y)) {
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

		if (upperRightCloseButton?.containsPoint(x, y) ?? false) {
			if (ChangePage(0) && playSound)
				Game1.playSound("smallSelect");

			return;
		}

		if (CurrentPage?.ReceiveRightClick(x, y, playSound) ?? false)
			return;

		if (LeftFlow.HasValue && LeftFlow.ReceiveRightClick(x, y))
			return;

		if (RightFlow.HasValue && RightFlow.ReceiveRightClick(x, y))
			return;

		if (calDays != null && CurrentPage is ICalendarPage page) {
			for (int day = 1; day <= Math.Min(28, ModEntry.DaysPerMonth); day++) {
				ClickableComponent cmp = calDays[day - 1];
				if (cmp.containsPoint(x, y)) {
					int oday = day + Day - 1;

					if (oday <= ModEntry.DaysPerMonth) {
						WorldDate date = new(Date);
						date.DayOfMonth = oday;

						int cx = x - cmp.bounds.X;
						int cy = y - cmp.bounds.Y;

						if (page.ReceiveCellRightClick(cx, cy, date, cmp.bounds))
							return;
					}

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

		// Our flows are high priority because they perform
		// middle click scrolling within their hover action,
		// and we want to stop all other hover behavior if
		// scrolling.
		bool skip_hover = false;

		if (RightFlow.IsMiddleScrolling() && RightFlow.PerformMiddleScroll(x, y))
			skip_hover = true;
		else if ((LeftFlow.IsMiddleScrolling() || LeftFlow.HasValue) && LeftFlow.PerformMiddleScroll(x, y))
			skip_hover = true;
		else if (RightFlow.HasValue && RightFlow.PerformMiddleScroll(x, y))
			skip_hover = true;

		if (skip_hover)
			y = -1;

		btnPrevious?.tryHover(x, Season > 0 ? y : -1);
		btnNext?.tryHover(x, Season < WorldDate.MonthsPerYear - 1 ? y : -1);

		btnTabsUp?.tryHover(x, y);
		btnTabsDown?.tryHover(x, y);

		if (skip_hover)
			return;

		foreach (var entry in Tabs) {
			if (entry.Item1.visible && entry.Item1.containsPoint(x, y) && Pages[entry.Item2] is ITab tab) {
				HoverNode = tab.TabAdvancedTooltip;
				HoverText = tab.TabSimpleTooltip;
				HoverMagic = tab.TabMagic;
				break;
			}
		}

		if (calDays != null && CurrentPage is ICalendarPage page) {
			for (int day = 1; day <= Math.Min(28, ModEntry.DaysPerMonth); day++) {
				ClickableComponent cmp = calDays[day - 1];
				if (cmp.containsPoint(x, y)) {
					int oday = day + Day - 1;

					if (oday <= ModEntry.DaysPerMonth) {
						WorldDate date = new(Date);
						date.DayOfMonth = day + Day - 1;

						page.PerformCellHover(x - cmp.bounds.X, y - cmp.bounds.Y, date, cmp.bounds);
					}

					break;
				}
			}
		}

		CurrentPage?.PerformHover(x, y);

		if (LeftFlow.HasValue) {
			if (HoveredItem == null && LeftFlow.GetExtraAt(x, y) is Item item)
				HoveredItem = item;

			LeftFlow.PerformHover(x, y);
		}

		if (RightFlow.HasValue) {
			if (HoveredItem == null && RightFlow.GetExtraAt(x, y) is Item item)
				HoveredItem= item;

			RightFlow.PerformHover(x, y);
		}
	}

	#endregion

	#region Drawing

	public void DrawCustomMouse(SpriteBatch b, bool ignore_transparency = false, int cursor = -1) {
		if (!Mod.Theme.CustomMouse || (cursor != 44 && cursor != 0 && cursor >= 0) || Game1.options.hardwareCursor) {
			drawMouse(b, ignore_transparency, cursor);
			return;
		}

		float transparency = ignore_transparency ? 1 : Game1.mouseCursorTransparency;
		if (cursor < 0)
			cursor = (Game1.options.SnappyMenus && Game1.options.gamepadControls) ? 44 : 0;

		Rectangle source;
		switch(cursor) {
			case 44:
				source = Sprites.MOUSE_SNAP;
				break;
			default:
				source = Sprites.MOUSE_NORMAL;
				break;
		}

		b.Draw(
			texture: background,
			position: new Vector2(Game1.getMouseX(), Game1.getMouseY()),
			sourceRectangle: source,
			color: Color.White * transparency,
			rotation: 0f,
			origin: Vector2.Zero,
			scale: 4f + Game1.dialogueButtonScale / 150f,
			effects: SpriteEffects.None,
			layerDepth: 1f
		);
	}

	public override void draw(SpriteBatch b) {

		PageType type = CurrentPage?.Type ?? PageType.Blank;

		WorldDate date = new(Date);
		int today = Game1.Date.TotalDays;

		// Background

		b.Draw(
			Game1.fadeToBlackRect,
			Game1.graphics.GraphicsDevice.Viewport.Bounds,
			Color.Black * 0.75f
		);

		var pageSource = CurrentPage?.Type == PageType.Cover ?
			COVER : OPEN_PAGES[IsMagic ? 1 : 0];

		Models.PageOffset? offset = null;

		if (CurrentPage != null && Mod.Theme.PageOffsets != null)
			Mod.Theme.PageOffsets.TryGetValue(CurrentPage.Id, out offset);

		b.Draw(
			background,
			new Vector2(xPositionOnScreen, yPositionOnScreen),
			offset == null ? pageSource : new Rectangle(
				pageSource.X + offset.X,
				pageSource.Y + offset.Y,
				pageSource.Width,
				pageSource.Height
			),
			Color.White,
			0f,
			Vector2.Zero,
			4f,
			SpriteEffects.None,
			1f
		);

		// Page Draw.
		CurrentPage?.Draw(b);

		// Calendar / Seasonal Draw

		// The Grid
		if (type == PageType.Calendar)
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
		if (type == PageType.Calendar || type == PageType.Seasonal) {
			RenderHelper.DrawCenteredSpriteText(
				b,
				I18n.Calendar_When(
					season: Utility.getSeasonNameFromNumber(date.SeasonIndex),
					year: date.Year
				),
				xPositionOnScreen + (width / 4),
				yPositionOnScreen + 40,
				color: Style.SeasonTextColor ??
					(IsMagic ? Color.White : null)
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
		}

		// The Rest of the Calendar
		if (type == PageType.Calendar) {
			// Headers
			for (int day = 1; day <= 7; day++) {
				int x = xPositionOnScreen + 64 + 76 * (day - 1);
				int y = yPositionOnScreen + 108;
				string text = Mod.Helper.Translation.Get($"calendar.day.{day}");

				Vector2 size = Game1.dialogueFont.MeasureString(text);

				b.DrawString(
					Game1.dialogueFont,
					text,
					new Vector2(
						x + (76 - size.X) / 2,
						y + (60 - size.Y) / 2
					),
					color: Style.CalendarLabelColor ??
						(IsMagic ? Color.SkyBlue : Game1.textColor)
				) ;
			}

			// Days
			ICalendarPage? page = CurrentPage as ICalendarPage;

			for (int day = 1; day <= 28; day++) {
				int oday = day + Day - 1;
				date.DayOfMonth = oday;
				int ddays = date.TotalDays;

				ClickableComponent? cmp = calDays?[day - 1];
				if (cmp == null)
					continue;

				if (oday > ModEntry.DaysPerMonth) {
					b.Draw(Game1.staminaRect, cmp.bounds, (IsMagic ? Color.Black : Color.Gray) * 0.25f);
					continue;
				}

				if ((page?.ShouldHighlightToday ?? false) && ddays == today) {
					int num = 4 + (int) (2.0 * Game1.dialogueButtonScale / 8.0);
					RenderHelper.DrawBox(
						b,
						texture: Game1.mouseCursors,
						sourceRect: DAY_BORDER,
						x: cmp.bounds.X - num,
						y: cmp.bounds.Y - num,
						width: cmp.bounds.Width + num * 2,
						height: cmp.bounds.Height + num * 2,
						color: Style.CalendarHighlightColor ?? Color.Blue,
						scale: 4f,
						drawShadow: false
					);
				}

				page?.DrawUnderCell(b, date, cmp.bounds);

				b.DrawString(
					Game1.smallFont,
					Convert.ToString(oday),
					new Vector2(cmp.bounds.X, cmp.bounds.Y - 4),
					color: Style.CalendarDayColor ??
						(IsMagic ? Color.LightSkyBlue : Game1.textColor)
				);

				page?.DrawOverCell(b, date, cmp.bounds);

				if ((page?.ShouldDimPastCells ?? false) && ddays < today)
					b.Draw(
						Game1.staminaRect,
						cmp.bounds,
						Style.CalendarDimColor != null ?
							(Style.CalendarDimColor.Value * (Style.CalendarDimOpacity ?? 1f)) :
							(IsMagic ? Color.Black * .4f : Color.Gray * 0.35f)
					);
			}
		}

		// Tabs
		if (btnTabsDown != null) {
			if (TabScroll + VISIBLE_TABS >= Tabs.Count)
				btnTabsDown.draw(b, Color.Gray, 0.89f);
			else
				btnTabsDown.draw(b);
		}

		if (btnTabsUp != null) {
			if (TabScroll == 0)
				btnTabsUp.draw(b, Color.Gray, 0.89f);
			else
				btnTabsUp.draw(b);
		}

		for (int i = 0; i < Tabs.Count; i++) {
			ClickableComponent cmp = Tabs[i].Item1;
			int page = Tabs[i].Item2;
			if (cmp != null && cmp.visible && Pages[page] is ITab tab) {
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

				Texture2D? tex = tab.TabTexture;
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

		if (LeftFlow.HasValue)
			LeftFlow.Draw(
				b,
				Style.TextColor ?? (IsMagic ? Color.White : Game1.textColor),
				Style.TextShadowColor ?? (IsMagic ? MAGIC_SHADOW_COLOR : null)
			);

		if (RightFlow.HasValue)
			RightFlow.Draw(
				b,
				Style.TextColor ?? (IsMagic ? Color.White : Game1.textColor),
				Style.TextShadowColor ?? (IsMagic ? MAGIC_SHADOW_COLOR : null)
			);

		base.draw(b);

		LeftFlow.DrawMiddleScroll(b);
		RightFlow.DrawMiddleScroll(b);

		// Mouse
		DrawCustomMouse(b, true);

		// Tooltip
		if (HoverNode != null || !string.IsNullOrEmpty(HoverText)) {
			bool custom = HoverMagic ? Mod.Theme.Magic.CustomTooltip : Mod.Theme.Standard.CustomTooltip;

			SimpleHelper.DrawHover(
				HoverNode ?? CachedHoverText.Value!,
				b,
				Game1.smallFont,
				bgTexture: custom ? background : null,
				bgSource: custom ? (HoverMagic ? Sprites.MAGIC_TOOLTIP.BACKGROUND : Sprites.STANDARD_TOOLTIP.BACKGROUND) : null,
				bgScale: custom ? 4f : 1f,
				divTexture: custom ? background : null,
				divVSource: custom ? (HoverMagic ? Sprites.MAGIC_TOOLTIP.VERTICAL_LINE : Sprites.STANDARD_TOOLTIP.VERTICAL_LINE) : null,
				divHSource: custom ? (HoverMagic ? Sprites.MAGIC_TOOLTIP.HORIZONTAL_LINE : Sprites.STANDARD_TOOLTIP.HORIZONTAL_LINE) : null,
				defaultColor: HoverMagic ?
					(Mod.Theme.Magic.TextColor ?? Color.White) :
					(Mod.Theme.Standard.TextColor ?? Game1.textColor),
				defaultShadowColor: HoverMagic ?
					(Mod.Theme.Magic.TextShadowColor ?? MAGIC_SHADOW_COLOR) :
					(Mod.Theme.Standard.TextShadowColor ?? null)
			);
		}
	}

	#endregion

}
