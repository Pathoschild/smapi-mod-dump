/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using Microsoft.Toolkit.HighPerformance;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Extensions;
using SpriteMaster.Types;
using StardewValley;
using System;

namespace SpriteMaster.Configuration.Preview;

abstract class Scene : IDisposable {
	internal static Scene? Current = null;
	protected const int TileSize = 16;
	protected const int TileSizeRendered = TileSize * 4;

	private struct CurrentScope : IDisposable {
		private readonly Scene? PreviousScene;

		internal CurrentScope(Scene scene) {
			PreviousScene = Current;
			Current = scene;
		}

		public void Dispose() {
			Current = PreviousScene;
		}
	}

	internal static readonly Lazy<StardewValley.GameLocation> SceneLocation = new(() => new StardewValley.Locations.Beach(@"Maps\Beach", "SMSettingsLocation"));

	internal abstract PrecipitationType Precipitation { get; }

	private bool IsWeatherStateSet = false;
	private WeatherState InternalWeatherState;

	private ref WeatherState CurrentWeatherState {
		get {
			if (!IsWeatherStateSet) {
				Harmonize.Patches.Game.Snow.PopulateWeather(Region.Extent);
				InternalWeatherState = WeatherState.Backup();
				IsWeatherStateSet = true;
			}

			return ref InternalWeatherState;
		}
	}

	protected readonly ref struct TempValue<T> {
		private readonly T? OriginalValue;
		private readonly Ref<T?> ReferenceValue;

		internal TempValue(ref T? value, in T? newValue) {
			OriginalValue = value;
			value = newValue;
			ReferenceValue = new(ref value);
		}

		public void Dispose() {
			ReferenceValue.Value = OriginalValue;
		}
	}

	internal Bounds Region { get; private set; }
	private RasterizerState? State = null;
	protected Vector2I Size => Region.Extent;

	protected Scene(in Bounds region) {
		Region = region;
	}

	protected static Vector2I GetSizeInTiles(Vector2I size) => size / TileSize;

	protected static Vector2I GetSizeInRenderedTiles(Vector2I size) => size / TileSizeRendered;

	internal void DrawAt(
		XNA.Graphics.SpriteBatch batch,
		XTexture2D texture,
		in Bounds destination,
		in Bounds? source = null,
		Color8? color = null,
		float rotation = 0.0f,
		SpriteEffects effects = SpriteEffects.None,
		float layerDepth = 0.0f
	) {
		Vector2I shift = Vector2I.Zero;
		if (destination.Extent.MaxOf > TileSizeRendered) {
			var centroid = destination.Center;
			var end = destination.Offset + destination.Extent;
			var difference = end - centroid;
			difference /= TileSizeRendered;
			difference *= TileSizeRendered;

			Vector2I hasValue = ((destination.Extent.X > TileSizeRendered).ToInt(), (destination.Extent.Y > TileSizeRendered).ToInt());
			difference -= hasValue * (TileSizeRendered / 2);

			var odd = (destination.Extent / TileSizeRendered) & 1;
			odd &= hasValue;
			difference -= odd * (TileSizeRendered / 2);

			shift = -difference;
		}

		var offset = (destination.Extent >> 3);

		batch.Draw(
			texture: texture,
			destinationRectangle: destination.OffsetBy(shift),
			sourceRectangle: source,
			color: color ?? XNA.Color.White,
			rotation: rotation,
			origin: offset,
			effects: effects,
			layerDepth: layerDepth
		);
	}

