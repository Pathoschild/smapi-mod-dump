using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Network;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace ProfessionAdjustments.Overrides
{
    public class SellToStorePriceMethodHook
    {

        public static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, MethodBase original, IEnumerable<CodeInstruction> insns)
        {

            var newInsns = new List<CodeInstruction>();
            foreach (var insn in insns)
            {
                ProfessionAdjustments.instance.Monitor.Log(insn.ToString());

                if (insn.opcode == OpCodes.Ldc_R4 && insn.operand.ToString() == "1.4") {
                    
                    ProfessionAdjustments.instance.Monitor.Log(insn.operand.ToString());

                    newInsns.Add(new CodeInstruction(OpCodes.Ldc_R4, 1.1f));
                } else {
                    newInsns.Add(insn);
                }
            }

            return newInsns;
        }
    }
}