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

using Leclair.Stardew.Almanac.Menus;
using Leclair.Stardew.Almanac.Pages;
using Leclair.Stardew.Common;
using Leclair.Stardew.Common.Types;
using Leclair.Stardew.Common.UI;
using Leclair.Stardew.Common.UI.FlowNode;
using Leclair.Stardew.Common.UI.SimpleLayout;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using StardewModdingAPI.Utilities;

using StardewValley;
using StardewValley.Menus;

namespace Leclair.Stardew.Almanac.Crops;

public enum SeedFilter {
	Disabled,
	Inventory,
	Owned
}

public class CropState : BaseState {
	public int FertIndex;
	public bool PaddyBonus;
	public bool Agriculturist;
	public SeedFilter Seeds;
}

public class CropPage : BasePage<CropState>, ICalendarPage, ITab {

	private readonly static Tuple<string, double>[] FERTILIZERS = new Tuple<string, double>[] {
		new("465", 0.10),
		new("466", 0.25),
		new("918", 0.33)
	};

	private List<CropInfo>[]? LastDays;

	private CropInfo? HoverCrop;

	// Fertilizer and Agriculturist Status
	public List<ClickableComponent> FertComponents;
	private readonly List<Tuple<Item?, SpriteInfo?, double, int>> Fertilizers;
	private int FertIndex = -1;
	private Tuple<Item?, SpriteInfo?, double, int>? Fertilizer => Fertilizers == null || FertIndex < 0 || FertIndex >= Fertilizers.Count ? null : Fertilizers[FertIndex];

	private bool PaddyBonus = true;
	public ClickableComponent tabPaddy;
	private readonly SpriteInfo spritePaddy;
	private readonly int tabPaddySprite;

	private bool Agriculturist;
	public ClickableComponent tabAgri;
	private readonly SpriteInfo spriteAgri;
	private readonly int tabAgriSprite;

	private SeedFilter Seeds;
	public ClickableComponent tabSeeds;
	private SpriteInfo? spriteSeeds;
	private readonly int tabSeedsSprite;

	private readonly Cache<ISimpleNode?, SeedFilter> SeedNode;
	private readonly Cache<SpriteInfo?[]?, CropInfo?> CropGrowth;
	private readonly Cache<ISimpleNode?, WorldDate?> CalendarTip;

	private readonly Dictionary<CropInfo, IFlowNode> CropNodes = new();

	private WorldDate? HoveredDate;
	private WorldDate? ClickedDate;

	private int ClickedIndex;

	#region Lifecycle

	public static CropPage? GetPage(AlmanacMenu menu, ModEntry mod) {
		if (!mod.Config.ShowCrops)
			return null;

		return new(menu, mod);
	}

