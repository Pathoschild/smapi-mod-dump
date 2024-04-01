/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shockah.CommonModCode.GMCM;
using Shockah.Kokoro;
using Shockah.Kokoro.GMCM;
using Shockah.Kokoro.Stardew;
using Shockah.Kokoro.UI;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.GameData.Machines;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using SObject = StardewValley.Object;

namespace Shockah.MachineStatus;

public class ModEntry : BaseMod<ModConfig>
{
	private static readonly ItemRenderer ItemRenderer = new();
	private static readonly Vector2 SingleMachineSize = new(64, 64);

	private static readonly (string titleKey, string[] machineIds)[] CategorizedMachineTypes = [
		("config.machine.category.artisan", new string[]
		{
			"(BC)10", // Bee House
			"(BC)163", // Cask
			"(BC)16", // Cheese Press
			"(BC)12", // Keg
			"(BC)17", // Loom
			"(BC)24", // Mayonnaise Machine
			"(BC)19", // Oil Maker
			"(BC)15" // Preserves Jar
		}),
		("config.machine.category.refining", new string[]
		{
			"(BC)90", // Bone Mill
			"(BC)114", // Charcoal Kiln
			"(BC)21", // Crystalarium
			"(BC)13", // Furnace
			"(BC)182", // Geode Crusher
			"(BC)264", // Heavy Tapper
			"(BC)9", // Lightning Rod
			"(BC)20", // Recycling Machine
			"(BC)25", // Seed Maker
			"(BC)158", // Slime Egg-Press
			"(BC)231", // Solar Panel
			"(BC)105", // Tapper
			"(BC)211", // Wood Chipper
			"(BC)154" // Worm Bin
		}),
		("config.machine.category.misc", new string[]
		{
			"(BC)246", // Coffee Maker
			"(O)710", // Crab Pot // not included in Data/Machines
			"(BC)265", // Deconstructor
			"(BC)101", // Incubator
			"(BC)128", // Mushroom Box
			"(BC)254", // Ostrich Incubator
			"(BC)156", // Slime Incubator
			"(BC)117", // Soda Machine
			"(BC)127", // Statue of Endless Fortune
			"(BC)160", // Statue of Perfection
			"(BC)280" // Statue of True Perfection
		})
	];

	internal static ModEntry Instance { get; set; } = null!;
	private bool IsConfigRegistered { get; set; } = false;
	internal IDynamicGameAssetsApi? DynamicGameAssetsApi { get; set; }

	private readonly List<WeakReference<SObject>> TrackedMachines = [];
	private readonly List<SObject> IgnoredMachinesForUpdates = [];
	private readonly HashSet<SObject> QueuedMachineUpdates = [];

	private readonly PerScreen<List<(LocationDescriptor location, SObject machine, MachineState state)>> PerScreenHostMachines = new(() => new());
	private readonly PerScreen<List<(LocationDescriptor location, SObject machine, MachineState state)>> PerScreenClientMachines = new(() => new());
	private readonly PerScreen<List<(LocationDescriptor location, SObject machine, MachineState state)>> PerScreenVisibleMachines = new(() => new());
	private readonly PerScreen<List<(LocationDescriptor location, SObject machine, MachineState state)>> PerScreenSortedMachines = new(() => new());
	private readonly PerScreen<List<(SObject machine, List<SObject> heldItems)>> PerScreenGroupedMachines = new(() => new());
	private readonly PerScreen<List<(IntPoint position, (SObject machine, List<SObject> heldItems) machine)>> PerScreenFlowMachines = new(() => new());
	private readonly PerScreen<bool> PerScreenAreVisibleMachinesDirty = new(() => true);
	private readonly PerScreen<bool> PerScreenAreSortedMachinesDirty = new(() => true);
	private readonly PerScreen<bool> PerScreenAreGroupedMachinesDirty = new(() => true);
	private readonly PerScreen<bool> PerScreenAreFlowMachinesDirty = new(() => true);
	private readonly PerScreen<(GameLocation, Vector2)?> PerScreenLastPlayerTileLocation = new();
	private readonly PerScreen<MachineRenderingOptions.Visibility> PerScreenVisibility = new(() => MachineRenderingOptions.Visibility.Normal);
	private readonly PerScreen<float> PerScreenVisibilityAlpha = new(() => 1f);
	private readonly PerScreen<bool> PerScreenIsHoveredOver = new(() => false);

	private List<(LocationDescriptor location, SObject machine, MachineState state)> HostMachines => PerScreenHostMachines.Value;
	private List<(LocationDescriptor location, SObject machine, MachineState state)> ClientMachines => PerScreenClientMachines.Value;
	private List<(LocationDescriptor location, SObject machine, MachineState state)> VisibleMachines => PerScreenVisibleMachines.Value;
	private List<(LocationDescriptor location, SObject machine, MachineState state)> SortedMachines => PerScreenSortedMachines.Value;
	private List<(SObject machine, List<SObject> heldItems)> GroupedMachines => PerScreenGroupedMachines.Value;
	private List<(IntPoint position, (SObject machine, List<SObject> heldItems) machine)> FlowMachines => PerScreenFlowMachines.Value;
	private bool AreVisibleMachinesDirty { get => PerScreenAreVisibleMachinesDirty.Value; set => PerScreenAreVisibleMachinesDirty.Value = value; }
	private bool AreSortedMachinesDirty { get => PerScreenAreSortedMachinesDirty.Value; set => PerScreenAreSortedMachinesDirty.Value = value; }
	private bool AreGroupedMachinesDirty { get => PerScreenAreGroupedMachinesDirty.Value; set => PerScreenAreGroupedMachinesDirty.Value = value; }
	private bool AreFlowMachinesDirty { get => PerScreenAreFlowMachinesDirty.Value; set => PerScreenAreFlowMachinesDirty.Value = value; }
	private (GameLocation, Vector2)? LastPlayerTileLocation { get => PerScreenLastPlayerTileLocation.Value; set => PerScreenLastPlayerTileLocation.Value = value; }
	private MachineRenderingOptions.Visibility Visibility { get => PerScreenVisibility.Value; set => PerScreenVisibility.Value = value; }
	private float VisibilityAlpha { get => PerScreenVisibilityAlpha.Value; set => PerScreenVisibilityAlpha.Value = value; }
	private bool IsHoveredOver { get => PerScreenIsHoveredOver.Value; set => PerScreenIsHoveredOver.Value = value; }

