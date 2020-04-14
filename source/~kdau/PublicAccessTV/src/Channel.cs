using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PublicAccessTV
{
	public class Channel
	{
		protected static IModHelper Helper => ModEntry.Instance.Helper;
		protected static IMonitor Monitor => ModEntry.Instance.Monitor;
		protected static ModConfig Config => ModConfig.Instance;

		protected static Type CustomTVMod => ModEntry.CustomTVMod;

		public readonly string localID;
		public readonly string globalID;

		public string title => Helper.Translation.Get ($"{localID}.title");

		private readonly Action<TV, TemporaryAnimatedSprite, Farmer, string> callback;
		private Queue<Scene> scenes = new Queue<Scene> ();

		protected Channel (string localID)
		{
			this.localID = localID ?? throw new ArgumentNullException (nameof (localID));
			globalID = $"kdau.PublicAccessTV.{localID}";
			callback = (tv, _sprite, _who, _response) => show (tv);
			callCustomTVMod ("addChannel", globalID, title, callback);
		}

		// Whether the channel should be available to players at present.
		internal virtual bool isAvailable => true;

		// Add or remove the channel based on its availability for the day.
		internal virtual void update ()
		{
			if (isAvailable)
			{
				callCustomTVMod ("addChannel", globalID, title, callback);
			}
			else
			{
				callCustomTVMod ("removeChannel", globalID);
			}
		}

		// Reset any persistent state for the channel.
		internal virtual void reset ()
		{}

		// Called by CustomTVMod to start the program. Override to implement.
		internal virtual void show (TV tv)
		{
			callCustomTVMod ("showProgram", globalID);
		}

		// Add a scene to the queue for display on TV.
		protected void queueScene (Scene scene)
		{
			scenes.Enqueue (scene);
		}

		// Run a program of all the queued scenes on the TV in order.
		public void runProgram (TV tv)
		{
			if (scenes.Count == 0)
			{
				tv.turnOffTV ();
				Game1.stopMusicTrack (Game1.MusicContext.Event);
			}
			else
			{
				Scene scene = scenes.Dequeue ();
				scene.run (tv, this);
			}
		}

		// Convenience method to handle common values for TV sprites.
		protected TemporaryAnimatedSprite loadSprite (TV tv, string textureName,
			Rectangle sourceRect, float animationInterval = 9999f,
			int animationLength = 1, Vector2 positionOffset = new Vector2 (),
			bool overlay = false, bool? scaleToFit = null, float extraScale = 1f)
		{
			float scale = extraScale *
				((scaleToFit ?? !overlay)
					? Math.Min (42f / sourceRect.Width, 28f / sourceRect.Height)
				: 1f);
			float layerDepth = (float) (((tv.boundingBox.Bottom - 1) / 10000.0) +
				(overlay ? 1.99999994947575E-05 : 9.99999974737875E-06));
			return new TemporaryAnimatedSprite (textureName, sourceRect,
				animationInterval, animationLength, 999999,
				tv.getScreenPosition () + (positionOffset * tv.getScreenSizeModifier ()),
				false, false, layerDepth, 0f, Color.White,
				tv.getScreenSizeModifier () * scale, 0f, 0f, 0f, false);
		}

		// Convenience method for background TV sprites from tilesheets.
		protected TemporaryAnimatedSprite loadBackground (TV tv, int scene,
			int condition = 0)
		{
			return loadSprite (tv,
				Helper.Content.GetActualAssetKey
					(Path.Combine ("assets", $"{localID}_backgrounds.png")),
				new Rectangle (condition * 120, scene * 80, 120, 80));
		}

		// Convenience method for portrait overlay TV sprites.
		protected TemporaryAnimatedSprite loadPortrait (TV tv, string npc,
			int xIndex, int yIndex)
		{
			return loadSprite (tv, $"Portraits\\{npc}",
				new Rectangle (xIndex * 64, yIndex * 64, 64, 64),
				positionOffset: new Vector2 (17.5f, 3.5f), overlay: true,
				scaleToFit: true, extraScale: 0.875f);
		}
		protected TemporaryAnimatedSprite loadPortrait (TV tv, string npc,
			Point? index = null)
		{
			Point _index = index ?? new Point (0, 0);
			return loadPortrait (tv, npc, _index.X, _index.Y);
		}

		protected void callCustomTVMod (string methodName, params object[] arguments)
		{
			callCustomTVModTyped (methodName, arguments,
				arguments.Select (arg => arg.GetType ()).ToArray ());
		}

		private void callCustomTVModTyped (string methodName, object[] arguments, Type[] types)
		{
			MethodInfo method = CustomTVMod.GetMethod (methodName, types)
				?? throw new NotImplementedException ($"CustomTVMod.{methodName} not found.");
			method.Invoke (null, arguments);
		}
	}
}
