/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Events.Content;

#region using directives

using System.Collections.Immutable;
using System.Linq;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.SMAPI;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class ProfessionAssetsInvalidatedEvent : AssetsInvalidatedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ProfessionAssetsInvalidatedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ProfessionAssetsInvalidatedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnAssetsInvalidatedImpl(object? sender, AssetsInvalidatedEventArgs e)
    {
        Textures.Refresh(e.NamesWithoutLocale.Where(name => name.IsEquivalentToAnyOf(
            $"{Manifest.UniqueID}/SkillBars",
            $"{Manifest.UniqueID}/UltimateMeter",
            $"{Manifest.UniqueID}/PrestigeProgression"))
            .ToImmutableHashSet());
    }
}
