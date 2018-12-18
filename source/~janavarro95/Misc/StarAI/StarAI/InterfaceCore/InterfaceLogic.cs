using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarAI.InterfaceCore
{
    public class InterfaceLogic
    {
        public static void interactWithCurrentMenu()
        {

            if (Game1.activeClickableMenu == null)
            {
                return;
            }

            //WindowsInput.InputSimulator.SimulateKeyUp(WindowsInput.VirtualKeyCode.ESCAPE);
            //ModCore.CoreMonitor.Log(Game1.activeClickableMenu.ToString());
            if(Game1.activeClickableMenu is StardewValley.Menus.DialogueBox)
            {
                Game1.activeClickableMenu.exitThisMenu(true);
                ModCore.CoreMonitor.Log("BYE");
            }

            if (Game1.activeClickableMenu is StardewValley.Menus.ShippingMenu)
            {
                //(Game1.activeClickableMenu as StardewValley.Menus.ShippingMenu).readyToClose
                //Game1.activeClickableMenu.exitThisMenu(true);
                ShippingMenuCore.closeMenu();
                ModCore.CoreMonitor.Log("Hello");
            }

        }


    }
}
