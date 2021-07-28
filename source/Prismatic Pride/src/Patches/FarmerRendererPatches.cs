/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/prismaticpride
**
*************************************************/

using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;

namespace PrismaticPride
{
	internal static class FarmerRendererPatches
	{
		private static IModHelper Helper => ModEntry.Instance.Helper;
		private static IMonitor Monitor => ModEntry.Instance.Monitor;
		private static ColorData ColorData => ModEntry.Instance.colorData;
		private static HarmonyInstance Harmony => ModEntry.Instance.harmony;

		private static uint Throttle = 5;
		private static readonly PerScreen<uint> Calls = new (() => 0);

		public static void Apply ()
		{
			// These other mods also patch the shoe texture, leading to lag when
			// combined with the Prismatic Boots. Throttle the prismatic effect
			// even more to compensate.
			if (Helper.ModRegistry.IsLoaded ("MartyrPher.GetGlam") ||
					Helper.ModRegistry.IsLoaded ("shaklin.changeshoecolor"))
				Throttle = 50;

			Harmony.Patch (
				original: AccessTools.Method (typeof (FarmerRenderer),
					"executeRecolorActions"),
				postfix: new HarmonyMethod (typeof (FarmerRendererPatches),
					nameof (FarmerRendererPatches.executeRecolorActions_Postfix))
			);
		}

#pragma warning disable IDE1006

		public static void executeRecolorActions_Postfix (FarmerRenderer __instance,
			NetInt ___shoes, Texture2D ___baseTexture)
		{
			try
			{
				// Throttle calls to this patch for performance.
				if (++Calls.Value >= Throttle)
					Calls.Value = 0;
				if (Calls.Value != 0)
					return;

				// Must be wearing Prismatic Boots.
				if (___shoes.Value != ModEntry.Instance.bootsColorIndex)
					return;

				Color[] pixels = new Color[___baseTexture.Width * ___baseTexture.Height];
				___baseTexture.GetData (pixels);

				// This will come from the current set iff ApplyColors is true.
				Color baseColor = Utility.GetPrismaticColor ();

				// Tint the base color for each of the four shoe shades.
				var _SwapColor = Helper.Reflection.GetMethod (__instance, "_SwapColor");
				_SwapColor.Invoke (__instance.textureName.Value, pixels, 268,
					ColorData.Tint (baseColor, 0.13f));
				_SwapColor.Invoke (__instance.textureName.Value, pixels, 269,
					ColorData.Tint (baseColor, 0.57f));
				_SwapColor.Invoke (__instance.textureName.Value, pixels, 270,
					ColorData.Tint (baseColor, 0.86f));
				_SwapColor.Invoke (__instance.textureName.Value, pixels, 271,
					ColorData.Tint (baseColor, 0.95f));

				___baseTexture.SetData (pixels);
			}
			catch (Exception e)
			{
				Monitor.Log ($"Failed in {nameof (executeRecolorActions_Postfix)}:\n{e}",
					LogLevel.Error);
				Monitor.Log (e.StackTrace, LogLevel.Trace);
			}
		}
	}
}
