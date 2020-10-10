/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/ShipAnywhere
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Reflection;
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

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            instance = this;

            Config = helper.ReadConfig<Configuration>();

            helper.Events.Input.ButtonPressed += OnButtonPressed;
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady || !Context.IsPlayerFree)
                return;

            if (e.Button == Config.OpenShippingBox)
            {
                var func = typeof(Farm).GetMethod("shipItem", BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(Item), typeof(Farmer) }, null);
                var del = (ItemGrabMenu.behaviorOnItemSelect)Delegate.CreateDelegate(typeof(ItemGrabMenu.behaviorOnItemSelect), Game1.getFarm(), func);

                ItemGrabMenu itemGrabMenu = new ItemGrabMenu((List<Item>)null, true, false, Utility.highlightShippableObjects, del, "", null, true, true, false);
                itemGrabMenu.initializeUpperRightCloseButton();
                int num1 = 0;
                itemGrabMenu.setBackgroundTransparency(num1 != 0);
                int num2 = 1;
                itemGrabMenu.setDestroyItemOnClick(num2 != 0);
                itemGrabMenu.initializeShippingBin();
                Game1.activeClickableMenu = itemGrabMenu;
            }
        }
    }
}
