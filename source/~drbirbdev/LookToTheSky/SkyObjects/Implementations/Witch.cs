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

namespace LookToTheSky;

class Witch : SkyObject
{
    public Witch(int yPos, bool moveRight) : base(new StardewValley.TemporaryAnimatedSprite(), yPos, moveRight)
    {
        this.Sprite.texture = Game1.content.Load<Texture2D>("LooseSprites\\Cursors");
        this.Sprite.sourceRect = new Rectangle(276, 1886, 35, 29);
        this.Sprite.sourceRectStartingPos = new Vector2(this.Sprite.sourceRect.X, this.Sprite.sourceRect.Y);
        this.Sprite.interval = 100f;
        this.Sprite.animationLength = 1;
        this.Sprite.motion = new Vector2(moveRight ? 3f : -3f, 0f);
    }

    public override StardewValley.Object GetDropItem(int type = 0)
    {
        return new StardewValley.Object("(O)305", 1);
    }

    public override bool OnHit(StardewValley.Object ammo)
    {
        Game1.playSound("coldSpell");
        this.Sprite.motion.Y = -3f;
        this.DropItem();
        return true;
    }

    public override void OnEnter()
    {
        if (ModEntry.Config.DoNotificationNoise)
        {
            Game1.playSound("cacklingWitch");
        }
    }
}
