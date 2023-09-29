/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Combat.Events.GameLoop.UpdateTicked;

#region using directives

using DaLion.Overhaul.Modules.Combat.Enums;
using DaLion.Overhaul.Modules.Combat.Extensions;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ComboResetUpdateTickedEvent : UpdateTickedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ComboResetUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ComboResetUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnEnabled()
    {
        var player = Game1.player;
        if (player.CurrentTool is not MeleeWeapon weapon || CombatModule.State.ComboHitQueued == ComboHitStep.Idle)
        {
            return;
        }

        CombatModule.State.ComboCooldown = (int)(400 * player.GetTotalSwingSpeedModifier(weapon));
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        CombatModule.State.ComboCooldown -= Game1.currentGameTime.ElapsedGameTime.Milliseconds;
        if (CombatModule.State.ComboCooldown > 0)
        {
            return;
        }

        CombatModule.State.ComboHitQueued = ComboHitStep.Idle;
        CombatModule.State.ComboHitStep = ComboHitStep.Idle;
        this.Disable();
    }
}
