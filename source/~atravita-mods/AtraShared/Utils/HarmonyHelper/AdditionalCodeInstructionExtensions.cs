/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using HarmonyLib;

namespace AtraShared.Utils.HarmonyHelper;

/// <summary>
/// ILHelper was, for some reason, used incorrectly.
/// </summary>
public class InvalidILHelperCommand : InvalidOperationException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidILHelperCommand"/> class.
    /// </summary>
    /// <param name="text">The text to show with the error.</param>
    public InvalidILHelperCommand(string text)
        : base(text)
    {
    }
}

/// <summary>
/// Throw helper for the ILHelper.
/// </summary>
public static partial class ILHelperThrowHelper
{
    /// <summary>
    /// Throws an error that means the ILHelper was used incorrectly.
    /// </summary>
    /// <param name="text">text to include.</param>
    /// <exception cref="InvalidILHelperCommand">ILHelper used incorrectly.</exception>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowInvalidILHelperCommand(string text)
    {
        throw new InvalidILHelperCommand(text);
    }

    /// <summary>
    /// Throws an error that means the ILHelper was used incorrectly.
    /// </summary>
    /// <typeparam name="T">A fake return type.</typeparam>
    /// <param name="text">text to include.</param>
    /// <returns>nothing.</returns>
    /// <exception cref="InvalidILHelperCommand">ILHelper used incorrectly.</exception>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static T ThrowInvalidILHelperCommand<T>(string text)
    {
        throw new InvalidILHelperCommand(text);
    }
}

/// <summary>
/// Code instruction extensions on top of Harmony's included ones.
/// </summary>
public static class AdditionalCodeInstructionExtensions
{
    /// <summary>
    /// Converts an instruction to the matching load local instruction.
    /// </summary>
    /// <param name="instruction">Instruction to convert.</param>
    /// <returns>Ldloc command.</returns>
    /// <exception cref="InvalidILHelperCommand">Could not convert to the ldloc.</exception>
    public static CodeInstruction ToLdLoc(this CodeInstruction instruction)
    {
        OpCode code = instruction.opcode;
        if (code == OpCodes.Ldloc_0 || code == OpCodes.Stloc_0)
        {
            return new CodeInstruction(OpCodes.Ldloc_0);
        }
        else if (code == OpCodes.Ldloc_1 || code == OpCodes.Stloc_1)
        {
            return new CodeInstruction(OpCodes.Ldloc_1);
        }
        else if (code == OpCodes.Ldloc_2 || code == OpCodes.Stloc_2)
        {
            return new CodeInstruction(OpCodes.Ldloc_2);
        }
        else if (code == OpCodes.Ldloc_3 || code == OpCodes.Stloc_3)
        {
            return new CodeInstruction(OpCodes.Ldloc_3);
        }
        else if (code == OpCodes.Ldloc || code == OpCodes.Stloc)
        {
            return new CodeInstruction(OpCodes.Ldloc, instruction.operand);
        }
        else if (code == OpCodes.Ldloc_S || code == OpCodes.Stloc_S)
        {
            return new CodeInstruction(OpCodes.Ldloc_S, instruction.operand);
        }
        else if (code == OpCodes.Ldloca || code == OpCodes.Ldloca_S)
        {
            return instruction.Clone();
        }
        return ILHelperThrowHelper.ThrowInvalidILHelperCommand<CodeInstruction>($"Could not make ldloc from {instruction}");
    }

    /// <summary>
    /// Converts an instruction to the matching store local instruction.
    /// </summary>
    /// <param name="instruction">Instruction to convert.</param>
    /// <returns>Stloc command.</returns>
    /// <exception cref="InvalidILHelperCommand">Could not convert to the ldloc.</exception>
    public static CodeInstruction ToStLoc(this CodeInstruction instruction)
    {
        OpCode code = instruction.opcode;
        if (code == OpCodes.Ldloc_0 || code == OpCodes.Stloc_0)
        {
            return new CodeInstruction(OpCodes.Stloc_0);
        }
        else if (code == OpCodes.Ldloc_1 || code == OpCodes.Stloc_1)
        {
            return new CodeInstruction(OpCodes.Stloc_1);
        }
        else if (code == OpCodes.Ldloc_2 || code == OpCodes.Stloc_2)
        {
            return new CodeInstruction(OpCodes.Stloc_2);
        }
        else if (code == OpCodes.Ldloc_3 || code == OpCodes.Stloc_3)
        {
            return new CodeInstruction(OpCodes.Stloc_3);
        }
        else if (code == OpCodes.Ldloc || code == OpCodes.Stloc)
        {
            return new CodeInstruction(OpCodes.Stloc, instruction.operand);
        }
        else if (code == OpCodes.Ldloc_S || code == OpCodes.Stloc_S)
        {
            return new CodeInstruction(OpCodes.Stloc_S, instruction.operand);
        }
        return ILHelperThrowHelper.ThrowInvalidILHelperCommand<CodeInstruction>($"Could not make stloc from {instruction}");
    }
}