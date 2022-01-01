/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using System;
using StardewModdingAPI.Events;
using StardewValley.Locations;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Events;

internal class SpelunkerWarpedEvent : WarpedEvent
{
    private static readonly SpelunkerBuffDisplayUpdateTickedEvent SpelunkerUpdateTickedEvent = new();

    /// <inheritdoc />
    public override void OnWarped(object sender, WarpedEventArgs e)
    {
        if (!e.IsLocalPlayer) return;

        if (e.NewLocation is MineShaft)
        {
            ++ModState.SpelunkerLadderStreak;

            if (e.Player.HasPrestigedProfession("Spelunker"))
            {
                var player = e.Player;
                player.health = Math.Min(player.health + (int) (player.maxHealth * 0.05f), player.maxHealth);
                player.Stamina = Math.Min(player.Stamina + player.MaxStamina * 0.05f, player.MaxStamina);
            }

            ModEntry.Subscriber.Subscribe(SpelunkerUpdateTickedEvent);
        }
        else
        {
            ModState.SpelunkerLadderStreak = 0;
            ModEntry.Subscriber.Unsubscribe(SpelunkerUpdateTickedEvent.GetType());
        }
    }
}