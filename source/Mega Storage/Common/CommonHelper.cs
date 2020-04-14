using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace furyx639.Common
{
    internal static class CommonHelper
    {
        /*********
        ** Fields
        *********/
        // Star Button
        public static Rectangle StarButtonActive => new Rectangle(310, 392, 16, 16);
        public static Rectangle StarButtonInactive => new Rectangle(294, 392, 16, 16);
        // Dialogue Box Tiles
        public static Rectangle MenuBackground => GetTile(1, 2);
        public static Rectangle MenuBorderTop => GetTile(2, 0);
        public static Rectangle MenuBorderRight => GetTile(3, 2);
        public static Rectangle MenuBorderBottom => GetTile(2, 3);
        public static Rectangle MenuBorderLeft => GetTile(0, 2);
        public static Rectangle MenuBorderTopRight => GetTile(3, 0);
        public static Rectangle MenuBorderBottomRight => GetTile(3, 3);
        public static Rectangle MenuBorderBottomLeft => GetTile(0, 3);
        public static Rectangle MenuBorderTopLeft => GetTile(0, 0);

        /*********
        ** Public methods
        *********/
        public static IEnumerable<GameLocation> GetLocations()
        {
            return Game1.locations
                .Concat(
                    from location in Game1.locations.OfType<BuildableGameLocation>()
                    from building in location.buildings
                    where building.indoors.Value != null
                    select building.indoors.Value
                );
        }
        public static Rectangle GetTile(int x, int y) =>
            new Rectangle(
                Game1.tileSize * x,
                Game1.tileSize * y,
                Game1.tileSize,
                Game1.tileSize);
        public static void DrawDialogueBox(SpriteBatch b, int x, int y, int width, int height)
        {
            // Background
            b.Draw(
                Game1.menuTexture,
                new Rectangle(
                    x + Game1.tileSize / 2,
                    y + Game1.tileSize / 2,
                    width - Game1.tileSize,
                    height - Game1.tileSize),
                MenuBackground,
                Color.White);

            // Top Border
            b.Draw(
                Game1.menuTexture,
                new Rectangle(
                    x + Game1.tileSize,
                    y,
                    width - Game1.tileSize * 2,
                    Game1.tileSize),
                MenuBorderTop,
                Color.White);

            // Bottom Border
            b.Draw(
                Game1.menuTexture,
                new Rectangle(
                    x + Game1.tileSize,
                    y + height - Game1.tileSize,
                    width - Game1.tileSize * 2,
                    Game1.tileSize),
                MenuBorderBottom,
                Color.White);

            // Left Border
            b.Draw(
                Game1.menuTexture,
                new Rectangle(
                    x,
                    y + Game1.tileSize,
                    Game1.tileSize,
                    height - Game1.tileSize * 2),
                MenuBorderLeft,
                Color.White);

            // Right Border
            b.Draw(
                Game1.menuTexture,
                new Rectangle(
                    x + width - Game1.tileSize,
                    y + Game1.tileSize,
                    Game1.tileSize,
                    height - Game1.tileSize * 2),
                MenuBorderRight,
                Color.White);

            // Top-Right Corner
            b.Draw(
                Game1.menuTexture,
                new Vector2(x + width - Game1.tileSize, y),
                MenuBorderTopRight,
                Color.White);

            // Top-Left Corner
            b.Draw(
                Game1.menuTexture,
                new Vector2(x, y),
                MenuBorderTopLeft,
                Color.White);

            // Bottom-Right Corner
            b.Draw(
                Game1.menuTexture,
                new Vector2(x + width - Game1.tileSize, y + height - Game1.tileSize),
                MenuBorderBottomRight,
                Color.White);

            // Bottom-Left Corner
            b.Draw(
                Game1.menuTexture,
                new Vector2(x, y + height - Game1.tileSize),
                MenuBorderBottomLeft,
                Color.White);
        }

        public static void DrawInventoryIcon(SpriteBatch b, int x, int y)
        {
            b.Draw(Game1.mouseCursors,
                new Vector2(x, y + 60),
                new Rectangle(16, 368, 12, 16),
                Color.White,
                4.712389f,
                Vector2.Zero,
                Game1.pixelZoom,
                SpriteEffects.None,
                1f);
            b.Draw(Game1.mouseCursors,
                new Vector2(x, y + 28),
                new Rectangle(21, 368, 11, 16),
                Color.White,
                4.712389f,
                Vector2.Zero,
                Game1.pixelZoom,
                SpriteEffects.None,
                1f);
            b.Draw(Game1.mouseCursors,
                new Vector2(x + 24, y),
                new Rectangle(4, 372, 8, 11),
                Color.White,
                0.0f,
                Vector2.Zero,
                Game1.pixelZoom,
                SpriteEffects.None,
                1f);
        }
        public static T NonNull<T>(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return obj;
        }
    }
}
