/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using ItemExtensions.Models.Contained;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

// ReSharper disable PossibleLossOfFraction

namespace ItemExtensions.Additions;

public class LetterWithImage : LetterViewerMenu
{
    private static void Log(string msg, LogLevel lv = LogLevel.Trace) => ModEntry.Mon.Log(msg, lv);

    private NoteData RawData { get; set; }
    private bool HasImage { get; set; }
    private Vector2 ImagePosition { get; set; } = new Vector2(-999);
    private Vector2 TextPosition { get; set; } = new Vector2(-999);
    private string Text { get; set; }
    
    public LetterWithImage(NoteData note) : base(null)
    {
        if (int.TryParse(note.LetterTexture, out var which))
        {
            letterTexture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\letterBG");
            whichBG = which;
        }
        else
        {
            letterTexture = Game1.temporaryContent.Load<Texture2D>(note.LetterTexture);
            whichBG = -1;
        }

        var hasText = !string.IsNullOrWhiteSpace(note.Message);
        if (hasText)
        {
            Text = note.Message;
        }
        
        HasImage = !string.IsNullOrWhiteSpace(note.Image);
        if (!HasImage) 
            return;
        
        secretNoteImageTexture = Game1.temporaryContent.Load<Texture2D>(note.Image);
        ImagePosition = hasText ? GetPosition(note.ImagePosition, secretNoteImageTexture) : GetCentered(secretNoteImageTexture);
    }

    private static string GetOpposite(string pos)
    {
        var result = pos switch
        {
            "left" => "right",
            "top" => "bot",
            "bottom" or "bot" => "top",
            "right" => "left",
            _ => "mid"
        };
        return result;
    }

    /// <summary>
    /// Get the position for a given texture.
    /// </summary>
    /// <param name="text">The alignment.</param>
    /// <param name="tex">The texture.</param>
    private static Vector2 GetPosition(string text, Texture2D tex)
    {
        try
        {
            var position = new Vector2();
            var alignment = text ?? "mid";

            //var middle = Game1.viewportCenter.ToVector2();
            var w = Game1.viewport.Width;
            var h = Game1.viewport.Height;
            var middleX = w / 2;
            var middleY = h / 2;

            position.Y = alignment switch
            {
                "top" => 0,
                "bot" or "bottom" => h - tex.Height * 4,
                _ => middleY - tex.Height * 2,
            };

            position.X = alignment switch
            {
                "left" => 0,
                "right" => h - tex.Width * 4,
                _ => middleX - tex.Width * 2,
            };

            return position;
        }
        catch (Exception e)
        {
            Log("Error: " + e, LogLevel.Error);
            throw;
        }
    }

    private static Vector2 GetCentered(Texture2D tex)
    {
        var middleX = Game1.viewport.Width / 2;
        var middleY = Game1.viewport.Height / 2;

        var position = new Vector2(middleX - tex.Width * 2,middleY - tex.Height * 2);
        return position;
    }

    public LetterWithImage(string text) : base(text)
    {
    }

    public LetterWithImage(int secretNoteIndex) : base(secretNoteIndex)
    {
    }

    public LetterWithImage(string mail, string mailTitle, bool fromCollection = false) : base(mail, mailTitle, fromCollection)
    {
    }
    
    public override void draw(SpriteBatch b)
    {
        //draw note
        b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);
        b.Draw(letterTexture, new Vector2(xPositionOnScreen + width / 2, yPositionOnScreen + height / 2), new Rectangle(whichBG * 320, 0, 320, 180), Color.White, 0.0f, new Vector2(160f, 90f), 4f * scale, SpriteEffects.None, 0.86f);
        
        if (HasImage)
        {
            b.Draw(
                secretNoteImageTexture, 
                position:ImagePosition, 
                sourceRectangle: new Rectangle(0,0, secretNoteImageTexture.Width, secretNoteImageTexture.Height), 
                color: Color.Black * 0.4f, 
                rotation: 0.0f, 
                origin: Vector2.Zero, 
                scale: 4f, 
                SpriteEffects.None, 
                layerDepth: 0.865f);
                
            b.Draw(
                texture: secretNoteImageTexture, 
                position: ImagePosition, 
                sourceRectangle: new Rectangle(0,0, secretNoteImageTexture.Width, secretNoteImageTexture.Height), 
                color: Color.White, 
                rotation: 0.0f, 
                origin: Vector2.Zero, 
                scale: 4f, 
                effects: SpriteEffects.None, 
                layerDepth: 0.865f);
            
            forwardButton.draw(b);
            backButton.draw(b);
        }
        if (!string.IsNullOrWhiteSpace(Text))
        {
            var textPosition = GetTextRect();
            SpriteText.drawString(
                b: b, 
                s: Text, 
                x: textPosition.X, 
                y: textPosition.Y, 
                width: textPosition.Width, //- 64,
                height: textPosition.Height,
                alpha: 0.75f, 
                layerDepth: 0.865f, 
                color: getTextColor()
                );
        }
        
        if (upperRightCloseButton != null || shouldDrawCloseButton())
            upperRightCloseButton?.draw(b);
        
        if (Game1.options.SnappyMenus && scale < 1.0 || Game1.options.SnappyMenus && !forwardButton.visible && !backButton.visible && !HasQuestOrSpecialOrder && !itemsLeftToGrab())
            return;
        
        drawMouse(b);
    }

    private Rectangle GetTextRect()
    {
        var x = xPositionOnScreen + 200;
        var y = yPositionOnScreen + 200;
        var w = Game1.viewport.Width;
        var h = Game1.viewport.Height;
        var imageHeight = secretNoteImageTexture?.Height ?? 0;
        var imageWidth = secretNoteImageTexture?.Width ?? 0;
        
        var result = GetOpposite(RawData.ImagePosition.ToLower()) switch
        {
            "left" => new Rectangle(x, y, letterWidth - imageWidth, letterHeight),
            "top" => new Rectangle(x, y, letterWidth, letterHeight - imageHeight),
            "bottom" or "bot" => new Rectangle(x,y + imageHeight, letterWidth, letterHeight - imageHeight),
            "right" => new Rectangle(x + imageWidth, y, letterWidth - imageWidth, letterHeight),
            _ => new Rectangle()
        };
        
        return result;
    }
}