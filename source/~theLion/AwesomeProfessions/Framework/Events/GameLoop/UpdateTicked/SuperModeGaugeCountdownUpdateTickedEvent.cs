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

internal class SuperModeGaugeCountdownUpdateTickedEvent : UpdateTickedEvent
{
    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object sender, UpdateTickedEventArgs e)
    {
        var amount = Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds /
                     (ModEntry.Config.SuperModeDrainFactor * 10);
        ModEntry.State.Value.SuperMode.Gauge.Countdown(amount);
    }
}