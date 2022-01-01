/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches;

[UsedImplicitly]
internal class BobberBarUpdatePatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal BobberBarUpdatePatch()
    {
        Original = RequireMethod<BobberBar>(nameof(BobberBar.update));
    }

    #region harmony patches

    /// <summary>Patch to slow-down catching bar decrease for Aquarist.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> BobberBarUpdateTranspiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Injected: if (Game1.player.professions.Contains(<aquarist_id>) distanceFromCatching -= GetAquaristBonusCatchingBarSpeed();
        /// After: distanceFromCatching -= ((whichBobber == 694 || beginnersRod) ? 0.002f : 0.003f);

        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldc_I4, 694)
                )
                .RetreatUntil(
                    new CodeInstruction(OpCodes.Brfalse_S)
                )
                .GetOperand(out var resumeExecution)
                .Return()
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Stfld)
                )
                .Advance()
                .InsertProfessionCheckForLocalPlayer(Utility.Professions.IndexOf("Aquarist"), (Label) resumeExecution)
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, typeof(BobberBar).Field("distanceFromCatching")),
                    new CodeInstruction(OpCodes.Call, typeof(Game1).PropertyGetter(nameof(Game1.player))),
                    new CodeInstruction(OpCodes.Call,
                        typeof(Utility.Professions).MethodNamed(nameof(Utility.Professions
                            .GetAquaristBonusCatchingBarSpeed))),
                    new CodeInstruction(OpCodes.Sub),
                    new CodeInstruction(OpCodes.Stfld, typeof(BobberBar).Field("distanceFromCatching"))
                );
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed while patching Aquarist catching bar speed. Helper returned {ex}", LogLevel.Error);
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}