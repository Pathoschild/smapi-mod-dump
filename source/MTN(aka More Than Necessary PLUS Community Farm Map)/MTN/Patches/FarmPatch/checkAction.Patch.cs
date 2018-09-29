using Harmony;
using MTN.MapTypes;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using xTile.Dimensions;

namespace MTN.Patches.FarmPatch
{
    class checkActionPatch
    {
        public static Farm currentFarm;

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            int i;
            var codes = new List<CodeInstruction>(instructions);

            for (i = 135; i < 196; i++)
            {
                codes[i].opcode = OpCodes.Nop;
            }

            return codes.AsEnumerable();
        }

        public static void Postfix(Farm __instance, ref bool __result, Location tileLocation, Rectangle viewport, Farmer who)
        {
            if (__instance is FarmExtension) return;
            int binX = (Memory.isCustomFarmLoaded) ? Memory.loadedFarm.shippingBin.pointOfInteraction.x : 71;
            int binY = (Memory.isCustomFarmLoaded) ? Memory.loadedFarm.shippingBin.pointOfInteraction.y : 13;
            if (tileLocation.X >= binX && tileLocation.X <= binX+1 && tileLocation.Y >= binY && tileLocation.Y <= binY+1)
            {
                currentFarm = __instance;
                ItemGrabMenu itemGrabMenu = new ItemGrabMenu(null, true, false, new InventoryMenu.highlightThisItem(Utility.highlightShippableObjects), new ItemGrabMenu.behaviorOnItemSelect(shipItem), "", null, true, true, false, true, false, 0, null, -1, null);
                itemGrabMenu.initializeUpperRightCloseButton();
                itemGrabMenu.setBackgroundTransparency(false);
                itemGrabMenu.setDestroyItemOnClick(true);
                itemGrabMenu.initializeShippingBin();
                Game1.activeClickableMenu = itemGrabMenu;
                __instance.playSound("shwip");
                if (Game1.player.FacingDirection == 1)
                {
                    Game1.player.Halt();
                }
                Game1.player.showCarrying();
                __result = true;
            }
        }

        public static void shipItem(Item i, Farmer who)
        {
            if (i != null)
            {
                who.removeItemFromInventory(i);
                currentFarm.shippingBin.Add(i);
                if (i is StardewValley.Object)
                {
                    currentFarm.showShipment(i as StardewValley.Object, false);
                }
                currentFarm.lastItemShipped = i;
                if (Game1.player.ActiveObject == null)
                {
                    Game1.player.showNotCarrying();
                    Game1.player.Halt();
                }
            }
        }
    }
}
