/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;

namespace Pathoschild.Stardew.LookupAnything.Components
{
    /// <summary>The base class for Lookup Anything menus.</summary>
    internal class BaseMenu : IClickableMenu
    {
        /*********
        ** Fields
        *********/
        /// <summary>Whether to use dimensions which are guaranteed to fit within the screen.</summary>
        /// <remarks>This is enabled automatically when the menu detects a rare scissor rectangle error ("The scissor rectangle cannot be larger than or outside of the current render target bounds"). The menu will usually be pushed into the top-left corner when this is active, so it be disabled unless it's needed.</remarks>
        protected static bool UseSafeDimensions { get; set; }


        /*********
        ** Protected methods
        *********/
        /// <summary>Get the viewport size adjusted for compatibility.</summary>
        protected Point GetViewportSize()
        {
            Point viewport = new Point(Game1.uiViewport.Width, Game1.uiViewport.Height);

            if (BaseMenu.UseSafeDimensions && Game1.graphics.GraphicsDevice.Viewport.Width < viewport.X)
            {
                viewport = new Point(
                    x: Math.Min(viewport.X, Game1.graphics.GraphicsDevice.Viewport.Width),
                    y: Math.Min(viewport.Y, Game1.graphics.GraphicsDevice.Viewport.Height)
                );
            }

            return viewport;
        }
    }
}
