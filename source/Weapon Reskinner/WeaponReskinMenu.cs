/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/WeaponReskinner
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewValley.Tools;

namespace WeaponReskinner
{
    public class WeaponReskinMenu : MenuWithInventory
    {
        private ClickableComponent baseItemClickable;
        private ClickableComponent skinItemClickable;

        public WeaponReskinMenu()
        :   base(highlighterMethod: highlightWeapon, okButton: true)
        {
            baseItemClickable = new ClickableComponent(new Rectangle(xPositionOnScreen + 200, yPositionOnScreen + 100, 64, 64), "Base Item");
            skinItemClickable = new ClickableComponent(new Rectangle(xPositionOnScreen + 500, yPositionOnScreen + 100, 64, 64), "Skin Item");
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if ( okButton.containsPoint(x, y) )
            {
                doReskinning();
            }
            else if ( baseItemClickable.containsPoint( x, y ) )
            {
                if (Game1.player.addItemToInventoryBool(baseItemClickable.item))
                    baseItemClickable.item = null;
            }
            else if ( skinItemClickable.containsPoint( x, y ) )
            {
                if (Game1.player.addItemToInventoryBool(skinItemClickable.item))
                    skinItemClickable.item = null;
            }
            else if ( baseItemClickable.item == null || skinItemClickable.item == null )
            {
                Item clicked = inventory.leftClick(x, y, null);
                if ( clicked != null )
                {
                    if (baseItemClickable.item == null)
                    {
                        baseItemClickable.item = clicked;
                    }
                    else
                    {
                        skinItemClickable.item = clicked;
                    }
                }
            }
        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b, false, false);

            drawTextureBox(b, baseItemClickable.bounds.X - 150, baseItemClickable.bounds.Y - 100, 700, 250, Color.White);
            SpriteText.drawStringHorizontallyCenteredAt(b, "Base Weapon", baseItemClickable.bounds.X + 32, baseItemClickable.bounds.Y - 64 );
            drawTextureBox(b, baseItemClickable.bounds.X - 16, baseItemClickable.bounds.Y - 16, baseItemClickable.bounds.Width + 32, baseItemClickable.bounds.Height + 32, Color.White);
            baseItemClickable.item?.drawInMenu(b, new Vector2(baseItemClickable.bounds.X, baseItemClickable.bounds.Y), 1);
            SpriteText.drawStringHorizontallyCenteredAt(b, "Weapon Skin", skinItemClickable.bounds.X + 32, skinItemClickable.bounds.Y - 64 );
            drawTextureBox(b, skinItemClickable.bounds.X - 16, skinItemClickable.bounds.Y - 16, skinItemClickable.bounds.Width + 32, skinItemClickable.bounds.Height + 32, Color.White);
            skinItemClickable.item?.drawInMenu(b, new Vector2(skinItemClickable.bounds.X, skinItemClickable.bounds.Y), 1);

            this.drawMouse(b);
        }

        protected override void cleanupBeforeExit()
        {
            emergencyShutDown();
        }

        public override void emergencyShutDown()
        {
            if (baseItemClickable.item != null)
                Utility.CollectOrDrop(baseItemClickable.item);
            if (skinItemClickable.item != null)
                Utility.CollectOrDrop(skinItemClickable.item);
        }

        private static bool highlightWeapon(Item item)
        {
            return item is MeleeWeapon;
        }

        private void doReskinning()
        {
            if (baseItemClickable.item != null && skinItemClickable.item != null )
            {
                var baseWeapon = baseItemClickable.item as MeleeWeapon;
                var skinWeapon = skinItemClickable.item as MeleeWeapon;
                baseWeapon.InitialParentTileIndex = skinWeapon.InitialParentTileIndex;
                baseWeapon.IndexOfMenuItemView = skinWeapon.IndexOfMenuItemView;
                baseWeapon.description = skinWeapon.description + "\nOriginally a " + baseWeapon.DisplayName;
                baseWeapon.Name = skinWeapon.Name;
                baseWeapon.DisplayName = skinWeapon.DisplayName;

                Game1.player.addItemByMenuIfNecessary(baseItemClickable.item);
            }
            else if (baseItemClickable.item == null )
                Game1.player.addItemByMenuIfNecessary(skinItemClickable.item);
            else if (skinItemClickable.item == null )
                Game1.player.addItemByMenuIfNecessary(baseItemClickable.item);

            Game1.activeClickableMenu = null;
        }
    }
}