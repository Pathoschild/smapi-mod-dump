/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Brandon22Adams/ToolPouch
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using ToolPouch.api;
using SpaceShared.APIs;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.GameData.Objects;
using StardewValley.GameData.Shops;
using StardewValley.Menus;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Inventories;
using System.Net.Mail;
using StardewValley.Network.NetEvents;
using StardewModdingAPI.Utilities;
using System.Text.Json;
using ToolPouch.UI;


namespace ToolPouch
{
    public class ModEntry : Mod
    {
        public static ModEntry instance;

        private PerScreen<ToolPouchMenu> toolPouchMenu;

        ModConfig Config;

        private static PerScreen<Pouch> toOpen;
        public static void QueueOpeningPouch(PerScreen<Pouch> pouch)
        {
            if (!pouch.Value.isOpen.Value)
            {
                toOpen = pouch;
            }
        }


        public override void Entry(IModHelper helper)
        {
            instance = this;
            I18n.Init(Helper.Translation);
            Helper.Events.GameLoop.UpdateTicked += onUpdate;
            Helper.Events.Content.AssetRequested += assetRequested;
            Helper.Events.Input.ButtonPressed += OnButtonPressed;
            Helper.Events.Input.ButtonReleased += OnButtonReleased;
            Helper.Events.GameLoop.DayStarted += dayStarted;
            Helper.Events.GameLoop.DayEnding += dayEnded;
            //support for generic mod menu
            Helper.Events.GameLoop.GameLaunched += onLaunched;
            Helper.Events.GameLoop.SaveLoaded += reinit;

            Config = Helper.ReadConfig<ModConfig>();
            setPerScreenToolPouchMenu(new ToolPouchMenu(Helper, this, Config, Monitor));

            var def = new PouchDataDefinition();
            ItemRegistry.ItemTypes.Add(def);
            Helper.Reflection.GetField<Dictionary<string, IItemDataDefinition>>(typeof(ItemRegistry), "IdentifierLookup").GetValue()[def.Identifier] = def;
        }


        private void onUpdate(object sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
        {
            if (toOpen != null && toOpen.Value != null && !Config.DisableOpeningPouchOutsideOfInventory)
            {
                var menu = new PouchMenu(toOpen.Value);

                if (Game1.activeClickableMenu == null)
                    Game1.activeClickableMenu = menu;
                else
                {
                    var theMenu = Game1.activeClickableMenu;
                    while (theMenu.GetChildMenu() != null)
                    {
                        theMenu = theMenu.GetChildMenu();
                    }
                    theMenu.SetChildMenu(menu);
                }

                toOpen = null;
            }
        }

        private void reinit(object sender, SaveLoadedEventArgs e)
        {
            Config = Helper.ReadConfig<ModConfig>();
            setPerScreenToolPouchMenu(new ToolPouchMenu(Helper, this, Config, Monitor));
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button == SButton.MouseRight) // Open pouch menu
            {
                if (Game1.activeClickableMenu is ShopMenu shop)
                {
                    foreach (var slot in shop.inventory.inventory)
                    {
                        if (slot.containsPoint(Game1.getMouseX(), Game1.getMouseY()))
                        {
                            int i = shop.inventory.inventory.IndexOf(slot);
                            if (shop.inventory.actualInventory[i] is Pouch pouch)
                            {
                                Game1.activeClickableMenu.SetChildMenu(new PouchMenu(pouch));
                                Helper.Input.Suppress(e.Button);
                                break;
                            }
                        }
                    }
                }
            }

            if (canOpenMenu(e.Button))
            {
                if (Game1.activeClickableMenu != null) // Close Tool Selection Wheel
                {
                    if (Game1.activeClickableMenu == toolPouchMenu.Value)
                    {
                        Monitor.Log("closing menu", LogLevel.Trace);
                        toolPouchMenu.Value.closeAndReturnSelected();
                        return;
                    }
                    return;
                }
                setPerScreenToolPouchMenu(new ToolPouchMenu(Helper, this, Config, Monitor));

                //Open Tool Selection wheel
                Farmer farmer = Game1.player;
                List<Item> inventory = new List<Item>();
                for (int i = 0; i < Game1.player.Items.Count; ++i)
                {
                    if (Game1.player.Items[i] is Pouch pouch)
                    {
                        inventory.AddRange(pouch.Inventory);
                        Monitor.Log($"ContextID {Context.ScreenId} Farmer {farmer.Name} with button {e.Button}", LogLevel.Trace);
                        toolPouchMenu.Value.updateToolList(getToolMap(inventory));
                        Game1.activeClickableMenu = toolPouchMenu.Value;
                    }
                }
            }
        }

