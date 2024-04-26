/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Leroymilo/MZG
**
*************************************************/

using System.Security.Principal;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using StardewValley.Objects;

namespace ModularZenGarden {
	
	class GardenCache
	{
		static readonly Dictionary<Point, Garden> gardens = new();

		static readonly HashSet<Garden> to_update = new(new GardenComparer());

		public static readonly Dictionary<string, List<Point>> buildings = new();

		public static void clear()
		{
			gardens.Clear();
			buildings.Clear();
		}

		public static void remove_garden<T>(T garden_source) where T : notnull
		{
			Garden? garden = get_garden(garden_source)
				?? throw new NullReferenceException(
					$"No registered garden matched at {Utils.get_pos(garden_source)}."
				);
			
			gardens.Remove(garden.position);

			HashSet<Garden> neighbors = get_neighbors(garden);

			foreach (Garden garden_2 in neighbors)
			{
				Point delta = garden_2.position - garden.position;
				garden.type.set_contacts(garden_2, Point.Zero - delta, false);
			}

			to_update.UnionWith(neighbors);
			to_update.Remove(garden);

			if (garden_source is Building)
			{
				string type = ((Building)(object)garden_source).buildingType.Value;
				if (buildings.ContainsKey(type))
				{
					buildings[type].Remove(garden.position);
				}
			}
		}

		public static Garden add_garden<T>(T garden_source) where T : notnull
		{
			Garden garden = Garden.create(garden_source);

			HashSet<Garden> neighbors = get_neighbors(garden);

			foreach (Garden garden_2 in neighbors)
			{
				Point delta = garden_2.position - garden.position;
				garden_2.type.set_contacts(garden, delta, true);
				garden.type.set_contacts(garden_2, Point.Zero - delta, true);
			}

			gardens[garden.position] = garden;
			
			to_update.UnionWith(neighbors);
			to_update.Add(garden);

			if (garden_source is Building)
			{
				string type = ((Building)(object)garden_source).buildingType.Value;
				if (!buildings.ContainsKey(type))
				{
					buildings[type] = new();
				}
				buildings[type].Add(garden.position);
			}

			return garden;
		}

		public static void update_textures()
		{
			foreach (Garden garden in to_update)
			{
				garden.update_texture();
			}

			to_update.Clear();
		}

		public static Garden? get_garden<T>(T garden_source) where T : notnull
		{
			GardenType type = GardenType.get_type(garden_source);
			Point base_pos = Utils.get_pos(garden_source);
			
			return get_garden(type, base_pos);
		}

		public static Garden? get_garden(GardenType type, Point base_pos)
		{
			// Removing a garden from a tile that's not its top left tile will move it
			// To find it anyway, its necessary to search around the position
			for (int x = 0; x < type.size.X; x++)
			{
				for (int y = 0; y < type.size.Y; y++)
				{
					Point rel_pos = base_pos - new Point(x, y);
					if (gardens.ContainsKey(rel_pos))
					{
						if (gardens[rel_pos].type == type) {
							return gardens[rel_pos];
						}
					}
				}
			}
			
			return null;
		}

		private static HashSet<Garden> get_neighbors(Garden garden)
		{
			HashSet<Garden> neighbors = new( new GardenComparer() );

			foreach (Garden garden_2 in gardens.Values)
			{
				Point delta = garden_2.position - garden.position;
				if (delta.X < -garden_2.type.size.X || delta.X > garden.type.size.X) continue;
				if (delta.Y < -garden_2.type.size.Y || delta.Y > garden.type.size.Y) continue;
				neighbors.Add(garden_2);
			}

			return neighbors;
		}
	
		public static void invalidate_buildings()
		{
			
		}
	}

}