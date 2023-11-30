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

class Santa : SkyObject
{

    public Santa(int yPos, bool moveRight) : base(new StardewValley.TemporaryAnimatedSprite(), yPos, moveRight)
    {
        this.Sprite.texture = Game1.content.Load<Texture2D>("LooseSprites\\Cursors");
        this.Sprite.sourceRect = new Rectangle(640, 800, 32, 16);
        this.Sprite.sourceRectStartingPos = new Vector2(this.Sprite.sourceRect.X, this.Sprite.sourceRect.Y);
        this.Sprite.interval = 100f;
        this.Sprite.animationLength = 2;
        this.Sprite.motion = new Vector2(moveRight ? 3f : -3f, 0f);
    }

    public override StardewValley.Object GetDropItem(int type = 0)
    {
        return new StardewValley.Object("(O)928", 1);
    }

    public override bool OnHit(StardewValley.Object ammo)
    {
        Game1.playSound("reward");
        this.Sprite.motion.X *= 2;
        this.Sprite.motion.Y = -3f;
        this.DropItem();
        return true;
    }

    public override void OnEnter()
    {
        if (ModEntry.Config.DoNotificationNoise)
        {
            Game1.playSound("moneyDial");
            DelayedAction.playSoundAfterDelay("moneyDial", 200);
            DelayedAction.playSoundAfterDelay("moneyDial", 400);
            DelayedAction.playSoundAfterDelay("moneyDial", 800);
            DelayedAction.playSoundAfterDelay("moneyDial", 1000);
            DelayedAction.playSoundAfterDelay("moneyDial", 1200);
        }
    }
}
