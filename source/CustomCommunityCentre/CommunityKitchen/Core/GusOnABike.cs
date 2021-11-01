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
using StardewValley;
using StardewValley.BellsAndWhistles;
using System;
using System.Linq;

namespace CommunityKitchen
{
	public class GusOnABike : Critter
	{
		public class Sprite
		{
			public int SourceIndex = 0;
			public int MaxSourceIndex = 4;
			public Rectangle SourceRect = new(0, 0, 32, 48);
			public Rectangle ShadowSourceRect = new(0, 48, 32, 8);
			public float Scale = Game1.pixelZoom;
			public string TextureAssetKey = GusDeliveryService.DeliveryTextureAssetKey;
			public Lazy<Texture2D> Texture;
		}

		// Constant values
		public float MinimumDurationBetweenBounces = 75f;
		public float MaxBounceTime = 30f;
		public float MaxTilesPerSecond = 6f;
		public float MinTilesPerSecond = 0.3f;
		public float TilesToMaxSpeed = 2f;
		public float MaxWaitTime = 275f;

		// Variables
		public new Sprite sprite;
		public int BounceTimer = 0;
		public int WaitTimer;
		public bool DeliveryComplete = false;
		public readonly Vector2 TargetPosition;

		/*
		public readonly ICue SoundCue;
		public string SoundCueName = "cm:scooter_loop";
		public int SoundRange = 1024;
		private int SoundUpdateTimer;
		private int MaxSoundUpdateTime = 100;
		private int MaxFrequency = 100;
		*/


		public GusOnABike()
		{
			this.WaitTimer = (int)this.MaxWaitTime;

			this.flip = true;
			this.sprite = new Sprite
			{
				Texture = GusDeliveryService.DeliveryTexture,
				TextureAssetKey = GusDeliveryService.DeliveryTextureAssetKey
			};

			Farm farm = Game1.getFarm();
			Vector2 mailboxPosition = Utility.PointToVector2(farm.mapMainMailboxPosition ?? Game1.MasterPlayer.getMailboxPosition());
			this.TargetPosition = (mailboxPosition + new Vector2(1f, 2.5f)) * Game1.tileSize;
			this.position = this.startingPosition = new Vector2(
				x: farm.Map.DisplayWidth + (this.sprite.SourceRect.Width * this.sprite.Scale * 10),
				y: this.TargetPosition.Y);

			/*
			this.SoundUpdateTimer = this.MaxSoundUpdateTime;
			this.SoundCue = Game1.soundBank.GetCue(
				"heavyEngine"
				//this.SoundCueName
				);
			this.SoundCue.Play();
			this.SoundCue.Pause();
			*/
		}

		internal static void Create()
		{
			GameLocation where = Game1.getFarm();
			foreach (GusOnABike gus in where.critters.OfType<GusOnABike>().ToList())
			{
				where.critters.Remove(gus);
				CustomCommunityCentre.Bundles.BroadcastPuffSprites(
					multiplayer: null,
					location: where,
					tilePosition: gus.position / Game1.tileSize);
			}
			where.addCritter(new GusOnABike());
		}

		internal static void Honk(bool isOnFarm)
		{
			string cue = ModEntry.Instance.Helper.ModRegistry.IsLoaded("Platonymous.CustomMusic")
				? "scooter_horn"
				: "questcomplete";
			Game1.getFarm().localSound(cue);
			if (!isOnFarm)
			{
				Game1.playSound(cue);
			}
		}

		public static bool IsGusOnFarm()
		{
			bool isHe = Game1.getFarm().critters.Any(c => c is GusOnABike);
			return isHe;
		}

		public static string WhereGus()
		{
			return Game1.getCharacterFromName(GusDeliveryService.ShopOwner).currentLocation?.Name;
		}

		/*
		internal void StopSounds()
		{
			SoundCue.SetVariable("Frequency", GusOnABike.MaxFrequency);
			SoundCue.Stop(Microsoft.Xna.Framework.Audio.AudioStopOptions.Immediate);
		}
		*/

