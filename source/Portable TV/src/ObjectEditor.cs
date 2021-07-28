/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/portabletv
**
*************************************************/

using StardewModdingAPI;

namespace PortableTV
{
	internal class ObjectEditor : IAssetEditor
	{
		protected static IModHelper Helper => ModEntry.Instance.Helper;
		protected static IMonitor Monitor => ModEntry.Instance.Monitor;
		protected static ModConfig Config => ModConfig.Instance;

		protected static int ParentSheetIndex => ModEntry.Instance.parentSheetIndex;

		public bool CanEdit<_T> (IAssetInfo asset)
		{
			return ParentSheetIndex != -1 &&
				asset.AssetNameEquals ("Data\\ObjectInformation");
		}

		public void Edit<T> (IAssetData asset)
		{
			// Json Assets doesn't support the equipmentCategory (-29), so
			// switch here from the placeholder category metalResources (-15)
			// used in the JA pack to the real one.
			var data = asset.AsDictionary<int, string> ().Data;
			if (data.ContainsKey (ParentSheetIndex))
			{
				data[ParentSheetIndex] = data[ParentSheetIndex]
					.Replace ("/Basic -15/", "/Basic -29/");
			}
		}
	}
}
