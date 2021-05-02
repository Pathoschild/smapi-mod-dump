/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/SailorStyles
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System.Collections.Generic;

namespace SailorStyles.Editors
{
	internal class NpcManager : IAssetLoader, IAssetEditor
	{
		private IModHelper Helper => ModEntry.Instance.Helper;

		public bool CanLoad<T>(IAssetInfo asset)
		{
			return asset.AssetNameEquals(ModConsts.GameContentCatSchedulePath) 
				|| asset.AssetNameEquals(ModConsts.GameContentCatSpritesPath) 
				|| asset.AssetNameEquals(ModConsts.GameContentCatPortraitPath);
		}

		public T Load<T>(IAssetInfo asset)
		{
			Log.D($"Loaded custom asset ({asset.AssetName})",
				ModEntry.Config.DebugMode);

			if (asset.AssetNameEquals(ModConsts.GameContentCatSchedulePath))
				return (T)(object) Helper.Content.Load
					<Dictionary<string, string>>
					(ModConsts.LocalCatSchedulePath + ".json");
			if (asset.AssetNameEquals(ModConsts.GameContentCatSpritesPath))
				return (T) (object) Helper.Content.Load
					<Texture2D>
					(ModConsts.LocalCatSpritesPath + ".png");
			if (asset.AssetNameEquals(ModConsts.GameContentCatPortraitPath))
				return (T) (object) Helper.Content.Load
					<Texture2D>
					(ModConsts.LocalCatPortraitPath + ".png");
			return (T) (object) null;
		}

		public bool CanEdit<T>(IAssetInfo asset)
		{
			return asset.AssetNameEquals(ModConsts.GameContentAnimationsPath);
		}

		public void Edit<T>(IAssetData asset)
		{
			Log.D($"Edited {(asset.AssetName.StartsWith("assets") ? "custom" : "default")} asset ({asset.AssetName})",
				ModEntry.Config.DebugMode);

			var json = Helper.Content.Load
				<Dictionary<string, string>>
				(ModConsts.LocalAnimationsPath + ".json");
			foreach (var pair in json)
				if (!asset.AsDictionary<string, string>().Data.ContainsKey(pair.Key))
					asset.AsDictionary<string, string>().Data.Add(pair);
		}
	}
}