        private void OnButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            if (!(canOpenMenu(e.Button) && Game1.activeClickableMenu == toolPouchMenu.Value)) return;
            if (Config.HoverSelects && e.Button != Config.ControllerToggleKey) swapItem();
        }

        private void setPerScreenToolPouchMenu(ToolPouchMenu menu)
        {
            PerScreen<ToolPouchMenu> perScreenMenu = new PerScreen<ToolPouchMenu>();
            perScreenMenu.Value = menu;
            toolPouchMenu = perScreenMenu;
        }

        public void swapItem()
        {
            int swapIndex = toolPouchMenu.Value.closeAndReturnSelected();
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
            newInventory.AddRange(farmer.Items);

            if(newInventory[currentIndex] != null && newInventory[currentIndex].ItemId == "Pouch")
            {
                currentIndex = -1;
                for (int i = 0; i < newInventory.Count; ++i)
                {
                    if (newInventory[i] == null)
                    {
                        currentIndex = i;
                        break;
                    }
                }
                if (currentIndex == -1)
                {
                    Game1.showRedMessage(I18n.Error_Pouch(), true);
                    return;
                }
            }

            Pouch newPouch = null;
            List<Item> pouchInventory = new List<Item>();
            for (int i = 0; i < Game1.player.Items.Count; ++i)
            {
                if (Game1.player.Items[i] is Pouch pouch)
                {
                    pouchInventory.AddRange(pouch.Inventory);
                    newPouch = pouch;
                }
            }

            Item temp = newInventory[currentIndex];
            newInventory[currentIndex] = pouchInventory[swapIndex];
            farmer.setInventory(newInventory);
            newPouch.Inventory[swapIndex] = temp;
            farmer.CurrentToolIndex = currentIndex;
            if (temp == null || swapIndex == Config.BagCapacity - 1) //Sort pouch after filling with empty slot or adding to the end of the pouch
            {
                newPouch.Inventory.Sort();
            }
        }

        private SortedDictionary<Item, int> getToolMap(List<Item> inventory)
        {
            SortedDictionary<Item, int> toolMap = new SortedDictionary<Item, int>(new DuplicateKeyComparer<Item>());
            int count = 0;
            foreach (Item item in inventory)
            {
                if(item != null)
                {
                    if (item.Name == null)
                    {
                        item.Name = "tempPouchID" + count.ToString();
                    }
                    toolMap.Add(item, count);
                    count++;
                }
            }
            return toolMap;
        }

        private bool canOpenMenu(SButton b)
        {
            bool using_tool = Game1.player.UsingTool;
            return (Context.IsWorldReady && !using_tool && (b.Equals(Config.ToggleKey) | b.Equals(Config.ControllerToggleKey)));
        }

