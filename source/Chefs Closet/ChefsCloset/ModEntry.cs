using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Locations;
using StardewValley.Objects;

namespace ChefsCloset
{
	public class ModEntry : Mod
	{
		public override void Entry(IModHelper helper)
		{
			LocationEvents.CurrentLocationChanged += UpdateLocation;
			MenuEvents.MenuChanged += ExtendCookingItems;
			MenuEvents.MenuClosed += ResolveCookedItems;
		}

		private FarmHouse _farmHouse;
		private Vector2 _kitchenRange = new Vector2(6, 0);
		private List<Item> _fridgeItems = new List<Item>();
		private List<List<Item>> _chestItems = new List<List<Item>>();
		private bool _isCookingSkillLoaded;

		private bool IsCookingMenu(IClickableMenu menu) {
			if (_farmHouse == null || _farmHouse.upgradeLevel == 1) {
				return false;
			}

			if (menu is CraftingPage)
			{
				return true;
			}

			if (_isCookingSkillLoaded && menu.GetType() == Type.GetType("CookingSkill.NewCraftingPage, CookingSkill"))
			{
				return true;
			}

			return false;
		}

		private void ResolveCookedItems(object seneder, EventArgsClickableMenuClosed e) {
			if (IsCookingMenu(e.PriorMenu)) {
				// remove all used items from fridge and reset fridge inventory
				if (_fridgeItems.Any()) { 
					_fridgeItems.RemoveAll(x => x.Stack == 0);
					_farmHouse.fridge.items = _fridgeItems;
				}

				// remove all used items from chests
				if (_chestItems.Any()) { 
					foreach (var obj in _farmHouse.objects)
					{
						Chest chest = obj.Value as Chest;
						if (chest == null || chest == _farmHouse.fridge || obj.Key.X > _kitchenRange.X || obj.Key.Y < _kitchenRange.Y)
							continue;
						
						chest.items = _chestItems.First(x => x == chest.items);

						if (chest.items.Any()) { 
							chest.items.RemoveAll(x => x.Stack == 0);
						}
					}
					
					_chestItems.Clear();
				}
			}
		}

		private void ExtendCookingItems(object sender, EventArgsClickableMenuChanged e)
		{
			if (IsCookingMenu(e.NewMenu))
			{
				_fridgeItems = _farmHouse.fridge.items;
				var cookingItems = new List<Item>();
				var chestKeys = new List<Vector2>();

				// collect chest keys from kitchen tiles
				foreach (var obj in _farmHouse.objects) {
					Chest chest = obj.Value as Chest;
					if (chest == null || chest == _farmHouse.fridge)
						continue;
					if (obj.Key.X > _kitchenRange.X || obj.Key.Y < _kitchenRange.Y)
					{
						// Monitor.Log($"Chest found out of range at {obj.Key.X},{obj.Key.Y}");
						continue;
					}

					chestKeys.Add(obj.Key);
				}

				// order keys to ensure chest items are consumed in the correct order: left-right/top-bottom
				chestKeys = chestKeys.OrderBy(x => x.X).ToList().OrderBy(x => x.Y).ToList();
				chestKeys.Reverse();

				// consolidate cooking items
				foreach (var chestKey in chestKeys)
				{
					StardewValley.Object chest;
					_farmHouse.objects.TryGetValue(chestKey, out chest);

					if (chest != null) {
						// Monitor.Log($"Adding {((Chest)chest).items.Count} items from chest at {chestKey.X},{chestKey.Y}");
						_chestItems.Add(((Chest)chest).items);
						cookingItems.AddRange(((Chest)chest).items);
					}
				}
				cookingItems.AddRange(_fridgeItems);

				// apply cooking items
				_farmHouse.fridge.items = cookingItems;
			}
		}

		// keeps track of location state
		private void UpdateLocation(object sender, EventArgsCurrentLocationChanged e)
		{
			_isCookingSkillLoaded = this.Helper.ModRegistry.IsLoaded("CookingSkill");

			if (e.NewLocation is FarmHouse)
			{
				_farmHouse = (FarmHouse)e.NewLocation;
				if (_farmHouse.upgradeLevel >= 2)
				{
					_kitchenRange.X = 9;
					_kitchenRange.Y = 14;
				}
			}
			else {
				_farmHouse = null;
			}
		}
	}
}
