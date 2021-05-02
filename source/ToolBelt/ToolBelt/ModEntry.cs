/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/yuri0r/toolbelt
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.Collections.Generic;

namespace ToolBelt
{
    public class ModEntry : Mod
    {
        private ToolBeltMenu toolBeltMenu = new ToolBeltMenu();
        private bool hold = false;


        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.Input.ButtonReleased += OnButtonReleased;
        }


        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!isGoTime(e.Button)) return;
            if (Game1.activeClickableMenu != null)
            {
                if (Game1.activeClickableMenu == toolBeltMenu)
                {
                    Monitor.Log("closing menu", LogLevel.Trace);
                    toolBeltMenu.closeAndReturnSelected();
                    return;
                }
                return;
            }

            //open Toolbelt menu
            Farmer farmer = Game1.player;
            Monitor.Log($"{farmer.Name} opening toolbelt menu with {e.Button}.", LogLevel.Trace);
            List<Item> inventory = new List<Item>();
            inventory.AddRange(farmer.items);
            toolBeltMenu.updateToolList(getToolMap(inventory));
            Game1.activeClickableMenu = toolBeltMenu;
        }

        private void OnButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!(isGoTime(e.Button) && Game1.activeClickableMenu == toolBeltMenu)) return;
            //placeholder for hold config
            if (hold) swapItem();
        }

        public void swapItem()
        {
            int swapIndex = toolBeltMenu.closeAndReturnSelected();
            Monitor.Log($"selected index is {swapIndex}", LogLevel.Trace);
            if (swapIndex == -1) return;
            swapItem(swapIndex);
        }

        public static void swapItem(int swapIndex)
        {
            Farmer farmer = Game1.player;
            List<Item> newInventory = new List<Item>();
            int currentIndex = farmer.CurrentToolIndex;
            if (swapIndex == -1) return;
            newInventory.AddRange(farmer.items);
            Item temp = newInventory[currentIndex];
            newInventory[currentIndex] = newInventory[swapIndex];
            newInventory[swapIndex] = temp;
            farmer.setInventory(newInventory);
        }

        private Dictionary<Tool, int> getToolMap(List<Item> inventory)
        {
            Monitor.Log("calling toolmap search", LogLevel.Trace);
            Dictionary<Tool, int> toolMap = new Dictionary<Tool, int>();
            int count = 0;
            foreach (Item item in inventory)
            {
                if (item != null && item is Tool tool)
                {
                    Monitor.Log($"found {tool.Name} on {count}", LogLevel.Trace);
                    toolMap.Add(tool, count);
                }
                count++;
            }
            return toolMap;
        }

        private bool isGoTime(SButton b)
        {
            return (Context.IsWorldReady && (b.Equals(SButton.LeftAlt) | b.Equals(SButton.LeftStick)));
        }
    }
}