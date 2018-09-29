using System;
using System.Collections.Generic;

using StardewValley;

namespace StardewNotification
{
	public class ProductionNotification
	{
		/// <summary>
		/// Production notification.
		/// Handles notifications for all production machines like
		/// Bee House, Cheese Press, Keg, etc. 
		/// </summary>

		public void CheckProductionAroundFarm()
		{
			CheckFarmProductions();
			CheckShedProductions();
			CheckGreenhouseProductions();
			CheckCellarProductions();
		}

		public void CheckFarmProductions()
		{
			if (!Util.Config.notifyFarm) return;
			CheckObjectsInLocation(Game1.getLocationFromName(Constants.FARM));
		}

		public void CheckShedProductions()
		{
			if (!Util.Config.notifyShed) return;
			foreach (var location in Game1.locations)
			{
				if (!location.Name.Equals(Constants.FARM)) continue;
				foreach (var building in (location as StardewValley.Locations.BuildableGameLocation).buildings)
				{
					if (!building.nameOfIndoorsWithoutUnique.Equals(Constants.SHED)) continue;
					CheckObjectsInLocation(building.indoors);
				}
			}
		}

		public void CheckGreenhouseProductions()
		{
			if (!Util.Config.notifyGreenhouse) return;
			CheckObjectsInLocation(Game1.getLocationFromName(Constants.GREENHOUSE));
		}

		public void CheckCellarProductions()
		{
			if (!Util.Config.notifyCellar) return;
			CheckObjectsInLocation(Game1.getLocationFromName(Constants.CELLAR));
		}

		private void CheckObjectsInLocation(GameLocation location)
		{
			var counter = new Dictionary<string, Pair<StardewValley.Object, int>>();
			foreach (var pair in location.Objects)
			{
				if (!pair.Value.readyForHarvest) continue;
				if (counter.ContainsKey(pair.Value.Name)) counter[pair.Value.Name].Second++;
				else counter.Add(pair.Value.Name, new Pair<StardewValley.Object, int>(pair.Value, 1));
			}
			foreach (var pair in counter)
				Util.ShowHarvestableMessage(pair);
		}
	}
}
