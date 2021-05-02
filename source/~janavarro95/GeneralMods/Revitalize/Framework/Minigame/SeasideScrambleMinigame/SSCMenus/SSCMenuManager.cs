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
using Microsoft.Xna.Framework.Graphics;
using StardustCore.UIUtilities;

namespace Revitalize.Framework.Minigame.SeasideScrambleMinigame.SSCMenus
{
    /// <summary>
    /// Deals with handling all menus for the minigame.
    /// </summary>
    public class SSCMenuManager
    {

        /// <summary>
        /// Gets the active menu aka the top of the menu stack.
        /// </summary>
        public IClickableMenuExtended activeMenu
        {
            get
            {
                if (this.menus == null) return null;
                if (this.menus.Count == 0)
                {
                    return null;
                }
                return this.menus.Peek();
            }
        }

        /// <summary>
        /// Checks if there is a menu up.
        /// </summary>
        public bool isMenuUp
        {
            get
            {
                if (this.menus == null) return false;
                return this.menus.Count > 0;
            }

        }

        /// <summary>
        /// A stack that controlls all active menus.
        /// </summary>
        public Stack<IClickableMenuExtended> menus;


        public SSCMenuManager()
        {
            this.menus = new Stack<IClickableMenuExtended>();
        }

        /// <summary>
        /// Adds a new menu to the menu stack.
        /// </summary>
        /// <param name="menu"></param>
        public void addNewMenu(IClickableMenuExtended menu)
        {
            this.menus.Push(menu);
        }

        /// <summary>
        /// Closes the top most active menu on the menu stack.
        /// </summary>
        public void closeActiveMenu()
        {
            if (this.menus == null)
            {
                return;
            }
            if (this.menus.Count == 0) return;
            IClickableMenuExtended m = this.menus.Pop();
            m.exitMenu();
        }

        /// <summary>
        /// Closes all menus in the menu stack.
        /// </summary>
        public void closeAllMenus()
        {
            if (this.menus == null)
            {
                return;
            }
            if (this.menus.Count == 0) return;
            while (this.isMenuUp)
            {
                this.closeActiveMenu();
            }
        }

        /// <summary>
        /// Closes menus until the passed in menu is on top of the stack.
        /// </summary>
        /// <param name="menu"></param>
        public void closeUntilThisMenu(IClickableMenuExtended menu)
        {
            while (this.activeMenu != menu)
            {
                this.closeActiveMenu();
            }
        }

        /// <summary>
        /// Checks if the give menu is the active menu.
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        public bool isThisActiveMenu(IClickableMenuExtended menu)
        {
            return this.menus.Peek() == menu;
        }

        /// <summary>
        /// Draws all menus in the menu stack.
        /// </summary>
        /// <param name="b"></param>
        public void drawAll(SpriteBatch b)
        {
            for(int i = this.menus.Count - 1; i>=0; i--)
            {
                this.menus.ElementAt(i).draw(b);
            }
        }

    }
}
