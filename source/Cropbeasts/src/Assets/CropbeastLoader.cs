using StardewModdingAPI;
using System.IO;

namespace Cropbeasts.Assets
{
	internal class CropbeastLoader : IAssetLoader
	{
		protected static IModHelper Helper => ModEntry.Instance.Helper;
		protected static IMonitor Monitor => ModEntry.Instance.Monitor;
		protected static ModConfig Config => ModConfig.Instance;

		public bool CanLoad<_T> (IAssetInfo asset)
		{
			return asset.AssetNameEquals ("Data\\Cropbeasts");
		}

		public T Load<T> (IAssetInfo asset)
		{
			return Helper.Content.Load<T> (Path.Combine ("assets", "Cropbeasts.json"));
		}
	}
}
