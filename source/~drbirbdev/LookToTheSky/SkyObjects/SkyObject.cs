/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using LookToTheSky.SkyObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;


namespace LookToTheSky;

public abstract class SkyObject
{
    public SkyObjectData SkyObjectData;

    public TemporaryAnimatedSprite Sprite;

    public int X => (int)this.Sprite.position.X;
    public int Y => (int)this.Sprite.position.Y;

    public int Hits = 0;

    public int Width => this.Sprite.sourceRect.Width * 4;

    public int Height => this.Sprite.sourceRect.Height * 4;

    public SkyObject(TemporaryAnimatedSprite sprite, int yPos, bool moveRight)
    {
        this.Sprite = sprite;
        this.Sprite.position.X = moveRight ? 0 : Game1.viewport.Width;
        this.Sprite.position.Y = Game1.viewport.Height / 100 * yPos;
        this.Sprite.flipped = moveRight;
        this.Sprite.totalNumberOfLoops = 9999;
        this.Sprite.scale = 4;
        this.Sprite.local = true;
        this.OnEnter();
    }

    /// <summary>
    /// What happens when an object is hit?
    /// </summary>
    /// <param name="ammo">The ammo type which hit the object.</param>
    /// <returns>whether to delete the projectile that hit the object</returns>
    public abstract bool OnHit(StardewValley.Object ammo);

    /// <summary>
    /// Check if SkyObject was hit by a projectile
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public virtual bool IsHit(Vector2 point)
    {
        return point.X > this.X && point.X < this.X + this.Width && point.Y > this.Y && point.Y < this.Y + this.Height;
    }

    /// <summary>
    /// Updates which happen each frame, movement and collision checks, etc
    /// </summary>
    public virtual void tick()
    {
        this.Sprite.update(Game1.currentGameTime);
        foreach (SkyProjectile projectile in ModEntry.Instance.Projectiles)
        {
            if (this.IsHit(new Vector2(projectile.X, projectile.Y)))
            {
                if (this.OnHit(projectile.Ammo))
                {
                    ModEntry.Instance.Projectiles.Remove(projectile);
                }
                break;
            }
        }
        if (this.X < 0 - (this.Width * this.Sprite.scale))
        {

        }
        else if (this.X > Game1.viewport.Width + (this.Width * this.Sprite.scale))
        {

        }
        else if (this.Y < 0 - (this.Height * this.Sprite.scale))
        {

        }
        else if (this.Y > Game1.viewport.Height + (this.Height * this.Sprite.scale))
        {

        }
    }

    public abstract StardewValley.Object GetDropItem(int type = 0);

    public virtual void DropItem(int type = 0)
    {
        this.Hits++;
        if (this.Hits > this.MaxDrops())
        {
            return;
        }
        StardewValley.Object drop = this.GetDropItem(type);
        ModEntry.Instance.SkyObjects.Add(new Drop(drop, this.X, this.Y));
    }

    /// <summary>
    /// Action to do when object leaves screen.
    /// </summary>
    public virtual void OnExit()
    {
        return;
    }

    /// <summary>
    /// Action to do when object enters screen.
    /// </summary>
    public virtual void OnEnter()
    {
        return;
    }

    /// <summary>
    /// Draw the object on screen
    /// </summary>
    public virtual void draw(SpriteBatch b)
    {
        this.Sprite.draw(b, true);
    }

    public virtual int MaxDrops()
    {
        return 1;
    }
}
