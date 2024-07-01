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
using HarmonyLib;
using StardewValley;
using weizinai.StardewValleyMod.Common.Patcher;
using weizinai.StardewValleyMod.FriendshipDecayModify.Framework;

namespace weizinai.StardewValleyMod.FriendshipDecayModify.Patcher;

internal class FarmerPatcher : BasePatcher
{
    private static ModConfig config = null!;

    public FarmerPatcher(ModConfig config)
    {
        FarmerPatcher.config = config;
    }

    public override void Apply(Harmony harmony)
    {
        harmony.Patch(this.RequireMethod<Farmer>(nameof(Farmer.resetFriendshipsForNewDay)),
            transpiler: this.GetHarmonyMethod(nameof(ResetFriendshipsForNewDayTranspiler))
        );
    }

    // 每日对话修改
    private static IEnumerable<CodeInstruction> ResetFriendshipsForNewDayTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var codes = instructions.ToList();

        var index = codes.FindIndex(code => code.opcode == OpCodes.Ldc_I4_S && code.operand.Equals((sbyte)-20));
        codes[index] = new CodeInstruction(OpCodes.Call, GetMethod(nameof(GetDailyGreetingModifyForSpouse)));
        index = codes.FindIndex(index, code => code.opcode == OpCodes.Ldc_I4_S && code.operand.Equals((sbyte)-8));
        codes[index] = new CodeInstruction(OpCodes.Call, GetMethod(nameof(GetDailyGreetingModifyForDatingVillager)));
        index = codes.FindIndex(index, code => code.opcode == OpCodes.Ldc_I4_S && code.operand.Equals((sbyte)-2));
        codes[index] = new CodeInstruction(OpCodes.Call, GetMethod(nameof(GetDailyGreetingModifyForVillager)));

        return codes.AsEnumerable();
    }

    private static int GetDailyGreetingModifyForVillager()
    {
        return -config.DailyGreetingModifyForVillager;
    }

    private static int GetDailyGreetingModifyForDatingVillager()
    {
        return -config.DailyGreetingModifyForDatingVillager;
    }

    private static int GetDailyGreetingModifyForSpouse()
    {
        return -config.DailyGreetingModifyForSpouse;
    }

    private static MethodInfo GetMethod(string name)
    {
        return AccessTools.Method(typeof(FarmerPatcher), name) ??
               throw new InvalidOperationException($"Can't find method {GetMethodString(typeof(FarmerPatcher), name)}.");
    }
}