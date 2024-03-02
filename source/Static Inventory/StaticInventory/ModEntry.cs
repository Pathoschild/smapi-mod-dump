/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/nrobinson12/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System.Reflection;

namespace StaticInventory
{
    /// <summary>
    /// The mod entry point.
    /// </summary>
    public class ModEntry : Mod
    {
        public static IMonitor MonitorObject { get; private set; }

        private static readonly string modDataKey = "static-inventory";
        private IClickableMenu oldMenu;
        private StaticInventory staticInventory;

        /*********
        ** Public methods
        *********/

        /// <summary>
        /// The mod entry point, called after the mod is first loaded.
        /// </summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            MonitorObject = Monitor;
            oldMenu = Game1.activeClickableMenu;

            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.Display.Rendering += OnRendering;
            helper.Events.GameLoop.Saving += OnSaving;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.Display.MenuChanged += OnMenuChanged;
        }

        /*********
        ** Private methods
        *********/

        /// <summary>
        /// Raised after the player presses a button on the keyboard, controller, or mouse.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady) return;

            // Same logic as Game1._update
            if (Game1.currentLocation == null || Game1.currentMinigame != null || Game1.emoteMenu != null || Game1.textEntry != null || Game1.activeClickableMenu != null || Game1.globalFade || Game1.freezeControls)
            {
                return;
            }

            staticInventory.OnButtonPressed();
        }

        private void OnRendering(object sender, RenderingEventArgs e)
        {
            if (Context.IsWorldReady)
            {
                bool openingMenu = oldMenu == null && IsMenuWithInventory(Game1.activeClickableMenu);
                bool closingMenu = Game1.activeClickableMenu == null && IsMenuWithInventory(oldMenu);

                if (openingMenu || closingMenu) staticInventory.OnMenuChange();
            }

            oldMenu = Game1.activeClickableMenu;
        }

        private void OnSaving(object sender, SavingEventArgs e)
        {
            SaveModData();
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            staticInventory = new(Helper.Data.ReadSaveData<ModData>(modDataKey));
        }

        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is GameMenu menu)
            {
                OptionsPage page = (OptionsPage)menu.pages[GameMenu.optionsTab];
                page.options.Add(new OptionsButton("Static Inventory: Make current row first", () => {
                    staticInventory.SetFirstRow();
                    SaveModData();

                    Game1.addHUDMessage(new HUDMessage("Saved current row as first!", null));

                    if (Game1.activeClickableMenu.readyToClose())
                    {
                        Game1.activeClickableMenu.exitThisMenu();
                    }
                }));
            }
        }

        private void SaveModData()
        {
            Helper.Data.WriteSaveData(modDataKey, staticInventory.GetModData());
        }

        private bool IsMenuWithInventory(IClickableMenu clickableMenu)
        {
            if (clickableMenu == null) return false;

            // Check if menu is an MenuWithInventory
            if (clickableMenu is GameMenu or MenuWithInventory) return true;

            // Check if menu has an InventoryMenu property
            PropertyInfo[] properties = clickableMenu.GetType().GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i].GetType() == typeof(InventoryMenu)) return true;
            }

            // Did not find inventory in menu
            return false;
        }
    }
}