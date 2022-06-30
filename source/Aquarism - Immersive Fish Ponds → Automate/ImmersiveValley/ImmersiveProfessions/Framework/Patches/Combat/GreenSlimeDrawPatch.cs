/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using DaLion.Common;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#endregion using directives

[UsedImplicitly]
internal sealed class GreenSlimeDrawPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.<w/ summary>
    internal GreenSlimeDrawPatch()
    {
        //Target = RequireMethod<GreenSlime>(nameof(GreenSlime.draw), new[] {typeof(SpriteBatch)});
    }

    #region harmony patches

    /// <summary>Patch to fix Green Slime eye and antenna position when inflated.</summary>
    private static IEnumerable<CodeInstruction>? GreenSlimeDrawTranspiler(IEnumerable<CodeInstruction> instructions,
        ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Injected: antenna position += GetAntennaOffset(this)
        ///			  eyes position += GetEyesOffset(this)

        var drawInstructions = new CodeInstruction[]
        {
            new(OpCodes.Ldarg_1),
            new(OpCodes.Ldarg_0),
            new(OpCodes.Callvirt,
                typeof(Character).RequirePropertyGetter(nameof(Character.Sprite))),
            new(OpCodes.Callvirt,
                typeof(AnimatedSprite).RequirePropertyGetter(nameof(AnimatedSprite.Texture))),
            new(OpCodes.Ldarg_0),
            new(OpCodes.Ldsfld, typeof(Game1).RequireField(nameof(Game1.viewport))),
            new(OpCodes.Call,
                typeof(Character).RequireMethod(nameof(Character.getLocalPosition)))
        };

        try
        {
            helper
                .FindFirst(drawInstructions) // the main sprite draw call
                .FindNext(drawInstructions) // find antenna draw call
                .AdvanceUntil( // advance until end of position argument
                    new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[5])
                )
                .Retreat()
                .GetInstructions(out var got, advance: true) // copy vector addition instruction
                .Insert( // insert custom offset
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call,
                        typeof(GreenSlimeDrawPatch).RequireMethod(nameof(GetAntennaeOffset)))
                )
                .Insert(got) // insert addition
                .FindNext(drawInstructions) // find eyes draw call
                .AdvanceUntil( // advance until end of position argument
                    new CodeInstruction(OpCodes.Ldc_I4_S, 32)
                )
                .Insert( // insert custom offset
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call,
                        typeof(GreenSlimeDrawPatch).RequireMethod(nameof(GetEyesOffset)))
                )
                .Insert(got); // insert addition
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching inflated Green Slime sprite.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static Vector2 GetAntennaeOffset(GreenSlime slime)
    {
        if (slime.Scale <= 1f) return Vector2.Zero;

        var x = MathHelper.Lerp(0, -32f, slime.Scale - 1f);
        var y = MathHelper.Lerp(0, -64f, slime.Scale - 1f);
        return new(x, y);
    }

    private static Vector2 GetEyesOffset(GreenSlime slime)
    {
        if (slime.Scale <= 1f) return Vector2.Zero;

        var x = MathHelper.Lerp(0, -32f, slime.Scale - 1f);
        var y = MathHelper.Lerp(0, -32f, slime.Scale - 1f);
        return new(x, y);
    }

    #endregion injected subroutines
}