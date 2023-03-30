/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace AtraShared.Utils.HarmonyHelper;

/// <summary>
/// Thrown when IL codes are expected but not matched.
/// </summary>
public class NoMatchFoundException : InvalidOperationException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NoMatchFoundException"/> class.
    /// </summary>
    public NoMatchFoundException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NoMatchFoundException"/> class.
    /// </summary>
    /// <param name="message">Message to include.</param>
    public NoMatchFoundException(string? message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NoMatchFoundException"/> class.
    /// </summary>
    /// <param name="instructions">Instructiosn searched for.</param>
    public NoMatchFoundException(IEnumerable<CodeInstructionWrapper> instructions)
        : base($"The desired pattern wasn't found:\n\n" + string.Join('\n', instructions.Select(i => i.ToString())))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NoMatchFoundException"/> class.
    /// </summary>
    /// <param name="message">Message to include.</param>
    /// <param name="innerException">Inner exception.</param>
    public NoMatchFoundException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}

/// <summary>
/// Throw helper for the ILHelper.
/// </summary>
public static partial class ILHelperThrowHelper
{
#if NET6_0_OR_GREATER
    [StaticTraceHidden]
#endif
    [DoesNotReturn]
    [DebuggerHidden]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowNoMatchFoundException()
    {
        throw new NoMatchFoundException();
    }

#if NET6_0_OR_GREATER
    [StaticTraceHidden]
#endif
    [DoesNotReturn]
    [DebuggerHidden]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowNoMatchFoundException(string? message)
    {
        throw new NoMatchFoundException(message);
    }

#if NET6_0_OR_GREATER
    [StaticTraceHidden]
#endif
    [DoesNotReturn]
    [DebuggerHidden]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowNoMatchFoundException(string? message, Exception? inner)
    {
        throw new NoMatchFoundException(message, inner);
    }

#if NET6_0_OR_GREATER
    [StaticTraceHidden]
#endif
    [DoesNotReturn]
    [DebuggerHidden]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowNoMatchFoundException(IEnumerable<CodeInstructionWrapper> codeInstructions)
    {
        throw new NoMatchFoundException(codeInstructions);
    }

#if NET6_0_OR_GREATER
    [StaticTraceHidden]
#endif
    [DoesNotReturn]
    [DebuggerHidden]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static T ThrowNoMatchFoundException<T>()
    {
        throw new NoMatchFoundException();
    }

#if NET6_0_OR_GREATER
    [StaticTraceHidden]
#endif
    [DoesNotReturn]
    [DebuggerHidden]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static T ThrowNoMatchFoundException<T>(string? message)
    {
        throw new NoMatchFoundException(message);
    }

#if NET6_0_OR_GREATER
    [StaticTraceHidden]
#endif
    [DoesNotReturn]
    [DebuggerHidden]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static T ThrowNoMatchFoundException<T>(string? message, Exception? inner)
    {
        throw new NoMatchFoundException(message, inner);
    }

#if NET6_0_OR_GREATER
    [StaticTraceHidden]
#endif
    [DoesNotReturn]
    [DebuggerHidden]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static T ThrowNoMatchFoundException<T>(IEnumerable<CodeInstructionWrapper> codeInstructions)
    {
        throw new NoMatchFoundException(codeInstructions);
    }
}