/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/prismaticpride
**
*************************************************/

using StardewModdingAPI;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace PrismaticPride
{
	// This editor adds the clothing items from this mod. Json Assets is not used
	// because the items reuse base game textures (as their prismatic alternatives).
	internal class ClothingEditor : IAssetEditor
	{
		protected static IModHelper Helper => ModEntry.Instance.Helper;
		protected static IMonitor Monitor => ModEntry.Instance.Monitor;

		public bool CanEdit<_T> (IAssetInfo asset)
		{
			return asset.AssetNameEquals ("Data\\ClothingInformation");
		}

		public void Edit<T> (IAssetData asset)
		{
			var data = asset.AsDictionary<int, string> ().Data;
			var newData = LoadData ();
			foreach (int key in newData.Keys)
			{
				data[key] = Regex.Replace (newData[key], @"\{([^}]+)\}",
					(match) => Helper.Translation.Get (match.Groups[1].Value));
			}
		}

		public static List<int> GetAllIDs ()
		{
			return LoadData ().Keys.ToList ();
		}

		private static Dictionary<int, string> LoadData ()
		{
			return Helper.Content.Load<Dictionary<int, string>>
				(Path.Combine ("assets", "clothing.json"));
		}
	}
}
