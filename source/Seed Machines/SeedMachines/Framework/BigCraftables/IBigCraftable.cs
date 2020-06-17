using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeedMachines.Framework.BigCraftables
{
    public abstract class IBigCraftable : StardewValley.Object
    {
        public static DateTime baseDate = new DateTime(1970, 1, 1);

        public int currentAnimationIndex = 0;
        public double timestampPreviousAnimation = -1;
        public StardewValley.Object baseObject;
        public IBigCraftableWrapper wrapper;

        public IBigCraftable(StardewValley.Object baseObject, IBigCraftableWrapper wrapper)
            : base(
                    baseObject.TileLocation,
                    baseObject.ParentSheetIndex
            )
        {
            this.baseObject = baseObject;
            this.wrapper = wrapper;

            this.animate();
        }

        private void animate()
        {
            if (timestampPreviousAnimation == -1)
            {
                timestampPreviousAnimation = (DateTime.Now - baseDate).TotalMilliseconds;
            }
            double timestampCurrentChecking = (DateTime.Now - baseDate).TotalMilliseconds;
            if (timestampCurrentChecking - timestampPreviousAnimation >= wrapper.millisecondsBetweenAnimation)
            {
                timestampPreviousAnimation = timestampCurrentChecking;
                changeFrame();
            }
        }

        private void changeFrame()
        {
            currentAnimationIndex = currentAnimationIndex < wrapper.maxAnimationIndex ? currentAnimationIndex + 1 : 0;
        }

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1)
        {
            if (isTemporarilyInvisible)
            {
                return;
            }
            Vector2 scaleFactor = getScale();
            scaleFactor *= 4f;
            Vector2 position = Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, y * 64 - 64));
            Microsoft.Xna.Framework.Rectangle destination =
                new Microsoft.Xna.Framework.Rectangle(
                    (int)(position.X - scaleFactor.X / 2f) + ((shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0),
                    (int)(position.Y - scaleFactor.Y / 2f) + ((shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0),
                    (int)(64f + scaleFactor.X), (int)(128f + scaleFactor.Y / 2f)
                );
            float draw_layer = Math.Max(0f, (float)((y + 1) * 64 - 24) / 10000f) + (float)x * 1E-05f;
            spriteBatch.Draw(
                Game1.bigCraftableSpriteSheet,
                destination,
                getSourceRectForBigCraftable(base.ParentSheetIndex + currentAnimationIndex),
                Color.White * alpha,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                draw_layer
            );
            animate();
        }

        public override void draw(SpriteBatch spriteBatch, int xNonTile, int yNonTile, float layerDepth, float alpha = 1)
        {
            if (isTemporarilyInvisible)
            {
                return;
            }
            Vector2 scaleFactor = getScale();
            scaleFactor *= 4f;
            Vector2 position = Game1.GlobalToLocal(Game1.viewport, new Vector2(xNonTile, yNonTile));
            Microsoft.Xna.Framework.Rectangle destination =
                new Microsoft.Xna.Framework.Rectangle(
                    (int)(position.X - scaleFactor.X / 2f) + ((shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0),
                    (int)(position.Y - scaleFactor.Y / 2f) + ((shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0),
                    (int)(64f + scaleFactor.X), (int)(128f + scaleFactor.Y / 2f)
                );
            spriteBatch.Draw(
                Game1.bigCraftableSpriteSheet,
                destination,
                getSourceRectForBigCraftable(base.ParentSheetIndex + currentAnimationIndex),
                Color.White * alpha,
                0f, Vector2.Zero,
                SpriteEffects.None,
                layerDepth
            );
            animate();
        }

        public abstract void onClick(ButtonPressedEventArgs args);
    }
}
