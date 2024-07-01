/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Framework.Buffs;

#region using directives

using DaLion.Core.Framework;

#endregion using directives

internal sealed class SpelunkerStreakBuff : StackableBuff
{
    internal const string ID = "DaLion.Professions.Buffs.SpelunkerStreak";
    private const int SHEET_INDEX = 56;

    internal SpelunkerStreakBuff()
        : base(
            id: ID,
            getStacks: () => State.SpelunkerLadderStreak,
            maxStacks: int.MaxValue,
            source: "Spelunker",
            displaySource: _I18n.Get("spelunker.title" + (Game1.player.IsMale ? ".male" : ".female")),
            duration: 17,
            iconTexture: Game1.buffsIcons,
            iconSheetIndex: SHEET_INDEX,
            getDescription: stacks => I18n.Spelunker_Buff_Desc((stacks * 0.005f).ToString("P1")))
    {
    }
}
