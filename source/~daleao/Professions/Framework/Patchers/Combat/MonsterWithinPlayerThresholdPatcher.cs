/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using System.Reflection;
using DaLion.Core.Framework.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class MonsterWithinPlayerThresholdPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MonsterWithinPlayerThresholdPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal MonsterWithinPlayerThresholdPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<Monster>(nameof(Monster.withinPlayerThreshold), []);
    }

    #region harmony patch

    /// <summary>Patch to make Poacher invisible in LimitBreak.</summary>
    [HarmonyPrefix]
    private static bool MonsterWithinPlayerThresholdPrefix(Monster __instance, ref bool __result)
    {
        if (__instance is Ghost)
        {
            return true; // run original logic
        }

        try
        {
            if (!__instance.Get_Target().IsAmbushing())
            {
                return true; // run original method
            }

            __result = false;
            return false; // don't run original method
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patch
}
