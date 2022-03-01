/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using System.Reflection;

namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.MushroomPropagator;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;

using Stardew.Common.Extensions;
using Extensions;

using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal class PropagatorMachineGetOutputPatch : BasePatch
{
    private static MethodInfo _GetEntity;

    /// <summary>Construct an instance.</summary>
    internal PropagatorMachineGetOutputPatch()
    {
        try
        {
            Original = "BlueberryMushroomAutomation.PropagatorMachine".ToType()
                .MethodNamed("GetOutput");
        }
        catch
        {
            // ignored
        }
    }

    #region harmony patches

    /// <summary>Patch for automated Propagator forage decrement.</summary>
    [HarmonyPostfix]
    private static void PropagatorMachineGetOutputPostfix(object __instance)
    {
        if (__instance is null) return;

        _GetEntity ??= __instance.GetType().PropertyGetter("Entity");
        var entity = (SObject) _GetEntity.Invoke(__instance, null);
        if (entity is null) return;

        var owner = Game1.getFarmerMaybeOffline(entity.owner.Value) ?? Game1.MasterPlayer;
        if (!owner.HasProfession(Profession.Ecologist)) return;

        if (owner.IsLocalPlayer && !ModEntry.Config.ShouldCountAutomatedHarvests)
            Game1.player.IncrementData(DataField.EcologistItemsForaged, -1);
        else if (ModEntry.Config.ShouldCountAutomatedHarvests)
            owner.IncrementData<uint>(DataField.EcologistItemsForaged);
    }

    #endregion harmony patches
}