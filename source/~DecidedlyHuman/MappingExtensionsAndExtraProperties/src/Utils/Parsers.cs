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
using System.Linq;
using System.Text;
using DecidedlyShared.Logging;
using MappingExtensionsAndExtraProperties.Models.TileProperties;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;

namespace MappingExtensionsAndExtraProperties.Utils;

public class Parsers
{
    private static Logger logger;
    private static IModHelper helper;

    public static void InitialiseParsers(Logger l, IModHelper h)
    {
        logger = l;
        helper = h;
    }

    public static bool TryParseIncludingKey(string property, out CloseupInteractionImage parsedProperty)
    {
        parsedProperty = new CloseupInteractionImage();

        string[] splitProperty = property.Split(" ");

        // We need index 0 = MEEP_CloseupInteraction_Image, index 1 = the texture asset name at minimum.
        if (splitProperty.Length < 2)
            return false;

        StringBuilder args = new StringBuilder();
        // We know we're dealing with the minimum required parameters now, so we rejoin them to pass into the parser.
        for (int i = 1; i < splitProperty.Length; i++)
        {
            if (i == splitProperty.Length - 1)
                args.Append($"{splitProperty[i]}");
            else
                args.Append($"{splitProperty[i]} ");
        }

        if (TryParse(args.ToString(), out CloseupInteractionImage finalParsedProperty))
        {
            parsedProperty = finalParsedProperty;

            return true;
        }

        return false;
    }

    public static bool TryParseIncludingKey(string property, out AddConversationTopic parsedProperty)
    {
        parsedProperty = null;

        string[] splitProperty = property.Split(" ");

        // We need one or two properties exclusively.
        if (splitProperty.Length < 1) return false;
        if (splitProperty.Length > 2) return false;

        StringBuilder args = new StringBuilder();
        // We know we're dealing with the minimum required parameters now, so we rejoin them to pass into the parser.
        for (int i = 1; i < splitProperty.Length; i++)
        {
            if (i == splitProperty.Length - 1)
                args.Append($"{splitProperty[i]}");
            else
                args.Append($"{splitProperty[i]} ");
        }

        if (TryParse(args.ToString(), out AddConversationTopic finalParsedProperty))
        {
            parsedProperty = finalParsedProperty;

            return true;
        }

        return false;
    }

    public static bool TryParseIncludingKey(string property, out CloseupInteractionText parsedProperty)
    {
        parsedProperty = new CloseupInteractionText();

        string[] splitProperty = property.Split(" ");

        if (splitProperty.Length < 2)
            return false;

        StringBuilder args = new StringBuilder();
        // We know we're dealing with the minimum required parameters now, so we rejoin them to pass into the parser.
        for (int i = 1; i < splitProperty.Length; i++)
        {
            if (i == splitProperty.Length - 1)
                args.Append($"{splitProperty[i]}");
            else
                args.Append($"{splitProperty[i]} ");
        }

        parsedProperty.Text = args.ToString();
        return true;
    }

    public static bool TryParseIncludingKey(string property, out LetterText parsedProperty)
    {
        parsedProperty = new LetterText();

        string[] splitProperty = property.Split(" ");

        if (splitProperty.Length < 2)
            return false;

        StringBuilder args = new StringBuilder();
        // We know we're dealing with the minimum required parameters now, so we rejoin them to pass into the parser.
        for (int i = 1; i < splitProperty.Length; i++)
        {
            if (i == splitProperty.Length - 1)
                args.Append($"{splitProperty[i]}");
            else
                args.Append($"{splitProperty[i]} ");
        }

        parsedProperty.Text = args.ToString();
        return true;
    }

    // public static bool TryParseIncludingKey(string property, out LetterType parsedProperty)
    // {
    //     parsedProperty = new LetterType();
    //
    //     string[] splitProperty = property.Split(" ");
    //
    //     if (splitProperty.Length < 1)
    //         return false;
    //
    //     if (int.TryParse(splitProperty[0], out parsedProperty.BgType))
    //         return true;
    //
    //     return false;
    // }