	public override void MigrateConfig(ISemanticVersion? configVersion, ISemanticVersion modVersion)
	{
		// do nothing, for now
	}

	public override void OnEntry(IModHelper helper)
	{
		Instance = this;

		helper.Events.GameLoop.GameLaunched += OnGameLaunched;
		helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
		helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
		helper.Events.World.ObjectListChanged += OnObjectListChanged;
		helper.Events.Display.RenderedHud += OnRenderedHud;
		helper.Events.Multiplayer.PeerConnected += OnPeerConnected;

		RegisterModMessageHandler<NetMessage.MachineUpsert>(OnMachineUpsertMessageReceived);
		RegisterModMessageHandler<NetMessage.MachineRemove>(OnMachineRemoveMessageReceived);
	}

	private void SetupConfig()
	{
		var api = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
		if (api is null)
			return;
		GMCMI18nHelper helper = new(api, ModManifest, Helper.Translation);

		if (IsConfigRegistered)
			api.Unregister(ModManifest);

		api.Register(
			ModManifest,
			reset: () => Config = new ModConfig(),
			save: () =>
			{
				var newSortingOptions = Config.Sorting.Where(o => o != MachineRenderingOptions.Sorting.None).ToList();
				Config.Sorting.Clear();
				foreach (var sortingOption in newSortingOptions)
					Config.Sorting.Add(sortingOption);
				Helper.WriteConfig(Config);
				ForceRefreshDisplayedMachines();
				SetupConfig();
				LogConfig();
			}
		);

		string BuiltInMachineSyntax(string machineName)
			=> $"*|{machineName}";

		var categorizedMachineTypes = CategorizedMachineTypes
			.Select(section => (titleKey: section.titleKey, machines: section.machineIds.Select(machineId => ItemRegistry.Create(machineId, allowNull: true)).WhereNotNull().ToList()))
			.ToList();

		void SetupExceptionsPage(string typeKey, MachineState state, IList<string> exceptions)
		{
			helper.AddPage(typeKey, typeKey);

			helper.AddTextOption(
				keyPrefix: "config.exceptions.manual",
				getValue: () => string.Join(", ", exceptions.Where(ex => !categorizedMachineTypes.Any(section => section.machines.Any(machine => BuiltInMachineSyntax(machine.Name) == ex)))),
				setValue: value =>
				{
					var existingVanillaValues = exceptions
						.Where(ex => categorizedMachineTypes.Any(section => section.machines.Any(machine => BuiltInMachineSyntax(machine.Name) == ex)))
						.ToList();
					var customInputValues = value
						.Split(',')
						.Select(s => s.Trim())
						.Where(s => s.Length > 0)
						.Where(s => !categorizedMachineTypes.Any(section => section.machines.Any(machine => BuiltInMachineSyntax(machine.Name) == s)))
						.ToList();

					exceptions.Clear();
					foreach (var existingVanillaValue in existingVanillaValues)
						exceptions.Add(existingVanillaValue);
					foreach (var customInputValue in customInputValues)
						exceptions.Add(customInputValue);
				}
			);

			void AddMultiSelectOption(string titleKey, IEnumerable<Item> machines)
				=> helper.AddMultiSelectTextOption(
					titleKey,
					getValue: v => exceptions.Contains(BuiltInMachineSyntax(v.Name)),
					addValue: v => exceptions.Add(BuiltInMachineSyntax(v.Name)),
					removeValue: v => exceptions.Remove(BuiltInMachineSyntax(v.Name)),
					columns: _ => 2,
					allowedValues: machines.Where(m => !(!m.GetContextTags().Contains("machine_input") && state == MachineState.Waiting)).ToArray(),
					formatAllowedValue: v => v.DisplayName
				);

			foreach (var (titleKey, machines) in categorizedMachineTypes)
				AddMultiSelectOption(titleKey, machines);

			var unknownMachines = Game1.content.Load<Dictionary<string, MachineData>>("Data\\Machines")
				.Where(kvp => !CategorizedMachineTypes.Any(section => section.machineIds.Contains(kvp.Key)))
				.Select(kvp => ItemRegistry.Create(kvp.Key, allowNull: true))
				.WhereNotNull()
				.ToList();
			if (unknownMachines.Count != 0)
				AddMultiSelectOption("config.machine.category.uncategorized", unknownMachines);
		}

		void SetupStateConfig(string optionKey, string pageKey, Expression<Func<bool>> boolProperty)
		{
			helper.AddBoolOption(optionKey, boolProperty);
			helper.AddPageLink(pageKey, "config.exceptions");
		}

		helper.AddSectionTitle("config.layout.section");
		helper.AddEnumOption("config.anchor.screen", valuePrefix: "config.anchor", property: () => Config.ScreenAnchorSide);
		helper.AddEnumOption("config.anchor.panel", valuePrefix: "config.anchor", property: () => Config.PanelAnchorSide);
		helper.AddNumberOption("config.anchor.inset", () => Config.AnchorInset);
		helper.AddNumberOption("config.anchor.x", () => Config.AnchorOffsetX);
		helper.AddNumberOption("config.anchor.y", () => Config.AnchorOffsetY);
		helper.AddEnumOption("config.layout.flowDirection", valuePrefix: "config.flowDirection", property: () => Config.FlowDirection);
		helper.AddNumberOption("config.layout.scale", () => Config.Scale, min: 0f, max: 12f, interval: 0.05f);
		helper.AddNumberOption("config.layout.xSpacing", () => Config.XSpacing, min: -16f, max: 64f, interval: 0.5f);
		helper.AddNumberOption("config.layout.ySpacing", () => Config.YSpacing, min: -16f, max: 64f, interval: 0.5f);
		helper.AddNumberOption("config.layout.maxColumns", () => Config.MaxColumns, min: 0, max: 20);

		helper.AddSectionTitle("config.bubble.section");
		helper.AddBoolOption("config.bubble.showItem", () => Config.ShowItemBubble);
		helper.AddNumberOption("config.bubble.itemCycleTime", () => Config.BubbleItemCycleTime, min: 0.2f, max: 5f, interval: 0.1f);
		helper.AddEnumOption("config.bubble.sway", () => Config.BubbleSway);

		helper.AddSectionTitle("config.appearance.section");
		helper.AddEnumOption("config.appearance.splitScreenScreens", valuePrefix: "config.splitScreenScreens", property: () => Config.SplitScreenScreens);
		helper.AddKeybindList("config.appearance.visibilityKeybind", () => Config.VisibilityKeybind);
		helper.AddNumberOption("config.appearance.alpha.focused", () => Config.FocusedAlpha, min: 0f, max: 1f, interval: 0.05f);
		helper.AddNumberOption("config.appearance.alpha.normal", () => Config.NormalAlpha, min: 0f, max: 1f, interval: 0.05f);

		helper.AddSectionTitle("config.groupingSorting.section");
		helper.AddEnumOption("config.groupingSorting.grouping", () => Config.Grouping);
		for (int i = 0; i < Math.Max(Config.Sorting.Count + 1, 3); i++)
		{
			int loopI = i;
			helper.AddEnumOption(
				"config.groupingSorting.sorting",
				valuePrefix: "config.sorting",
				tokens: new { Ordinal = loopI + 1 },
				getValue: () => loopI < Config.Sorting.Count ? Config.Sorting[loopI] : MachineRenderingOptions.Sorting.None,
				setValue: value =>
				{
					while (loopI >= Config.Sorting.Count)
						Config.Sorting.Add(MachineRenderingOptions.Sorting.None);
					Config.Sorting[loopI] = value;
					while (Config.Sorting.Count > 0 && Config.Sorting.Last() == MachineRenderingOptions.Sorting.None)
						Config.Sorting.RemoveAt(Config.Sorting.Count - 1);
				}
			);
		}

		helper.AddSectionTitle("config.show.section");
		SetupStateConfig("config.show.ready", "config.show.ready.exceptions", () => Config.ShowReady);
		SetupStateConfig("config.show.waiting", "config.show.waiting.exceptions", () => Config.ShowWaiting);
		SetupStateConfig("config.show.busy", "config.show.busy.exceptions", () => Config.ShowBusy);
		SetupExceptionsPage("config.show.ready.exceptions", MachineState.Ready, Config.ShowReadyExceptions);
		SetupExceptionsPage("config.show.waiting.exceptions", MachineState.Waiting, Config.ShowWaitingExceptions);
		SetupExceptionsPage("config.show.busy.exceptions", MachineState.Busy, Config.ShowBusyExceptions);

		IsConfigRegistered = true;
	}

