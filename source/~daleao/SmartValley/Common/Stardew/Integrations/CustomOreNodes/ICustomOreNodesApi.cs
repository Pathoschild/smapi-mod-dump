/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Common.Stardew.Integrations;

#region using directives

using System.Collections.Generic;

#endregion using directives

public interface ICustomOreNodesApi
{
    List<object> GetCustomOreNodes();

    List<string> GetCustomOreNodeIDs();
}