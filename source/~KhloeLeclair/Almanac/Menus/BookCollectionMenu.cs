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
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Leclair.Stardew.Common;

using StardewValley;
using StardewValley.Menus;

using Leclair.Stardew.Almanac.Models;

namespace Leclair.Stardew.Almanac.Menus;

internal class BookCollectionMenu : IClickableMenu {

	public static readonly Rectangle BLANK_TAB = new(16, 368, 16, 16);

	#region Fields

	// Important Stuff
	private readonly ModEntry Mod;
	public readonly bool Standalone;
	public readonly bool IsLibrary;

	// Rendering
	public Texture2D Background;
	public bool DrawBG = true;

	// Books
	private readonly List<Book> Books = new();
	private readonly Dictionary<Book, bool> LockedBooks = new();

	[SkipForClickableAggregation]
	private readonly Dictionary<Book, ClickableTextureComponent> BookComponents = new();
	[SkipForClickableAggregation]
	private readonly Dictionary<ClickableTextureComponent, Book> ComponentBooks = new();

	// Pagination
	private int pageIndex = 0;
	[SkipForClickableAggregation]
	private readonly List<List<ClickableTextureComponent>> Pages = new();
	private List<ClickableTextureComponent>? CurrentPage => pageIndex >= 0 && Pages.Count > pageIndex ? Pages[pageIndex] : null;

	// Components
	public ClickableTextureComponent? btnPageUp;
	public ClickableTextureComponent? btnPageDown;
	public List<ClickableTextureComponent> currentPageComponents = new();

	// Search
	//private string? Filter = null;

	// Hover State
	private string? HoverText = null;
	private Book? HoverBook = null;

	#endregion

	#region Constructor

	public BookCollectionMenu(ModEntry mod, int x, int y, int width, int height, bool standalone = false, bool library = false)
	: base(x, y, width, height) {

		Standalone = standalone;
		Mod = mod;
		IsLibrary = library;

		// Load our texture
		Background = Mod.ThemeManager.Load<Texture2D>("Menu.png");

		

		// Close Button
		if (Standalone)
			initializeUpperRightCloseButton();

		DiscoverBooks();
		LayoutBooks();

		if (Standalone)
			Game1.playSound("bigSelect");

		if (Game1.options.SnappyMenus)
			snapToDefaultClickableComponent();
	}

	#endregion

	#region Logic

	private void DiscoverBooks() {
		// Get all the books that should be displayed in this menu.
		Books.Clear();

		foreach(var book in Mod.Books.GetAllBooks()) {
			var value = IsLibrary ? book.Library : book.Collection;
			if (value == null || !value.Enable)
				continue;

			bool locked = !string.IsNullOrEmpty(value.Condition) && !Common.GameStateQuery.CheckConditions(value.Condition);
			if (value.Secret && locked)
				continue;

			LockedBooks[book] = locked;
			Books.Add(book);
		}

		BuildBookComponents();
	}

	private void BuildBookComponents() {
		int idx = 0;

		BookComponents.Clear();
		ComponentBooks.Clear();

		foreach (Book book in Books) {
			idx++;

			// TODO: Custom book textures.

			ClickableTextureComponent cmp = BookComponents[book] = new ClickableTextureComponent(
				name: "",
				bounds: new Rectangle(0, 0, 128, 128),
				label: null,
				hoverText: LockedBooks[book] ? "locked" : "",
				texture: Background,
				sourceRect: book.Color.HasValue ? Sprites.BOOK_UNCOLORED : Sprites.BOOK_BLANK,
				scale: 4f
			) {
				myID = 200 + idx,
				upNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				downNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				rightNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				fullyImmutable = true,
				region = 8000
			};

			ComponentBooks[cmp] = book;
		}
	}

	protected virtual List<ClickableTextureComponent> CreateNewPage() {
		List<ClickableTextureComponent> result = new();
		Pages.Add(result);
		return result;
	}