	private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
	{
		DynamicGameAssetsApi = Helper.ModRegistry.GetApi<IDynamicGameAssetsApi>("spacechase0.DynamicGameAssets");

		SetupConfig();
		Visibility = MachineRenderingOptions.Visibility.Normal;
		VisibilityAlpha = Config.NormalAlpha;
		IsHoveredOver = false;

		var harmony = new Harmony(ModManifest.UniqueID);
		harmony.TryPatchVirtual(
			monitor: Monitor,
			original: () => AccessTools.Method(typeof(SObject), nameof(SObject.performObjectDropInAction)),
			prefix: new HarmonyMethod(typeof(ModEntry), nameof(SObject_performObjectDropInAction_Prefix)),
			postfix: new HarmonyMethod(typeof(ModEntry), nameof(SObject_performObjectDropInAction_Postfix))
		);
		harmony.TryPatchVirtual(
			monitor: Monitor,
			original: () => AccessTools.Method(typeof(SObject), nameof(SObject.checkForAction)),
			prefix: new HarmonyMethod(typeof(ModEntry), nameof(SObject_checkForAction_Prefix)),
			postfix: new HarmonyMethod(typeof(ModEntry), nameof(SObject_checkForAction_Postfix))
		);
	}

	private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
	{
		QueuedMachineUpdates.Clear();
		IgnoredMachinesForUpdates.Clear();
		LastPlayerTileLocation = null;

		HostMachines.Clear();
		ForceRefreshDisplayedMachines();
	}

