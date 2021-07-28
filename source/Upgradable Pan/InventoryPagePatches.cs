/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AHilyard/UpgradeablePan
**
*************************************************/

using StardewValley;
using StardewValley.Tools;
using StardewValley.Menus;
using StardewValley.Objects;
using Microsoft.Xna.Framework.Input;

namespace UpgradablePan
{
	public class InventoryPagePatches
	{
		public static bool receiveLeftClick_Prefix(ref InventoryPage __instance, int x, int y, bool playSound)
		{
			bool performedSpecialFunctionality = false;
			bool heldItemWasNull = Game1.player.CursorSlotItem == null;
			foreach (ClickableComponent c in __instance.equipmentIcons)
			{
				if (c.containsPoint(x, y))
				{
					if (c.name == "Hat" && !performedSpecialFunctionality)
					{
						// Unequipping hat...
						if ((Game1.player.CursorSlotItem == null && (Game1.player.hat.Value != null && (Game1.player.hat.Value.which == 71 || Game1.player.hat.Value is PanHat))))
						{
							Item heldItem = Utility.PerformSpecialItemGrabReplacement(Game1.player.hat.Value);
							if (heldItem != null)
							{
								heldItem.NetFields.Parent = null;
							}
							Game1.player.hat.Value = null;
							Game1.player.CursorSlotItem = heldItem;
							Game1.playSound("dwop");

							// Hack to allow compatibility with WearMoreRings mod.
							Game1.exitActiveMenu();
							Game1.activeClickableMenu = new GameMenu(false);
							performedSpecialFunctionality = true;
						}
						// Equipping
						else if (Game1.player.CursorSlotItem is Pan pan && pan.UpgradeLevel > 1)
						{
							Game1.player.CursorSlotItem = Utility.PerformSpecialItemGrabReplacement(Game1.player.hat.Value);
							Game1.player.hat.Value = new PanHat(pan);
							Game1.playSound("grassyStep");

							// Hack to allow compatibility with WearMoreRings mod.
							Game1.exitActiveMenu();
							Game1.activeClickableMenu = new GameMenu(false);
							performedSpecialFunctionality = true;
						}
					}

					if (heldItemWasNull && performedSpecialFunctionality && Game1.oldKBState.IsKeyDown(Keys.LeftShift))
					{
						for (int i = 0; i < Game1.player.items.Count; i++)
						{
							if (Game1.player.items[i] == null)
							{
								Item cursorSlotItem = Game1.player.CursorSlotItem;
								Game1.player.CursorSlotItem = null;

								Utility.addItemToInventory(cursorSlotItem, i, __instance.inventory.actualInventory);
								Game1.playSound("stoneStep");
								break;
							}
						}
					}
				}

				if (heldItemWasNull && !performedSpecialFunctionality && Game1.oldKBState.IsKeyDown(Keys.LeftShift))
				{
					int inventorySlot = __instance.inventory.getInventoryPositionOfClick(x, y);
					if (inventorySlot != -1 && Game1.player.items[inventorySlot] is Pan pan)
					{
						__instance.inventory.leftClick(x, y, null, false);
						pan.NetFields.Parent = null;
						
						if (Game1.player.hat.Value != null)
						{
							Game1.player.items[inventorySlot] = Utility.PerformSpecialItemGrabReplacement(Game1.player.hat.Value);
						}

						if (pan.UpgradeLevel > 1)
						{
							Game1.player.hat.Value = new PanHat(pan);
							Game1.playSound("grassyStep");
						}
						else
						{
							Game1.player.hat.Value = new Hat(71);
							Game1.playSound("grassyStep");
						}

						// Hack to allow compatibility with WearMoreRings mod.
						Game1.exitActiveMenu();
						Game1.activeClickableMenu = new GameMenu(false);
						performedSpecialFunctionality = true;
					}
				}

			}

			return !performedSpecialFunctionality;
		}
	}
}