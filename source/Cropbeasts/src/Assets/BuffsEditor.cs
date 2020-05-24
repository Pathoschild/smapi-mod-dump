using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System.IO;

namespace Cropbeasts.Assets
{
	internal class BuffsEditor : IAssetEditor
	{
		protected static IModHelper Helper => ModEntry.Instance.Helper;
		protected static IMonitor Monitor => ModEntry.Instance.Monitor;
		protected static ModConfig Config => ModConfig.Instance;

		private Texture2D icon;

		public BuffsEditor ()
		{
			icon = Helper.Content.Load<Texture2D>
				(Path.Combine ("assets", "sandblast-icon.png"));
		}

		public bool CanEdit<_T> (IAssetInfo asset)
		{
			return asset.DataType == typeof (Texture2D) &&
				asset.AssetNameEquals ("TileSheets\\BuffsIcons");
		}

		public void Edit<T> (IAssetData asset)
		{
			IAssetDataForImage editor = asset.AsImage ();
			editor.ExtendImage (16, 64);
			editor.PatchImage (icon, targetArea: new Rectangle (0, 48, 16, 16));
		}
	}
}
