/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/flowerbombs
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;

namespace FlowerBombs
{
	internal class EventsEditor : IAssetEditor
	{
		protected static IModHelper Helper => ModEntry.Instance.Helper;
		protected static IMonitor Monitor => ModEntry.Instance.Monitor;
		protected static ModConfig Config => ModConfig.Instance;

		public static readonly int EventID = 79400701;

		public bool CanEdit<_T> (IAssetInfo asset)
		{
			if (!Config.LeahRecipe)
				return false;

			return asset.AssetNameEquals ($"Data\\Events\\FarmHouse");
		}

		public void Edit<_T> (IAssetData asset)
		{
			var data = asset.AsDictionary<string, string> ().Data;

			string leah1 = Helper.Translation.Get ("leahEvent.leah1");
			string leah2 = Helper.Translation.Get ("leahEvent.leah2");
			string leah3 = Helper.Translation.Get ("leahEvent.leah3");
			string message = Helper.Translation.Get ("leahEvent.message");

			string farmerStart = "35 14";
			string leahStart = "38 14";
			if (Context.IsWorldReady)
			{
				FarmHouse fh = Utility.getHomeOfFarmer (Game1.player);
				if (fh != null && fh.upgradeLevel < 2)
				{
					farmerStart = "29 5";
					leahStart = "32 5";
				}
			}

			data[$"{EventID}/f Leah 2500/O Leah/l {MailEditor.RecipeKey}/z winter"] =
				$"continue/-1000 -1000/farmer {farmerStart} 1 Leah {leahStart} 0/viewport -100 -100 true unfreeze/pause 2000/emote Leah 40/pause 500/move farmer 2 0 1/emote farmer 8/faceDirection Leah 3/pause 400/speak Leah \"{leah1}#$b#{leah2}#$b#{leah3}$h\"/pause 200/playSound getNewSpecialItem/addCraftingRecipe Flower Bomb/pause 500/message \"{message}\"/pause 500/end";
		}
	}
}
