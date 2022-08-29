/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Events.Ultimate;

/// <summary>Interface for the arguments of an <see cref="UltimateEmptiedEvent"/>.</summary>
public interface IUltimateEmptiedEventArgs
{
    /// <summary>The player who triggered the event.</summary>
    Farmer Player { get; }
}