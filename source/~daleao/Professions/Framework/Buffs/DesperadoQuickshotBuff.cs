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

internal sealed class DesperadoQuickshotBuff : Buff
{
    internal const string ID = "DaLion.Professions.Buffs.DesperadoQuickshot";
    private const int SHEET_INDEX = 54;

    internal DesperadoQuickshotBuff()
        : base(
            id: ID,
            source: "Desperado",
            displaySource: _I18n.Get("desperado.title" + (Game1.player.IsMale ? ".male" : ".female")) + " " +
                           I18n.Desperado_Buff_Name(),
            iconTexture: Game1.buffsIcons,
            iconSheetIndex: SHEET_INDEX,
            duration: 800,
            description: I18n.Desperado_Buff_Desc())
    {
    }
}
