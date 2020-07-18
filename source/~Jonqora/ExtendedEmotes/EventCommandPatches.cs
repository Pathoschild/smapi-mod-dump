using Harmony;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using Netcode;
using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace ExtendedEmotes
{
	class EventCommandPatches
	{
		private static IModHelper Helper => ModEntry.Instance.Helper;
		private static IMonitor Monitor => ModEntry.Instance.Monitor;
		private static HarmonyInstance Harmony => ModEntry.Instance.Harmony;

		protected static ITranslationHelper i18n = Helper.Translation;

		public static void Apply()
		{
			Harmony.Patch(
				original: AccessTools.Method(typeof(Event),
					nameof(Event.command_emote)),
				prefix: new HarmonyMethod(typeof(EventCommandPatches),
					nameof(EventCommandPatches.command_emote_Prefix))
			);
		}

		public static void command_emote_Prefix(ref string[] split)
		{
			try
			{
				// string[] split examples and conversions:

				// VANILLA
				// "emote", "farmer", "20"
				// "emote", "Gus", "12", "true"

				// EXTENDED
				// "emote", "farmer", "20", "false", "rainbow"    =>    "emote", "farmer", "64"
				// "emote", "Gus", "12", "true", "blank"          =>    "emote", "Gus", "68", "true"
				// "emote", "Emily", "rainbow"                    =>    "emote", "Emily", "64"
				// "emote", "Maru", "rainbow", "true"             =>    "emote", "Emily", "64", "true"

				// NEED TO CHECK HOW/IF GAME THROWS ERRORS:
				// "emote", "Emily", "80"       - out of range?
				// "emote", "Emily", "rainbow"  - not an integer?
				// "emote", "Emily", "null"     - is this anything special?

				// LOGIC STEPS
				// if split[3] is not true or false - raise a warning (don't know what it is)
				// if split[2] is not a number AND not a registered emote name, log an error.
				//			   Catch behaviour? Replace with blank emote? Truncate list?
				// if split[2] is a registered emote name, look up the index and replace it.
				// if split[4] is a registered emote name, look up the index and assign it to split[2]
				//     if split[3] is false, truncate to end at split[2]
				//     if split[3] is true, truncate to end at split[3]
			}
			catch (Exception ex)
			{
				Monitor.Log($"Failed in {nameof(command_emote_Prefix)}:\n{ex}",
					LogLevel.Error);
			}
		}
	}
}