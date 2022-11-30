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

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using Leclair.Stardew.Common;
using Leclair.Stardew.Common.Types;
using Leclair.Stardew.Common.UI;
using Leclair.Stardew.Common.UI.FlowNode;

using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;

using Leclair.Stardew.Almanac.Menus;
using Leclair.Stardew.Almanac.Pages;

namespace Leclair.Stardew.Almanac.Fish;

public class FishingState : BaseState {
	public string? Fish;
	public FishWeather Weather = FishWeather.None;
	public FishType FType = FishType.None;
	public CaughtStatus CStatus = CaughtStatus.None;
	public LocationFilter LFilter = LocationFilter.Off;
	public AquariumFilter AFilter = AquariumFilter.Off;
}

public enum LocationFilter {
	Off,
	On,
	Now
};

public enum AquariumFilter {
	Off,
	Donated,
	Undonated
};

public class FishingPage : BasePage<FishingState>, ILeftFlowMargins {

	public static readonly Rectangle FISHING_ICON = new(20, 428, 10, 10);

	public static readonly Rectangle WEATHER_NONE = new(352, 352, 16, 16);
	public static readonly Rectangle WEATHER_RAINY = new(352 + 16, 352, 16, 16);
	public static readonly Rectangle WEATHER_SUNNY = new(352 + 32, 352, 16, 16);
	public static readonly Rectangle WEATHER_ANY = new(352 + 48, 352, 16, 16);

	public static readonly Rectangle SOURCE_NONE = new(480, 352, 16, 16);
	public static readonly Rectangle SOURCE_CATCH = new(448, 352, 16, 16);
	public static readonly Rectangle SOURCE_TRAP = new(464, 352, 16, 16);

	public static readonly Rectangle COLLECTED_NONE = new(464, 304, 16, 16);
	public static readonly Rectangle COLLECTED_TRUE = new(464, 320, 16, 16);
	public static readonly Rectangle COLLECTED_FALSE = new(464, 336, 16, 16);

	public static readonly Rectangle LOCATION_FALSE = new(464, 272, 16, 16);
	public static readonly Rectangle LOCATION_TRUE = new(464, 288, 16, 16);
	public static readonly Rectangle LOCATION_NOW = new(464, 256, 16, 16);

	public static readonly Rectangle AQUARIUM_OFF = new(496, 352, 16, 16);
	public static readonly Rectangle AQUARIUM_DONATED = new(496, 320, 16, 16);
	public static readonly Rectangle AQUARIUM_UNDONATED = new(496, 336, 16, 16);

	private readonly MenuFishTank Tank;

	[SkipForClickableAggregation]
	public ClickableComponent tankComponent;

	public ClickableTextureComponent btnFilterWeather;
	public ClickableTextureComponent btnFilterType;
	public ClickableTextureComponent btnFilterCaught;
	public ClickableTextureComponent btnFilterLocation;
	public ClickableTextureComponent? btnFilterAquarium;

	public readonly bool HasAquarium;

	private FishInfo? CurrentFish;

	private readonly Dictionary<string, SelectableNode> FishNodes = new();
	private readonly Cache<IEnumerable<IFlowNode>?, FishInfo?> FishFlow;

	// Filters
	public FishWeather Weather = FishWeather.None;
	public FishType FType = FishType.None;
	public CaughtStatus CStatus = CaughtStatus.None;
	public LocationFilter LFilter = LocationFilter.Off;
	public AquariumFilter AFilter = AquariumFilter.Off;

	// Sprites
	public Rectangle SourceWeather => Weather switch {
		FishWeather.None => WEATHER_NONE,
		FishWeather.Any => WEATHER_ANY,
		FishWeather.Rainy => WEATHER_RAINY,
		FishWeather.Sunny => WEATHER_SUNNY,
		_ => Rectangle.Empty
	};

	public Rectangle SourceType => FType switch {
		FishType.None => SOURCE_NONE,
		FishType.Catch => SOURCE_CATCH,
		FishType.Trap => SOURCE_TRAP,
		_ => Rectangle.Empty
	};

	public Rectangle SourceCaught => CStatus switch {
		CaughtStatus.None => COLLECTED_NONE,
		CaughtStatus.Caught => COLLECTED_TRUE,
		CaughtStatus.Uncaught => COLLECTED_FALSE,
		_ => Rectangle.Empty
	};

	public Rectangle SourceLocation => LFilter switch {
		LocationFilter.Off => LOCATION_FALSE,
		LocationFilter.On => LOCATION_TRUE,
		LocationFilter.Now => LOCATION_NOW,
		_ => Rectangle.Empty
	};

