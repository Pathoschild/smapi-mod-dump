/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/predictivemods
**
*************************************************/

using Harmony;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;

namespace ScryingOrb
{
	internal class SpriteTextPatches
	{
		protected static IModHelper Helper => ModEntry.Instance.Helper;
		protected static IMonitor Monitor => ModEntry.Instance.Monitor;
		protected static HarmonyInstance Harmony => ModEntry.Instance.harmony;

		public static void Apply ()
		{
			if (Constants.TargetPlatform != GamePlatform.Android)
				return;

			Harmony.Patch (
				original: AccessTools.Method (typeof (SpriteText),
					"SetFontPixelZoom"),
				postfix: new HarmonyMethod (typeof (SpriteTextPatches),
					nameof (SpriteTextPatches.FixFontPixelZoom))
			);

			Harmony.Patch (
				original: AccessTools.Method (typeof (SpriteText),
					"setUpCharacterMap"),
				postfix: new HarmonyMethod (typeof (SpriteTextPatches),
					nameof (SpriteTextPatches.FixFontPixelZoom))
			);
		}

#pragma warning disable IDE1006

		public static void FixFontPixelZoom ()
		{
			try
			{
				if (ModEntry.Instance.orbsIlluminated > 0)
				{
					float altZoom = 2.5f;
					if (Game1.activeClickableMenu is DialogueBox db &&
							Helper.Reflection.GetField<bool> (db, "isQuestion").GetValue ())
						altZoom = 2f;
					SpriteText.fontPixelZoom = Math.Min (altZoom,
						SpriteText.fontPixelZoom);
				}
			}
			catch (Exception e)
			{
				Monitor.Log ($"Failed in {nameof (FixFontPixelZoom)}:\n{e}",
					LogLevel.Error);
			}
		}
	}
}