	public CropPage(AlmanacMenu menu, ModEntry mod) : base(menu, mod) {
		// Caches
		SeedNode = new(filter => {
			string text;

			switch(filter) {
				case SeedFilter.Inventory:
					text = I18n.Page_Crop_SeedFilter_Inventory();
					break;
				case SeedFilter.Owned:
					text =I18n.Page_Crop_SeedFilter_Owned();
					break;
				default:
					text = I18n.Page_Crop_SeedFilter_Disabled();
					break;
			}

			return SimpleHelper.Builder()
				.Text(I18n.Page_Crop_SeedFilter())
				.FormatText(text, color: Game1.textColor * 0.4f)
				.GetLayout();

		}, () => Seeds);

		CalendarTip = new(date => {
			if (date == null)
				return null;

			List<CropInfo>? crops = LastDays?[date.DayOfMonth - 1];
			if (crops == null)
				return null;

			SimpleBuilder builder = new();

			builder.FormatText(I18n.Crop_LastDay());
			builder.Divider();

			if (crops.Count > 5) {
				List<ISimpleNode> left = new();
				List<ISimpleNode> right = new();

				bool is_right = false;

				foreach(CropInfo crop in crops) {
					(is_right ? right : left).Add(new Common.UI.SimpleLayout.SpriteNode(crop.Sprite, scale: 3f, label: crop.Name));
					is_right = !is_right;
				}

				builder.Group(margin: 8)
					.Group(align: Alignment.Top).AddRange(left).EndGroup()
					.Group(align: Alignment.Top).AddRange(right).EndGroup()
					.EndGroup();

			} else {
				foreach (CropInfo crop in crops)
					builder.Sprite(crop.Sprite, 3f, label: crop.Name);
			}

			return builder.GetLayout();

		}, () => HoveredDate);

		CropGrowth = new(key => {
			if (!key.HasValue || !Mod.Config.ShowPreviews)
				return null;

			CropInfo crop = key.Value;

			SpriteInfo?[] sprites = new SpriteInfo[ModEntry.DaysPerMonth];
			int[] phases = GetActualPhaseTime(crop.Days, crop.Phases, crop.IsPaddyCrop);

			int phase = 0;
			int days = 0;
			bool grown = false;

			for (int i = 0; i < sprites.Length; i++) {
				if (grown) {
					if (crop.Regrow > 0) {
						days++;
						if (days > crop.Regrow)
							days = 1;

						if (days == 1)
							sprites[i] = Mod.Config.PreviewUseHarvestSprite ? crop.Sprite : crop.PhaseSprites?[^2];
						else
							sprites[i] = crop.PhaseSprites?[^1];

						continue;

					} else {
						grown = false;
						phase = 0;
						days = 1;

						sprites[i] = Mod.Config.PreviewUseHarvestSprite ? crop.Sprite : crop.PhaseSprites?[^1];
						continue;
					}

				} else {
					if (!Mod.Config.PreviewPlantOnFirst && Game1.Date.SeasonIndex == Menu.Season && Game1.Date.DayOfMonth > (i + 1)) {
						sprites[i] = null;
						continue;
					}

					while (true) {
						days++;
						if (days > phases[phase]) {
							phase++;
							days = 0;
							if (phase >= phases.Length) {
								grown = true;
								break;
							}
						} else
							break;
					}
					if (grown) {
						i--;
						continue;
					}
				}

				sprites[i] = crop.PhaseSprites?[phase];
			}

			return sprites;

		}, () => HoverCrop);

		// Cache Fertilizer items.
		Fertilizers = new(FERTILIZERS.Length);
		FertComponents = new(FERTILIZERS.Length);

		for (int i = 0; i < FERTILIZERS.Length; i++) {
			string id = FERTILIZERS[i].Item1;

			SObject? obj = id == "" ? null : new(FERTILIZERS[i].Item1, 1);
			Item? item = obj?.getOne();
			SpriteInfo? sprite = item == null ? null : SpriteHelper.GetSprite(item);

			Fertilizers.Add(new(item, sprite, FERTILIZERS[i].Item2, Game1.random.Next(2 * AlmanacMenu.TABS.Length)));
			FertComponents.Add(new(new Rectangle(0, 0, 64, 64), (string?) null) {
				myID = 5000 + i,
				upNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				rightNeighborID = ClickableComponent.SNAP_AUTOMATIC,
				leftNeighborID = ClickableComponent.SNAP_AUTOMATIC
			});
		}

		tabAgri = new(new(0, 0, 64, 64), (string?) null) {
			myID = 4999,
			upNeighborID = ClickableComponent.SNAP_AUTOMATIC,
			leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
			rightNeighborID = ClickableComponent.SNAP_AUTOMATIC
		};
		tabAgriSprite = Game1.random.Next(2 * AlmanacMenu.TABS.Length);
		spriteAgri = new SpriteInfo(Game1.mouseCursors, new Rectangle(80, 624, 16, 16));

		tabPaddy = new(new(0, 0, 64, 64), (string?) null) {
			myID = 4998,
			upNeighborID = ClickableComponent.SNAP_AUTOMATIC,
			leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
			rightNeighborID = ClickableComponent.SNAP_AUTOMATIC
		};
		tabPaddySprite = Game1.random.Next(2 * AlmanacMenu.TABS.Length);
		spritePaddy = new(Menu.background, new Rectangle(256, 352, 16, 16));

		tabSeeds = new(new(0, 0, 64, 64), (string?) null) {
			myID = 4997,
			upNeighborID = ClickableComponent.SNAP_AUTOMATIC,
			leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
			rightNeighborID = ClickableComponent.SNAP_AUTOMATIC
		};
		tabSeedsSprite = Game1.random.Next(2 * AlmanacMenu.TABS.Length);
		spriteSeeds = SpriteHelper.GetSprite(InventoryHelper.CreateItemById("(O)495", 1));

		// Cache Agriculturist status.
		Agriculturist = Game1.player.professions.Contains(Farmer.agriculturist);

		//Update();
	}