	internal void DrawAt(
		XNA.Graphics.SpriteBatch batch,
		XTexture2D texture,
		Vector2I destination,
		in Bounds? source = null,
		Color8? color = null,
		float rotation = 0.0f,
		Vector2F? origin = null,
		SpriteEffects effects = SpriteEffects.None,
		float layerDepth = 0.0f
	) {
		var size = new Vector2I(source?.Width ?? texture.Width, source?.Height ?? texture.Height) << 2;

		Vector2I shift = Vector2I.Zero;
		if (size.MaxOf > TileSizeRendered) {
			var centroid = size / 2;
			var end = size;
			var difference = end - centroid;
			difference /= TileSizeRendered;
			difference *= TileSizeRendered;

			Vector2I hasValue = ((size.X > TileSizeRendered).ToInt(), (size.Y > TileSizeRendered).ToInt());
			difference -= hasValue * (TileSizeRendered / 2);

			var odd = (size / TileSizeRendered) & 1;
			odd &= hasValue;
			difference -= odd * (TileSizeRendered / 2);

			shift = -difference;
		}

		var offset = (size >> 3);

		var bounds = new Bounds(destination, size);

		batch.Draw(
			texture: texture,
			destinationRectangle: bounds.OffsetBy(shift),
			sourceRectangle: source,
			color: color ?? XNA.Color.White,
			rotation: 0.0f,
			origin: offset,
			effects: effects,
			layerDepth: layerDepth
		);
	}

	internal void DrawAt(
		XNA.Graphics.SpriteBatch batch,
		AnimatedTexture texture,
		Vector2I destination,
		Color8? color = null,
		float rotation = 0.0f,
		SpriteEffects effects = SpriteEffects.None,
		float layerDepth = 0.0f
	) {
		DrawAt(
			batch: batch,
			texture: texture.Texture,
			destination: new Bounds(destination, texture.Size << 2),
			source: texture.Current,
			color: color,
			rotation: rotation,
			effects: effects,
			layerDepth: layerDepth
		);
	}

	protected abstract void OnDraw(XNA.Graphics.SpriteBatch batch, in Preview.Override overrideState);
	protected abstract void OnDrawOverlay(XNA.Graphics.SpriteBatch batch, in Preview.Override overrideState);
	
	private readonly ref struct DrawState {
		internal readonly Viewport Viewport { get; init; }
		internal readonly SamplerState? SamplerState { get; init; }
		internal readonly DepthStencilState? DepthStencilState { get; init; }
		internal readonly XRectangle ScissorRectangle { get; init; }
		internal readonly RasterizerState? RasterizerState { get; init; }
	}

	private void DrawBox(XNA.Graphics.SpriteBatch batch, in Preview.Override overrideState) {
		using var tempBatch = new TempValue<XNA.Graphics.SpriteBatch>(ref StardewValley.Game1.spriteBatch, batch);
		StardewValley.Game1.DrawBox(Region.X, Region.Y, Region.Width, Region.Height);
	}

	private void DrawFirst(XNA.Graphics.SpriteBatch batch, in Preview.Override overrideState, in DrawState drawState) {
		batch.Begin(
			sortMode: SpriteSortMode.FrontToBack,
			rasterizerState: State,
			samplerState: drawState.SamplerState,
			depthStencilState: drawState.DepthStencilState
		);
		try {
			OnDraw(batch, overrideState);
		}
		finally {
			batch.End();
		}
	}

	private void DrawPrecipitation(XNA.Graphics.SpriteBatch batch, in Preview.Override overrideState, in DrawState drawState) {
		batch.Begin(
			sortMode: SpriteSortMode.FrontToBack,
			rasterizerState: State,
			samplerState: drawState.SamplerState,
			depthStencilState: drawState.DepthStencilState
		);
		try {
			Game1.snowPos = Vector2.Zero;
			StardewValley.Game1.game1.drawWeather(Game1.currentGameTime, null);
		}
		finally {
			batch.End();
		}
	}

	private void DrawOverlay(XNA.Graphics.SpriteBatch batch, in Preview.Override overrideState, in DrawState drawState) {
		batch.Begin(
			sortMode: SpriteSortMode.Deferred,
			rasterizerState: State,
			samplerState: drawState.SamplerState,
			depthStencilState: drawState.DepthStencilState
		);
		try {
			OnDrawOverlay(batch, overrideState);
		}
		finally {
			batch.End();
		}
	}

