/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/emurphy42/PredictiveMods
**
*************************************************/

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
	public abstract class Channel
	{
		protected static IModHelper Helper => ModEntry.Instance.Helper;
		protected static IMonitor Monitor => ModEntry.Instance.Monitor;
		protected static ModConfig Config => ModConfig.Instance;

		public readonly string localID;
		public readonly string globalID;

		public string title => Helper.Translation.Get ($"{localID}.title");

		private readonly Queue<Scene> scenes = new ();

		protected Channel (string localID)
		{
			this.localID = localID ?? throw new ArgumentNullException (nameof (localID));
			globalID = $"kdau.PublicAccessTV.{localID}";
		}

		// Whether the channel should be available to players at present.
		internal abstract bool isAvailable { get; }

		// Perform any updates needed for a new day.
		internal virtual void update ()
		{ }

		// Reset any persistent state for the channel.
		internal virtual void reset ()
		{ }

		// Implement to perform the program.
		internal abstract void show (TV tv);

		// Add a scene to the queue for display on TV.
		protected void queueScene (Scene scene)
		{
			scenes.Enqueue (scene);
		}

		// Run the next scene in the queue.
		public void runNextScene (TV tv)
		{
			if (scenes.Count == 0)
			{
				tv.turnOffTV ();
				Game1.stopMusicTrack (StardewValley.GameData.MusicContext.Event);
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
				Helper.ModContent.GetInternalAssetName
					(Path.Combine ("assets", $"{localID}_backgrounds.png")).Name,
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
	}
}
