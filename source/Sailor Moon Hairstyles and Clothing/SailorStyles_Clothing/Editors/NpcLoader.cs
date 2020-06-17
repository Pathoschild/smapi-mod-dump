using System.Collections.Generic;
using System.IO;

using StardewModdingAPI;

using Microsoft.Xna.Framework.Graphics;

namespace SailorStyles_Clothing.Editors
{
	internal class NpcLoader : IAssetLoader
	{
		private readonly IModHelper _helper;

		public NpcLoader(IModHelper helper)
		{
			_helper = helper;
		}

		public bool CanLoad<T>(IAssetInfo asset)
		{
			return asset.AssetNameEquals(Const.CatSchedule)
		       || asset.AssetNameEquals(Const.CatSprite)
		       || asset.AssetNameEquals(Const.CatPortrait);
		}

		public T Load<T>(IAssetInfo asset)
		{
			Log.D($"Loaded custom asset ({asset.AssetName})",
				ModEntry.Instance.Config.DebugMode);

			if (asset.AssetNameEquals(Const.CatSchedule))
				return (T)(object)_helper.Content.Load<Dictionary<string, string>>(
					Path.Combine("assets", Const.CatDir, Const.CatSchedule + Const.JsonExt));
			if (asset.AssetNameEquals(Const.CatSprite))
				return (T) (object) _helper.Content.Load<Texture2D>(
					Path.Combine("assets", Const.CatDir, Const.CatSprite + Const.ImgExt));
			if (asset.AssetNameEquals(Const.CatPortrait))
				return (T)(object)_helper.Content.Load<Texture2D>(
					Path.Combine("assets", Const.CatDir, Const.CatPortrait + Const.ImgExt));
			return (T) (object) null;
		}
	}
}
