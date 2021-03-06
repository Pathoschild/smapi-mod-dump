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

namespace UpgradablePan
{
	public class InventoryPagePatches
	{
		public static bool receiveLeftClick_Prefix(ref InventoryPage __instance, int x, int y, bool playSound)
		{
			bool specialFunctionalityHandled = false;
			foreach (ClickableComponent c in __instance.equipmentIcons)
			{
				bool heldItemWasNull = Game1.player.CursorSlotItem == null;
				if (c.containsPoint(x, y) && c.name == "Hat")
				{
					if ((Game1.player.CursorSlotItem == null && Game1.player.hat.Value.which == 71) || (Game1.player.CursorSlotItem is Pan pan && pan.UpgradeLevel > 1))
					{
						Hat tmp = null;
						
						if (Game1.player.CursorSlotItem is Pan p)
						{
							tmp = new PanHat(p);
						}
						else
						{
							tmp = Game1.player.CursorSlotItem as Hat;
							Game1.player.CursorSlotItem = null;
						}

						Item heldItem2 = Game1.player.hat.Value;
						heldItem2 = Utility.PerformSpecialItemGrabReplacement(heldItem2);
						if (heldItem2 != null)
						{
							heldItem2.NetFields.Parent = null;
						}
						Game1.player.CursorSlotItem = heldItem2;
						Game1.player.hat.Value = tmp;
						if (Game1.player.hat.Value != null)
						{
							Game1.playSound("grassyStep");
						}
						else if (Game1.player.CursorSlotItem != null)
						{
							Game1.playSound("dwop");
						}
						specialFunctionalityHandled = true;
					}
				}
			}

			return !specialFunctionalityHandled;
		}
	}
}