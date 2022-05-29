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

public class TrainPage : BasePage<BaseState>, ICalendarPage {

	private int[]? Schedule;

	#region Lifecycle

	public static TrainPage? GetPage(AlmanacMenu menu, ModEntry mod) {
		if (!mod.Config.ShowTrains)
			return null;

		return new(menu, mod);
	}

	public TrainPage(AlmanacMenu menu, ModEntry mod) : base(menu, mod) {
		Update();
	}

	#endregion

	#region Logic

	public override void Update() {
		base.Update();

		Schedule = new int[ModEntry.DaysPerMonth];
		WorldDate date = new(Menu.Date);

		FlowBuilder builder = new();

		builder.FormatText(I18n.Page_Train_About());

		for (int day = 1; day <= ModEntry.DaysPerMonth; day++) {
			date.DayOfMonth = day;
			int time = Schedule[day - 1] = TrainHelper.GetTrainTime(date);

			if (time >= 0) {
				SDate sdate = new(day, date.Season);

				builder.Text("\n\n");
				builder.Text($"{sdate.ToLocaleString(withYear: false)}\n", font: Game1.dialogueFont);
				builder.Text($"  {TimeHelper.FormatTime(time)}");
			}
		}

		if (date.SeasonIndex == 0 && date.Year == 1)
			builder.FormatText($"\n\n{I18n.Page_Train_Notice()}", color: Color.Red);

		SetRightFlow(builder, 2);
	}

	#endregion

	#region ITab

	public override int SortKey => 5;
	public override string TabSimpleTooltip => I18n.Page_Train();
	public override Texture2D TabTexture => Game1.mouseCursors;
	public override Rectangle? TabSource => TrainHelper.TRAIN;
	public override float? TabScale => 0.4f;

	#endregion

	#region IAlmanacPage

	#endregion

	#region ICalendarPage

	public bool ShouldDimPastCells => true;
	public bool ShouldHighlightToday => true;

	public void DrawUnderCell(SpriteBatch b, WorldDate date, Rectangle bounds) {
		if (Schedule == null)
			return;

		int time = Schedule[date.DayOfMonth - 1];
		if (time < 0)
			return;

		Utility.drawWithShadow(
			b,
			Game1.mouseCursors,
			new Vector2(
				bounds.X + (bounds.Width - (TrainHelper.TRAIN.Width / 2)) / 2,
				bounds.Y + (bounds.Height - (TrainHelper.TRAIN.Height / 2)) / 2
			),
			TrainHelper.TRAIN,
			Color.White,
			0f,
			Vector2.Zero,
			scale: 0.5f
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
		if (Schedule == null)
			return;

		int time = Schedule[date.DayOfMonth - 1];
		if (time < 0)
			return;

		Menu.HoverText = TimeHelper.FormatTime(time);
	}

	#endregion

}
