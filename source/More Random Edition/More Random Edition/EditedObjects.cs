/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using StardewValley;
using StardewValley.GameData.Crops;
using StardewValley.GameData.Objects;
using System.Collections.Generic;
using System.Linq;

namespace Randomizer
{
    /// <summary>
    /// Contains information required for editing object data
    /// </summary>
    public class EditedObjects
	{
		public static IDictionary<string, ObjectData> DefaultObjectInformation;

        public Dictionary<string, ObjectData> ObjectsReplacements = new();

		/// <summary>
		/// Assumes the CropRandomizer has done its thing
		/// Goes through all the seeds that are randomized and adds their
		/// crop growth info to the crop replacements
		/// </summary>
		public static Dictionary<string, CropData> CropsReplacements
		{
			get
			{
				Dictionary<string, CropData> cropsReplacements = new();
                ItemList.GetSeeds()
					.Cast<SeedItem>()
					.Where(x => x.Randomize)
					.ToList()
					.ForEach(seedItem => 
						cropsReplacements.Add(seedItem.Id.ToString(), seedItem.CropGrowthInfo));

                return cropsReplacements;
			}
		}
		public Dictionary<string, string> FishReplacements = new();

		/// <summary>
		/// Refresh the object data each randomization run in case we have stale data
		/// </summary>
		public EditedObjects()
		{
			DefaultObjectInformation = Game1.objectData;
        }
	}
}
