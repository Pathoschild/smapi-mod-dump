/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/maxvollmer/DeepWoodsMod
**
*************************************************/


using DeepWoodsMod.API.Impl;
using DeepWoodsMod.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace DeepWoodsMod.Stuff
{
    public class BigWoodenSign : LargeTerrainFeature
    {
        public BigWoodenSign()
           : base(false)
        {
        }

        public BigWoodenSign(Vector2 tileLocation)
            : this()
        {
            this.tilePosition.Value = tileLocation;
        }

        public override bool isActionable()
        {
            return true;
        }

        public override Rectangle getBoundingBox(Vector2 tileLocation)
        {
            return new Rectangle((int)tileLocation.X * 64 + 8, (int)tileLocation.Y * 64, 128 - 24, 64);
        }

        public override bool isPassable(Character c = null)
        {
            return false;
        }

        public override bool performUseAction(Vector2 tileLocation, GameLocation location)
        {
            DeepWoodsQuestMenu.OpenQuestMenuWithModInfo(I18N.BigWoodenSignMessage, new Response[1]
            {
                new Response("No", I18N.MessageBoxClose).SetHotKey(Keys.Escape)
            });

            return true;
        }

        public override bool tickUpdate(GameTime time, Vector2 tileLocation, GameLocation location)
        {
            return false;
        }

        public override void dayUpdate(GameLocation environment, Vector2 tileLocation)
        {
        }

        public override bool seasonUpdate(bool onLoad)
        {
            return false;
        }

        public override bool performToolAction(Tool t, int explosion, Vector2 tileLocation, GameLocation location)
        {
            return false;
        }

        public override void performPlayerEntryAction(Vector2 tileLocation)
        {
        }

        public override void draw(SpriteBatch spriteBatch, Vector2 tileLocation)
        {
            Vector2 globalPosition = tileLocation * 64f;

            // 28 x 40 pixel

            Rectangle bottomSourceRectangle = new Rectangle(5, 28, 28, 22);
            Vector2 globalBottomPosition = new Vector2(globalPosition.X, globalPosition.Y);

            Rectangle topSourceRectangle = new Rectangle(5, 8, 28, 20);
            Vector2 globalTopPosition = new Vector2(globalPosition.X, globalPosition.Y - 80);

            spriteBatch.Draw(DeepWoodsTextures.Textures.BigWoodenSign, Game1.GlobalToLocal(Game1.viewport, globalTopPosition), topSourceRectangle, Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, ((tileLocation.Y + 1f) * 64f / 10000f + tileLocation.X / 100000f));
            spriteBatch.Draw(DeepWoodsTextures.Textures.BigWoodenSign, Game1.GlobalToLocal(Game1.viewport, globalBottomPosition), bottomSourceRectangle, Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, ((tileLocation.Y + 1f) * 64f / 10000f + tileLocation.X / 100000f));
        }
    }
}
