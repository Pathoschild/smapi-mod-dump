using Harmony;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using xTile.Dimensions;

namespace MTN2.Patches.FarmPatches
{
    /// <summary>
    /// REASON FOR PATCHING: To adjust for the X, Y locational checks of the starting
    /// shipping bin, for custom farm maps may have moved it.
    /// 
    /// 
    /// Patches the method Farm.checkAction to adjust for the movement
    /// of the starting shipping bin (the bin that is not classified as a building).
    /// </summary>
    public class checkActionPatch
    {
        private static CustomManager customManager;
        private static Farm currentFarm;

        /// <summary>
        /// Constructor. Awkward method of setting references needed. However, Harmony patches
        /// are required to be static. Thus we must break good Object Orientated practices.
        /// </summary>
        /// <param name="customManager">The class controlling information pertaining to the customs (and the loaded customs).</param>
        public checkActionPatch(CustomManager customManager) {
            checkActionPatch.customManager = customManager;
        }

        /// <summary>
        /// Transpiles the CLI to remove operations pertaining to the user clicking on the starting shipping bin
        /// (The only shipping bin that is not of class building)
        /// </summary>
        /// <param name="instructions">Code Instructions (in CLI)</param>
        /// <returns></returns>
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
            int i;
            var codes = new List<CodeInstruction>(instructions);

            for (i = 136; i < 197; i++) {
                codes[i].opcode = OpCodes.Nop;
            }

            return codes.AsEnumerable();
        }

        /// <summary>
        /// Postfix method. Occurs after the original method of Farm.checkAction is executed.
        /// 
        /// Reimplements operations pertaining to user clicking on the starting shipping bin,
        /// allowing for the shipping bin to be relocated.
        /// </summary>
        /// <param name="__instance">The instance of the Farm class</param>
        /// <param name="__result">The results from the original checkAction method.</param>
        /// <param name="tileLocation">From original method. The tile location that was clicked.</param>
        /// <param name="viewport">From original method. The user's viewport.</param>
        /// <param name="who">From original method. The farmer (player) who clicked</param>
        public static void Postfix(Farm __instance, ref bool __result, Location tileLocation, Rectangle viewport, Farmer who) {
            if (currentFarm != __instance) currentFarm = __instance;
            int binX = (customManager.Canon) ? 71 : customManager.ShippingBinPoints.X;
            int binY = (customManager.Canon) ? 13 : customManager.ShippingBinPoints.Y;
            if (tileLocation.X >= binX && tileLocation.X <= binX + 1 && tileLocation.Y >= binY && tileLocation.Y <= binY + 1) {
                ItemGrabMenu itemGrabMenu = new ItemGrabMenu(null, true, false, new InventoryMenu.highlightThisItem(Utility.highlightShippableObjects), new ItemGrabMenu.behaviorOnItemSelect(shipItem), "", null, true, true, false, true, false, 0, null, -1, null);
                itemGrabMenu.initializeUpperRightCloseButton();
                itemGrabMenu.setBackgroundTransparency(false);
                itemGrabMenu.setDestroyItemOnClick(true);
                itemGrabMenu.initializeShippingBin();
                Game1.activeClickableMenu = itemGrabMenu;
                __instance.playSound("shwip");
                if (Game1.player.FacingDirection == 1) {
                    Game1.player.Halt();
                }
                Game1.player.showCarrying();
                __result = true;
            }
            return;
        }

        /// <summary>
        /// Replica of SDV's "shipItem" method.
        /// </summary>
        /// <param name="item">The item to be shipped.</param>
        /// <param name="who">The player (farmer) who is shipping the item.</param>
        public static void shipItem(Item item, Farmer who) {
            if (item != null) {
                who.removeItemFromInventory(item);
                currentFarm.shippingBin.Add(item);
                if (item is StardewValley.Object) {
                    currentFarm.showShipment(item as StardewValley.Object, false);
                }
                currentFarm.lastItemShipped = item;
                if (Game1.player.ActiveObject == null) {
                    Game1.player.showNotCarrying();
                    Game1.player.Halt();
                }
            }
            return;
        }
    }
}
