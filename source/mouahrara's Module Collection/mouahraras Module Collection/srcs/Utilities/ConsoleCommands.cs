/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/mouahrarasModuleCollection
**
*************************************************/

using System.IO;
using StardewModdingAPI;
using StardewValley;

namespace mouahrarasModuleCollection.Utilities
{
	internal class ConsoleCommandsUtility
	{
		internal static void Load()
		{
			ModEntry.Helper.ConsoleCommands.Add("mmc_uninstall", "Uninstall the mouahrara's Module Collection mod", MMC_uninstall);
		}

		private static void MMC_uninstall(string command, string[] args)
		{
			if (!Context.IsWorldReady)
			{
				ModEntry.Monitor.Log("You must load a save to run this command.", LogLevel.Error);
				return;
			}
			if (!Game1.IsMasterGame)
			{
				ModEntry.Monitor.Log("You must be the masterplayer to run this command.", LogLevel.Error);
				return;
			}

			DisableModules();
			ModEntry.Monitor.Log($"To complete the uninstallation, please save the game and delete the {ModEntry.ModManifest.Name} mod from the {Path.Combine(Path.GetDirectoryName(ModEntry.Helper.GetType().Assembly.Location), "Mods")} folder.", LogLevel.Info);
		}

		private static void DisableModules()
		{
			ModEntry.Config.ArcadeGamesPayToPlay = false;
			ModEntry.Config.ArcadeGamesPayToPlayKonamiCode = false;
			ModEntry.Config.ArcadeGamesPayToPlayNonRealisticLeaderboard = false;
			ModEntry.Config.ShopsBetterAnimalPurchase = false;
			ModEntry.Config.ShopsSimultaneousServices = false;
			ModEntry.Config.ShopsGeodesAutoProcess = false;
			ModEntry.Config.MachinesSafeReplacement = false;
			ModEntry.Config.UserInterfaceZoom = false;
			ModEntry.Config.UserInterfaceFastScrolling = false;
			ModEntry.Config.OtherFestivalEndTime = false;
			ModEntry.Monitor.Log("Modules successfully disabled.", LogLevel.Info);
		}
	}
}
