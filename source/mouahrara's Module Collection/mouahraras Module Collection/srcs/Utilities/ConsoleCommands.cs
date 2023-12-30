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
using System.Collections.Generic;
using StardewModdingAPI;
using StardewValley;

namespace mouahrarasModuleCollection.Utilities
{
	internal class ConsoleCommandsUtility
	{
		internal static void Load()
		{
			ModEntry.Helper.ConsoleCommands.Add("mmc_uninstall", $"Usage: mmc_uninstall\nUninstall the mouahrara's Module Collection mod", (_, _) => MMC_uninstall());
		}

		private static void MMC_uninstall()
		{
			if (!Context.IsWorldReady)
			{
				ModEntry.Monitor.Log(ModEntry.Helper.Translation.Get("ConsoleCommands.NoSaveLoaded"), LogLevel.Warn);
				return;
			}
			if (!Game1.IsMasterGame)
			{
				ModEntry.Monitor.Log(ModEntry.Helper.Translation.Get("ConsoleCommands.NotMasterplayer"), LogLevel.Warn);
				return;
			}

			DisableModules();
			ModEntry.Monitor.Log(ModEntry.Helper.Translation.Get("ConsoleCommands.CompleteUninstallation", new { ModName = ModEntry.ModManifest.Name, ModsFolder = Path.Combine(Path.GetDirectoryName(ModEntry.Helper.GetType().Assembly.Location), "Mods")}), LogLevel.Info);
		}

		private static void DisableModules()
		{
			ModEntry.Config.ArcadeGamesPayToPlay = false;
			ModEntry.Config.ArcadeGamesPayToPlayKonamiCode = false;
			ModEntry.Config.ArcadeGamesPayToPlayNonRealisticLeaderboard = false;
			ModEntry.Config.ClintsShopSimultaneousServices = false;
			ModEntry.Config.ClintsShopGeodesAutoProcess = false;
			ModEntry.Config.FestivalsEndTime = false;
			ModEntry.Config.FarmViewZoom = false;
			ModEntry.Config.FarmViewFastScrolling = false;
			ModEntry.Config.MarniesShopAnimalPurchase = false;
			ModEntry.Monitor.Log(ModEntry.Helper.Translation.Get("ConsoleCommands.DisableModulesSuccess"), LogLevel.Info);
		}
	}
}
