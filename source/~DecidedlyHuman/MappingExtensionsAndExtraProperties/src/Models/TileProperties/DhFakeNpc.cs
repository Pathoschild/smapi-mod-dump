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

public struct DhFakeNpc : ITilePropertyData
{
    private string npcName;

    public static string PropertyKey => "MEEP_FakeNPC";
    public int SpriteWidth;
    public int SpriteHeight;
    public bool HasSpriteSizes;
    public string NpcName;
}
