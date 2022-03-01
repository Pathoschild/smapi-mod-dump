/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;

using HarmonyLib;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;

using StardewValley;

namespace Leclair.Stardew.Hydrology.Patches {
	public class GameLocationPatches {

		public static IMonitor Monitor => ModEntry.instance.Monitor;


		private static bool LoadingMap = false;

		[HarmonyPatch(typeof(GameLocation), nameof(GameLocation.loadMap))]
		public static class LoadMap {

			static void Prefix() {
				LoadingMap = true;
			}

			static void Postfix() {
				LoadingMap = false;
			}
		}

		[HarmonyPatch(typeof(GameLocation), nameof(GameLocation.drawWater))]
		public static class DrawWater {
			static void Postfix(GameLocation __instance, SpriteBatch b) {
				try {
					int[,] tiles = ModEntry.instance.GetWaterTileMap(__instance, false);
					bool[,] map = ModEntry.instance.GetWaterMap(__instance, false);
					if (tiles == null || map == null)
						return;

					Texture2D tex = ModEntry.instance.OutsideTexture.Value;
					if (tex == null)
						return;

					int vX = Game1.viewport.X / Game1.tileSize - 1;
					int vY = Game1.viewport.Y / Game1.tileSize - 1;

					int vmX = (Game1.viewport.X + Game1.viewport.Width) / Game1.tileSize + 1;
					int vmY = (Game1.viewport.Y + Game1.viewport.Height) / Game1.tileSize + 2;

					int xMax = Math.Min(tiles.GetLength(0), vmX);
					int yMax = Math.Min(tiles.GetLength(1), vmY);

					for (int y = Math.Max(0, vY); y < yMax; y++) {
						for (int x = Math.Max(0, vX); x < xMax; x++) {
							int sprite = tiles[x, y];
							if (sprite > 0) {
								int key = x * y + x + (y * tiles.GetLength(0));
								int group = (int) (50 * (1 + Math.Sin(100 * key)));

								if (group > 95)
									sprite = WaterFeature.RareShallows[key % WaterFeature.RareShallows.Length];
								else if (group > 70)
									sprite = WaterFeature.CommonShallows[key % WaterFeature.CommonShallows.Length];
								else
									sprite = WaterFeature.PlainShallows[key % WaterFeature.PlainShallows.Length];

								Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2(x * Game1.tileSize, y * Game1.tileSize));
								Rectangle source = Game1.getSourceRectForStandardTileSheet(tex, sprite, 16, 16);

								// Draw Base
								b.Draw(
									tex,
									local,
									source,
									Color.White,
									0f,
									Vector2.Zero,
									4f,
									SpriteEffects.None,
									(float) ((y * 64.0 + 2.0 + x / 10000.0) / 20000.0)
								);

								//b.DrawString(Game1.smallFont, $"{sp}", local, Game1.textColor);
								//b.DrawString(Game1.tinyFont, $"{source.X}", local + new Vector2(0, 20), Color.White);
								//b.DrawString(Game1.tinyFont, $"{source.Y}", local + new Vector2(0, 32), Color.LightGray);

								// Draw Water
								bool overridden = false;
								if (__instance.waterTiles == null) {
									__instance.waterTiles = map;
									overridden = true;
								}

								if (__instance.waterTiles != null && !Game1.oldKBState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.F3))
									__instance.drawWaterTile(b, x, y);

								if (overridden)
									__instance.waterTiles = null;

								// Other drawing is handled elsewhere.
							}
						}
					}
				} catch (Exception ex) {
					Monitor.Log($"Failed in {nameof(DrawWater)}:\n{ex}", LogLevel.Error);
				}
			}
		}

		[HarmonyPatch(typeof(GameLocation), nameof(GameLocation.isCollidingPosition), new Type[] {
			typeof(Rectangle), // position
			typeof(xTile.Dimensions.Rectangle), // viewport
			typeof(bool),
			typeof(int),
			typeof(bool),
			typeof(Character),
			typeof(bool),
			typeof(bool),
			typeof(bool)
		})]
		public static class CollidingPosition {
			static bool Prefix(GameLocation __instance, Rectangle position, xTile.Dimensions.Rectangle viewport, bool isFarmer, int damagesFarmer, bool glider, Character character, bool pathfinding, bool projectile, bool ignoreCharacterRequirement, ref bool __result) {
				try {
					if (!glider) {
						bool[,] map = ModEntry.instance.GetWaterMap(__instance, false);
						if (map != null) {
							int left = position.X / Game1.tileSize;
							int top = position.Y / Game1.tileSize;
							int right = (position.X + position.Width) / Game1.tileSize;
							int bottom = (position.Y + position.Height) / Game1.tileSize;

							bool check(int x, int y) {
								return x < map.GetLength(0) && y < map.GetLength(1) && map[x, y];
							}

							if (check(left, top) || check(left, bottom) || check(right, top) || check(right, bottom)) {
								__result = true;
								return false;
							}
						}
					}

				} catch (Exception ex) {
					Monitor.Log($"Failed in {nameof(CollidingPosition)}:\n{ex}", LogLevel.Error);
				}

				return true;
			}
		}

		[HarmonyPatch(typeof(GameLocation), nameof(GameLocation.isTilePassable), new Type[] {
			typeof(xTile.Dimensions.Location),
			typeof(xTile.Dimensions.Rectangle)
		})]
		public static class TilePassable {
			static bool Prefix(GameLocation __instance, xTile.Dimensions.Location tileLocation, xTile.Dimensions.Rectangle viewport, ref bool __result) {
				try {
					if (!LoadingMap) {
						bool[,] map = ModEntry.instance.GetWaterMap(__instance, false);
						if (map != null && tileLocation.X < map.GetLength(0) && tileLocation.Y < map.GetLength(1) && map[tileLocation.X, tileLocation.Y]) {
							__result = false;
							return false;
						}
					}

				} catch (Exception ex) {
					Monitor.Log($"Failed in {nameof(TilePassable)}:\n{ex}", LogLevel.Error);
				}

				return true;
			}
		}

		[HarmonyPatch(typeof(GameLocation), nameof(GameLocation.isPointPassable))]
		public static class PointPassable {
			static bool Prefix(GameLocation __instance, xTile.Dimensions.Location location, xTile.Dimensions.Rectangle viewport, ref bool __result) {
				try {
					if (!LoadingMap) {
						int x = location.X / 64;
						int y = location.Y / 64;

						bool[,] map = ModEntry.instance.GetWaterMap(__instance, false);
						if (map != null && x < map.GetLength(0) && y < map.GetLength(1) && map[x, y]) {
							__result = false;
							return false;
						}
					}

				} catch (Exception ex) {
					Monitor.Log($"Failed in {nameof(PointPassable)}:\n{ex}", LogLevel.Error);
				}

				return true;
			}
		}

		[HarmonyPatch(typeof(GameLocation), nameof(GameLocation.getTileIndexAt), new Type[] { typeof(Point), typeof(string) })]
		public static class GetTileIndexAtPoint {
			static bool Prefix(GameLocation __instance, Point p, string layer, ref int __result) {
				try {
					if (!LoadingMap) {
						if (layer.Equals("Buildings")) {
							bool[,] map = ModEntry.instance.GetWaterMap(__instance, false);
							if (map != null && p.X < map.GetLength(0) && p.Y < map.GetLength(1) && map[p.X, p.Y]) {
								__result = -1;
								return false;
							}
						}
					}

				} catch (Exception ex) {
					Monitor.Log($"Failed in {nameof(GetTileIndexAtPoint)}:\n{ex}", LogLevel.Error);
				}

				return true;
			}
		}

		[HarmonyPatch(typeof(GameLocation), nameof(GameLocation.getTileIndexAt), new Type[] { typeof(int), typeof(int), typeof(string) })]
		public static class GetTileIndexAt {
			static bool Prefix(GameLocation __instance, int x, int y, string layer, ref int __result) {
				try {
					if (!LoadingMap) {
						if (layer.Equals("Buildings")) {
							bool[,] map = ModEntry.instance.GetWaterMap(__instance, false);
							if (map != null && x < map.GetLength(0) && y < map.GetLength(1) && map[x, y]) {
								__result = -1;
								return false;
							}
						}
					}

				} catch (Exception ex) {
					Monitor.Log($"Failed in {nameof(GetTileIndexAt)}:\n{ex}", LogLevel.Error);
				}

				return true;
			}
		}

		[HarmonyPatch(typeof(GameLocation), nameof(GameLocation.doesTileHaveProperty))]
		public static class DoesTileHaveProperty {
			static bool Prefix(GameLocation __instance, int xTile, int yTile, string propertyName, string layerName, ref string __result) {
				try {
					if (!LoadingMap) {
						if (layerName.Equals("Back") && propertyName.Equals("Water")) {
							bool[,] map = ModEntry.instance.GetWaterMap(__instance, false);
							if (map != null && xTile < map.GetLength(0) && yTile < map.GetLength(1) && map[xTile, yTile]) {
								__result = "t";
								return false;
							}
						}

						if (layerName.Equals("Buildings") && propertyName.Equals("Passable")) {
							bool[,] map = ModEntry.instance.GetWaterMap(__instance, false);
							if (map != null && xTile < map.GetLength(0) && yTile < map.GetLength(1) && map[xTile, yTile]) {
								__result = null;
								return false;
							}
						}
					}

				} catch (Exception ex) {
					Monitor.Log($"Failed in {nameof(DoesTileHaveProperty)}:\n{ex}", LogLevel.Error);
				}

				return true;
			}
		}

		[HarmonyPatch(typeof(GameLocation), nameof(GameLocation.doesTileHavePropertyNoNull))]
		public static class DoesTileHavePropertyNoNull {
			static bool Prefix(GameLocation __instance, int xTile, int yTile, string propertyName, string layerName, ref string __result) {
				try {
					if (!LoadingMap) {
						if (layerName.Equals("Back") && propertyName.Equals("Water")) {
							bool[,] map = ModEntry.instance.GetWaterMap(__instance, false);
							if (map != null && xTile < map.GetLength(0) && yTile < map.GetLength(1) && map[xTile, yTile]) {
								__result = "t";
								return false;
							}
						}

						if (layerName.Equals("Buildings") && propertyName.Equals("Passable")) {
							bool[,] map = ModEntry.instance.GetWaterMap(__instance, false);
							if (map != null && xTile < map.GetLength(0) && yTile < map.GetLength(1) && map[xTile, yTile]) {
								__result = "";
								return false;
							}
						}
					}

				} catch (Exception ex) {
					Monitor.Log($"Failed in {nameof(DoesTileHavePropertyNoNull)}:\n{ex}", LogLevel.Error);
				}

				return true;
			}
		}

		[HarmonyPatch(typeof(GameLocation), nameof(GameLocation.isOpenWater))]
		public static class IsOpenWater {
			static bool Prefix(GameLocation __instance, int xTile, int yTile, ref bool __result) {
				try {
					if (!LoadingMap) {
						// TODO: Determine if the given tile is a border or not.
						bool[,] map = ModEntry.instance.GetWaterMap(__instance, false);
						if (map != null && map[xTile, yTile]) {
							Vector2 pos = new Vector2(xTile, yTile);
							__result = !__instance.objects.ContainsKey(pos);
							return false;
						}
					}

				} catch (Exception ex) {
					Monitor.Log($"Failed in {nameof(IsOpenWater)}:\n{ex}", LogLevel.Error);
				}

				return true;
			}
		}
	}
}
