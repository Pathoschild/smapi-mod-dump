/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Framework.Limits;

#region using directives

using DaLion.Professions.Framework.Events.Display.RenderingHud;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion using directives

/// <summary>HUD component to show the player their current Limit Break charge value.</summary>
internal sealed class LimitGauge
{
    private const int InitialBarHeight = 168;
    private const int TextureHeight = 56;
    private const int TicksBetweenShakes = 120;
    private const int ShakeDuration = 15;
    private const int FadeOutDelay = 180;
    private const int FadeOutDuration = 30;

    private readonly ILimitBreak _owner;
    private readonly Color _color;
    private float _opacity = 1f;
    private bool _shake;
    private double _shakeTimer = ShakeDuration;
    private double _nextShake = TicksBetweenShakes;
    private double _fadeOutTimer = FadeOutDelay + FadeOutDuration;

    /// <summary>Initializes a new instance of the <see cref="LimitGauge"/> class.</summary>
    /// <param name="limitBreak">The <see cref="ILimitBreak"/> instance that owns this gauge.</param>
    /// <param name="color">The color that will be used to fill the "S" icon above the gauge.</param>
    public LimitGauge(ILimitBreak limitBreak, Color color)
    {
        this._owner = limitBreak;
        this._color = color;
    }

    /// <summary>Gets the texture that will be used to draw the gauge.</summary>
    internal static Texture2D Texture => Textures.LimitGauge;

    /// <summary>Gets a value indicating whether determines whether the gauge is being drawn.</summary>
    internal static bool IsVisible => EventManager.IsEnabled<LimitGaugeRenderingHudEvent>();

