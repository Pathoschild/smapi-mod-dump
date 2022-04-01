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

using StardewModdingAPI.Events;
using StardewValley;

#endregion using directives

internal class UltimateCountdownUpdateTickedEvent : UpdateTickedEvent
{
    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object sender, UpdateTickedEventArgs e)
    {
        if (!Game1.game1.IsActive || !Game1.shouldTimePass()) return;

        ModEntry.PlayerState.RegisteredUltimate.Countdown(Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds);
    }
}