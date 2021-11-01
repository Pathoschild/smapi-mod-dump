/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/CustomCommunityCentre
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using System;

namespace CustomCommunityCentre
{
	public class AreaCompleteNightEvent : StardewValley.Events.FarmEvent
	{
		public int CutsceneLengthTimer;
		public int TimerSinceFade;
		public int SoundTimer;
		public int SoundInterval = 99999;
		public string Sound;
		public bool WasRaining;
		public GameLocation Location;
		public GameLocation PreEventLocation;

		public readonly NetInt WhichEvent = new();

		public NetFields NetFields { get; } = new();


		public AreaCompleteNightEvent()
		{
			this.NetFields.AddField(WhichEvent);
		}

		public AreaCompleteNightEvent(int which)
			: this()
		{
			Log.D($"Loading area completion cutscene for area {which}.",
				CustomCommunityCentre.ModEntry.Config.DebugMode);

			this.WhichEvent.Value = which;
		}

		public void draw(SpriteBatch b)
		{
		}

		public void drawAboveEverything(SpriteBatch b)
		{
		}

		public void makeChangesToLocation()
		{
		}

		public bool setUp()
		{
			CustomCommunityCentre.Data.BundleMetadata bundleMetadata
				= Bundles.GetCustomBundleMetadataFromAreaNumber(this.WhichEvent.Value);

			// Add dialogue event
			string eventId = string.Format(Bundles.ActiveDialogueEventAreaCompleted, bundleMetadata.AreaName);
			Game1.player.activeDialogueEvents[eventId] = Bundles.ActiveDialogueEventAreaCompletedDuration;

			// Set game
			Game1.currentLightSources.Clear();
			this.CutsceneLengthTimer = bundleMetadata.AreaCompleteCutscene.Duration;
			this.WasRaining = Game1.isRaining;
			Game1.isRaining = false;
			this.PreEventLocation = Game1.currentLocation;

			// Set location
			this.Location = Game1.getLocationFromName(bundleMetadata.AreaCompleteCutscene.LocationName);
			this.Location.resetForPlayerEntry();
			Point targetTile = bundleMetadata.AreaCompleteCutscene.CameraTileLocation;
			Vector2 targetPixel = Utility.PointToVector2(targetTile) * Game1.tileSize;

			// Set music
			Game1.changeMusicTrack(bundleMetadata.AreaCompleteCutscene.Music);

			// Add sprites
			for (int i = 0; i < bundleMetadata.AreaCompleteCutscene.Actors.Count; ++i)
			{
				CustomCommunityCentre.Data.BundleMetadata.CutsceneActor cutsceneJunimo
					= bundleMetadata.AreaCompleteCutscene.Actors[i];

				float periodicLoopTime = 2000f + (i * 300f);
				float periodicRange = 16f;
				Vector2 positionOffset = new (0f, -1 * cutsceneJunimo.SourceRectangle.Height * cutsceneJunimo.Scale);

				this.Location.temporarySprites.Add(
					new TemporaryAnimatedSprite(
						textureName: cutsceneJunimo.SourceTexture,
						sourceRect: cutsceneJunimo.SourceRectangle,
						animationInterval: cutsceneJunimo.AnimationInterval,
						animationLength: cutsceneJunimo.AnimationFrames,
						numberOfLoops: 999,
						position: (Utility.PointToVector2(cutsceneJunimo.TileLocation) * Game1.tileSize) + positionOffset,
						flicker: false,
						flipped: false)
					{
						color = cutsceneJunimo.Colour,
						scale = cutsceneJunimo.Scale,
						layerDepth = 1f,
						xPeriodic = cutsceneJunimo.FloatHorizontally,
						xPeriodicLoopTime = periodicLoopTime,
						xPeriodicRange = periodicRange,
						yPeriodic = cutsceneJunimo.FloatVertically,
						yPeriodicLoopTime = periodicLoopTime,
						yPeriodicRange = periodicRange,
						light = bundleMetadata.AreaCompleteCutscene.DrawEffects,
						lightcolor = Color.DarkGoldenrod,
						lightRadius = 1f - (1f / 10000f * i)
					});
			}

			// Set effects
			if (bundleMetadata.AreaCompleteCutscene.DrawEffects)
			{
				LightSource lightSource = new (
					textureIndex: 4,
					position: Utility.PointToVector2(targetTile) * Game1.tileSize,
					radius: 4f,
					color: Color.DarkGoldenrod);
				Game1.currentLightSources.Add(lightSource);
				Utility.addSprinklesToLocation(this.Location, targetTile.X, targetTile.Y, 7, 4, 15000, 150, Color.LightCyan);
				Utility.addStarsAndSpirals(this.Location, targetTile.X + 1, targetTile.Y, 7, 4, 15000, 350, Color.White);
			}

			// Set sounds
			this.SoundTimer = this.SoundInterval = bundleMetadata.AreaCompleteCutscene.SoundInterval;
			this.Sound = bundleMetadata.AreaCompleteCutscene.Sound;

			// Set viewport
			Game1.currentLocation = this.Location;
			Game1.fadeClear();
			Game1.nonWarpFade = true;
			Game1.timeOfDay = 2400;
			Game1.displayHUD = false;
			Game1.viewportFreeze = true;
			Game1.player.position.X = -999999f;
			Game1.viewport.X = Math.Max(
				0,
				Math.Min(
					this.Location.map.DisplayWidth - Game1.viewport.Width,
					(targetTile.X * Game1.tileSize) - (Game1.viewport.Width / 2)));
			Game1.viewport.Y = Math.Max(
				0,
				Math.Min(
					this.Location.map.DisplayHeight - Game1.viewport.Height,
					(targetTile.Y * Game1.tileSize) - (Game1.viewport.Height / 2)));
			if (!this.Location.IsOutdoors)
			{
				Game1.viewport.X = (int)targetPixel.X - (Game1.viewport.Width / 2);
				Game1.viewport.Y = (int)targetPixel.Y - (Game1.viewport.Height / 2);
			}
			Game1.previousViewportPosition = new Vector2(Game1.viewport.X, Game1.viewport.Y);
			if (Game1.debrisWeather != null && Game1.debrisWeather.Count > 0)
			{
				Game1.randomizeDebrisWeatherPositions(Game1.debrisWeather);
			}
			Game1.randomizeRainPositions();

			CustomCommunityCentre.Events.Game.InvokeOnAreaCompleteCutsceneStarted(
				areaName: bundleMetadata.AreaName,
				areaNumber: this.WhichEvent.Value);

			return false;
		}

