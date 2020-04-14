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

		public static void FixFontPixelZoom ()
		{
			try
			{
				if (ModEntry.Instance.OrbsIlluminated > 0)
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