	public override void ThemeChanged() {
		base.ThemeChanged();

		spritePaddy.Texture = Menu.background;
	}

	#endregion

	#region Logic

	public override CropState SaveState() {
		var state = base.SaveState();
		state.PaddyBonus = PaddyBonus;
		state.Agriculturist = Agriculturist;
		state.FertIndex = FertIndex;
		state.Seeds = Seeds;

		return state;
	}

	public override void LoadState(CropState state) {
		base.LoadState(state);
		PaddyBonus = state.PaddyBonus;
		Agriculturist = state.Agriculturist;
		FertIndex = state.FertIndex;
		Seeds = state.Seeds;
	}

	public static string AgriculturistName() {
		return LevelUpMenu.getProfessionTitleFromNumber(Farmer.agriculturist);
	}

	public int[] GetActualPhaseTime(int days, int[] phases, bool isPaddyCrop) {
		float modifier = (float) (Fertilizer?.Item3 ?? 0f);
		if (PaddyBonus && isPaddyCrop)
			modifier += 0.25f;
		if (Agriculturist)
			modifier += 0.1f;

		int remove = (int) Math.Ceiling(days * modifier);
		int tries = 0;

		int[] result = (int[]) phases.Clone();

		while (remove > 0 && tries < 3) {
			for (int i = 0; i < result.Length; i++) {
				if ((i > 0 || result[i] > 1) && result[i] > 0 && result[i] != Crop.finalPhaseLength) {
					result[i]--;
					remove--;
				}
				if (remove <= 0)
					break;
			}
			tries++;
		}

		return result;
	}

