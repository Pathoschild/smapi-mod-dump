/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.Common.Services.Integrations.Automate;

using System.ComponentModel;
using Microsoft.Xna.Framework;

/// <summary>
/// An automatable entity, which can implement a more specific type like <see cref="IMachine" /> or
/// <see cref="IContainer" />. If it doesn't implement a more specific type, it's treated as a connector with no additional
/// logic.
/// </summary>
public interface IAutomatable
{
    /*********
     ** Accessors
     *********/
    /// <summary>The location which contains the machine.</summary>
    GameLocation Location { get; }

    /// <summary>The tile area covered by the machine.</summary>
    Rectangle TileArea { get; }
}