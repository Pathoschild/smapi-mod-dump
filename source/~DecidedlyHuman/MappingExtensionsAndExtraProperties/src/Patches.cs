/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using MappingExtensionsAndExtraProperties.Models.TileProperties;
using MappingExtensionsAndExtraProperties.Utils;
using DecidedlyShared.Logging;
using DecidedlyShared.Ui;
using DecidedlyShared.Utilities;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using xTile.Dimensions;
using xTile.ObjectModel;

namespace MappingExtensionsAndExtraProperties;

public static class Patches
{
    private static Logger? logger = null;
    private static TilePropertyHandler? tileProperties = null;
    private static Properties propertyUtils;

    public static void InitialisePatches(Logger logger, TilePropertyHandler tileProperties)
    {
        Patches.logger = logger;
        Patches.tileProperties = tileProperties;
        Patches.propertyUtils = new Properties(logger);
    }

    // public static void Event_CheckAction_Postfix(Event __instance, Location tileLocation,
    // xTile.Dimensions.Rectangle viewport, Farmer who)
    // {
    //     // First, pull our tile co-ordinates from the location.
    //     int tileX = tileLocation.X;
    //     int tileY = tileLocation.Y;
    //
    //     // Check for our EndPlayerControl property.
    //     if (tileProperties.TryGetBackProperty(tileX, tileY, Game1.currentLocation, EndPlayerControl.PropertyKey,
    //             out PropertyValue endControl))
    //     {
    //         Game1.CurrentEvent.EndPlayerControlSequence();
    //         Game1.CurrentEvent.currentCommand++;
    //     }
    // }
    //
    // public static void Event_ReceiveActionPress_Postfix(Event __instance, int xTile, int yTile)
    // {
    //     // Check for our EndPlayerControl property.
    //     // if (tileProperties.TryGetBackProperty(xTile, yTile, Game1.currentLocation, EndPlayerControl.PropertyKey,
    //     //         out PropertyValue endControl))
    //     // {
    //         Game1.CurrentEvent.EndPlayerControlSequence();
    //         Game1.CurrentEvent.currentCommand++;
    //     // }
    // }

    // This is complete and utter nesting hell. TODO: Clean this up at some point. After release is fine.
    public static void GameLocation_CheckAction_Postfix(GameLocation __instance, Location tileLocation,
        xTile.Dimensions.Rectangle viewport, Farmer who)
    {
#if DEBUG
        Stopwatch timer = new Stopwatch();
        timer.Start();
#endif
        // Consider removing this try/catch.
        try
        {
            // First, pull our tile co-ordinates from the location.
            int tileX = tileLocation.X;
            int tileY = tileLocation.Y;

            // Check for a CloseupInteraction property on the given tile.
            if (tileProperties.TryGetBackProperty(tileX, tileY, __instance, CloseupInteractionImage.PropertyKey,
                    out PropertyValue closeupInteractionProperty))
            {
                DoCloseupInteraction(__instance, tileX, tileY, closeupInteractionProperty);
            }
            // If there isn't a single interaction property, we want to look for the start of a reel.
            else if (Patches.propertyUtils.TryGetInteractionReel(tileX, tileY, __instance,
                         CloseupInteractionImage.PropertyKey,
                         out List<MenuPage> pages))
            {
                string cueName = "bigSelect";

                // Now we check for a sound interaction property.
                if (tileProperties.TryGetBackProperty(tileX, tileY, __instance, CloseupInteractionSound.PropertyKey,
                        out PropertyValue closeupSoundProperty))
                {
                    if (Parsers.TryParse(closeupSoundProperty.ToString(),
                            out CloseupInteractionSound parsedSoundProperty))
                    {
                        cueName = parsedSoundProperty.CueName;
                    }
                }

                DoCloseupReel(pages, Patches.logger, cueName);
            }
            // There isn't a reel either, so we check for a letter property.
            else if (tileProperties.TryGetBackProperty(tileX, tileY, __instance, LetterText.PropertyKey,
                         out PropertyValue letterProperty))
            {
                DoLetter(__instance, letterProperty, tileX, tileY);
            }

            // Check for the DHSetMailFlag property on a given tile.
            if (tileProperties.TryGetBackProperty(tileX, tileY, __instance, SetMailFlag.PropertyKey,
                    out PropertyValue dhSetMailFlagProperty))
            {
                DoMailFlag(dhSetMailFlagProperty);
            }
        }
        catch (Exception e)
        {
            logger.Error("Caught exception handling GameLocation.checkAction in a postfix. Details follow:");
            logger.Exception(e);
        }
#if DEBUG
        timer.Stop();

        logger.Log($"Took {timer.ElapsedMilliseconds} to process in CheckAction patch.", LogLevel.Info);
#endif
    }

