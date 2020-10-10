/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/cropbeasts
**
*************************************************/

using StardewModdingAPI;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Cropbeasts.Assets
{
	internal class MonsterEditor : IAssetEditor
	{
		protected static IModHelper Helper => ModEntry.Instance.Helper;
		protected static IMonitor Monitor => ModEntry.Instance.Monitor;
		protected static ModConfig Config => ModConfig.Instance;

		public bool CanEdit<_T> (IAssetInfo asset)
		{
			return asset.AssetNameEquals ("Data\\Monsters");
		}

		public void Edit<T> (IAssetData asset)
		{
			var data = asset.AsDictionary<string, string> ().Data;
			var newData = Helper.Content.Load<Dictionary<string, string>>
				(Path.Combine ("assets", "Monsters.json"));
			foreach (string key in newData.Keys)
			{
				string trimmed = Regex.Replace (newData[key], @" */ *", "/");
				data[key] = Regex.Replace (trimmed, @"/([^/]+)$",
					(match) => $"/{Helper.Translation.Get (match.Groups[1].Value)}");
			}
		}

		public static List<string> List ()
		{
			return Helper.Content.Load<Dictionary<string, string>>
				(Path.Combine ("assets", "Monsters.json")).Keys.ToList ();
		}
	}
}
