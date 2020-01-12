using System.IO;

using StardewValley;
using StardewModdingAPI;

using Microsoft.Xna.Framework.Graphics;

using xTile;
using xTile.Dimensions;
using xTile.ObjectModel;
using xTile.Tiles;

/*
 *
 * todo: correct tilesheet indexes for anything in zss_cat, ie. Cate, noren, clothes
 *
 */

namespace SailorStyles_Clothing.Editors
{
	internal class MapEditor : IAssetEditor
	{
		private readonly IModHelper _helper;
		private readonly bool _isDebugging;

		public MapEditor(IModHelper helper, bool isDebugging)
		{
			_helper = helper;
			_isDebugging = isDebugging;
		}

		public bool CanEdit<T>(IAssetInfo asset)
		{
			return asset.AssetNameEquals(Path.Combine("Maps", Const.LocationTarget));
		}

		public void Edit<T>(IAssetData asset)
		{
			if (asset.AssetNameEquals(Path.Combine("Maps", Const.LocationTarget)))
			{
				if (Game1.dayOfMonth % 7 <= 1 || _isDebugging)
				{
					Log.D("Patching map file " + Const.LocationTarget,
						_isDebugging);
					PrepareMap((Map)asset.Data);
				}
			}
		}

		private void PrepareMap(Map map)
		{
			AddTilesheet(map);
			//AddLayers(map);
			AddTiles(map);
		}

		private void AddTilesheet(Map map)
		{
			var path = _helper.Content.GetActualAssetKey(Const.CatTiles + Const.ImgExt);

			var texture = _helper.Content.Load<Texture2D>(path);
			var sheet = new TileSheet(
				Const.CatID, map, path,
				new Size(texture.Width / 16, texture.Height / 16),
				new Size(16, 16));

			map.AddTileSheet(sheet);
			map.LoadTileSheets(Game1.mapDisplayDevice);
		}
		/*
		private void AddLayers(Map map)
		{
			var layer = map.GetLayer("Buildings");
			layer = new Layer(
				Const.ExtraLayerID, map, layer.LayerSize, layer.TileSize);
			layer.Properties.Add("DrawAbove", "Buildings");
			map.AddLayer(layer);
		}
		*/
		private void AddTiles(Map map)
		{
			const BlendMode mode = BlendMode.Additive;

			var sheet = map.GetTileSheet(Const.CatID);
			//var layer = map.GetLayer("Front");
			//var tiles = layer.Tiles;
			/* 
			 * noren
			 * 
			tiles[30, 94] = new StaticTile(layer, sheet, mode, 28);
			tiles[31, 94] = new StaticTile(layer, sheet, mode, 29);
			tiles[32, 94] = new StaticTile(layer, sheet, mode, 30);
			*/

			var layer = map.GetLayer("Buildings");
			if (layer.Tiles[Const.CatX, Const.CatY] == null)
				layer.Tiles[Const.CatX, Const.CatY] = new StaticTile(layer, sheet, mode, 0);
			layer.Tiles[Const.CatX, Const.CatY].Properties.Add("Action", new PropertyValue(Const.CatID));

			/*
			if (layer != null)
			{
				Log.D($"Added layer: {layer.Id}",
					_isDebugging);

				tiles = layer.Tiles;

				if (!ModEntry.Cate)
				{
					
					// kimono
					//tiles[CatX-1, CatY] = new StaticTile(layer, sheet, mode, 31);
					//tiles[CatX-1, CatY+1] = new StaticTile(layer, sheet, mode, 31 + sheet.SheetWidth);
					if (Game1.timeOfDay < 1300)
					{
						tiles[CatX, CatY] = new StaticTile(layer, sheet, mode, 0);
						tiles[CatX, CatY+1] = new StaticTile(layer, sheet, mode, 0 + sheet.SheetWidth);
					}
					else if (Game1.timeOfDay < 2100)
					{
						tiles[CatX, CatY] = new AnimatedTile(layer, new StaticTile[]{
							new StaticTile(layer, sheet, mode, 1),
							new StaticTile(layer, sheet, mode, 2)
							}, 10000);
						tiles[CatX, CatY+1] = new AnimatedTile(layer, new StaticTile[]{
							new StaticTile(layer, sheet, mode, 1 + sheet.SheetWidth),
							new StaticTile(layer, sheet, mode, 2 + sheet.SheetWidth)
							}, 10000);
					}
					else
					{
						tiles[CatX, CatY] = new StaticTile(layer, sheet, mode, 6);
						tiles[CatX, CatY+1] = new StaticTile(layer, sheet, mode, 6 + sheet.SheetWidth);
					}
				}
				else
				{
					// eeeewwsws
					if (Game1.timeOfDay < 2100)
					{
						tiles[CatX, CatY] = new StaticTile(layer, sheet, mode, 15);
						tiles[CatX-1, CatY+1] = new StaticTile(layer, sheet, mode, 14 + sheet.SheetWidth);
						tiles[CatX, CatY+1] = new StaticTile(layer, sheet, mode, 15 + sheet.SheetWidth);
					}
					else
					{
						tiles[CatX, CatY] = new StaticTile(layer, sheet, mode, 17);
						tiles[CatX-1, CatY+1] = new StaticTile(layer, sheet, mode, 16 + sheet.SheetWidth);
						tiles[CatX, CatY+1] = new StaticTile(layer, sheet, mode, 17 + sheet.SheetWidth);
					}
				}

				//layer = map.GetLayer("Buildings");
				//tiles = layer.Tiles;
			}
			else
			{
				Log.E("Failed to add CatShop sprites: Extra map layer couldn't be added.");
				return;
			}
			*/
		}
	}
}