		public bool tickUpdate(GameTime time)
		{
			Game1.UpdateGameClock(time);
			this.Location.updateWater(time);
			this.CutsceneLengthTimer -= time.ElapsedGameTime.Milliseconds;
			if (this.TimerSinceFade > 0)
			{
				this.TimerSinceFade -= time.ElapsedGameTime.Milliseconds;
				Game1.globalFade = true;
				Game1.fadeToBlackAlpha = 1f;
				return this.TimerSinceFade <= 0;
			}
			if (this.CutsceneLengthTimer <= 0 && !Game1.globalFade)
			{
				Game1.globalFadeToBlack(this.endEvent, 0.01f);
			}
			this.SoundTimer -= time.ElapsedGameTime.Milliseconds;
			if (this.SoundTimer <= 0 && !string.IsNullOrWhiteSpace(this.Sound))
			{
				Game1.playSound(this.Sound);
				this.SoundTimer = this.SoundInterval;
			}
			return false;
		}

		public void endEvent()
		{
			Log.D($"Ending area completion cutscene.",
				CustomCommunityCentre.ModEntry.Config.DebugMode);

			if (this.PreEventLocation != null)
			{
				Game1.currentLocation = this.PreEventLocation;
				Game1.currentLocation.resetForPlayerEntry();
				this.PreEventLocation = null;
			}
			Game1.changeMusicTrack("none");
			this.TimerSinceFade = 1500;
			Game1.isRaining = this.WasRaining;
			Game1.getFarm().temporarySprites.Clear();
		}
	}
}
