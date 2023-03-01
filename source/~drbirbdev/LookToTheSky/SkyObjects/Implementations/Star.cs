/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace LookToTheSky
{
    class Star : SkyObject
    {

        public Star(int xPos, int yPos) : base (new TemporaryAnimatedSprite(), 0, false)
        {
            this.Sprite.texture = Game1.content.Load<Texture2D>("LooseSprites\\Cursors");
            this.Sprite.position.X = xPos;
            this.Sprite.position.Y = yPos;
            this.Sprite.sourceRect = new Rectangle(366, 0, 5, 5);
            this.Sprite.sourceRectStartingPos = new Vector2(this.Sprite.sourceRect.X, this.Sprite.sourceRect.Y);
            this.Sprite.interval = 100f;
            this.Sprite.animationLength = 8;
        }

        public override StardewValley.Object GetDropItem(int type = 0)
        {
            return null;
        }

        public override bool OnHit(StardewValley.Object ammo)
        {
            return false;
        }
    }
}
