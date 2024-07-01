/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/voltaek/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoneyHarvestSync.API
{
	public class HoneyHarvestSyncAPI : IHoneyHarvestSyncAPI
	{
		// The API-accessing mod's `Manifest`
		private IManifest ModManifest { get; set; }

		public HoneyHarvestSyncAPI(IModInfo mod)
		{
			ModManifest = mod.Manifest;

			ModEntry.Logger.Log($"Mod {ModManifest.Name} ({ModManifest.UniqueID} {ModManifest.Version}) fetched API", Constants.buildLogLevel);
		}

		private void LogApiMethodRan(string methodName)
		{
			ModEntry.Logger.Log($"Mod {ModManifest.Name} is running {nameof(HoneyHarvestSyncAPI)}.{methodName}", Constants.buildLogLevel);
		}

		// These have method documentation over in `IHoneyHarvestSyncAPI`

		public string GetBeeHouseReadyIcon()
		{
			return ModEntry.Config.BeeHouseReadyIcon;
		}

		public int GetFlowerRange()
		{
			return ModEntry.Compat.FlowerRange;
		}

		public void RefreshTrackedReadyBeeHouses()
		{
			LogApiMethodRan(nameof(RefreshTrackedReadyBeeHouses));

			HoneyUpdater.RefreshBeeHouseHeldObjects();
		}

		public void RefreshAll()
		{
			LogApiMethodRan(nameof(RefreshAll));

			HoneyUpdater.RefreshAll();
		}
	}
}
