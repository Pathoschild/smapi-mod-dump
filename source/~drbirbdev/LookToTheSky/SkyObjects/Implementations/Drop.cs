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
using StardewValley;

namespace LookToTheSky
{
    class Drop : SkyObject
    {

        public StardewValley.Object Dropped;

        public Drop(StardewValley.Object drop, int xPos, int yPos) : base(new TemporaryAnimatedSprite(), 0, false)
        {
            this.Sprite.texture = Game1.objectSpriteSheet;
            this.Sprite.sourceRect = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, drop.ParentSheetIndex, 16, 16);
            this.Sprite.position.X = xPos;
            this.Sprite.position.Y = yPos;
            this.Sprite.sourceRectStartingPos = new Vector2(this.Sprite.sourceRect.X, this.Sprite.sourceRect.Y);
            this.Sprite.interval = 100f;
            this.Sprite.animationLength = 1;
            this.Sprite.rotationChange = 0.1f;
            this.Sprite.acceleration = new Vector2(0, 1f);
            this.Sprite.motion = new Vector2(0, -1f);
            this.Dropped = drop;
        }

        public override StardewValley.Object GetDropItem(int type = 0)
        {
            return null;
        }

        public override bool OnHit(StardewValley.Object ammo)
        {
            return false;
        }

        public override void OnExit()
        {
            Game1.createItemDebris(this.Dropped, Game1.player.position, 2, Game1.currentLocation);
        }
    }
}
