/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/portabletv
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Runtime.CompilerServices;

namespace PortableTV
{
	public class PortableTV : TV
	{
		protected static IModHelper Helper => ModEntry.Instance.Helper;
		protected static IMonitor Monitor => ModEntry.Instance.Monitor;
		protected static ModConfig Config => ModConfig.Instance;

		private IReflectedField<TemporaryAnimatedSprite> screenField;
		private IReflectedField<TemporaryAnimatedSprite> screenOverlayField;

		protected TemporaryAnimatedSprite deviceSprite;
		protected TemporaryAnimatedSprite lightSprite;
		protected TemporaryAnimatedSprite staticSprite;

		private bool hasWrappedAfterDialogues;
	
		public PortableTV ()
		{
			screenField = Helper.Reflection.GetField<TemporaryAnimatedSprite>
				(this, "screen");

			screenOverlayField = Helper.Reflection.GetField<TemporaryAnimatedSprite>
				(this, "screenOverlay");
			
			deviceSprite = createSprite ("device.png", 128, 196,
				getDevicePosition ());

			lightSprite = createSprite ("light.png", 8, 2,
				getLightPosition ());

			staticSprite = createSprite ("static.png", 168, 112,
				getScreenPosition (), 8, 200f);
			staticSprite.scale = getScreenSizeModifier () / 4f;
			staticSprite.timeBasedMotion = false;
		}

		public bool isTVOn { get; protected set; }

		public virtual void turnOnTV ()
		{
			if (isTVOn) return;

			// Start rendering the device. Stop the player.
			isTVOn = true;
			Game1.player.Halt ();

			// Ensure other sprites aren't visible yet.
			lightSprite.alpha = 0f;
			staticSprite.alpha = 0f;

			if (Config.Animate)
			{
				// Swoop the device onto the screen if animating.
				deviceSprite.Position = getDeviceOffscreenPosition ();
				deviceSprite.motion =
					new Vector2 (0f, -0.08f * Game1.viewport.Height);
				deviceSprite.acceleration =
					new Vector2 (0f, 0.002f * Game1.viewport.Height);
				deviceSprite.yStopCoordinate = (int) getDevicePosition ().Y;

				// Make a swishing sound.
				Game1.playSound ("swordswipe");
			}
			else
			{
				// If not animating, just show the device.
				deviceSprite.Position = getDevicePosition ();
			}

			DelayedAction.functionAfterDelay (() =>
			{
				// Light up the power button.
				lightSprite.Position = getLightPosition ();
				lightSprite.alpha = 1f;

				// Fade in some static on the screen until the channel is chosen.
				staticSprite.Position = getScreenPosition ();
				if (Config.Static && Config.Animate)
					staticSprite.alphaFade = -0.04f;

				// Play the sound of the TV clicking on and static tuning.
				playCustomSound ("turnOn.wav");

				DelayedAction.functionAfterDelay (() =>
				{
					// Show the channel list. PyTK's CustomTVMod works through
					// a Harmony patch of TV.checkForAction, so this works with
					// or without PyTK.
					checkForAction (Game1.player, false);
				}, 500);
			}, Config.Animate ? 1000 : 100);
		}

		protected static Dictionary<string, string> ChannelMusic
			{ get; private set; } = new Dictionary<string, string>
		{
			{ "Weather", "aerobics" },
			{ "Fortune", "WizardSong" },
			{ "Livin'", "event1" },
			{ "The", "playful" },
		};

		public override void selectChannel (Farmer who, string answer)
		{
			// Turn off the TV properly if "(Leave)" was selected.
			if (answer == "(Leave)")
			{
				turnOffTV ();
				return;
			}

			// Reduce the static to a vague overlay over the background.
			if (Config.Static)
				staticSprite.alpha = 0.05f;

			// Play an appropriate music track for each standard channel.
			if (Config.Music &&
				ChannelMusic.TryGetValue (answer, out string track))
			{
				Game1.changeMusicTrack (track, false, Game1.MusicContext.Event);
			}

			base.selectChannel (who, answer);
		}

		public override void turnOffTV ()
		{
			if (!isTVOn) return;

			// Stop rendering the scene on screen.
			screen = null;
			screenOverlay = null;

			// Stop any music for the TV program.
			Game1.stopMusicTrack (Game1.MusicContext.Event);

			// Turn off the power button.
			lightSprite.alpha = 0f;

			// Reset the static too.
			staticSprite.alpha = 0f;
			staticSprite.alphaFade = 0f;

			// Play the sound of the TV clicking off.
			playCustomSound ("turnOff.wav");

			DelayedAction.functionAfterDelay (() =>
			{
				// If animations aren't configured, cut things short here.
				if (!Config.Animate)
				{
					isTVOn = false;
					return;
				}

				// Swoop the device off the screen.
				deviceSprite.motion = new Vector2 (0f, 1f);
				deviceSprite.acceleration = new Vector2 (0f, 4f);
				deviceSprite.yStopCoordinate =
					(int) getDeviceOffscreenPosition ().Y;

				// Make a swishing sound.
				Game1.playSound ("swordswipe");

				DelayedAction.functionAfterDelay (() =>
				{
					// Stop rendering the device.
					isTVOn = false;
				}, 1000);
			}, 500);
		}