	public Rectangle SourceAquarium => AFilter switch {
		AquariumFilter.Off => AQUARIUM_OFF,
		AquariumFilter.Donated => AQUARIUM_DONATED,
		AquariumFilter.Undonated => AQUARIUM_UNDONATED,
		_ => Rectangle.Empty
	};

	#region Life Cycle

	public static FishingPage? GetPage(AlmanacMenu menu, ModEntry mod) {
		if (!mod.HasAlmanac(Game1.player) || !mod.Config.ShowFishing)
			return null;

		return new(menu, mod);
	}

	public FishingPage(AlmanacMenu menu, ModEntry mod) : base(menu, mod) {
		// Set up the cache for rendering the right page flow.
		FishFlow = new(_info => BuildRightPage(_info), () => CurrentFish);

		HasAquarium = Mod.Helper.ModRegistry.IsLoaded("Cherry.StardewAquarium");

		// Initialize our tank.
		Tank = new(Rectangle.Empty) {
			FloorTexture = Menu.background,
			FloorSource = new Rectangle(432, 352, 16, 16),

			GlassTexture = Game1.uncoloredMenuTexture,
			GlassSource = MenuFishTank.BG_SOURCE,
			GlassColor = new Color(0x30, 0x96, 0xd0) * .41f,

			FrameTexture = Menu.background,
			FrameSource = new Rectangle(416, 352, 16, 16)
		};

		tankComponent = new(
			new Rectangle(0, 0, 256, 192),
			name: ""
		);

		UpdateTank();

		// And our buttons.
		btnFilterWeather = new ClickableTextureComponent(
			new Rectangle(0, 0, 64, 64),
			Menu.background,
			new Rectangle(336, 352, 16, 16),
			4f
		) {
			myID = 1,
			upNeighborID = ClickableComponent.SNAP_AUTOMATIC,
			downNeighborID = 2,
			leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
			rightNeighborID = ClickableComponent.SNAP_AUTOMATIC,
		};

		btnFilterType = new ClickableTextureComponent(
			new Rectangle(0, 0, 64, 64),
			Menu.background,
			new Rectangle(336, 352, 16, 16),
			4f
		) {
			myID = 2,
			upNeighborID = 1,
			downNeighborID = 3,
			leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
			rightNeighborID = ClickableComponent.SNAP_AUTOMATIC,
		};

		btnFilterCaught = new ClickableTextureComponent(
			new Rectangle(0, 0, 64, 64),
			Menu.background,
			new Rectangle(336, 352, 16, 16),
			4f
		) {
			myID = 3,
			upNeighborID = 2,
			downNeighborID = 4,
			leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
			rightNeighborID = ClickableComponent.SNAP_AUTOMATIC,
		};

		btnFilterLocation = new ClickableTextureComponent(
			new Rectangle(0, 0, 64, 64),
			Menu.background,
			new Rectangle(336, 352, 16, 16),
			4f
		) {
			myID = 4,
			upNeighborID = 3,
			downNeighborID = 5,
			leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
			rightNeighborID = ClickableComponent.SNAP_AUTOMATIC,
		};

		btnFilterAquarium = HasAquarium ? new ClickableTextureComponent(
			new Rectangle(0, 0, 64, 64),
			Menu.background,
			new Rectangle(336, 352, 16, 16),
			4f
		) {
			myID = 5,
			upNeighborID = 4,
			downNeighborID = ClickableComponent.SNAP_AUTOMATIC,
			leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
			rightNeighborID = ClickableComponent.SNAP_AUTOMATIC,
		} : null;
	}

	public override void ThemeChanged() {
		base.ThemeChanged();

		btnFilterWeather.texture = Menu.background;
		btnFilterType.texture = Menu.background;
		btnFilterCaught.texture = Menu.background;
		btnFilterLocation.texture = Menu.background;

		if (btnFilterAquarium != null)
			btnFilterAquarium.texture = Menu.background;

		Tank.FloorTexture = Menu.background;
		Tank.FrameTexture = Menu.background;

		var flow = GetLeftFlow();
		if (flow != null)
			foreach(var node in flow)
				if (node is SelectableNode sn) {
					sn.SelectedTexture = Menu.background;
					sn.HoverTexture = Menu.background;
				}
	}

	#endregion

	#region Logic

	public override FishingState SaveState() {
		var state = base.SaveState();

		state.Fish = CurrentFish?.Id;
		state.Weather = Weather;
		state.FType = FType;
		state.CStatus = CStatus;
		state.LFilter = LFilter;
		state.AFilter = AFilter;

		return state;
	}

	public override void LoadState(FishingState state) {
		base.LoadState(state);

		CurrentFish = null;

		if (!string.IsNullOrEmpty(state.Fish))
			foreach(var fish in Mod.Fish.GetFish()) {
				if (fish.Id == state.Fish) {
					SelectFish(fish);
					break;
				}
			}

		Weather = state.Weather;
		FType = state.FType;
		CStatus = state.CStatus;
		LFilter = state.LFilter;
		AFilter = state.AFilter;
	}

