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

/// <summary>Represents an integration with a custom action.</summary>
internal interface IActionIntegration : ICustomIntegration
{
    /// <summary>Gets the unique mod id for the integration.</summary>
    string ModId { get; }

    /// <summary>Retrieves the custom action associated with the integration.</summary>
    /// <param name="mod">Mod info for the integration.</param>
    /// <returns>The custom action associated with the integration.</returns>
    public Action? GetAction(IMod mod);
}