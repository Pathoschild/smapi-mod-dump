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

using Leclair.Stardew.MoreNightlyEvents.Models;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.TerrainFeatures;

namespace Leclair.Stardew.MoreNightlyEvents.Events;

public class GrowthEvent : BaseFarmEvent<GrowthEventData> {

	private readonly List<FairySprite> Sprites = new();

	private int LastFairy = 0;
	private int timeEllapsed;

	public GrowthEvent(string key, GrowthEventData? data = null) : base(key, data) {

	}

	protected void AddFairy() {

		int x = Game1.viewport.X + Game1.viewport.Width + 128;
		int y = Game1.viewport.Y - 128 + Game1.random.Next(Game1.viewport.Height + 256);

		float speed = (float) Game1.random.NextDouble() + 0.5f;

		Sprites.Add(
			new FairySprite(
				id: LastFairy++,
				location: Game1.currentLocation,
				position: new Vector2(x, y),
				speed: speed,
				target: null
			)
		);
	}

	#region FarmEvent

	public override bool setUp() {
		if (!LoadData())
			return true;

		var loc = GetLocation();
		if (loc is null)
			return true;

		if (Game1.IsRainingHere(loc))
			return true;

		var pairs = loc.terrainFeatures.Pairs.Where(
			x => x.Value is HoeDirt hd &&
				hd.crop != null &&
				!hd.crop.isWildSeedCrop() &&
				hd.crop.currentPhase.Value < hd.crop.phaseDays.Count - 1
		).ToArray();

		ModEntry.Instance.Log($"Valid Targets: {pairs.Length}");

		if (pairs.Length == 0)
			return true;

		if (!Data.DrawFaeries)
			return false;

		// Try to enter the location.
		if (!EnterLocation())
			return true;

		// Lock. Down. Everything.
		Game1.fadeClear();
		Game1.nonWarpFade = true;
		Game1.timeOfDay = 2400;
		Game1.displayHUD = false;
		Game1.freezeControls = true;
		Game1.viewportFreeze = true;
		Game1.displayFarmer = false;

		// Now try to center the map on our target.
		double x = 0.0;
		double y = 0.0;

		foreach(var entry in pairs) {
			x += entry.Key.X;
			y += entry.Key.Y;
		}

		x /= pairs.Length;
		y /= pairs.Length;

		Game1.viewport.X = Math.Max(0, Math.Min(loc.map.DisplayWidth - Game1.viewport.Width, (int) x * 64 - Game1.viewport.Width / 2));
		Game1.viewport.Y = Math.Max(0, Math.Min(loc.map.DisplayHeight - Game1.viewport.Height, (int) y * 64 - Game1.viewport.Height / 2));

		Game1.changeMusicTrack("nightTime");

		// Add a single fairy.
		AddFairy();

		return false;
	}

	public override void InterruptEvent() {
		Sprites.Clear();
	}

	public override bool tickUpdate(GameTime time) {
		if (Sprites.Count == 0) {
			Game1.globalFadeToClear();
			Game1.changeMusicTrack("none");
			return true;
		}

		Game1.UpdateGameClock(time);
		Game1.currentLocation.UpdateWhenCurrentLocation(time);
		Game1.currentLocation.updateEvenIfFarmerIsntHere(time);
		Game1.UpdateOther(time);

		for(int i = Sprites.Count - 1; i >= 0; i--) {
			var sprite = Sprites[i];
			if (!sprite.Update(time))
				Sprites.RemoveAt(i);
		}

		timeEllapsed += time.ElapsedGameTime.Milliseconds;

		if (timeEllapsed > 500 && LastFairy == 1)
			AddFairy();

		if (timeEllapsed > 1000 && LastFairy == 2) {
			AddFairy();
			AddFairy();
			AddFairy();
			AddFairy();
		}

		if (timeEllapsed > 5000 && LastFairy < 10) {
			AddFairy();
			AddFairy();
			AddFairy();
			AddFairy();
			AddFairy();
			AddFairy();
			AddFairy();
			AddFairy();
		}

		return false;
	}

	public override void draw(SpriteBatch b) {
		foreach (var sprite in Sprites)
			sprite.Draw(b);
	}

	public override void makeChangesToLocation() {
		if (!Game1.IsMasterGame)
			return;

		var pairs = Game1.currentLocation.terrainFeatures.Pairs.Where(
			x => x.Value is HoeDirt hd &&
				hd.crop != null &&
				!hd.crop.isWildSeedCrop() &&
				hd.crop.currentPhase.Value < hd.crop.phaseDays.Count - 1
		).ToArray();

		foreach (var entry in pairs)
			if (entry.Value is HoeDirt hd && hd.crop != null)
				hd.crop.growCompletely();

		PerformSideEffects(Game1.currentLocation, Game1.player);

	}

	#endregion

}

public class FairySprite : IDisposable {

	public readonly int Id;
	public readonly GameLocation Location;
	public float Speed;

	private Vector2 Position;
	private Vector2? Target;
	private LightSource Light;
	private int Frame;
	private bool disposedValue;

	public FairySprite(int id, GameLocation location, Vector2 position, float speed, Vector2? target) {
		Id = id;
		Location = location;
		Position = position;
		Speed = speed;
		Target = target;

		Light = new LightSource(
			textureIndex: 4,
			position: Position,
			radius: 1f,
			color: Color.Black,
			identifier: 642069 + Id,
			light_context: LightSource.LightContext.None,
			playerID: 0L
		);

		Game1.currentLightSources.Add(Light);
	}

	public bool Update(GameTime time) {
		Light.position.Value = Position;

		Position.X -= time.ElapsedGameTime.Milliseconds * 0.15f * Speed;
		Position.Y += MathF.Cos(time.TotalGameTime.Milliseconds * (float)Math.PI / 512f) * 1f * Speed;

		if (Position.X + 128 < Game1.viewport.X)
			return false;

		int oldFrame = Frame;
		Frame = time.TotalGameTime.Milliseconds % 500 > 250 ? 1 : 0;

		if (oldFrame != Frame && Frame == 1) {
			Game1.playSound("batFlap");
			Location.TemporarySprites.Add(new TemporaryAnimatedSprite(
				rowInAnimationTexture: 11,
				position: Position + new Vector2(32, 0),
				color: Color.Purple
			));
		}

		return true;
	}

	public void Draw(SpriteBatch b) {
		b.Draw(
			texture: Game1.mouseCursors,
			position: Game1.GlobalToLocal(
				Game1.viewport, Position
			),
			sourceRectangle: new Rectangle(
				16 + Frame * 16, 592,
				16, 16
			),
			color: Color.White,
			rotation: 0f,
			origin: Vector2.Zero,
			scale: 4f,
			effects: SpriteEffects.None,
			layerDepth: 0.9999999f
		);
	}

	protected virtual void Dispose(bool disposing) {
		if (!disposedValue) {
			if (disposing)
				Game1.currentLightSources.Remove(Light);

			Light = null!;
			disposedValue = true;
		}
	}

	public void Dispose() {
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