	public IFlowNode[]? BuildRightPage(FishInfo? _info) {
		var builder = FlowHelper.Builder();

		if (!_info.HasValue) {
			if (Tank == null)
				return null;

			var urchin = new SObject(397, 1);
			FillTank(urchin, 4);

			builder.Text("\n\n\n\n");

			if (Mod.Config.ShowFishTank)
				builder.Add(new ComponentNode(
					tankComponent,
					wrapping: WrapMode.ForceBefore | WrapMode.ForceAfter,
					onClick: (_, _, _) => {
						FillTank(urchin, 4);
						return true;
					},
					onDraw: (b, pos, scale, _, _, _) => {
						UpdateTank();
						DrawTank(b);
					}
				));

			builder.Text("\n\n");
			builder.FormatText(
				I18n.Page_Fish_Nothing(),
				align: Alignment.Center
			);

			return builder.Build();
		}

		FishInfo info = _info.Value;

		bool OnHover(IFlowNodeSlice slice, int x, int y) {
			Menu.HoveredItem = info.Item;
			return true;
		}

		builder
			.Sprite(info.Sprite, 4f, Alignment.Center, onHover: OnHover, noComponent: true)
			.Text(
				$" {info.Name}\n",
				fancy: info.Legendary,
				font: Game1.dialogueFont,
				align: Alignment.Middle,
				onHover: OnHover,
				noComponent: true
			);

		if (!string.IsNullOrEmpty(info.Description))
			builder
				.Text("\n")
				.FormatText(info.Description)
				.Text("\n\n");

		if (Tank?.CanBeDeposited(info.Item) ?? false && Mod.Config.ShowFishTank) {
			int amount = info.Legendary ? 1 : -1;
			FillTank(info.Item, amount);

			builder.Add(new ComponentNode(
				tankComponent,
				wrapping: WrapMode.ForceBefore | WrapMode.ForceAfter,
				onClick: (_, _, _) => {
					FillTank(info.Item, amount);
					return true;
				},
				onDraw: (b, pos, scale, _, _, _) => {
					UpdateTank();
					DrawTank(b);
				}
			));

		} else
			builder
				.FormatText(I18n.Page_Fish_Aquarium_None())
				.Text("\n");

		builder.Text("\n");

		int number = info.NumberCaught(Game1.player);
		int biggest = info.BiggestCatch(Game1.player);

		if (number > 0) {
			builder.Translate(
				Mod.Helper.Translation.Get("page.fish.caught"),
				new {
					count = number
				}
			);

			if (biggest > 0)
				builder
					.Text(" ")
					.Translate(
						Mod.Helper.Translation.Get("page.fish.size"),
						new {
							big_inch = $"{biggest}",
							big_cm = $"{biggest * 2.54f:F}"
						}
					);
		} else
			builder.FormatText(I18n.Page_Fish_Caught_Not());

		if (HasAquarium) {
			bool donated = CheckAquariumDonated(info);
			if (donated)
				builder.FormatText($" {I18n.Page_Fish_Aquarium_Donated()}");
			else
				builder.FormatText($" {I18n.Page_Fish_Aquarium_NotDonated()}");
		}

		builder.Text("\n\n");

		if (info.Legendary)
			builder
				.Translate(
					Mod.Helper.Translation.Get("page.fish.legendary"),
					new { fish = info.Name }
				)
				.Text("\n\n");

		// Depending on whether this is a trap fish or a caught
		// fish, we show different info. Support both because
		// reasons.

		if (info.TrapInfo.HasValue) {
			TrapFishInfo trap = info.TrapInfo.Value;

			if (trap.Location == WaterType.Freshwater)
				builder.Translate(
					Mod.Helper.Translation.Get("page.fish.location.fresh"),
					new { fish = info.Name }
				);
			else if (trap.Location == WaterType.Ocean)
				builder.Translate(
					Mod.Helper.Translation.Get("page.fish.location.ocean"),
					new { fish = info.Name }
				);

			if (info.MinSize > 0 && info.MaxSize > 0) {
				builder.Text(" ");
				builder.Translate(
					Mod.Helper.Translation.Get("page.fish.size.range"),
					new {
						min_inch = info.MinSize,
						min_cm = $"{info.MinSize * 2.54f:F}",
						max_inch = info.MaxSize,
						max_cm = $"{info.MaxSize * 2.54f:F}"
					}
				);
			}

		}

		if (info.CatchInfo.HasValue) {
			CatchFishInfo caught = info.CatchInfo.Value;

			if (caught.Weather == FishWeather.Sunny)
				builder.Translate(
					Mod.Helper.Translation.Get("page.fish.weather.Sunny"),
					new { fish = info.Name }
				);

			else if (caught.Weather == FishWeather.Rainy)
				builder.Translate(
					Mod.Helper.Translation.Get("page.fish.weather.Rainy"),
					new { fish = info.Name }
				);

			else
				builder.Translate(
					Mod.Helper.Translation.Get("page.fish.weather.Any"),
					new { fish = info.Name }
				);

			if (caught.Times != null && caught.Times.Length > 0) {
				builder.Text(" ");

				if (caught.Times.Where(t => t.AllDay).Any())
					builder.FormatText(I18n.Page_Fish_Time_All());

				else if (caught.Times.Length == 1) {
					builder.Translate(
						Mod.Helper.Translation.Get("page.fish.time"),
						new {
							start = Mod.FormatTime(caught.Times[0].Start),
							end = Mod.FormatTime(caught.Times[0].End)
						}
					);

				} else {
					var b2 = FlowHelper.Builder();

					foreach (var t in caught.Times) {
						if (b2.Count > 0)
							b2.Text(", ");

						b2.Translate(
							Mod.Helper.Translation.Get("page.fish.time.range"),
							new {
								start = Mod.FormatTime(t.Start),
								end = Mod.FormatTime(t.End)
							}
						);
					}

					builder.Translate(
						Mod.Helper.Translation.Get("page.fish.time.many"),
						new {
							spans = b2.Build()
						}
					);
				}
			}

			if (info.MinSize > 0 && info.MaxSize > 0) {
				builder.Text(" ");
				builder.Translate(
					Mod.Helper.Translation.Get("page.fish.size.range"),
					new {
						min_inch = info.MinSize,
						min_cm = $"{info.MinSize * 2.54f:F}",
						max_inch = info.MaxSize,
						max_cm = $"{info.MaxSize * 2.54f:F}"
					}
				);
			}

			if (caught.Minlevel > 0) {
				var skill = FlowHelper.Builder()
					.Sprite(
						new SpriteInfo(
							Game1.mouseCursors,
							FISHING_ICON
						),
						2f,
						size: 10,
						align: Alignment.Middle
					)
					.Text(" ")
					.Text(
						Game1.content.LoadString(@"Strings\StringsFromCSFiles:SkillsPage.cs.11607"),
						bold: true
					)
					.Build();

				builder.Text(" ");
				builder.Translate(
					Mod.Helper.Translation.Get("page.fish.level"),
					new {
						skill,
						level = caught.Minlevel
					}
				);
			}

			builder.Text("\n\n");

			if (caught.Locations != null) {
				builder
					.FormatText(I18n.Page_Fish_Locations(), font: Game1.dialogueFont)
					.Text("\n");

				List<Tuple<string, bool, IFlowNode[]>> sorted = new();

				foreach (var pair in caught.Locations) {
					string? subloc = pair.Key.Area == -1 ? null
						: Mod.GetSubLocationName(pair.Key);

					string name = Mod.GetLocationName(pair.Key.Key, pair.Key.Location);
					string key = subloc == null ? name : $"{name} ({subloc})";

					bool matches = pair.Value.Contains(Menu.Season);
					bool all_seasons = true;

					for (int i = 0; i < WorldDate.MonthsPerYear; i++) {
						if (!pair.Value.Contains(i)) {
							all_seasons = false;
							break;
						}
					}

					Color? color = matches ? null : Game1.textColor * .5f;
					Color? shadow = matches ? null : Game1.textShadowColor * .5f;

					var b3 = FlowHelper.Builder()
						.Text("  ")
						.FormatText(key, color: color, shadowColor: shadow)
						.Text("\n    ");

					if (Mod.Config.DebugMode)
						b3.Text(
							$"id: {pair.Key.Key}:{pair.Key.Area}",
							shadow: false,
							color: Game1.textColor * 0.5f
						)
						.Text("\n    ");

					if (all_seasons)
						b3.FormatText(I18n.Page_Fish_Seasons_All(), shadow: false, color: color);
					else {
						bool first = true;

						foreach (int season in pair.Value) {
							string sName = Utility.getSeasonNameFromNumber(season);
							if (first)
								first = false;
							else
								b3.Text(", ", shadow: false, color: color);

							b3.Text(sName, color: color, shadow: false, bold: season == Menu.Season);
						}
					}

					b3.Text("\n");
					sorted.Add(new(key, matches, b3.Build()));
				}

				sorted.Sort((a, b) => {
					if (a.Item2 && !b.Item2) return -1;
					if (!a.Item2 && b.Item2) return 1;

					return a.Item1.CompareTo(b.Item1);
				});

				foreach (var item in sorted)
					builder.AddRange(item.Item3);

			} else
				builder
					.FormatText(I18n.Page_Fish_Locations_None());
		}

		/*if (info.PondInfo.HasValue) {
			PondInfo pond = info.PondInfo.Value;

			builder
				.Text("\n\n")
				.FormatText("Farming", font: Game1.dialogueFont)
				.Text("\n");

			builder
				.FormatText($"When bred in a fish pond, {info.Name} reproduce every @B{pond.SpawnTime}@b days.");

			if (pond.Initial < 10)
				builder
					.FormatText($"  support an initial population of @B{pond.Initial}@b");

			builder
				.FormatText($" Captive {info.Name} have been known to produce the following:\n");

			foreach(var item in pond.ProducedItems)
				builder
					.Text("\n  ")
					.Sprite(SpriteHelper.GetSprite(item), 2f, align: Alignment.Middle, extra: item)
					.Text(" ")
					.Text(item.DisplayName, extra: item);

			builder.Text("\n");
		}*/

		return builder.Build();
	}