    public static bool TryParseIncludingKey(string property, out CloseupInteractionSound parsedProperty)
    {
        parsedProperty = new CloseupInteractionSound();

        string[] splitProperty = property.Split(" ");

        if (splitProperty.Length < 2)
            return false;

        parsedProperty.CueName = splitProperty[1];
        return true;
    }

    public static bool TryParse(string property, out SetMailFlag parsedProperty)
    {
        // There isn't really anything to parse here.
        parsedProperty = new SetMailFlag(property);

        return true;
    }

    public static bool TryParse(string property, out CloseupInteractionText parsedProperty)
    {
        // Not much to parse here, so we just set and return.
        parsedProperty = new CloseupInteractionText();
        parsedProperty.Text = property;

        return true;
    }

    public static bool TryParse(string property, out CloseupInteractionSound parsedProperty)
    {
        // Not much to parse here, so we just set and return.
        parsedProperty = new CloseupInteractionSound();
        parsedProperty.CueName = property;

        return true;
    }

    public static bool TryParse(string property, out LetterText parsedProperty)
    {
        parsedProperty = new LetterText();
        parsedProperty.Text = property;

        return true;
    }

    public static bool TryParse(string property, out LetterType parsedProperty)
    {
        parsedProperty = new LetterType();
        string[] splitProperty = property.Split(" ");
        Texture2D letterTexture = null;

        /*
         * For this property, we expect:
         * 1) The vanilla BG letter type (an int)
         * OR
         * 1) The texture asset name (a string)
         */

        if (splitProperty.Length == 1)
        {
            if (int.TryParse(splitProperty[0], out int letterBgType))
            {
                // The first value parsed to an int, so we're done.
                parsedProperty = new LetterType(letterBgType);
                return true;
            }
            else
            {
                // If that failed, we may be dealing with a texture asset instead.
                Texture2D texture;

                try
                {
                    texture = Game1.content.Load<Texture2D>(splitProperty[0]);
                    parsedProperty = new LetterType(0, texture);

                    return true;
                }
                catch (ContentLoadException e)
                {
                    Parsers.logger.Error("First parameter of property wasn't a valid asset name. Is it spelled correctly?");
                    Parsers.logger.Error($"Asset name: {splitProperty[0]}");
                    Parsers.logger.Exception(e);

                    return false;
                }
            }
        }
        else
        {
            Parsers.logger.Error("Couldn't parse this letter type property. Check that it has the correct number of parameters.");
        }

        return false;
    }

    public static bool TryParse(string property, out CloseupInteractionImage parsedProperty)
    {
        parsedProperty = new CloseupInteractionImage();
        string[] splitProperty = property.Split(" ");

        /*
        For this property, we expect:
        1) One parameter (the texture asset name), or
        2) Five parameters (the texture asset name, plus an x, y, width, and height)
        */

        if (splitProperty.Length < 1 || (splitProperty.Length > 1 && splitProperty.Length < 5))
            return false;

        // First, we want to validate the asset points to a valid texture.
        try
        {
            parsedProperty.Texture = Game1.content.Load<Texture2D>(splitProperty[0]);
        }
        catch (Exception e)
        {
            Parsers.logger.Error($"Couldn't load texture {splitProperty[0]} from property {property}.");
            Parsers.logger.Error("Did you load the image correctly, or is spelled incorrectly in the patch?");
            return false;
        }

        // We've gotten this far, so we check to see if we only have the one argument.
        if (splitProperty.Length == 1)
        {
            // We're only worrying about texture asset, so we set our sourceRect.
            parsedProperty.SourceRect = parsedProperty.Texture.Bounds;

            return true;
        }

        // We know we're dealing with the full set of arguments.
        int rectX, rectY, rectWidth, rectHeight;

        if (!int.TryParse(splitProperty[1], out rectX)) return false;
        if (!int.TryParse(splitProperty[2], out rectY)) return false;
        if (!int.TryParse(splitProperty[3], out rectWidth)) return false;
        if (!int.TryParse(splitProperty[4], out rectHeight)) return false;

        // All of our ints were parsed successfully, so we assign them to a brand new sourceRect.
        parsedProperty.SourceRect = new Rectangle(rectX, rectY, rectWidth, rectHeight);

        return true;
    }

