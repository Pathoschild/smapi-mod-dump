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
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class DuggyBehaviorAtGameTickPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="DuggyBehaviorAtGameTickPatcher"/> class.</summary>
    internal DuggyBehaviorAtGameTickPatcher()
    {
        this.Target = this.RequireMethod<Duggy>(nameof(Duggy.behaviorAtGameTick));
    }

    #region harmony patches

    /// <summary>Patch to hide Poacher from Duggies during Ultimate.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? DuggyBehaviorAtGameTickTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: if (Sprite.currentFrame < 4)
        // To: if (Sprite.currentFrame < 4 && !player.IsInAmbush())
        try
        {
            helper
                .Match(new[] { new CodeInstruction(OpCodes.Ldc_I4_4) })
                .Move()
                .GetOperand(out var dontDoDamage)
                .Move()
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Monster).RequirePropertyGetter(nameof(Monster.Player))),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(FarmerExtensions).RequireMethod(nameof(FarmerExtensions.IsInAmbush))),
                        new CodeInstruction(OpCodes.Brtrue, dontDoDamage),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed hiding ambushing Poacher from Duggies.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
