using System.Collections.Generic;
using System.IO;
using StardewModdingAPI;

namespace SailorStyles_Clothing.Editors
{
	internal class AnimDescEditor : IAssetEditor
	{
		private readonly IModHelper _helper;
		private readonly bool _isDebugging;

		public AnimDescEditor(IModHelper helper, bool isDebugging)
		{
			_helper = helper;
			_isDebugging = isDebugging;
		}

		public bool CanEdit<T>(IAssetInfo asset)
		{
			return asset.AssetNameEquals(Const.AnimDescs);
		}

		public void Edit<T>(IAssetData asset)
		{
			Log.E($"Adding lines to {Const.AnimDescs}: ");

			var json = _helper.Content.Load<Dictionary<string, string>>(
				Path.Combine(Const.CatDir, Const.AnimDescs + Const.JsonExt));
			foreach (var line in json)
			{
				Log.E($"{line.Key}: {line.Value}");
				asset.AsDictionary<string, string>().Data.Add(line);
			}
		}

	}
}