	private bool CheckAquariumDonated(FishInfo fish) {
		return Game1.MasterPlayer.mailReceived.Contains($"AquariumDonated:{fish.Name.Replace(" ", "")}");
	}

	public override void Update() {
		base.Update();

		FlowBuilder builder = new();
		FishInfo? to_select = null;
		FishNodes.Clear();

		var selected = CurrentFish;

		var sorted = Mod.Fish.GetSeasonFish(Menu.Date.Season);
		sorted.Sort((a, b) => {
			return a.Name.CompareTo(b.Name);
		});

		foreach (var fish in sorted) {
			if (fish.Legendary && !Mod.Config.FishShowLegendary)
				continue;

			if (Weather != FishWeather.None) {
				FishWeather fw = fish.CatchInfo?.Weather ?? FishWeather.Any;
				if (fw != Weather)
					continue;
			}

			if (FType == FishType.Trap && !fish.TrapInfo.HasValue)
				continue;

			if (FType == FishType.Catch && !fish.CatchInfo.HasValue)
				continue;

			if (CStatus != CaughtStatus.None) {
				int caught = fish.NumberCaught(Game1.player);
				if (CStatus == CaughtStatus.Caught && caught == 0)
					continue;
				if (CStatus == CaughtStatus.Uncaught && caught > 0)
					continue;
			}

			if (HasAquarium && AFilter != AquariumFilter.Off) {
				bool donated = CheckAquariumDonated(fish);
				if (donated != (AFilter == AquariumFilter.Donated))
					continue;
			}

			if (LFilter != LocationFilter.Off) {
				bool found = false;

				if (fish.CatchInfo.HasValue) {
					var locations = fish.CatchInfo.Value.Locations;
					if (locations == null)
						continue;

					string name = Game1.currentLocation.Name;
					if (Game1.currentLocation is MineShaft shaft)
						name = "UndergroundMine";

					foreach(var lp in locations) {
						if (lp.Key.Key.Equals(name)) {
							if (
								LFilter == LocationFilter.On ||
								lp.Value.Contains(Menu.Date.SeasonIndex)
							) {
								found = true;
								break;
							}
						}
					}
				}

				if (fish.TrapInfo.HasValue) {
					if (FishHelper.GetTrapWaterType(Game1.currentLocation).HasFlag(fish.TrapInfo.Value.Location))
						found = true;
				}

				if (!found)
					continue;
			}


			if (!to_select.HasValue || (selected.HasValue && selected.Value == fish))
				to_select = fish;

			var sb = FlowHelper.Builder()
				.Sprite(fish.Sprite, 4f, Alignment.Middle)
				.Text($" {fish.Name}", font: Game1.dialogueFont, align: Alignment.Middle);

			if (Mod.Config.DebugMode)
				sb.Text($" (#{fish.Id})", align: Alignment.Middle | Alignment.Right);

			var node = new SelectableNode(
				sb.Build(),

				onHover: (_, _, _) => {
					Menu.HoveredItem = fish.Item;
					return true;
				},

				onClick: (_, _, _) => {
					if (SelectFish(fish))
						Game1.playSound("smallSelect");
					return true;
				}
			) {
				SelectedTexture = Menu.background,
				SelectedSource = new(336, 352, 16, 16),
				HoverTexture = Menu.background,
				HoverSource = new(336, 352, 16, 16),
				HoverColor = Color.White * 0.4f
			};

			if (selected.HasValue & selected == fish)
				node.Selected = true;

			FishNodes.Add(fish.Id, node);
			builder.Add(node);
		}

		if (builder.Count == 0)
			builder
				.Text("\n\n\n")
				.FormatText(I18n.Page_Fish_None(), align: Alignment.Center);

		SetLeftFlow(builder, scroll: -1);
		SelectFish(to_select);
	}

