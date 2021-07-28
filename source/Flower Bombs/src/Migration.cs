/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/flowerbombs
**
*************************************************/

using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.Tools;
using System.Linq;
using System.Text.RegularExpressions;
using SObject = StardewValley.Object;

namespace FlowerBombs
{
	public partial class FlowerBomb
	{
		// In the PyTK style, Flower Bombs were persisted as Chests. Like in
		// other mods, this code is based on Utility.iterateChestsAndStorage,
		// but is missing branches not relevant for this purpose.
		internal static void MigrateV1 ()
		{
			// Consider all players' inventory.
			foreach (var player in Game1.getAllFarmers ())
#pragma warning disable AvoidNetField
				MigrateContents (player.items, $"{player.displayName}'s inventory");
#pragma warning restore AvoidNetField

			// Consider the team's combined Junimo Chest inventory.
			MigrateContents (Game1.player.team.junimoChest, "Junimo Chest contents");

			// Consider locations in the world.
			foreach (var location in Game1.locations)
				MigrateLocation (location, location.Name);
		}

		private static void MigrateLocation (GameLocation location, string context)
		{
			// Consider objects placed in the location.
			foreach (var pair in location.objects.Pairs.ToArray ())
			{
				string context2 = $"at ({(int) pair.Key.X},{(int) pair.Key.Y}) in {context}";
				// Maybe it's actually a Flower Bomb.
				if (TryMigrateItem (pair.Value, $"world {context2}", out SObject bomb))
				{
					bomb.TileLocation = pair.Key;
					location.objects[pair.Key] = bomb;
					continue;
				}

				// Maybe it's a Chest.
				if (pair.Value is Chest chest)
					MigrateContents (chest.items, $"Chest {context2}");
			}

			// Consider Chests placed in storage furniture.
			foreach (var furniture in location.furniture.OfType<StorageFurniture> ())
				MigrateContents (furniture.heldItems, $"{furniture.DisplayName} in {context}");

			// Consider locations with special storage.
			switch (location)
			{
			case FarmHouse fh:
				MigrateContents (fh.fridge.Value.items, $"Refrigerator in {context}");
				break;
			case IslandFarmHouse ifh:
				MigrateContents (ifh.fridge.Value.items, $"Refrigerator in {context}");
				break;
			case BuildableGameLocation bgl:
				foreach (var building in bgl.buildings)
				{
					string context2 = $"at ({building.tileX.Value},{building.tileY.Value}) in {context}";
					if (building.indoors.Value != null)
						MigrateLocation (building.indoors.Value, $"{building.nameOfIndoors} {context2}");
					switch (building)
					{
					case Mill mill:
						MigrateContents (mill.output.Value.items, $"Mill {context2}");
						break;
					case JunimoHut hut:
						MigrateContents (hut.output.Value.items, $"Junimo Hut {context2}");
						break;
					}
				}
				break;
			};
		}

		private static void MigrateContents (NetObjectList<Item> items, string context)
		{
			if (items == null)
				return;

			for (int i = 0; i < items.Count; ++i)
			{
				if (TryMigrateItem (items[i], context, out SObject bomb))
					items[i] = bomb;

				else if (items[i] is Slingshot slingshot &&
						TryMigrateItem (slingshot.attachments[0], $"ammo of {slingshot.DisplayName} in {context}", out SObject bomb2))
					slingshot.attachments[0] = bomb2;
			}
		}

		private static bool TryMigrateItem (Item item, string context, out SObject bomb)
		{
			if (item is not Chest chest)
			{
				bomb = null;
				return false;
			}

			var fields = chest.name.Split ('|').ToList ();
			if (!fields.Contains ("id=kdau.FlowerBombs.FlowerBomb") &&
				!fields.Contains ("id=kdau.FlowerBombs.FlowerBombFull"))
			{
				bomb = null;
				return false;
			}

			var seedField = fields.Find ((field) => Regex.IsMatch (field, @"^seed=\d+$"));
			SObject seed = (seedField != null)
				? new SObject (int.Parse (seedField.Substring (5)), 1)
				: null;

			var stackField = fields.Find ((field) => Regex.IsMatch (field, @"^stack=\d+$"));
			int stack = (stackField != null)
				? int.Parse (stackField.Substring (6))
				: 1;

			Monitor.Log ($"Migrating {stack} Flower Bomb(s) with {seed?.DisplayName ?? "no seeds"} in {context} to new format.", LogLevel.Info);
			bomb = GetNew (seed, stack);
			return true;
		}
	}

	public partial class Wildflower
	{
		internal static void MigrateV1 ()
		{
			uint count = 0;
			foreach (GameLocation location in Game1.locations)
			{
				foreach (var pair in location.objects.Pairs.ToArray ())
				{
					if (pair.Value is ColoredObject co &&
						co.SpecialVariable == 79400700)
					{
						var flower = GetNew (co.ParentSheetIndex, co.color.Value);
						flower.IsSpawnedObject = co.IsSpawnedObject;
						flower.CanBeGrabbed = co.CanBeGrabbed;
						flower.TileLocation = pair.Key;
						location.objects[pair.Key] = flower;
						++count;
					}
				}
			}
			if (count > 0)
				Monitor.Log ($"Migrated {count} Wildflower(s) to new format.", LogLevel.Info);
		}
	}
}
