/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DeathGameDev/SDV-FastTravel
**
*************************************************/

using System;

namespace FastTravel
{
	[Serializable]
	public class ModConfig
	{
		/// <summary>Whether the game should run in balanced mode. See the mod page for an explanation.</summary>
		public bool BalancedMode { get; set; }
		public bool DebugMode { get; set; }

        /// <summary>A list of locations which can be teleported to.</summary>
        public FastTravelPoint[] FastTravelPoints { get; set; }
	}
}