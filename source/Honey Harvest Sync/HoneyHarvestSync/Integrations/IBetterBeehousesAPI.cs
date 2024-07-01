/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/voltaek/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;

namespace HoneyHarvestSync.Integrations
{
	/// <summary>Better Beehouses v2.1.1 API</summary>
	public interface IBetterBeehousesAPI
    {
		/// <returns>The distance bees will search for flowers</returns>
		public int GetSearchRadius();

		/// <returns>The value multiplier for honey from bee houses</returns>
		//public float GetValueMultiplier();

		/// <returns>Whether or not anything can be used as a honey flavor</returns>
		public bool UsingAnythingHoney();

		/// <returns>The GSQ condition for beehouse machine operation, based on config options</returns>
		//public string GetConditionString();

		/// <summary>
		/// Finds nearby honey sources recognized by this mod
		/// </summary>
		/// <param name="where">The location to searh in</param>
		/// <param name="tile">The tile at the center of the search area</param>
		/// <param name="range">The range, in tiles, to search. If omitted, will use the configured range</param>
		/// <param name="predicate">A filter</param>
		/// <returns>A sequence of pairs- the key is the position it was found at, and the value is the ID of the item</returns>
		//public IEnumerable<KeyValuePair<Vector2, string>> GetAllHoneySourcesInRange(GameLocation where, Vector2 tile, int range = -1, Func<Crop, bool> predicate = null);

		/// <summary>
		/// Get honey IDs for a set of tiles. Order of input tiles determines search order.
		/// </summary>
		/// <param name="where">The location to search in</param>
		/// <param name="tiles">The set of tiles to search</param>
		/// <returns>The item IDs of the found items and their tile locations</returns>
		//public IEnumerable<KeyValuePair<Vector2, string>> GetAllHoneySources(GameLocation where, IEnumerable<Vector2> tiles);

		/// <summary>
		/// Get raw honey source data for a set of tiles. Order of input tiles determines search order.
		/// </summary>
		/// <param name="where">The location to search in</param>
		/// <param name="tiles">The set of tiles to search</param>
		/// <returns>The raw data found.</returns>
		//public IEnumerable<object> GetRawHoneySources(GameLocation where, IEnumerable<Vector2> tiles);
    }
}
