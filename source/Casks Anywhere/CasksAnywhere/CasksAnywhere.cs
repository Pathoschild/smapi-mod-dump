using System;
using System.Collections.Generic;
using StardewValley;
using StardewValley.Objects;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework;
using System.Linq;

namespace CasksAnywhere
{
	public class CasksAnywhere : Mod
	{
		struct MapPosition
		{
			public GameLocation Location;
			public Vector2 Position;

			public MapPosition(GameLocation location, Vector2 position)
			{
				Location = location;
				Position = position;
			}
		};

		static HashSet<MapPosition> JackedCasks = new HashSet<MapPosition>();
		static HashSet<GameLocation> SweepedLocations = new HashSet<GameLocation>();
		public static IModHelper helper;

		public override void Entry(IModHelper helper)
		{
			CasksAnywhere.helper = helper;
			PlayerEvents.InventoryChanged += OnInventoryChanged;
			SaveEvents.BeforeSave += OnBeforeSave;
			SaveEvents.AfterSave += OnAfterSave;
			SaveEvents.AfterLoad += OnAfterLoad;
		}

		void OnPlayerWarped(object sender, EventArgsPlayerWarped e)
		{
			if (!SweepedLocations.Contains(e.NewLocation))
				CaskSweep();
		}

		void OnBeforeSave(object sender, EventArgs e)
		{
			foreach (var j in JackedCasks)
			{
				if (j.Location.objects.ContainsKey(j.Position))
				{
					var o = j.Location.objects[j.Position];
					if (o is HijackCask)
						j.Location.objects[j.Position] = CaskBack(o as HijackCask);
				}
			}
		}

		void OnAfterSave(object sender, EventArgs e)
		{
			foreach (var j in JackedCasks)
			{
				if (j.Location.objects.ContainsKey(j.Position))
				{
					var o = j.Location.objects[j.Position];
					if (o is Cask)
						j.Location.objects[j.Position] = new HijackCask(o as Cask);
				}
			}
		}

		void OnAfterLoad(object sender, EventArgs e)
		{
			PlayerEvents.Warped += OnPlayerWarped;
			LocationEvents.ObjectsChanged += OnLocationObjectsChanged;
		}

		void OnLocationObjectsChanged(object sender, EventArgsLocationObjectsChanged e)
		{
			LocationEvents.ObjectsChanged -= OnLocationObjectsChanged;
			CaskSweep();
		}


		void OnInventoryChanged(object sender, EventArgsInventoryChanged e)
		{
			foreach (var item in e.Removed)
			{
				// ensure this is a cask
				if (item.Item.Name != "Cask")
					continue;

			foreach (var o in Game1.currentLocation.objects.Pairs)
				if (o.Value is Cask && !(o.Value is HijackCask))
					Hijack(Game1.currentLocation, o.Key);

			}
		}

		private void Hijack(GameLocation location, Vector2 tileLocation)
		{
			// if we're in a menu we likely didn't place a cask down
			if (Game1.activeClickableMenu != null)
				return;
			
			// if our location is valid and we have an object in this location
			if (location != null && Game1.currentLocation.objects.ContainsKey(tileLocation))
			{
				// the item in question
				var item = location.objects[tileLocation];
				// if this is a cask then hijack it!
				if (item != null && item is Cask)
				{
					// turn the cask into a Hijacked Cask
					location.objects[tileLocation] = new HijackCask(item as Cask);
					// add this cask to the jacked casks list
					JackedCasks.Add(new MapPosition(location, tileLocation));
				}
			}
		}

		private Cask CaskBack(HijackCask j)
		{
			// get a cask back
			var cask = new Cask(j.TileLocation);

			// reset all the fields
			cask.heldObject.Value = j.heldObject.Value;
			cask.agingRate.Value = j.agingRate.Value;
			cask.daysToMature.Value = j.daysToMature.Value;
			cask.MinutesUntilReady = j.MinutesUntilReady;

			// return the cask
			return cask;
		}

		private void CaskSweep()
		{
			if (SweepedLocations.Contains(Game1.currentLocation))
				return;
			foreach (var o in Game1.currentLocation.objects.Pairs)
				if (o.Value is Cask)
					Hijack(Game1.currentLocation, o.Key);
			SweepedLocations.Add(Game1.currentLocation);
		}
	}
}