	internal void Draw(XNA.Graphics.SpriteBatch batch, in Preview.Override overrideState) {
		using var savedWeatherState = WeatherState.Backup();
		CurrentWeatherState.Restore();
		using var currentScope = new CurrentScope(this);

		var originalSpriteBatch = StardewValley.Game1.spriteBatch;
		StardewValley.Game1.spriteBatch = batch;

		var originalLocation = StardewValley.Game1.currentLocation;
		StardewValley.Game1.currentLocation = SceneLocation.Value;

		try {
			DrawBox(batch, overrideState);

			var originalDrawState = new DrawState {
				Viewport = batch.GraphicsDevice.Viewport,
				SamplerState = batch.GraphicsDevice.SamplerStates[0],
				DepthStencilState = batch.GraphicsDevice.DepthStencilState,
				ScissorRectangle = batch.GraphicsDevice.ScissorRectangle,
				RasterizerState = batch.GraphicsDevice.RasterizerState
			};

			var cloneRasterizerState = originalDrawState.RasterizerState ?? RasterizerState.CullCounterClockwise;
			State ??= new RasterizerState {
				CullMode = cloneRasterizerState.CullMode,
				DepthBias = cloneRasterizerState.DepthBias,
				FillMode = cloneRasterizerState.FillMode,
				MultiSampleAntiAlias = cloneRasterizerState.MultiSampleAntiAlias,
				ScissorTestEnable = true,
				SlopeScaleDepthBias = cloneRasterizerState.SlopeScaleDepthBias,
				DepthClipEnable = cloneRasterizerState.DepthClipEnable,
			};

			batch.End();
			try {
				using var tempOverrideState = new TempValue<Preview.Override>(ref Preview.Override.Instance, overrideState);

				batch.GraphicsDevice.ScissorRectangle = Region;
				batch.GraphicsDevice.Viewport = new(Region);

				DrawFirst(batch, overrideState, originalDrawState);
				DrawPrecipitation(batch, overrideState, originalDrawState);
				DrawOverlay(batch, overrideState, originalDrawState);
			}
			finally {
				batch.GraphicsDevice.Viewport = originalDrawState.Viewport;
				batch.GraphicsDevice.ScissorRectangle = originalDrawState.ScissorRectangle;
				batch.Begin(
					rasterizerState: originalDrawState.RasterizerState,
					samplerState: originalDrawState.SamplerState,
					depthStencilState: originalDrawState.DepthStencilState
				);
			}
		}
		finally {
			StardewValley.Game1.spriteBatch = originalSpriteBatch;
			StardewValley.Game1.currentLocation = originalLocation;
		}

		CurrentWeatherState = WeatherState.Backup();
	}

	protected abstract void OnTick();

	private void TickPrecipitation() {
		var originalDeviceViewport = Game1.graphics.GraphicsDevice.Viewport;
		var originalGameViewport = Game1.viewport;
		var originalFadeToBlackAlpha = Game1.fadeToBlackAlpha;
		try {
			Game1.graphics.GraphicsDevice.Viewport = new(Region);
			Game1.viewport = Region;
			Game1.fadeToBlackAlpha = 0.0f;
			StardewValley.Game1.updateWeather(StardewValley.Game1.currentGameTime);
		}
		finally {
			Game1.graphics.GraphicsDevice.Viewport = originalDeviceViewport;
			Game1.viewport = originalGameViewport;
			Game1.fadeToBlackAlpha = originalFadeToBlackAlpha;
		}
	}

	internal void Tick() {
		using var savedWeatherState = WeatherState.Backup();
		CurrentWeatherState.Restore();
		using var currentScope = new CurrentScope(this);

		var originalLocation = StardewValley.Game1.currentLocation;
		StardewValley.Game1.currentLocation = SceneLocation.Value;
		try {
			TickPrecipitation();
			OnTick();
		}
		finally {
			StardewValley.Game1.currentLocation = originalLocation;
		}

		CurrentWeatherState = WeatherState.Backup();
	}
	protected abstract void OnResize(Vector2I Size, Vector2I OldSize);

	internal void Resize(in Bounds newRegion) {
		using var savedWeatherState = WeatherState.Backup();
		CurrentWeatherState.Restore();
		using var currentScope = new CurrentScope(this);

		var oldSize = Size;
		Region = newRegion;

		OnResize(Size, oldSize);

		CurrentWeatherState = WeatherState.Backup();
	}

	public abstract void Dispose();
}
