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

using Leclair.Stardew.Almanac.Menus;
using Leclair.Stardew.Common;
using Leclair.Stardew.Common.UI;
using Leclair.Stardew.Common.UI.FlowNode;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI.Utilities;

using StardewValley;

namespace Leclair.Stardew.Almanac.Pages {
	public class WeatherPage : BasePage, ICalendarPage {

		private readonly int Seed;
		private int[] Forecast;

		readonly bool IsIsland;

		private IEnumerable<IFlowNode> Flow;

		#region Lifecycle

		public static WeatherPage GetPage(AlmanacMenu menu, ModEntry mod) {
			if (!mod.Config.ShowWeather || !mod.Config.EnableDeterministicWeather)
				return null;

			return new(menu, mod, false);
		}

		public static WeatherPage GetIslandPage(AlmanacMenu menu, ModEntry mod) {
			if (!mod.Config.ShowWeather || !mod.Config.EnableDeterministicWeather)
				return null;

			if (!mod.HasIsland(Game1.player))
				return null;

			return new(menu, mod, true);
		}

		public WeatherPage(AlmanacMenu menu, ModEntry mod, bool isIsland) : base(menu, mod) {
			Seed = Mod.GetBaseWorldSeed();
			IsIsland = isIsland;
			UpdateForecast();
		}

		#endregion

		#region Logic

		public void UpdateForecast() {
			Forecast = new int[WorldDate.DaysPerMonth];
			WorldDate date = new(Menu.Date);

			FlowBuilder builder = new();
			List<int> pirateDays = IsIsland ? new() : null;

			if (!IsIsland)
				builder.FormatText($"{I18n.Festival_About(Utility.getSeasonNameFromNumber(date.SeasonIndex))}\n\n");

			for (int day = 1; day <= WorldDate.DaysPerMonth; day++) {
				date.DayOfMonth = day;
				int weather = Forecast[day - 1] = Mod.Weather.GetWeatherForDate(Seed, date, IsIsland ? GameLocation.LocationContext.Island : GameLocation.LocationContext.Default);

				if (IsIsland && day % 2 == 0 && ! WeatherHelper.IsRainOrSnow(weather))
					pirateDays.Add(day);

				if (! IsIsland && Utility.isFestivalDay(day, date.Season)) {
					SDate sdate = new(day, date.Season);

					var data = Game1.temporaryContent.Load<Dictionary<string, string>>("Data\\Festivals\\" + date.Season + day);
					if (!data.ContainsKey("name") || !data.ContainsKey("conditions"))
						continue;

					string name = data["name"];
					string[] conds = data["conditions"].Split('/');
					string where = conds.Length >= 1 ? conds[0] : null;

					int start = -1;
					int end = -1;

					if (conds.Length >= 2) {
						string[] bits = conds[1].Split(' ');
						if (bits.Length >= 2) {
							start = Convert.ToInt32(bits[0]);
							end = Convert.ToInt32(bits[1]);
						}
					}

					builder.Text($"{name}\n", font: Game1.dialogueFont, shadow: true);
					builder.FormatText($"  {I18n.Festival_Date()} ", shadow: false);
					builder.Text($"{sdate.ToLocaleString(withYear: false)}\n");

					builder.FormatText($"  {I18n.Festival_Where()} ", shadow: false);
					builder.Text($"{where}\n");

					if (start >= 0 && end >= 0) {
						builder.FormatText($"  {I18n.Festival_When()} ", shadow: false);
						builder.Translate(Mod.Helper.Translation.Get("festival.when-times"), new {
							start = TimeHelper.FormatTime(start),
							end = TimeHelper.FormatTime(end)
						}, new TextStyle(shadow: false));
						builder.Text("\n\n");
					}
				}
			}

			if (IsIsland) {
				builder.Text($"Island Weather\n", font: Game1.dialogueFont, shadow: true);

				if (pirateDays.Count > 0) {
					builder.Text($"Pirate activity is expected near the island on the following dates: {string.Join(", ", pirateDays)}\n");
				}

			}

			Flow = builder?.Build();
			if (Active)
				Menu.SetFlow(Flow, 2);
		}

		#endregion

		#region ITab

		public override int SortKey => IsIsland ? 1000 : 1;

		public override string TabSimpleTooltip => IsIsland ?
			I18n.Page_WeatherIsland() : I18n.Page_Weather();

		public override Texture2D TabTexture => IsIsland ?
			SpriteHelper.GetTexture(Common.Enums.GameTexture.MouseCursors2) : Menu.background;

		public override Rectangle? TabSource => IsIsland ?
			SpriteHelper.MouseIcons2.GOLDEN_NUT : WeatherHelper.GetWeatherIcon(0, null);

		#endregion

		#region IAlmanacPage

		public override void Activate() {
			base.Activate();
			Menu.SetFlow(Flow, 2);
		}

		public override void DateChanged(WorldDate old, WorldDate newDate) {
			UpdateForecast();
		}

		#endregion

		#region ICalendarPage

		public bool ShouldDimPastCells => true;
		public bool ShouldHighlightToday => true;

		public void DrawUnderCell(SpriteBatch b, WorldDate date, Rectangle bounds) {
			if (Forecast == null)
				return;

			Utility.drawWithShadow(
				b,
				Menu.background,
				new Vector2(
					bounds.X + (bounds.Width - 64) / 2,
					bounds.Y + (bounds.Height - 64) / 2
				),
				WeatherHelper.GetWeatherIcon(Forecast[date.DayOfMonth - 1], date.Season),
				Color.White,
				0f,
				Vector2.Zero,
				scale: 4f,
				horizontalShadowOffset: 0
			);
		}

		public void DrawOverCell(SpriteBatch b, WorldDate date, Rectangle bounds) {

		}

		public bool ReceiveCellLeftClick(int x, int y, WorldDate date, Rectangle bounds) {
			return false;
		}

		public bool ReceiveCellRightClick(int x, int y, WorldDate date, Rectangle bounds) {
			return false;
		}

		public void PerformCellHover(int x, int y, WorldDate date, Rectangle bounds) {
			if (Forecast == null)
				return;

			int weather = Forecast[date.DayOfMonth - 1];
			Menu.HoverText = WeatherHelper.LocalizeWeather(weather);
		}

		#endregion

	}
}
