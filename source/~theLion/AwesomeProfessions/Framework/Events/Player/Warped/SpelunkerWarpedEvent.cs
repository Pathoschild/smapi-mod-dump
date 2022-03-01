/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Events.Player;

#region using directives

using System;
using StardewModdingAPI.Events;
using StardewValley.Locations;

using GameLoop;
using Extensions;

#endregion using directives

internal class SpelunkerWarpedEvent : WarpedEvent
{
    /// <inheritdoc />
    protected override void OnWarpedImpl(object sender, WarpedEventArgs e)
    {
        if (e.NewLocation.Equals(e.OldLocation)) return;

        if (e.NewLocation is MineShaft newShaft && e.OldLocation is MineShaft oldShaft &&
            newShaft.mineLevel > oldShaft.mineLevel)
        {
            ++ModEntry.PlayerState.Value.SpelunkerLadderStreak;

            if (e.Player.HasProfession(Profession.Spelunker, true))
            {
                var player = e.Player;
                player.health = Math.Min(player.health + (int) (player.maxHealth * 0.05f), player.maxHealth);
                player.Stamina = Math.Min(player.Stamina + player.MaxStamina * 0.05f, player.MaxStamina);
            }

            EventManager.Enable(typeof(SpelunkerUpdateTickedEvent));
        }
        else if (e.NewLocation is not MineShaft && e.OldLocation is MineShaft)
        {
            ModEntry.PlayerState.Value.SpelunkerLadderStreak = 0;
            EventManager.Disable(typeof(SpelunkerUpdateTickedEvent));
        }
    }
}