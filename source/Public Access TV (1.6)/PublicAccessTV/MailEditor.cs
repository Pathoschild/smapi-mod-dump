/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/emurphy42/PredictiveMods
**
*************************************************/

using PredictiveCore;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace PublicAccessTV
{
	internal class MailEditor
	{
		protected static IModHelper Helper => ModEntry.Instance.Helper;
		protected static IMonitor Monitor => ModEntry.Instance.Monitor;

        public void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.Name.IsEquivalentTo("Data\\mail"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, string>().Data;
                    string letter = Helper.Translation.Get("mining.letter.content") +
                        "[#]" + Helper.Translation.Get("mining.letter.title");
                    data["kdau.PublicAccessTV.mining"] = letter;
                });
            }
        }
	}
}
