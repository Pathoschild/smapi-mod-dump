/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.SuperMode;

#region using directives

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

using Events.Display;
using Events.GameLoop;

#endregion using directives

/// <summary>Fullscreen tinted overlay activated during Super Mode.</summary>
internal class SuperModeOverlay
{
    private const float MAX_OPACITY_F = 0.3f;

    private float _opacity;
    private readonly Color _color;

    /// <summary>Construct an instance.</summary>
    /// <param name="index">The currently registered Super Mode profession's index.</param>
    public SuperModeOverlay(Color color)
    {
        _color = color;
        _opacity = 0f;
    }

    /// <summary>Draw the overlay over the world.</summary>
    /// <param name="b">A <see cref="SpriteBatch" /> to draw to.</param>
    /// <remarks>This should be called from a <see cref="RenderedWorldEvent" />.</remarks>
    public void Draw(SpriteBatch b)
    {
        b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, _color * _opacity);
    }

    /// <summary>Gradually increase the overlay's opacity.</summary>
    public void FadeIn()
    {
        if (_opacity < MAX_OPACITY_F) _opacity += 0.01f;
        if (_opacity >= MAX_OPACITY_F)
            EventManager.Disable(typeof(SuperModeOverlayFadeInUpdateTickedEvent));
    }

    /// <summary>Gradually decrease the overlay's opacity.</summary>
    public void FadeOut()
    {
        if (_opacity > 0) _opacity -= 0.01f;
        if (_opacity <= 0)
            EventManager.Disable(typeof(SuperModeActiveOverlayRenderedWorldEvent),
                typeof(SuperModeOverlayFadeOutUpdateTickedEvent));
    }
}