	public override void Update() {
		base.Update();

		string seasonSeeds;
		switch (Menu.Date.SeasonIndex) {
			case 0:
				seasonSeeds = "(O)495";
				break;
			case 1:
				seasonSeeds = "(O)496";
				break;
			case 2:
				seasonSeeds = "(O)497";
				break;
			case 3:
			default:
				seasonSeeds = "(O)498";
				break;
		}

		spriteSeeds = SpriteHelper.GetSprite(InventoryHelper.CreateItemById(seasonSeeds, 1));

		LastDays = new List<CropInfo>[ModEntry.DaysPerMonth];

		CropNodes.Clear();

		List<CropInfo> crops = Mod.Crops.GetSeasonCrops(Menu.Season);
		crops.Sort((a,b) => StringComparer.CurrentCultureIgnoreCase.Compare(a.Name, b.Name));

		FlowBuilder builder = new();

		var agriculturist = Agriculturist ? FlowHelper.Builder()
			.Sprite(spriteAgri, 2)
			.Text($" {AgriculturistName()}", bold: true, color: Color.ForestGreen)
			.Build() : null;

		IFlowNode[]? fertilizer;
		if (Fertilizer?.Item1 == null)
			fertilizer = null;
		else {
			fertilizer = FlowHelper.Builder()
				.Sprite(Fertilizer.Item2, 2, extra: Fertilizer.Item1)
				.Text($" {Fertilizer.Item1.DisplayName}", bold: true, extra: Fertilizer.Item1)
				.Build();
		}

		if (Agriculturist && fertilizer != null)
			builder.Translate(
				Mod.Helper.Translation.Get("crop.using-both"),
				new { agriculturist, fertilizer },
				id: "start"
			);
		else if (Agriculturist)
			builder.Translate(
				Mod.Helper.Translation.Get("crop.using-agri"),
				new { agriculturist },
				id: "start"
			);
		else if (fertilizer != null)
			builder.Translate(
				Mod.Helper.Translation.Get("crop.using-speed"),
				new { fertilizer },
				id: "start"
			);
		else
			builder.FormatText(I18n.Crop_UsingNone(), id: "start");

		if (PaddyBonus)
			builder.FormatText($" {I18n.Crop_UsingPaddy()}");

		builder.Text("\n\n");

		WorldDate start = new(Menu.Date);
		start.Year = 1;

		WorldDate end = new(start);
		start.DayOfMonth = 1;
		end.DayOfMonth = ModEntry.DaysPerMonth;

		bool first = true;

		IList<Item>? items;
		switch(Seeds) {
			case SeedFilter.Inventory:
				items = Game1.player.Items;
				break;
			case SeedFilter.Owned:
				items = new List<Item>();
				Utility.ForEachItem(item => {
					items.Add(item);
					return true;
				});
				break;
			case SeedFilter.Disabled:
			default:
				items = null;
				break;
		}

		int displayed = 0;

		foreach (CropInfo crop in crops) {
			if (items != null) {
				if (crop.Seeds == null || crop.Seeds.Length == 0 || items.Count == 0)
					continue;

				bool matched = false;
				foreach(var iitem in items) {
					foreach (var item in crop.Seeds) {
						if (item.canStackWith(iitem)) {
							matched = true;
							break;
						}
					}
					if (matched)
						break;
				}

				if (!matched)
					continue;
			}

			// Null value protection. Can remove later.
			if (crop.EndDate == null || crop.StartDate == null)
				continue;

			displayed++;

			WorldDate last = new(crop.EndDate);

			int[] phases = GetActualPhaseTime(crop.Days, crop.Phases, crop.IsPaddyCrop);
			int days = phases.Sum();
			last.TotalDays -= days;

			// Count how many times we can harvest this season.
			int harvests = 0;
			bool harvest_offset = !Mod.Config.PreviewPlantOnFirst && Game1.Date.SeasonIndex == Menu.Season;
			int cur = harvest_offset ? Game1.Date.DayOfMonth : 1;
			bool first_grow = true;

			if (days > 0) {
				int limit = crop.EndDate.TotalDays + 1 - start.TotalDays - (cur - 1);
				cur = 1;

				while (cur <= limit) {
					if (first_grow)
						cur += days;
					else if (crop.Regrow > 0)
						cur += crop.Regrow;
					else
						cur += days;

					first_grow = false;
					if (cur <= limit)
						harvests++;
				}
			}

			if (last.SeasonIndex == Menu.Season) {
				int day = last.DayOfMonth - 1;
				if (LastDays.Length > day && LastDays[day] == null)
					LastDays[day] = new();

				LastDays[day].Add(crop);
			}

			bool OnHover(IFlowNodeSlice slice, int x, int y) {
				HoverCrop = crop;
				Menu.HoveredItem = crop.Item;
				return true;
			}

			SDate sdate = new(last.DayOfMonth, last.Season);

			IFlowNode node = new Common.UI.FlowNode.SpriteNode(
				crop.Sprite,
				3f,
				onHover: OnHover,
				id: crop.Id
			);
			CropNodes[crop] = node;

			if (first)
				first = false;
			else
				builder.Text("\n\n");

			builder
				.Add(node)
				.Text($" {crop.Name}", font: Game1.dialogueFont, align: Alignment.VCenter, onHover: OnHover, noComponent: true);

			if (Mod.Config.DebugMode)
				builder.Text($" (#{crop.Id})", align: Alignment.VCenter);

			builder
				.Text("\n")
				.FormatText(I18n.Crop_GrowTime(count: days), shadow: false);

			if (crop.Regrow > 0)
				builder.FormatText($" {I18n.Crop_RegrowTime(count: crop.Regrow)}", shadow: false);

			if (harvest_offset) {
				SDate hdate = new(Game1.Date.DayOfMonth, Game1.Date.Season);
				builder.FormatText($" {I18n.Crop_HarvestsDate(count: harvests, date: hdate.ToLocaleString(withYear: false))}", shadow: false);
			} else if (Game1.Date.SeasonIndex > Menu.Season) {
				SDate hdate = new(1, start.Season);
				builder.FormatText($" {I18n.Crop_HarvestsDate(count: harvests, date: hdate.ToLocaleString(withYear: false))}", shadow: false);
			} else
				builder.FormatText($" {I18n.Crop_Harvests(count: harvests)}", shadow: false);

			if (crop.IsTrellisCrop)
				builder.FormatText($" {I18n.Crop_TrellisNote()}", shadow: false);

			if (crop.IsPaddyCrop)
				builder.FormatText($" {I18n.Crop_PaddyNote()}", shadow: false);

			if (crop.IsGiantCrop) {
				var link = FlowHelper.FormatText(
					I18n.Crop_GiantHover(),
					onHover: (_,_,_) => {
						if (crop.GiantSprite == null)
							return false;

						Menu.HoverNode = SimpleHelper.Builder(LayoutDirection.Horizontal)
							.Sprite(crop.GiantSprite, scale: 8)
							.GetLayout();

						return true;
					}
				);

				builder
					.Text(" ")
					.Translate(
						Mod.Helper.Translation.Get("crop.giant-note"),
						new {
							link
						},
						new TextStyle(shadow: false)
					);
			}

			builder.FormatText($" {I18n.Crop_LastDate(date: sdate.ToLocaleString(withYear: false))}", shadow: false);
		}

		if (displayed == 0)
			builder.FormatText(I18n.Page_Crop_None());

		SetRightFlow(builder, 2, -1);

		if (Active)
			CropGrowth.Invalidate();
	}

