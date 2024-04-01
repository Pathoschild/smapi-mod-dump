/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AcidicNic/StardewValleyMods
**
*************************************************/

using AcidicNic.MenuCycle.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.SpecialOrders;
using System.IO;

namespace AcdicNic.Stardew.MenuCycle
{

    public class ModEntry : Mod
    {
#pragma warning disable CS8618
        private ModConfig Config;
#pragma warning restore CS8618

        public override void Entry(IModHelper helper)
        {
            this.Config = helper.ReadConfig<ModConfig>();

            DeleteTranslationDirectory(helper.DirectoryPath);

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.Input.ButtonsChanged += this.OnButtonsChanged;
        }

        /// Generic Mod Config Menu
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // Get Generic Mod Config Menu's API (if it's installed)
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // Register Mod
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this.Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(this.Config)
            );

            // Control Settings
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Control Settings"
            );
            configMenu.AddKeybindList(
                mod: this.ModManifest,
                name: () => "Toggle Menu Key",
                tooltip: () => "Opens the first enabled menu in the cycle. Closes menu, if one is open. Default: P",
                getValue: () => this.Config.ActivateKey,
                setValue: value => this.Config.ActivateKey = value
            );
            configMenu.AddKeybindList(
                mod: this.ModManifest,
                name: () => "Next Menu Key",
                tooltip: () => "Cycle from one menu to the next. Default: ]",
                getValue: () => this.Config.CycleRightKey,
                setValue: value => this.Config.CycleRightKey = value
            );
            configMenu.AddKeybindList(
                mod: this.ModManifest,
                name: () => "Prev Menu Key",
                tooltip: () => "Cycle from one menu to the previous one. (Set this to `None` if you only want one cycle key.) Default: [",
                getValue: () => this.Config.CycleLeftKey,
                setValue: value => this.Config.CycleLeftKey = value
            );

            // Menu Settings
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Menu Settings",
                tooltip: () => "Choose the menus you want included in your cycle."
            );
            configMenu.AddParagraph(
                mod: this.ModManifest,
                text: () => "@—> Calendar @—> Daily Quests @—> Special Orders @—> Qi's Special Orders @—> Journal @—>"
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Calendar",
                tooltip: () => "Include the `Calendar` in the menu cycle. Default: true",
                getValue: () => this.Config.EnableCalendar,
                setValue: value => this.Config.EnableCalendar = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Daily Quest Board",
                tooltip: () => "Include the `Daily Quest Board` in the menu cycle. Default: true",
                getValue: () => this.Config.EnableDailyQuests,
                setValue: value => this.Config.EnableDailyQuests = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Special Orders",
                tooltip: () => "Include the `Special Orders Board` in the menu cycle. Default: true",
                getValue: () => this.Config.EnableSpecialOrders,
                setValue: value => this.Config.EnableSpecialOrders = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Qi's Special Orders",
                tooltip: () => "Include the `Qi's Special Orders Board` in the menu cycle. Default: true",
                getValue: () => this.Config.EnableQiSpecialOrders,
                setValue: value => this.Config.EnableQiSpecialOrders = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Journal",
                tooltip: () => "Includes the 'Journal' in the menu cycle. (Default: false)",
                getValue: () => this.Config.EnableJournal,
                setValue: value => this.Config.EnableJournal = value
            );

            // Extra Settings
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Extra Settings",
                tooltip: () => "= hackz! :o ="
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Special Orders Before Unlock",
                tooltip: () => "Access the `Special Orders Board` before it's unlocked. Default: false",
                getValue: () => this.Config.SpecialOrdersBeforeUnlocked,
                setValue: value => this.Config.SpecialOrdersBeforeUnlocked = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Qi's Special Orders Before Unlock",
                tooltip: () => "Access `Qi's Special Orders Board` before it's unlocked. Default: false",
                getValue: () => this.Config.QiBeforeUnlocked,
                setValue: value => this.Config.QiBeforeUnlocked = value
            );
        }

        private void OnButtonsChanged(object sender, ButtonsChangedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            // Activate Key Pressed
            if (this.Config.ActivateKey.JustPressed())
            {
                // Open menu
                if (Context.IsPlayerFree)
                {
                    if (this.Config.EnableCalendar)
                        this.OpenMenuByStr("CALENDAR");
                    else if (this.Config.EnableDailyQuests)
                        this.OpenMenuByStr("DAILY_QUESTS");
                    else if (this.Config.EnableSpecialOrders && (this.Config.SpecialOrdersBeforeUnlocked || SpecialOrder.IsSpecialOrdersBoardUnlocked()))
                        this.OpenMenuByStr("SPECIAL_ORDERS");
                    else if (this.Config.EnableQiSpecialOrders && (this.Config.QiBeforeUnlocked || Game1.netWorldState.Value.GoldenWalnutsFound >= 100))
                        this.OpenMenuByStr("QI_SPECIAL_ORDERS");
                    else if (this.Config.EnableJournal)
                        this.OpenMenuByStr("JOURNAL");
                }
                // Close menu
                else if (GetActiveMenuStr() != "")
                    Game1.exitActiveMenu();

                return;
            }

            // Cycle Right or Left Cycle Pressed
            if ((this.Config.CycleRightKey.JustPressed() || this.Config.CycleLeftKey.JustPressed()) && !Context.IsPlayerFree)
            {
                // Get active menu type, return if a different menu is open
                string activeMenu = GetActiveMenuStr();
                if (activeMenu == "")
                    return;

                // Create list of enabled menus
                List<string> menuList = new();
                if (this.Config.EnableCalendar)
                    menuList.Add("CALENDAR");
                if (this.Config.EnableDailyQuests)
                    menuList.Add("DAILY_QUESTS");
                if (this.Config.EnableSpecialOrders && (this.Config.SpecialOrdersBeforeUnlocked || SpecialOrder.IsSpecialOrdersBoardUnlocked()))
                    menuList.Add("SPECIAL_ORDERS");
                if (this.Config.EnableQiSpecialOrders && (this.Config.QiBeforeUnlocked || Game1.netWorldState.Value.GoldenWalnutsFound >= 100))
                    menuList.Add("QI_SPECIAL_ORDERS");
                if (this.Config.EnableJournal)
                    menuList.Add("JOURNAL");

                // the next menu in the cycle to open according to the key pressed & current menu
                var activeMenuIndex = menuList.IndexOf(activeMenu);

                string nextMenu = "";

                // Cycle Right
                if (this.Config.CycleRightKey.JustPressed())
                    nextMenu = menuList[(activeMenuIndex + 1) % menuList.Count];
                    
                // Cycle Left
                else if (this.Config.CycleLeftKey.JustPressed())
                    nextMenu = menuList[(activeMenuIndex - 1 + menuList.Count) % menuList.Count];

                this.OpenMenuByStr(nextMenu);
            }
        }

        // Helper method - open a menu based on a string
        private void OpenMenuByStr(string menu)
        {
            if (menu == "CALENDAR")
                Game1.activeClickableMenu = new Billboard();
            else if (menu == "DAILY_QUESTS")
                Game1.activeClickableMenu = new Billboard(true);
            else if (menu == "SPECIAL_ORDERS")
                Game1.activeClickableMenu = new SpecialOrdersBoard();
            else if (menu == "QI_SPECIAL_ORDERS")
                Game1.activeClickableMenu = new SpecialOrdersBoard("Qi");
            else if (menu == "JOURNAL")
                Game1.activeClickableMenu = new QuestLog();
            else
                this.Monitor.Log($"Failed to open menu: {menu}", LogLevel.Error);
        }

        // Helper method - get string value of the current active menu
        private static string GetActiveMenuStr()
        {
            if (Game1.activeClickableMenu is QuestLog)
                return "JOURNAL";
            else if (Game1.activeClickableMenu is Billboard billBoard)
                return billBoard.calendarDays == null ? "DAILY_QUESTS" : "CALENDAR";
            else if (Game1.activeClickableMenu is SpecialOrdersBoard specialOrders)
                return specialOrders.boardType == "Qi" ? "QI_SPECIAL_ORDERS" : "SPECIAL_ORDERS";

            return "";
        }

        public void DeleteTranslationDirectory(string modDir)
        {
            string translationDir = Path.Combine(modDir, "i18n");

            if (Directory.Exists(translationDir))
            {
                this.Monitor.Log($"Deleting MenuCycle/i18n/ ...", LogLevel.Info);
                Directory.Delete(translationDir, recursive: true);
            }
        }
    }
}
