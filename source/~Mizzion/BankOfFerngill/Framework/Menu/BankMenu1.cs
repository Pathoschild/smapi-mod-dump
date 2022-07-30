/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace BankOfFerngill.Framework.Menu
{
    internal class BankMenu1 : IClickableMenu
    {
        /*
         * This menu is based on the CheatMenu from CJB Cheat Menu which can be found at https://github.com/CJBok/SDV-Mods/tree/5f96066858f230d2396540457e352ecc8660287c/CJBCheatsMenu.
         * Credit goes to the authors of that mod.
         */
        
        //Lets grab the MainMod monitor, so we can log 
        private readonly IMonitor Monitor;
        
        //Menu Items
        private readonly List<ClickableComponent> _bankOptions = new();
        private ClickableComponent _bankTitle;
        private TextBox _textBox;
        private TextBoxEvent _tbEvent;
        private readonly List<ClickableComponent> Tabs = new();

        private string HoverText = "";
        private bool _justOpened = true;
        
        //Now we make the menu happen
        
        public MenuTabs CurrentTab { get; }

        public BankMenu1(MenuTabs initialTab, IMonitor monitor, bool isNewMenu)
        {
            this.Monitor = monitor;
            this.CurrentTab = initialTab;
            //this.ResetComponents();
            //this.SetOptions();

            //this.ResetComponents();
        }

        public void ExitIfValid()
        {
            if (this.readyToClose() && !GameMenu.forcePreventClose)
            {
                Game1.exitActiveMenu();
            }
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
           // this.ResetComponents();
        }

        public override void leftClickHeld(int x, int y)
        {
            if (GameMenu.forcePreventClose)
                return;
            base.leftClickHeld(x,y);
        }

        public override void releaseLeftClick(int x, int y)
        {
            if (GameMenu.forcePreventClose)
                return;
            base.releaseLeftClick(x, y);
            
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (GameMenu.forcePreventClose)
                return;
            base.receiveLeftClick(x, y, playSound);

            foreach (ClickableComponent tab in this.Tabs)
            {
                if (tab.bounds.Contains(x, y))
                {
                    MenuTabs tabID = this.GetTabID(tab);
                    Game1.activeClickableMenu = new BankMenu1(tabID, this.Monitor, isNewMenu: false);
                    break;
                }
            }
        }

        public override void performHoverAction(int x, int y)
        {
            if (GameMenu.forcePreventClose)
                return;
            this.HoverText = "";
            
        }

        public override void draw(SpriteBatch b)
        {
            if(!Game1.options.showMenuBackground)
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);
            base.draw(b);
            
        }
        
        //Custom Methods

        private MenuTabs GetTabID(ClickableComponent tab)
        {
            if (!Enum.TryParse(tab.name, out MenuTabs tabID))
                throw new InvalidOperationException($"Couldn't parse the tab name: {tab.name}");
            return tabID;
        }
    }
    
    //Tab enum
    internal enum MenuTabs
    {
        BankInfo,
        Deposit,
        Withdraw,
        TakeOutLoan,
        PayBackLoan
    }
}