/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Leroymilo/FurnitureFramework
**
*************************************************/

using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace FurnitureFramework
{
	class Collisions
	{
		public static readonly Point tile_game_size = new(64); 

		#region CollisionData

		private class CollisionData
		{
			public readonly bool is_valid = false;
			public readonly string? error_msg;

			public readonly Point size = new();
			Point game_size;

			bool has_tiles = false;
			HashSet<Point> tiles = new();
			HashSet<Point> game_tiles = new();
			// in game coordinates (tile coordinates * 64)

			#region CollisionData Parsing

			public CollisionData(JObject col_object)
			{
				// Parsing required collision box size

				error_msg = "Missing Width in Collision Data.";
				JToken? w_token = col_object.GetValue("Width");
				if (w_token == null || w_token.Type != JTokenType.Integer) return;
				size.X = (int)w_token;

				error_msg = "Missing Height in Collision Data.";
				JToken? h_token = col_object.GetValue("Height");
				if (h_token == null || h_token.Type != JTokenType.Integer) return;
				size.Y = (int)h_token;

				is_valid = true;
				game_size = size * tile_game_size;

				// Parsing optional custom collision map

				JToken? map_token = col_object.GetValue("Map");
				if (map_token != null && map_token.Type == JTokenType.String)
				{
					string? map_string = (string?)map_token;
					if (map_string == null)
					{
						ModEntry.log(
							$"Map at {map_token.Path} must be a string.",
							LogLevel.Warn
						);
						ModEntry.log($"Ignoring Map.", LogLevel.Warn);
						return;
					}
					
					string[] map = map_string.Split('/');
					if (map.Length != size.Y)
					{
						ModEntry.log(
							$"Map at {map_token.Path} must have as many rows as Height : {size.Y}.",
							LogLevel.Warn
						);
						ModEntry.log($"Ignoring Map.", LogLevel.Warn);
						return;
					}

					for (int y = 0; y < size.Y; y++)
					{
						string map_line = map[y];
						if (map_line.Length != size.X)
						{
							ModEntry.log(
								$"All lines of Map at {map_token.Path} must be as long as Height : {size.X}.",
								LogLevel.Warn
							);
							ModEntry.log($"Ignoring Map.", LogLevel.Warn);
							return;
						}

						for (int x = 0; x < size.X; x++)
						{
							if (map[y][x] == 'X')
							{
								Point tile = new(x, y);
								tiles.Add(tile);
								game_tiles.Add(tile * tile_game_size);
							}
						}
					}

					has_tiles = tiles.Count > 0;
				}
			}

			#endregion

			#region CollisionData Methods

			public Rectangle get_bounding_box(Point this_game_pos)
			{
				return new(
					this_game_pos,
					game_size
				);
			}

			public bool is_colliding(Rectangle rect, Point this_game_pos)
			{
				Rectangle bounding_box = get_bounding_box(this_game_pos);

				if (!bounding_box.Intersects(rect))
					return false; // bounding box does not intersect

				if (!has_tiles) return true;	// no custom collision map				
				
				foreach (Point tile_game_pos in game_tiles)
				{
					Rectangle tile_rect = new(
						this_game_pos + tile_game_pos,
						tile_game_size
					);
					if (tile_rect.Intersects(rect))
					{
						return true;	// collision map tile intersects
					}
				}
				return false;	// no collision map tile intersects
			}

			public bool can_be_placed_here(
				Furniture furniture, GameLocation loc, Point tile_pos,
				CollisionMask collisionMask, CollisionMask passable_ignored)
			{
				if (has_tiles)
				{
					foreach (Point tile in tiles)
					{
						if (!is_tile_free(
							furniture,
							tile + tile_pos, loc,
							collisionMask, passable_ignored
						))
						{
							return false;
						}
					}
					return true;
				}

				else
				{
					for (int y = 0; y < size.Y; y++)
					{
						for (int x = 0; x < size.X; x++)
						{
							if (can_place_on_table(new Point(x, y) + tile_pos, loc))
							{
								return true;
							}

							if (!is_tile_free(
								furniture,
								new Point(x, y) + tile_pos, loc,
								collisionMask, passable_ignored
							))
							{
								return false;
							}
						}
					}
					return true;
				}
			}

			private bool is_tile_free(
				Furniture furniture, Point tile, GameLocation loc,
				CollisionMask collisionMask, CollisionMask passable_ignored
			)
			{
				Vector2 v_tile = tile.ToVector2();
				Vector2 center = (v_tile + new Vector2(0.5f)) * 64;

				// checking for general map placeability
				if (!loc.isTilePlaceable(v_tile, furniture.isPassable()))
					return false;

				foreach (Furniture item in loc.furniture)
				{
					// obstructed by non-rug furniture
					if (
						!item.isPassable() &&
						item.GetBoundingBox().Contains(center) &&
						!item.AllowPlacementOnThisTile(tile.X, tile.Y)
					) return false;

					// cannot stack rugs
					if (
						item.isPassable() && furniture.isPassable() &&
						item.GetBoundingBox().Contains(center)
					) return false;
				}

				if (loc.objects.TryGetValue(v_tile, out var value) && value.isPassable() && furniture.isPassable())
					return false;

				if (loc.IsTileOccupiedBy(v_tile, collisionMask, passable_ignored))
					return false;

				if (!furniture.isGroundFurniture())
					return true;
				
				if (
					loc.terrainFeatures.ContainsKey(v_tile) &&
					loc.terrainFeatures[v_tile] is HoeDirt hoeDirt &&
					hoeDirt.crop != null
				) return false;

				return true;
			}

			private bool can_place_on_table(Point tile, GameLocation loc)
			{
				if (size.X > 1 || size.Y > 1)
					return false;

				Rectangle tile_rect = new Rectangle(
					tile * tile_game_size,
					tile_game_size
				);

				foreach (Furniture item in loc.furniture)
				{
					if (!FurniturePack.try_get_type(item, out FurnitureType? _))
					{
						// vanilla furniture
						if (
							item.furniture_type.Value == 11 &&
							item.IntersectsForCollision(tile_rect) &&
							item.heldObject.Value == null
						) return true;
					}
				}

				return false;
			}

			#endregion
		}

		#endregion

		List<CollisionData> collisions = new();

		#region Collisions Parsing

		public Collisions(JToken? token, List<string> rot_names)
		{
			int rot_count = 1;
			bool directional = false;
			if (rot_names.Count > 0)
			{
				rot_count = rot_names.Count;
				directional = true;
			}

			if (token is not JObject collision_obj)
				throw new InvalidDataException("Invalid or missing Collisions.");
			
			CollisionData collision = new(collision_obj);
			if (collision.is_valid)
			{
				// Single collision

				collisions.AddRange(Enumerable.Repeat(collision, rot_count));
			}

			else if (directional)
			{
				foreach ((string key, int rot) in rot_names.Select((value, index) => (value, index)))
				{
					JToken? dir_col_tok = collision_obj.GetValue(key);
					if (dir_col_tok is not JObject dir_col_obj)
					{
						string msg = $"Invalid Collisions or missing directional Collisions for direction {key}";
						throw new InvalidDataException(msg);
					}
					collisions.Add(new(dir_col_obj));
				}
			}
			
			else
			{
				throw new InvalidDataException("Invalid or missing non-directionnal Collisions.");
			}
		}

		#endregion

		#region Collisions Methods

		public Rectangle get_bounding_box(Point this_game_pos, int this_rot = 0)
		{
			return collisions[this_rot].get_bounding_box(this_game_pos);
		}

		public bool is_colliding(Rectangle rect, Point this_game_pos, int this_rot = 0)
		{
			return collisions[this_rot].is_colliding(rect, this_game_pos);
		}

		public bool can_be_placed_here(
			Furniture furniture, GameLocation loc, Point tile_pos,
			CollisionMask collisionMask, CollisionMask passable_ignored
		)
		{
			int rot = furniture.currentRotation.Value;
			return collisions[rot].can_be_placed_here(furniture, loc, tile_pos, collisionMask, passable_ignored);
		}

		public Point get_size(int rot)
		{
			return collisions[rot].size;
		}

		#endregion
	}
}