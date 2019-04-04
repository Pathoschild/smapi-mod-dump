using DeepWoodsMod.API.Impl;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;

namespace DeepWoodsMod
{
    class HealingFountain : LargeTerrainFeature
    {
        public HealingFountain()
           : base(false)
        {
        }

        public HealingFountain(Vector2 tile)
            : this()
        {
            this.tilePosition.Value = tile;
        }

        public override Rectangle getBoundingBox(Vector2 tileLocation)
        {
            return new Rectangle((int)tileLocation.X * 64, (int)tileLocation.Y * 64, 5 * 64, 3 * 64);
        }

        public override void draw(SpriteBatch spriteBatch, Vector2 tileLocation)
        {
            Vector2 globalPosition = tileLocation * 64f;

            Rectangle fountainTopSourceRectangle = new Rectangle(0, 0, 5 * 32, 2 * 32);
            Rectangle fountainBottomSourceRectangle = new Rectangle(0, 2 * 32, 5 * 32, 3 * 32);

            Vector2 globalTopPosition = new Vector2(globalPosition.X, globalPosition.Y - 2 * 64);
            Vector2 globalBottomPosition = new Vector2(globalPosition.X, globalPosition.Y);

            spriteBatch.Draw(DeepWoodsTextures.Textures.HealingFountain, Game1.GlobalToLocal(Game1.viewport, globalTopPosition), fountainTopSourceRectangle, Color.White, 0.0f, Vector2.Zero, 2f, SpriteEffects.None, ((tileLocation.Y + 1f) * 64f / 10000f + tileLocation.X / 100000f));
            spriteBatch.Draw(DeepWoodsTextures.Textures.HealingFountain, Game1.GlobalToLocal(Game1.viewport, globalBottomPosition), fountainBottomSourceRectangle, Color.White, 0.0f, Vector2.Zero, 2f, SpriteEffects.None, ((tileLocation.Y + 1f) * 64f / 10000f + tileLocation.X / 100000f));
        }

        public override bool performToolAction(Tool t, int damage, Vector2 tileLocation, GameLocation location)
        {
            return false;
        }

        public override bool performUseAction(Vector2 tileLocation, GameLocation location)
        {
            Game1.player.health = Game1.player.maxHealth;
            Game1.player.Stamina = Game1.player.MaxStamina;
            Game1.player.addedLuckLevel.Value = Math.Max(10, Game1.player.addedLuckLevel.Value);
            return true;
        }
    }
}
