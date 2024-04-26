/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/emurphy42/PredictiveMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PublicAccessTV
{
	internal class EventsEditor
	{
		protected static IModHelper Helper => ModEntry.Instance.Helper;
		protected static IMonitor Monitor => ModEntry.Instance.Monitor;
		protected static ModConfig Config => ModConfig.Instance;

        public void OnAssetRequested(object sender, AssetRequestedEventArgs e)
		{
            if (Config.BypassFriendships)
            {
                return;
            }

            if (e.Name.IsEquivalentTo($"Data\\Events\\{GarbageChannel.EventMap}"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, string>().Data;
                    applyEvents("garbage", data, GarbageChannel.Events);
                });
            }

            if (e.Name.IsEquivalentTo($"Data\\Events\\{TrainsChannel.EventMap}"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, string>().Data;
                    applyEvents("trains", data, TrainsChannel.Events);
                });
            }
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
