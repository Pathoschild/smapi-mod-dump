/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

#define USE_MULTIDRAW

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Configuration;
using SpriteMaster.Configuration.Preview;
using SpriteMaster.Core;
using SpriteMaster.Extensions;
using SpriteMaster.Extensions.Reflection;
using SpriteMaster.Types;
using SpriteMaster.Types.Exceptions;
using SpriteMaster.Types.Pooling;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SpriteMaster.Harmonize.Patches.Game;

internal static class Snow {
	private static readonly XColor SnowColor = new(255, 250, 250);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static Bounds GetSourceRect<TWeatherDebris>(this TWeatherDebris debris) where TWeatherDebris : WeatherDebris =>
		debris switch {
			SnowWeatherDebris snowDebris => snowDebris.ReferenceSourceRect,
			_ => debris.sourceRect
		};

	internal sealed class DebrisMap {
		internal readonly Dictionary<Bounds, List<SnowWeatherDebris>> Map = new();
		internal int ListMax = 0;

		internal int Count => Map.Count;

		internal Dictionary<Bounds, List<SnowWeatherDebris>>.ValueCollection Values => Map.Values;

		public DebrisMap() {
		}

		internal DebrisMap(DebrisMap map) {
			Map = new(map.Map);
			ListMax = map.ListMax;
		}

		internal void Clear() {
			Map.Clear();
			ListMax = 0;
		}

		internal void CloneFrom(DebrisMap map) {
			Map.EnsureCapacity(map.Map.Count);
			foreach (var item in map.Map) {
				Map.Add(item.Key, item.Value);
			}

			ListMax = map.ListMax;
		}
	}

	private static DebrisMap MappedWeatherDebris = new();
	private static List<WeatherDebris> AllWeatherDebris = new();

