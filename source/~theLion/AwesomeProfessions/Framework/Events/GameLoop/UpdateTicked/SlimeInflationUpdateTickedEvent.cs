/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using System;
using System.Linq;
using StardewModdingAPI.Events;
using StardewValley;

#endregion using directives

internal class SlimeInflationUpdateTickedEvent : UpdateTickedEvent
{
    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object sender, UpdateTickedEventArgs e)
    {
        var uninflatedSlimes = ModEntry.State.Value.PipedSlimeScales.Keys.ToList();
        for (var i = uninflatedSlimes.Count - 1; i >= 0; --i)
        {
            uninflatedSlimes[i].Scale = Math.Min(uninflatedSlimes[i].Scale * 1.1f,
                Math.Min(ModEntry.State.Value.PipedSlimeScales[uninflatedSlimes[i]] * 2f, 2f));

            if (uninflatedSlimes[i].Scale >= 1.8f) uninflatedSlimes[i].willDestroyObjectsUnderfoot = true;

            if (uninflatedSlimes[i].Scale <= 1f || Game1.random.NextDouble() >
                0.2 - Game1.player.DailyLuck / 2 - Game1.player.LuckLevel * 0.01 && uninflatedSlimes[i].Scale <
                ModEntry.State.Value.PipedSlimeScales[uninflatedSlimes[i]] * 2f) continue;

            uninflatedSlimes[i].DamageToFarmer =
                (int) Math.Round(uninflatedSlimes[i].DamageToFarmer * uninflatedSlimes[i].Scale);
            uninflatedSlimes.RemoveAt(i);
        }

        if (!uninflatedSlimes.Any()) Disable();
    }
}