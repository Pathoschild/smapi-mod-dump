/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Videogamers0/SDV-CombineMachines
**
*************************************************/

using CombineMachines.Helpers;
using Harmony;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using SObject = StardewValley.Object;

namespace CombineMachines.Patches
{
    [HarmonyPatch(typeof(Farm), nameof(Farm.addCrows))]
    public static class ScarecrowPatches
    {
        public static int GetScarecrowBaseRadius(this SObject Obj)
        {
            if (Obj.IsScarecrow())
                return Obj.ParentSheetIndex == 167 ? 17 : 9;
            else
                return 9;
        }

        public static int GetScarecrowRadius(this SObject Obj)
        {
            int BaseRadius = GetScarecrowBaseRadius(Obj);
            if (Obj.IsScarecrow())
            {
                if (Obj.IsCombinedMachine())
                {
                    double RadiusMultiplier = Obj.GetScarecrowRadiusMultiplier();
                    double Result = BaseRadius * RadiusMultiplier;
#if NEVER //DEBUG
                    ModEntry.Logger.Log(string.Format("{0}: Base={1}, Computed={2}", nameof(GetScarecrowRadius), BaseRadius, Result), ModEntry.InfoLogLevel);
#endif
                    return (int)Math.Round(Result, MidpointRounding.AwayFromZero);
                }
                else
                    return BaseRadius;
            }
            else
                return BaseRadius;
        }

        public static double GetScarecrowRadiusMultiplier(this SObject Obj)
        {
            if (Obj.IsScarecrow() && Obj.IsCombinedMachine())
            {
                double TilesMultiplier = Obj.GetProcessingPower();
                double RadiusMultiplier = Math.Sqrt(TilesMultiplier);
                return RadiusMultiplier;
            }
            else
                return 1.0;
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> original = instructions.ToList();
            List<CodeInstruction> patched = new List<CodeInstruction>();

            //  Find the IL code that is basically doing this:
            //  int radius = this.objects[s].Name.Contains("Deluxe") ? 17 : 9;
            //  (Located in StardewValley.Farm.addCrows(), 5 instructions before it attempts to compare the item's name to "Deluxe")
            int startIndex = -1;
            for (int i = 0; i < original.Count; i++)
            {
                CodeInstruction current = original[i];
                if (current.opcode == OpCodes.Ldstr)
                {
                    string value = (string)current.operand;
                    if (value.Equals("Deluxe", StringComparison.CurrentCultureIgnoreCase))
                    {
                        int startOffset = 5;
                        startIndex = i - startOffset;
                    }
                }
            }

            for (int i = 0; i < original.Count; i++)
            {
                CodeInstruction current = original[i];

#if NEVER //DEBUG
                //  Make the crows extremely common for testing purposes

                //  Before: if (Game1.random.NextDouble() < 0.3)
                //  After:  if (Game1.random.NextDouble() < 1.0)
                if (current.opcode == OpCodes.Ldc_R8)
                {
                    double doubleValue = (double)current.operand;
                    if (doubleValue == 0.3)
                    {
                        current.operand = 1.0;
                    }
                }

                //  Before: while (attempts < 10)
                //  After:  while (attempts < 127)
                if (current.opcode == OpCodes.Ldc_I4_S)
                {
                    sbyte value = (sbyte)current.operand;
                    if (value == 10)
                    {
                        current.operand = sbyte.MaxValue;
                    }
                }

                //  Before: int potentialCrows = Math.Min(4, numCrops / 16);
                //  After:  int potentialCrows = Math.Min(500, numCrops / 16);
                if (current.opcode == OpCodes.Ldc_I4_4)
                {
                    current.opcode = OpCodes.Ldc_I4;
                    current.operand = 500;
                }
#endif

                if (i != startIndex)
                    patched.Add(current);
                else
                {
                    //  Replace these 12 instructions:
                    /*
                        IL_01af: ldarg.0
						IL_01b0: ldfld class StardewValley.Network.OverlaidDictionary StardewValley.GameLocation::objects
						IL_01b5: ldloc.s s
						IL_01b7: callvirt instance class StardewValley.Object StardewValley.Network.OverlaidDictionary::get_Item(valuetype [Microsoft.Xna.Framework]Microsoft.Xna.Framework.Vector2)
						IL_01bc: callvirt instance string StardewValley.Item::get_Name()
						IL_01c1: ldstr "Deluxe"
						IL_01c6: callvirt instance bool [mscorlib]System.String::Contains(string)
						IL_01cb: brtrue.s IL_01d1

						IL_01cd: ldc.i4.s 9
						IL_01cf: br.s IL_01d3

						IL_01d1: ldc.i4.s 17

						IL_01d3: stloc.s radius
                    */
                    //  Which are basically just doing this:
                    //  int radius = this.objects[s].Name.Contains("Deluxe") ? 17 : 9;

                    //  Patch them with instructions that will instead do this:
                    //  int radius = this.objects[s].GetScarecrowRadius();

                    List<CodeInstruction> radiusInstructions = new List<CodeInstruction>()
                    {
                        //  Push 'this' onto the stack ('this' is StardewValley.Farm instance)
                        new CodeInstruction(OpCodes.Ldarg_0),
                        //  Call this.objects[s]
                        new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(GameLocation), nameof(GameLocation.objects))),
                        new CodeInstruction(OpCodes.Ldloc_S, 15),
                        new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(OverlaidDictionary), "get_Item", new Type[] { typeof(Vector2) })),
                        //  Call my custom extension method to compute the desired radius
                        new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ScarecrowPatches), nameof(GetScarecrowRadius), new Type[] { typeof(SObject) })),
                        //  Store the value to the 'radius' local variable which is local variable with index=16
                        new CodeInstruction(OpCodes.Stloc_S, 16)

                        //  TODO: Do I need to pop 'this' from the stack? 
                        //new CodeInstruction(OpCodes.Pop)
                        //  Even if I should, it probably doesn't matter since it is probably already be at the very bottom of the stack? Idk
                    };

                    patched.AddRange(radiusInstructions);
                    i += 12 - 1;
                }
            }

            return patched.AsEnumerable();
        }
    }
}
