using Harmony;
using StardewValley;
using StardewValley.Tools;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace ToolGeodes.Overrides
{
    public static class Game1ToolRangeHook
    {
        public static int toolRangeHook()
        {
            var tool = Game1.player.CurrentTool;
            if (tool == null)
                return 1;
            else if ( tool is Hoe || tool is Pickaxe || tool is WateringCan || tool is Axe )
            {
                var type = ToolType.Weapon;
                if (tool is Hoe) type = ToolType.Hoe;
                else if (tool is Pickaxe) type = ToolType.Pickaxe;
                else if (tool is WateringCan) type = ToolType.WateringCan;
                else if (tool is Axe) type = ToolType.Axe;

                if (Game1.player.HasAdornment(type, Mod.Config.GEODE_REMOTE_USE) > 0)
                    return 100;
                else
                    return 1;
            }
            else
                return 1;
        }

        public static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, MethodBase original, IEnumerable<CodeInstruction> insns)
        {
            // TODO: Learn how to use ILGenerator
            
            int utilWithinRadiusCount = 0;

            var newInsns = new List<CodeInstruction>();
            foreach (var insn in insns)
            {
                if (insn.opcode == OpCodes.Call && insn.operand is MethodInfo meth)
                {
                    if ( meth.Name == "withinRadiusOfPlayer" )
                    {
                        if ( utilWithinRadiusCount++ == 1 )
                        {
                            Log.trace("Found second Utility.withinRadiusOfPlayer call, replacing i-2 with our hook function");
                            newInsns[newInsns.Count - 2] = new CodeInstruction(OpCodes.Call, typeof(Game1ToolRangeHook).GetMethod("toolRangeHook"));
                        }
                    }
                }
                newInsns.Add(insn);
            }

            return newInsns;
        }
    }
}
