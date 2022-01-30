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

using Extensions;

#endregion using directives

internal class DemolitionistBuffDisplayUpdateTickedEvent : UpdateTickedEvent
{
    private const int SHEET_INDEX_I = 41;

    private readonly int _buffId;

    /// <summary>Construct an instance.</summary>
    internal DemolitionistBuffDisplayUpdateTickedEvent()
    {
        _buffId = (ModEntry.Manifest.UniqueID + (int) Profession.Demolitionist).GetHashCode();
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object sender, UpdateTickedEventArgs e)
    {
        if (ModEntry.State.Value.DemolitionistExcitedness <= 0) Disable();

        if (e.Ticks % 30 == 0)
        {
            var buffDecay = ModEntry.State.Value.DemolitionistExcitedness > 4 ? 2 : 1;
            ModEntry.State.Value.DemolitionistExcitedness =
                Math.Max(0, ModEntry.State.Value.DemolitionistExcitedness - buffDecay);
        }

        var buffId = _buffId + ModEntry.State.Value.DemolitionistExcitedness;
        var buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == buffId);
        if (buff is not null) return;

        Game1.buffsDisplay.addOtherBuff(
            new(0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                ModEntry.State.Value.DemolitionistExcitedness,
                0,
                0,
                1,
                "Demolitionist",
                ModEntry.ModHelper.Translation.Get(
                    "demolitionist.name." + (Game1.player.IsMale ? "male" : "female")))
            {
                which = buffId,
                sheetIndex = SHEET_INDEX_I,
                millisecondsDuration = 0,
                description = ModEntry.ModHelper.Translation.Get("demolitionist.buffdesc")
            }
        );
    }
}