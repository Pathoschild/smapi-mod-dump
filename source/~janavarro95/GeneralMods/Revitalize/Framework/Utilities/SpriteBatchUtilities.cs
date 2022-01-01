/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Revitalize;
using Revitalize.Framework.World.Objects;
using StardewValley;

namespace Revitalize.Framework.Utilities
{
    /// <summary>
    /// Utilities for dealing with the XNA/Monogames spritebatch class.
    /// </summary>
    public class SpriteBatchUtilities
    {

        /// <summary>
        /// Used to draw SDV items to the screen.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="obj"></param>
        /// <param name="itemToDraw"></param>
        /// <param name="alpha"></param>
        /// <param name="addedDepth"></param>
        public static void Draw(SpriteBatch spriteBatch, CustomObject obj, Item itemToDraw, float alpha, float addedDepth)
        {
            if (itemToDraw.GetType() == typeof(StardewValley.Object))
            {
                Rectangle rectangle;
                SpriteBatch spriteBatch1 = spriteBatch;
                Texture2D shadowTexture = Game1.shadowTexture;
                Vector2 position = Game1.GlobalToLocal(Game1.viewport, new Vector2(obj.TileLocation.X, obj.TileLocation.Y));
                Rectangle? sourceRectangle = new Rectangle?(Game1.shadowTexture.Bounds);
                Color color = Color.White * alpha;
                rectangle = Game1.shadowTexture.Bounds;
                double x1 = rectangle.Center.X;
                rectangle = Game1.shadowTexture.Bounds;
                double y1 = rectangle.Center.Y;
                Vector2 origin = new Vector2((float)x1, (float)y1);
                double num = obj.boundingBox.Bottom / 10000.0;
                spriteBatch1.Draw(shadowTexture, position, sourceRectangle, color, 0.0f, origin, 4f, SpriteEffects.None, (float)num);
                spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, obj.TileLocation * Game1.tileSize), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, itemToDraw.ParentSheetIndex, 16, 16), Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (obj.boundingBox.Bottom + 1) / 10000f + addedDepth);

                (itemToDraw as StardewValley.Object).draw(spriteBatch, (int)obj.TileLocation.X * Game1.tileSize, (int)obj.TileLocation.Y * Game1.tileSize, Math.Max(0f, (obj.TileLocation.Y + 1 + addedDepth) * Game1.tileSize / 10000f) + .0001f, alpha);
            }
            if (ModCore.Serializer.IsSameOrSubclass(typeof(CustomObject), itemToDraw.GetType()))
                (itemToDraw as CustomObject).draw(spriteBatch, (int)obj.TileLocation.X, (int)obj.TileLocation.Y);
        }

    }
}
