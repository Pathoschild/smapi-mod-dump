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
using System.Diagnostics.CodeAnalysis;
using System.IO;

using HarmonyLib;

using Leclair.Stardew.CloudySkies.Effects;
using Leclair.Stardew.CloudySkies.Integrations.ContentPatcher;
using Leclair.Stardew.CloudySkies.Integrations.UltimateFertilizer;
using Leclair.Stardew.CloudySkies.LayerData;
using Leclair.Stardew.CloudySkies.Layers;
using Leclair.Stardew.CloudySkies.Models;
using Leclair.Stardew.Common;
using Leclair.Stardew.Common.Events;
using Leclair.Stardew.Common.Integrations.GenericModConfigMenu;
using Leclair.Stardew.Common.Types;
using Leclair.Stardew.Common.UI;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

using StardewValley;
using StardewValley.Delegates;
using StardewValley.Extensions;
using StardewValley.Mods;

namespace Leclair.Stardew.CloudySkies;


public partial class ModEntry : PintailModSubscriber {

	public static ModEntry Instance { get; private set; } = null!;

	public const string WEATHER_TOTEM_DATA = @"leclair.cloudyskies/WeatherTotem";

	public const string ALLOW_TOTEM_DATA = @"leclair.cloudyskies/AllowTotem:";

	public const string HISTORY_DATA = @"Mods/leclair.cloudyskies/History";
	public const string DATA_ASSET = @"Mods/leclair.cloudyskies/WeatherData";
	public const string EXTENSION_DATA_ASSET = @"Mods/leclair.cloudyskies/LocationContextExtensionData";

	public const string WEATHER_OVERLAY_DEFAULT_ASSET = @"Mods/leclair.cloudyskies/Textures/WeatherOverlay";
	public const string GINGER_ISLE_BASE_ASSET = @"Mods/leclair.cloudyskies/Textures/GingerIsleBase";

	private static readonly Func<ModHooks> HookDelegate = AccessTools.Field(typeof(Game1), "hooks").CreateGetter<ModHooks>();

#nullable disable

	public ModConfig Config { get; private set; }

	internal Harmony Harmony;

#nullable enable

	internal GMCMIntegration<ModConfig, ModEntry>? intGMCM;
	internal CPIntegration? intCP;
	internal UFIntegration? intUF;

	private ulong lastLayerId = 0;

	internal readonly Dictionary<ulong, HashSet<string>> AssetsByLayer = new();
	internal readonly Dictionary<string, HashSet<ulong>> AssetsByName = new();

	internal Dictionary<string, WeatherData>? Data;
	internal Dictionary<string, LocationContextExtensionData>? ContextData;

	internal readonly Dictionary<string, Func<ulong, ILayerData, IWeatherLayer?>> LayerFactories = new();
	internal readonly Dictionary<string, Func<ulong, IEffectData, IWeatherEffect?>> EffectFactories = new();

	internal readonly PerScreen<double> UpdateTiming = new();
	internal readonly PerScreen<double> DrawTiming = new();

	public override void Entry(IModHelper helper) {
		base.Entry(helper);

		Instance = this;

		RegisterBuiltinEffects();
		RegisterBuiltinLayers();

		// Harmony
		Harmony = new Harmony(ModManifest.UniqueID);

		Patches.PatchHelper.Init(this);

		Patches.DayTimeMoneyBox_Patches.Patch(this);
		Patches.Game1_Patches.Patch(this);
		Patches.GameLocation_Patches.Patch(this);
		Patches.LocationWeather_Patches.Patch(this);
		Patches.Music_Patches.Patch(this);
		Patches.SObject_Patches.Patch(this);
		Patches.TV_Patches.Patch(this);

		// Read Config
		Config = Helper.ReadConfig<ModConfig>();

		// Init
		I18n.Init(Helper.Translation);
	}

	public override object? GetApi(IModInfo mod) {
		return new ModApi(this, mod.Manifest);
	}

	#region Weather History

	private string? WeatherHistorySource;
	internal Dictionary<string, Dictionary<int, string>>? WeatherHistory;

	[MemberNotNull(nameof(WeatherHistory))]
	internal void LoadWeatherHistory() {
		if (!Context.IsWorldReady)
			throw new Exception("Tried loading history before world is ready.");

		Game1.MasterPlayer.modData.TryGetValue(HISTORY_DATA, out string? source);
		if (WeatherHistory != null && WeatherHistorySource == source)
			return;

		WeatherHistorySource = source;

		if (source != null)
			try {
				WeatherHistory = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<int, string>>>(source);
			} catch (Exception ex) {
				Log($"Unable to read weather history data: {ex}", LogLevel.Error);
			}

		WeatherHistory ??= new();
	}

	internal void SaveWeatherHistory() {
		if (!Game1.IsMasterGame) {
			Log($"Attempted to save weather history from non-master game. Ignoring.", LogLevel.Warn);
			return;
		}

		if (WeatherHistory != null && WeatherHistory.Count > 0) {
			WeatherHistorySource = JsonConvert.SerializeObject(WeatherHistory);
			Game1.MasterPlayer.modData[HISTORY_DATA] = WeatherHistorySource;
		} else {
			WeatherHistorySource = null;
			Game1.MasterPlayer.modData.Remove(HISTORY_DATA);
		}
	}

	internal void RecordWeatherToday(string contextId, string weather) {
		// Do not record history when setting up the world.
		if (!Context.IsWorldReady)
			return;

		// Do not record history from contexts that copy their weather.
		if (Game1.locationContextData.TryGetValue(contextId, out var data) && data.CopyWeatherFromLocation != null)
			return;

		LoadWeatherHistory();

		int today = Game1.Date.TotalDays;

		if (!WeatherHistory.TryGetValue(contextId, out var ctxHistory)) {
			ctxHistory = [];
			WeatherHistory[contextId] = ctxHistory;
		}

		// If we already recorded it, no need to re-record it.
		if (ctxHistory.TryGetValue(today, out string? recorded) && recorded == weather)
			return;

		ctxHistory[today] = weather;

		// Record a full week and nothing more.
		ctxHistory.RemoveWhere(pair => today - pair.Key > 7);

		Log($"Recorded Weather for {contextId}. On {today} ({Game1.Date}), the weather is: {weather}", LogLevel.Trace);
	}

