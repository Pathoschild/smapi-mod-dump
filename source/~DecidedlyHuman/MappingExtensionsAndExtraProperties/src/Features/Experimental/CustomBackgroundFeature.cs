/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

namespace MappingExtensionsAndExtraProperties.Features.Experimental;

public class CustomBackgroundFeature
{
    // TODO: Finish MEEP background functionality.
    // string image = args.NewLocation.getMapProperty("MEEP_Background_Image");
    // string tile = args.NewLocation.getMapProperty("MEEP_Background_Tile_Size");
    // string variation = args.NewLocation.getMapProperty("MEEP_Background_Tile_Variation");
    //
    // int mapWidth = args.NewLocation.map.DisplayWidth;
    // int mapHeight = args.NewLocation.map.DisplayHeight;
    //
    // // I want all of these for this test.
    // if (image is null || tile is null || variation is null)
    //     return;
    //
    // bool imageParsed = Parsers.TryParse(image, out MapBackgroundImage backgroundImage);
    // bool sizeParsed = Parsers.TryParse(tile, out MapBackgroundTileSize tileSize);
    // bool variationParsed = Parsers.TryParse(variation, out MapBackgroundTileVariation tileVariation);
    //
    // if (!imageParsed || !sizeParsed || !variationParsed)
    //     return;
    //
    // Texture2D texture = Game1.content.Load<Texture2D>(backgroundImage.ImagePath);
    // int numTiles = (texture.Width / tileSize.Width) * (texture.Height / tileSize.Height);
    // Game1.background = new Background(texture, 1, mapWidth / tileSize.Width, mapHeight / tileSize.Height,
    //     tileSize.Width, tileSize.Height, 4f, 1, numTiles, 1d, Color.White);
}
