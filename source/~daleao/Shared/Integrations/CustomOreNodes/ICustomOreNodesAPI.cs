/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

#pragma warning disable CS1591
namespace DaLion.Shared.Integrations.CustomOreNodes;

#region using directives

using System.Collections.Generic;

#endregion using directives

/// <summary>The API provided by Custom Ore Nodes.</summary>
public interface ICustomOreNodesApi
{
    List<object> GetCustomOreNodes();

    List<string> GetCustomOreNodeIDs();
}
