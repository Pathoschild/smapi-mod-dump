using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System.IO;

namespace Cropbeasts.Assets
{
	internal class SpriteLoader : IAssetLoader
	{
		protected static IModHelper Helper => ModEntry.Instance.Helper;
		protected static IMonitor Monitor => ModEntry.Instance.Monitor;
		protected static ModConfig Config => ModConfig.Instance;

		public bool CanLoad<_T> (IAssetInfo asset)
		{
			if (asset.DataType != typeof (Texture2D))
				return false;
			foreach (string monster in MonsterEditor.List ())
			{
				if (asset.AssetNameEquals ($"Characters\\Monsters\\{monster}"))
					return true;
			}
			return false;
		}

		public T Load<T> (IAssetInfo asset)
		{
			string monster = Path.GetFileNameWithoutExtension (asset.AssetName)
				.Replace (" ", "");
			return Helper.Content.Load<T> (Path.Combine ("assets", "beasts",
				$"{monster}.png"));
		}
	}
}
