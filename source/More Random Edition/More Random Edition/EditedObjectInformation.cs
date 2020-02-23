using System.Collections.Generic;

namespace Randomizer
{
	/// <summary>
	/// Contains information required for editing object information
	/// </summary>
	public class EditedObjectInformation
	{
		public Dictionary<int, string> ObjectInformationReplacements = new Dictionary<int, string>();
		public Dictionary<int, string> FruitTreeReplacements = new Dictionary<int, string>();
		public Dictionary<int, string> CropsReplacements
		{
			get
			{
				Dictionary<int, string> cropsReplacements = new Dictionary<int, string>();
				foreach (KeyValuePair<int, CropGrowthInformation> cropInfo in CropGrowthInformation.CropIdsToInfo)
				{
					cropsReplacements.Add(cropInfo.Key, cropInfo.Value.ToString());
				}
				return cropsReplacements;
			}
		}
		public Dictionary<int, string> FishReplacements = new Dictionary<int, string>();
	}
}
