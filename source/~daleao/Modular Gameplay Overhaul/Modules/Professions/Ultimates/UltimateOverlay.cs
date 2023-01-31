/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Ultimates;

#region using directives

using DaLion.Overhaul.Modules.Professions.Events.Display;
using DaLion.Overhaul.Modules.Professions.Events.GameLoop;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion using directives

/// <summary>Fullscreen tinted overlay activated during Ultimate.</summary>
internal sealed class UltimateOverlay
{
    private const float MaxOpacity = 0.3f;
    private readonly Color _color;

    private float _opacity;

    /// <summary>Initializes a new instance of the <see cref="UltimateOverlay"/> class.</summary>
    /// <param name="color">The overlay <see cref="Color"/>.</param>
    internal UltimateOverlay(Color color)
    {
        this._color = color;
        this._opacity = 0f;
    }

    /// <summary>Draw the overlay over the world.</summary>
    /// <param name="b">A <see cref="SpriteBatch"/> to draw to.</param>
    /// <remarks>This should be called from a <see cref="Shared.Events.RenderedWorldEvent"/>.</remarks>
    internal void Draw(SpriteBatch b)
    {
        b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, this._color * this._opacity);
    }

    /// <summary>Gradually increase the overlay's opacity.</summary>
    internal void FadeIn()
    {
        if (this._opacity < MaxOpacity)
        {
            this._opacity += 0.01f;
        }

        if (this._opacity >= MaxOpacity)
        {
            EventManager.Disable<UltimateOverlayFadeInUpdateTickedEvent>();
        }
    }

    /// <summary>Gradually decrease the overlay's opacity.</summary>
    internal void FadeOut()
    {
        if (this._opacity > 0)
        {
            this._opacity -= 0.01f;
        }

        if (!(this._opacity <= 0))
        {
            return;
        }

        EventManager.Disable<UltimateOverlayFadeOutUpdateTickedEvent>();
        EventManager.Disable<UltimateOverlayRenderedWorldEvent>();
    }
}
