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
using System.Linq;
using System.Collections.Generic;

using Leclair.Stardew.Common;
using Leclair.Stardew.Common.Types;
using Leclair.Stardew.Common.UI;
using Leclair.Stardew.Common.UI.FlowNode;
using Leclair.Stardew.Common.UI.SimpleLayout;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI.Utilities;
using StardewValley;

using Leclair.Stardew.Almanac.Menus;

namespace Leclair.Stardew.Almanac.Pages;

public class NoticesPage : BasePage<BaseState>, ICalendarPage {

	private IFlowNode[]? Nodes;
	private List<NPC>[]? Birthdays;
	private SpriteInfo[]?[]? Sprites;

	private readonly Dictionary<NPC, SpriteInfo> Portraits = new();
	private readonly Dictionary<NPC, SpriteInfo> Heads = new();

	private Dictionary<string, Models.NPCOverride>? Overrides;

	//private WorldDate HoveredDate;
	//private Cache<ISimpleNode, WorldDate> CalendarTip;

	#region Lifecycle

	public static NoticesPage? GetPage(AlmanacMenu menu, ModEntry mod) {
		if (! mod.Config.ShowNotices)
			return null;

		return new(menu, mod);
	}

	public NoticesPage(AlmanacMenu menu, ModEntry mod) : base(menu, mod) {

		LoadOverrides();

	}

	public void LoadOverrides() {
		Overrides = Game1.content.Load<Dictionary<string, Models.NPCOverride>>(AssetManager.NPCOverridesPath);
	}

	#endregion

	#region Logic

