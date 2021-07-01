/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/predictivemods
**
*************************************************/

using StardewModdingAPI;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PublicAccessTV
{
	internal class EventsEditor : IAssetEditor
	{
		protected static IModHelper Helper => ModEntry.Instance.Helper;
		protected static IMonitor Monitor => ModEntry.Instance.Monitor;
		protected static ModConfig Config => ModConfig.Instance;

		public bool CanEdit<_T> (IAssetInfo asset)
		{
			if (Config.BypassFriendships)
				return false;

			return asset.AssetNameEquals ($"Data\\Events\\{GarbageChannel.EventMap}") ||
				asset.AssetNameEquals ($"Data\\Events\\{TrainsChannel.EventMap}");
		}

		public void Edit<_T> (IAssetData asset)
		{
			var data = asset.AsDictionary<string, string> ().Data;

			if (asset.AssetNameEquals ($"Data\\Events\\{GarbageChannel.EventMap}"))
				applyEvents ("garbage", data, GarbageChannel.Events);

			if (asset.AssetNameEquals ($"Data\\Events\\{TrainsChannel.EventMap}"))
				applyEvents ("trains", data, TrainsChannel.Events);
		}

		private void applyEvents (string module, IDictionary<string, string> to,
			IDictionary<string, string> from)
		{
			foreach (string key in from.Keys.ToList ())
			{
				to[key] = from[key] = Regex.Replace (from[key], @"\{\{([^}]+)\}\}",
					(match) => Helper.Translation.Get ($"{module}.event.{match.Groups[1]}"));
			}
		}
	}
}
