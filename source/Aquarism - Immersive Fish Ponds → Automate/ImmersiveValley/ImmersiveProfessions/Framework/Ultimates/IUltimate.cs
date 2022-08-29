/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

using System;

namespace DaLion.Stardew.Professions.Framework.Ultimates;

/// <summary>Interface for Ultimate abilities.</summary>
public interface IUltimate : IDisposable
{
    /// <summary>The index of this Ultimate, which equals the index of the corresponding combat profession.</summary>
    UltimateIndex Index { get; }

    /// <summary>Whether this Ultimate is currently active.</summary>
    bool IsActive { get; }

    /// <summary>The current charge value.</summary>
    double ChargeValue { get; set; }

    /// <summary>The maximum charge value.</summary>
    int MaxValue { get; }

    /// <summary>Check whether all activation conditions for this Ultimate are currently met.</summary>
    bool CanActivate { get; }

    /// <summary>Check whether the <see cref="UltimateHUD"/> is currently showing.</summary>
    bool IsHudVisible { get; }
}