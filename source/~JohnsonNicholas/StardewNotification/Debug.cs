using System;
using System.Collections.Generic;

using StardewValley;
using StardewModdingAPI;

namespace StardewNotification
{
	public static class Debug
	{
		public static bool DEBUG = false;

		public static void PrintMailForTomorrow()
		{
			if (!DEBUG) return;
			Util.Monitor.Log("Mail For Tomorrow =============================", LogLevel.Info);
			foreach (var mail in Game1.player.mailForTomorrow)
			{
				Util.Monitor.Log(mail, LogLevel.Info);
			}
		}

		public static void PrintMailReceived()
		{
			if (!DEBUG) return;
			Util.Monitor.Log("Mail Received =============================", LogLevel.Info);
			foreach (var mail in Game1.player.mailReceived)
			{
				Util.Monitor.Log(mail, LogLevel.Info);
			}
		}

		public static void PrintPlayerEventsSeen()
		{
			if (!DEBUG) return;
			Util.Monitor.Log("Player Events Seen ========================", LogLevel.Info);
			foreach (var v in Game1.player.eventsSeen)
			{
				Util.Monitor.Log(string.Format("{0}", v), LogLevel.Info);
			}
		}

		public static void PrintLocations()
		{
			if (!DEBUG) return;
			foreach (var location in Game1.locations)
			{
				Util.Monitor.Log(string.Format("Location: {0}", location.Name), LogLevel.Info);
			}
		}

		public static void SetCaskReady()
		{
			if (!DEBUG) return;
			GameLocation location = Game1.getLocationFromName("Cellar");
			foreach (var pair in location.Objects)
				pair.Value.readyForHarvest = true;
		}
	}
}
