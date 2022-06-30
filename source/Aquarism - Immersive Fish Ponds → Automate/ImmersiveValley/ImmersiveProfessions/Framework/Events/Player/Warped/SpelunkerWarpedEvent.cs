/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Events.Player;

#region using directives

using Common.Events;
using Extensions;
using GameLoop;
using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley.Locations;
using System;

#endregion using directives

[UsedImplicitly]
internal sealed class SpelunkerWarpedEvent : WarpedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal SpelunkerWarpedEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnWarpedImpl(object? sender, WarpedEventArgs e)
    {
        if (e.NewLocation.Equals(e.OldLocation)) return;

        if (e.NewLocation is MineShaft newShaft && e.OldLocation is MineShaft oldShaft &&
            newShaft.mineLevel > oldShaft.mineLevel)
        {
            ++ModEntry.PlayerState.SpelunkerLadderStreak;

            if (e.Player.HasProfession(Profession.Spelunker, true))
            {
                var player = e.Player;
                player.health = Math.Min(player.health + (int)(player.maxHealth * 0.025f), player.maxHealth);
                player.Stamina = Math.Min(player.Stamina + player.MaxStamina * 0.01f, player.MaxStamina);
            }

            Manager.Hook<SpelunkerUpdateTickedEvent>();
        }
        else if (e.NewLocation is not MineShaft && e.OldLocation is MineShaft)
        {
            ModEntry.PlayerState.SpelunkerLadderStreak = 0;
            Manager.Hook<SpelunkerUpdateTickedEvent>();
        }
    }
}