	private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
	{
		if (!Context.IsWorldReady)
			return;

		if (GameExt.GetMultiplayerMode() != MultiplayerMode.Client)
		{
			foreach (var machine in QueuedMachineUpdates)
				UpdateMachine(machine);
			QueuedMachineUpdates.Clear();
		}

		if (Context.IsPlayerFree && Config.VisibilityKeybind.JustPressed())
		{
			Visibility = Visibility switch
			{
				MachineRenderingOptions.Visibility.Hidden => Config.FocusedAlpha == Config.NormalAlpha ? MachineRenderingOptions.Visibility.Focused : MachineRenderingOptions.Visibility.Normal,
				MachineRenderingOptions.Visibility.Normal => MachineRenderingOptions.Visibility.Focused,
				MachineRenderingOptions.Visibility.Focused => MachineRenderingOptions.Visibility.Hidden,
				_ => throw new ArgumentException($"{nameof(MachineRenderingOptions.Visibility)} has an invalid value."),
			};
		}

		var targetAlpha = Visibility switch
		{
			MachineRenderingOptions.Visibility.Hidden => 0f,
			MachineRenderingOptions.Visibility.Normal => IsHoveredOver ? Config.FocusedAlpha : Config.NormalAlpha,
			MachineRenderingOptions.Visibility.Focused => Config.FocusedAlpha,
			_ => throw new ArgumentException($"{nameof(MachineRenderingOptions.Visibility)} has an invalid value."),
		};

		VisibilityAlpha += (targetAlpha - VisibilityAlpha) * 0.15f;
		if (VisibilityAlpha <= 0.01f)
			VisibilityAlpha = 0f;
		else if (VisibilityAlpha >= 0.99f)
			VisibilityAlpha = 1f;

		var player = Game1.player;
		var newPlayerLocation = (player.currentLocation, player.Tile);
		if (Config.Sorting.Any(s => s is MachineRenderingOptions.Sorting.ByDistanceAscending or MachineRenderingOptions.Sorting.ByDistanceDescending))
			if (LastPlayerTileLocation is null || LastPlayerTileLocation.Value != newPlayerLocation)
				SortMachines(player);
		LastPlayerTileLocation = newPlayerLocation;
	}

	private void OnObjectListChanged(object? sender, ObjectListChangedEventArgs e)
	{
		if (GameExt.GetMultiplayerMode() == MultiplayerMode.Client)
			return;

		foreach (var @object in e.Removed)
			RemoveMachine(@object.Value);
		foreach (var @object in e.Added)
			StartTrackingMachine(@object.Value);
	}

