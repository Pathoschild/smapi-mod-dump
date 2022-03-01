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

using System.Linq;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Monsters;

using Extensions;

#endregion using directives

internal class PiperUpdateTickedEvent : UpdateTickedEvent
{
    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object sender, UpdateTickedEventArgs e)
    {
        // countdown contact timer
        if (ModEntry.PlayerState.Value.SlimeContactTimer > 0 && Game1.game1.IsActive && Game1.shouldTimePass())
            --ModEntry.PlayerState.Value.SlimeContactTimer;

        // countdown key press accumulator
        if (ModEntry.PlayerState.Value.KeyPressAccumulator == 1 && e.IsMultipleOf(40)) --ModEntry.PlayerState.Value.KeyPressAccumulator;
    }
}