        private void onLaunched(object sender, GameLaunchedEventArgs e)
        {
            var sc = Helper.ModRegistry.GetApi<ISpaceCoreApi>("spacechase0.SpaceCore");
            sc.RegisterSerializerType(typeof(Pouch));

            if (!Helper.ModRegistry.IsLoaded("spacechase0.GenericModConfigMenu")) return;
            var api = Helper.ModRegistry.GetApi<GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
            api.RegisterModConfig(ModManifest, () => Config = new ModConfig(), () => Helper.WriteConfig(Config));
            api.SetDefaultIngameOptinValue(ModManifest, true);
            api.RegisterLabel(ModManifest, I18n.Options_Title(), I18n.Options_Description());
            api.RegisterSimpleOption(ModManifest, I18n.Backdrop_Title(), I18n.Backdrop_Description(), () => Config.UseBackdrop, (bool val) => Config.UseBackdrop = val);
            api.RegisterClampedOption(ModManifest, I18n.Animation_Title(), I18n.Animation_Description(), () => Config.AnimationMilliseconds, (int val) => Config.AnimationMilliseconds = val, 0, 500);
            api.RegisterSimpleOption(ModManifest, I18n.GamepadSelect_Title(), I18n.GamepadSelect_Description(), () => Config.LeftStickSelection, (bool val) => Config.LeftStickSelection = val);
            api.RegisterSimpleOption(ModManifest, I18n.Hover_Title(), $"{I18n.Hover_Description1()} {Config.ToggleKey} {I18n.Hover_Description2()}", () => Config.HoverSelects, (bool val) => Config.HoverSelects = val);
            api.RegisterSimpleOption(ModManifest, I18n.Menu_Title(), I18n.Menu_Description(), () => Config.ToggleKey, (SButton val) => Config.ToggleKey = val);
            api.RegisterSimpleOption(ModManifest, I18n.GamepadMenu_Title(), I18n.GamepadMenu_Description(), () => Config.ControllerToggleKey, (SButton val) => Config.ControllerToggleKey = val);

            api.RegisterLabel(ModManifest, "", "");
        }

        private void assetRequested(object sender, StardewModdingAPI.Events.AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo($"{ModManifest.UniqueID}/Pouches"))
            {
                e.LoadFrom(() =>
                {
                    Dictionary<string, PouchData> ret = new();
                    for (int i = 0; i < 1; ++i)
                    {
                        ret.Add($"Pouch",
                            new PouchData()
                            {
                                TextureIndex = i,
                                DisplayName = I18n.Pouch_Name(),
                                Description = I18n.Pouch_Description(),
                                Capacity = Config.BagCapacity * (i + 1),
                            });
                    }
                    return ret;
                }, StardewModdingAPI.Events.AssetLoadPriority.Exclusive);
            }
            else if (e.NameWithoutLocale.IsEquivalentTo($"{ModManifest.UniqueID}/pouch.png"))
            {
                e.LoadFromModFile<Texture2D>("assets/pouch.png", StardewModdingAPI.Events.AssetLoadPriority.Low);
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/Mail"))
            {
                e.Edit((asset) =>
                {
                    var data = asset.AsDictionary<string, string>().Data;
                    data.Add("CodeWordZ.ToolPouch", I18n.Letter_Body() + " %item object (CWZ)Pouch 1 %%[#]" + I18n.Letter_Signoff());
                });
            }
        }
        private void dayStarted(object sender, EventArgs e)
        {
            foreach (Farmer farmer in Game1.getOnlineFarmers())
            {
                if (!farmer.hasOrWillReceiveMail("CodeWordZ.ToolPouch"))
                {
                    bool upgradedToolInInventory = false;
                    for (int i = 0; i < farmer.Items.Count; i++)
                    {
                        if (farmer.Items[i] != null && farmer.Items[i] is Tool && (farmer.Items[i] as Tool).UpgradeLevel > 0)
                        {
                            upgradedToolInInventory = true;
                        }
                    }
                    if (upgradedToolInInventory)
                    {
                        farmer.mailbox.Add("CodeWordZ.ToolPouch");
                    }
                }
            }
        }

        private void dayEnded(object sender, EventArgs e)
        {
            foreach (Farmer farmer in Game1.getOnlineFarmers())
            {
                if (!farmer.team.globalInventories.ContainsKey("pouch"))
                {
                    for (int i = 0; i < farmer.Items.Count; ++i)
                    {
                        if (farmer.Items[i] is Pouch pouch)
                        {
                            farmer.team.globalInventories.Add("pouch", pouch.Inventory);
                        }
                    }
                }
            }
        }

    }
}