	internal bool TryGetWeatherHistory(GameLocation location, WorldDate date, [NotNullWhen(true)] out string? weatherId) {
		return TryGetWeatherHistory(location.GetLocationContextId(), date.TotalDays, out weatherId);
	}

	internal bool TryGetWeatherHistory(GameLocation location, int date, [NotNullWhen(true)] out string? weatherId) {
		return TryGetWeatherHistory(location.GetLocationContextId(), date, out weatherId);
	}

	internal bool TryGetWeatherHistory(string? contextId, WorldDate date, [NotNullWhen(true)] out string? weatherId) {
		return TryGetWeatherHistory(contextId, date.TotalDays, out weatherId);
	}

	[return: NotNullIfNotNull(nameof(contextId))]
	internal static string? GetSourceContext(string? contextId) {
		if (contextId == null || Game1.locationContextData is null)
			return contextId;

		int i = 0;
		string target = contextId;

		while (i < 100) {
			if (Game1.locationContextData.TryGetValue(target, out var data) && data.CopyWeatherFromLocation != null)
				target = data.CopyWeatherFromLocation;
			else
				break;

			i++;
		}

		return target;
	}

	internal bool TryGetWeatherHistory(string? contextId, int day, [NotNullWhen(true)] out string? weatherId) {
		if (Context.IsWorldReady && !string.IsNullOrEmpty(contextId)) {
			LoadWeatherHistory();
			if (WeatherHistory.TryGetValue(GetSourceContext(contextId), out var ctxHistory) && ctxHistory.TryGetValue(day, out weatherId))
				return true;
		}

		weatherId = null;
		return false;
	}

	#endregion

	#region Effects and Layer Registration

	public bool RegisterLayerType(string type, Func<ulong, ILayerData, IWeatherLayer?> factory) {
		if (LayerFactories.TryAdd(type, factory)) {
			UncacheLayers();
			return true;
		}

		return false;
	}

	public bool UnregisterLayerType(string type) {
		if (LayerFactories.Remove(type)) {
			UncacheLayers();
			return true;
		}

		return false;
	}

	public bool RegisterEffectType(string type, Func<ulong, IEffectData, IWeatherEffect?> factory) {
		if (EffectFactories.TryAdd(type, factory)) {
			UncacheEffects();
			return true;
		}

		return false;
	}

	public bool UnregisterEffectType(string type) {
		if (EffectFactories.Remove(type)) {
			UncacheEffects();
			return true;
		}

		return false;
	}

	internal void RegisterBuiltinLayers() {
		RegisterLayerType("Color", (id, data) => data is IColorLayerData colorData ? new ColorLayer(id, colorData) : null);
		RegisterLayerType("Debris", (id, data) => data is IDebrisLayerData debrisData ? new DebrisLayer(this, id, debrisData) : null);
		RegisterLayerType("Particle", (id, data) => data is ParticleLayerData particleData ? new ParticleLayer(this, id, particleData) : null);
		RegisterLayerType("Rain", (id, data) => data is IRainLayerData rainData ? new RainLayer(this, id, rainData) : null);
		RegisterLayerType("Snow", (id, data) => data is ITextureScrollLayerData snowData ? new TextureScrollLayer(this, id, snowData) : null);
		RegisterLayerType("TextureScroll", (id, data) => data is ITextureScrollLayerData snowData ? new TextureScrollLayer(this, id, snowData) : null);
		RegisterLayerType("Shader", (id, data) => data is IShaderLayerData shaderData ? new ShaderLayer(this, id, shaderData) : null);
	}

	internal void RegisterBuiltinEffects() {
		RegisterEffectType("Buff", (id, data) => data is IBuffEffectData buffData ? new BuffEffect(this, id, buffData) : null);
		RegisterEffectType("ModifyHealth", (id, data) => data is IModifyHealthEffectData healthData ? new ModifyHealthEffect(id, healthData) : null);
		RegisterEffectType("ModifyStamina", (id, data) => data is IModifyStaminaEffectData staminaData ? new ModifyStaminaEffect(id, staminaData) : null);
		RegisterEffectType("Trigger", (id, data) => data is ITriggerEffectData triggerData ? new TriggerEffect(this, id, triggerData) : null);
	}

	#endregion

	#region Configuration

	private void RegisterSettings() {
		if (intGMCM is null || !intGMCM.IsLoaded)
			return;

		intGMCM.Unregister();
		intGMCM.Register(true);

		intGMCM
			.Add(
				I18n.Setting_ReplaceTvMenu,
				I18n.Setting_ReplaceTvMenu_About,
				c => c.ReplaceTVMenu,
				(c, v) => c.ReplaceTVMenu = v
			)
			.Add(
				I18n.Setting_CustomGingerIsle,
				I18n.Setting_CustomGingerIsle_About,
				c => c.UseCustomGingerIsleArt,
				(c, v) => c.UseCustomGingerIsleArt = v
			)
			.Add(
				I18n.Setting_WeatherTooltip,
				I18n.Setting_WeatherTooltip_About,
				c => c.ShowWeatherTooltip,
				(c, v) => c.ShowWeatherTooltip = v
			)
			.AddLabel("")
			.AddLabel(I18n.Setting_Shaders, I18n.Setting_Shaders_Description, shortcut: "shaders")
			.AddLabel(I18n.Setting_Development, shortcut: "dev");

		intGMCM.StartPage("dev", I18n.Setting_Development)
			.Add(
				I18n.Setting_Debug,
				I18n.Setting_Debug_About,
				c => c.ShowDebugTiming,
				(c, v) => c.ShowDebugTiming = v
			)
			.Add(
				I18n.Setting_Debug_Shaders,
				I18n.Setting_Debug_Shaders_About,
				c => c.RecompileShaders,
				(c, v) => c.RecompileShaders = v
			);

		intGMCM.StartPage("shaders", I18n.Setting_Shaders)
			.AddParagraph(I18n.Setting_Shaders_Description)
			.Add(
				I18n.Setting_Shaders_Enable,
				I18n.Setting_Shaders_Enable_About,
				c => c.AllowShaders,
				(c, v) => {
					c.AllowShaders = v;
					UncacheLayers();
				}
			)
			.AddLabel(I18n.Setting_Shaders_Specific);

		LoadWeatherData();

		HashSet<string> shaders = [];

		foreach (var entry in Data.Values) {
			if (entry.Layers is not null)
				foreach (var layer in entry.Layers)
					if (layer is IShaderLayerData sld && !string.IsNullOrEmpty(sld.Shader))
						shaders.Add(Path.GetFileNameWithoutExtension(sld.Shader));
		}

		foreach (string shader in shaders) {
			Func<string> name;
			var tl = Helper.Translation.Get($"setting.shader.{shader}");
			if (tl.HasValue())
				name = tl.ToString;
			else
				name = () => shader;

			intGMCM.Add(
				name,
				null,
				c => !c.DisabledShaders.Contains(shader),
				(c, v) => {
					if (!v ? c.DisabledShaders.Add(shader) : c.DisabledShaders.Remove(shader))
						UncacheLayers();
				}
			);
		}

	}

