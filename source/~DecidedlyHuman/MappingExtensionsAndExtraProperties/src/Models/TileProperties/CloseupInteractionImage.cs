/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MappingExtensionsAndExtraProperties.Models.TileProperties;

public struct CloseupInteractionImage : ITilePropertyData
{
    public static string PropertyKey => "MEEP_CloseupInteraction_Image";
    public static string ReelProperty => "MEEP_CloseupInteraction_Image_1";
    public Texture2D Texture;
    public Rectangle SourceRect;
    private ITilePropertyData tilePropertyDataImplementation;

    public int Width
    {
        get => this.SourceRect.Width;
    }

    public int Height
    {
        get => this.SourceRect.Height;
    }

}
