using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Microsoft.Xna.Framework;
using StardewValley.Locations;
using xTile.Tiles;
using xTile.Layers;
using xTile.ObjectModel;
using System.Collections.Generic;

namespace RopeBridge
{
	/// <summary>The mod entry point.</summary>
	public class ModEntry : Mod
	{
		/*********
		** Fields
		*********/
		//This is the tile property that says a Tile is "Passable" AKA can be walked through
		public PropertyValue propValue = new PropertyValue("Passable");

		//This is the tile list of all the ladders to check if they've been made passable or not
		public SerializableDictionary<Vector2, Tile> ladderList = new SerializableDictionary<Vector2, Tile>();


		/*********
		** Public methods
		*********/
		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper)
		{
			helper.Events.Player.Warped += this.OnWarped;
			helper.Events.World.ObjectListChanged += this.OnObjectListChanged;
		}


		/*********
		** Private methods
		*********/
		/// <summary>Raised after a player warps to a new location.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnWarped(object sender, WarpedEventArgs e)
		{
			// make ladders passable
			if (e.IsLocalPlayer && Game1.currentLocation is MineShaft)
			{
				ladderList = new SerializableDictionary<Vector2, Tile>();

				Layer currentLayer = Game1.currentLocation.map.GetLayer("Buildings");
				for (int yTile = 0; yTile < currentLayer.LayerHeight; yTile++)
				{
					for (int xTile = 0; xTile < currentLayer.LayerWidth; xTile++)
					{
						Tile currentTile = currentLayer.Tiles[xTile, yTile];
						if (currentTile != null && currentTile.TileIndex == 173)
						{
							Game1.currentLocation.map.GetLayer("Buildings").Tiles[xTile, yTile].TileIndexProperties.Add(new KeyValuePair<string, PropertyValue>("Passable", propValue));
						}
					}
				}
			}
		}

		/// <summary>Raised after objects are added or removed in a location.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnObjectListChanged(object sender, ObjectListChangedEventArgs e)
		{
			// detect new ladders
			if (e.IsCurrentLocation && Game1.currentLocation is MineShaft)
			{
				Layer currentLayer = Game1.currentLocation.map.GetLayer("Buildings");
				for (int yTile = 0; yTile < currentLayer.LayerHeight; yTile++)
				{
					for (int xTile = 0; xTile < currentLayer.LayerWidth; xTile++)
					{
						Tile currentTile = currentLayer.Tiles[xTile, yTile];
						if (currentTile != null && currentTile.TileIndex == 173 && !ladderList.ContainsKey(new Vector2(xTile, yTile)))
						{
							ladderList.Add(new Vector2(xTile, yTile), currentTile);
							Game1.currentLocation.map.GetLayer("Buildings").Tiles[xTile, yTile].TileIndexProperties.Add(new KeyValuePair<string, PropertyValue>("Passable", propValue));
						}
					}
				}
			}
		}
	}
}