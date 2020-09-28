using System.Collections.Generic;
using System.IO;

using StardewModdingAPI;

using Microsoft.Xna.Framework.Graphics;

namespace SailorStyles.Editors
{
	internal class NpcLoader : IAssetLoader, IAssetEditor
	{
		private readonly IModHelper _helper;

		public NpcLoader(IModHelper helper)
		{
			_helper = helper;
		}

		public bool CanLoad<T>(IAssetInfo asset)
		{
			return asset.AssetNameEquals(ModConsts.CatSchedule) 
				|| asset.AssetNameEquals(ModConsts.CatSpritesheet) 
				|| asset.AssetNameEquals(ModConsts.CatPortrait);
		}

		public T Load<T>(IAssetInfo asset)
		{
			Log.D($"Loaded custom asset ({asset.AssetName})",
				ModEntry.Instance.Config.DebugMode);

			if (asset.AssetNameEquals(ModConsts.CatSchedule))
				return (T)(object)_helper.Content.Load<Dictionary<string, string>>(
					Path.Combine("assets", ModConsts.CatDir, $"{ModConsts.CatSchedule}.json"));
			if (asset.AssetNameEquals(ModConsts.CatSpritesheet))
				return (T) (object) _helper.Content.Load<Texture2D>(
					Path.Combine("assets", ModConsts.CatDir, $"{ModConsts.CatSpritesheet}.png"));
			if (asset.AssetNameEquals(ModConsts.CatPortrait))
				return (T)(object)_helper.Content.Load<Texture2D>(
					Path.Combine("assets", ModConsts.CatDir, $"{ModConsts.CatPortrait}.png"));
			return (T) (object) null;
		}

		public bool CanEdit<T>(IAssetInfo asset)
		{
			return asset.AssetNameEquals(ModConsts.AnimDescs);
		}

		public void Edit<T>(IAssetData asset)
		{
			Log.D($"Edited {(asset.AssetName.StartsWith("assets") ? "custom" : "default")} asset ({asset.AssetName})",
				ModEntry.Instance.Config.DebugMode);

			var json = _helper.Content.Load<Dictionary<string, string>>(
				Path.Combine("assets", ModConsts.CatDir, $"{ModConsts.AnimDescs}.json"));
			foreach (var line in json)
				if (!asset.AsDictionary<string, string>().Data.ContainsKey(line.Key))
					asset.AsDictionary<string, string>().Data.Add(line);
		}
	}
}
