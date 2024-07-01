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

using DaLion.Enchantments.Framework.Animations;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="EnchantmentsDayEndingEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class EnchantmentsDayEndingEvent(EventManager? manager = null)
    : DayEndingEvent(manager ?? EnchantmentsMod.EventManager)
{
    /// <inheritdoc />
    public override bool IsEnabled => Game1.player.health > Game1.player.maxHealth;

    /// <inheritdoc />
    protected override void OnDayEndingImpl(object? sender, DayEndingEventArgs e)
    {
        Game1.player.health = Game1.player.maxHealth;
        ShieldAnimation.Instance = null;
    }
}
