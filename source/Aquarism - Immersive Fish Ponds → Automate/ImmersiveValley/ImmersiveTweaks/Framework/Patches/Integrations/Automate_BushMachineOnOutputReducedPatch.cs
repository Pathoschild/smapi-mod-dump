/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Tweex.Framework.Patches;

#region using directives

using Common.Extensions.Reflection;
using Common.Harmony;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;

#endregion using directives

[UsedImplicitly]
internal sealed class BushMachineOnOutputReducedPatch : HarmonyPatch
{
    private static Func<object, Bush>? _GetMachine;

    /// <summary>Construct an instance.</summary>
    internal BushMachineOnOutputReducedPatch()
    {
        try
        {
            Target = "Pathoschild.Stardew.Automate.Framework.Machines.TerrainFeatures.BushMachine".ToType()
                .RequireMethod("OnOutputReduced");
        }
        catch
        {
            // ignored
        }
    }

    #region harmony patches

    /// <summary>Adds foraging experience for automated berry bushes.</summary>
    private static void BushMachineOnOutputReducedPostfix(object __instance)
    {
        if (!ModEntry.Config.BerryBushesRewardExp) return;

        _GetMachine ??= __instance.GetType().RequirePropertyGetter("Machine")
            .CompileUnboundDelegate<Func<object, Bush>>();
        var machine = _GetMachine(__instance);
        if (machine.size.Value >= Bush.greenTeaBush) return;

        Game1.MasterPlayer.gainExperience(Farmer.foragingSkill, 3);
    }

    #endregion harmony patches
}