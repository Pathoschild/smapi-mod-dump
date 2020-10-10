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

			// Respond to clicking on a Flower Bomb.
			Item pointedItem = GetPointedItem (menu, e.Cursor);
			if (pointedItem is FlowerBomb bomb)
				return bomb.handleInventoryRightClick ();

			// Make an empty Flower Bomb suitable as ammo for a Slingshot.
			if (pointedItem is Slingshot &&
				Game1.player.CursorSlotItem is FlowerBomb ammo &&
				ammo.heldObject.Value == null)
			{
				ammo.Category = -75;
				DelayedAction.functionAfterDelay (() => ammo.Category = -8, 1);
				// The base game action should still occur too.
			}

			return false;
		}

		private bool handleInventoryRightClick ()
		{
			// If interacting empty-handed with a loaded bomb, detach the seed.
			if (heldObject.Value != null &&
				Game1.player.CursorSlotItem == null)
			{
				Game1.player.CursorSlotItem = detach ();
				return true;
			}

			// If holding an object, that object must be a seed.
			SObject seed = Game1.player.CursorSlotItem as SObject;
			if (seed?.GetType () != typeof (SObject) ||
					seed.Category != SObject.SeedsCategory)
				return false;

			// The seed must be a wild or flower seed.
			Crop testCrop = new Crop (seed.ParentSheetIndex, 0, 0);
			if (testCrop.whichForageCrop.Value == 0)
			{
				SObject harvest = new SObject (testCrop.indexOfHarvest.Value, 1);
				if (harvest.Category != SObject.flowersCategory)
					return false;
			}

			// Attach the new seed and detach any old seed.
			SObject oldSeed = null;
			if (heldObject.Value != null)
				oldSeed = detach (false);
			Game1.player.CursorSlotItem = attach (seed);
			if (oldSeed != null)
				Game1.player.CursorSlotItem = oldSeed;
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
			foreach (ClickableComponent item in menu.inventory)
			{
				if (!item.containsPoint ((int) cursor.ScreenPixels.X,
						(int) cursor.ScreenPixels.Y))
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