	protected virtual ClickableTextureComponent[,] CreateNewPageLayout() {
		return new ClickableTextureComponent[10, 8];
	}

	protected virtual bool SpaceOccupied(ClickableTextureComponent[,] layout, int x, int y, int width, int height) {
		if (width == 1 && height == 1)
			return layout[x, y] != null;

		if (x + width > layout.GetLength(0) || y + height > layout.GetLength(1))
			return true;

		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				if (layout[x + i, y + j] != null)
					return true;
			}
		}

		return false;
	}

	protected virtual int PageX() => xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth;
	protected virtual int PageY() => yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth - 16;

	protected void LayoutBooks() {
		int offsetX = PageX();
		int offsetY = PageY();
		int marginX = 10;
		int marginY = 0;

		Pages.Clear();

		List<ClickableTextureComponent> page = CreateNewPage();
		ClickableTextureComponent[,] layout = CreateNewPageLayout();

		int xLimit = layout.GetLength(0);
		int yLimit = layout.GetLength(1);

		int x = 0;
		int y = 0;

		List<Book> sorted = Books.ToList();

		sorted.Sort((a, b) => {
			if (a.Title != null && b.Title != null)
				return a.Title.CompareTo(b.Title);

			return a.Id!.ToString().CompareTo(b.Id!.ToString());
		});

		foreach (Book book in sorted) {
			BookComponents.TryGetValue(book, out ClickableTextureComponent? cmp);
			if (cmp == null)
				continue;

			// TODO: Bigger books?
			int width = 2;
			int height = 2;

			float scale = Math.Min(xLimit / width, yLimit / height);
			if (scale < 1) {
				width = Math.Max(1, (int) (width * scale));
				height = Math.Max(1, (int) (height * scale));
			}

			while (SpaceOccupied(layout, x, y, width, height)) {
				x++;
				if (x >= xLimit) {
					x = 0;
					y++;
					if (y >= yLimit) {
						page = CreateNewPage();
						layout = CreateNewPageLayout();
						y = 0;
					}
				}
			}

			cmp.bounds = new Rectangle(
				offsetX + x * (64 + marginX),
				offsetY + y * (64 + marginY),
				64 * width,
				64 * height
			);

			page.Add(cmp);

			if (width == 1 && height == 1)
				layout[x, y] = cmp;
			else
				for (int i = 0; i < width; i++) {
					for (int j = 0; j < height; j++) {
						layout[x + i, y + j] = cmp;
					}
				}
		}

		if (Pages.Count > 1 && btnPageUp == null) {
			btnPageUp = new ClickableTextureComponent(
				new Rectangle(xPositionOnScreen + 768 + 16, offsetY, 64, 64),
				Game1.mouseCursors,
				Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 12),
				0.8f
			) {
				myID = 88,
				downNeighborID = 89,
				leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				rightNeighborID = ClickableComponent.SNAP_AUTOMATIC,
			};

			btnPageDown = new ClickableTextureComponent(
					new Rectangle(xPositionOnScreen + 768 + 16, offsetY + 192 + 32, 64, 64),
					Game1.mouseCursors,
					Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 11),
					0.8f
				) {
				myID = 89,
				upNeighborID = 88,
				leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				rightNeighborID = ClickableComponent.SNAP_AUTOMATIC
			};

		} else if (Pages.Count <= 1 && btnPageUp != null) {
			btnPageUp = null;
			btnPageDown = null;
		}

		if (btnPageUp != null)
			btnPageUp.bounds.Y = offsetY;
		if (btnPageDown != null)
			btnPageDown.bounds.Y = offsetY + (yLimit - 1) * (64 + marginY) + marginY;

		if (pageIndex >= Pages.Count)
			pageIndex = Pages.Count - 1;

		UpdateCurrentComponents();
	}

	protected virtual void UpdateCurrentComponents() {
		currentPageComponents.Clear();
		if (CurrentPage != null)
			currentPageComponents.AddRange(CurrentPage);

		populateClickableComponentList();
	}

	public virtual void ChangePage(int change) {
		SetPage(pageIndex + change);
	}

	public virtual void SetPage(int idx) {
		pageIndex = idx;
		if (pageIndex < 0)
			pageIndex = 0;
		if (pageIndex >= Pages.Count)
			pageIndex = Pages.Count - 1;

		UpdateCurrentComponents();
	}

	#endregion

	#region Events

	public override void snapToDefaultClickableComponent() {
		currentlySnappedComponent = CurrentPage != null && CurrentPage.Count > 0 ? CurrentPage[0] : null;
		snapCursorToCurrentSnappedComponent();
	}

	public override void receiveScrollWheelAction(int direction) {
		base.receiveScrollWheelAction(direction);

		int change;
		if (direction > 0 && pageIndex > 0)
			change = -1;
		else if (direction < 0 && pageIndex < Pages.Count - 1)
			change = 1;
		else
			return;

		ChangePage(change);
		Game1.playSound("shwip");

		if (!Game1.options.SnappyMenus)
			return;

		setCurrentlySnappedComponentTo((change > 0 ? btnPageDown?.myID : btnPageUp?.myID) ?? ClickableComponent.SNAP_TO_DEFAULT);
		snapCursorToCurrentSnappedComponent();
	}

	public Book? GetBookAt(int x, int y) {
		if (CurrentPage == null)
			return null;

		foreach(var cmp in CurrentPage) {
			if (cmp.containsPoint(x, y) && ComponentBooks.TryGetValue(cmp, out Book? book)) {
				if (!cmp.hoverText.Equals("locked"))
					return book;
				break;
			}
		}

		return null;
	}

	public override void receiveLeftClick(int x, int y, bool playSound = true) {
		base.receiveLeftClick(x, y, playSound);

		// Pagination
		if (btnPageUp != null && btnPageUp.containsPoint(x, y) && pageIndex > 0) {
			ChangePage(-1);
			if (playSound)
				Game1.playSound("smallSelect");
			btnPageUp.scale = btnPageUp.baseScale;
		}

		if (btnPageDown != null && btnPageDown.containsPoint(x, y) && pageIndex < Pages.Count - 1) {
			ChangePage(+1);
			if (playSound)
				Game1.playSound("smallSelect");
			btnPageDown.scale = btnPageDown.baseScale;
			return;
		}

		// Clickable Books
		if (CurrentPage != null)
			foreach(var cmp in CurrentPage) {
				if (cmp.containsPoint(x, y) && ComponentBooks.TryGetValue(cmp, out Book? book)) {
					if (!cmp.hoverText.Equals("locked")) {
						// TODO: Book clicked action
					}

					cmp.scale = cmp.baseScale;
					return;
				}
			}
	}

	public override void receiveRightClick(int x, int y, bool playSound = true) {
		// Clickable Books
		if (CurrentPage != null)
			foreach (var cmp in CurrentPage) {
				if (cmp.containsPoint(x, y) && ComponentBooks.TryGetValue(cmp, out Book? book)) {
					if (!cmp.hoverText.Equals("locked")) {
						// TODO: Favoriting books
					}

					cmp.scale = cmp.baseScale;
					return;
				}
			}
	}

	public override void performHoverAction(int x, int y) {
		base.performHoverAction(x, y);

		HoverText = null;
		HoverBook = null;

		// Clickable Books
		if (CurrentPage != null)
			foreach (var cmp in CurrentPage) {
				if (ComponentBooks.TryGetValue(cmp, out Book? book)) {
					bool locked = LockedBooks[book];
					cmp.tryHover(x, locked ? -1 : y);
					if (cmp.containsPoint(x, y)) {
						if (locked)
							HoverText = "???";
						else
							HoverBook = book;
					}
				}
			}

		// Pagination
		if (btnPageUp != null)
			btnPageUp.tryHover(x, pageIndex > 0 ? y : -1);

		if (btnPageDown != null)
			btnPageDown.tryHover(x, pageIndex < Pages.Count - 1 ? y : -1);
	}

	#endregion

	#region Drawing

	public void DrawButton(SpriteBatch batch, ClickableComponent cmp, bool selected) {
		Vector2 pos = new(cmp.bounds.X, cmp.bounds.Y + (selected ? 8 : 0));

		batch.Draw(
			texture: Game1.mouseCursors,
			position: pos,
			sourceRectangle: BLANK_TAB,
			color: Color.White,
			rotation: 0f,
			origin: Vector2.Zero,
			scale: 4f,
			effects: SpriteEffects.None,
			layerDepth: 0.0001f
		);

		batch.Draw(
			texture: Background,
			position: pos,
			sourceRectangle: Sprites.ICON_BOOK,
			color: Color.White,
			rotation: 0f,
			origin: Vector2.Zero,
			scale: 4f,
			effects: SpriteEffects.None,
			layerDepth: 1f
		);
	}

	public override void draw(SpriteBatch b) {
		if (Standalone && DrawBG)
			b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black * 0.5f);

		if (Standalone)
			Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, speaker: false, drawOnlyBox: true);

		// Books
		List<ClickableTextureComponent>? books = CurrentPage;

		bool drawn = false;

		int idx = 20 * pageIndex;

		if (books != null)
			foreach(var cmp in books) {
				if (!ComponentBooks.TryGetValue(cmp, out Book? book))
					continue;

				if (LockedBooks[book]) {
					drawn = true;
					cmp.DrawBounded(b, Color.Black * 0.35f, 0.89f);

				} else if (book.Color.HasValue) {
					drawn = true;

					cmp.sourceRect = Sprites.BOOK_BLANK;
					cmp.DrawBounded(b, Color.White, 0.88f);
					cmp.sourceRect = Sprites.BOOK_UNCOLORED;

					Color[] colors = new Color[] {
						Color.AliceBlue,
						Color.SeaGreen,
						Color.Aquamarine,
						Color.ForestGreen,
						Color.Purple,
						Color.OrangeRed,
						Color.CornflowerBlue,
						Color.DeepPink,
						Color.Gold
					};

					cmp.DrawBounded(b, colors[idx % colors.Length], 0.89f);

				} else {
					// TODO: Colored draw
					drawn = true;
					cmp.DrawBounded(b);
				}

				// TODO: Draw overlay texture

				if (LockedBooks[book])
					continue;

				var sprite = SpriteHelper.GetSprite(InventoryHelper.CreateItemById($"(O){idx}", 1));
				idx++;

				float scale = cmp.scale - 1;
				float size = 16 * scale;

				if (sprite is not null)
					sprite.Draw(
						batch: b,
						location: new Vector2(
							cmp.bounds.X + (cmp.bounds.Width - size) / 2,
							cmp.bounds.Y + (cmp.bounds.Height - size) / 2 - 16
						),
						scale: cmp.scale - 1
					);

				// TODO: Draw favorite star
			}

		if (!drawn) {
			// TODO: Message when no books are available.
		}

		// Base Menu
		base.draw(b);

		// Pagination
		if (pageIndex < Pages.Count - 1)
			btnPageDown?.draw(b);
		else
			btnPageDown?.draw(b, Color.Black * 0.35f, 0.89f);

		if (pageIndex > 0)
			btnPageUp?.draw(b);
		else
			btnPageUp?.draw(b, Color.Black * 0.35f, 0.89f);

		if (Standalone) {
			Game1.mouseCursorTransparency = 1f;
			drawMouse(b);
		}

		if (HoverBook != null)
			IClickableMenu.drawHoverText(b, HoverBook.Title ?? HoverBook.Id!.ToString(), Game1.smallFont);
		else if (HoverText != null)
			IClickableMenu.drawHoverText(b, HoverText, Game1.smallFont);
	}

	#endregion

}
