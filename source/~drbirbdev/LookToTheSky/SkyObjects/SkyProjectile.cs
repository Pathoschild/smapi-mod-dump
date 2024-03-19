/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Projectiles;

namespace LookToTheSky;

public class SkyProjectile(
    int parentSheetIndex,
    int xPos,
    string collisionSound,
    StardewValley.Object ammo,
    int clickY = 0,
    int speed = 1)
    : BasicProjectile(0, parentSheetIndex, 0, 0, (float)(Math.PI / (64f + Game1.random.Next(-63, 64))), 0,
        -12 * speed, new Vector2(xPos, Game1.viewport.Height), collisionSound, null, null, false, false, null,
        Game1.player)
{
    private static readonly Color[] COLORS =
    [
        Color.White,
        Color.Red,
        Color.Yellow,
        Color.Orange,
        Color.Blue,
        Color.Purple,
        Color.CornflowerBlue,
        Color.GreenYellow,
        Color.Green,
        Color.Aquamarine
    ];

    public int X => (int)this.position.X + 32;
    public int Y => (int)this.position.Y + 32;

    public StardewValley.Object Ammo = ammo;

    public override void draw(SpriteBatch b)
    {
        b.Draw(Game1.objectSpriteSheet, new Rectangle(this.X - 32, this.Y - 32, 64, 64), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, this.currentTileSheetIndex.Value, 16, 16), Color.White); //, this.rotation, Game1.GlobalToLocal(Game1.viewport, Game1.player.position), 4, SpriteEffects.None, 0);
    }

    public bool UpdatePosition(GameTime time)
    {
        this.updatePosition(time);
        if (this.Y > clickY)
        {
            return false;
        }

        switch (this.currentTileSheetIndex.Value)
        {
            case 441:
                // Add firework
                ModEntry.Instance.SkyObjects.Add(new Firework(this.X - 64, clickY - 64, COLORS[Game1.random.Next(COLORS.Length)]));
                Game1.playSound("explosion");
                return true;
            case 387:
                // Add a star
                ModEntry.Instance.SkyObjects.Add(new Star(this.X - 10, clickY - 10));
                return true;
        }
        return false;
    }
}
