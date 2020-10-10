/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/pregnancyrole
**
*************************************************/

using StardewModdingAPI;
using System.Collections.Generic;

namespace PregnancyRole
{
	internal class DialogueEditor : IAssetEditor
	{
		protected static IModHelper Helper => ModEntry.Instance.Helper;
		protected static IMonitor Monitor => ModEntry.Instance.Monitor;

		public static readonly List<string> Events =
			new List<string>
		{
			"BirthMessage_PlayerMother",
			"BirthMessage_SpouseMother",
		};

		public static readonly List<string> StringsFromCSFiles =
			new List<string>
		{
			"NPC.cs.4442",
			"NPC.cs.4443",
			"NPC.cs.4445",
			"NPC.cs.4446",
			"NPC.cs.4448",
		};

		public bool CanEdit<_T> (IAssetInfo asset)
		{
			return asset.AssetNameEquals ($"Strings\\Events") ||
				asset.AssetNameEquals ($"Strings\\StringsFromCSFiles");
		}

		public void Edit<_T> (IAssetData asset)
		{
			var data = asset.AsDictionary<string, string> ().Data;

			if (asset.AssetNameEquals ($"Strings\\Events"))
				applyDialogue (Events, data);

			if (asset.AssetNameEquals ($"Strings\\StringsFromCSFiles"))
				applyDialogue (StringsFromCSFiles, data);
		}

		private void applyDialogue (IEnumerable<string> keys,
			IDictionary<string, string> to)
		{
			foreach (string key in keys)
			{
				if (to.ContainsKey (key))
					to[key] = Helper.Translation.Get (key);
			}
		}
	}
}
