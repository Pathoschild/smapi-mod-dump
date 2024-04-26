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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI.Utilities;
using StardewValley;

using Leclair.Stardew.Almanac.Menus;
using Leclair.Stardew.Almanac.Models;

namespace Leclair.Stardew.Almanac.Pages;

public class FortunePage : BasePage<BaseState>, ICalendarPage {

	public static readonly Rectangle CRYSTAL_BALL = new(272, 352, 16, 16);

	private readonly ulong Seed;
	private double?[]? Luck;
	private SpriteInfo?[]? Sprites;
	private SpriteInfo[]?[]? Extras;
	private IFlowNode[]? Nodes;

	private readonly bool HasLuck;

	#region Lifecycle

	public static FortunePage? GetPage(AlmanacMenu menu, ModEntry mod) {
		if (!mod.Config.ShowFortunes)
			return null;

		if (!mod.HasMagic(Game1.player))
			return null;

		return new(menu, mod);
	}

	public FortunePage(AlmanacMenu menu, ModEntry mod) : base(menu, mod) {
		Seed = Mod.GetBaseWorldSeed();
		HasLuck = Mod.Config.EnableDeterministicLuck;

		Update();
	}

	#endregion

	#region Logic

	public override void Update() {
		base.Update();

		Luck = HasLuck ? new double?[ModEntry.DaysPerMonth] : null;
		Sprites = HasLuck ? new SpriteInfo[ModEntry.DaysPerMonth] : null;
		Extras = new SpriteInfo[ModEntry.DaysPerMonth][];
		Nodes = new IFlowNode[ModEntry.DaysPerMonth];
		WorldDate date = new(Menu.Date);

		FlowBuilder builder = new();

		builder.FormatText(
			I18n.Page_Fortune_About(Utility.getSeasonNameFromNumber(date.SeasonIndex))
		);

		bool had_event = false;

		int today = Game1.Date.TotalDays;
		int forecastLength = Mod.Config.LuckForecastLength;

		for (int day = 1; day <= ModEntry.DaysPerMonth; day++) {
			date.DayOfMonth = day;
			if (forecastLength != -1 && date.TotalDays - today > forecastLength) {
				if (HasLuck)
					Luck![day - 1] = null;
				continue;
			}

			SpriteInfo? sprite;
			if (HasLuck) {
				double luck = Mod.Luck.GetModifiedLuckForDate(Seed, date);
				sprite = Sprites![day - 1] = Mod.Luck.GetLuckSprite(luck);
				Luck![day - 1] = luck;
			} else
				sprite = null;

			FlowBuilder db = new();
			List<SpriteInfo> extra = new();

			foreach (var evt in Mod.Luck.GetEventsForDate(Seed, date)) {
				if (evt == null)
					continue;

				bool has_simple = !string.IsNullOrEmpty(evt.SimpleLabel);
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

				if (evt.Sprite != null)
					extra.Add(evt.Sprite);

				if (evt.AdvancedLabel != null)
					db.AddRange(evt.AdvancedLabel);
				else if (has_simple)
					db.FormatText(evt.SimpleLabel!, align: Alignment.VCenter, onHover: onHover);
			}

			Extras[day - 1] = extra.Count > 0 ? extra.ToArray() : null;

			if (db.Count == 0)
				continue;

			had_event = true;

			SDate sdate = new(day, date.Season);

			IFlowNode node = new SpriteNode(sprite, 3, Alignment.VCenter, size: 13);
			Nodes[day - 1] = node;

			builder
				.Text("\n\n")
				.Add(node)
				.Text(" ")
				.Text(sdate.ToLocaleString(withYear: false), font: Game1.dialogueFont)
				.AddRange(db);
		}

		if (!had_event)
			builder
				.Text("\n\n")
				.FormatText(I18n.Page_Fortune_Event_None());

		SetRightFlow(builder, 2);
	}

	#endregion

	#region ITab

	public override int SortKey => 50;
	public override string TabSimpleTooltip => I18n.Page_Fortune();

	public override Texture2D TabTexture => Menu.background;

	public override Rectangle? TabSource => CRYSTAL_BALL;

	#endregion

	#region IAlmanacPage

	public override bool IsMagic => true;

	#endregion

	#region ICalendarPage

	public bool ShouldDimPastCells => true;
	public bool ShouldHighlightToday => true;

	public void DrawUnderCell(SpriteBatch b, WorldDate date, Rectangle bounds) {
		int day = date.DayOfMonth;
		SpriteInfo? sprite = Sprites?[day - 1];
		SpriteInfo[]? extras = Extras?[day - 1];

		if (sprite == null && extras == null)
			return;

		sprite?.Draw(
			b,
			new Vector2(
				bounds.Center.X - 39 / 2,
				bounds.Center.Y - 39 / 2 - (extras != null ? 16 : 0)
			),
			3,
			size: 13
		);

		if (extras == null)
			return;

		Vector2 pos;
		if (sprite == null)
			pos = new(bounds.Center.X - 48 / 2, bounds.Center.Y - 33 / 2);
		else
			pos = new(bounds.Right - 40, bounds.Bottom - 40);

		int idx = (int) (
				Game1.currentGameTime.TotalGameTime.TotalMilliseconds
				% (Mod.Config.CycleTime * extras.Length)) / Mod.Config.CycleTime;

		SpriteInfo extra = extras[idx];
		extra?.Draw(b, pos, sprite == null ? 3f : 2f);
	}

	public void DrawOverCell(SpriteBatch b, WorldDate date, Rectangle bounds) {
		
	}

	public bool ReceiveCellLeftClick(int x, int y, WorldDate date, Rectangle bounds) {
		IFlowNode? node = Nodes?[date.DayOfMonth - 1];
		if (node != null) {
			if (Menu.ScrollRightFlow(node)) {
				Game1.playSound("shiny4");
				return true;
			}
		}

		return false;
	}

	public bool ReceiveCellRightClick(int x, int y, WorldDate date, Rectangle bounds) {
		return false;
	}

	public void PerformCellHover(int x, int y, WorldDate date, Rectangle bounds) {
		if (Luck == null || ! Luck[date.DayOfMonth - 1].HasValue)
			return;

		double luck = Luck[date.DayOfMonth - 1]!.Value;
		string fortune = Mod.Luck.GetLuckText(luck);

		Menu.HoverMagic = true;
		Menu.HoverText = Mod.Config.ShowExactLuck
			? $"{fortune} ({(luck*100):F1}%)"
			: fortune;
	}

	#endregion

}
