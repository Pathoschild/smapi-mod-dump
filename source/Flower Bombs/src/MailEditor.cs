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

namespace FlowerBombs
{
	internal class MailEditor : IAssetEditor
	{
		protected static IModHelper Helper => ModEntry.Instance.Helper;
		protected static IMonitor Monitor => ModEntry.Instance.Monitor;
		protected static ModConfig Config => ModConfig.Instance;

		public static readonly string RecipeKey = "kdau.FlowerBombs.recipe";

		public bool CanEdit<_T> (IAssetInfo asset)
		{
			return asset.AssetNameEquals ("Data\\mail");
		}

		public void Edit<_T> (IAssetData asset)
		{
			var data = asset.AsDictionary<string, string> ().Data;

			// Replace Kent's letterbombs with Flower Bombs if so configured.
			if (Config.KentGifts && FlowerBomb.TileIndex != -1 &&
				data.TryGetValue ("Kent", out string kent))
			{
				data["Kent"] = kent.Replace (" 286 1 287 1 288 1 ",
					$" {FlowerBomb.TileIndex} 1 ");
			}

			// Add Leah's letter with the Flower Bomb recipe if so configured.
			if (Config.LeahRecipe)
			{
				string leah = Helper.Translation.Get ("leahLetter.content") +
					"%item craftingRecipe Flower_Bomb %%[#]" +
					Helper.Translation.Get ("leahLetter.title");
				data[RecipeKey] = leah;
			}
		}

		public static void Invalidate ()
		{
			Helper.Content.InvalidateCache ("Data\\mail");
		}
	}
}