    /// <summary>Draws the gauge and all it's components to the HUD.</summary>
    /// <param name="b">A <see cref="SpriteBatch"/> to draw to.</param>
    /// <remarks>This should be called from a <see cref="Shared.Events.RenderingHudEvent"/>.</remarks>
    internal void Draw(SpriteBatch b)
    {
        if (this._opacity <= 0f)
        {
            return;
        }

        var bonusLevelHeight = (LimitBreak.MaxCharge - LimitBreak.BASE_MAX_CHARGE) * 0.2;

        // get bar position
        var topOfBar = new Vector2(
            Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Right - 56,
            Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Bottom - 16 - (TextureHeight * 4) - (float)bonusLevelHeight);
        if (Game1.isOutdoorMapSmallerThanViewport())
        {
            topOfBar.X = Math.Min(
                topOfBar.X,
                -Game1.viewport.X + (Game1.currentLocation.map.Layers[0].LayerWidth * 64) - 48);
        }

        if (Game1.showingHealth)
        {
            topOfBar.X -= 112;
        }
        else
        {
            topOfBar.X -= 56;
        }

        // user overrides
        topOfBar.X += Config.Masteries.LimitGaugeOffset.X;
        topOfBar.Y += Config.Masteries.LimitGaugeOffset.Y;

        // shake horizontally if full and on stand-by, if active also shake vertically
        if (this._shake || this._owner.IsActive)
        {
            topOfBar.X += Game1.random.Next(-3, 4);
            if (this._owner.IsActive)
            {
                topOfBar.Y += Game1.random.Next(-3, 4);
            }
        }

        // draw bar in thirds so that it may grow with combat level
        Rectangle sourceRect, destRect;

        // top
        var width = 12;
        sourceRect = new Rectangle(0, 0, width, 16);
        b.Draw(
            Texture,
            topOfBar,
            sourceRect,
            Color.White * this._opacity,
            0f,
            Vector2.Zero,
            Game1.pixelZoom,
            SpriteEffects.None,
            1f);

        // draw top 'L'
        b.Draw(
            Texture,
            topOfBar + new Vector2(16, -212 + (TextureHeight * 4)),
            new Rectangle(13, 3, 4, 5),
            this._color * this._opacity,
            0f,
            Vector2.Zero,
            Game1.pixelZoom,
            SpriteEffects.None,
            1f);

        // middle
        var srcY = 16;
        sourceRect = new Rectangle(0, srcY, width, 20);
        destRect = new Rectangle(
            (int)topOfBar.X,
            (int)(topOfBar.Y + (srcY * 4f)),
            width * 4,
            ((TextureHeight - 32) * 4) + (int)Math.Ceiling(bonusLevelHeight));
        b.Draw(
            Texture,
            destRect,
            sourceRect,
            Color.White * this._opacity);

        // bottom
        srcY = TextureHeight - 16;
        sourceRect = new Rectangle(0, srcY, width, 16);
        b.Draw(
            Texture,
            new Vector2(topOfBar.X, topOfBar.Y + (srcY * 4f) + (float)bonusLevelHeight),
            sourceRect,
            Color.White * this._opacity,
            0f,
            Vector2.Zero,
            Game1.pixelZoom,
            SpriteEffects.None,
            1f);

        // draw fill
        var fillPercent = this._owner.ChargeValue / LimitBreak.MaxCharge;
        var fullBarHeight = InitialBarHeight + bonusLevelHeight;
        var srcHeight = (int)(42 * fillPercent);
        var destHeight = (int)(fullBarHeight * fillPercent);

        width = 6;
        srcY = TextureHeight - 2 - srcHeight;
        sourceRect = new Rectangle(12, srcY, width, srcHeight);
        destRect = new Rectangle(
            (int)topOfBar.X + 12,
            (int)(topOfBar.Y + ((TextureHeight - 44) * 4) + (float)fullBarHeight - destHeight),
            width * 4,
            destHeight);
        b.Draw(
            Texture,
            destRect,
            sourceRect,
            Color.White * this._opacity,
            0f,
            Vector2.Zero,
            SpriteEffects.None,
            1f);

        // draw top shadow
        destRect.Height = 4;
        b.Draw(
            Game1.staminaRect,
            destRect,
            Game1.staminaRect.Bounds,
            Color.Black * 0.3f * this._opacity,
            0f,
            Vector2.Zero,
            SpriteEffects.None,
            1f);

        // draw hover text
        if (Game1.getOldMouseX() >= topOfBar.X && Game1.getOldMouseY() >= topOfBar.Y &&
            Game1.getOldMouseX() < topOfBar.X + 36f)
        {
            Game1.drawWithBorder(
                Math.Max(0, (int)this._owner.ChargeValue) + "/" + LimitBreak.MaxCharge,
                Color.Black * 0f,
                Color.White,
                topOfBar + new Vector2(0f - Game1.dialogueFont.MeasureString("999/999").X - 32f, 64f));
        }
    }

    /// <summary>Countdown the gauge shake timer .</summary>
    internal void UpdateShake()
    {
        if (!Game1.game1.IsActive || !Game1.shouldTimePass())
        {
            return;
        }

        if (this._shakeTimer > 0)
        {
            if (--this._shakeTimer <= 0)
            {
                this._shake = false;
            }
        }
        else if (this._nextShake > 0)
        {
            if (--this._nextShake > 0)
            {
                return;
            }

            this._shake = true;
            this._shakeTimer = ShakeDuration;
            this._nextShake = TicksBetweenShakes;
        }
    }

    /// <summary>Forcefully sets shaking state to <c>False</c>.</summary>
    internal void ForceStopShake()
    {
        this._shake = false;
    }

    /// <summary>Gradually reduces the gauge's opacity value.</summary>
    /// <returns><see langword="true"/> if the fade out is complete, otherwise <see langword="false"/>.</returns>
    internal bool FadeOut()
    {
        if (--this._fadeOutTimer >= FadeOutDuration)
        {
            return false;
        }

        var ratio = (float)this._fadeOutTimer / FadeOutDuration;
        this._opacity = (float)((-1d / (1d + Math.Exp((12d * ratio) - 6d))) + 1d);
        if (this._fadeOutTimer > 0)
        {
            return false;
        }

        EventManager.Disable<LimitGaugeRenderingHudEvent>();
        this._fadeOutTimer = FadeOutDelay + FadeOutDuration;
        this._opacity = 1f;
        return true;
    }
}
