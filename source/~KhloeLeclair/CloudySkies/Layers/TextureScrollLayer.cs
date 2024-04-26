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
using System.Text;
using System.Threading.Tasks;

using Leclair.Stardew.CloudySkies.LayerData;
using Leclair.Stardew.CloudySkies.Models;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Enchantments;

namespace Leclair.Stardew.CloudySkies.Layers;


public class TextureScrollLayer : IWeatherLayer, IDisposable {

	private readonly ModEntry Mod;

	public ulong Id { get; }

	public LayerDrawType DrawType { get; }

	private Texture2D Texture;
	private string TextureName;
	private Rectangle? _Source;

	private Rectangle Source => _Source ?? Texture.Bounds;

	private Vector2 Origin;
	private Vector2 Speed;

	private SpriteEffects Effects;
	private float Scale;

	private Color Color;
	private float Opacity;

	private int Width;
	private int Height;

	private bool IsDisposed;

	#region Life Cycle

	public TextureScrollLayer(ModEntry mod, ulong id, TextureScrollLayerData data) {
		Mod = mod;
		Id = id;

		TextureName = data.Texture ?? throw new ArgumentNullException("Texture cannot be null");
		Mod.MarkLoadsAsset(Id, TextureName);
		Texture = Game1.content.Load<Texture2D>(TextureName);
		_Source = data.Source;

		Scale = data.Scale;
		Speed = data.Speed;
		DrawType = data.Mode;

		Effects = SpriteEffects.None;
		if (data.FlipHorizontal)
			Effects |= SpriteEffects.FlipHorizontally;
		if (data.FlipVertical)
			Effects |= SpriteEffects.FlipVertically;

		Color = data.Color ?? Color.White;
		Opacity = data.Opacity;

		Width = (int) (Source.Width * Scale);
		Height = (int) (Source.Height * Scale);

		Origin = new Vector2(-Game1.random.Next(Width), -Game1.random.Next(Height));
	}

	public void ReloadAssets() {
		Texture = Game1.content.Load<Texture2D>(TextureName);
		Width = (int) (Source.Width * Scale);
		Height = (int) (Source.Height * Scale);
		ClampToViewport();
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

	#region Updating

	private void ClampToViewport() {
		// We need to clamp our X and Y such that:
		// -Width < X <= 0
		// -Height < Y <= 0

		if (Origin.X <= -Width)
			Origin.X += Width;
		if (Origin.X > 0)
			Origin.X -= Width;

		if (Origin.Y <= -Height)
			Origin.Y += Height;
		else if (Origin.Y > 0)
			Origin.Y -= Height;
	}

	public void MoveWithViewport(int offsetX, int offsetY) {
		Origin.X -= offsetX;
		Origin.Y -= offsetY;
		ClampToViewport();
	}

	public void Resize(Point newSize, Point oldSize) {
		ClampToViewport();
	}

	public void Update(GameTime time) {
		Origin.X += Speed.X * time.ElapsedGameTime.Milliseconds;
		Origin.Y += Speed.Y * time.ElapsedGameTime.Milliseconds;

		ClampToViewport();
	}

	#endregion

	#region Drawing

	public void Draw(SpriteBatch batch, GameTime time, RenderTarget2D targetScreen) {
		if (IsDisposed)
			return;

		int x = (int) Origin.X;
		int y = (int) Origin.Y;
		Color c = Color * Opacity;

		while(x < Game1.viewport.Width) {
			int j = y;

			while(j < Game1.viewport.Height) {
				batch.Draw(
					Texture,
					new Vector2(x, j),
					Source,
					c,
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

	#endregion
}
