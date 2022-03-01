/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Events.Input;

#region using directives

using StardewModdingAPI.Events;
using StardewValley;

#endregion using directives

internal class PiperButtonsChangedEvent : ButtonsChangedEvent
{
    /// <inheritdoc />
    protected override void OnButtonsChangedImpl(object sender, ButtonsChangedEventArgs e)
    {
        if (!ModEntry.Config.ModKey.JustPressed()) return;

        ++ModEntry.PlayerState.Value.KeyPressAccumulator;
        if (ModEntry.PlayerState.Value.KeyPressAccumulator <= 1) return;

        ModEntry.PlayerState.Value.PipeMode = 1 - ModEntry.PlayerState.Value.PipeMode;
        Game1.playSound("objectiveComplete");
        Log.D($"Toggled {ModEntry.PlayerState.Value.PipeMode} targeting mode.");

        ModEntry.PlayerState.Value.KeyPressAccumulator = 0;
    }
}