/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/CustomCommunityCentre
**
*************************************************/

using StardewModdingAPI;
using System.Collections.Generic;

namespace CustomCommunityCentre.API
{
	public interface ICustomCommunityCentreAPI
	{
		public void LoadContentPack(string absoluteDirectoryPath);
		public StardewValley.Locations.CommunityCenter GetCommunityCentre();
		public Dictionary<string, int> GetCustomAreaNamesAndNumbers();
		public bool IsCommunityCentreCompleteEarly();
		public bool IsCommunityCentreDefinitelyComplete();
		public bool AreAnyCustomAreasLoaded();
		public bool AreAnyCustomBundlesLoaded();
		public bool AreaAllCustomAreasComplete();
		public IEnumerable<string> GetAllAreaNames();
		public Dictionary<int, int[]> GetAllCustomAreaNumbersAndBundleNumbers();
		public bool IsCustomArea(int areaNumber);
		public bool IsCustomBundle(int bundleNumber);
		public int GetTotalAreasComplete();
		public int GetTotalAreaCount();
		public string GetAreaNameAsAssetKey(string areaName);
	}

	public class CustomCommunityCentreAPI : ICustomCommunityCentreAPI
	{
		private readonly IReflectionHelper Reflection;

		public CustomCommunityCentreAPI(IReflectionHelper reflection)
		{
			this.Reflection = reflection;
		}

		public void LoadContentPack(string absoluteDirectoryPath)
        {
			CustomCommunityCentre.Events.Content.LoadingContentPacks +=
				(object sender, CustomCommunityCentre.Events.Content.LoadingContentPacksEventArgs e) =>
				{
					e.LoadContentPack(absoluteDirectoryPath: absoluteDirectoryPath);
				};
        }

		public StardewValley.Locations.CommunityCenter GetCommunityCentre()
		{
			return Bundles.CC;
		}

		public Dictionary<string, int> GetCustomAreaNamesAndNumbers()
		{
			return Bundles.CustomAreaNamesAndNumbers;
		}

		public bool IsCommunityCentreCompleteEarly()
		{
			return Bundles.IsCommunityCentreCompleteEarly(Bundles.CC);
		}

		public bool IsCommunityCentreDefinitelyComplete()
		{
			return Bundles.IsCommunityCentreDefinitelyComplete(Bundles.CC);
		}

		public bool AreAnyCustomAreasLoaded()
		{
			return Bundles.AreAnyCustomAreasLoaded();
		}

		public bool AreAnyCustomBundlesLoaded()
		{
			return Bundles.AreAnyCustomBundlesLoaded();
		}

		public bool AreaAllCustomAreasComplete()
		{
			return Bundles.AreaAllCustomAreasComplete(Bundles.CC);
		}

		public IEnumerable<string> GetAllAreaNames()
		{
			return Bundles.GetAllAreaNames();
		}

		public string GetAreaNameAsAssetKey(string areaName)
		{
			return Bundles.GetAreaNameAsAssetKey(areaName);
		}

		public Dictionary<int, int[]> GetAllCustomAreaNumbersAndBundleNumbers()
		{
			return Bundles.GetAllCustomAreaNumbersAndBundleNumbers();
		}

		public bool IsCustomArea(int areaNumber)
		{
			return Bundles.IsCustomArea(areaNumber);
		}

		public bool IsCustomBundle(int bundleNumber)
		{
			return Bundles.IsCustomBundle(bundleNumber);
		}

		public int GetTotalAreasComplete()
		{
			return Bundles.TotalAreasCompleteCount;
		}

		public int GetTotalAreaCount()
		{
			return Bundles.TotalAreaCount;
		}
	}
}