    public static bool TryParse(string property, out AddConversationTopic parsedProperty)
    {
        parsedProperty = null;
        string[] splitProperty = property.Split(" ");

        // For this property, we want either:
        // 1) One argument (just the conversation topic string)
        // 2) Two argument (the conversation topic string, and an integer to specify the days it will last for)
        if (splitProperty.Length < 1) return false;
        if (splitProperty.Length > 2) return false;

        string conversationTopic = splitProperty[0];
        if (!int.TryParse(splitProperty[1], out int numDays)) return false;

        // Everything's parsed correctly, so we stick everything in the parsed property and be on our way.
        parsedProperty = new AddConversationTopic(conversationTopic, numDays);

        return true;
    }

    public static bool TryParse(string property, out DhFakeNpc parsedProperty)
    {
        parsedProperty = new DhFakeNpc();
        parsedProperty.HasSpriteSizes = false;
        string[] splitProperty = property.Split(" ");
        int npcSpriteWidth;
        int npcSpriteHeight;

        /*
        For this property, we expect:
        1) One parameter (NPC name)
        2) Three parameters (NPC name, sprite width, sprite height)
        */
        if (splitProperty.Length < 1 || (splitProperty.Length > 1 && splitProperty.Length != 3))
            return false;

        parsedProperty.NpcName = splitProperty[0];

        // We've gotten this far, so we check to see if we only have the one argument.
        if (splitProperty.Length == 1)
            return true; // If we only have an NPC name argument, we can simply return true here.

        // We know we're dealing with the full set of arguments, so we try to parse them all.
        if (!int.TryParse(splitProperty[1], out npcSpriteWidth)) return false;
        if (!int.TryParse(splitProperty[2], out npcSpriteHeight)) return false;

        parsedProperty.SpriteWidth = npcSpriteHeight;
        parsedProperty.SpriteHeight = npcSpriteHeight;
        parsedProperty.HasSpriteSizes = true;

        return true;
    }

    public static bool TryParse(string property, out MapBackgroundImage parsedProperty)
    {
        // Very little validation to do at this point for this one.
        parsedProperty = new MapBackgroundImage(property);

        return true;
    }

    public static bool TryParse(string property, out MapBackgroundTileSize parsedProperty)
    {
        int width = 0;
        int height = 0;

        // Uniquely for this property, we want to split with an X-like character, instead of a space.
        string[] splitProperty = property.Split(new char[]{'x', 'X', '*'});

        /*
         * For this, we need either:
         * 1) Two values (e.g., 200x200)
         * 2) One value (200, to be parsed into 200x200)
         */

        if (splitProperty.Length == 1)
        {
            if (int.TryParse(splitProperty[0], out int size))
            {
                width = size;
                height = size;
            }
            else
            {
                Parsers.logger.Error($"Couldn't parse an integer from property: {property}");
                parsedProperty = new MapBackgroundTileSize();
                return false;
            }
        }
        else if (splitProperty.Length == 2)
        {
            if (int.TryParse(splitProperty[0], out int parsedWidth))
            {
                width = parsedWidth;
            }
            else
            {
                Parsers.logger.Error($"Couldn't parse an integer from property: {property}");
                parsedProperty = new MapBackgroundTileSize();
                return false;
            }

            if (int.TryParse(splitProperty[0], out int parsedHeight))
            {
                height = parsedHeight;
            }
            else
            {
                Parsers.logger.Error($"Couldn't parse an integer from property: {property}");
                parsedProperty = new MapBackgroundTileSize();
                return false;
            }
        }

        parsedProperty = new MapBackgroundTileSize(width, height);

        return true;
    }

    public static bool TryParse(string property, out MapBackgroundTileVariation parsedProperty)
    {
        double variation = 0d;

        if (double.TryParse(property, out double parsedVariation))
        {
            variation = parsedVariation;
        }
        else
        {
            Parsers.logger.Error($"Couldn't parse a double from property: {property}");
            parsedProperty = new MapBackgroundTileVariation();

            return false;
        }

        parsedProperty = new MapBackgroundTileVariation(variation);
        return true;
    }
}
