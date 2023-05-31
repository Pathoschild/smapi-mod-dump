/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

namespace MappingExtensionsAndExtraProperties.Models.TileProperties;

public struct SetMailFlag : ITilePropertyData
{
    public static string PropertyKey => "MEEP_SetMailFlag";
    private string mailFlag;

    public string MailFlag
    {
        get => this.mailFlag;
    }

    public SetMailFlag(string mailFlag)
    {
        this.mailFlag = mailFlag;
    }
}
