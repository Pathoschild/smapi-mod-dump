/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.Automate;

#region using directives

using DaLion.Common.Data;
using DaLion.Common.Extensions.Reflection;
using Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using System;
using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal sealed class GeodeCrusherMachineSetInputPatch : DaLion.Common.Harmony.HarmonyPatch
{
    private static Func<object, SObject>? _GetMachine;

    /// <summary>Construct an instance.</summary>
    internal GeodeCrusherMachineSetInputPatch()
    {
        try
        {
            Target = "Pathoschild.Stardew.Automate.Framework.Machines.Objects.GeodeCrusherMachine".ToType()
                .RequireMethod("SetInput");
        }
        catch
        {
            // ignored
        }
    }

    #region harmony patches

    /// <summary>Patch to apply Gemologist effects to automated Geode Crusher.</summary>
    [HarmonyPostfix]
    private static void GeodeCrusherMachineSetInputPostfix(object __instance)
    {
        _GetMachine ??= __instance.GetType().RequirePropertyGetter("Machine")
            .CompileUnboundDelegate<Func<object, SObject>>();
        var machine = _GetMachine(__instance);
        if (machine.heldObject.Value is null) return;

        var owner = Game1.getFarmerMaybeOffline(machine.owner.Value) ?? Game1.MasterPlayer;
        if (!owner.HasProfession(Profession.Gemologist) ||
            !machine.heldObject.Value.IsForagedMineral() && !machine.heldObject.Value.IsGemOrMineral()) return;

        machine.heldObject.Value.Quality = owner.GetGemologistMineralQuality();
        if (!ModEntry.Config.ShouldCountAutomatedHarvests) return;

        ModDataIO.IncrementData<uint>(owner, ModData.GemologistMineralsCollected.ToString());
    }

    #endregion harmony patches
}