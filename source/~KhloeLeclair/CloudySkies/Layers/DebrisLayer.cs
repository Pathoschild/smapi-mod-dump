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
using System.Linq;

using Leclair.Stardew.CloudySkies.LayerData;
using Leclair.Stardew.CloudySkies.Models;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Locations;

namespace Leclair.Stardew.CloudySkies.Layers;

public class DebrisLayer : IWeatherLayer, IDisposable {

	internal static bool HasUpdatedThisFrame = false;

	private readonly ModEntry Mod;

	public ulong Id { get; }

	public LayerDrawType DrawType { get; }

	private Texture2D Texture;
	private string? TextureName;

	private bool UsedTextureBoundsAsSource;
	private Rectangle[] Sources;

	private readonly SpriteEffects Effects;
	private readonly float Scale;

	private readonly Color Color;
	private readonly float Opacity;

	private List<CustomDebris> Debris = new();

	private bool IsDisposed;

	#region Life Cycle

	public DebrisLayer(ModEntry mod, ulong id, DebrisLayerData data) {
		Mod = mod;
		Id = id;
		DrawType = LayerDrawType.Normal;

		TextureName = data.Texture ?? @"LooseSprites/Cursors";
		bool should_animate;

		if (data.Texture is null) {
			TextureName = @"LooseSprites/Cursors";
			int season = data.UseSeasonal >= 0 ? data.UseSeasonal : Game1.season switch {
				Season.Summer => 1,
				Season.Fall => 2,
				Season.Winter => 3,
				_ => 0
			};

			should_animate = season != 3;

			Sources = season switch {
				1 => [new Rectangle(352, 1200, 16, 16)],
				2 => [new Rectangle(352, 1216, 16, 16)],
				3 => [
					new Rectangle(391 + 4 * 0, 1236, 4, 4),
					new Rectangle(391 + 4 * 1, 1236, 4, 4),
					new Rectangle(391 + 4 * 2, 1236, 4, 4),
					new Rectangle(391 + 4 * 3, 1236, 4, 4),
					new Rectangle(391 + 4 * 4, 1236, 4, 4),
				],
				_ => [new Rectangle(352, 1184, 16, 16)]
			};

			if (season == 999) {
				should_animate = false;
				TextureName = Game1.objectSpriteSheetName;

				var source_list = new List<Rectangle>();

				foreach(var entry in Game1.objectData.Values) {
					if (entry.Texture is null || entry.Texture == Game1.objectSpriteSheetName)
						source_list.Add(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, entry.SpriteIndex, 16, 16));
				}

				Sources = source_list.ToArray();
			}

		} else {
			TextureName = data.Texture;
			if (data.Sources is null || data.Sources.Count == 0)
				Sources = null!;
			else
				Sources = data.Sources.ToArray();

			should_animate = data.ShouldAnimate;
		}

		Mod.MarkLoadsAsset(id, TextureName);
		Texture = Game1.content.Load<Texture2D>(TextureName);

		if (Sources is null) {
			UsedTextureBoundsAsSource = true;
			Sources = [Texture.Bounds];
		}

		Effects = SpriteEffects.None;
		if (data.FlipHorizontal)
			Effects |= SpriteEffects.FlipHorizontally;
		if (data.FlipVertical)
			Effects |= SpriteEffects.FlipVertically;

		Color = data.Color ?? Color.White;
		Opacity = data.Opacity;

		Scale = data.Scale;

		int width = 0;
		int height = 0;

		foreach(Rectangle source in Sources) {
			if (source.Width > width) width = source.Width;
			if (source.Height > height) height = source.Height;
		}

		if (data.MinCount < 1)
			data.MinCount = 1;
		if (data.MaxCount < data.MinCount)
			data.MaxCount = data.MinCount;

		int debrisToMake = Game1.random.Next(data.MinCount, data.MaxCount);

