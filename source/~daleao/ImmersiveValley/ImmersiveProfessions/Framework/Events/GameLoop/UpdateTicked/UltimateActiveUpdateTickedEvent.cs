/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using Common.Events;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;

#endregion using directives

[UsedImplicitly]
internal sealed class UltimateActiveUpdateTickedEvent : UpdateTickedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal UltimateActiveUpdateTickedEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (!Game1.player.isGlowing)
        {
            var glowColor = ModEntry.PlayerState.RegisteredUltimate!.GlowColor;
            if (glowColor != Color.White)
                Game1.player.startGlowing(ModEntry.PlayerState.RegisteredUltimate.GlowColor, false, 0.05f);
        }

        if (Game1.game1.IsActive && Game1.shouldTimePass())
            ModEntry.PlayerState.RegisteredUltimate!.Countdown(Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds);
    }
}