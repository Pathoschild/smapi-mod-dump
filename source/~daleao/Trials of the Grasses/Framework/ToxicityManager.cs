/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Alchemy.Framework;

#region using directives

using Events.Toxicity;
using StardewValley;
using System;

#endregion using directives

internal class ToxicityManager
{
    public const int BASE_TOLERANCE_I = 100;

    public static int MaxTolerance { get; }

    public static int OverdoseThreshold { get; }

    private static int _ToxicityValueValue;

    public static int ToxicityValue
    {
        get => _ToxicityValueValue;
        set => _ToxicityValueValue = value;
    }

    #region event handlers

    /// <inheritdoc cref="OnChanged"/>
    internal static event EventHandler<IToxicityChangedEventArgs>? Changed;

    /// <inheritdoc cref="OnCleared"/>
    internal static event EventHandler<IToxicityClearedEventArgs>? Cleared;

    /// <inheritdoc cref="OnFilled"/>
    internal static event EventHandler<IToxicityFilledEventArgs>? Filled;

    /// <inheritdoc cref="OnOverdosed"/>
    internal static event EventHandler<IPlayerOverdosedEventArgs>? Overdosed;

    #endregion event handlers

    #region event callbacks

    /// <summary>Raised when a player's Toxicity value changes.</summary>
    /// <param name="oldValue">The old toxicity value.</param>
    /// <param name="newValue">The old charge value.</param>
    protected void OnChanged(double oldValue, double newValue)
    {
        Changed?.Invoke(this, new ToxicityChangedEventArgs(Game1.player, oldValue, newValue));
    }

    /// <summary>Raised when a player's Toxicity value drops back to zero.</summary>
    protected void OnCleared()
    {
        Cleared?.Invoke(this, new ToxicityClearedEventArgs(Game1.player));
    }

    /// <summary>Raised when a player's Toxicity value reaches the maximum value.</summary>
    protected void OnFilled()
    {
        Filled?.Invoke(this, new ToxicityFilledEventArgs(Game1.player));
    }

    /// <summary>Raised when a player's Toxicity value crosses the overdose threshold.</summary>
    protected void OnOverdosed()
    {
        Overdosed?.Invoke(this, new PlayerOverdosedEventArgs(Game1.player));
    }

    #endregion event callbacks
}