/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Bouhm/stardew-valley-mods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewValley;

namespace NPCMapLocations.Framework
{
    // Library for methods that get tile position, map position, drag and drop position
    // based on the location of the cursor
    internal class MouseUtil
    {
        public static Vector2 BeginMousePosition { get; set; }
        public static Vector2 EndMousePosition { get; set; }

        public static void Reset()
        {
            BeginMousePosition = new Vector2(-1000, -1000);
            EndMousePosition = new Vector2(-1000, -1000);
        }

        // Return Vector2 position of tile at cursor
        public static Vector2 GetTilePositionAtCursor()
        {
            return Game1.currentCursorTile;
        }

        // Return Vector2 position of pixel position on the map at cursor
        public static Vector2 GetMapPositionAtCursor()
        {
            Vector2 mapPos = Utility.getTopLeftPositionForCenteringOnScreen(ModEntry.Map.Bounds.Width * 4, 720);
            return new Vector2((int)(Math.Ceiling(Game1.getMousePosition().X - mapPos.X)), (int)(Math.Ceiling(Game1.getMousePosition().Y - mapPos.Y)));
        }

        // Handle mouse down for beginning of drag and drop action
        // Accepts a callback function as an argument
        public static void HandleMouseDown(Action fn = null)
        {
            BeginMousePosition = new Vector2(Game1.getMouseX(), Game1.getMouseY());
            fn?.Invoke();
        }

        // Handle mouse release for end of drag and drop action
        // Accepts a callback function as an argument
        public static void HandleMouseRelease(Action fn = null)
        {
            EndMousePosition = new Vector2(Game1.getMouseX(), Game1.getMouseY());
            fn?.Invoke();
        }

        // Return Rectangle of current dragging area
        public static Rectangle GetCurrentDraggingArea()
        {
            return new((int)BeginMousePosition.X, (int)BeginMousePosition.Y, (int)(Game1.getMouseX() - BeginMousePosition.X), (int)(Game1.getMouseY() - BeginMousePosition.Y));
        }

        // Return Rectangle of drag and drop area
        public static Rectangle GetDragAndDropArea()
        {
            return new((int)BeginMousePosition.X, (int)BeginMousePosition.Y, (int)(EndMousePosition.X - BeginMousePosition.X), (int)(EndMousePosition.Y - BeginMousePosition.Y));
        }

        // Convert absolute positions to map positions
        public static Rectangle GetRectangleOnMap(Rectangle rect)
        {
            Vector2 mapBounds = Utility.getTopLeftPositionForCenteringOnScreen(ModEntry.Map.Bounds.Width * 4, 720);
            return new Rectangle((int)(rect.X - mapBounds.X), (int)(rect.Y - mapBounds.Y), (int)(EndMousePosition.X - BeginMousePosition.X), (int)(EndMousePosition.Y - BeginMousePosition.Y));
        }
    }
}