    // NOTE: COMPLETELY INACTIVE CURRENTLY.
    public static void SObject_PerformUseAction(SObject __instance, GameLocation location)
    {
        HashSet<string> tags = __instance.GetContextTags();
        bool hasInteractionProperty = false;
        bool hasText = false;
        bool hasSound = false;
        bool hasLetter = false;
        bool hasLetterType = false;
        CloseupInteractionImage parsedProperty = new CloseupInteractionImage();
        CloseupInteractionText parsedTextProperty = new CloseupInteractionText();
        CloseupInteractionSound parsedSoundProperty = new CloseupInteractionSound();
        LetterText parsedLetterProperty = new LetterText();
        LetterType parsedLetterTypeProperty = new LetterType();

        foreach (string tag in tags)
        {
            if (tag.Contains("MEEP"))
            {
                logger.Log($"Working with tag {tag}.", LogLevel.Info);

                // We're dealing with an abomination of a MEEP in-item property.
                if (tag.Contains(CloseupInteractionImage.PropertyKey))
                    hasInteractionProperty = Parsers.TryParseIncludingKey(tag, out parsedProperty);
                if (tag.Contains(CloseupInteractionText.PropertyKey))
                    hasText = Parsers.TryParseIncludingKey(tag, out parsedTextProperty);
                if (tag.Contains(CloseupInteractionSound.PropertyKey))
                    hasSound = Parsers.TryParseIncludingKey(tag, out parsedSoundProperty);
                // if (tag.Contains(LetterText.PropertyKey))
                //     hasLetter = Parsers.TryParseIncludingKey(tag, out parsedLetterProperty);
                // if (tag.Contains(LetterType.PropertyKey))
                //     hasLetterType = Parsers.TryParseIncludingKey(tag, out parsedLetterTypeProperty);

            }
        }

        logger.Log($"hasProperty: {hasInteractionProperty}.", LogLevel.Info);
        logger.Log($"hasText: {hasText}.", LogLevel.Info);
        logger.Log($"hasSound: {hasSound}.", LogLevel.Info);

        if (hasInteractionProperty)
        {
            CreateInteractionUi(parsedProperty,
                hasText ? parsedTextProperty : null,
                hasSound ? parsedSoundProperty : null
            );
        }
        else if (hasLetter)
        {
            LetterViewerMenu letterViewer = new LetterViewerMenu(parsedLetterProperty.Text, "Test");

            if (hasLetterType)
                letterViewer.whichBG = parsedLetterTypeProperty.BgType;

            Game1.activeClickableMenu = letterViewer;
        }
    }

    private static void DoMailFlag(PropertyValue dhSetMailFlagProperty)
    {

        // It exists, so parse it.
        if (Parsers.TryParse(dhSetMailFlagProperty.ToString(), out SetMailFlag parsedProperty))
        {
            // We've parsed it, so we try setting the mail flag appropriately.
            Player.TryAddMailFlag(parsedProperty.MailFlag, Game1.player);
        }
        else
        {
            logger.Error($"Failed to parse property {dhSetMailFlagProperty.ToString()}");
        }
    }

