/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeonBlade/CasksAnywhere
**
*************************************************/

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
		    helper.Events.Player.InventoryChanged += OnInventoryChanged;
			//PlayerEvents.InventoryChanged += OnInventoryChanged;
		    helper.Events.GameLoop.Saving += OnBeforeSave;
			//SaveEvents.BeforeSave += OnBeforeSave;
		    helper.Events.GameLoop.Saved += OnAfterSave;
			//SaveEvents.AfterSave += OnAfterSave;
		    helper.Events.GameLoop.SaveLoaded += OnAfterLoad;
			//SaveEvents.AfterLoad += OnAfterLoad;
		}

		void OnPlayerWarped(object sender, WarpedEventArgs e)
		{
			if (!SweepedLocations.Contains(e.NewLocation))
				CaskSweep();
		}

		void OnBeforeSave(object sender, SavingEventArgs e)
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

		void OnAfterSave(object sender, SavedEventArgs e)
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

		void OnAfterLoad(object sender, SaveLoadedEventArgs e)
		{
		    Helper.Events.Player.Warped += OnPlayerWarped;
            //PlayerEvents.Warped += OnPlayerWarped;
		    Helper.Events.World.ObjectListChanged += OnLocationObjectsChanged;
            //LocationEvents.ObjectsChanged += OnLocationObjectsChanged;
		}

		void OnLocationObjectsChanged(object sender, ObjectListChangedEventArgs e)
		{
		    Helper.Events.World.ObjectListChanged -= OnLocationObjectsChanged;
			//LocationEvents.ObjectsChanged -= OnLocationObjectsChanged;
			CaskSweep();
		}


		void OnInventoryChanged(object sender, InventoryChangedEventArgs e)
		{
			foreach (var item in e.Removed)
			{
				// ensure this is a cask
				if (item.Name != "Cask")
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
