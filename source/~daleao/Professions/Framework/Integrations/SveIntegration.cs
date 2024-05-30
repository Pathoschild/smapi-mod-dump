/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Framework.Integrations;

#region using directives

using DaLion.Professions.Framework.Events.Player.Warped;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Integrations;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="SveIntegration"/> class.</summary>
[ModRequirement("FlashShifter.StardewValleyExpandedCP")]
internal sealed class SveIntegration()
    : ModIntegration<SveIntegration>(ModHelper.ModRegistry)
{
    /// <summary>Gets a value indicating whether the <c>DisableGaldoranTheme</c> config setting is enabled.</summary>
    internal bool DisabeGaldoranTheme => this.IsLoaded && ModHelper
        .ReadContentPackConfig("FlashShifter.StardewValleyExpandedCP")
        ?.Value<bool?>("DisableGaldoranTheme") == true;

    /// <summary>Gets a value indicating whether the <c>UseGaldoranThemeAllTimes</c> config setting is enabled.</summary>
    internal bool UseGaldoranThemeAllTimes => this.IsLoaded && ModHelper
        .ReadContentPackConfig("FlashShifter.StardewValleyExpandedCP")
        ?.Value<bool?>("UseGaldoranThemeAllTimes") == true;

    protected override bool RegisterImpl()
    {
        if (!this.IsLoaded)
        {
            return false;
        }

        EventManager.Enable<SveWarpedEvent>();
        Log.D("Registered the Stardew Valley Expanded integration.");
        return true;
    }
}
