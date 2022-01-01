/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Bouhm/stardew-valley-mods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace NPCMapLocations.Framework.Menus
{
    // Mod button for the three main modes
    internal class MapModButton : OptionsElement
    {
        /*********
        ** Fields
        *********/
        private bool IsActive;


        /*********
        ** Accessors
        *********/
        public Rectangle Rect { get; set; }


        /*********
        ** Public methods
        *********/
        public MapModButton(string label, int whichOption, int x, int y, int width, int height)
            : base(label, x, y, 9 * Game1.pixelZoom, 9 * Game1.pixelZoom, whichOption)
        {
            this.Rect = new Rectangle(x, y, width, height);

            this.greyedOut = ModEntry.Config.ImmersionOption != whichOption - 2;
        }

        public override void receiveLeftClick(int x, int y)
        {
            if (!this.IsActive)
            {
                if (this.whichOption == 3 || this.whichOption == 4 || this.whichOption == 5)
                {
                    Game1.playSound("drumkit6");
                    base.receiveLeftClick(x, y);
                    this.IsActive = true;
                    this.greyedOut = false;
                    ModEntry.Config.ImmersionOption = this.whichOption - 2;
                }
            }
        }

        public void GreyOut()
        {
            this.IsActive = false;
            this.greyedOut = true;
        }

        public override void draw(SpriteBatch b, int slotX, int slotY, IClickableMenu context = null)
        {
            base.draw(b, slotX - 32, slotY, context);
        }
    }
}
