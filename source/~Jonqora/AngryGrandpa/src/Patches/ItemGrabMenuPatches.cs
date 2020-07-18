using Harmony;
using CIL = Harmony.CodeInstruction;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;

namespace AngryGrandpa
{
    /// <summary>The class for patching methods on the StardewValley.Menus.ItemGrabMenu class.</summary>
    class ItemGrabMenuPatches
    {
        /*********
        ** Accessors
        *********/
        private static IModHelper Helper => ModEntry.Instance.Helper;
        private static IMonitor Monitor => ModEntry.Instance.Monitor;
        private static HarmonyInstance Harmony => ModEntry.Instance.Harmony;


        /*********
        ** Public methods
        *********/
        /// <summary>
        /// Applies the harmony patches defined in this class.
        /// </summary>
        public static void Apply()
        {
            // No need to apply this patch if BugFixAddItem in installed:
            if (Helper.ModRegistry.IsLoaded("Jonqora.BugFixAddItem"))
            {
                Monitor.Log($"BugFixAddItem Mod detected. Angry Grandpa will not harmony patch ItemGrabMenu methods; BugFixAddItem already takes care of this.", LogLevel.Debug);
            }
            else
            {
                // Add an inventory.onAddItem call if Game1.player.addItemToInventoryBool();
                Harmony.Patch(
                    original: AccessTools.Method(typeof(ItemGrabMenu), nameof(ItemGrabMenu.receiveLeftClick)),
                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(ItemGrabMenuPatches), nameof(ItemGrabMenuPatches.receiveLeftClick_Transpiler)))
                );
            }
        }

        /// <summary>
        /// Invokes the onAddItem delegate action of type (ItemGrabMenu.behaviorOnItemSelect) for an item that's been added to inventory.
        /// </summary>
        /// <param name="grabMenu">ItemGrabMenu instance where the method call originated</param>
        /// <param name="who">Farmer (Game1.player) who is using the menu</param>
        public static void OnAddItemCheck_Hook(ItemGrabMenu grabMenu, Farmer who)
        {
            try
            {
                if (grabMenu.inventory.onAddItem != null)
                {
                    grabMenu.inventory.onAddItem(grabMenu.heldItem, who);
                    Monitor.Log($"Ran patch for bug in game code: {nameof(OnAddItemCheck_Hook)}", LogLevel.Trace);
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(OnAddItemCheck_Hook)}:\n{ex}", LogLevel.Error);
            }
        }

        /// <summary>
        /// Finds any sequence matching `if(Game1.player.addItemToInventoryBool(this.heldItem, false))` followed if true by `this.heldItem = null;`
        /// If found, injects a call to ItemGrabMenuPatches.OnAddItemCheck_Hook(this.heldItem, Game1.player) to invoke the missing onAddItem action.
        /// </summary>
        /// <param name="instructions">CodeInstruction enumerable provided by Harmony for the original receiveLeftClick function</param>
        /// <returns>Altered CodeInstruction enumerable if a suitable patch injection site was found; else returns original codes</returns>
        public static IEnumerable<CIL> receiveLeftClick_Transpiler(IEnumerable<CIL> instructions)
        {
            try
            {
                var codes = new List<CodeInstruction>(instructions);
                bool patched = false;

                for (int i = 0; i < codes.Count - 8; i++)
                {
                    // Find any sequence matching if(Game1.player.addItemToInventoryBool(this.heldItem, false)
                    //     which is followed by this.heldItem = null assignment if true.

                    if (//call class StardewValley.Farmer StardewValley.Game1::get_player()
                        codes[i].opcode == OpCodes.Call &&
                        (MethodInfo)codes[i].operand == typeof(Game1).GetProperty("player").GetGetMethod() &&
                        //ldarg.0
                        codes[i + 1].opcode == OpCodes.Ldarg_0 &&
                        //ldfld class StardewValley.Item StardewValley.Menus.MenuWithInventory::heldItem
                        codes[i + 2].opcode == OpCodes.Ldfld &&
                        (FieldInfo)codes[i + 2].operand == typeof(MenuWithInventory).GetField("heldItem") &&
                        //ldc.i4.0
                        codes[i + 3].opcode == OpCodes.Ldc_I4_0 &&
                        //callvirt instance bool StardewValley.Farmer::addItemToInventoryBool(class StardewValley.Item, bool)
                        codes[i + 4].opcode == OpCodes.Callvirt &&
                        (MethodInfo)codes[i + 4].operand == typeof(Farmer).GetMethod("addItemToInventoryBool") &&
                        //brfalse IL_05dc || brfalse.s IL_02f0 (return)
                        (codes[i + 5].opcode == OpCodes.Brfalse || codes[i + 5].opcode == OpCodes.Brfalse_S) &&
                        //ldarg.0 // **INSERT NEW CODES AT THIS INDEX**
                        codes[i + 6].opcode == OpCodes.Ldarg_0 &&
                        //ldnull
                        codes[i + 7].opcode == OpCodes.Ldnull &&
                        //stfld class StardewValley.Item StardewValley.Menus.MenuWithInventory::heldItem
                        codes[i + 8].opcode == OpCodes.Stfld &&
                        (FieldInfo)codes[i + 8].operand == typeof(MenuWithInventory).GetField("heldItem"))
                    {
                        // Compose the new instructions to inject
                        var codesToInsert = new List<CodeInstruction>
                        {
                            new CIL(OpCodes.Ldarg_0),
                            new CIL(OpCodes.Call, typeof(Game1).GetProperty("player").GetGetMethod()),
                            new CIL(OpCodes.Call, Helper.Reflection.GetMethod(
                                typeof(ItemGrabMenuPatches),nameof(OnAddItemCheck_Hook)).MethodInfo)
                        };

                        // Inject the instructions
                        codes.InsertRange(i + 6, codesToInsert);
                        patched = true;
                    }
                }
                if (patched) { Monitor.LogOnce($"Applied harmony patch to ItemGrabMenu: {nameof(receiveLeftClick_Transpiler)}", LogLevel.Trace); }
                else { Monitor.Log($"Couldn't apply harmony patch to ItemGrabMenu: {nameof(receiveLeftClick_Transpiler)}" +
                    $"This will not severely affect the game, but collecting rewards from grandpa's shrine might not register correctly.", LogLevel.Warn); }

                return codes.AsEnumerable();
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(receiveLeftClick_Transpiler)}:\n{ex}", LogLevel.Error);
                return instructions; // use original code
            }
        }
    }
}
