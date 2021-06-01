/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://sourceforge.net/p/sdvmod-silo-size/
**
*************************************************/

using Harmony;
using Microsoft.Xna.Framework;
using PatcherHelper;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace SiloSize
    {
    internal static class StringBuilderExtensions
        {
        internal const string SPSPNL = "  ^";

        internal static StringBuilder AppendLoadStr(this StringBuilder sb, string source, object obj, string end = SPSPNL) {
            sb
                .Append(Game1.content.LoadString(source, obj))
                .Append(end)
                ;
            return sb;
            }
        internal static StringBuilder AppendLoadStr(this StringBuilder sb, string source, object obj1, object obj2, string end = SPSPNL) {
            sb
                .Append(Game1.content.LoadString(source, obj1, obj2))
                .Append(end)
                ;
            return sb;
            }
        }
    public class SiloSizePatcher
        {
        private static string UniqueID;
        private static IMonitor Monitor;

        //internal class WopcSiloSize : WantOpCache
        //    {
        //    internal WantOp
        //        int_240 = new WantOp(OpCodes.Ldc_I4, 240),
        //        call_numsilos = new WantOp(OpCodes.Call, "numSilos"),
        //        fld_piecesofhay = new WantOp(OpCodes.Ldfld, "piecesOfHay")
        //        ;
        //    }

        //private static WopcSiloSize wopc;
        private static HarmonyPatcherHelper harp;
        private static int SiloCapacity = 240;

        private static readonly StringBuilder strbldr = new StringBuilder();

        public static void Initialize(Mod mod, int siloCapacity) {
            UniqueID = mod.ModManifest.UniqueID;
            Monitor = mod.Monitor;
            SiloCapacity = siloCapacity;
            //wopc = new WopcSiloSize();
            harp = new HarmonyPatcherHelper(mod);
            }

        public static void Execute() {
            harp.TryPatching(
                typeof(StardewValley.Farm),
                nameof(StardewValley.Farm.tryToAddHay),
                prefix: new HarmonyMethod(typeof(SiloSizePatcher), nameof(Farm_tryToAddHay_prefix))
                );
            //harp.TryPatching(
            //    typeof(StardewValley.Buildings.Building),
            //    nameof(StardewValley.Buildings.Building.doAction),
            //    transpiler: new HarmonyMethod(typeof(SiloSizePatcher), nameof(Buildings_Building_doAction_transp))
            //    );
            harp.TryPatching(
                typeof(StardewValley.Buildings.Building),
                nameof(StardewValley.Buildings.Building.doAction),
                prefix: new HarmonyMethod(typeof(SiloSizePatcher), nameof(Buildings_Building_doAction_prefix))
                );
            //harp.TryPatching(
            //    typeof(StardewValley.Object),
            //    nameof(StardewValley.Object.checkForAction),
            //    transpiler: new HarmonyMethod(typeof(Patcher), nameof(Object_checkForAction_transp))
            //    );
            harp.TryPatching(
                typeof(StardewValley.Object),
                nameof(StardewValley.Object.checkForAction),
                prefix: new HarmonyMethod(typeof(SiloSizePatcher), nameof(Object_checkForAction_prefix))
                );
            }

        public static void Cleanup() {
            //wopc = null;
            harp = null;
            }

        public static bool Farm_tryToAddHay_prefix(StardewValley.Farm __instance, ref int __result, int num) {
            try {
                int num1 = Math.Min(Utility.numSilos() * SiloCapacity - __instance.piecesOfHay.Value, num);
                __instance.piecesOfHay.Value += num1;
                __result = num - num1;
                return false;
                }
            catch (Exception ex) {
                Monitor.Log($"Farm_tryToAddHay_prefix technical problem:\n{ex}", LogLevel.Error);
                Monitor.Log("Reverting to unpatched Farm.tryToAddHay behavior!", LogLevel.Warn);
                return true;
                }
            }

        //public static IEnumerable<CodeInstruction> Buildings_Building_doAction_transp(IEnumerable<CodeInstruction> oldInstructions) {
        //    var walker = new InstructionsWalker(oldInstructions);

        //    /* Looking for CIL:
        //        // [280 13 - 280 200]
        //        IL_02f1: ldsfld       class StardewValley.LocalizedContentManager StardewValley.Game1::content
        //        IL_02f6: ldstr        "Strings\\Buildings:PiecesOfHay"
        //        IL_02fb: ldstr        "Farm"
        //        IL_0300: call         class StardewValley.GameLocation StardewValley.Game1::getLocationFromName(string)
        //        IL_0305: isinst       StardewValley.Farm
        //        IL_030a: ldfld        class [Netcode]Netcode.NetInt StardewValley.Farm::piecesOfHay
        //        IL_030f: call         int32 StardewValley.Utility::numSilos()
        //        IL_0314: ldc.i4       240 // 0x000000f0
        //        IL_0319: mul
        //        IL_031a: box          [mscorlib]System.Int32
        //        IL_031f: callvirt     instance string StardewValley.LocalizedContentManager::LoadString(string, object, object)
        //        IL_0324: call         void StardewValley.Game1::drawObjectDialogue(string)
        //    */
        //    var to_replace = new WantOp(OpCodes.Ldc_I4, 240);
        //    var target = new WantOp[] {
        //        new WantOp(OpCodes.Ldsfld, "content"),
        //        new WantOp(OpCodes.Ldstr, "Strings\\Buildings:PiecesOfHay"),
        //        new WantOp(OpCodes.Ldstr, "Farm"),
        //        new WantOp(OpCodes.Call, "getLocationFromName"),
        //        new WantOp(OpCodes.Isinst),
        //        wopc.fld_piecesofhay,
        //        wopc.call_numsilos,
        //        wopc.int_240,
        //        wopc.mul,
        //        wopc.box,
        //        new WantOp(OpCodes.Callvirt, "LoadString"),
        //        new WantOp(OpCodes.Call, "drawObjectDialogue")
        //        };
        //    walker
        //        .GoFind(target)
        //        .GoForward(7)
        //        .ReplaceAt(with: new CodeInstruction(OpCodes.Ldc_I4, SiloCapacity), assert_previous: to_replace)
        //        ;

        //    return walker.Instructions;
        //    }

        public static bool Buildings_Building_doAction_prefix(StardewValley.Buildings.Building __instance, ref bool __result, Vector2 tileLocation, Farmer who) {
            try {
                if (who.isRidingHorse())
                    // Hand over to original
                    return true;
                float tileLocX = tileLocation.X;
                float tileLocY = tileLocation.Y;
                int tileX = __instance.tileX.Value;
                int tileY = __instance.tileY.Value;
                if (
                    who.IsLocalPlayer
                    && tileLocX >= tileX
                    && tileLocX < (double)(tileX + __instance.tilesWide.Value)
                    && tileLocY >= tileY
                    && tileLocY < (double)(tileY + __instance.tilesHigh.Value)
                    && __instance.daysOfConstructionLeft.Value > 0
                    )
                    // Hand over to original
                    return true;
                Point humanDoor = __instance.humanDoor.Value;
                if (!(
                    who.IsLocalPlayer
                    && tileLocX == (double)(humanDoor.X + tileX)
                    && tileLocY == (double)(humanDoor.Y + tileY)
                    && __instance.indoors.Value != null
                    ))
                    if (who.IsLocalPlayer && __instance.buildingType.Equals("Silo") && !__instance.isTilePassable(tileLocation)) {
                        if (!(who.ActiveObject != null && who.ActiveObject.parentSheetIndex.Value == 178)) {
                            Game1.drawObjectDialogue(
                                Game1.content.LoadString(
                                    "Strings\\Buildings:PiecesOfHay",
                                    Game1.getFarm().piecesOfHay,
                                    Utility.numSilos() * SiloCapacity
                                    )
                                );
                            __result = false;
                            return false;  // We've taken over, do not execute original function
                            }
                        }
                // Everything else, execute original function
                return true;
                }
            catch (Exception ex) {
                Monitor.Log($"Buildings_Building_doAction_prefix technical issue:\n{ex}", LogLevel.Error);
                Monitor.Log("Reverting to unpatched Buildings.Building.doAction behavior!", LogLevel.Warn);
                return true;
                }
            }


        //public static IEnumerable<CodeInstruction> Object_checkForAction_transp(IEnumerable<CodeInstruction> oldInstructions) {
        //    var walker = new InstructionsWalker(oldInstructions);

        //    /* Looking for CIL:
        //        // [3412 13 - 3426 21]
        //        IL_03b3: ldsfld       class StardewValley.Object/'<>c' StardewValley.Object/'<>c'::'<>9'
        //        IL_03b8: ldftn        instance void StardewValley.Object/'<>c'::'<checkForAction>b__282_0'()
        //        IL_03be: newobj       instance void StardewValley.DelayedAction/delayedBehavior::.ctor(object, native int)
        //        IL_03c3: dup
        //     */
        //    var target = new WantOp[] {
        //        wopc.ldsfld,
        //        wopc.ldftn,
        //        wopc.newobj,
        //        wopc.dup
        //        };
        //    walker
        //        .GoFind(target)
        //        .GoForward(1, assert_op: wopc.ldftn)
        //        .GoFetchOp(out CodeInstruction lambda)
        //        ;

        //    MethodInfo lambda_method = (lambda.operand as MethodInfo);
        //    harp.TryPatching(lambda_method, transpiler: new HarmonyMethod(typeof(SiloSizePatcher), nameof(Object_checkForAction_lambda_transp)));

        //    // Not editing anything
        //    return oldInstructions;
        //    }

        //public static IEnumerable<CodeInstruction> Object_checkForAction_lambda_transp(IEnumerable<CodeInstruction> oldInstructions) {
        //    var walker = new InstructionsWalker(oldInstructions);

        //    /* Looking for CIL:
        //          // [3422 15 - 3425 17]
        //          IL_005c: ldc.i4.1
        //          IL_005d: newarr       [mscorlib]System.String
        //          IL_0062: dup

        //          IL_0063: ldc.i4.0
        //          IL_0064: ldc.i4.s     18 // 0x12
        //          IL_0066: newarr       [mscorlib]System.String
        //          IL_006b: dup
        //          IL_006c: ldc.i4.0
        //          IL_006d: ldsfld       class StardewValley.LocalizedContentManager StardewValley.Game1::content
        //          IL_0072: ldstr        "Strings\\StringsFromCSFiles:FarmComputer_Intro"
        //          IL_0077: call         class StardewValley.Farmer StardewValley.Game1::get_player()
        //          IL_007c: ldfld        class [Netcode]Netcode.NetString StardewValley.Farmer::farmName
        //          IL_0081: callvirt     instance !0<string> class [Netcode]Netcode.NetFieldBase`2<string, class [Netcode]Netcode.NetString>::get_Value()
        //          IL_0086: callvirt     instance string StardewValley.LocalizedContentManager::LoadString(string, object)
        //          IL_008b: stelem.ref
        //          IL_008c: dup
        //          IL_008d: ldc.i4.1
        //          IL_008e: ldstr        "^--------------^"
        //          IL_0093: stelem.ref
        //          IL_0094: dup
        //          IL_0095: ldc.i4.2
        //          IL_0096: ldsfld       class StardewValley.LocalizedContentManager StardewValley.Game1::content
        //          IL_009b: ldstr        "Strings\\StringsFromCSFiles:FarmComputer_PiecesHay"
        //          IL_00a0: ldstr        "Farm"
        //          IL_00a5: call         class StardewValley.GameLocation StardewValley.Game1::getLocationFromName(string)
        //          IL_00aa: isinst       StardewValley.Farm
        //          IL_00af: ldfld        class [Netcode]Netcode.NetInt StardewValley.Farm::piecesOfHay
        //          IL_00b4: call         int32 StardewValley.Utility::numSilos()
        //          IL_00b9: ldc.i4       240 // 0x000000f0
        //          IL_00be: mul
        //     */
        //    var target2 = new WantOp[] {
        //        wopc.fld_piecesofhay,
        //        wopc.call_numsilos,
        //        wopc.int_240,
        //        wopc.mul
        //        };
        //    walker
        //        .GoFind(target2)
        //        .GoForward(2)
        //        .ReplaceAt(with: new CodeInstruction(OpCodes.Ldc_I4, SiloCapacity), assert_previous: wopc.int_240)
        //        ;

        //    return walker.Instructions;
        //    }

        [HarmonyBefore(new string[] { "Digus.ProducerFrameworkMod" })]
        public static bool Object_checkForAction_prefix(StardewValley.Object __instance, ref bool __result, Farmer who, bool justCheckingForActivity = false) {
            try {
                if (__instance.isTemporarilyInvisible)
                    // Hand over to original
                    return true;
                int _whoX = who.getTileX();
                int _whoY = who.getTileY();
                if (
                    !justCheckingForActivity
                    && who != null
                    && who.currentLocation.isObjectAtTile(_whoX, _whoY - 1)
                    && who.currentLocation.isObjectAtTile(_whoX, _whoY + 1)
                    && who.currentLocation.isObjectAtTile(_whoX + 1, _whoY)
                    && who.currentLocation.isObjectAtTile(_whoX - 1, _whoY)
                    && !who.currentLocation.getObjectAtTile(_whoX, _whoY - 1).isPassable()
                    && !who.currentLocation.getObjectAtTile(_whoX, _whoY + 1).isPassable()
                    && !who.currentLocation.getObjectAtTile(_whoX - 1, _whoY).isPassable()
                    && !who.currentLocation.getObjectAtTile(_whoX + 1, _whoY).isPassable()
                    )
                    // Hand over to original
                    return true;
                if (__instance.bigCraftable.Value && !justCheckingForActivity && __instance.parentSheetIndex.Value == 239) {
                    __instance.shakeTimer = 500;
                    who.currentLocation.localSound("DwarvishSentry");
                    who.freezePause = 500;
                    DelayedAction.functionAfterDelay(
                        () => {
                            Farm _farm = Game1.getFarm();
                            int totalCrops = _farm.getTotalCrops();
                            int totalOpenHoeDirt = _farm.getTotalOpenHoeDirt();
                            int cropsReadyForHarvest1 = _farm.getTotalCropsReadyForHarvest();
                            int totalUnwateredCrops = _farm.getTotalUnwateredCrops();
                            int cropsReadyForHarvest2 = _farm.getTotalGreenhouseCropsReadyForHarvest();
                            int totalForageItems = _farm.getTotalForageItems();
                            int machinesReadyForHarvest = _farm.getNumberOfMachinesReadyForHarvest();
                            bool flag = _farm.doesFarmCaveNeedHarvesting();
                            // Rewrote totally the series-of-string-concats in the original func because that was practically unreadable
                            strbldr.Clear()
                                .AppendLoadStr("Strings\\StringsFromCSFiles:FarmComputer_Intro", Game1.player.farmName.Value,
                                               end: "^--------------^")
                                .AppendLoadStr("Strings\\StringsFromCSFiles:FarmComputer_PiecesHay", _farm.piecesOfHay, Utility.numSilos() * SiloCapacity)
                                .AppendLoadStr("Strings\\StringsFromCSFiles:FarmComputer_TotalCrops", totalCrops)
                                .AppendLoadStr("Strings\\StringsFromCSFiles:FarmComputer_CropsReadyForHarvest", cropsReadyForHarvest1)
                                .AppendLoadStr("Strings\\StringsFromCSFiles:FarmComputer_CropsUnwatered", totalUnwateredCrops)
                                ;
                            if (cropsReadyForHarvest2 != -1) {
                                strbldr.AppendLoadStr("Strings\\StringsFromCSFiles:FarmComputer_CropsReadyForHarvest_Greenhouse", cropsReadyForHarvest2);
                                }
                            strbldr.AppendLoadStr("Strings\\StringsFromCSFiles:FarmComputer_TotalOpenHoeDirt", totalOpenHoeDirt);
                            if (Game1.whichFarm == 2 || Game1.whichFarm == 6) {
                                strbldr.AppendLoadStr("Strings\\StringsFromCSFiles:FarmComputer_TotalForage", totalForageItems);
                                }
                            strbldr
                                .AppendLoadStr("Strings\\StringsFromCSFiles:FarmComputer_MachinesReady", machinesReadyForHarvest)
                                .AppendLoadStr(
                                    "Strings\\StringsFromCSFiles:FarmComputer_FarmCave"
                                    ,
                                    flag
                                        ? Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_Yes")
                                        : Game1.content.LoadString("Strings\\Lexicon:QuestionDialogue_No")
                                    ,
                                    end: "  "
                                )
                                ;
                            Game1.multipleDialogues(new string[1]{ strbldr.ToString() });
                            },
                        500
                        );  // DelayedAction.functionAfterDelay
                    __result = true;
                    return false;  // We've taken over, do not execute original function
                    }
                // Everything else, execute original function
                return true;
                }
            catch (Exception ex) {
                Monitor.Log($"Object_checkForAction_prefix technical error:\n{ex}", LogLevel.Error);
                Monitor.Log("Reverting to unpatched behavior of Object.checkForAction", LogLevel.Warn);
                // Execute original function
                return true;
                }
            }
        }
    }
