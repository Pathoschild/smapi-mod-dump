/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardustCore.UIUtilities.MenuComponents.ComponentsV2;

namespace Revitalize.Framework.Minigame.SeasideScrambleMinigame.SSCMenus
{
    public class TitleScreen: StardustCore.UIUtilities.IClickableMenuExtended
    {
        StardustCore.UIUtilities.Texture2DExtended background;
        BlinkingText menuText;

        public bool closeMenu;

        public TitleScreen(int x, int y, int width, int height):base(x,y,width,height,false)
        {
            this.background = SeasideScramble.self.textureUtils.getExtendedTexture("SSCMaps", "TitleScreenBackground");
            this.menuText = new BlinkingText("Sea Side Scramble: Lite Edition" + System.Environment.NewLine + "Click or press A to start.",1000);
        }

        public TitleScreen(xTile.Dimensions.Rectangle viewport) : this(0, 0, viewport.Width, viewport.Height)
        {

        }

        /// <summary>
        /// What happens when the game's window size changes.
        /// </summary>
        /// <param name="oldBounds"></param>
        /// <param name="newBounds"></param>
        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            this.xPositionOnScreen = newBounds.X;
            this.yPositionOnScreen = newBounds.Y;
            this.width = newBounds.Width;
            this.height = newBounds.Height;
            base.gameWindowSizeChanged(oldBounds, newBounds);
        }


        /// <summary>
        /// What happens when the menu receives a left click.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="playSound"></param>
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (SeasideScramble.self.menuManager.isThisActiveMenu(this) == false) return;
            SeasideScramble.self.menuManager.addNewMenu(new CharacterSelectScreen(SeasideScramble.self.camera.viewport));
        }

        /// <summary>
        /// Checks if the menu is ready to close.
        /// </summary>
        /// <returns></returns>
        public override bool readyToClose()
        {
            if (this.closeMenu == true)
            {
                return true;
            }
            //When menu is closed!
            return false;
        }

        /// <summary>
        /// Updates the menu.
        /// </summary>
        /// <param name="time"></param>
        public override void update(GameTime time)
        {
            if (SeasideScramble.self.menuManager.isThisActiveMenu(this) == false) return;
            this.menuText.update(time);   
        }

        /// <summary>
        /// Draws the menu to the screen.
        /// </summary>
        /// <param name="b"></param>
        public override void draw(SpriteBatch b)
        {
            if (SeasideScramble.self.menuManager.isThisActiveMenu(this) == false) return;
            this.drawTitleBackground(b);
            this.drawTitleText(b);
            this.drawMouse(b);
        }

        /// <summary>
        /// Draws the background for the title screen.
        /// </summary>
        /// <param name="b"></param>
        public void drawTitleBackground(SpriteBatch b)
        {
            b.Draw(this.background.texture,new Vector2(this.xPositionOnScreen,this.yPositionOnScreen),SeasideScramble.self.camera.getXNARect() ,Color.White);
            //this.drawDialogueBoxBackground(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, Color.Black);
        }

        /// <summary>
        /// Draws the text for the title screen.
        /// </summary>
        /// <param name="b"></param>
        public void drawTitleText(SpriteBatch b)
        {
            Vector2 offset=StardewValley.Game1.dialogueFont.MeasureString(this.menuText.displayText);
            this.menuText.draw(b, StardewValley.Game1.dialogueFont, new Vector2((this.width / 2) - (offset.X / 2), this.height / 2), Color.White);
           
        }

        public override void exitMenu(bool playSound = true)
        {
            base.exitMenu(playSound);
        }

    }
}
