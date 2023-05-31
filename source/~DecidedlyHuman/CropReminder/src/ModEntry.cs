/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using CropReminder.Utilities;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;


namespace CropReminder
{
	public class ModEntry : Mod
	{
		private IMonitor _monitor;
		private IModHelper _helper;
		private Logger _logger;
		private Dictionary<GameLocation, Dictionary<Vector2, Crop>> _crops;

		public override void Entry(IModHelper helper)
		{
			_monitor = Monitor;
			_helper = helper;
			_logger = new Logger(_monitor);
			_crops = new Dictionary<GameLocation, Dictionary<Vector2, Crop>>();

			 _helper.Events.GameLoop.DayStarted += OnDayStarted;
			//_helper.Events.Input.ButtonPressed += OnButtonPressed;
		}

		private bool IsCropHarvestable(Crop c)
		{
			int harvestablePhase = c.phaseDays.Count - 1;
			
			return (c.currentPhase.Value >= harvestablePhase) && (!c.fullyGrown.Value || c.dayOfCurrentPhase.Value <= 0);
		}

		private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
		{
			if (e.Button == SButton.OemComma)
			{
				foreach (var v in _crops)
				{
					_logger.Log($"Location: {v.Key}");

					foreach (var b in _crops.Values)
					{
						foreach (var c in b)
						{
							_logger.Log($"Tile: {c.Key}");
						}
					}
				}
			}
			
			// if (e.Button == SButton.OemComma)
			// {
			// 	Vector2 cursorTile = Game1.currentCursorTile;
			//
			// 	if (Game1.currentLocation.terrainFeatures.ContainsKey(cursorTile))
			// 	{
			// 		if (Game1.currentLocation.terrainFeatures[cursorTile] is HoeDirt)
			// 		{
			// 			HoeDirt dirt = (HoeDirt)Game1.currentLocation.terrainFeatures[cursorTile];
			//
			// 			if (dirt.crop != null)
			// 			{
			// 				
			// 			}
			// 		}
			// 		else
			// 		{
			// 			_logger.Log("There was no HoeDirt here.");
			// 		}
			// 	}
			// 	else
			// 	{
			// 		_logger.Log("There was no TerrainFeature here.");
			// 	}
			// }
		}

		private void OnDayStarted(object? sender, DayStartedEventArgs e)
		{
			foreach (GameLocation location in Game1.locations)
			{
				foreach (SerializableDictionary<Vector2, TerrainFeature> feature in location.terrainFeatures)
				{
					foreach (TerrainFeature t in feature.Values)
					{
						if (t is HoeDirt)
						{
							HoeDirt dirt = (HoeDirt)t;

							if (dirt.crop != null)
							{
								if (_crops.ContainsKey(location))
									_crops[location].Add(t.currentTileLocation, dirt.crop);
								else
								{
									Dictionary<Vector2, Crop> cropInfo = new Dictionary<Vector2, Crop>();
									cropInfo.Add(t.currentTileLocation, dirt.crop);
									_crops.Add(location, cropInfo);
								}
							}
						}
					}
				}
			}
		}
	}
}