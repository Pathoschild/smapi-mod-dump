using System.IO;

using StardewValley;
using StardewModdingAPI;

using Microsoft.Xna.Framework.Graphics;

using xTile;
using xTile.Dimensions;
using xTile.ObjectModel;
using xTile.Tiles;

namespace SailorStyles_Clothing.Editors
{
	internal class MapEditor : IAssetEditor
	{
		private readonly IModHelper _helper;

		public MapEditor(IModHelper helper)
		{
			_helper = helper;
		}

		public bool CanEdit<T>(IAssetInfo asset)
		{
			return asset.AssetNameEquals(Path.Combine("Maps", Const.LocationTarget));
		}

		public void Edit<T>(IAssetData asset)
		{
			if (Game1.dayOfMonth % 7 <= 1 || ModEntry.Instance.Config.DebugMode)
				PrepareMap((Map)asset.Data);
		}

		private void PrepareMap(Map map)
		{
			AddTilesheet(map);
			AddTiles(map);
		}

		private void AddTilesheet(Map map)
		{
			var path = _helper.Content.GetActualAssetKey(Const.CatTiles + Const.ImgExt);

			var texture = _helper.Content.Load<Texture2D>(path);
			var sheet = new TileSheet(
				Const.CatId, map, path,
				new Size(texture.Width / 16, texture.Height / 16),
				new Size(16, 16));

			map.AddTileSheet(sheet);
			map.LoadTileSheets(Game1.mapDisplayDevice);
		}

		private void AddTiles(Map map)
		{
			const BlendMode mode = BlendMode.Additive;

			var sheet = map.GetTileSheet(Const.CatId);
			var layer = map.GetLayer("Buildings");
			if (layer.Tiles[Const.CatX, Const.CatY] == null)
				layer.Tiles[Const.CatX, Const.CatY] = new StaticTile(layer, sheet, mode, 0);
			layer.Tiles[Const.CatX, Const.CatY].Properties["Action"] = new PropertyValue(Const.CatId);
		}
	}
}