    private static void DoLetter(GameLocation location, PropertyValue letterProperty, int tileX, int tileY)
    {
        if (Parsers.TryParse(letterProperty.ToString(), out LetterText letter))
        {
            LetterViewerMenu letterViewer = new LetterViewerMenu(letter.Text, "Test");

            if (tileProperties.TryGetBackProperty(tileX, tileY, location, LetterType.PropertyKey,
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

    private static void DoCloseupReel(List<MenuPage> pages, Logger logger, string soundCue = "bigSelect")
    {

        // List<MenuPage> pages = new List<MenuPage>();
        //
        // foreach (PropertyValue value in properties)
        // {
        //     if (Parsers.TryParse(value.ToString(),
        //             out CloseupInteractionImage parsed))
        //     {
        //         MenuPage menuPage = new MenuPage();
        //         UiElement picture = new UiElement(
        //             "Picture",
        //             new Microsoft.Xna.Framework.Rectangle(0, 0, parsed.SourceRect.Width * 4,
        //                 parsed.SourceRect.Height * 4),
        //             DrawableType.Texture,
        //             parsed.Texture,
        //             parsed.SourceRect,
        //             Color.White);
        //
        //         menuPage.page = picture;
        //
        //         pages.Add(menuPage);
        //     }
        // }

        PaginatedMenu pagedMenu = new PaginatedMenu(
            "Interaction Reel",
            pages,
            Utility.xTileToMicrosoftRectangle(Game1.uiViewport),
            Patches.logger,
            DrawableType.None,
            soundCue);

        // Finally, we create our menu, and set it to be the current, active menu.
        MenuBase menu = new MenuBase(pagedMenu, $"Reel", logger, soundCue);

        // And set our container's parent.
        pagedMenu.SetParent(menu);

        Game1.activeClickableMenu = menu;
        menu.MenuOpened();
    }

    private static void DoCloseupInteraction(GameLocation location, int tileX, int tileY,
        PropertyValue closeupInteractionProperty)
    {
        // We have our tile property. We need to check for the presence of an existing Action tile property.
        if (tileProperties.TryGetBuildingProperty(tileX, tileY, location, "Action",
                out PropertyValue _))
        {
            // We want to return so we don't conflict with opening a shop, going through a door, etc.

            Patches.logger.Warn(
                $"Found CloseupInteraction_Image tile property on {tileX}, {tileY} in {location.Name}. Interaction with it was blocked due to there being an Action tile property on the same tile.");
            Patches.logger.Warn(
                $"It's recommended that you contact the author of the mod that added the tile property to let them know.");

            return;
        }

        // Next, we try to parse our tile property.
        if (!Parsers.TryParse(closeupInteractionProperty.ToString(),
                out CloseupInteractionImage closeupInteractionParsed))
        {
            // If the parsing failed, we want to nope out.
        }

        // At this point, we have our correctly-parsed tile property, so we create our image container.
        VBoxElement vBox = new VBoxElement(
            "Picture Box",
            new Microsoft.Xna.Framework.Rectangle(
                0,
                0,
                closeupInteractionParsed.SourceRect.Width * 2, closeupInteractionParsed.SourceRect.Height),
            Patches.logger,
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
            Patches.logger,
            DrawableType.Texture,
            closeupInteractionParsed.Texture,
            closeupInteractionParsed.SourceRect,
            Color.White));

        // Next, we want to see if there's a text tile property to display.
        if (tileProperties.TryGetBackProperty(tileX, tileY, location, CloseupInteractionText.PropertyKey,
                out PropertyValue closeupTextProperty))
        {
            // There is, so we try to parse it.
            if (Parsers.TryParse(closeupTextProperty.ToString(), out CloseupInteractionText parsedTextProperty))
            {
                // It parsed successfully, so we create a text element, and add it to our image container.
                vBox.AddChild(new TextElement(
                    "Popup Text Box",
                    Microsoft.Xna.Framework.Rectangle.Empty,
                    Patches.logger,
                    600,
                    parsedTextProperty.Text));
            }
            else
            {
                logger.Error($"Failed to parse property {closeupTextProperty.ToString()}");
            }
        }

        // Finally, we create our menu.
        MenuBase menu = new MenuBase(vBox, $"{CloseupInteractionImage.PropertyKey}", Patches.logger);

        // And set our container's parent.
        vBox.SetParent(menu);

        // Now we check for a sound interaction property.
        if (tileProperties.TryGetBackProperty(tileX, tileY, location, CloseupInteractionSound.PropertyKey,
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

    private static void CreateInteractionUi(CloseupInteractionImage closeupInteractionParsed,
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
            Patches.logger,
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
            Patches.logger,
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
                Patches.logger,
                600,
                closeupInteractionText.Value.Text));
        }

        if (closeupInteractionSound.HasValue)
        {
            soundCue = closeupInteractionSound.Value.CueName;
        }

        // // Next, we want to see if there's a text tile property to display.
        // if (tileProperties.TryGetBackProperty(tileX, tileY, location, CloseupInteractionText.PropertyKey,
        //         out PropertyValue closeupTextProperty))
        // {
        //     // There is, so we try to parse it.
        //     if (Parsers.TryParse(closeupTextProperty.ToString(), out CloseupInteractionText parsedTextProperty))
        //     {
        //         // It parsed successfully, so we create a text element, and add it to our image container.
        //         vBox.AddChild(new TextElement(
        //             "Popup Text Box",
        //             Microsoft.Xna.Framework.Rectangle.Empty,
        //             600,
        //             parsedTextProperty.Text));
        //     }
        //     else
        //     {
        //         logger.Error($"Failed to parse property {closeupTextProperty.ToString()}");
        //     }
        // }

        // Finally, we create our menu, and set it to be the current, active menu.
        MenuBase menu = new MenuBase(vBox, $"{CloseupInteractionImage.PropertyKey}", Patches.logger, soundCue);

        // And set our container's parent.
        vBox.SetParent(menu);

        Game1.activeClickableMenu = menu;
        menu.MenuOpened();
    }

    public static void Game1_drawMouseCursor_Prefix(Game1 __instance)
    {
        int xTile = (int)Game1.currentCursorTile.X;
        int yTile = (int)Game1.currentCursorTile.Y;

        if (tileProperties.TryGetBackProperty(xTile, yTile, Game1.currentLocation, SetMailFlag.PropertyKey,
                out PropertyValue _) ||
            tileProperties.TryGetBackProperty(xTile, yTile, Game1.currentLocation, CloseupInteractionImage.PropertyKey,
                out PropertyValue _) ||
            tileProperties.TryGetBackProperty(xTile, yTile, Game1.currentLocation,
                $"{CloseupInteractionImage.PropertyKey}_1",
                out PropertyValue _) ||
            tileProperties.TryGetBackProperty(xTile, yTile, Game1.currentLocation, LetterType.PropertyKey,
                out PropertyValue _) ||
            tileProperties.TryGetBackProperty(xTile, yTile, Game1.currentLocation, LetterText.PropertyKey,
                out PropertyValue _) ||
            tileProperties.TryGetBackProperty(xTile, yTile, Game1.currentLocation, SetMailFlag.PropertyKey,
                out PropertyValue _))
        {
            Game1.mouseCursor = 5;
        }
    }
}
