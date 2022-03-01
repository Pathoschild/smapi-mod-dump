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
	public class MinesPage : BasePage, ICalendarPage {

		private readonly int Seed;

		private Dictionary<LevelType, SpriteInfo> Sprites;
		private Dictionary<LevelType, List<int>>[] Levels;
		private IFlowNode[] Nodes;

		private IEnumerable<IFlowNode> Flow;

		#region Lifecycle

		public static MinesPage GetPage(AlmanacMenu menu, ModEntry mod) {
			if (!mod.HasMagic(Game1.player))
				return null;

			return new(menu, mod);
		}

		public MinesPage(AlmanacMenu menu, ModEntry mod) : base(menu, mod) {
			Seed = Mod.GetBaseWorldSeed();

			Sprites = new();

			Sprites[LevelType.Mushroom] = SpriteHelper.GetSprite(
				new Object(420, 1), // Red Mushroom
				Mod.Helper
			);

			Sprites[LevelType.InfestedMonster] = SpriteHelper.GetSprite(
				new StardewValley.Tools.MeleeWeapon(0), // Rusty Sword
				//new Object(109, 1), // Ancient Sword
				Mod.Helper
			);

			Sprites[LevelType.InfestedSlime] = SpriteHelper.GetSprite(
				new Object(766, 1), // Slime
				Mod.Helper
			);

			Sprites[LevelType.Quarry] = new SpriteInfo(
				Mod.Helper.Content.Load<Texture2D>("Characters/Monsters/Haunted Skull", StardewModdingAPI.ContentSource.GameContent),
				new Rectangle(0, 0, 16, 16)
			);

			Sprites[LevelType.InfestedQuarry] = new SpriteInfo(
				Mod.Helper.Content.Load<Texture2D>("Characters/Monsters/Haunted Skull", StardewModdingAPI.ContentSource.GameContent),
				new Rectangle(0, 0, 16, 16)
			);

			Sprites[LevelType.Dino] = SpriteHelper.GetSprite(
				new Object(107, 1), // Dino Egg
				Mod.Helper
			);

			UpdateLevels();
		}

		#endregion

		#region Logic

		public void UpdateLevels() {
			Levels = new Dictionary<LevelType, List<int>>[WorldDate.DaysPerMonth];
			Nodes = new IFlowNode[WorldDate.DaysPerMonth];
			WorldDate date = new(Menu.Date);

			FlowBuilder builder = new();

			builder.FormatText(
				I18n.Page_Mines_About(Utility.getSeasonNameFromNumber(date.SeasonIndex))
			);

			for (int day = 1; day <= WorldDate.DaysPerMonth; day++) {
				date.DayOfMonth = day;
				int days = date.TotalDays;
				Levels[day - 1] = new();

				// Normal Mines
				for(int floor = 1; floor <= 120; floor++) {
					LevelType type = MineHelper.GetLevelType(
						floor: floor,
						seed: Seed,
						date: days
					);

					if (type == LevelType.None)
						continue;

					if (! Levels[day-1].ContainsKey(type))
						Levels[day-1].Add(type, new List<int>());

					Levels[day - 1][type].Add(floor);
				}

				// Dinosaurs
				for (int floor = 121; floor <= 600; floor++) {
					LevelType type = MineHelper.GetLevelType(
						floor: floor,
						seed: Seed,
						date: days
					);

					if (type != LevelType.Dino)
						continue;

					if (!Levels[day - 1].ContainsKey(type))
						Levels[day - 1].Add(type, new List<int>());

					Levels[day - 1][type].Add(floor - 120);
				}

				SDate sdate = new(day, date.Season);

				builder.Text("\n\n");

				var node = new TextNode(
					$"{sdate.ToLocaleString(withYear: false)}\n",
					new TextStyle(font: Game1.dialogueFont),
					onClick: slice => false
				);

				Nodes[day - 1] = node;
				builder.Add(node);

				bool fline = true;

				foreach (var entry in Levels[day - 1]) {
					string key = $"page.mines.type.{entry.Key}";
					string floors = string.Join(", ", entry.Value);

					if (!fline)
						builder.Text("\n");
					else
						fline = false;

					if (Sprites != null && Sprites.TryGetValue(entry.Key, out SpriteInfo sprite))
						builder.Sprite(Sprites[entry.Key], 2);

					builder
						.FormatText($" {Mod.Helper.Translation.Get(key)}: ", color: Color.White * 0.5f);

					builder.Text(
						string.Join(", ", entry.Value),
						color: Color.White * 0.75f
					);

					/*bool first = true;
					foreach (int floor in entry.Value) {
						if (!first)
							builder.Text(", ");
						else
							first = false;

						builder.Add(new TextNode($"{floor}", onClick: slice => {
							Game1.enterMine(floor + (entry.Key == LevelType.Dino ? 120 : 0));
							return true;
						}));
					}*/
				}
			}

			Flow = builder.Build();
			if (Active)
				Menu.SetFlow(Flow, 4);
		}

		#endregion

		#region ITab

		public override int SortKey => 11;
		public override string TabSimpleTooltip => I18n.Page_Mines();
		public override Texture2D TabTexture =>
			SpriteHelper.GetTexture(Common.Enums.GameTexture.MouseCursors);
		public override Rectangle? TabSource => new(30, 428, 10, 10);

		#endregion

		#region IAlmanacPage

		public override bool IsMagic => true;

		public override void Activate() {
			base.Activate();
			Menu.SetFlow(Flow, 4);
		}

		public override void DateChanged(WorldDate oldDate, WorldDate newDate) {
			UpdateLevels();
		}

		#endregion

		#region ICalendarPage

		public bool ShouldDimPastCells => true;
		public bool ShouldHighlightToday => true;

		public void DrawUnderCell(SpriteBatch b, WorldDate date, Rectangle bounds) {
			if (Levels?[date.DayOfMonth - 1] is not Dictionary<LevelType, List<int>> data)
				return;

			int x = bounds.X + 4;
			int y = bounds.Y + 20 + 4;

			foreach(var entry in data) {
				Sprites.TryGetValue(entry.Key, out SpriteInfo sprite);
				if (sprite == null)
					continue;

				if (entry.Key == LevelType.Dino)
					y = bounds.Y + bounds.Height - (16 + 4);

				sprite.Draw(b, new Vector2(x, y), 1);

				Utility.drawTinyDigits(entry.Value[0], b, new Vector2(x + 16 + 4, y), 2, 1, Color.White);

				y += 16 + 4;
				if ((y + 16 + 4) > (bounds.Y + bounds.Height))
					break;
			}
		}

		public void DrawOverCell(SpriteBatch b, WorldDate date, Rectangle bounds) {
			

		}

		public bool ReceiveCellLeftClick(int x, int y, WorldDate date, Rectangle bounds) {
			int day = date.DayOfMonth;
			if (Nodes?[day - 1] is IFlowNode node && Menu.ScrollFlow(node)) {
				Game1.playSound("smallSelect");
				return true;
			}

			return false;
		}

		public bool ReceiveCellRightClick(int x, int y, WorldDate date, Rectangle bounds) {
			return false;
		}

		public void PerformCellHover(int x, int y, WorldDate date, Rectangle bounds) {
			
		}

		#endregion

	}
}
