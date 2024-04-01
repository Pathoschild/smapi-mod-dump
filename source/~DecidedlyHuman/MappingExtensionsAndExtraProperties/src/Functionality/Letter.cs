/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using DecidedlyShared.Logging;
using DecidedlyShared.Utilities;
using MappingExtensionsAndExtraProperties.Models.TileProperties;
using MappingExtensionsAndExtraProperties.Utils;
using StardewValley;
using StardewValley.Menus;
using xTile.ObjectModel;

namespace MappingExtensionsAndExtraProperties.Functionality;

public class Letter
{
    public static void DoLetter(GameLocation location, PropertyValue letterProperty, int tileX, int tileY,
        Logger logger)
    {
        TilePropertyHandler handler = new TilePropertyHandler(logger);

        if (Parsers.TryParseIncludingKey(letterProperty.ToString(), out LetterText letter))
        {
            LetterViewerMenu letterViewer = new LetterViewerMenu(letter.Text, "Test");

            if (handler.TryGetBuildingProperty(tileX, tileY, location, LetterType.PropertyKey,
                    out PropertyValue letterTypeProperty))
            {
                if (Parsers.TryParse(letterTypeProperty.ToString(), out LetterType letterType))
                {
                    if (letterType.HasCustomTexture)
                    {
                        letterViewer.letterTexture = letterType.Texture;
                        letterViewer.whichBG = 0;
                    }
                    else
                        letterViewer.whichBG = letterType.BgType;
                    // letterViewer.whichBG = letterType.BgType - 1;
                }
            }

            Game1.activeClickableMenu = letterViewer;
        }
    }
}
