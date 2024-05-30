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

using DaLion.Professions.Framework.Limits;
using Microsoft.Xna.Framework;

#endregion using directives

internal sealed class DesperadoBlossomBuff : Buff
{
    internal const string ID = "DaLion.Professions.Buffs.Limit.Blossom";
    private const int BASE_DURATION = 15_000;
    private const int SHEET_INDEX = 55;

    internal DesperadoBlossomBuff()
        : base(
            id: ID,
            source: "Blossom",
            displaySource: Game1.player.IsMale ? I18n.Desperado_Limit_Title_Male() : I18n.Desperado_Limit_Title_Female(),
            duration: (int)(BASE_DURATION * LimitBreak.GetDurationMultiplier),
            iconTexture: Game1.buffsIcons,
            iconSheetIndex: SHEET_INDEX,
            description: I18n.Desperado_Limit_Desc())
    {
        this.glow = Color.DarkGoldenrod;
    }

    public override void OnAdded()
    {
        SoundBox.DesperadoWhoosh.PlayAll(Game1.player.currentLocation, Game1.player.Tile);
    }
}