		public override bool update(GameTime time, GameLocation environment)
		{
			this.sprite.SourceIndex = Math.Max(0, (int)Math.Ceiling(this.sprite.MaxSourceIndex * (this.BounceTimer / this.MaxBounceTime)));

			bool isAtTargetPosition = this.position.X <= this.TargetPosition.X;
			bool isAtEndPosition = this.position.X > this.startingPosition.X;
			bool isDoneWaiting = this.WaitTimer <= 0;

			float maxDelta = this.TilesToMaxSpeed * Game1.tileSize;
			float delta = Math.Abs(this.TargetPosition.X - this.position.X);
			float positionDelta = Math.Min(maxDelta, delta);
			float speedRatio = Math.Max(0, positionDelta / maxDelta);

			if (time.TotalGameTime.Ticks % 2 == 0)
			{
				this.yOffset = Game1.random.Next(0, (int)(0.75f * Game1.pixelZoom * (speedRatio + 0.5f)));
			}

			--this.BounceTimer;
			if (!isAtTargetPosition || isDoneWaiting)
			{
				// Gus drives around whenever he isn't waiting
				float tilesPerTick = (float)(Math.Max(this.MinTilesPerSecond, (this.MaxTilesPerSecond * speedRatio))
					* time.ElapsedGameTime.TotalSeconds);
				this.position.X += tilesPerTick * Game1.tileSize * (this.flip ? -1 : 1);

				/*
				this.SoundCue.SetVariable("Frequency", GusOnABike.MaxFrequency * speedRatio);
				*/

				// Bounce Gus on his bike every once in a while
				if (speedRatio > 0.8f
					&& this.BounceTimer < -(this.MinimumDurationBetweenBounces)
					&& this.position.X < this.startingPosition.X - (this.sprite.SourceRect.Width * this.sprite.Scale * 10)
					&& Game1.random.NextDouble() < 0.025f)
				{
					this.BounceTimer = (int)this.MaxBounceTime - 1;
					Game1.getFarm().localSoundAt("dwop", this.position / Game1.tileSize);
				}
			}
			else
			{
				--this.WaitTimer;
			}

			if (!this.DeliveryComplete)
			{
				if (isAtTargetPosition && this.WaitTimer <= this.MaxWaitTime / 3)
				{
					// Complete delivery when Gus has driven to the target and waited a while
					this.DeliveryComplete = true;
					GusDeliveryService.AddDeliveryChests();
				}
			}
			else
			{
				if (isDoneWaiting && this.flip)
				{
					// Turn around to leave when done waiting
					this.flip = false;
					GusOnABike.Honk(isOnFarm: Game1.player.currentLocation is Farm);
				}
				if (isAtEndPosition)
				{
					// Destroy Gus when he drives into the void

					/*
					this.StopSounds();
					*/
					
					return true;
				}
			}

			/*
			{
				// Ambient sound logic taken from StardewValley.BellsAndWhistles.AmbientLocationSounds
				this.SoundUpdateTimer -= time.ElapsedGameTime.Milliseconds;
				if (this.SoundUpdateTimer <= 0)
				{
					float distance = Vector2.Distance(this.position, Game1.player.getStandingPosition());
					this.SoundUpdateTimer = this.MaxSoundUpdateTime;
					if (distance <= this.SoundRange)
					{
						float volume = Math.Min(1f, 1f - (distance / this.SoundRange));
						float realvolume = volume * 100f * //Math.Min(Game1.ambientPlayerVolume, Game1.options.ambientVolumeLevel);
							Game1.options.soundVolumeLevel;
						this.SoundCue.SetVariable("Volume", realvolume / 10);
						this.SoundCue.Resume();
					}
				}
			}
			*/

			return false;
		}

		public override void draw(SpriteBatch b)
		{
			if (this.sprite == null || this.sprite.Texture == null)
				return;

			float scale = this.sprite.Scale;
			Rectangle[] sourceRects = new [] { this.sprite.SourceRect, this.sprite.ShadowSourceRect };
			for (int i = 0; i < sourceRects.Length; ++i)
			{
				Vector2 drawPosition = this.position + new Vector2(0, (-1 * sourceRects[i].Height * scale) - (i == 0 ? this.yOffset : 0));
				Vector2 localPosition = Game1.GlobalToLocal(
					viewport: Game1.viewport,
					globalPosition: drawPosition);
				Rectangle rect = sourceRects[i];
				rect.X += this.sprite.ShadowSourceRect.Width * this.sprite.SourceIndex;

				const float layerDepthDivisor = 5.5f;
				float layerDepth // Why 5.5f and + 0.01f? I have no idea, but it works on the standard farm
					= 0.01f + ((drawPosition.X / layerDepthDivisor) / 10000f) + ((drawPosition.Y / layerDepthDivisor - i) / 10000f);

				b.Draw(
					texture: this.sprite.Texture.Value,
					position: localPosition,
					sourceRectangle: rect,
					color: Color.White,
					rotation: 0f,
					origin: Vector2.Zero,
					scale: scale,
					effects: this.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
					layerDepth: layerDepth);
			}
		}
	}
}