	public bool SelectFish(FishInfo? fish) {
		if (fish == CurrentFish)
			return false;

		CurrentFish = fish;

		foreach (var pair in FishNodes)
			pair.Value.Selected = fish?.Id == pair.Key;

		SetRightFlow(FishFlow.Value);
		return true;
	}

	public void FillTank(Item item, int count = -1) {
		if (Tank == null)
			return;

		// Reset the tank
		Tank.heldItems.Clear();
		Tank.ResetFish();
		Tank.generationSeed.Value++;

		// Decor
		if (Mod.Config.DecorateFishTank) {
			Tank.heldItems.Add(new SObject(152, 1));
			Tank.heldItems.Add(new SObject(390, 1));
			Tank.heldItems.Add(new SObject(393, 1));
		}

		// Add the Fish
		if (count < 1) {
			var cat = Tank.GetCategoryFromItem(item);
			count = (int) Math.Ceiling(Tank.GetCapacityForCategory(cat) / 2f);
		}

		if (count < 1)
			count = 1;

		if (count > 10)
			count = 10;

		for (int i = 0; i < count; i++)
			Tank.heldItems.Add(item.getOne());

		Tank.UpdateDecorAndFish();

		// Do we want hats?

		if (Tank.tankFish.Count > 0 && Tank.tankFish[0].fishIndex == 86) {
			Dictionary<int, string> dictionary = Game1.content.Load<Dictionary<int, string>>(@"Data\hats");

			for (int i = 0; i < 4; i++) {
				int hat = Game1.random.Next(0, dictionary.Keys.Count);
				Tank.heldItems.Add(new Hat(hat));
			}
		}
	}

