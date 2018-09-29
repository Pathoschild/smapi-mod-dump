using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Microsoft.Xna.Framework;
using System;
using StardewValley.Locations;
using xTile.Tiles;
using xTile.Layers;
using xTile.Dimensions;
using xTile.ObjectModel;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Collections;

namespace RopeBridge
{
	/// <summary>The mod entry point.</summary>
	public class ModEntry : Mod
	{

		//This is the tile property that says a Tile is "Passable" AKA can be walked through
		public PropertyValue propValue = new PropertyValue("Passable");

		//This is the tile list of all the ladders to check if they've been made passable or not
		public SerializableDictionary<Vector2, Tile> ladderList = new SerializableDictionary<Vector2, Tile>();

		/*********
		** Public methods
		*********/
		/// <summary>Initialise the mod.</summary>
		/// <param name="helper">Provides methods for interacting with the mod directory, such as read/writing a config file or custom JSON files.</param>
		public override void Entry(IModHelper helper)
		{
			MineEvents.MineLevelChanged += this.EnteredNewLevel;
			LocationEvents.LocationObjectsChanged += this.CreatedStairs;
		}


		/*********
		** Private methods
		*********/
		/// <summary>The method invoked when the player presses a keyboard button.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void ReceiveKeyPress(object sender, EventArgsKeyPressed e)
		{
			this.Monitor.Log($"Player pressed {e.KeyPressed}.");
		}

		private void EnteredNewLevel(object sender, EventArgsMineLevelChanged e)
		{
			if (Game1.currentLocation is MineShaft)
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

		private void CreatedStairs(object sender, EventArgsLocationObjectsChanged e)
		{
			if (Game1.currentLocation is MineShaft)
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