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

public class TextureScrollLayer : IWeatherLayer {

	private readonly ModEntry Mod;

	public ulong Id { get; }

	public LayerDrawType DrawType { get; }

	private Texture2D? Texture;
	private string? TextureName;
	private readonly Rectangle? _Source;

	private Rectangle Source => _Source ?? Texture?.Bounds ?? Rectangle.Empty;

	private int Width;
	private int Height;

	private readonly int Frames;
	private readonly int TimePerFrame;
	private int CurrentFrame;
	private int FrameAccumulator;

	private Vector2 Position;

	private bool IsSnow;

	private readonly Vector2 Speed;
	private readonly Vector2 ViewSpeed;

	private readonly SpriteEffects Effects;
	private readonly float Scale;

	private readonly Color Color;
	private readonly float Opacity;

	private bool IsDisposed;

	#region Life Cycle

	public TextureScrollLayer(ModEntry mod, ulong id, ITextureScrollLayerData data) {
		Mod = mod;
		Id = id;
		DrawType = data.Mode;

		IsSnow = data.IsSnow;

		TextureName = data.Texture;
		if (TextureName is not null) {
			Mod.MarkLoadsAsset(id, TextureName);
			Texture = Game1.content.Load<Texture2D>(TextureName);
		} else if (!IsSnow)
			throw new ArgumentNullException("Texture cannot be null");

		_Source = Texture is null ? null : data.Source;

		// If we get here with a null texture, this is snow.
		if (Texture is null)
			Frames = 16;
		else if (data.Frames.HasValue)
			Frames = data.Frames.Value;
		else
			Frames = 1;

		TimePerFrame = data.TimePerFrame;

		Scale = data.Scale;
		Speed = data.Speed;

		ViewSpeed = data.ViewSpeed;

		Width = (int) ((Source.Width <= 0 ? 16 : Source.Width) * Scale);
		Height = (int) ((Source.Height <= 0 ? 16 : Source.Height) * Scale);

		Effects = SpriteEffects.None;
		if (data.FlipHorizontal)
			Effects |= SpriteEffects.FlipHorizontally;
		if (data.FlipVertical)
			Effects |= SpriteEffects.FlipVertically;

		Color = data.Color ?? Color.White;
		Opacity = data.Opacity;

		RandomizePosition();
	}

	public void ReloadAssets() {
		if (TextureName is not null) {
			Texture = Game1.content.Load<Texture2D>(TextureName);
			Width = (int) ((Source.Width <= 0 ? 16 : Source.Width) * Scale);
			Height = (int) ((Source.Height <= 0 ? 16 : Source.Height) * Scale);
		}

		RandomizePosition();
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

	private void RandomizePosition() {
		Position = new Vector2(
			-Game1.random.Next(Width),
			-Game1.random.Next(Height)
		);
	}

	public void MoveWithViewport(int offsetX, int offsetY) {

	}

	public void Draw(SpriteBatch batch, GameTime time, RenderTarget2D targetScreen) {

		Color color = Color * Opacity;
		if (IsSnow)
			color *= Game1.options.snowTransparency;

		Rectangle source = Texture is not null
			? CurrentFrame > 0
				? new Rectangle(
					Source.X + CurrentFrame * Source.Width,
					Source.Y,
					Source.Width,
					Source.Height
				)
				: Source
			: new Rectangle(
				368 + CurrentFrame * 16,
				192,
				16, 16
			);

		var texture = Texture ?? Game1.mouseCursors;

		int x = (int) Position.X;
		int y = (int) Position.Y;

		while (x < Game1.viewport.Width) {
			int j = y;

			while (j < Game1.viewport.Height) {
				batch.Draw(
					texture,
					new Vector2(x, j),
					source,
					color,
					0f,
					Vector2.Zero,
					Scale,
					Effects,
					1f
				);

				j += Height;
			}

			x += Width;
		}
	}

	public void Resize(Point newSize, Point oldSize) {
		RandomizePosition();
	}

	public void Update(GameTime time) {

		if (Frames > 0) {
			FrameAccumulator += time.ElapsedGameTime.Milliseconds;
			if (FrameAccumulator >= TimePerFrame) {
				FrameAccumulator = 0;
				CurrentFrame = (CurrentFrame + 1) % Frames;
			}
		}

		if (ViewSpeed.X != 0 || ViewSpeed.Y != 0)
			Position = UpdatePosition(
				w: Position,
				current: new Vector2(Game1.viewport.X, Game1.viewport.Y),
				previous: Game1.previousViewportPosition,
				viewSpeed: ViewSpeed
			);

		Position.X += Speed.X * time.ElapsedGameTime.Milliseconds;
		Position.Y += Speed.Y * time.ElapsedGameTime.Milliseconds;

		if (Position.X <= -Width)
			Position.X += Width;
		if (Position.X > 0)
			Position.X -= Width;

		if (Position.Y <= -Height)
			Position.Y += Height;
		if (Position.Y > 0)
			Position.Y -= Height;
	}

	private static Vector2 UpdatePosition(Vector2 w, Vector2 current, Vector2 previous, Vector2 viewSpeed) {
		w.Y += (current.Y - previous.Y) * viewSpeed.Y;
		w.X += (current.X - previous.X) * viewSpeed.X;

		return w;
	}

}
