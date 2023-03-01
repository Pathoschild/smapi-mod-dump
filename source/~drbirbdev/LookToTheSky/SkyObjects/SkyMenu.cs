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
using StardewValley.Menus;
using StardewValley.Tools;

namespace LookToTheSky
{
    class SkyMenu : IClickableMenu
    {


        public SkyMenu()
        {
            base.initialize(0, 0, Game1.viewport.Width, Game1.viewport.Height);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (Game1.player.CurrentTool is Slingshot slingshot)
            {
                if (slingshot.attachments[0] is null)
                {
                    // TODO: figure out better pitch or sound
                    Game1.playSoundPitched("clam_tone", 0);
                    return;
                }
                int speed = 1;
                if (slingshot.InitialParentTileIndex == 33)
                {
                    speed = 2;
                }
                else if (slingshot.InitialParentTileIndex == 34)
                {
                    speed = 3;
                }
                StardewValley.Object ammunition = (StardewValley.Object)slingshot.attachments[0].getOne();
                string collisionSound = "hammer";
                if (--slingshot.attachments[0].Stack <= 0)
                {
                    slingshot.attachments[0] = null;
                }
                if (ammunition.ParentSheetIndex >= 378 && ammunition.ParentSheetIndex <= 390)
                {
                    ammunition.ParentSheetIndex++;
                }
                if (ammunition.ParentSheetIndex == 441)
                {
                    collisionSound = "explosion";
                }
                else if (ammunition.Category == -5)
                {
                    collisionSound = "slimedead";
                }
                ModEntry.Instance.Projectiles.Add(new SkyProjectile(ammunition.ParentSheetIndex, x - 32, collisionSound, ammunition, y, speed));
            }
        }

        public override void draw(SpriteBatch b)
        {
            drawBackground(b);
            foreach (SkyObject skyObject in ModEntry.Instance.SkyObjects)
            {
                skyObject.draw(b);
            }
            foreach (SkyProjectile projectile in ModEntry.Instance.Projectiles)
            {
                projectile.draw(b);
            }
            if (Game1.player.CurrentTool is Slingshot)
            {
                b.Draw(Game1.mouseCursors, new Vector2(Game1.getMouseX(), Game1.getMouseY()), Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 43), Color.White, 0f, new Vector2(32f, 32f), 1f, SpriteEffects.None, 0.999999f);
            }
            else
            {
                drawMouse(b, true);
            }
        }
    }
}