	#endregion

	#region ILeftFlowMargins

	public int LeftMarginTop => 64;
	public int LeftMarginLeft => 64 + 16;
	public int LeftMarginRight => 0;
	public int LeftMarginBottom => 0;
	public int LeftScrollMarginTop => 0;
	public int LeftScrollMarginBottom => 0;

	#endregion

	#region ITab

	public override int SortKey => 40;
	public override string TabSimpleTooltip => I18n.Page_Fish();
	public override Texture2D TabTexture => Game1.mouseCursors;
	public override Rectangle? TabSource => FISHING_ICON;

	#endregion

	#region IAlmanacPage

	public override bool IsMagic => false;

	public override PageType Type => PageType.Seasonal;

	#endregion

	#region Events

	public void UpdateTank() {
		if (Tank.Bounds == tankComponent.bounds)
			return;

		Tank.Bounds = new Rectangle(
			tankComponent.bounds.X,
			tankComponent.bounds.Y,
			tankComponent.bounds.Width,
			tankComponent.bounds.Height
		);
	}

	public override void UpdateComponents() {
		base.UpdateComponents();

		btnFilterWeather.bounds.X = Menu.xPositionOnScreen + 32 + 16;
		btnFilterWeather.bounds.Y = Menu.yPositionOnScreen + 64 + 28 + 16;

		GUIHelper.MoveComponents(
			GUIHelper.Side.Down, 16,
			btnFilterWeather,
			btnFilterType,
			btnFilterCaught,
			btnFilterLocation,
			btnFilterAquarium
		);

		tankComponent.bounds = new(
			tankComponent.bounds.X,
			tankComponent.bounds.Y,
			Menu.width / 2 - 112,
			192
		);

		UpdateTank();
	}

	public override void Activate() {
		// We need to update our tank component here, since
		// the page width was probably different when our
		// components were updated.
		UpdateComponents();

		base.Activate();
	}

	#endregion

	#region Input

