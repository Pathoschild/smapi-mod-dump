/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Rings.Events;

#region using directives

using System.Collections.Generic;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class WarriorUpdateTickedEvent : UpdateTickedEvent
{
    private readonly int _buffId;
    private readonly string _buffDescription;
    private readonly string _buffSource;

    /// <summary>Initializes a new instance of the <see cref="WarriorUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal WarriorUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
        this._buffId = (Manifest.UniqueID + "Warrior").GetHashCode();
        this._buffSource =
            ModHelper.GameContent
                .Load<Dictionary<int, string>>("Data/ObjectInformation")[Constants.WarriorRingIndex]
                .SplitWithoutAllocation('/')[0]
                .ToString();
        this._buffDescription = Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.468");
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        var killCount = RingsModule.State.WarriorKillCount;
        if (killCount < 10)
        {
            this.Disable();
            return;
        }

        if (Game1.player.hasBuff(this._buffId))
        {
            return;
        }

        var magnitude = killCount / 10;
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
                magnitude,
                1,
                "Warrior Ring",
                this._buffSource)
            {
                which = this._buffId,
                sheetIndex = 20,
                millisecondsDuration = 0,
                description = this._buffDescription,
                glow = Color.DarkRed,
            });
    }
}