		protected TemporaryAnimatedSprite screen
		{
			get { return screenField.GetValue (); }
			set { screenField.SetValue (value); }
		}

		protected TemporaryAnimatedSprite screenOverlay
		{
			get { return screenOverlayField.GetValue (); }
			set { screenOverlayField.SetValue (value); }
		}

		protected virtual float scaleFactor => 3f;
		protected virtual float screenAdjustFactor => 1.95f;

		public override float getScreenSizeModifier ()
		{
			return scaleFactor * screenAdjustFactor;
		}

		public virtual Vector2 getDevicePosition ()
		{
			return new Vector2 ((Game1.viewport.Width - scaleFactor * 128) * 0.5f,
				(Game1.viewport.Height - scaleFactor * 196) * 0.25f);
		}

		public virtual Vector2 getDeviceOffscreenPosition ()
		{
			return getDevicePosition () + new Vector2 (0f, Game1.viewport.Height);
		}

		public override Vector2 getScreenPosition ()
		{
			return getDevicePosition () + scaleFactor * new Vector2 (23, 26.5f);
		}

		public virtual Vector2 getLightPosition ()
		{
			return getDevicePosition () + scaleFactor * new Vector2 (92, 95);
		}

		public override void draw (SpriteBatch b, int x, int y, float alpha = 1f)
		{
			if (!isTVOn) return;

			// Use this as a convenient point to intercept and wrap CustomTVMod's
			// question response handler.
			if (Game1.activeClickableMenu != null &&
				Game1.currentLocation.afterQuestion != null &&
				!hasWrappedAfterDialogues)
			{
				var originalCallback = Game1.currentLocation.afterQuestion;
				Game1.currentLocation.afterQuestion =
					(Farmer who, string answer) =>
				{
					hasWrappedAfterDialogues = false;
					if (answer == "leave")
						turnOffTV ();
					originalCallback (who, answer);
				};
				hasWrappedAfterDialogues = true;
			}

			// Draw the device itself. Calculate how far off course it landed.
			deviceSprite.update (Game1.currentGameTime);
			deviceSprite.draw (b, false, Game1.viewport.X, Game1.viewport.Y);
			int fudge = (int) (deviceSprite.Position.Y - getDevicePosition ().Y);

			// Draw the light on the power button.
			lightSprite.update (Game1.currentGameTime);
			lightSprite.draw (b, false, Game1.viewport.X,
				Game1.viewport.Y + fudge);

			// Draw the screen elements from the base class.
			if (screen != null)
			{
				screen.update (Game1.currentGameTime);
				screen.draw (b, false, Game1.viewport.X,
					Game1.viewport.Y + fudge);

				if (screenOverlay != null)
				{
					screenOverlay.update (Game1.currentGameTime);
					screenOverlay.draw (b, false, Game1.viewport.X,
						Game1.viewport.Y + fudge);
				}

				// Ensure the static has been tamped down.
				if (staticSprite.alpha > 0.05f)
					staticSprite.alpha = 0.05f;
			}
			
			// Draw the static over the screen.
			if (Config.Static && (Config.Animate || screen != null))
			{
				staticSprite.update (Game1.currentGameTime);
				staticSprite.draw (b, false, Game1.viewport.X,
					Game1.viewport.Y + fudge);
			}
		}

		private TemporaryAnimatedSprite createSprite (string textureFilename,
			int width, int height, Vector2 position, int frames = 1,
			float rate = 9999f)
		{
			string assetKey = Helper.Content.GetActualAssetKey
				(Path.Combine ("assets", textureFilename));
			return new TemporaryAnimatedSprite (assetKey,
				new Rectangle (0, 0, width, height), rate, frames, 999999, position,
				flicker: false, flipped: false) { scale = scaleFactor };
		}

		protected void playCustomSound (string filename)
		{
			var path = Path.Combine (Helper.DirectoryPath, "assets", filename);
			try
			{
				playSoundWithSoundPlayer (path);
			}
			catch (TypeLoadException)
			{
				// best effort, since Android lacks System.Media
			}
		}

		[MethodImplAttribute(MethodImplOptions.NoInlining)] 
		private void playSoundWithSoundPlayer (string path)
		{
			var sound = new SoundPlayer (path);
			sound.Play ();
		}
	}
}
