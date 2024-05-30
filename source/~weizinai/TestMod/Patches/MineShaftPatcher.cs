/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using System.Reflection;
using System.Reflection.Emit;
using Common.Patch;
using HarmonyLib;
using StardewValley;
using StardewValley.Locations;
using TestMod.Framework;

namespace TestMod.Patches;

internal class MineShaftPatcher : BasePatcher
{
    private static ModConfig config = null!;

    public MineShaftPatcher(ModConfig config)
    {
        MineShaftPatcher.config = config;
    }

    public override void Apply(Harmony harmony)
    {
        harmony.Patch(RequireMethod<MineShaft>(nameof(MineShaft.loadLevel)), transpiler: GetHarmonyMethod(nameof(LoadLevelTranspiler)));
    }

    private static IEnumerable<CodeInstruction> LoadLevelTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var codes = new List<CodeInstruction>(instructions);

        var findCreateDaySaveRandomMethod = false;
        var findChanceField = false;
        for (var i = 0; i < codes.Count; i++)
        {
            if (!findChanceField && codes[i].opcode == OpCodes.Call)
            {
                if (codes[i].opcode == OpCodes.Call && codes[i].operand.Equals(AccessTools.Method(typeof(Utility), nameof(Utility.CreateDaySaveRandom))))
                    findCreateDaySaveRandomMethod = true;
            }

            if (findCreateDaySaveRandomMethod && !findChanceField)
            {
                if (codes[i].opcode == OpCodes.Ldc_R8 && Math.Abs((double)codes[i].operand - 0.06) < 0.1)
                {
                    codes[i].operand = 1.0;
                    findChanceField = true;
                }
            }

            if (findChanceField)
            {
                if (codes[i].opcode == OpCodes.Stloc_1)
                {
                    codes.Insert(i - 3, new CodeInstruction(OpCodes.Ldc_I4, config.MineShaftMap));
                    codes.Insert(i - 2, new CodeInstruction(OpCodes.Stloc_0));
                    break;
                }
            }
        }

        return codes.AsEnumerable();
    }
}