		for (int i = 0; i < debrisToMake; i++) {
			Debris.Add(new CustomDebris(
				new Vector2(Game1.random.Next(0, Game1.viewport.Width), Game1.random.Next(0, Game1.viewport.Height)),
				(int) (width * Scale),
				(int) (height * Scale),
				data.CanBlow,
				Game1.random.Next(1, 4) * 16,
				Game1.random.Next(data.MinTimePerFrame, data.MaxTimePerFrame),
				should_animate
			));
		}

	}

	protected virtual void Dispose(bool disposing) {
		if (!IsDisposed) {
			Debris = null!;

			Texture = null!;
			TextureName = null!;
			Mod.RemoveLoadsAsset(Id);
			IsDisposed = true;
		}
	}

	public void Dispose() {
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	public void ReloadAssets() {
		if (IsDisposed)
			return;

		Texture = Game1.content.Load<Texture2D>(TextureName);

		if (UsedTextureBoundsAsSource) {
			Sources = [Texture.Bounds];
			int width = Texture.Bounds.Width;
			int height = Texture.Bounds.Height;

			foreach (var debris in Debris) {
				debris.Width = width;
				debris.Height = height;
				debris.Position = new Vector2(Game1.random.Next(0, Game1.viewport.Width), Game1.random.Next(0, Game1.viewport.Height));
				debris.ClampToViewport();
			}
		}
	}

	#endregion

	#region Updating

	public void Resize(Point newSize, Point oldSize) {
		if (IsDisposed)
			return;

		foreach(var debris in Debris) {
			debris.Position = new Vector2(Game1.random.Next(0, Game1.viewport.Width), Game1.random.Next(0, Game1.viewport.Height));
			debris.ClampToViewport();
		}
	}

	public void MoveWithViewport(int offsetX, int offsetY) {
		if (IsDisposed)
			return;

		foreach(var debris in Debris) {
			debris.Position.X -= offsetX;
			debris.Position.Y -= offsetY;

			debris.ClampToViewport();
		}
	}

	public void Update(GameTime time) {
		if (IsDisposed)
			return;

		// First, update the wind.
		if (! HasUpdatedThisFrame) {
			HasUpdatedThisFrame = true;

			if (Game1.currentLocation.GetSeason() == Season.Fall) {
				if (WeatherDebris.globalWind == 0f)
					WeatherDebris.globalWind = -0.5f;

				if (Game1.random.NextDouble() < 0.001 && Game1.windGust == 0f && WeatherDebris.globalWind >= -0.5f) {
					Game1.windGust += Game1.random.Next(-10, -1) / 100f;
					Game1.playSound("wind", out Game1.wind);

				} else if (Game1.windGust != 0) {
					Game1.windGust = Math.Max(-5f, Game1.windGust * 1.02f);
					WeatherDebris.globalWind = -0.5f + Game1.windGust;
					if (Game1.windGust < -0.2f && Game1.random.NextDouble() < 0.007)
						Game1.windGust = 0f;
				}

				if (WeatherDebris.globalWind < -0.5f) {
					WeatherDebris.globalWind = Math.Min(-0.5f, WeatherDebris.globalWind + 0.015f);
					if (Game1.wind != null) {
						Game1.wind.SetVariable("Volume", (0f - WeatherDebris.globalWind) * 20f);
						Game1.wind.SetVariable("Frequency", (0f - WeatherDebris.globalWind) * 20f);
						if (WeatherDebris.globalWind == -0.5f)
							Game1.wind.Stop(Microsoft.Xna.Framework.Audio.AudioStopOptions.AsAuthored);
					}
				}

			} else {
				if (WeatherDebris.globalWind == 0f)
					WeatherDebris.globalWind = -0.25f;

				if (Game1.wind != null) {
					Game1.wind.Stop(Microsoft.Xna.Framework.Audio.AudioStopOptions.AsAuthored);
					Game1.wind = null;
				}
			}
		}

		foreach (var item in Debris)
			item.Update(time);
	}

	#endregion

	#region Drawing

	public void Draw(SpriteBatch batch, GameTime time, RenderTarget2D targetScreen) {
		if (IsDisposed)
			return;

		Color color = Color * Opacity;

		foreach (var debris in Debris) {
			if (debris.SourceIndex < 0 || debris.SourceIndex >= Sources.Length)
				debris.SourceIndex = Game1.random.Next(Sources.Length);

			debris.Draw(batch, Texture, Sources[debris.SourceIndex], color, Scale, Effects);
		}

	}

	#endregion

}
