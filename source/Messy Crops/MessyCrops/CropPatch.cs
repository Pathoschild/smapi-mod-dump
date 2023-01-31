/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/MessyCrops
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MessyCrops
{
	[HarmonyPatch(typeof(Crop))]
	internal class CropPatch
	{
		const float pixelDepth = .0001f;

		private static readonly FieldInfo drawpos = typeof(Crop).FieldNamed("drawPosition");
		private static readonly FieldInfo layerdepth = typeof(Crop).FieldNamed("layerDepth");
		internal static readonly Dictionary<Crop, Vector2> offsets = new();

		[HarmonyPatch("updateDrawMath")]
		[HarmonyPostfix]
		public static void AddToList(Crop __instance, Vector2 tileLocation)
		{
			var offset = offsets.GetOrAdd(__instance, GetOffset);
			if (ModEntry.config.ApplyToTrellis || !__instance.raisedSeeds.Value)
			{
				layerdepth.SetValue(__instance, ((tileLocation.Y * 64f + 32f + offset.Y) * pixelDepth + (tileLocation.X % 5) * .00001f) / 
					((__instance.currentPhase.Value == 0 && __instance.shouldDrawDarkWhenWatered()) ? 2f : 1f));
				drawpos.SetValue(__instance, (Vector2)drawpos.GetValue(__instance) + offset);
			}
		}

		public static Vector2 GetOffset(Crop _)
		{
			int amt = ModEntry.config.Amount;
			return new(Game1.random.Next(-amt, amt) * 4, Game1.random.Next(-amt, amt) * 4);
		}
	}
}
