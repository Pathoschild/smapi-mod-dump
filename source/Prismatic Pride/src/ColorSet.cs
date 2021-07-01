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
using System.Collections.Generic;

namespace PrismaticPride
{
	public class ColorSet
	{
		internal protected static IModHelper Helper => ModEntry.Instance.Helper;
		internal protected static IMonitor Monitor => ModEntry.Instance.Monitor;
		internal protected static ModConfig Config => ModConfig.Instance;

		public string key { get; set; }
		public string displayName => Helper.Translation.Get ($"ColorSet.{key}");

		public List<Color> colors { get; set; }
		public int count => colors.Count;

		public Color getColor (int index, float phase, Color? asTintOn = null)
		{
			int index2 = (index + 1) % count;
			Color result = (Demo.IndexOverride != -1)
				? colors[Demo.IndexOverride]
				: Color.Lerp (colors[index], colors[index2], phase);
			if (asTintOn != null)
			{
				result.R = (byte) (result.R * asTintOn.Value.R / 255);
				result.G = (byte) (result.G * asTintOn.Value.G / 255);
				result.B = (byte) (result.B * asTintOn.Value.B / 255);
			}
			result.A = 255;
			return result;
		}
	}
}
