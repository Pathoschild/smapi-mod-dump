/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using StardewModdingAPI.Events;
using StardewValley;

namespace TheLion.Stardew.Professions.Framework.Events;

internal class SuperModeActivationTimerUpdateTickedEvent : UpdateTickedEvent
{
    private const int BASE_SUPERMODE_ACTIVATION_DELAY_I = 60;

    private int _superModeActivationTimer =
        (int) (BASE_SUPERMODE_ACTIVATION_DELAY_I * ModEntry.Config.SuperModeActivationDelay);

    /// <inheritdoc />
    public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
    {
        if (Game1.game1.IsActive && Game1.shouldTimePass()) --_superModeActivationTimer;

        if (_superModeActivationTimer > 0) return;
        ModState.IsSuperModeActive = true;
        ModEntry.Subscriber.Unsubscribe(GetType());
    }
}