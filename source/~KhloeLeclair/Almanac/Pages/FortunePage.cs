/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System.Collections.Generic;

using Leclair.Stardew.Common;
using Leclair.Stardew.Common.UI;
using Leclair.Stardew.Common.UI.FlowNode;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI.Utilities;
using StardewValley;

using Leclair.Stardew.Almanac.Menus;

namespace Leclair.Stardew.Almanac.Pages {
	public class FortunePage : BasePage, ICalendarPage {

		public static readonly Rectangle CRYSTAL_BALL = new(272, 352, 16, 16);

		private readonly int Seed;
		private double[] Luck;
		private SpriteInfo[] Sprites;
		private SpriteInfo[] Extras;
		private IFlowNode[] Nodes;

		private IEnumerable<IFlowNode> Flow;

		private readonly bool HasLuck;

		#region Lifecycle

		public static FortunePage GetPage(AlmanacMenu menu, ModEntry mod) {
			if (!mod.Config.ShowFortunes)
				return null;

			if (!mod.HasMagic(Game1.player))
				return null;

			return new(menu, mod);
		}

		public FortunePage(AlmanacMenu menu, ModEntry mod) : base(menu, mod) {
			Seed = Mod.GetBaseWorldSeed();
			HasLuck = Mod.Config.EnableDeterministicLuck;

			UpdateLuck();
		}

		#endregion

		#region Logic

		public void UpdateLuck() {
			Luck = HasLuck ? new double[WorldDate.DaysPerMonth] : null;
			Sprites = HasLuck ? new SpriteInfo[WorldDate.DaysPerMonth] : null;
			Extras = new SpriteInfo[WorldDate.DaysPerMonth];
			Nodes = new IFlowNode[WorldDate.DaysPerMonth];
			WorldDate date = new(Menu.Date);

			FlowBuilder builder = new();

			builder.FormatText(
				I18n.Page_Fortune_About(Utility.getSeasonNameFromNumber(date.SeasonIndex))
			);

			bool had_event = false;

			for (int day = 1; day <= WorldDate.DaysPerMonth; day++) {
				date.DayOfMonth = day;
				SpriteInfo sprite;
				if (HasLuck) {
					double luck = LuckHelper.GetLuckForDate(Seed, date);
					sprite = Sprites[day - 1] = LuckHelper.GetLuckSprite(luck);
					Luck[day - 1] = luck;
				} else
					sprite = null;

				LuckHelper.IHoroscopeEvent evt = LuckHelper.GetEventForDate(Seed, date);
				if ( evt == null )
					evt = LuckHelper.GetTrashEvent(Seed, date);

				if ( evt == null )
					continue;

				had_event = true;

				SDate sdate = new(day, date.Season);

				IFlowNode node = new SpriteNode(sprite, 3, Alignment.Bottom, size: 13);
				Nodes[day - 1] = node;

				builder
					.Text("\n\n")
					.Add(node)
					.Text($" {sdate.ToLocaleString(withYear: false)}\n", font: Game1.dialogueFont);

				builder.Text("  ");

				if (evt.Sprite != null)
					Extras[day - 1] = evt.Sprite;

				if (evt.AdvancedLabel != null)
					builder.AddRange(evt.AdvancedLabel);
				else if (!string.IsNullOrEmpty(evt.SimpleLabel))
					builder.FormatText(evt.SimpleLabel);
			}

			if (!had_event)
				builder.FormatText($"\n\n{I18n.Page_Fortune_Event_None()}");

			Flow = builder.Build();
			if (Active)
				Menu.SetFlow(Flow, 2);
		}

		#endregion

		#region ITab

		public override int SortKey => 10;
		public override string TabSimpleTooltip => I18n.Page_Fortune();

		public override Texture2D TabTexture => Menu.background;

		public override Rectangle? TabSource => CRYSTAL_BALL;

		#endregion

		#region IAlmanacPage

		public override bool IsMagic => true;

		public override void Activate() {
			base.Activate();
			Menu.SetFlow(Flow, 2);
		}

		public override void DateChanged(WorldDate oldDate, WorldDate newDate) {
			UpdateLuck();
		}

		#endregion

		#region ICalendarPage

		public bool ShouldDimPastCells => true;
		public bool ShouldHighlightToday => true;

		public void DrawUnderCell(SpriteBatch b, WorldDate date, Rectangle bounds) {
			int day = date.DayOfMonth;
			SpriteInfo sprite = Sprites?[day - 1];
			SpriteInfo extra = Extras?[day - 1];

			if (sprite == null && extra == null)
				return;

			sprite?.Draw(
				b,
				new Vector2(
					bounds.Center.X - 39 / 2,
					bounds.Center.Y - 39 / 2 - (extra != null ? 16 : 0)
				),
				3,
				size: 13
			);

			if (extra == null)
				return;

			if (sprite == null)
				extra.Draw(
					b,
					new Vector2(
						bounds.Center.X - 48 / 2,
						bounds.Center.Y - 33 / 2
					),
					3,
					size: 16
				);
			else
				extra.Draw(
					b,
					new Vector2(
						bounds.Right - 40,
						bounds.Bottom - 40
					),
					2
				);
		}

		public void DrawOverCell(SpriteBatch b, WorldDate date, Rectangle bounds) {
			
		}

		public bool ReceiveCellLeftClick(int x, int y, WorldDate date, Rectangle bounds) {
			IFlowNode node = Nodes?[date.DayOfMonth - 1];
			if (node != null) {
				if (Menu.ScrollFlow(node)) {
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
			if (Luck == null)
				return;

			double luck = Luck[date.DayOfMonth - 1];
			string fortune = LuckHelper.GetLuckText(luck);

			Menu.HoverMagic = true;
			Menu.HoverText = Mod.Config.ShowExactLuck
				? $"{fortune} ({(luck*100):F1}%)"
				: fortune;
		}

		#endregion

	}
}
