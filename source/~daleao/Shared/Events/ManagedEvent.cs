/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Shared.Events;

#region using directives

using System.Runtime.CompilerServices;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Reflection;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>Base implementation of an event wrapper allowing dynamic enabling / disabling.</summary>
public abstract class ManagedEvent : IManagedEvent, IComparable<ManagedEvent>, IEquatable<ManagedEvent>
{
    private readonly PerScreen<bool> _enabled = new(() => false);
    private readonly bool _alwaysEnabled;

    /// <summary>Initializes a new instance of the <see cref="ManagedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    protected ManagedEvent(EventManager manager)
    {
        this.Manager = manager;
        if (this.GetType().HasAttribute<AlwaysEnabledEventAttribute>())
        {
            this._alwaysEnabled = true;
        }
    }

    /// <inheritdoc />
    /// <remarks>Overriding this property may prevent <see cref="Enable"/> and <see cref="Disable"/> from doing anything.</remarks>
    public virtual bool IsEnabled => this._enabled.Value || this._alwaysEnabled;

    /// <summary>Gets the <see cref="EventManager"/> instance that manages this event.</summary>
    protected EventManager Manager { get; }

    /// <summary>Compares whether two <see cref="ManagedEvent" /> instances are equal.</summary>
    /// <param name="left"><see cref="ManagedEvent" /> instance on the left of the equal sign.</param>
    /// <param name="right"><see cref="ManagedEvent" /> instance on the right of the equal sign.</param>
    /// <returns><see langword="true"/> if the instances are equal; <see langword="false"/> otherwise.</returns>
    public static bool operator ==(ManagedEvent? left, ManagedEvent? right) => left?.Equals(right) ?? right is null;

    /// <summary>Compares whether two <see cref="ManagedEvent" /> instances are not equal.</summary>
    /// <param name="left"><see cref="ManagedEvent" /> instance on the left of the not equal sign.</param>
    /// <param name="right"><see cref="ManagedEvent" /> instance on the right of the not equal sign.</param>
    /// <returns><see langword="true"/> if the instances are not equal; <see langword="false"/> otherwise.</returns>
    public static bool operator !=(ManagedEvent? left, ManagedEvent? right) => !(left == right);

    /// <inheritdoc />
    public bool IsEnabledForScreen(int screenId)
    {
        return this._enabled.GetValueForScreen(screenId);
    }

    /// <inheritdoc />
    public bool Enable()
    {
        if (this._enabled.Value || !(this._enabled.Value = true))
        {
            return false;
        }

        this.OnEnabled();
        return this._enabled.Value;
    }

    /// <inheritdoc />
    /// <remarks>This will not invoke the <see cref="OnEnabled"/> callback.</remarks>
    public bool EnableForScreen(int screenId)
    {
        if (!Context.IsMainPlayer || !Context.IsSplitScreen)
        {
            return false;
        }

        if (this._enabled.GetValueForScreen(screenId))
        {
            return false;
        }

        this._enabled.SetValueForScreen(screenId, true);
        return true;
    }

    /// <inheritdoc />
    /// <remarks>This will not invoke the <see cref="OnEnabled"/> callback.</remarks>
    public void EnableForAllScreens()
    {
        this._enabled.GetActiveValues().ForEach(pair => this._enabled.SetValueForScreen(pair.Key, true));
    }

    /// <inheritdoc />
    public bool Disable()
    {
        if (!this._enabled.Value || (this._enabled.Value = false))
        {
            return false;
        }

        this.OnDisabled();
        return !this._enabled.Value;
    }

    /// <inheritdoc />
    /// <remarks>This will not invoke the <see cref="OnDisabled"/> callback.</remarks>
    public bool DisableForScreen(int screenId)
    {
        if (!Context.IsMainPlayer || !Context.IsSplitScreen)
        {
            return false;
        }

        if (!this._enabled.GetValueForScreen(screenId))
        {
            return false;
        }

        this._enabled.SetValueForScreen(screenId, false);
        return true;
    }

    /// <inheritdoc />
    /// <remarks>This will not invoke the <see cref="OnDisabled"/> callback.</remarks>
    public void DisableForAllScreens()
    {
        this._enabled.GetActiveValues().ForEach(pair => this._enabled.SetValueForScreen(pair.Key, false));
    }

    /// <inheritdoc />
    public void Reset()
    {
        this._enabled.Value = false;
    }

    /// <inheritdoc />
    public void ResetForAllScreens()
    {
        this._enabled.ResetAllScreens();
    }

    /// <inheritdoc />
    public void Unmanage()
    {
        this.Manager.Unmanage(this);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return this.GetType().Name;
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode()
    {
        return this.GetType().GetHashCode();
    }

    /// <inheritdoc />
    public int CompareTo(ManagedEvent? other)
    {
        return string.Compare(this.GetType().Name, other?.GetType().Name, StringComparison.Ordinal);
    }

    /// <summary>Determines whether two <see cref="ManagedEvent"/> instances are equal.</summary>
    /// <param name="other">A <see cref="ManagedEvent"/> to compare to this instance.</param>
    /// <returns>
    ///     <see langword="true"/> if <paramref name="other"/> has the same type as this instance, otherwise
    ///     <see langword="false"/>.
    /// </returns>
    public virtual bool Equals(ManagedEvent? other)
    {
        // ReSharper disable once CheckForReferenceEqualityInstead.1
        return this.GetType().Equals(other?.GetType());
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is ManagedEvent other && this.Equals(other);
    }

    /// <inheritdoc />
    public abstract void Dispose();

    /// <summary>Invoked once when the event is enabled.</summary>
    protected virtual void OnEnabled()
    {
    }

    /// <summary>Invoked once when the event is disabled.</summary>
    protected virtual void OnDisabled()
    {
    }
}
