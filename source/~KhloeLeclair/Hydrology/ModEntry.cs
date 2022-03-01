/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System.Collections;
using System.Collections.Generic;

using HarmonyLib;

using Leclair.Stardew.Common.Events;
using Leclair.Stardew.Common.Types;

using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;

namespace Leclair.Stardew.Hydrology {
	public class ModEntry : ModSubscriber {

		internal static ModEntry instance;
		internal Harmony Harmony;

		// Textures
		internal Cache<Texture2D, string> OutsideTexture;
		internal Texture2D Objects;

		// Water Tiles
		private Dictionary<string, bool[,]> _WaterMap = new();
		private Dictionary<string, int[,]> _WaterTiles = new();

		public override void Entry(IModHelper helper) {
			base.Entry(helper);
			instance = this;

			Harmony = new Harmony(ModManifest.UniqueID);
			Harmony.PatchAll();

			OutsideTexture = new(key => key == null ? null : Helper.Content.Load<Texture2D>(key, ContentSource.GameContent), () => {
				if (!Context.IsWorldReady)
					return null;

				string season = Game1.GetSeasonForLocation(Game1.currentLocation);
				return $"Maps\\{season}_outdoorsTileSheet";
			});

			Objects = Helper.Content.Load<Texture2D>("assets/Objects");

			Helper.ConsoleCommands.Add("give", "give", (_, __) => {
				Game1.player.addItemToInventory(new WaterFeatureObject() {
					Stack = 999
				});
			});
		}

		#region Events

		[Subscriber]
		private void GameLaunched(object sender, GameLaunchedEventArgs e) {

		}

		[Subscriber]
		private void DayStarted(object sender, DayStartedEventArgs e) {
			OutsideTexture.Invalidate();
		}

		[Subscriber]
		private void ReturnToTitle(object sender, ReturnedToTitleEventArgs e) {
			// This is basically un-loading, so clear the tile cache.
			ClearTiles();
		}

		#endregion

		#region Tile Access

		public void ClearTiles() {
			lock ((_WaterMap as ICollection).SyncRoot)
				_WaterMap.Clear();

			lock ((_WaterTiles as ICollection).SyncRoot)
				_WaterTiles.Clear();
		}

		public void ClearTiles(GameLocation location) {
			string key = location.NameOrUniqueName;

			lock ((_WaterMap as ICollection).SyncRoot)
				_WaterMap.Remove(key);
			lock ((_WaterTiles as ICollection).SyncRoot)
				_WaterTiles.Remove(key);
		}

		public int[,] GetWaterTileMap(GameLocation location, bool create = true) {
			string key = location.NameOrUniqueName;
			lock ((_WaterTiles as ICollection).SyncRoot) {
				_WaterTiles.TryGetValue(key, out int[,] map);
				if (map == null && create) {
					var layer = location.Map.Layers[0];
					map = new int[layer.LayerWidth, layer.LayerHeight];
					_WaterTiles[key] = map;
				}

				return map;
			}
		}

		public bool[,] GetWaterMap(GameLocation location, bool create = true) {
			string key = location.NameOrUniqueName;
			lock ((_WaterMap as ICollection).SyncRoot) {
				_WaterMap.TryGetValue(key, out bool[,] map);
				if (map == null && create) {
					var layer = location.Map.Layers[0];
					map = new bool[layer.LayerWidth, layer.LayerHeight];
					_WaterMap[key] = map;
				}

				return map;
			}
		}

		public bool HasWater(GameLocation location, int x, int y) {
			bool[,] map = GetWaterMap(location, false);
			return map != null && map[x, y];
		}

		public int GetWaterTile(GameLocation location, int x, int y) {
			int[,] map = GetWaterTileMap(location, false);
			return map == null ? 0 : map[x, y];
		}

		public void SetWater(GameLocation location, int x, int y, bool water) {
			GetWaterMap(location)[x, y] = water;
		}

		public void SetWaterTile(GameLocation location, int x, int y, int index) {
			GetWaterTileMap(location)[x, y] = index;
		}

		#endregion

	}
}
