using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using xTile.Format;

namespace SailorStyles_Clothing.Editors
{
	class CustomNPCLoader : IAssetLoader
	{
		private readonly IModHelper _helper;
		private readonly bool _isDebugging;

		public CustomNPCLoader(IModHelper helper, bool isDebugging)
		{
			_helper = helper;
			_isDebugging = isDebugging;
		}

		public bool CanLoad<T>(IAssetInfo asset)
		{
			return asset.AssetNameEquals(Const.CatSchedule)
		       || asset.AssetNameEquals(Const.CatSprite)
		       || asset.AssetNameEquals(Const.CatPortrait);
		}

		public T Load<T>(IAssetInfo asset)
		{
			if (asset.AssetNameEquals(Const.CatSchedule))
			{
				Log.D($"Loaded schedule data: {asset.AssetName}", _isDebugging);
				var data = _helper.Content.Load<Dictionary<string, string>>(
					Path.Combine(Const.CatDir, Const.CatSchedule + Const.JsonExt));
				foreach (var line in data)
				{
					Log.D($"{line.Key}: {line.Value}", _isDebugging);
				}
				return (T)(object)data;
			}
			if (asset.AssetNameEquals(Const.CatSprite))
			{
				Log.D($"Loaded sprite data: {asset.AssetName}", _isDebugging);
				return (T)(object)_helper.Content.Load<Texture2D>(
					Path.Combine(Const.CatDir, Const.CatSprite + Const.ImgExt));
			}
			if (asset.AssetNameEquals(Const.CatPortrait))
			{
				Log.D($"Loaded portrait data: {asset.AssetName}", _isDebugging);
				return (T)(object)_helper.Content.Load<Texture2D>(
					Path.Combine(Const.CatDir, Const.CatPortrait + Const.ImgExt));
			}
			return (T) (object) null;
		}
	}
}
