using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SObject = StardewValley.Object;

namespace Cropbeasts
{
	public class Spawner
	{
		protected static IModHelper Helper => ModEntry.Instance.Helper;
		protected static IMonitor Monitor => ModEntry.Instance.Monitor;
		protected static ModConfig Config => ModConfig.Instance;

		internal class Message
		{
			public string locationName;
			public Point tileLocation;
			public int harvestIndex;
			public bool giantCrop;
			public int baseQuantity;
			public bool showWitchFlyover;
		}

		public readonly GuestCropTile cropTile;
		private readonly GameLocation location;
		private readonly int baseQuantity;

		private readonly List<Cropbeast> beasts;
		private readonly bool showWitchFlyover;

		private Texture2D cropTexture;
		private Rectangle cropSourceRect;
		private Vector2 cropPosition;
		private List<Vector2> cropOffsets;
		private float cropTint;
		private float cropRotation;
		private Vector2 cropOrigin;
		private float cropScale;
		private bool cropFlipped;
		private float cropLayerDepth;

		private bool shakeLeft;
		private float maxShake;
		private float shakeRate;
		private Vector2 levitateRate;
		private float corruptRate;

		public static void Spawn (CropTile cropTile)
		{
			// Check preconditions.
			if (!Context.IsMainPlayer)
				throw new InvalidOperationException ("Only the host can do that.");

			// Find the nature of the beast.
			ConstructorInfo ctor = cropTile.mapping.type?.GetConstructor
				(new Type[] { typeof (CropTile), typeof (bool) });
			if (ctor == null)
				throw new Exception ($"Invalid cropbeast type '{cropTile.mapping.beastName}'.");

			// Create the beast(s).
			List<Cropbeast> beasts = new List<Cropbeast> ();
			beasts.Add (ctor.Invoke (new object[] { cropTile, true }) as Cropbeast);
			if (cropTile.baseQuantity > 1 && !cropTile.giantCrop)
			{
				for (int i = 1; i < cropTile.baseQuantity; ++i)
					beasts.Add (ctor.Invoke (new object[] { cropTile, false }) as Cropbeast);
			}

			// Register the beast(s) centrally in advance.
			foreach (Cropbeast beast in beasts)
				ModEntry.Instance.registerMonster (beast);

			// Have the witch fly by only if configured, only outdoors and only
			// on the first real spawn of the day.
			bool showWitchFlyover = Config.WitchFlyovers &&
				cropTile.location.IsOutdoors && !cropTile.fake &&
				!ModEntry.Instance.chooser.witchFlyoverShown;

			// Create the host spawner here.
			new Spawner (cropTile, cropTile.location, cropTile.baseQuantity,
				beasts, showWitchFlyover);

			// Signal farmhands to create guest spawners.
			Message message = new Message
			{
				locationName = cropTile.location.Name,
				tileLocation = cropTile.tileLocation,
				harvestIndex = cropTile.harvestIndex,
				giantCrop = cropTile.giantCrop,
				baseQuantity = cropTile.baseQuantity,
				showWitchFlyover = showWitchFlyover,
			};
			Helper.Multiplayer.SendMessage (message, "CreateSpawner",
				modIDs: new string[] { ModEntry.Instance.ModManifest.UniqueID });
		}

		internal static void Create (Message message)
		{
			// Check preconditions.
			if (Context.IsMainPlayer)
				throw new InvalidOperationException ("Only farmhands can do that.");

			// Create guest spawner for farmhand.
			GuestCropTile cropTile = GuestCropTile.Create (message.harvestIndex,
				message.giantCrop, message.tileLocation);
			GameLocation location = Game1.getLocationFromName
				(message.locationName);
			new Spawner (cropTile, location, message.baseQuantity, null,
				message.showWitchFlyover);
		}

		private Spawner (GuestCropTile cropTile, GameLocation location,
			int baseQuantity, List<Cropbeast> beasts, bool showWitchFlyover)
		{
			this.cropTile = cropTile;
			this.location = location;
			this.baseQuantity = baseQuantity;
			this.beasts = beasts;
			this.showWitchFlyover = showWitchFlyover;

			if (showWitchFlyover)
				witchFlyover ();
			else
				startTransformation ();
		}

