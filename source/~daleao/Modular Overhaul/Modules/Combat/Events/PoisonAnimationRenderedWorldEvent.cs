/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Combat.Events;

#region using directives

using System.Linq;
using DaLion.Overhaul.Modules.Combat.StatusEffects;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Collections;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class PoisonAnimationRenderedWorldEvent : RenderedWorldEvent
{
    /// <summary>Initializes a new instance of the <see cref="PoisonAnimationRenderedWorldEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal PoisonAnimationRenderedWorldEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnRenderedWorldImpl(object? sender, RenderedWorldEventArgs e)
    {
        if (!PoisonAnimation.PoisonAnimationByMonster.Any())
        {
            this.Disable();
        }

        PoisonAnimation.PoisonAnimationByMonster.ForEach(pair => pair.Value.draw(e.SpriteBatch));
    }
}