	public override void PerformHover(int x, int y) {
		base.PerformHover(x, y);

		btnFilterWeather.tryHover(x, y);
		btnFilterType.tryHover(x, y);
		btnFilterCaught.tryHover(x, y);
		btnFilterLocation.tryHover(x, y);
		btnFilterAquarium?.tryHover(x, y);

		if (btnFilterWeather.containsPoint(x,y)) {
			var builder = SimpleHelper.Builder()
				.FormatText(I18n.Page_Fish_Filter_Weather());

			if (Weather == FishWeather.None)
				builder.FormatText(I18n.Page_Fish_Filter_None(), color: Game1.textColor * 0.4f);
			if (Weather == FishWeather.Any)
				builder.FormatText(I18n.Fish_Weather_Any(), color: Game1.textColor * 0.4f);
			if (Weather == FishWeather.Sunny)
				builder.FormatText(I18n.Fish_Weather_Sunny(), color: Game1.textColor * 0.4f);
			if (Weather == FishWeather.Rainy)
				builder.FormatText(I18n.Fish_Weather_Rainy(), color: Game1.textColor * 0.4f);

			Menu.HoverNode = builder.GetLayout();
		}

		if (btnFilterType.containsPoint(x, y)) {
			var builder = SimpleHelper.Builder()
				.FormatText(I18n.Page_Fish_Filter_Type());

			if (FType == FishType.None)
				builder.FormatText(I18n.Page_Fish_Filter_None(), color: Game1.textColor * 0.4f);
			if (FType == FishType.Catch)
				builder.FormatText(I18n.Page_Fish_Filter_Type_Caught(), color: Game1.textColor * 0.4f);
			if (FType == FishType.Trap)
				builder.FormatText(I18n.Page_Fish_Filter_Type_Trap(), color: Game1.textColor * 0.4f);

			Menu.HoverNode = builder.GetLayout();
		}

		if (btnFilterCaught.containsPoint(x, y)) {
			var builder = SimpleHelper.Builder()
				.FormatText(I18n.Page_Fish_Filter_Caught());

			if (CStatus == CaughtStatus.None)
				builder.FormatText(I18n.Page_Fish_Filter_None(), color: Game1.textColor * 0.4f);
			if (CStatus == CaughtStatus.Caught)
				builder.FormatText(I18n.Page_Fish_Filter_True(), color: Game1.textColor * 0.4f);
			if (CStatus == CaughtStatus.Uncaught)
				builder.FormatText(I18n.Page_Fish_Filter_False(), color: Game1.textColor * 0.4f);

			Menu.HoverNode = builder.GetLayout();
		}

		if (btnFilterLocation.containsPoint(x, y)) {
			var builder = SimpleHelper.Builder()
				.FormatText(I18n.Page_Fish_Filter_Location());

			if (LFilter == LocationFilter.Off)
				builder.FormatText(I18n.Page_Fish_Filter_None(), color: Game1.textColor * 0.4f);
			else if (LFilter == LocationFilter.On)
				builder.FormatText(I18n.Page_Fish_Filter_True(), color: Game1.textColor * 0.4f);
			else
				builder.FormatText(I18n.Page_Fish_Filter_Now(), color: Game1.textColor * 0.4f);

			Menu.HoverNode = builder.GetLayout();
		}

		if (btnFilterAquarium != null && btnFilterAquarium.containsPoint(x, y)) {
			var builder = SimpleHelper.Builder()
				.FormatText(I18n.Page_Fish_Filter_Aquarium());

			if (AFilter == AquariumFilter.Off)
				builder.FormatText(I18n.Page_Fish_Filter_None(), color: Game1.textColor * 0.4f);
			else if (AFilter == AquariumFilter.Donated)
				builder.FormatText(I18n.Page_Fish_Aquarium_True(), color: Game1.textColor * 0.4f);
			else
				builder.FormatText(I18n.Page_Fish_Aquarium_False(), color: Game1.textColor * 0.4f);

			Menu.HoverNode = builder.GetLayout();
		}
	}

	public override bool ReceiveLeftClick(int x, int y, bool playSound) {

		if (btnFilterWeather.containsPoint(x, y)) {
			Weather = CommonHelper.Cycle(Weather);

			Update();
			btnFilterWeather.scale = btnFilterWeather.baseScale;
			if (playSound)
				Game1.playSound("smallSelect");

			return true;
		}

		if (btnFilterType.containsPoint(x, y)) {
			FType = CommonHelper.Cycle(FType);

			Update();
			btnFilterType.scale = btnFilterType.baseScale;
			if (playSound)
				Game1.playSound("smallSelect");

			return true;
		}

		if (btnFilterCaught.containsPoint(x, y)) {
			CStatus = CommonHelper.Cycle(CStatus);

			Update();
			btnFilterCaught.scale = btnFilterCaught.baseScale;
			if (playSound)
				Game1.playSound("smallSelect");

			return true;
		}

		if (btnFilterLocation.containsPoint(x, y)) {
			LFilter = CommonHelper.Cycle(LFilter);

			Update();
			btnFilterLocation.scale = btnFilterLocation.baseScale;
			if (playSound)
				Game1.playSound("smallSelect");

			return true;
		}

		if (btnFilterAquarium != null && btnFilterAquarium.containsPoint(x, y)) {
			AFilter = CommonHelper.Cycle(AFilter);

			Update();
			btnFilterAquarium.scale = btnFilterAquarium.baseScale;
			if (playSound)
				Game1.playSound("smallSelect");

			return true;
		}

		return base.ReceiveLeftClick(x, y, playSound);
	}

