/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Patchers.Combat;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponSetFarmerAnimatingPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponSetFarmerAnimatingPatcher"/> class.</summary>
    internal MeleeWeaponSetFarmerAnimatingPatcher()
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.setFarmerAnimating));
    }

    #region harmony patches

    /// <summary>Patch to increase prestiged Brute attack speed with rage.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? MeleeWeaponSetFarmerAnimatingTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Injected: if (who.professions.Contains(100 + <brute_id>) swipeSpeed *= 1f - who.Get_BruteRageCounter() * 0.005f;
        // After: if (who.IsLocalPlayer)
        try
        {
            var skipRageBonus = generator.DefineLabel();
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Farmer).RequirePropertyGetter(nameof(Farmer.IsLocalPlayer))),
                    })
                .Match(new[] { new CodeInstruction(OpCodes.Ldarg_0) })
                .AddLabels(skipRageBonus)
                .Insert(new[] { new CodeInstruction(OpCodes.Ldarg_1) }) // arg 1 = Farmer who
                .InsertProfessionCheck(Profession.Brute.Value + 100, forLocalPlayer: false)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Brfalse_S, skipRageBonus),
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, typeof(MeleeWeapon).RequireField("swipeSpeed")),
                        new CodeInstruction(OpCodes.Ldc_R4, 1f),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.State))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ModState).RequirePropertyGetter(nameof(ModState.Professions))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(State).RequirePropertyGetter(nameof(State.BruteRageCounter))),
                        new CodeInstruction(OpCodes.Conv_R4),
                        new CodeInstruction(OpCodes.Ldc_R4, 0.005f),
                        new CodeInstruction(OpCodes.Mul),
                        new CodeInstruction(OpCodes.Sub),
                        new CodeInstruction(OpCodes.Mul),
                        new CodeInstruction(OpCodes.Stfld, typeof(MeleeWeapon).RequireField("swipeSpeed")),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding attack speed to prestiged Brute.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
