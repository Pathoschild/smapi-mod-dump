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

using StardewValley.Buffs;

#endregion using directives

internal sealed class PoacherBackstabBuff : Buff
{
    internal const string ID = "DaLion.Professions.Buffs.Limit.Backstab";
    private const int SHEET_INDEX = 50;

    internal PoacherBackstabBuff(int duration)
        : base(
            id: ID,
            source: "Ambush",
            displaySource: Game1.player.IsMale ? I18n.Poacher_Limit_Title_Male() : I18n.Poacher_Limit_Title_Female(),
            duration: duration,
            iconTexture: Game1.buffsIcons,
            iconSheetIndex: SHEET_INDEX,
            effects: new BuffEffects
            {
                CriticalPowerMultiplier = { 2f },
            },
            description: I18n.Poacher_Limit_Desc_Revealed())
    {
    }
}
