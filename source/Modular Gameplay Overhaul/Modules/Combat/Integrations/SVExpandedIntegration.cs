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

using DaLion.Overhaul.Modules.Combat.Events.Player.Warped;
using DaLion.Shared.Attributes;
using DaLion.Shared.Integrations;

#endregion using directives

[ModRequirement("FlashShifter.StardewValleyExpandedCP", "StardewValleyExpanded")]
internal sealed class SVExpandedIntegration : ModIntegration<SVExpandedIntegration>
{
    /// <summary>Initializes a new instance of the <see cref="SVExpandedIntegration"/> class.</summary>
    internal SVExpandedIntegration()
        : base(ModHelper.ModRegistry)
    {
    }

    protected override bool RegisterImpl()
    {
        if (!this.IsLoaded)
        {
            return false;
        }

        EventManager.Enable<SveWarpedEvent>();
        Log.D("[CMBT]: Registered the Stardew Valley Expanded integration.");
        return true;
    }
}