	private void ResetConfig() {
		bool use_custom_ginger_isle = Config.UseCustomGingerIsleArt;

		Config = new();

		if (use_custom_ginger_isle != Config.UseCustomGingerIsleArt)
			Helper.GameContent.InvalidateCache(EXTENSION_DATA_ASSET);
	}

	private void SaveConfig() {
		Helper.WriteConfig(Config);
	}

	#endregion

	#region Events

	[Subscriber]
	[EventPriority((EventPriority) int.MinValue)]
	private void AfterGameLaunched(object? sender, GameLaunchedEventArgs e) {
		var builder = ReflectionHelper.WhatPatchesMe(this, "  ", false);
		if (builder is not null)
			Log($"Detected Harmony Patches:\n{builder}", LogLevel.Trace);
	}

	[Subscriber]
	private void OnGameLaunched(object? sender, GameLaunchedEventArgs e) {
		intGMCM = new(this, () => Config, ResetConfig, SaveConfig);
		intCP = new(this);
		intUF = new(this);

		RegisterSettings();

	}

	[Subscriber]
	private void OnWindowResized(object? sender, WindowResizedEventArgs e) {
		// Send a resize event to all our layers.
		var layers = CachedLayers.Value;
		if (layers.HasValue && layers.Value.Layers is not null)
			foreach (var layer in layers.Value.Layers)
				layer.Resize(e.NewSize, e.OldSize);
	}

	[Subscriber]
	private void OnAssetRequested(object? sender, AssetRequestedEventArgs e) {

		if (e.Name.IsEquivalentTo(DATA_ASSET))
			e.LoadFrom(() => new Dictionary<string, WeatherData>(), priority: AssetLoadPriority.Exclusive);

		else if (e.Name.IsEquivalentTo(WEATHER_OVERLAY_DEFAULT_ASSET))
			e.LoadFromModFile<Texture2D>("assets/WeatherChannelOverlay.png", AssetLoadPriority.Low);

		else if (e.Name.IsEquivalentTo(GINGER_ISLE_BASE_ASSET))
			e.LoadFromModFile<Texture2D>("assets/GingerIsleBase.png", AssetLoadPriority.Low);

		else if (e.Name.IsEquivalentTo(EXTENSION_DATA_ASSET)) {
			var data = new Dictionary<string, LocationContextExtensionData>() {
				{ "Default", new() {
					DisplayName = "[LocalizedText location.stardew-valley]",
					IncludeInWeatherChannel = true,
				} },
				{ "Desert", new() {
					DisplayName = "[LocalizedText location.calico-desert]",
					IncludeInWeatherChannel = true,
					WeatherChannelCondition = "PLAYER_HAS_MAIL Host ccVault Any",
					WeatherChannelBackgroundTexture = "LooseSprites\\map"
				} }
			};

			if (Config.UseCustomGingerIsleArt)
				data["Island"] = new() {
					IncludeInWeatherChannel = true,
					DisplayName = "[LocalizedText Strings\\StringsFromCSFiles:IslandName]",
					WeatherChannelCondition = "PLAYER_HAS_MAIL Current Visited_Island Any",
					WeatherForecastPrefix = "[LocalizedText Strings\\StringsFromCSFiles:TV_IslandWeatherIntro]",
					WeatherChannelBackgroundTexture = GINGER_ISLE_BASE_ASSET,
					WeatherChannelBackgroundFrames = 4,
					WeatherChannelBackgroundSpeed = 120f,
					WeatherChannelOverlayTexture = GINGER_ISLE_BASE_ASSET,
					WeatherChannelOverlayIntroSource = new Point(0, 28),
					WeatherChannelOverlayIntroFrames = 1,
					WeatherChannelOverlayWeatherSource = new Point(42, 28),
					WeatherChannelOverlayWeatherFrames = 1
				};

			else
				data["Island"] = new() {
					IncludeInWeatherChannel = true,
					DisplayName = "[LocalizedText Strings\\StringsFromCSFiles:IslandName]",
					WeatherChannelCondition = "PLAYER_HAS_MAIL Current Visited_Island Any",
					WeatherForecastPrefix = "[LocalizedText Strings\\StringsFromCSFiles:TV_IslandWeatherIntro]",
					WeatherChannelOverlayTexture = "LooseSprites\\Cursors2",
					WeatherChannelOverlayIntroSource = new Point(148, 62),
					WeatherChannelOverlayWeatherSource = new Point(148, 62)
				};

			e.LoadFrom(() => data, priority: AssetLoadPriority.Exclusive);
		}
	}

