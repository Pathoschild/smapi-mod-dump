/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

#if IS_FAUXCORE
namespace StardewMods.FauxCore.Common.Services.Integrations.FauxCore;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#else
namespace StardewMods.Common.Services.Integrations.FauxCore;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endif

/// <summary>Represents a framed custom menu.</summary>
public interface IFramedMenu
{
    /// <summary>Gets the current offset.</summary>
    Point CurrentOffset { get; }

    /// <summary>Gets the frame.</summary>
    Rectangle Frame { get; }

    /// <summary>Gets the maximum offset.</summary>
    Point MaxOffset { get; }

    /// <summary>Gets the step size for scrolling.</summary>
    int StepSize { get; }

    /// <summary>Draws the specified render action within a specified area of the screen.</summary>
    /// <param name="spriteBatch">The SpriteBatch used for drawing.</param>
    /// <param name="cursor">The mouse position.</param>
    void DrawInFrame(SpriteBatch spriteBatch, Point cursor);

    /// <summary>Sets the current offset.</summary>
    /// <param name="value">The current offset value.</param>
    /// <returns>Returns the menu.</returns>
    IFramedMenu SetCurrentOffset(Point value);

    /// <summary>Sets the maximum offset.</summary>
    /// <param name="value">The maximum offset value.</param>
    /// <returns>Returns the menu.</returns>
    IFramedMenu SetMaxOffset(Point value);
}