	#endregion

	#region ITab

	public override int SortKey => 0;
	public override string TabSimpleTooltip => I18n.Page_Crops();
	public override Texture2D TabTexture => Game1.objectSpriteSheet;
	public override Rectangle? TabSource => Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 24, 16, 16);

	#endregion

	#region IAlmanacPage

	public override void UpdateComponents() {
		base.UpdateComponents();

		int x = Menu.xPositionOnScreen + Menu.width / 2 + 64;
		int y = Menu.yPositionOnScreen + Menu.height - 20;

		var view = Game1.uiViewport;
		if (view.Height >= 720 && view.Height <= 795) {
			y -= (795 - view.Height) / 2 - 8;
		}

		foreach (ClickableComponent cmp in FertComponents) {
			cmp.bounds = new(x, y, 64, 64);
			x += 68;
		}

		x += 36;
		tabAgri.bounds = new(x, y, 64, 64);

		x += 68;
		tabPaddy.bounds = new(x, y, 64, 64);

		x += 68 + 36 + 36;
		tabSeeds.bounds = new(x, y, 64, 64);
	}

	public override bool ReceiveKeyPress(Keys key) {

		if (key == Keys.D4) {
			Agriculturist = !Agriculturist;
			Update();
			Game1.playSound("smallSelect");
			return true;
		}

		if (key == Keys.D5) { 
			PaddyBonus = !PaddyBonus;
			Update();
			Game1.playSound("smallSelect");
			return true;
		}

		if (key == Keys.D6) {
			Seeds = CommonHelper.Cycle(Seeds);
			Update();
			Game1.playSound("smallSelect");
			return true;
		}

		int idx = -1;

		if (key == Keys.D1)
			idx = 0;
		else if (key == Keys.D2)
			idx = 1;
		else if (key == Keys.D3)
			idx = 2;

		if (idx != -1) {
			FertIndex = FertIndex == idx ? -1 : idx;
			Update();
			Game1.playSound("smallSelect");
			return true;
		}

		return base.ReceiveKeyPress(key);
	}

	public override void PerformHover(int x, int y) {
		base.PerformHover(x, y);

		HoverCrop = null;

		for (int i = 0; i < FertComponents.Count; i++) {
			ClickableComponent cmp = FertComponents[i];
			if (cmp.containsPoint(x, y)) {
				Menu.HoveredItem = Fertilizers[i].Item1;
				Menu.HoverText = Fertilizers[i].Item1?.DisplayName ?? "???";
				return;
			}
		}

		if (tabAgri.containsPoint(x, y)) {
			Menu.HoverText = I18n.Crop_Toggle(AgriculturistName());
			return;
		}

		if (tabPaddy.containsPoint(x, y)) {
			Menu.HoverText = I18n.Crop_Toggle(I18n.Crop_Paddy());
			return;
		}

		if (tabSeeds.containsPoint(x, y)) {
			Menu.HoverNode = SeedNode.Value;
			return;
		}
	}

	public override bool ReceiveLeftClick(int x, int y, bool playSound) {
		for (int i = 0; i < FertComponents.Count; i++) {
			ClickableComponent cmp = FertComponents[i];
			if (cmp.containsPoint(x, y)) {
				FertIndex = FertIndex == i ? -1 : i;
				Update();
				if (playSound)
					Game1.playSound("smallSelect");

				return true;
			}
		}

		if (tabAgri.containsPoint(x, y)) {
			Agriculturist = !Agriculturist;
			Update();
			if (playSound)
				Game1.playSound("smallSelect");

			return true;
		}

		if (tabPaddy.containsPoint(x, y)) {
			PaddyBonus = !PaddyBonus;
			Update();
			if (playSound)
				Game1.playSound("smallSelect");
		}

		if (tabSeeds.containsPoint(x, y)) {
			Seeds = CommonHelper.Cycle(Seeds);
			Update();
			if (playSound)
				Game1.playSound("smallSelect");
		}

		return base.ReceiveLeftClick(x, y, playSound);
	}

	public override bool ReceiveRightClick(int x, int y, bool playSound) {
		if (tabSeeds.containsPoint(x, y)) {
			Seeds = CommonHelper.Cycle(Seeds, -1);
			Update();
			if (playSound)
				Game1.playSound("smallSelect");
		}

		return base.ReceiveRightClick(x, y, playSound);
	}

	public override void Draw(SpriteBatch b) {
		base.Draw(b);

		for (int i = 0; i < FertComponents.Count; i++) {
			ClickableComponent cmp = FertComponents[i];
			SpriteInfo? sprite = Fertilizers[i].Item2;

			DrawTab(
				b,
				sprite,
				cmp.bounds.X,
				cmp.bounds.Y + (FertIndex == i ? -8 : 0),
				Fertilizers[i].Item4
			);
		}

		// Agriculturist Tab
		DrawTab(
			b,
			spriteAgri,
			tabAgri.bounds.X,
			tabAgri.bounds.Y + (Agriculturist ? -8 : 0),
			tabAgriSprite
		);

		// Paddy Tab
		DrawTab(
			b,
			spritePaddy,
			tabPaddy.bounds.X,
			tabPaddy.bounds.Y + (PaddyBonus ? -8 : 0),
			tabPaddySprite
		);

		// Owned Tab
		DrawTab(
			b,
			spriteSeeds,
			tabSeeds.bounds.X,
			tabSeeds.bounds.Y + (Seeds != SeedFilter.Disabled ? -8 : 0),
			tabSeedsSprite
		);
	}

	private void DrawTab(SpriteBatch b, SpriteInfo? sprite, int x, int y, int index) {
		bool reflect = false;
		if (index >= AlmanacMenu.TABS.Length) {
			index -= AlmanacMenu.TABS.Length;
			reflect = true;
		}

		bool upside_down = Game1.uiViewport.Height < 795;

		// Tab Background
		b.Draw(
			Menu.background,
			new Vector2(x, y),
			AlmanacMenu.TABS[0][index],
			Color.White,
			1 * (float) Math.PI / 2f,
			new Vector2(0, 16),
			4f,
			upside_down ? (reflect ? SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically : SpriteEffects.FlipHorizontally) : reflect ? SpriteEffects.FlipVertically : SpriteEffects.None,
			0.0001f
		);

		// Tab Texture
		sprite?.Draw(b, new Vector2(x + 16, y + 12), 2f);
	}

	#endregion

	#region ICalendarPage

	public bool ShouldDimPastCells => true;
	public bool ShouldHighlightToday => true;

	public void DrawUnderCell(SpriteBatch b, WorldDate date, Rectangle bounds) {
		SpriteInfo?[]? sprites = CropGrowth.Value;
		if (sprites != null) {
			SpriteInfo? sprite = sprites[date.DayOfMonth - 1];
			if (sprite == null)
				return;

			bool tall = sprite.BaseSource.Height > sprite.BaseSource.Width;
			int size = (tall ? 32 : 16) * 3;

			sprite.Draw(
				b,
				new Vector2(
					bounds.X + (bounds.Width - size) / 2,
					bounds.Y + (bounds.Height - size) - (tall ? 0 : 8)
				),
				tall ? 6f : 3f
			);

			return;
		}

		List<CropInfo>? crops = LastDays?[date.DayOfMonth - 1];
		if (crops == null)
			return;

		int row = 0;
		int col = 0;
		float scale = 2;

		int padX = 2;
		int padY = 4;

		int rows, cols;
		if (crops.Count <= 2) {
			rows = crops.Count;
			cols = 1;
		} else if (crops.Count <= 4) {
			rows = cols = 2;
		} else if (crops.Count <= 6) {
			cols = 2;
			rows = Math.Min(3, (int) Math.Ceiling(crops.Count / 2f));
			if (rows > 2)
				padY = 1;
			scale = 2;
		} else if (crops.Count <= 15) {
			cols = 3;
			rows = Math.Min(5, (int) Math.Ceiling(crops.Count / 3f));
			padX = 4;
			padY = 8;
			if (rows > 4)
				padY = 3;
			scale = 1;
		} else {
			cols = 4;
			rows = Math.Min(5, (int) Math.Ceiling(crops.Count / 4f));
			padY = 8;
			if (rows > 4)
				padY = 3;
			scale = 1;
		}

		int width = (int) (16 * scale) * cols + (cols > 1 ? (int) (padX * scale) * cols - 1 : 0);
		int height = (int) (16 * scale) * rows + (rows > 1 ? (int) (padY * scale) * rows - 1 : 0);

		int offsetX = bounds.X + (bounds.Width - width) / 2 + 2;
		int offsetY = bounds.Y + 22 + (bounds.Height - height - 22) / 2;

		foreach (CropInfo crop in crops) {
			if (crop.Sprite == null)
				continue;

			float sX = offsetX + ((16 + padX) * scale) * col;
			float sY = offsetY + ((16 + padY) * scale) * row;

			if (Menu.HoveredItem != null && Menu.HoveredItem.Equals(crop.Item)) {
				crop.Sprite.Draw(
					b,
					new Vector2(sX - 2, sY + 2),
					scale,
					baseColor: Color.Black,
					overlayColor: Color.Black,
					alpha: 0.35f
				);
			}

			crop.Sprite.Draw(
				b,
				new Vector2(sX, sY),
				scale
			);

			col++;
			if (col >= cols) {
				col = 0;
				row++;
				if (row >= rows)
					break;
			}
		}
	}

	public void DrawOverCell(SpriteBatch b, WorldDate date, Rectangle bounds) {

	}

	private CropInfo? GetCropAt(int x, int y, WorldDate date, Rectangle bounds) {
		List<CropInfo>? crops = LastDays?[date.DayOfMonth - 1];
		if (crops == null || crops.Count == 0)
			return null;

		int row = 0;
		int col = 0;
		float scale = 2;

		int padX = 2;
		int padY = 4;

		int rows, cols;
		if (crops.Count <= 2) {
			rows = crops.Count;
			cols = 1;
		} else if (crops.Count <= 4) {
			rows = cols = 2;
		} else if (crops.Count <= 6) {
			cols = 2;
			rows = Math.Min(3, (int) Math.Ceiling(crops.Count / 2f));
			if (rows > 2)
				padY = 1;
			scale = 2;
		} else if (crops.Count <= 15) {
			cols = 3;
			rows = Math.Min(5, (int) Math.Ceiling(crops.Count / 3f));
			padX = 4;
			padY = 8;
			if (rows > 4)
				padY = 3;
			scale = 1;
		} else {
			cols = 4;
			rows = Math.Min(5, (int) Math.Ceiling(crops.Count / 4f));
			padY = 8;
			if (rows > 4)
				padY = 3;
			scale = 1;
		}

		int width = (int) (16 * scale) * cols + (cols > 1 ? (int) (padX * scale) * cols - 1 : 0);
		int height = (int) (16 * scale) * rows + (rows > 1 ? (int) (padY * scale) * rows - 1 : 0);

		int offsetX = 2 + (bounds.Width - width) / 2;
		int offsetY = 22 + (bounds.Height - height - 24) / 2;

		foreach (CropInfo crop in crops) {
			int startX = offsetX + (int) ((16 + padX) * scale) * col;
			int startY = offsetY + (int) ((16 + padY) * scale) * row;

			int endX = startX + (int) (16 * scale);
			int endY = startY + (int) (16 * scale);

			if (x >= startX && x <= endX && y >= startY && y <= endY)
				return crop;

			col++;
			if (col >= cols) {
				col = 0;
				row++;
				if (row >= rows)
					break;
			}
		}

		return null;
	}

	public bool ReceiveCellLeftClick(int x, int y, WorldDate date, Rectangle bounds) {
		List<CropInfo>? crops = LastDays?[date.DayOfMonth - 1];
		if (crops == null || crops.Count == 0)
			return false;

		// If we're using gamepad controls, loop through every crop in the tile.
		if (Game1.options.gamepadControls && Game1.options.SnappyMenus) {
			if (date != ClickedDate) {
				ClickedDate = date;
				ClickedIndex = -1;
			}

			ClickedIndex++;
			if (ClickedIndex >= crops.Count)
				ClickedIndex = 0;

			if (CropNodes.TryGetValue(crops[ClickedIndex], out IFlowNode? node))
				if (Menu.ScrollRightFlow(node))
					Game1.playSound("shiny4");

			return true;
		}

		// If we're using mouse controls, determine which crop is at that exact
		// position and scroll there.
		CropInfo? crop = GetCropAt(x, y, date, bounds);
		if (crop.HasValue) {
			if (CropNodes.TryGetValue(crop.Value, out IFlowNode? node)) {
				if (Menu.ScrollRightFlow(node))
					Game1.playSound("shiny4");
			}

			return true;
		}

		return false;
	}

	public bool ReceiveCellRightClick(int x, int y, WorldDate date, Rectangle bounds) {
		return false;
	}

	public void PerformCellHover(int x, int y, WorldDate date, Rectangle bounds) {
		HoveredDate = date;
		Menu.HoverNode = CalendarTip.Value;

		CropInfo? crop = GetCropAt(x, y, date, bounds);
		if (crop.HasValue) {
			Menu.HoveredItem = crop.Value.Item;
		}
	}

	#endregion

}
