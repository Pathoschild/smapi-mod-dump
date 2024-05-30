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

/// <summary>Represents an integration with a method.</summary>
internal interface IMethodIntegration : ICustomIntegration
{
    /// <summary>Gets the arguments for the method.</summary>
    object?[] Arguments { get; }

    /// <summary>Gets the method name for the integration.</summary>
    string MethodName { get; }

    /// <summary>Gets the unique mod id for the integration.</summary>
    string ModId { get; }
}