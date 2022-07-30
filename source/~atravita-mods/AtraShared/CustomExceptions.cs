/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Runtime.CompilerServices;

namespace AtraShared;

/// <summary>
/// Thrown when a save is not loaded but I expect one to be.
/// </summary>
public class SaveNotLoadedError : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SaveNotLoadedError"/> class.
    /// </summary>
    public SaveNotLoadedError()
        : base("Save not loaded")
    {
    }
}

/// <summary>
/// ThrowHelper for AtraShared exceptions.
/// </summary>
public static class ASThrowHelper
{
    /// <summary>
    /// Throws a new SaveNotLoadedError.
    /// </summary>
    /// <exception cref="SaveNotLoadedError">always.</exception>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowSaveNotLoaded()
    {
        throw new SaveNotLoadedError();
    }

    /// <summary>
    /// Throws a new SaveNotLoadedError.
    /// </summary>
    /// <exception cref="SaveNotLoadedError">always.</exception>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static T ThrowSaveNotLoaded<T>()
    {
        throw new SaveNotLoadedError();
    }
}