	[Subscriber]
	private void OnAssetsInvalidated(object? sender, AssetsInvalidatedEventArgs e) {

		HashSet<ulong> MatchedLayers = new();

		foreach (var name in e.Names) {
			if (name.IsEquivalentTo(DATA_ASSET)) {
				Log($"Invalidated our weather data.", LogLevel.Debug);
				Data = null;
				CachedTryGetName.ResetAllScreens();
				CachedTryGetValue.ResetAllScreens();
				CachedWeather.ResetAllScreens();
			}

			if (name.IsEquivalentTo(EXTENSION_DATA_ASSET)) {
				Log($"Invalidated our location context extension data.", LogLevel.Debug);
				ContextData = null;
			}

			if (AssetsByName.TryGetValue(name.BaseName, out var layers))
				foreach (ulong id in layers)
					MatchedLayers.Add(id);
		}

		// TODO: Better invalidation logic to apply certain changes immediately
		// for a better developer experience.

		if (Data is not null && MatchedLayers.Count > 0) {
			foreach (var pair in CachedLayers.GetActiveValues()) {
				if (pair.Value.HasValue && pair.Value.Value.Layers is not null)
					foreach (var layer in pair.Value.Value.Layers) {
						if (MatchedLayers.Contains(layer.Id))
							layer.ReloadAssets();
					}
			}
			foreach (var pair in CachedEffects.GetActiveValues()) {
				if (pair.Value.HasValue && pair.Value.Value.Effects is not null)
					foreach (var effect in pair.Value.Value.Effects) {
						if (MatchedLayers.Contains(effect.Id))
							effect.ReloadAssets();
					}
			}
		}
	}

	[Subscriber]
	private void OnRenderedHud(object? sender, RenderedHudEventArgs e) {

		double drawing = DrawTiming.Value;
		double updating = UpdateTiming.Value;

		DrawTiming.Value = 0;
		UpdateTiming.Value = 0;

		if (!Config.ShowDebugTiming || Game1.game1.takingMapScreenshot || (drawing <= 0 && updating <= 0))
			return;

		var builder = SimpleHelper.Builder()
			.Text(string.Format("Update: {0:0.0000} ms", updating))
			.Text(string.Format("  Draw: {0:0.0000} ms", drawing));

		if (Game1.oldKBState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift)) {
			var weather = Game1.currentLocation.GetWeather();

			builder = builder
				.Divider(false)
				.Text($"Weather: {weather.Weather}")
				.Text($"Tomorrow: {weather.WeatherForTomorrow}")
				.Divider(false)
				.Text($"Raining: {weather.IsRaining}")
				.Text($"Snowing: {weather.IsSnowing}")
				.Text($"Lightning: {weather.IsLightning}")
				.Text($"Debris: {weather.IsDebrisWeather}")
				.Text($"???: {weather.IsGreenRain}")
				.Divider(false)
				.Text($"Draw Lighting: {Game1.drawLighting}")
				.Text($"Ambient: {Game1.ambientLight}")
				.Text($"Outdoor: {Game1.outdoorLight}");
		}

