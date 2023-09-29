/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Combat.Integrations;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Integrations;

#endregion using directives

[ModRequirement("BBR.BetterRings", "Better Rings")]
[ModConflict("Taiyo.VanillaTweaks")]
internal sealed class BetterRingsIntegration : ModIntegration<BetterRingsIntegration>
{
    /// <summary>Initializes a new instance of the <see cref="BetterRingsIntegration"/> class.</summary>
    internal BetterRingsIntegration()
        : base(ModHelper.ModRegistry)
    {
    }

    /// <inheritdoc />
    protected override bool RegisterImpl()
    {
        if (!this.IsLoaded)
        {
            return false;
        }

        ModHelper.GameContent.InvalidateCacheAndLocalized("Maps/springobjects");
        Log.D("[CMBT]: Registered the Better Rings integration.");
        return true;
    }
}
