/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace SplashText;

internal class SplashText
{
    private static SpriteFont splashFont;

    private static readonly Random rand = new();
    private static float delta = 30f;

    private static float xPosition;
    private static float yPosition;

    private static readonly float sinePeriod = 2f;

    private static string splashText;
    private static int splashWidth;
    private static int splashHeight;

    private static Vector2 drawPosition;
    private static Vector2 drawPositionOffset1;
    private static Vector2 drawPositionOffset2;
    private static Vector2 splashMeasurements;
    private static Vector2 splashCenter;

    private static readonly Color shadowColor = new(60, 30, 0, 180);

    private static float windowSizeFactor = 1;

    private static List<string> splashTexts;

    internal static void Initialize()
    {
        // load spritefont
        splashFont = Globals.GameContent.Load<SpriteFont>(Globals.SplashFontPath);
        // set initial splash text
        GetRandomSplashText();
        // determine initial position and scale factor
        RecalculatePositionAndSize(new Point(Game1.uiViewport.Width, Game1.uiViewport.Height));
    }

    internal static void RecalculatePositionAndSize(Point newSize)
    {
        // position splash text relative to new window size
        xPosition = 0.75f * newSize.X;
        yPosition = 0.55f * newSize.Y;

        drawPosition = new Vector2(xPosition, yPosition);
        drawPositionOffset1 = new Vector2(xPosition + 4, yPosition + 2);
        drawPositionOffset2 = new Vector2(xPosition + 2, yPosition + 1);

        // determine scaling factor
        windowSizeFactor = newSize.X / 1920f;
    }

    internal static void GetRandomSplashText()
    {
        // load splash texts asset if not already loaded
        splashTexts ??= Globals.GameContent.Load<List<string>>(Globals.ContentPath);

        // select random splash text
        int randomIndex = rand.Next(splashTexts.Count);
        splashText = splashTexts[randomIndex];

        // measure selected splash text and store scaled width and height
        splashMeasurements = splashFont.MeasureString(splashText);
        splashWidth = (int) (splashMeasurements.X * windowSizeFactor);
        splashHeight = (int) (splashMeasurements.Y * windowSizeFactor);
        splashCenter = new Vector2(splashWidth / (float) 2, splashHeight / (float) 2);

        drawPosition = new Vector2(xPosition, yPosition);
        drawPositionOffset1 = new Vector2(xPosition + 4, yPosition + 2);
        drawPositionOffset2 = new Vector2(xPosition + 2, yPosition + 1);
    }

    internal static void RenderSplashText(object sender, RenderedActiveMenuEventArgs e)
    {
        // back out if we're on any submenu (New, Load, etc)
        if (Game1.activeClickableMenu is not TitleMenu || TitleMenu.subMenu is not null)
            return;

        // update delta time value
        delta = (delta + ((float) Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds * 0.002f)) % 50000f;

        // calculate delta-based values (rotation and scale)
        float degrees = -30 + (sinePeriod * (float) Math.Sin(delta));
        float splashRotation = (float) (degrees * Math.PI / 180); // convert degrees to radians
        float splashScale = 1.2f + (0.1f * (float) Math.Sin(delta * 0.75f));

        // draw shadow - 2 offset draws
        e.SpriteBatch.DrawString(
            splashFont,
            splashText,
            drawPositionOffset1,
            shadowColor,
            splashRotation,
            splashCenter,
            splashScale * windowSizeFactor,
            SpriteEffects.None,
            1f
        );

        e.SpriteBatch.DrawString(
            splashFont,
            splashText,
            drawPositionOffset2,
            shadowColor,
            splashRotation,
            splashCenter,
            splashScale * windowSizeFactor,
            SpriteEffects.None,
            1f
        );

        // draw text on top of shadow
        e.SpriteBatch.DrawString(
            splashFont,
            splashText,
            drawPosition,
            Color.Yellow,
            splashRotation,
            splashCenter,
            splashScale * windowSizeFactor,
            SpriteEffects.None,
            1f
        );
    }
}
