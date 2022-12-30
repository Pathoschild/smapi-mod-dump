/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Arsenal.Events;

#region using directives

using System.Collections.Generic;
using DaLion.Overhaul.Modules.Arsenal.Enchantments;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class EnergizedUpdateTickedEvent : UpdateTickedEvent
{
    private readonly Dictionary<Farmer, uint> _previousStepsByFarmer = new();

    /// <summary>Initializes a new instance of the <see cref="EnergizedUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal EnergizedUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnEnabled()
    {
        this._previousStepsByFarmer[Game1.player] = Game1.stats.StepsTaken;
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        var player = Game1.player;
        var energized = (player.CurrentTool as MeleeWeapon)?.GetEnchantmentOfType<EnergizedEnchantment>();
        if (energized is null)
        {
            this.Disable();
            return;
        }

        if (e.IsOneSecond)
        {
            var gained = (Game1.stats.StepsTaken - this._previousStepsByFarmer[player]) / 3;
            if (gained > 0)
            {
                energized.Stacks += (int)gained;
                this._previousStepsByFarmer[player] = Game1.stats.StepsTaken;
            }
        }

        if (energized.Stacks <= 0 || player.hasBuff(EnergizedEnchantment.BuffId))
        {
            return;
        }

        Game1.buffsDisplay.addOtherBuff(
            new Buff(
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                1,
                "Energized",
                I18n.Get("enchantments.energized"))
            {
                which = EnergizedEnchantment.BuffId,
                sheetIndex = EnergizedEnchantment.BuffSheetIndex,
                millisecondsDuration = 0,
                description = string.Empty,
            });
    }
}
