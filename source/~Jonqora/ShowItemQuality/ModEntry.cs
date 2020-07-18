using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Harmony;
using CIL = Harmony.CodeInstruction;
using StardewModdingAPI;
using StardewValley;
using System.Reflection.Emit;
using System.Reflection;
using StardewValley.Objects;
using Object = StardewValley.Object;
using System.Runtime.InteropServices;
using System.Text;

namespace ShowItemQuality
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        internal static ModEntry Instance { get; private set; }
        internal HarmonyInstance Harmony { get; private set; }
        public static IMonitor ModMonitor { get; private set; }

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // Make resources available.
            Instance = this; 
            ModMonitor = this.Monitor;
            Harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

            // Apply the patch to show item quality when drawing in HUD
            Harmony.Patch(
                original: AccessTools.Method(typeof(HUDMessage), nameof(HUDMessage.draw)),
                transpiler: new HarmonyMethod(AccessTools.Method(typeof(HUDPatch), nameof(HUDPatch.HUDMessageDraw_Transpiler)))
            );
            // Apply the patch to use most recent item in a stack to display HUD icon
            Harmony.Patch(
                original: AccessTools.Method(typeof(Game1), nameof(Game1.addHUDMessage)),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(HUDPatch), nameof(HUDPatch.addHUDMessage_Postfix)))
            );
            // Apply the patch to avoid the usual method of drawing initial stack values in HUD
            Harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.addItemToInventoryBool)),
                transpiler: new HarmonyMethod(AccessTools.Method(typeof(FarmerPatch), nameof(FarmerPatch.addItemToInventoryBool_Transpiler)))
            );
        }

        /*********
        ** Harmony patches
        *********/
        /// <summary>Contains patches for patching game code in the StardewValley.HUDMessage class.</summary>
        internal class HUDPatch
        {
            /// <summary>Changes an argument in this.messageSubject.drawInMenu() to use StackDrawType.Draw instead of StackDrawType.Hide)</summary>
            internal static IEnumerable<CodeInstruction> HUDMessageDraw_Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                try
                {
                    var codes = new List<CodeInstruction>(instructions);

                    for (int i = 0; i < codes.Count - 1; i++)
                    {
                        // find Enum value of 0 (StackDrawType.Hide) loaded for the last argument of this.messageSubject.drawInMenu()
                        if (codes[i].opcode == OpCodes.Ldc_I4_0 &&
                            codes[i + 1].opcode == OpCodes.Callvirt &&
                            codes[i + 1].operand.ToString() == "Void drawInMenu(Microsoft.Xna.Framework.Graphics.SpriteBatch, Microsoft.Xna.Framework.Vector2, Single, Single, Single, StardewValley.StackDrawType)")
                        {
                            // change to Enum value 1 (StackDrawType.Draw)
                            codes[i].opcode = OpCodes.Ldc_I4_1;
                            ModMonitor.LogOnce($"Changed StackDrawType Enum in HUDMessage.draw method: {nameof(HUDMessageDraw_Transpiler)}", LogLevel.Trace);
                            break;
                        }
                    }
                    return codes.AsEnumerable();
                }
                catch (Exception ex)
                {
                    ModMonitor.Log($"Failed in {nameof(HUDMessageDraw_Transpiler)}:\n{ex}", LogLevel.Error);
                    return instructions; // use original code
                }
            }
            /// <summary>When adding an HUD message that stacks with a previous one, use the newest message to update messageSubject.</summary>
            internal static void addHUDMessage_Postfix(HUDMessage message)
            {
                try
                {
                    if (message.type != null || message.whatType != 0)
                    {   
                        for (int index = 0; index < Game1.hudMessages.Count; ++index)
                        {
                            if (message.type != null && Game1.hudMessages[index].type != null && (Game1.hudMessages[index].type.Equals(message.type) && Game1.hudMessages[index].add == message.add))
                            {
                                // Altered code to affect and keep current message in the place of existing one
                                // Keep the updated stack number
                                message.number = Game1.hudMessages[index].number; 
                                // Replace the old HUDMessage with the new one, instead of keeping the old.
                                Game1.hudMessages.RemoveAt(index);
                                Game1.hudMessages.Insert(index, message);

                                ModMonitor.LogOnce($"Ran patch for Game1.addHUDMessage method in game code: {nameof(addHUDMessage_Postfix)}", LogLevel.Trace);
                                return;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModMonitor.Log($"Failed in {nameof(addHUDMessage_Postfix)}:\n{ex}", LogLevel.Error);
                }
            }
        }

        internal class FarmerPatch
        {
            /// <summary>Insertable, altered HUDMessage constructor that uses a single-stack clone of the original object.</summary>
            public static HUDMessage FixedHUDMessage_Hook(string displayName, int number, bool add, Color color, Item item)
            {
                try
                {
                    Item hudItem = item.getOne(); // Grabs a clone of the item to display in HUDMessage

                    ModMonitor.LogOnce($"Ran patch for HUDMessage creation in Farmer.addItemToInventoryBool method: {nameof(FixedHUDMessage_Hook)}", LogLevel.Trace);
                    return new HUDMessage(displayName, number, add, color, hudItem);
                }
                catch (Exception ex)
                {
                    ModMonitor.Log($"Failed in {nameof(FixedHUDMessage_Hook)}:\n{ex}", LogLevel.Error);
                    return new HUDMessage(displayName, number, add, color, item);
                }
            }

            /// <summary>Adds a hook that calls FixedHUDMessage_Hook inside the game's addItemToInventoryBool method.</summary>
            public static IEnumerable<CIL> addItemToInventoryBool_Transpiler(IEnumerable<CIL> instructions)
            {
                try
                {
                    var codes = new List<CodeInstruction>(instructions);

                    for (int i = 0; i < codes.Count - 9; i++)
                    {
                        // This is the line of code we want to find and change:
                        //     Game1.addHUDMessage(new HUDMessage(displayName, Math.Max(1, item.Stack), true, color, item));
                        //     Game1.addHUDMessage(new HUDMessage(displayName, 1, true, color, item));
                        // We can most easily do this by replacing several instructions with nop

                        if (//ldloc.3
                            codes[i].opcode == OpCodes.Ldloc_3 &&
                            //ldc.i4.1                             // Replace with nop
                            codes[i + 1].opcode == OpCodes.Ldc_I4_1 &&
                            //ldarg.1                                // Replace with nop
                            codes[i + 2].opcode == OpCodes.Ldarg_1 &&
                            //callvirt instance int32 StardewValley.Item::get_Stack() // Replace with nop
                            codes[i + 3].opcode == OpCodes.Callvirt &&
                            (MethodInfo)codes[i + 3].operand == typeof(Item).GetMethod("get_Stack") &&
                            //call int32 [mscorlib]System.Math::Max(int32, int32) // Replace with Ldc_I4_1
                            codes[i + 4].opcode == OpCodes.Call &&
                            (MethodInfo)codes[i + 4].operand == typeof(Math).GetMethod("Max", types: new Type[] { typeof(int), typeof(int) }) &&
                            //ldc.i4.1
                            codes[i + 5].opcode == OpCodes.Ldc_I4_1 &&
                            //ldloc.2
                            codes[i + 6].opcode == OpCodes.Ldloc_2 &&
                            //ldarg.1
                            codes[i + 7].opcode == OpCodes.Ldarg_1 &&
                            //newobj instance void StardewValley.HUDMessage::.ctor(string, int32, bool, valuetype [Microsoft.Xna.Framework]Microsoft.Xna.Framework.Color, class StardewValley.Item)
                            codes[i + 8].opcode == OpCodes.Newobj &&
                            (ConstructorInfo)codes[i + 8].operand == typeof(HUDMessage).GetConstructor(
                                types: new Type[] { typeof(string), typeof(int), typeof(bool), typeof(Color), typeof(Item) }) &&
                            //call void StardewValley.Game1::addHUDMessage(class StardewValley.HUDMessage)
                            codes[i + 9].opcode == OpCodes.Call &&
                            (MethodInfo)codes[i + 9].operand == typeof(Game1).GetMethod("addHUDMessage"))
                        {
                            // Compose the new instructions to use as replacements
                            codes[i + 8] = new CIL(OpCodes.Call, Instance.Helper.Reflection.GetMethod(
                                typeof(FarmerPatch), nameof(FixedHUDMessage_Hook)).MethodInfo); // Call my function that returns a fixed HUDMessage
                            ModMonitor.LogOnce($"Added hook to Farmer.addItemToInventoryBool method: {nameof(addItemToInventoryBool_Transpiler)}", LogLevel.Trace);
                        }
                    }
                    return codes.AsEnumerable();
                }
                catch (Exception ex)
                {
                    ModMonitor.Log($"Failed in {nameof(addItemToInventoryBool_Transpiler)}:\n{ex}", LogLevel.Error);
                    return instructions; // use original code
                }
            }
        }
    }
}