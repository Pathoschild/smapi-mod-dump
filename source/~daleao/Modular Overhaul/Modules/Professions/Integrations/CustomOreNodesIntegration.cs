/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Integrations;

#region using directives

using System.Collections.Immutable;
using System.Linq;
using DaLion.Shared.Attributes;
using DaLion.Shared.Integrations;
using DaLion.Shared.Integrations.CustomOreNodes;

#endregion using directives

[RequiresMod("aedenthorn.CustomOreNodes", "Custom Ore Nodes", "2.1.1")]
internal sealed class CustomOreNodesIntegration : ModIntegration<CustomOreNodesIntegration, ICustomOreNodesApi>
{
    /// <summary>Initializes a new instance of the <see cref="CustomOreNodesIntegration"/> class.</summary>
    internal CustomOreNodesIntegration()
        : base("aedenthorn.CustomOreNodes", "Custom Ore Nodes", "2.1.1", ModHelper.ModRegistry)
    {
    }

    internal void RegisterCustomOreData()
    {
        if (!this.IsLoaded)
        {
            return;
        }

        Collections.ResourceNodeIds = Collections.ResourceNodeIds
            .Concat(
                this.ModApi
                    .GetCustomOreNodes()
                    .Select(n => Reflector
                        .GetUnboundFieldGetter<object, int>(n, "parentSheetIndex")
                        .Invoke(n)))
            .ToImmutableHashSet();
    }
}