		private void witchFlyover ()
		{
			if (Context.IsMainPlayer)
				ModEntry.Instance.chooser.witchFlyoverShown = true;

			// She flies by (from "witchFlyby" SpecificTemporarySprite).
			Game1.screenOverlayTempSprites.Add (new TemporaryAnimatedSprite
				(Game1.mouseCursorsName, new Rectangle (276, 1886, 35, 29),
				9999f, 1, 999999, new Vector2 (Game1.viewport.Width, 192f),
				flicker: false, flipped: false, 1f, 0f, Color.White, 4f, 0f, 0f, 0f)
				{
					motion = new Vector2 (-4f, 0f),
					acceleration = new Vector2 (-0.025f, 0f),
					yPeriodic = true,
					yPeriodicLoopTime = 2000f,
					yPeriodicRange = 64f,
					local = true
				});

			// She cackles (from WitchEvent).
			DelayedAction.playSoundAfterDelay ("cacklingWitch", 1250);

			// Proceed to next stage.
			DelayedAction.functionAfterDelay (startTransformation, 2500);
		}

		private void lightWickedStatues ()
		{
			foreach (SObject wickedStatue in
				Utilities.FindWickedStatues (location))
			{
				Vector2 pos = wickedStatue.TileLocation;
				wickedStatue.shakeTimer = 1000;
				wickedStatue.showNextIndex.Value = true;
				Game1.currentLightSources.Add (new LightSource (4, pos * 64f + new Vector2 (32f, 0f), 1f, Color.Cyan * 0.75f, (int) (pos.X * 797f + pos.Y * 13f + 666f), LightSource.LightContext.None, 0L));
			}
		}

		private void startTransformation ()
		{
			// The spell is cast.
			DelayedAction.screenFlashAfterDelay (2f, 10, "flameSpell");

			// Wicked Statues take notice (as when usual farm monsters spawn)
			lightWickedStatues ();

			// Start drawing our fake crop.
			prepareFakeCrop ();

			// Completely remove the real crop for now.
			if (Context.IsMainPlayer)
				(cropTile as CropTile).spawn ();

			// Shake twice...
			DelayedAction.functionAfterDelay (shakeCrop,  750);
			DelayedAction.functionAfterDelay (shakeCrop, 1750);

			// Then levitate and corrupt the crop with a building sound...
			DelayedAction.functionAfterDelay (levitateCrop, 2750);

			// Then with a magic poof...
			DelayedAction.functionAfterDelay (() => poofCrop ("wand", true), 4750);

			// Finally replace the crop with the beast.
			DelayedAction.functionAfterDelay (finishTransformation, 5000);
		}

		private void prepareFakeCrop ()
		{
			cropTexture = cropTile.cropTexture;
			cropSourceRect = cropTile.cropSourceRect;
			cropTint = 0f;
			cropRotation = 0f;
			cropScale = 4f;
			cropOffsets = new List<Vector2> { Vector2.Zero };
			levitateRate = new Vector2 ();

			if (cropTile.giantCrop)
			{
				cropPosition = new Vector2 ((cropTile.tileLocation.X + 1.5f) * 64f,
					(cropTile.tileLocation.Y + 2f) * 64f);
				cropOrigin = new Vector2 (cropTile.cropSourceRect.Width * 0.5f,
					cropTile.cropSourceRect.Height * 0.75f);
				cropFlipped = false;
				cropLayerDepth = (cropTile.tileLocation.Y + 2f) * 64f / 10000f;
			}
			else
			{
				cropPosition = Helper.Reflection.GetField<Vector2>
					(cropTile.crop, "drawPosition").GetValue ();
				cropOrigin = new Vector2 (8f, 24f);
				cropFlipped = cropTile.crop.flip.Value;
				cropLayerDepth = Helper.Reflection.GetField<float>
					(cropTile.crop, "layerDepth").GetValue ();
			}

			Helper.Events.GameLoop.UpdateTicked += updateCrop;
			Helper.Events.Display.RenderedWorld += drawCrop;
		}

