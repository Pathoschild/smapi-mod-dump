/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using System.Reflection.Emit;
using Common.Patch;
using FriendshipDecayModify.Framework;
using HarmonyLib;
using StardewValley;

namespace FriendshipDecayModify.Patches;

internal class GameLocationPatcher : BasePatcher
{
    private static ModConfig config = null!;

    public GameLocationPatcher(ModConfig config)
    {
        GameLocationPatcher.config = config;
    }

    public override void Apply(Harmony harmony)
    {
        harmony.Patch(
            RequireMethod<GameLocation>(nameof(GameLocation.CheckGarbage)),
            transpiler: GetHarmonyMethod(nameof(CheckGarbageTranspiler))
        );
    }

    // 垃圾桶修改
    private static IEnumerable<CodeInstruction> CheckGarbageTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var codes = instructions.ToList();

        var index = codes.FindIndex(code => code.opcode == OpCodes.Callvirt && code.operand.Equals(AccessTools.Method(typeof(Farmer), nameof(Farmer.changeFriendship))));
        codes.Insert(index - 1, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(GameLocationPatcher), nameof(GetGarbageCanModify))));

        return codes.AsEnumerable();
    }

    private static int GetGarbageCanModify(int friendshipChange)
    {
        return friendshipChange >= 0 ? friendshipChange : -config.GarbageCanModify;
    }
}