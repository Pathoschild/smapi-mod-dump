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

public class ColorLayer : IWeatherLayer {

	public ulong Id { get; }

	public LayerDrawType DrawType { get; }

	private Color Color;
	private float Opacity;

	public ColorLayer(ulong id, IColorLayerData data) {

		Id = id;
		DrawType = data.Mode;

		Color = data.Color ?? Color.White;
		Opacity = Math.Clamp(data.Opacity, 0f, 1f);
	}

	public void ReloadAssets() {

	}

	public void Resize(Point newSize, Point oldSize) {

	}

	public void MoveWithViewport(int offsetX, int offsetY) {

	}

	public void Update(GameTime time) {

	}

	public void Draw(SpriteBatch batch, GameTime time, RenderTarget2D targetScreen) {

		batch.Draw(
			Game1.fadeToBlackRect,
			batch.GraphicsDevice.Viewport.Bounds,
			Game1.fadeToBlackRect.Bounds,
			Color * Opacity,
			0f,
			Vector2.Zero,
			SpriteEffects.None,
			1f
		);

	}
}
