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

using Leclair.Stardew.CloudySkies.Models;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace Leclair.Stardew.CloudySkies.Layers;

public class RainLayer : IWeatherLayer {

	private readonly ModEntry Mod;

	public ulong Id { get; }

	public LayerDrawType DrawType { get; }

	private Texture2D? Texture;
	private string? TextureName;
	private readonly Rectangle? _Source;

	private Rectangle Source => _Source ?? Texture?.Bounds ?? Rectangle.Empty;


	private int Frames;

	private readonly Vector2 Speed;

	private readonly SpriteEffects Effects;
	private readonly float Scale;

	private readonly Color Color;
	private readonly float Opacity;
	private readonly int Vibrancy;

	private bool IsDisposed;

	private readonly RainDrop[] Drops;

	#region Life Cycle

	public RainLayer(ModEntry mod, ulong id, IRainLayerData data) {
		Mod = mod;
		Id = id;
		DrawType = data.Mode;

		TextureName = data.Texture;
		if (TextureName is not null) {
			Mod.MarkLoadsAsset(id, TextureName);
			Texture = Game1.content.Load<Texture2D>(TextureName);
		}

		_Source = Texture is null ? null : data.Source;
		Frames = Texture is null ? 4 : data.Frames - 1;

		Scale = data.Scale;
		Speed = data.Speed;

		Effects = SpriteEffects.None;
		if (data.FlipHorizontal)
			Effects |= SpriteEffects.FlipHorizontally;
		if (data.FlipVertical)
			Effects |= SpriteEffects.FlipVertically;

		Color = data.Color ?? Color.White;
		Opacity = data.Opacity;
		Vibrancy = data.Vibrancy;

		Drops = new RainDrop[data.Count];
		RandomizeDrops();
	}

	public void ReloadAssets() {
		if (TextureName is not null)
			Texture = Game1.content.Load<Texture2D>(TextureName);

		RandomizeDrops();
	}

	protected virtual void Dispose(bool disposing) {
		if (!IsDisposed) {
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

	#endregion

	private void RandomizeDrops() {
		for (int i = 0; i < Drops.Length; i++) {
			Drops[i] = new RainDrop(
				Game1.random.Next(Game1.viewport.Width),
				Game1.random.Next(Game1.viewport.Height),
				Game1.random.Next(Frames),
				Game1.random.Next(70)
			);
		}
	}


	public void MoveWithViewport(int offsetX, int offsetY) {
		int maxY = Game1.viewport.Height + 64;
		int maxX = Game1.viewport.Width + 64;

		for (int i = 0; i < Drops.Length; i++) {
			RainDrop drop = Drops[i];
			drop.position.X -= offsetX;
			drop.position.Y -= offsetY;

			if (drop.position.Y > maxY)
				drop.position.Y = -64f;
			else if (drop.position.Y < -64f)
				drop.position.Y = maxY;

			if (drop.position.X > maxX)
				drop.position.X = -64f;
			else if (drop.position.X < -64f)
				drop.position.X = maxX;

			Drops[i] = drop;
		}
	}


	public void Draw(SpriteBatch batch, GameTime time, RenderTarget2D targetScreen) {

		Rectangle source = Source;
		Color color = Color * Opacity;

		for (int i = 0; i < Drops.Length; i++) {
			RainDrop drop = Drops[i];
			for (int v = 0; v < Vibrancy; v++) {
				if (Texture != null)
					batch.Draw(
						Texture,
						drop.position,
						new Rectangle(
							source.X + (drop.frame * source.Width),
							source.Y,
							source.Width,
							source.Height
						),
						color,
						0f,
						Vector2.Zero,
						Scale,
						Effects,
						1f
					);
				else
					batch.Draw(
						Game1.rainTexture,
						drop.position,
						Game1.getSourceRectForStandardTileSheet(Game1.rainTexture, drop.frame + (Color == Color.White ? 0 : 4), 16, 16),
						color,
						0f,
						Vector2.Zero,
						Scale,
						Effects,
						1f
					);
			}
		}

	}

	public void Resize(Point newSize, Point oldSize) {
		RandomizeDrops();
	}

	public void Update(GameTime time) {
		int length = Drops.Length;

		for (int i = 0; i < length; i++) {
			RainDrop drop = Drops[i];
			drop.accumulator += time.ElapsedGameTime.Milliseconds;

			if (drop.accumulator > 70) {
				drop.accumulator = 0;

				if (drop.frame == 0) {
					drop.position += new Vector2(Speed.X + i * (Speed.X < 0 ? 8 : -8) / length, Speed.Y + i * (Speed.Y < 0 ? 8 : -8) / length);

					if (drop.position.Y > (Game1.viewport.Height + 64))
						drop.position.Y = -64f;

					if (drop.position.X > (Game1.viewport.Width + 64))
						drop.position.X = -64f;
					else if (drop.position.X < -64f)
						drop.position.X = Game1.viewport.Width + 64f;

					if (Game1.random.NextDouble() < 0.1)
						drop.frame++;

				} else {
					drop.frame = (drop.frame + 1) % Frames;
					if (drop.frame == 0)
						drop.position = new Vector2(Game1.random.Next(Game1.viewport.Width), Game1.random.Next(Game1.viewport.Height));
				}
			}

			// Push the modified drop back into the array.
			Drops[i] = drop;
		}
	}
}
