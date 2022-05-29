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

using Leclair.Stardew.Almanac.Menus;
using Leclair.Stardew.Common;
using Leclair.Stardew.Common.UI;
using Leclair.Stardew.Common.UI.FlowNode;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI.Utilities;

using StardewValley;

namespace Leclair.Stardew.Almanac.Pages;

public class WeatherPage : BasePage<BaseState>, ICalendarPage {

	public static readonly Rectangle WEATHER_ICON = new(384, 352, 16, 16);

	private readonly ulong Seed;
	private IFlowNode[]? Nodes;
	private int[]? Forecast;
	private bool[]? Festivals;
	private bool[]? Pirates;

	readonly bool IsIsland;

	private readonly SpriteInfo? FestivalFlag;

	#region Lifecycle

	public static WeatherPage? GetPage(AlmanacMenu menu, ModEntry mod) {
		if (!mod.Config.ShowWeather || !mod.Config.EnableDeterministicWeather)
			return null;

		return new(menu, mod, false);
	}

	public static WeatherPage? GetIslandPage(AlmanacMenu menu, ModEntry mod) {
		if (!mod.Config.ShowWeather || !mod.Config.EnableDeterministicWeather)
			return null;

		if (!mod.HasIsland(Game1.player))
			return null;

		return new(menu, mod, true);
	}

	public WeatherPage(AlmanacMenu menu, ModEntry mod, bool isIsland) : base(menu, mod) {
		Seed = Mod.GetBaseWorldSeed();
		IsIsland = isIsland;

		if (!IsIsland)
			FestivalFlag = new SpriteInfo(
				Game1.temporaryContent.Load<Texture2D>("LooseSprites\\Billboard"),
				new Rectangle(
					1, 398,
					84, 12
				),
				baseFrames: 6
			);

		Update();
	}

	#endregion

	#region Logic

	public override void Update() {
		base.Update();

		Forecast = new int[ModEntry.DaysPerMonth];
		Nodes = new IFlowNode[ModEntry.DaysPerMonth];
		Festivals = IsIsland ? null : new bool[ModEntry.DaysPerMonth];
		Pirates = IsIsland ? new bool[ModEntry.DaysPerMonth] : null;
		WorldDate date = new(Menu.Date);

		FlowBuilder builder = new();
		List<int>? pirateDays = IsIsland ? new() : null;

		int today = Game1.Date.TotalDays;
		int forecastLength = Mod.Config.WeatherForecastLength;

		if (!IsIsland)
			builder.FormatText(I18n.Festival_About(Utility.getSeasonNameFromNumber(date.SeasonIndex)));

		for (int day = 1; day <= ModEntry.DaysPerMonth; day++) {
			date.DayOfMonth = day;
			bool shown = forecastLength == -1 || date.TotalDays - today <= forecastLength;
			int weather = Forecast[day - 1] = shown ?
				Mod.Weather.GetWeatherForDate(Seed, date, IsIsland ? GameLocation.LocationContext.Island : GameLocation.LocationContext.Default)
				: -1;

			if (IsIsland) {
				bool pirates = Pirates![day - 1] = shown && day % 2 == 0 && ! WeatherHelper.IsRainOrSnow(weather);
				if (pirates)
					pirateDays!.Add(day);

			} else if ( Utility.isFestivalDay(day, date.Season)) {
				SDate sdate = new(day, date.Season);

				var data = Game1.temporaryContent.Load<Dictionary<string, string>>("Data\\Festivals\\" + date.Season + day);
				if (!data.ContainsKey("name") || !data.ContainsKey("conditions"))
					continue;

				Festivals![day - 1] = true;

				string name = data["name"];
				string[] conds = data["conditions"].Split('/');
				string? where = conds.Length >= 1 ? conds[0] : null;

				int start = -1;
				int end = -1;

				if (conds.Length >= 2) {
					string[] bits = conds[1].Split(' ');
					if (bits.Length >= 2) {
						start = Convert.ToInt32(bits[0]);
						end = Convert.ToInt32(bits[1]);
					}
				}

				foreach(GameLocation loc in Game1.locations) {
					if (loc?.Name == where) {
						where = Mod.GetLocationName(loc);
						break;
					}
				}

				var node = new TextNode(
					$"{name}\n",
					new TextStyle(
						font: Game1.dialogueFont,
						shadow: true
					),
					onClick: (_,_,_) => false
				);

				Nodes[day - 1] = node;

				builder
					.Text("\n\n")
					.Add(node)
					.FormatText($"  {I18n.Festival_Date()} ", shadow: false)
					.Text($"{sdate.ToLocaleString(withYear: false)}");

				if (!string.IsNullOrEmpty(where))
					builder
						.FormatText($"\n  {I18n.Festival_Where()} ", shadow: false)
						.Text(where);

				if (start >= 0 && end >= 0) {
					builder
						.FormatText($"\n  {I18n.Festival_When()} ", shadow: false)
						.Translate(Mod.Helper.Translation.Get("festival.when-times"), new {
							start = TimeHelper.FormatTime(start),
							end = TimeHelper.FormatTime(end)
						}, new TextStyle(shadow: false));
				}
			}
		}

		if (IsIsland) {
			builder.FormatText($"{I18n.Page_WeatherIsland()}\n\n", font: Game1.dialogueFont);

			if (pirateDays!.Count > 0) {
				string dates = string.Join(", ", pirateDays);

				builder.FormatText($"{I18n.Page_Weather_Pirates()}\n");

				foreach (int day in pirateDays) {
					SDate sdate = new(day, date.Season);
					builder.Text($"\n  {sdate.ToLocaleString(withYear: false)}", shadow: false);
				}
			}
		}

		SetRightFlow(builder, 2);
	}

