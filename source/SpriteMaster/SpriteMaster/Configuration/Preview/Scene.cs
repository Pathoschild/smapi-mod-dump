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
using Microsoft.Xna.Framework.Graphics;
using SpriteMaster.Extensions;
using SpriteMaster.Types;
using StardewValley;
using System;
using System.Runtime.InteropServices;
using SMDrawState = SpriteMaster.DrawState;

namespace SpriteMaster.Configuration.Preview;

internal abstract class Scene : IDisposable {
	internal static Scene? Current = null;
	protected const int TileSize = 16;
	protected const int TileSizeRendered = TileSize * 4;
	protected const int RegionVerticalOffset = -20;

	[StructLayout(LayoutKind.Auto)]
	private readonly struct CurrentScope : IDisposable {
		private readonly Scene? PreviousScene;

		internal CurrentScope(Scene scene) {
			PreviousScene = Current;
			Current = scene;
		}

		public void Dispose() {
			Current = PreviousScene;
		}
	}

	internal static readonly Lazy<GameLocation> SceneLocation = new(() => new StardewValley.Locations.Beach(@"Maps\Beach", "SMSettingsLocation"));

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

	[StructLayout(LayoutKind.Auto)]
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

	internal Bounds ReferenceRegion { get; private set; }
	internal Bounds Region {
		get {
			var offsetRegion = ReferenceRegion.OffsetBy((0, RegionVerticalOffset));

			if (ConfigMenu.Setup.LockPreview) {
				var baseScissor = SMDrawState.Device.ScissorRectangle;
				if (offsetRegion.Y < baseScissor.Y) {
					offsetRegion.Y = baseScissor.Y;
				}
			}

			return offsetRegion;
		}
	}
	private RasterizerState? State = null;
	protected Vector2I Size => Region.Extent;

	protected Scene(Bounds region) {
		ReferenceRegion = region;
	}

	protected static Vector2I GetSizeInTiles(Vector2I size) => size / TileSize;

	protected static Vector2I GetSizeInRenderedTiles(Vector2I size) => size / TileSizeRendered;

	internal void DrawAt(
		XSpriteBatch batch,
		XTexture2D texture,
		Bounds destination,
		Bounds? source = null,
		Color8? color = null,
		float rotation = 0.0f,
		SpriteEffects effects = SpriteEffects.None,
		float layerDepth = 0.0f
	) {
		Vector2I shift = Vector2I.Zero;
		if (destination.Extent.MaxOf > TileSizeRendered) {
			var centroid = destination.Center;
			var end = destination.End;
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
			color: color ?? XColor.White,
			rotation: rotation,
			origin: offset,
			effects: effects,
			layerDepth: layerDepth
		);
	}

	internal void DrawAt(
		XSpriteBatch batch,
		XTexture2D texture,
		Vector2I destination,
		Bounds? source = null,
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
			color: color ?? XColor.White,
			rotation: 0.0f,
			origin: offset,
			effects: effects,
			layerDepth: layerDepth
		);
	}

	internal void DrawAt(
		XSpriteBatch batch,
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

	protected abstract void OnDraw(XSpriteBatch batch, in Override overrideState);
	protected abstract void OnDrawOverlay(XSpriteBatch batch, in Override overrideState);

	[StructLayout(LayoutKind.Auto)]
	private readonly ref struct DrawState {
		internal Viewport Viewport { get; init; }
		internal SamplerState? SamplerState { get; init; }
		internal DepthStencilState? DepthStencilState { get; init; }
		internal XRectangle ScissorRectangle { get; init; }
		internal RasterizerState? RasterizerState { get; init; }
	}

	private void DrawBox(XSpriteBatch batch, in Override overrideState, in DrawState drawState) {
		batch.Begin(
			sortMode: SpriteSortMode.Deferred,
			rasterizerState: State,
			samplerState: drawState.SamplerState,
			depthStencilState: drawState.DepthStencilState
		);
		try {
			using var tempBatch = new TempValue<XSpriteBatch>(ref Game1.spriteBatch, batch);
			Game1.DrawBox(Region.X, Region.Y, Region.Width, Region.Height);
		}
		finally {
			batch.End();
		}
	}

	private void DrawFirst(XSpriteBatch batch, in Override overrideState, in DrawState drawState) {
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

	private void DrawPrecipitation(XSpriteBatch batch, in Override overrideState, in DrawState drawState) {
		batch.Begin(
			sortMode: SpriteSortMode.FrontToBack,
			rasterizerState: State,
			samplerState: drawState.SamplerState,
			depthStencilState: drawState.DepthStencilState
		);
		try {
			Game1.snowPos = XVector2.Zero;
			Game1.game1.drawWeather(Game1.currentGameTime, null);
		}
		finally {
			batch.End();
		}
	}

	private void DrawOverlay(XSpriteBatch batch, in Override overrideState, in DrawState drawState) {
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

	internal void Draw(XSpriteBatch batch, in Override overrideState) {
		using var savedWeatherState = WeatherState.Backup();
		CurrentWeatherState.Restore();
		using var currentScope = new CurrentScope(this);

		var originalSpriteBatch = Game1.spriteBatch;
		Game1.spriteBatch = batch;

		var originalLocation = Game1.currentLocation;
		Game1.currentLocation = SceneLocation.Value;

		try {
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
				DrawBox(batch, overrideState, originalDrawState);

				using var tempOverrideState = new TempValue<Override>(ref Override.Instance, overrideState);

				batch.GraphicsDevice.ScissorRectangle = Region.ClampTo(batch.GraphicsDevice.ScissorRectangle);

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
			Game1.spriteBatch = originalSpriteBatch;
			Game1.currentLocation = originalLocation;
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
			Game1.updateWeather(Game1.currentGameTime);
		}
		finally {
			Game1.graphics.GraphicsDevice.Viewport = originalDeviceViewport;
			Game1.viewport = originalGameViewport;
			Game1.fadeToBlackAlpha = originalFadeToBlackAlpha;
		}
	}

	internal void Tick(in Override overrideState) {
		using var tempOverrideState = new TempValue<Override>(ref Override.Instance, overrideState);

		using var savedWeatherState = WeatherState.Backup();
		CurrentWeatherState.Restore();
		using var currentScope = new CurrentScope(this);

		var originalLocation = Game1.currentLocation;
		Game1.currentLocation = SceneLocation.Value;
		try {
			TickPrecipitation();
			OnTick();
		}
		finally {
			Game1.currentLocation = originalLocation;
		}

		CurrentWeatherState = WeatherState.Backup();
	}
	protected abstract void OnResize(Vector2I size, Vector2I oldSize);

	internal void Resize(Bounds newRegion) {
		using var savedWeatherState = WeatherState.Backup();
		CurrentWeatherState.Restore();
		using var currentScope = new CurrentScope(this);

		var oldSize = Size;
		ReferenceRegion = newRegion;

		OnResize(Size, oldSize);

		CurrentWeatherState = WeatherState.Backup();
	}

	internal void ChangeOffset(Vector2I offset) {
		ReferenceRegion = new(offset, ReferenceRegion.Extent);
	}

	public abstract void Dispose();
}