	[StructLayout(LayoutKind.Auto)]
	internal readonly struct SnowState {
		internal DebrisMap MappedWeatherDebris { get; init; }
		internal List<WeatherDebris> AllWeatherDebris { get; init; }

		internal static SnowState Backup() => new() {
			MappedWeatherDebris = new(Snow.MappedWeatherDebris),
			AllWeatherDebris = new(Snow.AllWeatherDebris)
		};

		internal void Restore() {
			Snow.MappedWeatherDebris = MappedWeatherDebris;
			Snow.AllWeatherDebris = AllWeatherDebris;
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	[Harmonize(
		typeof(WeatherDebris),
		".ctor",
		Harmonize.Fixation.Reverse,
		Harmonize.PriorityLevel.First,
		critical: false
	)]
	public static void WeatherDebrisReverse(WeatherDebris __instance, Vector2 position, int which, float rotationVelocity, float dx, float dy) {
		throw new ReversePatchException();
	}

	/*
	[Harmonize(
		typeof(StardewValley.WeatherDebris),
		".ctor",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.First,
		critical: false
	)]
	public static bool WeatherDebrisConstructorPrefix(WeatherDebris __instance, Vector2 position, int which, float rotationVelocity, float dx, float dy) {
		if (__instance is SnowWeatherDebris) {
			WeatherDebrisReverse(__instance, position, which, rotationVelocity, dx, dy);
			return false;
		}

		return true;
	}
	*/

	internal sealed class SnowWeatherDebris : WeatherDebris {
		internal float Rotation;
		internal readonly float RotationRate;
		internal readonly float Scale;
		internal readonly Bounds ReferenceSourceRect;

		internal SnowWeatherDebris(XVector2 position, int which, float rotationVelocity, float dx, float dy, float rotation, float rotationRate, float scale) :
			base(position, which, rotationVelocity, dx, dy) {
			WeatherDebrisReverse(this, position, which, rotationVelocity, dx, dy);

			Rotation = rotation;
			RotationRate = rotationRate;
			Scale = scale;
			ReferenceSourceRect = sourceRect;
		}

		internal void Update() {
			update();
			Rotation = (Rotation + RotationRate) % (2.0f * MathF.PI);
		}
	}

	private static bool IsPuffersnow = false;

	private static bool ShouldDrawSnow => PrecipitationPatches.IsSnowingHereExt() && Game1.currentLocation.IsOutdoors && Game1.currentLocation is not Desert;

	private static readonly Lazy<XTexture2D> FishTexture = new(() => SpriteMaster.Self.Helper.GameContent.Load<XTexture2D>(@"LooseSprites\AquariumFish"));

	public readonly record struct DrawWeatherState(bool Ran = false, bool Overridden = false, PrecipitationType? PreviousOverride = null, bool PreviousSnowValue = false);

	// The only mod I know of that changes snow color:
	private static readonly Func<XColor>? DynamicRainAndWindGetSnowColor =
		ReflectionExt.GetTypeExt("FerngillDynamicRainAndWind.RainAndWind")?.
			GetStaticMethod("GetSnowColor")?.
			CreateDelegate<Func<XColor>>();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static XColor GetSnowColor(out bool additive) {
		if (DynamicRainAndWindGetSnowColor is not null) {
			var color = DynamicRainAndWindGetSnowColor();
			if (color != XColor.White) {
				additive = false;
				return color;
			}
		}

		additive = true;
		return SnowColor;
	}

	[Harmonize(
		typeof(Game1),
		"drawWeather",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.Last,
		critical: false
	)]
	// ReSharper disable once RedundantAssignment
	public static bool DrawWeatherPre(Game1 __instance, GameTime time, RenderTarget2D target_screen, ref DrawWeatherState __state) {
		__state = new(Ran: false);

		if (!Config.IsEnabled || !Config.Extras.Snow.Enabled) {
			return true;
		}

		bool isPrecipitationSnow = Scene.Current?.Precipitation == PrecipitationType.Snow;

		if ((!ShouldDrawSnow || Scene.Current is not null) && !isPrecipitationSnow) {
			return true;
		}

		if (MappedWeatherDebris.Count == 0) {
			return true;
		}

		var snowColor = GetSnowColor(out bool additive);

		var batchSortMode = DrawState.CurrentSortMode;
		var batchBlendState = DrawState.CurrentBlendState;
		var batchSamplerState = DrawState.CurrentSamplerState;

		Game1.spriteBatch.End();
		Game1.spriteBatch.Begin(
			sortMode: (IsPuffersnow || !additive) ? SpriteSortMode.Texture : SpriteSortMode.Deferred,
			blendState: (IsPuffersnow || !additive) ? BlendState.AlphaBlend : BlendState.Additive,
			samplerState: Config.Resample.IsEnabled ? SamplerState.LinearClamp : SamplerState.PointClamp,
			rasterizerState: Game1.spriteBatch.GraphicsDevice.RasterizerState
		);

		try {
			if (__instance.takingMapScreenshot) {
				XTexture2D drawTexture = IsPuffersnow ? FishTexture.Value : Game1.mouseCursors;

				foreach (var weatherDebris in AllWeatherDebris) {
					var item = (SnowWeatherDebris)weatherDebris;
					var position = new XVector2(
						Game1.random.Next(Game1.viewport.Width - item.ReferenceSourceRect.Width * 3),
						Game1.random.Next(Game1.viewport.Height - item.ReferenceSourceRect.Height * 3)
					);

					Game1.spriteBatch.Draw(
						texture: drawTexture,
						position: position,
						sourceRectangle: IsPuffersnow ? new(0, 0, 24, 24) : item.ReferenceSourceRect,
						color: snowColor,
						rotation: item.Rotation,
						origin: XVector2.Zero,
						scale: item.Scale,
						effects: SpriteEffects.None,
						layerDepth: 1E-06f
					);
				}
			}
			else if (Game1.viewport.X > -Game1.viewport.Width) {
				if (IsPuffersnow) {
					XTexture2D drawTexture = FishTexture.Value;

					foreach (var weatherDebris in MappedWeatherDebris.Values) {
						foreach (SnowWeatherDebris item in weatherDebris) {
							Game1.spriteBatch.Draw(
								texture: drawTexture,
								position: item.position,
								sourceRectangle: new(0, 0, 24, 24),
								color: snowColor,
								rotation: item.Rotation,
								origin: XVector2.Zero,
								scale: item.Scale,
								effects: SpriteEffects.None,
								layerDepth: 1E-06f
							);
						}
					}
				}
				else {
					XTexture2D drawTexture = Game1.mouseCursors;

#if USE_MULTIDRAW
					Span<OnDrawImpl.DrawInstance> instances = stackalloc OnDrawImpl.DrawInstance[MappedWeatherDebris.ListMax];
#endif

					foreach (var (source, list) in MappedWeatherDebris.Map) {
#if !USE_MULTIDRAW
						foreach (SnowWeatherDebris item in list) {
							Game1.spriteBatch.Draw(
								texture: drawTexture,
								position: item.position,
								sourceRectangle: source, 
								color: snowColor,
								rotation: item.Rotation,
								origin: XVector2.Zero,
								scale: item.Scale,
								effects: SpriteEffects.None,
								layerDepth: 1E-06f
							);
						}
#else
						for (int i = 0; i < list.Count; ++i) {
							var debris = list[i];

							instances[i] = new(debris);
						}

						Game1.spriteBatch.DrawMulti(
							texture: drawTexture,
							source: source,
							color: snowColor,
							origin: XVector2.Zero,
							effects: SpriteEffects.None,
							layerDepth: 1E-06F,
							instances: instances[..list.Count]
						);
#endif
					}
				}
			}
		}
		finally {
			Game1.spriteBatch.End();
			Game1.spriteBatch.Begin(batchSortMode, batchBlendState, batchSamplerState);
		}

		var locationWeather = Game1.netWorldState.Value.GetWeatherForLocation(GameLocation.LocationContext.Default);

		__state = new(true, true, PrecipitationPatches.PrecipitationOverride, locationWeather.isSnowing.Value);
		PrecipitationPatches.PrecipitationOverride = PrecipitationType.None;
		locationWeather.isSnowing.Value = false;
		return true;
	}

	[Harmonize(
		typeof(Game1),
		"drawWeather",
		Harmonize.Fixation.Postfix,
		Harmonize.PriorityLevel.First,
		critical: false
	)]
	public static void DrawWeatherPost(Game1 __instance, GameTime time, RenderTarget2D target_screen, DrawWeatherState __state) {
		// Some mods, like Climates of Ferngill, patch and return false, breaking our prefix.
		if (!__state.Ran) {
			_ = DrawWeatherPre(__instance, time, target_screen, ref __state);
		}

		if (!__state.Overridden) {
			return;
		}

		PrecipitationPatches.PrecipitationOverride = __state.PreviousOverride;
		Game1.netWorldState.Value.GetWeatherForLocation(GameLocation.LocationContext.Default).isSnowing.Value = __state.PreviousSnowValue;
	}

	private static float PreviousWind = 0.0f;

	[Harmonize(
		typeof(Game1),
		"updateWeather",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.Last,
		instance: false,
		critical: false
	)]
	public static bool UpdateWeatherPre(GameTime time, ref bool __state) {
		__state = true;

		if (!Config.IsEnabled || !Config.Extras.Snow.Enabled) {
			return true;
		}

		bool isPrecipitationSnow = Scene.Current?.Precipitation == PrecipitationType.Snow;

		if ((!ShouldDrawSnow || Scene.Current is not null) && !isPrecipitationSnow) {
			return true;
		}

		if (Game1.windGust == 0f) {
			if (Game1.random.NextDouble() < 0.001) {
				Game1.windGust += Game1.random.Next(-10, -1) / 100f;
				PreviousWind = WeatherDebris.globalWind;
				WeatherDebris.globalWind += Game1.windGust;
				if (Game1.soundBank is not null) {
					Game1.wind = Game1.soundBank.GetCue("wind");
					Game1.wind.Play();
				}
			}
		}
		else {
			if (Game1.random.NextDouble() < 0.007) {
				Game1.windGust = 0f;
				WeatherDebris.globalWind = PreviousWind;
			}
		}

		// Update particles
		foreach (var item in AllWeatherDebris) {
			(item as SnowWeatherDebris)!.Update();
		}

		if (Scene.Current is null) {
			Game1.updateDebrisWeatherForMovement(AllWeatherDebris);
		}

		return true;
	}

	[Harmonize(
		typeof(Game1),
		"updateWeather",
		Harmonize.Fixation.Postfix,
		Harmonize.PriorityLevel.Last,
		instance: false,
		critical: false
	)]
	public static void UpdateWeatherPost(GameTime time, ref bool __state) {
		if (!__state) {
			_ = UpdateWeatherPre(time, ref __state);
		}
	}

	[Harmonize(
		typeof(Game1),
		"updateRainDropPositionForPlayerMovement",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.Last,
		instance: false,
		critical: false
	)]
	public static bool UpdateRainDropPositionForPlayerMovement(int direction, bool overrideConstraints, float speed) {
		if (!Config.IsEnabled || !Config.Extras.Snow.Enabled) {
			return true;
		}

		if (!ShouldDrawSnow) {
			return true;
		}

		if (
			!overrideConstraints && (
				!PrecipitationPatches.IsSnowingHereExt() ||
				!Game1.currentLocation.IsOutdoors ||
				direction is not (0 or 2) &&
				(
					Game1.player.getStandingX() < Game1.viewport.Width / 2 ||
					Game1.player.getStandingX() > Game1.currentLocation.Map.DisplayWidth - Game1.viewport.Width / 2) ||
					direction is not (1 or 3) && (
						Game1.player.getStandingY() < Game1.viewport.Height / 2 ||
						Game1.player.getStandingY() > Game1.currentLocation.Map.DisplayHeight - Game1.viewport.Height / 2
					)
				)
			) {
			return true;
		}

		Game1.updateDebrisWeatherForMovement(AllWeatherDebris, direction, overrideConstraints, speed);
		return true;
	}

	private const int ReferenceScreenArea = (2560 * 1440) / 16;

	[Harmonize(
		typeof(Game1),
		"populateDebrisWeatherArray",
		Harmonize.Fixation.Prefix,
		Harmonize.PriorityLevel.Last,
		instance: false,
		critical: false
	)]
	public static bool PopulateDebrisWeatherArray() {
		if (!Config.IsEnabled || !Config.Extras.Snow.Enabled) {
			return true;
		}

		if (!PrecipitationPatches.IsSnowingHereExt()) {
			return true;
		}

		PopulateWeather((Game1.viewport.Width, Game1.viewport.Height));

		return true;
	}

	internal static void PopulateWeather(Vector2I screenSize) {
		if (!Config.IsEnabled || !Config.Extras.Snow.Enabled) {
			return;
		}

		IsPuffersnow = Game1.random.NextDouble() < Config.Extras.Snow.PuffersnowChance;

		Game1.isDebrisWeather = true;
		int debrisToMake = Game1.random.Next(Config.Extras.Snow.MinimumDensity, Config.Extras.Snow.MaximumDensity);
		int currentScreenArea = screenSize.Width * screenSize.Height;
		double ratio = (double)currentScreenArea / ReferenceScreenArea;
		debrisToMake = (debrisToMake * ratio).RoundToInt();
		MappedWeatherDebris.Clear();
		AllWeatherDebris.Clear();
		for (int i = 0; i < debrisToMake; i++) {
			var debris = new SnowWeatherDebris(
				position: new XVector2(Game1.random.Next(0, screenSize.Width), Game1.random.Next(0, screenSize.Height)),
				which: 3,
				rotationVelocity: 0.0f,
				dx: Game1.random.Next(-10, 0) / 50f,
				dy: Game1.random.Next(10) / 50f,
				rotation: (float)(Game1.random.NextDouble() * 2.0 * Math.PI),
				rotationRate: (float)(((Game1.random.NextDouble() * 2.0) - 1.0) * Config.Extras.Snow.MaximumRotationSpeed),
				scale: (float)Math.Sqrt((Game1.random.NextDouble() * 0.75) + 0.25) * Config.Extras.Snow.MaximumScale
			);

			if (!MappedWeatherDebris.Map.TryGetValue(debris.ReferenceSourceRect, out var mappedList)) {
				MappedWeatherDebris.Map.Add(debris.ReferenceSourceRect, mappedList = new List<SnowWeatherDebris>());
			}
			mappedList.Add(debris);
			AllWeatherDebris.Add(debris);
		}
		AllWeatherDebris.Sort((d1, d2) => {
			var sourceDiff = GetSourceRect(d1).GetHashCode() - GetSourceRect(d2).GetHashCode();
			if (sourceDiff != 0) {
				return sourceDiff;
			}
			return ((SnowWeatherDebris)d1).Scale.CompareTo(((SnowWeatherDebris)d2).Scale);
		});

		int listMax = 0;
		foreach (var mappedList in MappedWeatherDebris.Values) {
			mappedList.Sort((d1, d2) => d1.Scale.CompareTo(d2.Scale));
			listMax = Math.Max(listMax, mappedList.Count);
		}

		MappedWeatherDebris.ListMax = listMax;
	}

	internal static void OnWindowResized(Vector2I size) {
		if (!Config.IsEnabled || !Config.Extras.Snow.Enabled) {
			return;
		}

		if (PrecipitationPatches.IsSnowingHereExt()) {
			PopulateDebrisWeatherArray();
		}
	}
}
