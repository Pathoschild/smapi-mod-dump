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

public struct MapBackgroundImage : ITilePropertyData
{
    public static string PropertyKey => "MEEP_CloseupInteraction_Image";
    private string imagePath;

    public string ImagePath => this.imagePath;

    public MapBackgroundImage(string imagePath)
    {
        this.imagePath = imagePath;
    }
}
