/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Alchemy.Framework;

#region using directives

using Common.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;

#endregion using directives

/// <summary>HUD component to show the player their current Ultimate charge value.</summary>
internal class ToxicityHUD
{
    private const int INITIAL_BAR_HEIGHT_I = 168,
        TEXTURE_HEIGHT_I = 56,
        TICKS_BETWEEN_SHAKES_I = 120,
        SHAKE_DURATION_I = 15,
        FADE_OUT_DELAY_I = 180,
        FADE_OUT_DURATION_I = 30;

    private double _shakeTimer = SHAKE_DURATION_I,
        _nextShake = TICKS_BETWEEN_SHAKES_I,
        _fadeOutTimer = FADE_OUT_DELAY_I + FADE_OUT_DURATION_I;

    private float _opacity = 1f;
    private bool _shake;

    /// <summary>Construct an instance.</summary>
    public ToxicityHUD()
    {
    }

    #region properties

    /// <summary>The texture that will be used to draw the gauge.</summary>
    internal static Texture2D? Texture { get; }

    /// <summary>Whether the gauge is being drawn.</summary>
    internal bool IsVisible { get; }

    #endregion properties

    #region public methods

    /// <summary>Draw the gauge and all it's components to the HUD.</summary>
    /// <param name="b">A <see cref="SpriteBatch" /> to draw to.</param>
    /// <remarks>This should be called from a <see cref="RenderingHudEvent" />.</remarks>
    internal void Draw(SpriteBatch b)
    {
        if (_opacity <= 0f) return;

        var bonusLevelHeight = (ToxicityManager.MaxTolerance - ToxicityManager.BASE_TOLERANCE_I) * 0.2;

        // get bar position
        var topOfBar = new Vector2(
            Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Right - 56,
            Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Bottom - 16 - TEXTURE_HEIGHT_I * 4 - (float)bonusLevelHeight
        );

        if (Game1.isOutdoorMapSmallerThanViewport())
            topOfBar.X = Math.Min(topOfBar.X,
                -Game1.viewport.X + Game1.currentLocation.map.Layers[0].LayerWidth * 64 - 48);

        if (Game1.showingHealth) topOfBar.X -= 112;
        else topOfBar.X -= 56;

        // shake horizontally above overdose threshold
        if (_shake)
            topOfBar.X += Game1.random.Next(-3, 4);

        // draw bar in thirds so that it may grow with combat level
        Rectangle srcRect, destRect;

        // top
        var width = 12;
        srcRect = new(0, 0, width, 16);
        b.Draw(
            Texture,
            topOfBar,
            srcRect,
            Color.White * _opacity,
            0f,
            Vector2.Zero,
            Game1.pixelZoom,
            SpriteEffects.None,
            1f
        );

        // middle
        var srcY = 16;
        srcRect = new(0, srcY, width, 20);
        destRect = new((int)topOfBar.X, (int)(topOfBar.Y + srcY * 4f), width * 4,
            16 + (TEXTURE_HEIGHT_I - 32) * 4 + (int)Math.Ceiling(bonusLevelHeight));
        b.Draw(
            Texture,
            destRect,
            srcRect,
            Color.White * _opacity
        );

        // bottom
        srcY = TEXTURE_HEIGHT_I - 16;
        srcRect = new(0, srcY, width, 16);
        b.Draw(
            Texture,
            new(topOfBar.X, topOfBar.Y + srcY * 4f + (float)bonusLevelHeight),
            srcRect,
            Color.White * _opacity,
            0f,
            Vector2.Zero,
            Game1.pixelZoom,
            SpriteEffects.None,
            1f
        );

        // draw fill
        var fillPct = (double)ToxicityManager.ToxicityValue / ToxicityManager.MaxTolerance;
        var fullBarHeight = INITIAL_BAR_HEIGHT_I + bonusLevelHeight;
        var srcHeight = (int)(42 * fillPct);
        var destHeight = (int)(fullBarHeight * fillPct);

        width = 6;
        srcY = TEXTURE_HEIGHT_I - 2 - srcHeight;
        srcRect = new(12, srcY, width, srcHeight);
        destRect = new((int)topOfBar.X + 12,
            (int)(topOfBar.Y + (TEXTURE_HEIGHT_I - 44) * 4 + (float)fullBarHeight - destHeight), width * 4, destHeight);

        b.Draw(
            Texture,
            destRect,
            srcRect,
            Color.White,
            0f,
            Vector2.Zero,
            SpriteEffects.None,
            1f
        );

        // draw top shadow
        destRect.Height = 4;
        b.Draw(
            Game1.staminaRect,
            destRect,
            Game1.staminaRect.Bounds,
            Color.Black * 0.3f,
            0f,
            Vector2.Zero,
            SpriteEffects.None,
            1f
        );

        // draw hover text
        if (Game1.getOldMouseX() >= topOfBar.X && Game1.getOldMouseY() >= topOfBar.Y &&
            Game1.getOldMouseX() < topOfBar.X + 36f)
            Game1.drawWithBorder(Math.Max(0, ToxicityManager.ToxicityValue) + "/" + ToxicityManager.MaxTolerance, Color.Black * 0f,
                Color.White,
                topOfBar + new Vector2(0f - Game1.dialogueFont.MeasureString("999/999").X - 32f, 64f));
    }

    /// <summary>Countdown the gauge shake timer .</summary>
    internal void UpdateShake()
    {
        if (!Game1.game1.IsActive || !Game1.shouldTimePass()) return;

        if (_shakeTimer > 0)
        {
            --_shakeTimer;
            if (_shakeTimer <= 0) _shake = false;
        }
        else if (_nextShake > 0)
        {
            --_nextShake;
            if (_nextShake > 0) return;

            _shake = true;
            _shakeTimer = SHAKE_DURATION_I;
            _nextShake = TICKS_BETWEEN_SHAKES_I;
        }
    }

    /// <summary>Forcefully set shaking state to <c>False</c>.</summary>
    internal void ForceStopShake()
    {
        _shake = false;
    }

    /// <summary>Gradually reduce the gauge's opacity value.</summary>
    internal void FadeOut()
    {
        --_fadeOutTimer;
        if (_fadeOutTimer >= FADE_OUT_DURATION_I) return;

        var ratio = (float)_fadeOutTimer / FADE_OUT_DURATION_I;
        _opacity = (float)(-1.0 / (1.0 + Math.Exp(12.0 * ratio - 6.0)) + 1.0);
        if (_fadeOutTimer > 0) return;

        _fadeOutTimer = FADE_OUT_DELAY_I + FADE_OUT_DURATION_I;
        _opacity = 1f;
    }

    #endregion public methods
}