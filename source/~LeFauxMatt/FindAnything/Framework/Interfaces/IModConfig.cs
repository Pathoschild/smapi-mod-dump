/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.FindAnything.Framework.Interfaces;

using StardewMods.FindAnything.Framework.Models;

/// <summary>Mod config data for Find Anything.</summary>
internal interface IModConfig
{
    /// <summary>Gets the controls.</summary>
    public Controls Controls { get; }
}