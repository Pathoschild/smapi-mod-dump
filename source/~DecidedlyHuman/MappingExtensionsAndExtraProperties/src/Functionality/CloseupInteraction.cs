/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using DecidedlyShared.Logging;
using DecidedlyShared.Ui;
using DecidedlyShared.Utilities;
using MappingExtensionsAndExtraProperties.Models.TileProperties;
using MappingExtensionsAndExtraProperties.Utils;
using Microsoft.Xna.Framework;
using StardewValley;
using xTile.ObjectModel;

namespace MappingExtensionsAndExtraProperties.Functionality;

public class CloseupInteraction
{
    public static void DoCloseupReel(List<MenuPage> pages, Logger logger, string soundCue = "bigSelect")
    {
        TilePropertyHandler handler = new TilePropertyHandler(logger);

        PaginatedMenu pagedMenu = new PaginatedMenu(
            "Interaction Reel",
            pages,
            Geometry.RectToRect(Game1.uiViewport),
            logger,
            DrawableType.None,
            soundCue);

        // Finally, we create our menu, and set it to be the current, active menu.
        MenuBase menu = new MenuBase(pagedMenu, $"Reel", logger, soundCue);

        // And set our container's parent.
        pagedMenu.SetParent(menu);

        Game1.activeClickableMenu = menu;
        menu.MenuOpened();
    }

    public static void DoCloseupInteraction(GameLocation location, int tileX, int tileY,
        PropertyValue closeupInteractionProperty, Logger logger)
    {
        TilePropertyHandler handler = new TilePropertyHandler(logger);

        // Next, we try to parse our tile property.
        if (!Parsers.TryParseIncludingKey(closeupInteractionProperty.ToString(),
                out CloseupInteractionImage closeupInteractionParsed))
        {
            // If the parsing failed, we want to nope out.

            return;
        }

        // At this point, we have our correctly-parsed tile property, so we create our image container.
        VBoxElement vBox = new VBoxElement(
            "Picture Box",
            new Microsoft.Xna.Framework.Rectangle(
                0,
                0,
                closeupInteractionParsed.SourceRect.Width * 2, closeupInteractionParsed.SourceRect.Height),
            logger,
            DrawableType.None,
            Game1.menuTexture,
            new Microsoft.Xna.Framework.Rectangle(0, 256, 60, 60),
            Color.White,
            16,
            16,
            16,
            16);

        // And the image element itself.
        vBox.AddChild(new UiElement(
            "Picture",
            new Microsoft.Xna.Framework.Rectangle(0, 0, closeupInteractionParsed.SourceRect.Width * 4,
                closeupInteractionParsed.SourceRect.Height * 4),
            logger,
            DrawableType.Texture,
            closeupInteractionParsed.Texture,
            closeupInteractionParsed.SourceRect,
            Color.White));

        // Next, we want to see if there's a text tile property to display.
        if (handler.TryGetBuildingProperty(tileX, tileY, location, CloseupInteractionText.PropertyKey,
                out PropertyValue closeupTextProperty))
        {
            // There is, so we try to parse it.
            if (Parsers.TryParse(closeupTextProperty.ToString(), out CloseupInteractionText parsedTextProperty))
            {
                // It parsed successfully, so we create a text element, and add it to our image container.
                vBox.AddChild(new TextElement(
                    "Popup Text Box",
                    Microsoft.Xna.Framework.Rectangle.Empty,
                    logger,
                    600,
                    parsedTextProperty.Text));
            }
            else
            {
                logger.Error($"Failed to parse property {closeupTextProperty.ToString()}");
            }
        }

        // Finally, we create our menu.
        MenuBase menu = new MenuBase(vBox, $"{CloseupInteractionImage.PropertyKey}", logger);

        // And set our container's parent.
        vBox.SetParent(menu);

        // Now we check for a sound interaction property.
        if (handler.TryGetBuildingProperty(tileX, tileY, location, CloseupInteractionSound.PropertyKey,
                out PropertyValue closeupSoundProperty))
        {
            if (Parsers.TryParse(closeupSoundProperty.ToString(), out CloseupInteractionSound parsedSoundProperty))
            {
                menu.SetOpenSound(parsedSoundProperty.CueName);
            }
        }

        Game1.activeClickableMenu = menu;
        menu.MenuOpened();
    }

    public static void CreateInteractionUi(CloseupInteractionImage closeupInteractionParsed, Logger logger,
        CloseupInteractionText? closeupInteractionText = null, CloseupInteractionSound? closeupInteractionSound = null)
    {
        string soundCue = "bigSelect";

        // At this point, we have our correctly-parsed tile property, so we create our image container.
        VBoxElement vBox = new VBoxElement(
            "Picture Box",
            new Microsoft.Xna.Framework.Rectangle(
                0,
                0,
                closeupInteractionParsed.SourceRect.Width * 2, closeupInteractionParsed.SourceRect.Height),
            logger,
            DrawableType.None,
            Game1.menuTexture,
            new Microsoft.Xna.Framework.Rectangle(0, 256, 60, 60),
            Color.White,
            16,
            16,
            16,
            16);

        // And the image element itself.
        vBox.AddChild(new UiElement(
            "Picture",
            new Microsoft.Xna.Framework.Rectangle(0, 0, closeupInteractionParsed.SourceRect.Width * 4,
                closeupInteractionParsed.SourceRect.Height * 4),
            logger,
            DrawableType.Texture,
            closeupInteractionParsed.Texture,
            closeupInteractionParsed.SourceRect,
            Color.White));

        if (closeupInteractionText.HasValue)
        {
            // And our text element.
            vBox.AddChild(new TextElement(
                "Popup Text Box",
                Microsoft.Xna.Framework.Rectangle.Empty,
                logger,
                600,
                closeupInteractionText.Value.Text));
        }

        if (closeupInteractionSound.HasValue)
        {
            soundCue = closeupInteractionSound.Value.CueName;
        }

        // Finally, we create our menu, and set it to be the current, active menu.
        MenuBase menu = new MenuBase(vBox, $"{CloseupInteractionImage.PropertyKey}", logger, soundCue);

        // And set our container's parent.
        vBox.SetParent(menu);

        Game1.activeClickableMenu = menu;
        menu.MenuOpened();
    }
}
