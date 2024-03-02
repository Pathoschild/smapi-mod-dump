/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Hedgehog-Technologies/StardewMods
**
*************************************************/

using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using xTile.Tiles;
using AllowBeachSprinklers.Helpers;

namespace AllowBeachSprinklers
{
	public class ModEntry : Mod
	{
		private const int _beachFarm = 6;
		private const string _noSprinklersKey = "NoSprinklers";
		private const string _sandLayerName = "Back";

		public override void Entry(IModHelper helper)
		{
			I18n.Init(helper.Translation);

			helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
		}

		private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
		{
			if (Game1.whichFarm == _beachFarm)
			{
				var farm = Game1.getFarm();
				var tileList = farm.Map.GetLayer(_sandLayerName).Tiles.Array.Cast<Tile>().ToList();

				var noSprinklerTiles = tileList
					.Where(tile => tile?.TileIndexProperties != null && tile.TileIndexProperties.ContainsKey(_noSprinklersKey))
					.Distinct(new TileEqualityComparer())
					.OrderBy(tile => tile.TileIndex);

				foreach (var tile in noSprinklerTiles)
				{
					tile.TileIndexProperties[_noSprinklersKey] = false;
				}

				Monitor.Log(I18n.Log_SprinklersAllowed(), LogLevel.Info);
			}
		}
	}
}
