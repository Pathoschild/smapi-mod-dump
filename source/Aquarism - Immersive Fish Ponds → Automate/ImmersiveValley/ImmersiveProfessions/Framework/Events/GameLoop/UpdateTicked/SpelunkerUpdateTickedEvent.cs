/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using System;
using System.Linq;
using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;

#endregion using directives

[UsedImplicitly]
internal class SpelunkerUpdateTickedEvent : UpdateTickedEvent
{
    private const int SHEET_INDEX_I = 40;

    private readonly int _buffId = ModEntry.Manifest.UniqueID.GetHashCode() + (int) Profession.Spelunker;

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object sender, UpdateTickedEventArgs e)
    {
        if (Game1.currentLocation is not MineShaft) return;

        var buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == _buffId);
        if (buff is not null) return;

        var bonusLadderChance = (ModEntry.PlayerState.SpelunkerLadderStreak * 0.5f).ToString("0.0");
        var bonusSpeed = Math.Min(ModEntry.PlayerState.SpelunkerLadderStreak / 10 + 1,
            (int) ModEntry.Config.SpelunkerSpeedCap);
        Game1.buffsDisplay.addOtherBuff(
            new(0, 0, 0, 0, 0, 0, 0, 0, 0, bonusSpeed, 0, 0,
                1,
                "Spelunker",
                ModEntry.ModHelper.Translation.Get("spelunker.name." + (Game1.player.IsMale ? "male" : "female")))
            {
                which = _buffId,
                sheetIndex = SHEET_INDEX_I,
                millisecondsDuration = 0,
                description =
                    ModEntry.ModHelper.Translation.Get("spelunker.buffdesc", new {bonusLadderChance, bonusSpeed})
            }
        );
    }
}