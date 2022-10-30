/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-challenger
**
*************************************************/

using System.Runtime.CompilerServices;
using HarmonyLib;
using Netcode;
using Slothsoft.Challenger.Models;
using StardewValley.Network;

namespace Slothsoft.Challenger.Objects;

// ReSharper disable InconsistentNaming
internal static class MagicalWorldState {
    private class Holder {
        protected internal readonly NetRef<ChallengerState> Value = new(new ChallengerState());
    }

    private static readonly ConditionalWeakTable<IWorldState, Holder> Values = new();

    internal static void PatchObject(Harmony harmony) {
        harmony.Patch(
            original: AccessTools.Constructor(
                typeof(NetWorldState)
            ),
            postfix: new HarmonyMethod(typeof(MagicalWorldState), nameof(AddNetFields))
        );
    }

    private static void AddNetFields(NetWorldState __instance) {
        __instance.NetFields.AddFields(
            __instance.GetNetRefChallengerState()
        );
    }

    public static ChallengerState GetChallengerState(this IWorldState worldState) => worldState.GetNetRefChallengerState().Value;
    private static NetRef<ChallengerState> GetNetRefChallengerState(this IWorldState worldState) => Values.GetOrCreateValue(worldState).Value;
}