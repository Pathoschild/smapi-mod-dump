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
				System.IO.Path.Combine("assets", Const.CatDir, Const.AnimDescs + Const.JsonExt));
			foreach (var line in json)
				if (!asset.AsDictionary<string, string>().Data.ContainsKey(line.Key))
					asset.AsDictionary<string, string>().Data.Add(line);
		}
	}
}
