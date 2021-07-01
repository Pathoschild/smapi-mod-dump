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

		public static void Apply ()
		{
			Harmony.Patch (
				original: AccessTools.Method (typeof (FarmerRenderer),
					"executeRecolorActions"),
				postfix: new HarmonyMethod (typeof (FarmerRendererPatches),
					nameof (FarmerRendererPatches.executeRecolorActions_Postfix))
			);
		}

#pragma warning disable IDE1006

		public static void executeRecolorActions_Postfix (FarmerRenderer __instance)
		{
			try
			{
				if (Helper.Reflection.GetField<NetInt> (__instance, "shoes").GetValue ().Value
						!= ModEntry.Instance.bootsColorIndex)
					return;

				var baseTexture = Helper.Reflection.GetField<Texture2D> (__instance,
					"baseTexture").GetValue ();
				Color[] pixels = new Color[baseTexture.Width * baseTexture.Height];
				baseTexture.GetData (pixels);

				Color color1 = ColorData.getCurrentColor (asTintOn: 0.13f);
				Color color2 = ColorData.getCurrentColor (asTintOn: 0.57f);
				Color color3 = ColorData.getCurrentColor (asTintOn: 0.86f);
				Color color4 = ColorData.getCurrentColor (asTintOn: 0.95f);

				var _SwapColor = Helper.Reflection.GetMethod (__instance, "_SwapColor");
				_SwapColor.Invoke (__instance.textureName.Value, pixels, 268, color1);
				_SwapColor.Invoke (__instance.textureName.Value, pixels, 269, color2);
				_SwapColor.Invoke (__instance.textureName.Value, pixels, 270, color3);
				_SwapColor.Invoke (__instance.textureName.Value, pixels, 271, color4);

				baseTexture.SetData (pixels);
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