		builder
			.GetLayout()
			.DrawHover(e.SpriteBatch, Game1.smallFont, overrideX: 0, overrideY: 0);

	}

	[Subscriber]
	private void OnDayStarted(object? sender, DayStartedEventArgs e) {

		foreach (var pair in Game1.locationContextData) {
			if (pair.Value.CopyWeatherFromLocation != null)
				continue;

			string? weather = Game1.netWorldState.Value?.GetWeatherForLocation(pair.Key)?.Weather;
			if (weather is not null)
				RecordWeatherToday(pair.Key, weather);
		}

		if (Game1.IsMasterGame)
			SaveWeatherHistory();

	}

	[Subscriber]
	private void OnUpdating(object? sender, UpdateTickingEventArgs e) {

		// Clear the cache every tick to keep stale items from sticking around.
		Triggers.CachedItems.Clear();

		// Do not process effects when time is not passing. We need a special
		// check for when the process is out of focus, since this event still
		// fires in that situation. Also don't process effects in events.
		if (!Game1.game1.IsActiveNoOverlay && Game1.options.pauseWhenOutOfFocus && Game1.multiplayerMode == 0)
			return;
		if (Game1.eventUp || !Game1.shouldTimePass())
			return;

		// See if we have effects. If we don't, return.
		var effects = GetCachedWeatherEffects(Game1.player!.currentLocation, Game1.timeOfDay);
		if (effects is null)
			return;

		foreach (var effect in effects) {
			// Only run effects at a multiple of their Rate.
			if (e.IsMultipleOf(effect.Rate))
				effect.Update(Game1.currentGameTime);
		}
	}

	#endregion

	#region Loading

	[MemberNotNull(nameof(ContextData))]
	internal void LoadContextData() {
		if (ContextData is not null)
			return;

		ContextData = Helper.GameContent.Load<Dictionary<string, LocationContextExtensionData>>(EXTENSION_DATA_ASSET);

		// Normalize all the Id fields.
		foreach (var entry in ContextData)
			entry.Value.Id = entry.Key;
	}


	public bool TryGetContextData(string? id, [NotNullWhen(true)] out LocationContextExtensionData? data) {
		if (id is null) {
			data = null;
			return false;
		}

		LoadContextData();
		return ContextData.TryGetValue(id, out data);
	}

	[MemberNotNull(nameof(Data))]
	internal void LoadWeatherData() {
		if (Data is not null)
			return;

		Data = Helper.GameContent.Load<Dictionary<string, WeatherData>>(DATA_ASSET);

		// Normalize all the Id fields, as well as de-duplicating layer Ids.
		foreach (var entry in Data) {
			entry.Value.Id = entry.Key;

			// Un-box all the things we can.
			if (entry.Value.Lighting is not null) {
				for (int i = 0; i < entry.Value.Lighting.Count; i++) {
					var item = entry.Value.Lighting[i];
					if (item is not ScreenTintData && TryUnproxy(item, out object? obj) && obj is ScreenTintData sd)
						entry.Value.Lighting[i] = sd;
				}
			}

			if (entry.Value.Layers is not null) {
				for (int i = 0; i < entry.Value.Layers.Count; i++) {
					var item = entry.Value.Layers[i];
					if (item is not BaseLayerData && TryUnproxy(item, out object? obj) && obj is BaseLayerData ld)
						item = entry.Value.Layers[i] = ld;

					// While we're unboxing, make sure any CustomLayerData is
					// actually using ValueEqualityDictionary like we want.
					//if (item is CustomLayerData cd && cd.Fields is not FieldsEqualityDictionary)
					//cd.Fields = new FieldsEqualityDictionary(cd.Fields);

					// Same with ShaderData.
					//if (item is ShaderData sd && sd.Fields is not FieldsEqualityDictionary)
					//sd.Fields = new FieldsEqualityDictionary(sd.Fields);
				}
			}

			if (entry.Value.Effects is not null) {
				for (int i = 0; i < entry.Value.Effects.Count; i++) {
					var item = entry.Value.Effects[i];
					if (item is not BaseEffectData && TryUnproxy(item, out object? obj) && obj is BaseEffectData ed)
						entry.Value.Effects[i] = ed;

					// While we're unboxing, make sure any CustomEffectData is
					// actually using ValueEqualityDictionary like we want.
					if (item is CustomEffectData cd && cd.Fields is not ValueEqualityDictionary<string, JToken>)
						cd.Fields = new ValueEqualityDictionary<string, JToken>(cd.Fields);
				}
			}

			// Migrate to the new lighting format.
			if (entry.Value.AmbientColor.HasValue || entry.Value.LightingTint.HasValue || entry.Value.PostLightingTint.HasValue) {
				if (entry.Value.Lighting is null || entry.Value.Lighting.Count == 0) {
					entry.Value.Lighting ??= new List<IScreenTintData>();

					if (entry.Value.AmbientOutdoorOpacity.HasValue && entry.Value.AmbientOutdoorOpacity.Value < 0.93) {
						entry.Value.Lighting.Add(new ScreenTintData() {
							AmbientColor = entry.Value.AmbientColor,
							AmbientOutdoorOpacity = entry.Value.AmbientOutdoorOpacity,
							LightingTint = entry.Value.LightingTint,
							LightingTintOpacity = entry.Value.LightingTintOpacity,
							PostLightingTint = entry.Value.PostLightingTint,
							PostLightingTintOpacity = entry.Value.PostLightingTintOpacity
						});

						entry.Value.Lighting.Add(new ScreenTintData() {
							AmbientColor = entry.Value.AmbientColor,
							TimeOfDay = -200,
							TweenMode = LightingTweenMode.After,
							AmbientOutdoorOpacity = entry.Value.AmbientOutdoorOpacity,
							LightingTint = entry.Value.LightingTint,
							LightingTintOpacity = entry.Value.LightingTintOpacity,
							PostLightingTint = entry.Value.PostLightingTint,
							PostLightingTintOpacity = entry.Value.PostLightingTintOpacity
						});

						entry.Value.Lighting.Add(new ScreenTintData() {
							AmbientColor = entry.Value.AmbientColor,
							TimeOfDay = 0,
							AmbientOutdoorOpacity = 0.93f,
							LightingTint = entry.Value.LightingTint,
							LightingTintOpacity = entry.Value.LightingTintOpacity,
							PostLightingTint = entry.Value.PostLightingTint,
							PostLightingTintOpacity = entry.Value.PostLightingTintOpacity
						});

					} else
						entry.Value.Lighting.Add(new ScreenTintData() {
							AmbientColor = entry.Value.AmbientColor,
							AmbientOutdoorOpacity = entry.Value.AmbientOutdoorOpacity,
							LightingTint = entry.Value.LightingTint,
							LightingTintOpacity = entry.Value.LightingTintOpacity,
							PostLightingTint = entry.Value.PostLightingTint,
							PostLightingTintOpacity = entry.Value.PostLightingTintOpacity
						});

				} else
					Log($"Ignoring legacy lighting data for weather type '{entry.Key}' because Lighting is present and has data.", LogLevel.Warn);
			}

			if (entry.Value.Layers is not null) {
				HashSet<string> seen_ids = new();
				int i = 0;

				foreach (var layer in entry.Value.Layers) {
					if (string.IsNullOrEmpty(layer.Id) || !seen_ids.Add(layer.Id)) {
						layer.Id = $"{i}#{layer.Type}";
						seen_ids.Add(layer.Id);
					}
					i++;
				}
			}
		}
	}

	private readonly PerScreen<string?> CachedTryGetName = new();
	private readonly PerScreen<WeatherData?> CachedTryGetValue = new();

	/// <summary>
	/// Try to get weather data for the weather with the provided key.
	/// This uses an internal lookup cache between calls to improve
	/// performance, only falling back to a dictionary lookup if both
	/// caches aren't set to the value.
	/// </summary>
	/// <param name="key">The Id of the weather condition we want data for.</param>
	/// <param name="weather">The data, if it exists.</param>
	/// <returns>Whether or not the data exists.</returns>
	public bool TryGetWeather(string? key, [NotNullWhen(true)] out WeatherData? weather) {
		if (key is null) {
			weather = null;
			return false;
		}

		if (key == CachedTryGetName.Value) {
			weather = CachedTryGetValue.Value;
			return weather is not null;
		}

		if (key == CachedWeatherName.Value) {
			weather = CachedWeather.Value;
			return weather is not null;
		}

		LoadWeatherData();
		Data.TryGetValue(key, out weather);

		CachedTryGetName.Value = key;
		CachedTryGetValue.Value = weather;

		return weather is not null;
	}

	public void MarkLoadsAsset(ulong id, string path) {
		if (!AssetsByLayer.TryGetValue(id, out var layerAssets)) {
			layerAssets = new();
			AssetsByLayer[id] = layerAssets;
		}

		if (!layerAssets.Add(path))
			return;

		if (!AssetsByName.TryGetValue(path, out var assetLayers)) {
			assetLayers = new();
			AssetsByName[path] = assetLayers;
		}

		assetLayers.Add(id);
	}

	public void RemoveLoadsAsset(ulong id) {
		if (!AssetsByLayer.TryGetValue(id, out var layerAssets))
			return;

		AssetsByLayer.Remove(id);

		foreach (string path in layerAssets) {
			if (AssetsByName.TryGetValue(path, out var assetLayers) && assetLayers.Remove(id)) {
				if (assetLayers.Count == 0)
					AssetsByName.Remove(path);
			}
		}
	}

	public void RemoveLoadsAsset(ulong id, string path) {
		if (!AssetsByLayer.TryGetValue(id, out var layerAssets) || !layerAssets.Remove(path))
			return;

		if (AssetsByName.TryGetValue(path, out var assetLayers))
			assetLayers.Remove(id);
	}

	#endregion

	#region Per-Screen Data Cache

	private readonly PerScreen<string?> CachedWeatherName = new();
	private readonly PerScreen<WeatherData?> CachedWeather = new();
	private readonly PerScreen<LayerCache?> CachedLayers = new();
	internal readonly PerScreen<bool> HasShaderLayer = new();
	private readonly PerScreen<EffectCache?> CachedEffects = new();
	private readonly PerScreen<CachedTintData?> CachedTint = new();

	internal void UncacheLayers(string? weatherId = null, bool force_reinstance = false) {
		foreach (var entry in CachedLayers.GetActiveValues()) {
			string? id = CachedWeatherName.GetValueForScreen(entry.Key);
			if (weatherId != null && id != weatherId)
				continue;

			if (entry.Value.HasValue) {
				var thing = entry.Value.Value;
				thing.Hour = force_reinstance ? -2 : -1;
				CachedLayers.SetValueForScreen(entry.Key, thing);
			}
		}
	}

	internal void UncacheTint() {
		foreach (var entry in CachedTint.GetActiveValues()) {
			if (entry.Value.HasValue) {
				var thing = entry.Value.Value;
				thing.EndTime = 0;
				CachedTint.SetValueForScreen(entry.Key, thing);
			}
		}
	}

	internal void UncacheEffects(string? weatherId = null, bool force_reinstance = false) {
		foreach (var entry in CachedEffects.GetActiveValues()) {
			string? id = CachedWeatherName.GetValueForScreen(entry.Key);
			if (weatherId != null && id != weatherId)
				continue;

			if (entry.Value.HasValue) {
				var thing = entry.Value.Value;
				thing.Hour = force_reinstance ? -2 : -1;
				CachedEffects.SetValueForScreen(entry.Key, thing);
			}
		}
	}


	internal CachedTintData? GetCachedTint(GameLocation? location = null) {
		location ??= Game1.player?.currentLocation;
		string? weather = location?.GetWeather()?.Weather;
		TryGetWeather(weather, out var data);
		if (location is null || data?.Lighting is null || data.Lighting.Count == 0)
			return null;

		int time = Game1.timeOfDay;

		CachedTintData? cache = CachedTint.Value;
		if (cache.HasValue &&
			cache.Value.Data == data &&
			cache.Value.EventUp == Game1.eventUp &&
			cache.Value.Location == location &&
			time >= cache.Value.StartTime &&
			time <= cache.Value.EndTime &&
			(!cache.Value.HasAmbientColor || (time >= cache.Value.AmbientStartTime && (time <= cache.Value.AmbientEndTime || time >= cache.Value.FullyDarkTime)))
		)
			return cache;

		GameStateQueryContext ctx = new(location, Game1.player, null, null, Game1.random);

		// We need to collect a start and end value.
		IScreenTintData? start = null;
		IScreenTintData? end = null;

		int darkTime = Game1.getTrulyDarkTime(location);
		int startDarkTime = Game1.getStartingToGetDarkTime(location);

		foreach (var tintData in data.Lighting) {
			if (!string.IsNullOrEmpty(tintData.Condition) && !GameStateQuery.CheckConditions(tintData.Condition, ctx))
				continue;

			int tod = tintData.TimeOfDay;
			if (tod <= 0)
				tod = darkTime + tod;

			if (tod <= time)
				start = tintData;

			else if (start != null && tod > time) {
				end = tintData;
				break;
			}
		}

		int fullyDarkTime = darkTime + 300;

		// If we have no data, return nothing basically.
		if (start == null) {
			cache = new() {
				Data = data,
				Location = location,
				EventUp = Game1.eventUp,
				StartTime = int.MinValue,
				EndTime = int.MaxValue,
				FullyDarkTime = fullyDarkTime,
				DurationInTenMinutes = 1,
				HasAmbientColor = false,
				HasLightingTint = false,
				HasPostLightingTint = false,
			};
		} else {
			bool can_tween = start.TweenMode.HasFlag(LightingTweenMode.After) &&
				end != null && end.TweenMode.HasFlag(LightingTweenMode.Before);

			int start_time = start.TimeOfDay;
			int end_time;

			if (end is null) {
				end_time = int.MaxValue;
				end = start;
			} else {
				end_time = end.TimeOfDay;
				if (!can_tween)
					end = start;
			}

			if (start_time <= 0)
				start_time = darkTime - start_time;
			if (end_time <= 0)
				end_time = darkTime - end_time;


			// Okay, we need to figure out our ambient opacity stuff.

			bool has_ambient_color = start.AmbientColor.HasValue || end.AmbientColor.HasValue;

			int ambientStartTime = start_time;
			int ambientEndTime = end_time;

			float start_opacity;
			float end_opacity;

			// So, if we have both values, then we just tween between
			// those values and don't think about it.
			if (start != end && start.AmbientOutdoorOpacity.HasValue && end.AmbientOutdoorOpacity.HasValue) {
				// Both values? Easy mode!
				start_opacity = start.AmbientOutdoorOpacity.Value;
				end_opacity = end.AmbientOutdoorOpacity.Value;

			} else {
				// If we *don't*, then we figure out what time of day we're
				// in, mark its start and end times, calculate the start
				// and end opacities for that segment of time, and then do
				// some very minor logic to adjust the values based on
				// the start time, end time, and either opacity we already
				// happen to know.

				if (time < startDarkTime) {
					ambientStartTime = 0;
					ambientEndTime = startDarkTime;

					start_opacity = 0.3f;
					end_opacity = 0.3f;

				} else if (time < darkTime) {
					ambientStartTime = startDarkTime;
					ambientEndTime = darkTime;

					start_opacity = 0.3f;
					end_opacity = 0.75f;

				} else if (time < fullyDarkTime) {
					ambientStartTime = darkTime;
					ambientEndTime = fullyDarkTime;

					start_opacity = 0.75f;
					end_opacity = 0.93f;

				} else {
					ambientStartTime = fullyDarkTime;
					ambientEndTime = int.MaxValue;

					start_opacity = 0.93f;
					end_opacity = 0.93f;
				}

				if (start.AmbientOutdoorOpacity.HasValue)
					start_opacity = Math.Max(start.AmbientOutdoorOpacity.Value, start_opacity);

				if (end.AmbientOutdoorOpacity.HasValue)
					end_opacity = Math.Max(end.AmbientOutdoorOpacity.Value, end_opacity);

				if (end_opacity < start_opacity)
					end_opacity = start_opacity;
			}

			cache = new() {
				Data = data,
				Location = location,
				EventUp = Game1.eventUp,

				StartTime = start_time,
				EndTime = end_time,

				DurationInTenMinutes = Utility.CalculateMinutesBetweenTimes(start_time, end_time) / 10,

				FullyDarkTime = fullyDarkTime,

				HasAmbientColor = has_ambient_color,
				HasLightingTint = start.LightingTint.HasValue || end.LightingTint.HasValue,
				HasPostLightingTint = start.PostLightingTint.HasValue || end.PostLightingTint.HasValue,

				StartAmbientColor = start.AmbientColor ?? Color.White,
				EndAmbientColor = end.AmbientColor ?? Color.White,

				AmbientDurationInTenMinutes = has_ambient_color
					? Utility.CalculateMinutesBetweenTimes(ambientStartTime, ambientEndTime) / 10
					: 1,
				AmbientStartTime = ambientStartTime,
				AmbientEndTime = ambientEndTime,

				StartAmbientOutdoorOpacity = start_opacity,
				EndAmbientOutdoorOpacity = end_opacity,

				StartLightingTint = start.LightingTint ?? Color.White,
				EndLightingTint = end.LightingTint ?? Color.White,

				StartLightingTintOpacity = start.LightingTintOpacity,
				EndLightingTintOpacity = end.LightingTintOpacity,

				StartPostLightingTint = start.PostLightingTint ?? Color.White,
				EndPostLightingTint = end.PostLightingTint ?? Color.White,

				StartPostLightingTintOpacity = start.PostLightingTintOpacity,
				EndPostLightingTintOpacity = end.PostLightingTintOpacity
			};
		}

		CachedTint.Value = cache;
		return cache;
	}


	internal List<IWeatherEffect>? GetCachedWeatherEffects(GameLocation? location = null, int? timeOfDay = null) {
		var data = CachedWeather.Value;
		location ??= Game1.player?.currentLocation;
		EffectCache? cache = CachedEffects.Value;

		if (location is null || data?.Effects is null || data.Effects.Count == 0) {
			if (cache.HasValue) {
				CachedEffects.Value = null;
				if (cache.Value.Effects is not null)
					foreach (var effect in cache.Value.Effects) {
						RemoveLoadsAsset(effect.Id);
						effect.Remove();
					}
			}
			return null;
		}

		int hour = (timeOfDay ?? Game1.timeOfDay) / 100;

		if (cache.HasValue && cache.Value.Data == data && cache.Value.EventUp == Game1.eventUp && cache.Value.Hour == hour && cache.Value.Location == location)
			return cache.Value.Effects;

		bool force_reinstance = cache?.Hour == -2;
		var old_by_id = cache?.EffectsById;
		var old_data_by_id = cache?.DataById;

		Dictionary<string, IWeatherEffect> effectsById = new();
		Dictionary<string, IEffectData> dataById = new();
		List<IWeatherEffect> result = new();
		HashSet<string> groups = new();

		GameStateQueryContext ctx = new(location, Game1.player, null, null, Game1.random);

		int reused = 0;
		int instanced = 0;

		TargetMapType targetMapType = location.IsOutdoors ? TargetMapType.Outdoors : TargetMapType.Indoors;

		foreach (var effect in data.Effects) {
			if (effect.Group != null && groups.Contains(effect.Group))
				continue;

			if (!effect.TargetMapType.HasFlag(targetMapType))
				continue;

			if (!string.IsNullOrEmpty(effect.Condition) && !GameStateQuery.CheckConditions(effect.Condition, ctx))
				continue;

			if (effect.Group != null)
				groups.Add(effect.Group);

			IWeatherEffect instance;

			// We rely upon record value equality checks.
			if (old_by_id is not null && old_data_by_id is not null &&
				!force_reinstance &&
				old_data_by_id.TryGetValue(effect.Id, out var existingData) &&
				EqualityComparer<IEffectData>.Default.Equals(existingData, effect) &&
				old_by_id.TryGetValue(effect.Id, out var existing)
			) {
				// Remove the old instance.
				old_by_id.Remove(effect.Id);
				instance = existing;
				reused++;

			} else
				try {
					if (!EffectFactories.TryGetValue(effect.Type, out var factory))
						throw new ArgumentException($"unknown effect type: {effect.Type}");

					var created = factory(lastLayerId, effect);
					if (created is null)
						continue;

					instance = created;
					lastLayerId++;
					instanced++;

				} catch (Exception ex) {
					Log($"Unable to instantiate weather layer '{effect.Id}' with type '{effect.Type}': {ex}", LogLevel.Warn);
					continue;
				}

			dataById[effect.Id] = effect;
			effectsById[effect.Id] = instance;
			result.Add(instance);
		}

		// Clean up the old instances that weren't used.
		if (old_by_id is not null)
			foreach (var effect in old_by_id.Values) {
				RemoveLoadsAsset(effect.Id);
				effect.Remove();
			}

		int skipped = data.Effects.Count - (reused + instanced);
#if DEBUG
		LogLevel level = LogLevel.Debug;
#else
		LogLevel level = LogLevel.Trace;
#endif

		Log($"Regenerated weather effects for: {Game1.player?.displayName}\n\tReused {reused} effect instances.\n\tCreated {instanced} new effect instances.\n\tSkipped {skipped} effects.", level);

		CachedEffects.Value = new() {
			Data = data,
			DataById = dataById,
			EventUp = Game1.eventUp,
			EffectsById = effectsById,
			Location = location,
			Hour = hour,
			Effects = result.Count > 0 ? result : null
		};

		return result.Count > 0 ? result : null;
	}


	internal List<IWeatherLayer>? GetCachedWeatherLayers(GameLocation? location = null, int? timeOfDay = null) {
		var data = CachedWeather.Value;
		location ??= Game1.currentLocation;
		LayerCache? cache = CachedLayers.Value;
		if (location is null || data?.Layers is null || data.Layers.Count == 0) {
			HasShaderLayer.Value = false;
			if (cache.HasValue) {
				CachedLayers.Value = null;
				if (cache.Value.Layers is not null)
					foreach (var layer in cache.Value.Layers)
						RemoveLoadsAsset(layer.Id);
			}
			return null;
		}

		int hour = (timeOfDay ?? Game1.timeOfDay) / 100;

		if (cache.HasValue && cache.Value.Data == data && cache.Value.EventUp == Game1.eventUp && cache.Value.Hour == hour && cache.Value.Location == location)
			return cache.Value.Layers;

		bool force_reinstance = cache?.Hour == -2;
		var old_by_id = cache?.LayersById;
		var old_data_by_id = cache?.DataById;

		Dictionary<string, IWeatherLayer> layersById = new();
		Dictionary<string, ILayerData> dataById = new();
		List<IWeatherLayer> result = new();
		HashSet<string> groups = new();

		GameStateQueryContext ctx = new(location, Game1.player, null, null, Game1.random);

		int reused = 0;
		int instanced = 0;

		bool has_shader_layer = false;

		TargetMapType targetMapType = location.IsOutdoors ? TargetMapType.Outdoors : TargetMapType.Indoors;

		foreach (var layer in data.Layers) {
			if (layer.Group != null && groups.Contains(layer.Group))
				continue;

			if (!layer.TargetMapType.HasFlag(targetMapType))
				continue;

			if (!string.IsNullOrEmpty(layer.Condition) && !GameStateQuery.CheckConditions(layer.Condition, ctx))
				continue;

			if (layer is IShaderLayerData sld && (!Config.AllowShaders || (sld.Shader != null && Config.DisabledShaders.Contains(sld.Shader))))
				continue;

			if (layer.Group != null)
				groups.Add(layer.Group);

			IWeatherLayer instance;

			// We rely upon record value equality checks.
			if (old_by_id is not null && old_data_by_id is not null &&
				!force_reinstance &&
				old_data_by_id.TryGetValue(layer.Id, out var existingData) &&
				EqualityComparer<ILayerData>.Default.Equals(existingData, layer) &&
				old_by_id.TryGetValue(layer.Id, out var existing)
			) {
				old_by_id.Remove(layer.Id);
				instance = existing;
				reused++;

			} else
				try {
					if (!LayerFactories.TryGetValue(layer.Type, out var factory))
						throw new ArgumentException($"unknown layer type: {layer.Type}");

					var created = factory(lastLayerId, layer);
					if (created is null)
						continue;

					instance = created;
					lastLayerId++;
					instanced++;

				} catch (Exception ex) {
					Log($"Unable to instantiate weather layer '{layer.Id}' with type '{layer.Type}': {ex}", LogLevel.Warn);
					continue;
				}

			if (!has_shader_layer)
				has_shader_layer = instance is ShaderLayer;

			dataById[layer.Id] = layer;
			layersById[layer.Id] = instance;
			result.Add(instance);
		}

		// Clean up the old instances that weren't used.
		if (old_by_id is not null)
			foreach (var layer in old_by_id.Values) {
				RemoveLoadsAsset(layer.Id);
			}

		int skipped = data.Layers.Count - (reused + instanced);
#if DEBUG
		LogLevel level = LogLevel.Debug;
#else
		LogLevel level = LogLevel.Trace;
#endif

		Log($"Regenerated weather layers for: {Game1.player.displayName}\n\tReused {reused} layer instances.\n\tCreated {instanced} new layer instances.\n\tSkipped {skipped} layers.", level);

		HasShaderLayer.Value = has_shader_layer;

		CachedLayers.Value = new() {
			Data = data,
			DataById = dataById,
			EventUp = Game1.eventUp,
			LayersById = layersById,
			Location = location,
			Hour = hour,
			Layers = result.Count > 0 ? result : null
		};

		return result.Count > 0 ? result : null;
	}

	/// <summary>
	/// Use a per-screen cache to locate our weather data. We cache the
	/// data rather than performing dictionary lookups for optimal
	/// performance in a very hot loop.
	/// </summary>
	/// <param name="name">The current location's weather's name.
	/// If this changes, we re-cache the weather data from the
	/// <see cref="Data"/> dictionary.</param>
	/// <returns>The weather data, if any exists.</returns>
	internal WeatherData? GetCachedWeatherData(string? name) {
		if (Data is not null && CachedWeatherName.Value == name)
			return CachedWeather.Value;

		WeatherData? result;
		if (name != null) {
			LoadWeatherData();
			Data.TryGetValue(name, out result);
		} else
			result = null;

		CachedWeatherName.Value = name;
		CachedWeather.Value = result;
		return result;
	}

	#endregion

}
