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
            foreach (CodeInstruction instruction in instructions)
            {
#if NEVER // DEBUG
                //  Make the crows extremely common for testing purposes

                //  Before: if (Game1.random.NextDouble() < 0.3)
                //  After:  if (Game1.random.NextDouble() < 1.0)
                if (instruction.opcode == OpCodes.Ldc_R8)
                {
                    double doubleValue = (double)instruction.operand;
                    if (doubleValue == 0.3)
                    {
                        instruction.operand = 1.0;
                    }
                }

                //  Before: while (attempts < 10)
                //  After:  while (attempts < 127)
                if (instruction.opcode == OpCodes.Ldc_I4_S)
                {
                    sbyte value = (sbyte)instruction.operand;
                    if (value == 10)
                    {
                        instruction.operand = sbyte.MaxValue;
                    }
                }

                //  Before: int potentialCrows = Math.Min(4, numCrops / 16);
                //  After:  int potentialCrows = Math.Min(500, numCrops / 16);
                if (instruction.opcode == OpCodes.Ldc_I4_4)
                {
                    instruction.opcode = OpCodes.Ldc_I4;
                    instruction.operand = 500;
                }
#endif

                if (instruction.opcode == OpCodes.Ldc_I4_S)
                {
                    sbyte value = (sbyte)instruction.operand;
                    if (value == 9 || value == 17)
                    {
                        //  Intended logic: "int radius = this.objects[s].GetScarecrowRadius();" where this refers to StardewValley.Farm (sub-type of GameLocation)
                        //  Push 'this' onto the stack
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        //  Call this.objects[s]
                        yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(GameLocation), nameof(GameLocation.objects)));
                        yield return new CodeInstruction(OpCodes.Ldloc_S, 15);
                        yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(OverlaidDictionary), "get_Item", new Type[] { typeof(Vector2) }));
                        //  Call my custom extension method to compute the desired radius
                        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ScarecrowPatches), nameof(GetScarecrowRadius), new Type[] { typeof(SObject) }));
                        //  Store the value to the 'radius' local variable
                        yield return new CodeInstruction(OpCodes.Stloc_S, 16);

                        //  TODO: Do I need to pop 'this' from the stack? 
                        //yield return new CodeInstruction(OpCodes.Pop);
                        //  Even if I should, it probably doesn't matter since it is probably already be at the very bottom of the stack? Idk

                        //  Clear the current instruction that would normally push a hardcoded value of 9 or 17 onto the stack (the scarecrow's radius)
                        instruction.opcode = OpCodes.Nop;
                        yield return instruction;

                        //  Push our computed value onto the stack instead
                        yield return new CodeInstruction(OpCodes.Ldloc_S, 16);

                        continue;
                    }
                }

                yield return instruction;
            }
        }
    }
}
