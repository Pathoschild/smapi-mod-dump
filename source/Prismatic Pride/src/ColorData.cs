/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/prismaticpride
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using System.Collections.Generic;
using System.Linq;

namespace PrismaticPride
{
	public class ColorData
	{
		internal protected static IModHelper Helper => ModEntry.Instance.Helper;
		internal protected static IMonitor Monitor => ModEntry.Instance.Monitor;
		internal protected static ModConfig Config => ModConfig.Instance;

		public Dictionary<string, ColorSet> sets { get; set; }

		public ColorSet initialSet;

		private static string CurrentSetKey =>
			$"{ModEntry.Instance.ModManifest.UniqueID}/currentSet";

		private readonly PerScreen<ColorSet> currentSet_;
		public ColorSet currentSet
		{
			get
			{
				return currentSet_.Value;
			}
			set
			{
				currentSet_.Value = value;
				Game1.player.modData[CurrentSetKey] = value.key;
			}
		}

		ColorData ()
		{
			currentSet_ = new (() => initialSet);
		}

		internal void prepare ()
		{
			foreach (var setPair in sets)
				setPair.Value.key = setPair.Key;

			if (!sets.TryGetValue (Config.DefaultColorSet, out ColorSet initial))
			{
				initial = sets.Values.First ();
				Monitor.Log ($"Could not find default color set '{Config.DefaultColorSet}'; falling back to '{initial.key}'",
					LogLevel.Warn);
			}
			initialSet = initial;
		}

		internal void loadForPlayer ()
		{
			if (Game1.player.modData.TryGetValue (CurrentSetKey, out string key))
			{
				if (sets.TryGetValue (key, out ColorSet set))
					currentSet = set;
			}
		}

		public Color getCurrentColor (int offset = 0, float speedMultiplier = 1)
		{
			double phases = Game1.currentGameTime.TotalGameTime.TotalMilliseconds
				* speedMultiplier / (Config.ColorDuration * 1000.0);
			float phase = (float) (phases % 1.0);
			if (phase < 0.25f)
				phase = 0f;
			else if (phase >= 0.75f)
				phase = 1f;
			else
				phase = 0.5f + 2f * (phase - 0.5f);
			int index = ((int) phases + offset) % currentSet.count;
			return currentSet.getColor (index, phase);
		}

		public static Color Tint (Color baseColor, float tint)
		{
			return Tint (baseColor, new Color (tint, tint, tint));
		}

		public static Color Tint (Color baseColor, Color tint)
		{
			return new Color (
				baseColor.R * tint.R / 255,
				baseColor.G * tint.G / 255,
				baseColor.B * tint.B / 255,
				baseColor.A
			);
		}
	}
}
