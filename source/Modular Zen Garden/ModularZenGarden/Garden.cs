/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Leroymilo/MZG
**
*************************************************/

using System.Data;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using StardewValley.Objects;

namespace ModularZenGarden {
	
	class Garden
	{
		public Point position { get; private set; }
		public GardenType type { get; private set; }

		private readonly Dictionary<Point, bool> contacts = new();
		public readonly List<BorderPart> border_parts = new();

		private Garden(Point pos, GardenType g_type)
		{
			position = pos;
			type = g_type;
			
			// building the dictionary of contacts around the Garden
			for (int x = -1; x <= type.size.X; x++)
			{
				contacts[new Point(x, -1)] = false;
				contacts[new Point(x, type.size.Y)] = false;
			}
			for (int y = 0; y < type.size.Y; y++)
			{
				contacts[new Point(-1, y)] = false;
				contacts[new Point(type.size.X, y)] = false;
			}
		}

		static public Garden create<T>(T garden_source) where T : notnull
		{
			Point position = Utils.get_pos(garden_source);
			GardenType type = GardenType.get_type(garden_source);
			return new Garden(position, type);
		}

		public void set_contact(Point tile, bool has_contact)
		{
			if (contacts.ContainsKey(tile))
				contacts[tile] = has_contact;
		}
	
		public void update_texture()
		{
			type.get_border(get_contact_value, border_parts);
			Utils.invalidate_cache(get_skin_id());
		}

		public string get_skin_id()
		{
			return $"MZG_B {type.name}@{position.X}x{position.Y}";
		}

		private int get_contact_value(Point tile)
		{
			int value = 0;
			int p = 0;
			for (int y = -1; y <= 1; y++)
			{
				for (int x = -1; x <= 1; x++)
				{
					Point contact_tile = tile + new Point(x, y);
					if (contacts.ContainsKey(contact_tile))
					{
						if (contacts[contact_tile])
							value += 1 << p;
						p ++;
					}
				}
			}
			return value;
		}
	}

	class GardenComparer : IEqualityComparer<Garden>
	{
		public bool Equals(Garden? g_1, Garden? g_2)
		{
			if (g_1 != null && g_2 != null) 
			{
				return g_1.position == g_2.position
					& g_1.type.Equals(g_2.type);
			}
			return false;	
		}

		public int GetHashCode(Garden garden)
		{
			return garden.position.GetHashCode()
				+ garden.type.GetHashCode();
		} 
	}
}