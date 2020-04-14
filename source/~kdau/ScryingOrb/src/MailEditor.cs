using StardewModdingAPI;

namespace ScryingOrb
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
			string letter = Helper.Translation.Get ("welwickLetter.content") +
				"%item craftingRecipe Scrying_Orb %%[#]" +
				Helper.Translation.Get ("welwickLetter.title");
			data["kdau.ScryingOrb.welwickInstructions"] = letter;
		}
	}
}
