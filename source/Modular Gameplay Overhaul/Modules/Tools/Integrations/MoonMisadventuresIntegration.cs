/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Tools.Integrations;

#region using directives

using DaLion.Overhaul.Modules.Core.ConfigMenu;
using DaLion.Shared.Attributes;
using DaLion.Shared.Integrations;

#endregion using directives

[ModRequirement("spacechase0.MoonMisadventures", "Moon Misadventures")]
internal sealed class MoonMisadventuresIntegration : ModIntegration<MoonMisadventuresIntegration>
{
    /// <summary>Initializes a new instance of the <see cref="MoonMisadventuresIntegration"/> class.</summary>
    internal MoonMisadventuresIntegration()
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

        if (ModEntry.Config.Tools.Validate())
        {
            return true;
        }

        GenericModConfigMenu.Instance?.Reload();
        Log.D("[TOLS]: Registered the Moon Misadventures integration.");
        return true;
    }
}
