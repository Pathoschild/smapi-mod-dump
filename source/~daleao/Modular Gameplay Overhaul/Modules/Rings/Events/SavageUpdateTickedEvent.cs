/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Rings.Events;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Shared.Events;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class SavageUpdateTickedEvent : UpdateTickedEvent
{
    private readonly int _buffId;
    private readonly string _buffDescription;
    private readonly string _buffSource;

    /// <summary>Initializes a new instance of the <see cref="SavageUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal SavageUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
        this._buffId = (Manifest.UniqueID + "Savage").GetHashCode();
        this._buffSource =
            ModHelper.GameContent.Load<Dictionary<int, string>>("Data/ObjectInformation")[Constants.SavangeRingIndex]
                .Split('/')[0];
        this._buffDescription = Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.472");
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        var savageness = RingsModule.State.SavageExcitedness;
        if (savageness <= 0)
        {
            this.Disable();
        }

        var buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == this._buffId);
        if (buff is not null)
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
                savageness,
                0,
                0,
                1,
                "Savage Ring",
                this._buffSource)
            {
                which = this._buffId,
                sheetIndex = 9,
                millisecondsDuration = 1111,
                description = this._buffDescription,
                glow = Color.Cyan,
            });

        var buffDecay = savageness switch
        {
            > 6 => 3,
            >= 4 => 2,
            _ => 1,
        };

        RingsModule.State.SavageExcitedness = Math.Max(0, savageness - buffDecay);
    }
}
