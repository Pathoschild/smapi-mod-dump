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
using StardewValley;
using StardewValley.Objects;
using System.Collections.Generic;

namespace PrismaticPride
{
	internal static class Demo
	{
		// private static IModHelper Helper => ModEntry.Instance.Helper;
		// private static IMonitor Monitor => ModEntry.Instance.Monitor;
		// private static ModConfig Config => ModConfig.Instance;

		public static void Demonstrate ()
		{
			if (++IndexOverride >= ModEntry.Instance.colorData.currentSet.count)
				IndexOverride = -1;
			Game1.player.changeHairStyle (Game1.random.Next (32));
			Game1.player.changeHairColor (GetRandomColor ());
			Game1.player.changeEyeColor (GetRandomColor ());
			Game1.player.changeSkinColor (Game1.random.Next (6));
			Game1.player.shirtItem.Value = new Clothing (Utility.GetRandom (new List<int>
				{ 1223, 1998, 1999, 1997, 79400800, 79400801, 79400802, 79400803, 79400804, 79400805, 79400806, 79400807, 79400808, 79400809 }));
			Game1.player.pantsItem.Value = new Clothing (Utility.GetRandom (new List<int>
				{ 998, 999, 1993, 1994, 1995, 1996, 79400810, 79400811, 79400812, 79400813, 79400814 }));
			Game1.player.boots.Value = new Boots (ModEntry.Instance.bootsSheetIndex);
		}

		public static void Undemonstrate ()
		{
			IndexOverride = -1;
		}

		public static int IndexOverride { get; private set; } = -1;

		private static Color GetRandomColor ()
		{
			Color color = new (Game1.random.Next (25, 254),
				Game1.random.Next (25, 254), Game1.random.Next (25, 254));

			if (Game1.random.NextDouble () < 0.5)
			{
				color.R /= 2;
				color.G /= 2;
				color.B /= 2;
			}

			if (Game1.random.NextDouble () < 0.5)
				color.R = (byte) Game1.random.Next (15, 50);
			if (Game1.random.NextDouble () < 0.5)
				color.G = (byte) Game1.random.Next (15, 50);
			if (Game1.random.NextDouble () < 0.5)
				color.B = (byte) Game1.random.Next (15, 50);

			return color;
		}
	}
}
