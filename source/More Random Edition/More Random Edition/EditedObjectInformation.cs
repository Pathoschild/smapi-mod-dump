/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using System.Collections.Generic;

namespace Randomizer
{
	/// <summary>
	/// Contains information required for editing object information
	/// </summary>
	public class EditedObjectInformation
	{
		public Dictionary<int, string> ObjectInformationReplacements = new();
		public Dictionary<int, string> FruitTreeReplacements = new();
		public Dictionary<int, string> CropsReplacements
		{
			get
			{
				Dictionary<int, string> cropsReplacements = new();
				foreach (KeyValuePair<int, CropGrowthInformation> cropInfo in CropGrowthInformation.CropIdsToInfo)
				{
					cropsReplacements.Add(cropInfo.Key, cropInfo.Value.ToString());
				}
				return cropsReplacements;
			}
		}
		public Dictionary<int, string> FishReplacements = new();
	}
}