		private void shakeCrop ()
		{
			Game1.playSound ("leafrustle");
			maxShake = (float) Math.PI / 24f;
			shakeRate = (float) Math.PI / 100f;
			shakeLeft = Game1.random.NextDouble () < 0.5;
			cropRotation = 0f;
		}

		private void levitateCrop ()
		{
			// For repeating crops, put the repeating plant back without its
			// harvest and switch to drawing the fake harvest item.
			if (cropTile.repeatingCrop)
			{
				poofCrop ("harvest", false);
				if (Context.IsMainPlayer)
					(cropTile as CropTile).restorePlant ();
				cropTexture = cropTile.mapping.harvestTexture;
				cropSourceRect = cropTile.mapping.harvestSourceRect;
				cropOrigin = new Vector2 (8f, 8f);
				cropScale = 2.75f;
				cropPosition += new Vector2 (0f, -32f);
				for (int i = 1; i < baseQuantity; ++i)
				{
					cropOffsets.Add (Game1.random.Next (8, 16) * new Vector2
						(((i + 1) / 2) * ((i % 2 == 1) ? 1 : -1),
						((i + 1) / 2) * ((i % 2 == 1) ? -1 : 1)));
				}
			}

			Game1.playSoundPitched ("toolCharge", Game1.random.Next (8, 12) * 100);
			levitateRate = new Vector2 (0f, cropTile.giantCrop ? -0.5f : -0.25f);
			corruptRate = 0.002f;
		}

		private void updateCrop (object _sender, UpdateTickedEventArgs e)
		{
			// Update rotation for shaking, based on HoeDirt.shake et al.
			if (maxShake > 0f)
			{
				if (shakeLeft)
				{
					cropRotation -= shakeRate;
					if (Math.Abs (cropRotation) >= maxShake)
						shakeLeft = false;
				}
				else
				{
					cropRotation += shakeRate;
					if (cropRotation >= maxShake)
					{
						shakeLeft = true;
						cropRotation -= shakeRate;
					}
				}
				maxShake = Math.Max (0f, maxShake - (float) Math.PI / 300f);
			}
			else
			{
				cropRotation /= 2f;
				if (cropRotation <= 0.01f)
					cropRotation = 0f;
			}

			// Update position for levitation and tint for corruption.
			cropPosition += levitateRate;
			cropTint += corruptRate;
		}

		private void drawCrop (object _sender,
			RenderedWorldEventArgs e)
		{
			Color color = Color.Lerp (Color.White, Color.Orange, cropTint);
			if (Game1.isRaining && location.IsOutdoors)
				color = Color.Lerp (color, Color.LightSlateGray, 0.45f);
			foreach (Vector2 offset in cropOffsets)
			{
				e.SpriteBatch.Draw (cropTexture, Game1.GlobalToLocal
					(Game1.viewport, cropPosition + offset), cropSourceRect,
					color, cropRotation, cropOrigin, cropScale,
					cropFlipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
					cropLayerDepth);
			}
		}

		private void poofCrop (string soundCue, bool big)
		{
			if (!Context.IsMainPlayer)
				return;
			float radius = 32f * (cropTile.giantCrop ? 3f : 1f);
			Vector2 center = cropPosition +
				new Vector2 (-radius + (cropTile.giantCrop ? 64f : 0f), -radius);
			int count = (big ? 12 : 3) * (cropTile.giantCrop ? 6 : 1);
			Color color = Color.Lerp (beasts?.First ()?.primaryColor ?? Color.White,
				Color.Orange, big ? 0f : 0.5f);
			Utilities.MagicPoof (location, center, radius, count,
				color, soundCue);
		}

		private void finishTransformation ()
		{
			// Stop other spawning effects.
			Helper.Events.GameLoop.UpdateTicked -= updateCrop;
			Helper.Events.Display.RenderedWorld -= drawCrop;

			// Release the beasts.
			if (beasts != null)
			{
				foreach (Cropbeast beast in beasts)
					beast.spawn ();
			}
		}
	}
}
