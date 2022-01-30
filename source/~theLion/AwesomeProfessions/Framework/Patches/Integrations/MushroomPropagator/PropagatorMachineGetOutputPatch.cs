/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Integrations;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;

using Stardew.Common.Harmony;
using Extensions;

using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal class PropagatorMachineGetOutputPatch : BasePatch
{
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

        var entity = ModEntry.ModHelper.Reflection.GetProperty<SObject>(__instance, "Entity").GetValue();
        if (entity is null) return;

        var who = Game1.getFarmerMaybeOffline(entity.owner.Value) ?? Game1.MasterPlayer;
        if (!who.HasProfession(Profession.Ecologist)) return;

        if (who.IsLocalPlayer && !ModEntry.Config.ShouldCountAutomatedHarvests)
            ModData.Increment(DataField.EcologistItemsForaged, -1);
        else if (ModEntry.Config.ShouldCountAutomatedHarvests)
            ModData.Increment<uint>(DataField.EcologistItemsForaged, who);
    }

    #endregion harmony patches
}