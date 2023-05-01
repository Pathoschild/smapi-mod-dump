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

using System.Linq;
using DaLion.Shared.Attributes;
using DaLion.Shared.Integrations;
using DaLion.Shared.Integrations.CustomResourceClumps;

#endregion using directives

[RequiresMod("aedenthorn.CustomResourceClumps", "Custom Resource Clumps", "0.7.0")]
internal sealed class CustomResourceClumpsIntegration : ModIntegration<CustomResourceClumpsIntegration, ICustomResourceClumpsApi>
{
    /// <summary>Initializes a new instance of the <see cref="CustomResourceClumpsIntegration"/> class.</summary>
    internal CustomResourceClumpsIntegration()
        : base("aedenthorn.CustomResourceClumps", "Custom Resource Clumps", "0.7.0", ModHelper.ModRegistry)
    {
    }

    internal void RegisterCustomClumpData()
    {
        if (!this.IsLoaded)
        {
            return;
        }

        Collections.ResourceClumpIds = this.ModApi
            .GetCustomClumpData()
            .Select(c => Reflector.GetUnboundFieldGetter<object, int>(c, "index").Invoke(c))
            .ToHashSet();
    }
}
