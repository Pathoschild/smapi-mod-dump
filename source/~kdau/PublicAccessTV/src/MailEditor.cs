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

namespace PublicAccessTV
{
	internal class MailEditor : IAssetEditor
	{
		protected static IModHelper Helper => ModEntry.Instance.Helper;
		protected static IMonitor Monitor => ModEntry.Instance.Monitor;

		public bool CanEdit<_T> (IAssetInfo asset)
		{
			return asset.AssetNameEquals ("Data\\mail");
		}

		public void Edit<_T> (IAssetData asset)
		{
			var data = asset.AsDictionary<string, string> ().Data;
			string letter = Helper.Translation.Get ("mining.letter.content") +
				"[#]" + Helper.Translation.Get ("mining.letter.title");
			data["kdau.PublicAccessTV.mining"] = letter;
		}
	}
}
