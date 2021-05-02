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
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardustCore.UIUtilities;
using StardustCore.UIUtilities.MenuComponents.ComponentsV2.Buttons;

namespace Revitalize.Framework.Menus.Machines
{
    /// <summary>
    /// Need to 
    /// </summary>
    public class MachineMenu : IClickableMenuExtended
    {

        public Dictionary<string,KeyValuePair<AnimatedButton,IClickableMenuExtended>> menuPages;
        public string currentTab;
        public string hoverText;


        public IClickableMenuExtended CurrentMenu
        {
            get
            {
                if(string.IsNullOrEmpty(this.currentTab)) return null;
                else
                {
                    if (this.menuPages.ContainsKey(this.currentTab))
                    {
                        return this.menuPages[this.currentTab].Value;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }


        public MachineMenu()
        {

        }

        public MachineMenu(int x, int y, int width, int height) : base(x, y, width, height, false)
        {
            this.menuPages = new Dictionary<string, KeyValuePair<AnimatedButton, IClickableMenuExtended>>();
        }

        public void addInMenuTab(string Name,AnimatedButton Button, IClickableMenuExtended Menu,bool DefaultTab=false)
        {
            int count = this.menuPages.Count;

            Vector2 newPos = new Vector2(208 + (24 * 2) * (count+1), this.yPositionOnScreen+(80));
            Button.Position = newPos;
            this.menuPages.Add(Name,new KeyValuePair<AnimatedButton, IClickableMenuExtended>(Button, Menu));

            if (DefaultTab)
            {
                this.currentTab = Name;
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            foreach (var v in this.menuPages)
            {
                if (v.Value.Key.containsPoint(x, y))
                {
                    this.currentTab = v.Key;
                }
            }

            if (this.CurrentMenu != null)
            {
                this.CurrentMenu.receiveLeftClick(x, y, playSound);
            }

            
        }
        public override void receiveKeyPress(Keys key)
        {
            if (this.CurrentMenu != null)
            {
                this.CurrentMenu.receiveKeyPress(key);
            }
        }

        public override void performHoverAction(int x, int y)
        {

            bool hovered = false;

            foreach(var v in this.menuPages)
            {
                if (v.Value.Key.containsPoint(x, y))
                {
                    this.hoverText = v.Key;
                    hovered = true;
                }
            }

            if (this.CurrentMenu != null)
            {
                this.CurrentMenu.performHoverAction(x, y);
            }

            if (hovered == false)
            {
                this.hoverText = "";
            }
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            if (this.CurrentMenu != null)
            {
                this.CurrentMenu.receiveRightClick(x, y,playSound);
            }
        }

        public override void update(GameTime time)
        {
            if (this.CurrentMenu != null)
            {
                this.CurrentMenu.update(time);
            }
        }

        public override void draw(SpriteBatch b)
        {
            foreach(KeyValuePair<AnimatedButton,IClickableMenuExtended> button in this.menuPages.Values)
            {
                button.Key.draw(b);
            }
            if (this.CurrentMenu != null)
            {
                this.CurrentMenu.draw(b);
            }

            if (string.IsNullOrEmpty(this.hoverText) == false)
            {
                IClickableMenuExtended.drawHoverText(b, this.hoverText, Game1.dialogueFont);
            }

            this.drawMouse(b);
        }

        public void updateInventoryMenuIfPossible()
        {
            if (this.menuPages.ContainsKey("Inventory"))
            {
                (this.menuPages["Inventory"].Value as InventoryTransferMenu).updateInventory();
            }
        }
    }
}
