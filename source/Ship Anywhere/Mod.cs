using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace ShipAnywhere
{
    public class Mod : StardewModdingAPI.Mod
    {
        public static Mod instance;
        public static Configuration Config;

        public override void Entry(IModHelper helper)
        {
            base.Entry(helper);
            instance = this;

            Config = helper.ReadConfig<Configuration>();

            ControlEvents.KeyPressed += onKeyPress;
        }

        private void onKeyPress(object mod, EventArgsKeyPressed args)
        {
            if (!Context.IsWorldReady || !Context.IsPlayerFree)
                return;

            if ( args.KeyPressed == Config.OpenShippingBox.key )
            {
                var func = typeof(Farm).GetMethod("shipItem", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                var del = (ItemGrabMenu.behaviorOnItemSelect) Delegate.CreateDelegate(typeof(ItemGrabMenu.behaviorOnItemSelect), Game1.getFarm(), func);
                
                ItemGrabMenu itemGrabMenu = new ItemGrabMenu((List<Item>)null, true, false, new InventoryMenu.highlightThisItem(Utility.highlightShippableObjects), del, "", (ItemGrabMenu.behaviorOnItemSelect)null, true, true, false, true, false, 0, (Item)null, -1, (object)null);
                itemGrabMenu.initializeUpperRightCloseButton();
                int num1 = 0;
                itemGrabMenu.setBackgroundTransparency(num1 != 0);
                int num2 = 1;
                itemGrabMenu.setDestroyItemOnClick(num2 != 0);
                itemGrabMenu.initializeShippingBin();
                Game1.activeClickableMenu = (IClickableMenu)itemGrabMenu;
            }
        }
    }
}
