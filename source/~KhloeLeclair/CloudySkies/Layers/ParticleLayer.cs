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

using Leclair.Stardew.CloudySkies.LayerData;
using Leclair.Stardew.CloudySkies.Models;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;

namespace Leclair.Stardew.CloudySkies.Layers;


public class ParticleLayer : IWeatherLayer, IDisposable {

	private readonly ModEntry Mod;

	public ulong Id { get; }

	public LayerDrawType DrawType { get; }

	private readonly string TextureName;

	private Texture2D Texture;

	private bool IsDisposed;

	#region Life Cycle

	public ParticleLayer(ModEntry mod, ulong id, ParticleLayerData data) {
		Mod = mod;
		Id = id;

		if (data.Texture is null)
			throw new ArgumentException("Texture cannot be null");

		TextureName = data.Texture;
		Texture = Game1.content.Load<Texture2D>(TextureName);
		Mod.MarkLoadsAsset(Id, TextureName);

	}

	protected virtual void Dispose(bool disposing) {
		if (!IsDisposed) {

			Texture = null!;

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

	public void ReloadAssets() {



	}

	public void Resize(Point newSize, Point oldSize) {



	}

	public void MoveWithViewport(int offsetX, int offsetY) {

	}

	public void Update(GameTime time) {

	}

	public void Draw(SpriteBatch batch, GameTime time, RenderTarget2D targetScreen) {

	}
}