	private void OnRenderedHud(object? sender, RenderedHudEventArgs e)
	{
		if (!Context.IsWorldReady || Game1.eventUp)
			return;
		if (!Config.SplitScreenScreens.MatchesCurrentScreen())
			return;
		if (HostMachines.Count == 0 && ClientMachines.Count == 0)
			return;
		if (VisibilityAlpha <= 0f)
			return;

		UpdateFlowMachinesIfNeeded(Game1.player);
		if (FlowMachines.Count == 0)
			return;
		var minX = FlowMachines.Min(e => e.position.X);
		var minY = FlowMachines.Min(e => e.position.Y);
		var maxX = FlowMachines.Max(e => e.position.X);
		var maxY = FlowMachines.Max(e => e.position.Y);
		var width = maxX - minX + 1;
		var height = maxY - minY + 1;

		var viewportBounds = Game1.graphics.GraphicsDevice.Viewport.Bounds;
		var screenSize = new Vector2(viewportBounds.Size.X, viewportBounds.Size.Y);
		var panelSize = (SingleMachineSize * new Vector2(width, height) + Config.Spacing * new Vector2(width - 1, height - 1)) * Config.Scale;
		var panelLocation = Config.Anchor.GetAnchoredPoint(Vector2.Zero, screenSize, panelSize);

		var mouseLocation = Game1.getMousePosition();
		IsHoveredOver =
			mouseLocation.X >= panelLocation.X &&
			mouseLocation.Y >= panelLocation.Y &&
			mouseLocation.X < panelLocation.X + panelSize.X &&
			mouseLocation.Y < panelLocation.Y + panelSize.Y;

		foreach (var ((x, y), (machine, heldItems)) in FlowMachines)
		{
			float GetBubbleSwayOffset()
			{
				return Config.BubbleSway switch
				{
					MachineRenderingOptions.BubbleSway.Static => 0f,
					MachineRenderingOptions.BubbleSway.Together => 2f * (float)Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250), 2),
					MachineRenderingOptions.BubbleSway.Wave => 2f * (float)Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250 + x + y), 2),
					_ => throw new ArgumentException($"{nameof(MachineRenderingOptions.BubbleSway)} has an invalid value."),
				};
			}

			var machineUnscaledOffset = new Vector2(x - minX, y - minY) * SingleMachineSize + new Vector2(x - minX, y - minY) * Config.Spacing;
			var machineLocation = panelLocation + machineUnscaledOffset * Config.Scale;
			var machineState = GetMachineState(machine);

			ItemRenderer.DrawItem(
				e.SpriteBatch, machine,
				machineLocation,
				SingleMachineSize * Config.Scale,
				Color.White * VisibilityAlpha,
				StackDrawType.Draw
			);

			float timeVariableOffset = GetBubbleSwayOffset();

			void DrawEmote(int emoteX, int emoteY)
			{
				var xEmoteRectangle = new Rectangle(emoteX * 16, emoteY * 16, 16, 16);
				float emoteScale = 2.5f;

				e.SpriteBatch.Draw(
					Game1.emoteSpriteSheet,
					machineLocation + new Vector2(SingleMachineSize.X * 0.5f - xEmoteRectangle.Width * emoteScale * 0.5f, timeVariableOffset - xEmoteRectangle.Height * emoteScale * 0.5f) * Config.Scale,
					xEmoteRectangle,
					Color.White * 0.75f * VisibilityAlpha,
					0f, Vector2.Zero, emoteScale * Config.Scale, SpriteEffects.None, 0f
				);
			}

			switch (machineState)
			{
				case MachineState.Ready:
					{
						if (Config.ShowItemBubble)
						{
							var bubbleRectangle = new Rectangle(141, 465, 20, 24);
							float bubbleScale = 2f;

							e.SpriteBatch.Draw(
								Game1.mouseCursors,
								machineLocation + new Vector2(SingleMachineSize.X * 0.5f - bubbleRectangle.Width * bubbleScale * 0.5f, timeVariableOffset - bubbleRectangle.Height * bubbleScale * 0.5f) * Config.Scale,
								bubbleRectangle,
								Color.White * 0.75f * VisibilityAlpha,
								0f, Vector2.Zero, bubbleScale * Config.Scale, SpriteEffects.None, 0.91f
							);

							if (heldItems.Count == 0)
							{
								Monitor.LogOnce($"Detected invalid machine state `{machineState}` for machine `{machine.DisplayName}`, but there are no items inside.", LogLevel.Error);
								e.SpriteBatch.Draw(
									Game1.mouseCursors,
									machineLocation + new Vector2(SingleMachineSize.X * 0.5f, timeVariableOffset - 4) * Config.Scale,
									new Rectangle(322, 498, 12, 12),
									Color.White * VisibilityAlpha,
									0f,
									new Vector2(6),
									2f * Config.Scale,
									SpriteEffects.None,
									0f
								);
							}
							else
							{
								int heldItemVariableIndex = (int)(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / (1000.0 * Config.BubbleItemCycleTime)) % heldItems.Count;
								ItemRenderer.DrawItem(
									e.SpriteBatch, heldItems[heldItemVariableIndex],
									machineLocation + new Vector2(SingleMachineSize.X * 0.5f, timeVariableOffset - 4) * Config.Scale,
									new Vector2(24, 24) * bubbleScale * 0.6f * Config.Scale,
									Color.White * VisibilityAlpha,
									StackDrawType.HideButShowQuality,
									rectAnchorSide: UIAnchorSide.Center
								);
							}
						}
						else
						{
							DrawEmote(3, 0);
						}

						break;
					}
				case MachineState.Waiting:
					DrawEmote(0, 4);
					break;
				case MachineState.Busy:
					int frame = (int)(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 400) % 4;
					DrawEmote(frame, 10);
					break;
				default:
					throw new ArgumentException($"{nameof(MachineState)} has an invalid value.");
			}
		}
	}

	private void OnPeerConnected(object? sender, PeerConnectedEventArgs e)
	{
		if (GameExt.GetMultiplayerMode() != MultiplayerMode.Server)
			return;
		if (e.Peer.GetMod(ModManifest.UniqueID) is null)
			return;
		foreach (var (location, machine, state) in HostMachines)
			SendModMessage(NetMessage.MachineUpsert.Create(location, machine, state), e.Peer);
	}

	private void OnMachineUpsertMessageReceived(NetMessage.MachineUpsert message)
	{
		var existingEntry = ClientMachines.FirstOrNull(e => message.Location == e.location && message.MatchesMachine(e.machine));
		if (existingEntry is not null)
		{
			static bool IsHeldObjectEqual(SObject? @object, NetMessage.Entity.SObject? message)
			{
				if (@object is null && message is null)
					return true;
				if (@object is null != message is null)
					return false;
				return message!.Value.Matches(@object!);
			}

			if (existingEntry.Value.state == message.State && IsHeldObjectEqual(existingEntry.Value.machine.GetAnyHeldObject(), message.HeldObject))
				return;
			ClientMachines.Remove(existingEntry.Value);
			AreVisibleMachinesDirty = true;
		}

		var recreatedMachine = message.RetrieveMachine();
		ClientMachines.Add((message.Location, recreatedMachine, message.State));
		AreVisibleMachinesDirty = true;
	}

	private void OnMachineRemoveMessageReceived(NetMessage.MachineRemove message)
	{
		var existingEntry = ClientMachines.FirstOrNull(e => message.Location == e.location && message.MatchesMachine(e.machine));
		if (existingEntry is not null)
		{
			ClientMachines.Remove(existingEntry.Value);
			AreVisibleMachinesDirty = true;
		}
	}

	private void ForceRefreshDisplayedMachines()
	{
		PerScreenVisibleMachines.ResetAllScreens();
		PerScreenSortedMachines.ResetAllScreens();
		PerScreenGroupedMachines.ResetAllScreens();
		PerScreenFlowMachines.ResetAllScreens();

		PerScreenAreVisibleMachinesDirty.ResetAllScreens();
		PerScreenAreSortedMachinesDirty.ResetAllScreens();
		PerScreenAreGroupedMachinesDirty.ResetAllScreens();
		PerScreenAreFlowMachinesDirty.ResetAllScreens();

		if (!Context.IsWorldReady || Game1.currentLocation is null || GameExt.GetMultiplayerMode() == MultiplayerMode.Client)
			return;
		foreach (var location in GameExt.GetAllLocations())
			foreach (var @object in location.Objects.Values)
				StartTrackingMachine(@object);
	}

	private void UpdateFlowMachinesIfNeeded(Farmer player)
	{
		GroupMachinesIfNeeded(player);
		if (AreFlowMachinesDirty)
			UpdateFlowMachines();
	}

	private void UpdateFlowMachines()
	{
		List<((int column, int row) position, (SObject machine, List<SObject> heldItems) machine)> machineCoords = [];
		int column = 0;
		int row = 0;

		foreach (var entry in GroupedMachines)
		{
			machineCoords.Add((position: (column++, row), machine: entry));
			if (column == Config.MaxColumns)
			{
				column = 0;
				row++;
			}
		}

		var machineFlowCoords = machineCoords
			.Select(e => (position: Config.FlowDirection.GetXYPositionFromZeroOrigin(e.position), machine: e.machine))
			.Select(e => (position: new IntPoint(e.position.x, e.position.y), machine: e.machine))
			.OrderBy(e => e.position.Y)
			.ThenByDescending(e => e.position.X);
		FlowMachines.Clear();
		foreach (var entry in machineFlowCoords)
			FlowMachines.Add(entry);
		AreFlowMachinesDirty = false;
	}

	private void GroupMachinesIfNeeded(Farmer player)
	{
		SortMachinesIfNeeded(player);
		if (AreGroupedMachinesDirty)
			GroupMachines();
	}

	private void GroupMachines()
	{
		void AddHeldItem(List<SObject> heldItems, SObject newHeldItem)
		{
			foreach (var heldItem in heldItems)
			{
				if (heldItem.Name == newHeldItem.name)
					return;
			}
			heldItems.Add(newHeldItem);
		}

		List<SObject> CopyHeldItems(SObject machine)
		{
			var list = new List<SObject>();
			if (machine.TryGetAnyHeldObject(out var heldObject))
				AddHeldItem(list, heldObject);
			return list;
		}

		List<(SObject machine, List<SObject> heldItems)> results = [];
		foreach (var (location, machine, state) in SortedMachines)
		{
			switch (Config.Grouping)
			{
				case MachineRenderingOptions.Grouping.None:
					results.Add((CopyMachine(machine), new List<SObject>()));
					break;
				case MachineRenderingOptions.Grouping.ByMachine:
					foreach (var (result, resultHeldItems) in results)
					{
						if (
							machine.Name == result.Name && machine.readyForHarvest.Value == result.readyForHarvest.Value &&
							machine.GetAnyHeldObject()?.bigCraftable.Value == resultHeldItems.FirstOrDefault()?.bigCraftable.Value
						)
						{
							result.Stack++;
							if (machine.TryGetAnyHeldObject(out var heldObject))
								AddHeldItem(resultHeldItems, GetOne(heldObject));
							goto machineLoopContinue;
						}
					}
					results.Add((CopyMachine(machine), CopyHeldItems(machine)));
					break;
				case MachineRenderingOptions.Grouping.ByMachineAndItem:
					foreach (var (result, resultHeldItems) in results)
					{
						if (
							machine.Name == result.Name && machine.readyForHarvest.Value == result.readyForHarvest.Value &&
							machine.GetAnyHeldObject()?.bigCraftable.Value == resultHeldItems.FirstOrDefault()?.bigCraftable.Value &&
							machine.GetAnyHeldObject()?.Name == resultHeldItems.FirstOrDefault()?.Name
						)
						{
							result.Stack++;
							if (machine.TryGetAnyHeldObject(out var heldObject))
								AddHeldItem(resultHeldItems, GetOne(heldObject));
							goto machineLoopContinue;
						}
					}
					results.Add((CopyMachine(machine), CopyHeldItems(machine)));
					break;
				default:
					throw new ArgumentException($"{nameof(MachineRenderingOptions.Grouping)} has an invalid value.");
			}
			machineLoopContinue:;
		}

		var final = results.ToList();
		if (!final.SequenceEqual(GroupedMachines))
		{
			GroupedMachines.Clear();
			foreach (var entry in final)
				GroupedMachines.Add(entry);
			AreFlowMachinesDirty = true;
		}
		AreGroupedMachinesDirty = false;
	}

	private void SortMachinesIfNeeded(Farmer player)
	{
		UpdateVisibleMachinesIfNeeded();
		if (AreSortedMachinesDirty)
			SortMachines(player);
	}

	private void SortMachines(Farmer player)
	{
		IEnumerable<(LocationDescriptor location, SObject machine, MachineState state)> results = VisibleMachines;

		void SortResults<T>(bool ascending, Func<(LocationDescriptor location, SObject machine, MachineState state), T> keySelector) where T : IComparable<T>
		{
			results = results is IOrderedEnumerable<(LocationDescriptor location, SObject machine, MachineState state)> ordered
				? (ascending ? ordered.ThenBy(keySelector) : ordered.ThenByDescending(keySelector))
				: (ascending ? results.OrderBy(keySelector) : results.OrderByDescending(keySelector));
		}

		foreach (var sorting in Config.Sorting)
		{
			switch (sorting)
			{
				case MachineRenderingOptions.Sorting.None:
					break;
				case MachineRenderingOptions.Sorting.ByMachineAZ:
				case MachineRenderingOptions.Sorting.ByMachineZA:
					SortResults(
						sorting == MachineRenderingOptions.Sorting.ByMachineAZ,
						e => e.machine.DisplayName
					);
					break;
				case MachineRenderingOptions.Sorting.ReadyFirst:
					SortResults(false, e => e.state == MachineState.Ready);
					break;
				case MachineRenderingOptions.Sorting.WaitingFirst:
					SortResults(false, e => e.state == MachineState.Waiting);
					break;
				case MachineRenderingOptions.Sorting.BusyFirst:
					SortResults(false, e => e.state == MachineState.Busy);
					break;
				case MachineRenderingOptions.Sorting.ByDistanceAscending:
				case MachineRenderingOptions.Sorting.ByDistanceDescending:
					SortResults(
						sorting == MachineRenderingOptions.Sorting.ByDistanceAscending,
						e => e.location == LocationDescriptor.Create(player.currentLocation) ? (player.Tile - e.machine.TileLocation).Length() : float.PositiveInfinity
					);
					break;
				case MachineRenderingOptions.Sorting.ByItemAZ:
				case MachineRenderingOptions.Sorting.ByItemZA:
					SortResults(
						sorting == MachineRenderingOptions.Sorting.ByItemAZ,
						e => e.machine.GetAnyHeldObject()?.DisplayName ?? ""
					);
					break;
				default:
					throw new ArgumentException($"{nameof(MachineRenderingOptions.Sorting)} has an invalid value.");
			}
		}

		var final = results.ToList();
		if (!final.SequenceEqual(SortedMachines))
		{
			SortedMachines.Clear();
			foreach (var entry in final)
				SortedMachines.Add(entry);
			AreGroupedMachinesDirty = true;
		}
		AreSortedMachinesDirty = false;
	}

	private void UpdateVisibleMachinesIfNeeded()
	{
		if (AreVisibleMachinesDirty)
			UpdateVisibleMachines();
	}

	private void UpdateVisibleMachines()
	{
		var final = (GameExt.GetMultiplayerMode() == MultiplayerMode.Client ? ClientMachines : HostMachines)
			.Where(e => ShouldShowMachine(e.machine, e.state))
			.ToList();
		if (!final.SequenceEqual(VisibleMachines))
		{
			VisibleMachines.Clear();
			foreach (var entry in final)
				VisibleMachines.Add(entry);
			AreSortedMachinesDirty = true;
		}
		AreVisibleMachinesDirty = false;
	}

	private static bool MachineMatches(SObject machine, IEnumerable<IWildcardPattern> patterns)
		=> patterns.Any(p => p.Matches(machine.Name) || p.Matches(machine.DisplayName));

	[return: NotNullIfNotNull(nameof(@object))]
	private static SObject? GetOne(SObject? @object)
	{
		if (@object is CrabPot) // TODO: check if this is still needed
			return new CrabPot();
		else
			return @object?.getOne() as SObject;
	}

	private static SObject CopyMachine(SObject machine)
	{
		var newMachine = GetOne(machine);
		newMachine.TileLocation = machine.TileLocation;
		newMachine.readyForHarvest.Value = machine.readyForHarvest.Value;
		newMachine.showNextIndex.Value = machine.showNextIndex.Value;
		newMachine.heldObject.Value = GetOne(machine.heldObject?.Value);
		if (newMachine is CrabPot newCrabPot && machine is CrabPot crabPot)
			newCrabPot.bait.Value = GetOne(crabPot.bait?.Value);
		newMachine.MinutesUntilReady = machine.MinutesUntilReady;
		return newMachine;
	}

	private static bool IsLocationAccessible(GameLocation location)
	{
		if (location is Cellar cellar)
		{
			var farmHouse = cellar.GetFarmHouse();
			if (farmHouse is null)
				return false;
			return farmHouse.owner.HouseUpgradeLevel >= 3;
		}
		else if (location is FarmCave)
		{
			return GameExt.GetAllLocations().Where(l => l is FarmCave).First() == location;
		}
		return true;
	}

	private static MachineState GetMachineState(SObject machine)
	{
		if (machine is CrabPot crabPot)
			if (crabPot.bait.Value is not null && crabPot.heldObject.Value is null)
				return MachineState.Busy;
		if (machine is WoodChipper woodChipper)
			if (woodChipper.depositedItem.Value is not null && woodChipper.heldObject.Value is null)
				return MachineState.Busy;

		var readyForHarvest = machine.readyForHarvest.Value;
		var minutesUntilReady = machine.MinutesUntilReady;
		var heldObject = machine.GetAnyHeldObject();

		var result = GetMachineState(readyForHarvest, minutesUntilReady, heldObject);
		if (result == MachineState.Ready && readyForHarvest && minutesUntilReady <= 0 && heldObject is null && machine.GetType().Assembly.GetName().Name == "DynamicGameAssets")
		{
			Instance.Monitor.LogOnce($"Detected invalid machine state `{result}` for a DynamicGameAssets machine `{machine.DisplayName}`, but there are no items inside. However, this is a known issue with DynamicGameAssets.", LogLevel.Warn);
			return MachineState.Waiting;
		}
		return result;
	}

	private bool ShouldShowMachine(SObject machine, MachineState state)
		=> state switch
		{
			MachineState.Ready => Config.ShowReady != MachineMatches(machine, Config.ShowReadyExceptionPatterns),
			MachineState.Waiting => Config.ShowWaiting != MachineMatches(machine, Config.ShowWaitingExceptionPatterns),
			MachineState.Busy => Config.ShowBusy != MachineMatches(machine, Config.ShowBusyExceptionPatterns),
			_ => throw new InvalidOperationException(),
		};

	private static MachineState GetMachineState(bool readyForHarvest, int minutesUntilReady, SObject? heldObject)
	{
		// this is NOT a valid check for the "Waiting" state - it would break Incubator support:
		// `readyForHarvest || heldObject is null`

		if (readyForHarvest || (heldObject is not null && minutesUntilReady <= 0))
			return MachineState.Ready;
		else if (minutesUntilReady > 0)
			return MachineState.Busy;
		else
			return MachineState.Waiting;
	}

	private static bool IsMachine(SObject @object)
		=> @object is CrabPot || @object.GetMachineData() is not null;

	private bool UpsertMachine(SObject machine)
	{
		if (machine.Location is null)
			return false;

		var newState = GetMachineState(machine);
		var locationDescriptor = LocationDescriptor.Create(machine.Location);
		var existingEntry = HostMachines.FirstOrNull(e => e.location == locationDescriptor && e.machine.Name == machine.Name && e.machine.TileLocation == machine.TileLocation);
		if (existingEntry is null)
		{
			Monitor.Log($"Added {newState} machine {{Name: {machine.Name}, DisplayName: {machine.DisplayName}, Type: {machine.GetType().GetBestName()}}} in location {locationDescriptor}", LogLevel.Trace);
		}
		else
		{
			if (existingEntry.Value.state == newState)
				return false;
			HostMachines.Remove(existingEntry.Value);
		}
		HostMachines.Add((locationDescriptor, machine, newState));
		AreVisibleMachinesDirty = true;
		SendModMessageToEveryone(NetMessage.MachineUpsert.Create(locationDescriptor, machine, newState));
		return true;
	}

	private bool RemoveMachine(SObject machine)
	{
		if (machine.Location is null)
			return false;

		var locationDescriptor = LocationDescriptor.Create(machine.Location);
		var existingEntry = HostMachines.FirstOrNull(e => e.location == locationDescriptor && e.machine.Name == machine.Name && e.machine.TileLocation == machine.TileLocation);
		if (existingEntry is not null)
		{
			HostMachines.Remove(existingEntry.Value);
			SendModMessageToEveryone(NetMessage.MachineRemove.Create(machine.Location, machine));
			Monitor.Log($"Removed {existingEntry.Value.state} machine {{Name: {machine.Name}, DisplayName: {machine.DisplayName}, Type: {machine.GetType().GetBestName()}}} in location {locationDescriptor}", LogLevel.Trace);
			AreVisibleMachinesDirty = true;
		}
		return existingEntry is not null;
	}

	internal bool UpdateMachine(SObject machine)
	{
		if (machine.Location is null)
			return false;
		if (!IsMachine(machine))
			return false;
		if (!IsLocationAccessible(machine.Location))
			return RemoveMachine(machine);
		return UpsertMachine(machine);
	}

	[SuppressMessage("SMAPI.CommonErrors", "AvoidNetField:Avoid Netcode types when possible", Justification = "Registering for events")]
	internal void StartTrackingMachine(SObject machine)
	{
		if (GameExt.GetMultiplayerMode() == MultiplayerMode.Client)
			return;
		if (!IsMachine(machine))
			return;

		UpdateMachine(machine);
		foreach (var refToRemove in TrackedMachines.Where(r => !r.TryGetTarget(out _)).ToList())
			TrackedMachines.Remove(refToRemove);
		if (TrackedMachines.Any(r => r.TryGetTarget(out var trackedMachine) && machine == trackedMachine))
			return;

		machine.readyForHarvest.fieldChangeVisibleEvent += (_, oldValue, newValue) =>
		{
			if (IgnoredMachinesForUpdates.Contains(machine))
				return;
			var oldState = GetMachineState(oldValue, machine.MinutesUntilReady, machine.GetAnyHeldObject());
			var newState = GetMachineState(newValue, machine.MinutesUntilReady, machine.GetAnyHeldObject());
			if (newState != oldState)
				QueuedMachineUpdates.Add(machine);
		};
		machine.minutesUntilReady.fieldChangeVisibleEvent += (_, oldValue, newValue) =>
		{
			if (IgnoredMachinesForUpdates.Contains(machine))
				return;
			var oldState = GetMachineState(machine.readyForHarvest.Value, oldValue, machine.GetAnyHeldObject());
			var newState = GetMachineState(machine.readyForHarvest.Value, newValue, machine.GetAnyHeldObject());
			if (newState != oldState)
				QueuedMachineUpdates.Add(machine);
		};
		machine.heldObject.fieldChangeVisibleEvent += (_, oldValue, newValue) =>
		{
			if (IgnoredMachinesForUpdates.Contains(machine))
				return;
			var oldState = GetMachineState(machine.readyForHarvest.Value, machine.MinutesUntilReady, oldValue);
			var newState = GetMachineState(machine.readyForHarvest.Value, machine.MinutesUntilReady, newValue);
			if (newState != oldState)
				QueuedMachineUpdates.Add(machine);
		};
		if (machine is CrabPot crabPot)
		{
			crabPot.bait.fieldChangeVisibleEvent += (_, oldValue, newValue) =>
			{
				if (IgnoredMachinesForUpdates.Contains(machine))
					return;
				QueuedMachineUpdates.Add(machine);
			};
		}

		TrackedMachines.Add(new(machine));
	}

	private static void SObject_performObjectDropInAction_Prefix(SObject __instance, bool __1 /* probe */)
	{
		if (__1)
			Instance.IgnoredMachinesForUpdates.Add(__instance);
	}

	private static void SObject_performObjectDropInAction_Postfix(SObject __instance, bool __1 /* probe */)
	{
		if (__1)
			Instance.IgnoredMachinesForUpdates.Remove(__instance);
	}

	private static void SObject_checkForAction_Prefix(SObject __instance, bool __1 /* justCheckingForActivity */)
	{
		if (__1)
			Instance.IgnoredMachinesForUpdates.Add(__instance);
	}

	private static void SObject_checkForAction_Postfix(SObject __instance, bool __1 /* justCheckingForActivity */)
	{
		if (__1)
			Instance.IgnoredMachinesForUpdates.Remove(__instance);
	}
}