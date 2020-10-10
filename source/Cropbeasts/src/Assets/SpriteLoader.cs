/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/cropbeasts
**
*************************************************/

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
