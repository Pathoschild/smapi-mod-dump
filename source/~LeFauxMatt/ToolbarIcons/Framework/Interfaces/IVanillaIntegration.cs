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

/// <summary>Represents an integration for a vanilla method.</summary>
internal interface IVanillaIntegration : ICustomIntegration
{
    /// <summary>Performs an action.</summary>
    public void DoAction();
}