/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Rings.Integrations;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Integrations;
using DaLion.Shared.Integrations.WearMoreRings;

#endregion using directives

[RequiresMod("bcmpinc.WearMoreRings", "Wear More Rings", "5.1")]
internal sealed class WearMoreRingsIntegration : ModIntegration<WearMoreRingsIntegration, IWearMoreRingsApi>
{
    /// <summary>Initializes a new instance of the <see cref="WearMoreRingsIntegration"/> class.</summary>
    internal WearMoreRingsIntegration()
        : base("bcmpinc.WearMoreRings", "Wear More Rings", "5.1", ModHelper.ModRegistry)
    {
    }
}
