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

using System.Linq;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="EnchantmentsAssetsInvalidatedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class EnchantmentsAssetsInvalidatedEvent(EventManager? manager = null)
    : AssetsInvalidatedEvent(manager ?? EnchantmentsMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnAssetsInvalidatedImpl(object? sender, AssetsInvalidatedEventArgs e)
    {
        Textures.Reload(e.NamesWithoutLocale.AsEnumerable());
    }
}
