/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/flowerbombs
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;
using System;
using SObject = StardewValley.Object;

namespace FlowerBombs
{
	public partial class FlowerBomb
	{
		public override bool isPassable () => true;

		internal static bool HandleButtonPress (ButtonPressedEventArgs e)
		{
			// Only respond to the action button.
			if (!e.Button.IsActionButton ())
				return false;

			// Only respond in an inventory menu.
			InventoryMenu menu = GetActiveInventoryMenu ();
			if (menu == null)
				return false;

			// Determine the inventory item being pointed at.
			Item pointedItem = GetPointedItem (menu, e.Cursor);

			// Respond to clicking on a Flower Bomb.
			if (TryGetLinked (pointedItem, out FlowerBomb bomb))
				return bomb.handleInventoryRightClick ();

			// Make an empty Flower Bomb suitable as ammo for a Slingshot.
			if (pointedItem is Slingshot &&
				TryGetLinked (Game1.player.CursorSlotItem, out FlowerBomb ammoBomb) &&
				ammoBomb.seed == null)
			{
				ammoBomb.Base.Category = -75;
				DelayedAction.functionAfterDelay (() => ammoBomb.Base.Category = -8, 1);
				// The base game action should still occur too.
			}

			return false;
		}

		private bool handleInventoryRightClick ()
		{
			// If interacting empty-handed with a loaded bomb, detach the seed.
			if (seed != null && Game1.player.CursorSlotItem == null)
			{
				Game1.player.CursorSlotItem = detach ();
				return true;
			}

			// If holding an object, that object must be a seed.
			SObject newSeed = Game1.player.CursorSlotItem as SObject;
			if (newSeed?.GetType () != typeof (SObject) ||
					newSeed.Category != SObject.SeedsCategory)
				return false;

			// The seed must be a wild or flower seed.
			Crop testCrop = new (newSeed.ParentSheetIndex, 0, 0);
			if (testCrop.whichForageCrop.Value == 0)
			{
				SObject harvest = new (testCrop.indexOfHarvest.Value, 1);
				if (harvest.Category != SObject.flowersCategory)
					return false;
			}

			// Attach the new seed and detach any old seed.
			SObject oldSeed = detach (playSound: false);
			Game1.player.CursorSlotItem = attach (newSeed);
			if (oldSeed != null)
			{
				if (Game1.player.CursorSlotItem?.canStackWith (oldSeed) ?? false)
					Game1.player.CursorSlotItem.Stack += oldSeed.Stack;
				else if (Game1.player.CursorSlotItem != null)
					Game1.player.addItemToInventory (oldSeed);
				else
					Game1.player.CursorSlotItem = oldSeed;
			}
			return true;
		}

		private static InventoryMenu GetActiveInventoryMenu () =>
			Game1.activeClickableMenu switch
			{
				MenuWithInventory mwi => mwi.inventory,
				GameMenu gm => gm.GetCurrentPage () switch
				{
					CraftingPage cp => cp.inventory,
					InventoryPage ip => ip.inventory,
					_ => null,
				},
				JunimoNoteMenu jnm => jnm.inventory,
				ShopMenu sm => sm.inventory,
				_ => null,
			};

		private static Item GetPointedItem (InventoryMenu menu,
			ICursorPosition cursor)
		{
			var coords = Utility.ModifyCoordinatesForUIScale (cursor.ScreenPixels);
			int x = (int) coords.X;
			int y = (int) coords.Y;

			foreach (ClickableComponent item in menu.inventory)
			{
				if (!item.containsPoint (x, y))
					continue;
				int index = Convert.ToInt32 (item.name);
				if (index < menu.actualInventory.Count &&
						menu.highlightMethod (menu.actualInventory[index]))
					return menu.actualInventory[index];
				else
					return null;
			}
			return null;
		}
	}
}
