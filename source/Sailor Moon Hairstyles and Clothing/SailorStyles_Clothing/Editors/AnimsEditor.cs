using System.Collections.Generic;

using StardewModdingAPI;

namespace SailorStyles_Clothing.Editors
{
	internal class AnimsEditor : IAssetEditor
	{
		private readonly IModHelper _helper;

		public AnimsEditor(IModHelper helper)
		{
			_helper = helper;
		}

		public bool CanEdit<T>(IAssetInfo asset)
		{
			return asset.AssetNameEquals(Const.AnimDescs);
		}

		public void Edit<T>(IAssetData asset)
		{
			var json = _helper.Content.Load<Dictionary<string, string>>(
				System.IO.Path.Combine("Assets", Const.CatDir, Const.AnimDescs + Const.JsonExt));
			foreach (var line in json)
				asset.AsDictionary<string, string>().Data.Add(line);
		}
	}
}