	private SpriteInfo? GetPortrait(NPC? npc) {
		if (npc is null)
			return null;

		if (Portraits.TryGetValue(npc, out SpriteInfo? sprite))
			return sprite;

		Texture2D texture;
		try {
			texture = Game1.content.Load<Texture2D>(@"Characters\" + npc.getTextureName());
		} catch(Exception) {
			texture = npc.Sprite.Texture;
		}

		sprite = new SpriteInfo(
			texture,
			new Rectangle(0, 0, 16, 24)
		);

		Portraits[npc] = sprite;
		return sprite;
	}

	private SpriteInfo? GetHead(NPC? npc) {
		if (npc is null)
			return null;

		if (Heads.TryGetValue(npc, out SpriteInfo? sprite))
			return sprite;

		Texture2D texture;
		try {
			texture = Game1.content.Load<Texture2D>(@"Characters\" + npc.getTextureName());
		} catch (Exception) {
			texture = npc.Sprite.Texture;
		}

		Models.HeadSize? info = null;

		if (Overrides != null && Overrides.TryGetValue(npc.Name, out var ovr))
			info = ovr.Head;

		if (info == null)
			Mod.HeadSizes?.TryGetValue(npc.Name, out info);

		sprite = new SpriteInfo(
			texture,
			new Rectangle(
				info?.OffsetX ?? 0,
				info?.OffsetY ?? 0,
				info?.Width ?? 16,
				info?.Height ?? 15
			)
		);

		Heads[npc] = sprite;
		return sprite;
	}

	public override void Update() {
		base.Update();

		Nodes = new IFlowNode[ModEntry.DaysPerMonth];
		Birthdays = new List<NPC>[ModEntry.DaysPerMonth];
		Sprites = new SpriteInfo[ModEntry.DaysPerMonth][];
		WorldDate date = new(Menu.Date);

		// Build a map of this month's birthdays.
		foreach (NPC npc in Utility.getAllCharacters())
			if (npc.isVillager() && date.Season.Equals(npc.Birthday_Season)) {
				// Don't show villagers we can't socialize with.
				if (!npc.CanSocialize && !Game1.player.friendshipData.ContainsKey(npc.Name))
					continue;

				// Don't show forbidden villagers.
				if (Overrides != null && Overrides.TryGetValue(npc.Name, out var ovr) && !ovr.Visible)
					continue;

				int day = npc.Birthday_Day;
				if (Birthdays[day - 1] == null)
					Birthdays[day - 1] = new();

				Birthdays[day - 1].Add(npc);
			}

		FlowBuilder builder = new();

		builder.FormatText(
				I18n.Page_Notices(),
				fancy: true,
				align: Alignment.Center
			);

		for (int day = 1; day <= ModEntry.DaysPerMonth; day++) {

			FlowBuilder db = new();

			// Other Events
			date.DayOfMonth = day;
			List<SpriteInfo> sprites = new();

			foreach(var evt in Mod.Notices.GetEventsForDate(0, date)) {
				if (evt == null)
					continue;

				bool has_simple = ! string.IsNullOrEmpty(evt.SimpleLabel);
				bool has_line = has_simple || evt.AdvancedLabel != null;

				Func<IFlowNodeSlice, int, int, bool>? onHover = null;

				if (has_line) {
					db.Text("\n");
					if (evt.Item != null)
						onHover = (_,_,_) => {
							Menu.HoveredItem = evt.Item;
							return true;
						};
				}

				if (evt.Sprite != null) {
					sprites.Add(evt.Sprite);

					if (has_line)
						db
							.Sprite(evt.Sprite, 3f, onHover: onHover)
							.Text(" ");
				}

				if (evt.AdvancedLabel != null)
					db.AddRange(evt.AdvancedLabel);
				else if (has_simple)
					db.FormatText(
						evt.SimpleLabel!,
						align: Alignment.Middle,
						onHover: onHover,
						noComponent: true
					);
			}

			Sprites[day - 1] = sprites.Count > 0 ? sprites.ToArray() : null;

			// Birthdays
			List<NPC> birthdays = Birthdays[day - 1];
			if (birthdays != null) {
				foreach (NPC npc in birthdays) {
					char last = npc.displayName.Last<char>();

					bool no_s = last == 's' ||
						LocalizedContentManager.CurrentLanguageCode ==
						LocalizedContentManager.LanguageCode.de &&
							(last == 'x' || last == 'ß' || last == 'z');

					var name = new Common.UI.FlowNode.TextNode(
						npc.displayName,
						align: Alignment.Middle
					);

					db
						.Text("\n")
						.Sprite(GetHead(npc), 3f)
						.Text(" ")
						.Translate(
							Mod.Helper.Translation.Get(
								no_s ?
									"page.notices.birthday.no-s" :
									"page.notices.birthday.s"
							),
							new { name },
							align: Alignment.Middle
						);

					if (Mod.Config.DebugMode)
						db.Text($" (#{npc.Name})", align: Alignment.Middle);
				}
			}

			if (db.Count == 0)
				continue;

			SDate sdate = new(day, date.Season);

			var node = new Common.UI.FlowNode.TextNode(
				sdate.ToLocaleString(withYear: false),
				new TextStyle(
					font: Game1.dialogueFont
				)
			);

			Nodes[day - 1] = node;

			builder
				.Text("\n\n")
				.Add(node)
				.AddRange(db.Build());
		}

		SetRightFlow(
			builder.Build(),
			4,
			0
		);
	}

	#endregion

	#region ITab

	public override int SortKey => 11;
	public override string TabSimpleTooltip => I18n.Page_Notices();

	public override Texture2D TabTexture => Game1.mouseCursors;
	public override Rectangle? TabSource => new(208, 320, 16, 16);

	#endregion

	#region IAlmanacPage

	public override void Refresh() {
		LoadOverrides();
		Heads.Clear();
		Portraits.Clear();

		base.Refresh();
	}

	#endregion

	#region ICalendarPage

	public bool ShouldDimPastCells => true;
	public bool ShouldHighlightToday => true;

	public void DrawUnderCell(SpriteBatch b, WorldDate date, Rectangle bounds) {
		SpriteInfo[]? sprites = Sprites?[date.DayOfMonth - 1];
		List<NPC>? bdays = Birthdays?[date.DayOfMonth - 1];

		double ms = Game1.currentGameTime.TotalGameTime.TotalMilliseconds;

		if (sprites != null) {
			if (sprites.Length > 1 && sprites.Length < 4 && bdays == null) {
				for (int i = 0; i < sprites.Length; i++) {
					sprites[i]?.Draw(
						b,
						new Vector2(
							bounds.X + bounds.Width - 36,
							bounds.Y + 4 + (i * 36)
						),
						2f
					);
				}

			} else {
				int idx = (int) (ms / Mod.Config.CycleTime) % sprites.Length;

				SpriteInfo sprite = sprites[idx];
				sprite?.Draw(
					b,
					new Vector2(
						bounds.X + bounds.Width - 36,
						bounds.Y + 4
					),
					2f
				);
			}
		}

		if (bdays != null) {
			int idx = (int) (ms / Mod.Config.CycleTime) % bdays.Count;

			NPC? npc = bdays?[idx];
			SpriteInfo? sprite = GetPortrait(npc);
			sprite?.Draw(
				b,
				new Vector2(
					bounds.X + 4,
					bounds.Y + bounds.Height - 72
				),
				3f,
				new Vector2(16, 24)
			);
		}
	}

	public void DrawOverCell(SpriteBatch b, WorldDate date, Rectangle bounds) {
		
	}

	public bool ReceiveCellLeftClick(int x, int y, WorldDate date, Rectangle bounds) {
		int day = date.DayOfMonth - 1;

		while (day >= 0) {
			// Do we have something click-worthy?
			if ((Sprites?[day]?.Length ?? 0) == 0 && (Birthdays?[day]?.Count ?? 0) == 0)
				return false;

			if (Nodes?[day] is IFlowNode node) {
				if (Menu.ScrollRightFlow(node))
					Game1.playSound("shiny4");
				return true;
			}

			day--;
		}

		return false;
	}

	public bool ReceiveCellRightClick(int x, int y, WorldDate date, Rectangle bounds) {
		return false;
	}

	public void PerformCellHover(int x, int y, WorldDate date, Rectangle bounds) {
		//HoveredDate = date;
		//Menu.HoverNode = CalendarTip.Value;
	}

	#endregion

}
