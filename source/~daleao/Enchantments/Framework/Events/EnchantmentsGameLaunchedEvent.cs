/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Enchantments.Framework.Events;

#region using directives

using DaLion.Enchantments.Framework.Integrations;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="EnchantmentsGameLaunchedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class EnchantmentsGameLaunchedEvent(EventManager? manager = null)
    : GameLaunchedEvent(manager ?? EnchantmentsMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnGameLaunchedImpl(object? sender, GameLaunchedEventArgs e)
    {
        SpaceCoreIntegration.Instance!.Register();
        if (EnchantmentsConfigMenu.Instance?.IsLoaded == true)
        {
            EnchantmentsConfigMenu.Instance.Register();
        }
    }
}
