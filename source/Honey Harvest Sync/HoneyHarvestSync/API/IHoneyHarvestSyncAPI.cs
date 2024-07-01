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
	public interface IHoneyHarvestSyncAPI
	{
		/// <summary>
		/// Returns the config value chosen for the icon type to show above ready-for-harvest bee houses.
		/// </summary>
		/// <returns>The chosen icon type.</returns>
		public string GetBeeHouseReadyIcon();

		/// <summary>
		/// The max tile range that flowers interact with bee houses at being used.
		/// Will either be the vanilla value or another compatible mod's value.
		/// NOTE - This won't be always accurate until after `GameLaunched` since we incorporate values
		/// from other mod's APIs' (when present) into determining this value.
		/// </summary>
		/// <returns>The flower range being used.</returns>
		public int GetFlowerRange();

		/// <summary>
		/// Refresh the "held object" in all tracked, ready-for-harvest bee houses.
		/// This will refresh the icon shown overtop those bee houses.
		/// This can be used in cases where the bee houses should now be showing a different icon above them
		/// due to another mod's config value being changed, which could/would affect the assigned/shown item.
		/// </summary>
		public void RefreshTrackedReadyBeeHouses();

		/// <summary>
		/// This will refresh all tracking - bee houses being tracked as well as their honey flavor sources - across all locations.
		/// This is what runs at the start of each day and should ideally only be run then,
		/// but if everything should be thrown out and re-evaluated for some reason, this will do that.
		/// </summary>
		public void RefreshAll();
	}
}
