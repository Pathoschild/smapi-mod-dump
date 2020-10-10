/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

using Igorious.StardewValley.ColoredChestsMod.Utils;
using Igorious.StardewValley.DynamicAPI.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Objects;

namespace Igorious.StardewValley.ColoredChestsMod.SmartObjects
{
    public sealed class ColoredChest : Chest, ISmartObject
    {
        public ColoredChest() : base(true) { }

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1)
        {
            if (tint == Color.White)
            {
                base.draw(spriteBatch, x, y, alpha);
                return;
            }
            
            DrawSprite(spriteBatch, x, y, Game1.bigCraftableSpriteSheet, ParentSheetIndex, Color.White, 0.00041f);
            DrawSprite(spriteBatch, x, y, Game1.bigCraftableSpriteSheet, currentLidFrame, Color.White, 0.00051f);
            DrawSprite(spriteBatch, x, y, Textures.ChestTint, 0, tint, 0.00042f);
            DrawSprite(spriteBatch, x, y, Textures.ChestTint, currentLidFrame - ParentSheetIndex, tint, 0.00052f);
        }

        private void DrawSprite(SpriteBatch spriteBatch, int x, int y, Texture2D texture, int index, Color color, float depthDelta)
        {
            spriteBatch.Draw(
                texture,
                Game1.GlobalToLocal(Game1.viewport, new Vector2(x * Game1.tileSize + (shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0), (y - 1) * Game1.tileSize)),
                Game1.getSourceRectForStandardTileSheet(texture, index, 16, 32),
                color,
                0,
                Vector2.Zero,
                Game1.pixelZoom,
                SpriteEffects.None,
                y * Game1.tileSize / 10000f + depthDelta);
        }
    }
}