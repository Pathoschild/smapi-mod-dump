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

internal class FarmAnimalPatcher : BasePatcher
{
    private static ModConfig config = null!;
    private static int friendshipTowardFarmer;

    public FarmAnimalPatcher(ModConfig config)
    {
        FarmAnimalPatcher.config = config;
    }

    public override void Apply(Harmony harmony)
    {
        harmony.Patch(this.RequireMethod<FarmAnimal>(nameof(FarmAnimal.dayUpdate)), this.GetHarmonyMethod(nameof(DayUpdatePrefix)),
            transpiler: this.GetHarmonyMethod(nameof(DayUpdateTranspiler))
        );
    }

    private static bool DayUpdatePrefix(FarmAnimal __instance)
    {
        friendshipTowardFarmer = __instance.friendshipTowardFarmer.Value;
        return true;
    }

    private static IEnumerable<CodeInstruction> DayUpdateTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var codes = instructions.ToList();

        // 抚摸动物友谊修改
        var index = codes.FindIndex(code => code.opcode == OpCodes.Ldc_I4_S && code.operand.Equals((sbyte)10));
        codes[index] = new CodeInstruction(OpCodes.Call, GetMethod(nameof(GetPetAnimalModifyForFriendship)));
        // 抚摸动物心情修改
        index = codes.FindIndex(index, code => code.opcode == OpCodes.Ldc_I4_S && code.operand.Equals((sbyte)50));
        codes[index] = new CodeInstruction(OpCodes.Call, GetMethod(nameof(GetPetAnimalModifyForHappiness)));
        // 喂食动物心情修改
        index = codes.FindIndex(index, code => code.opcode == OpCodes.Ldc_I4_S && code.operand.Equals((sbyte)100));
        codes[index] = new CodeInstruction(OpCodes.Call, GetMethod(nameof(GetFeedAnimalModifyForHappiness)));
        // 喂食动物友谊修改
        index = codes.FindIndex(index, code => code.opcode == OpCodes.Ldc_I4_S && code.operand.Equals((sbyte)20));
        codes[index] = new CodeInstruction(OpCodes.Call, GetMethod(nameof(GetFeedAnimalModifyForFriendship)));

        return codes.AsEnumerable();
    }

    // 抚摸动物友谊修改
    private static int GetPetAnimalModifyForFriendship()
    {
        var petAnimalDecay = config.PetAnimalModifyForFriendship - friendshipTowardFarmer / 200;
        return petAnimalDecay < 0 ? petAnimalDecay : config.PetAnimalModifyForFriendship;
    }

    // 抚摸动物心情修改
    private static int GetPetAnimalModifyForHappiness()
    {
        return config.PetAnimalModifyForHappiness;
    }

    // 喂食动物友谊修改
    private static int GetFeedAnimalModifyForFriendship()
    {
        return config.FeedAnimalModifyForFriendship;
    }

    // 喂食动物心情修改
    private static int GetFeedAnimalModifyForHappiness()
    {
        return config.FeedAnimalModifyForHappiness;
    }

    private static MethodInfo GetMethod(string name)
    {
        return AccessTools.Method(typeof(FarmAnimalPatcher), name) ??
               throw new InvalidOperationException($"Can't find method {GetMethodString(typeof(FarmAnimalPatcher), name)}.");
    }
}