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
using ToolBelt.api;

namespace ToolBelt
{
    public class ModEntry : Mod
    {
        private ToolBeltMenu toolBeltMenu;

        ModConfig Config;


        public override void Entry(IModHelper helper)
        {
            Helper.Events.Input.ButtonPressed += OnButtonPressed;
            Helper.Events.Input.ButtonReleased += OnButtonReleased;


            Config = Helper.ReadConfig<ModConfig>();
            toolBeltMenu = new ToolBeltMenu(Helper, this, Config, Monitor);

            //support for generic mod menu
            Helper.Events.GameLoop.GameLaunched += onLaunched;
            Helper.Events.GameLoop.SaveLoaded += reinit;
        }

        private void reinit(object sender, SaveLoadedEventArgs e)
        {
            Config = Helper.ReadConfig<ModConfig>();
            toolBeltMenu = new ToolBeltMenu(Helper, this, Config, Monitor);
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
            if (Config.HoverSelects && e.Button != SButton.LeftStick) swapItem();
        }

        public void swapItem()
        {
            int swapIndex = toolBeltMenu.closeAndReturnSelected();
            Monitor.Log($"selected index is {swapIndex}", LogLevel.Trace);
            if (swapIndex == -1) return;
            swapItem(swapIndex);
        }

        public void swapItem(int swapIndex)
        {
            Farmer farmer = Game1.player;
            List<Item> newInventory = new List<Item>();
            int currentIndex = farmer.CurrentToolIndex;
            if (swapIndex == -1) return;
            newInventory.AddRange(farmer.items);

            if (Config.SwapTools)
            {
                Item temp = newInventory[currentIndex];
                newInventory[currentIndex] = newInventory[swapIndex];
                newInventory[swapIndex] = temp;
                farmer.setInventory(newInventory);
            }
            else
            {
                List<Item> tempInventory = new List<Item>();
                int rowIndex = (int)((float)swapIndex / 12f) * 12;
                Monitor.Log($"row index {rowIndex}", LogLevel.Trace);
                tempInventory.AddRange(newInventory.GetRange(rowIndex, newInventory.Count - rowIndex));
                tempInventory.AddRange(newInventory.GetRange(0, rowIndex));
                farmer.setInventory(tempInventory);
                farmer.CurrentToolIndex = swapIndex % 12;
                Monitor.Log($"swap index {swapIndex % 12}", LogLevel.Trace);

            }
        }

        private SortedDictionary<Item, int> getToolMap(List<Item> inventory)
        {
            Monitor.Log("calling toolmap search", LogLevel.Trace);
            SortedDictionary<Item, int> toolMap = new SortedDictionary<Item, int>(new DuplicateKeyComparer<Item>());
            int count = 0;
            foreach (Item item in inventory)
            {
                if (validItem(item))
                {
                    Monitor.Log($"found {item.Name} on {count} with sheetindex {item.parentSheetIndex}", LogLevel.Warn);
                    toolMap.Add(item, count);
                }
                count++;
            }
            return toolMap;
        }

        private bool validItem(Item item)
        {
            if (item == null) return false;

            if (Config.ConsiderHorseFlutAsTool && item.ParentSheetIndex == 911) return true;
            if (Config.BlacklistIds.Contains(item.ParentSheetIndex)) return false;
            if (Config.BlacklistNames.Contains(item.Name)) return false;
            return item is Tool;
        }

        private bool isGoTime(SButton b)
        {
            bool using_tool = Game1.player.UsingTool;
            return (Context.IsWorldReady && !using_tool && (b.Equals(Config.ToggleKey) | b.Equals(SButton.LeftStick)));
        }

        private void onLaunched(object sender, GameLaunchedEventArgs e)
        {
            if (!Helper.ModRegistry.IsLoaded("spacechase0.GenericModConfigMenu")) return;
            var api = Helper.ModRegistry.GetApi<GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
            api.RegisterModConfig(ModManifest, () => Config = new ModConfig(), () => Helper.WriteConfig(Config));
            api.SetDefaultIngameOptinValue(ModManifest, true);
            api.RegisterLabel(ModManifest, "Toolbelt Options", "Configure to your Liking");
            api.RegisterSimpleOption(ModManifest, "Use Backdrop", "Use fancy Backdrop for better Visibilaty", () => Config.UseBackdrop, (bool val) => Config.UseBackdrop = val);
            api.RegisterClampedOption(ModManifest, "Animation Time (Milliseconds)", "Duration off the opening Animation", () => Config.AnimationMilliseconds, (int val) => Config.AnimationMilliseconds = val, 0, 500);
            api.RegisterSimpleOption(ModManifest, "Select with Leftstick", "Use left Stick of Gamepad to select Tool", () => Config.LeftStickSelection, (bool val) => Config.LeftStickSelection = val);
            api.RegisterSimpleOption(ModManifest, "Hover Tool selection", $"hold {Config.ToggleKey} and hover over item, stop holding to select", () => Config.HoverSelects, (bool val) => Config.HoverSelects = val);
            api.RegisterSimpleOption(ModManifest, "Swap Tools", "use default tool swaping, else active hotbar will be changed", () => Config.SwapTools, (bool val) => Config.SwapTools = val);
            api.RegisterSimpleOption(ModManifest, "Horizontal Selection", "easier (but slower) selection method designed for people strugling with controller best paired with Left stick selection!", () => Config.HorizontalSelect, (bool val) => Config.HorizontalSelect = val);
            api.RegisterSimpleOption(ModManifest, "Consider the Horse Flute as a Tool", "The Game does not consider the Horse Flute a Tool but you may have acces to it via toolbelt anyway", () => Config.ConsiderHorseFlutAsTool, (bool val) => Config.ConsiderHorseFlutAsTool = val);
            api.RegisterSimpleOption(ModManifest, "MenuKey", "Key for opening the Toolbelt Menu (Keyboard)", () => Config.ToggleKey, (SButton val) => Config.ToggleKey = val);
            //api.RegisterSimpleOption(ModManifest, "Blacklisted Names", "item names to blacklist use list_items command to see list of items", () => Config.BlacklistNames, (List<string> val) => Config.BlacklistNames = val);

            api.RegisterLabel(ModManifest, "", "");
        }
    }
}