/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.ToolbarIcons.Framework.Interfaces;

using StardewMods.ToolbarIcons.Framework.Models;

/// <summary>Mod config data for Toolbar Icons.</summary>
internal interface IModConfig
{
    /// <summary>Gets a value containing the toolbar icons.</summary>
    public List<ToolbarIcon> Icons { get; }

    /// <summary>Gets the size that icons will be scaled to.</summary>
    public float Scale { get; }
}