	#endregion

	#region ITab

	public override int SortKey => IsIsland ? 1000 : 1;

	public override string? TabSimpleTooltip => IsIsland ?
		I18n.Page_WeatherIsland() : I18n.Page_Weather();

	public override Texture2D? TabTexture => IsIsland ?
		SpriteHelper.GetTexture(Common.Enums.GameTexture.MouseCursors2) : Menu.background;

	public override Rectangle? TabSource => IsIsland ?
		SpriteHelper.MouseIcons2.GOLDEN_NUT : WEATHER_ICON; // WeatherHelper.GetWeatherIcon(0, null);

	#endregion

	#region IAlmanacPage

	#endregion

	#region ICalendarPage

	public bool ShouldDimPastCells => true;
	public bool ShouldHighlightToday => true;

	public void DrawUnderCell(SpriteBatch b, WorldDate date, Rectangle bounds) {
		if (Forecast == null)
			return;

		int day = date.DayOfMonth - 1;

		if (Forecast[day] != -1)
			Utility.drawWithShadow(
				b,
				Menu.background,
				new Vector2(
					bounds.X + (bounds.Width - 64) / 2,
					bounds.Y + (bounds.Height - 64) / 2
				),
				WeatherHelper.GetWeatherIcon(Forecast[day], date.Season),
				Color.White,
				0f,
				Vector2.Zero,
				scale: 4f,
				horizontalShadowOffset: 0
			);

		if (Festivals != null && Festivals[day])
			FestivalFlag?.Draw(
				b,
				new Vector2(
					bounds.X + 4,
					bounds.Y + bounds.Height - 26
				),
				scale: 2,
				size: 14
			);

		if (Pirates != null && Pirates[day])
			b.Draw(
				SpriteHelper.GetTexture(Common.Enums.GameTexture.Hats),
				new Vector2(bounds.X + bounds.Width - 22, bounds.Y + bounds.Height - 22),
				new Rectangle(80, 480, 20, 20),
				color: Color.White,
				rotation: 0f,
				origin: Vector2.Zero,
				scale: 1f,
				effects: SpriteEffects.None,
				layerDepth: 1f
			);
	}

	public void DrawOverCell(SpriteBatch b, WorldDate date, Rectangle bounds) {

	}

	public bool ReceiveCellLeftClick(int x, int y, WorldDate date, Rectangle bounds) {
		int day = date.DayOfMonth;
		if (Nodes?[day - 1] is IFlowNode node && Menu.ScrollRightFlow(node)) {
			Game1.playSound("shiny4");
			return true;
		}

		return false;
	}

	public bool ReceiveCellRightClick(int x, int y, WorldDate date, Rectangle bounds) {
		return false;
	}

	public void PerformCellHover(int x, int y, WorldDate date, Rectangle bounds) {
		if (Forecast == null)
			return;

		int weather = Forecast[date.DayOfMonth - 1];
		if (weather == -1)
			return;

		Menu.HoverText = WeatherHelper.LocalizeWeather(weather);
	}

	#endregion

}