	public override bool ReceiveRightClick(int x, int y, bool playSound) {

		if (btnFilterWeather.containsPoint(x, y)) {
			Weather = CommonHelper.Cycle(Weather, -1);

			Update();
			btnFilterWeather.scale = btnFilterWeather.baseScale;
			if (playSound)
				Game1.playSound("smallSelect");

			return true;
		}

		if (btnFilterType.containsPoint(x, y)) {
			FType = CommonHelper.Cycle(FType, -1);

			Update();
			btnFilterType.scale = btnFilterType.baseScale;
			if (playSound)
				Game1.playSound("smallSelect");

			return true;
		}

		if (btnFilterCaught.containsPoint(x, y)) {
			CStatus = CommonHelper.Cycle(CStatus, -1);

			Update();
			btnFilterCaught.scale = btnFilterCaught.baseScale;
			if (playSound)
				Game1.playSound("smallSelect");

			return true;
		}

		if (btnFilterLocation.containsPoint(x, y)) {
			LFilter = CommonHelper.Cycle(LFilter, -1);

			Update();
			btnFilterLocation.scale = btnFilterLocation.baseScale;
			if (playSound)
				Game1.playSound("smallSelect");

			return true;
		}

		if (btnFilterAquarium != null && btnFilterAquarium.containsPoint(x, y)) {
			AFilter = CommonHelper.Cycle(AFilter, -1);

			Update();
			btnFilterAquarium.scale = btnFilterAquarium.baseScale;
			if (playSound)
				Game1.playSound("smallSelect");

			return true;
		}

		return base.ReceiveRightClick(x, y, playSound);
	}

	#endregion

	#region Drawing

	public override void Draw(SpriteBatch b) {
		base.Draw(b);

		// TODO: Make these colors and rectangles constants.

		/*if (Mod.Config.DebugMode) {
			var pos = Game1.getMousePosition(false);

			int x = (Game1.viewport.X + pos.X) / Game1.tileSize;
			int y = (Game1.viewport.Y + pos.Y) / Game1.tileSize; // pixelZoom;

			int zone = Game1.currentLocation.getFishingLocation(new Vector2(x, y));

			SimpleHelper.Builder()
				.Group()
					.Text($"Pos: ({x}, {y})")
				.EndGroup()
				.Group()
					.Text($"Map: ")
					.Text($"{Game1.currentLocation.Name} => {Mod.GetLocationName(Game1.currentLocation)}")
				.EndGroup()
				.Group()
					.Text($"Fish Area: ")
					.Text($"{zone} => {Mod.GetSubLocationName(new Models.SubLocation(Game1.currentLocation.Name, zone))}")
				.EndGroup()
				.GetLayout()
				.DrawHover(
					batch: b,
					defaultFont: Game1.smallFont,
					overrideX: 4,
					overrideY: 4
				);
		}*/

		// Divider?
		b.Draw(
			Game1.uncoloredMenuTexture,
			new Rectangle(
				Menu.xPositionOnScreen + 32 + 8 - 2,
				Menu.yPositionOnScreen + 64 + 28 + 2,
				Menu.width / 2 - 64,
				4
			),
			new Rectangle(16, 272, 28, 28),
			Menu.Style.TextShadowColor ?? new Color(0xE0, 0x96, 0x50)
		);

		b.Draw(
			Game1.uncoloredMenuTexture,
			new Rectangle(
				Menu.xPositionOnScreen + 32 + 8,
				Menu.yPositionOnScreen + 64 + 28,
				Menu.width / 2 - 64,
				4
			),
			new Rectangle(16, 272, 28, 28),
			Menu.Style.TextColor ?? new Color(0x56, 0x16, 0x0C)
		);


		// Weather Button
		btnFilterWeather.draw(b);

		b.Draw(
			Menu.background,
			btnFilterWeather.bounds,
			SourceWeather,
			Color.White
		);

		// Type Button
		btnFilterType.draw(b);

		b.Draw(
			Menu.background,
			btnFilterType.bounds,
			SourceType,
			Color.White
		);

		// Caught Button
		btnFilterCaught.draw(b);

		b.Draw(
			Menu.background,
			btnFilterCaught.bounds,
			SourceCaught,
			Color.White
		);

		// Location Button
		btnFilterLocation.draw(b);

		b.Draw(
			Menu.background,
			btnFilterLocation.bounds,
			SourceLocation,
			Color.White
		);

		// Aquarium Button
		if (btnFilterAquarium != null) {
			btnFilterAquarium.draw(b);

			b.Draw(
				Menu.background,
				btnFilterAquarium.bounds,
				SourceAquarium,
				Color.White
			);
		}
	}

	public void DrawTank(SpriteBatch b) { 
		if (!tankComponent.visible)
			return;

		Tank.draw(b);
	}

	#endregion

}
