/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Common.Events;

#region using directives

using System;
using System.Runtime.CompilerServices;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>Base implementation for an event wrapper allowing dynamic hooking / unhooking.</summary>
internal abstract class ManagedEvent : IManagedEvent, IEquatable<ManagedEvent>
{
    protected readonly PerScreen<bool> hooked = new();

    /// <inheritdoc />
    public bool IsHooked => hooked.Value;

    /// <inheritdoc />
    public bool IsHookedForScreen(int screenID)
    {
        return hooked.GetValueForScreen(screenID);
    }

    /// <inheritdoc />
    public void Hook()
    {
        hooked.Value = true;
    }

    /// <inheritdoc />
    public void Unhook()
    {
        hooked.Value = false;
    }

    /// <inheritdoc />
    public override string ToString() => GetType().Name;

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => GetType().GetHashCode();

    /// <summary>Determines if the specified <see cref="ManagedEvent"/> is equal to the current instance.</summary>
    /// <param name="other">A <see cref="ManagedEvent"/> to compare to this instance.</param>
    /// <returns><see langword="true"/> if <paramref name="other"/> has the same type as this instance; otherwise, <see langword="false">.</returns>
    public virtual bool Equals(ManagedEvent? other)
    {
        // ReSharper disable once CheckForReferenceEqualityInstead.1
        return GetType().Equals(other?.GetType());
    }

    /// <inheritdoc />
    public override bool Equals(object? @object)
    {
        return @object is ManagedEvent other && Equals(other);
    }

    public static bool operator ==(ManagedEvent? left, ManagedEvent? right) =>
        (object?) left == null ? (object?) right == null : left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(ManagedEvent? left, ManagedEvent? right) => !(left == right);
}