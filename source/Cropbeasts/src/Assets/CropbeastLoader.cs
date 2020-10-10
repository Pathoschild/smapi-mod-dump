/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/cropbeasts
**
*************************************************/

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
