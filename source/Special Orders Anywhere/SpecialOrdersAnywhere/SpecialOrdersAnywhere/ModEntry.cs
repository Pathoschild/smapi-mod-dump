/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AcidicNic/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;
using GenericModConfigMenu;

namespace AcidicNic.SpecialOrdersAnywhere {
    
    public class ModEntry : Mod {
        private ModConfig Config;

        public override void Entry(IModHelper helper) {
            this.Config = helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }

        /// Generic Mod Config Menu
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e) {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this.Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(this.Config)
            );

            // keybind title
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Control Settings"
            );
            // activate key config
            configMenu.AddKeybind(
                mod: this.ModManifest,
                name: () => "Toggle Menu Key",
                tooltip: () => "Opens the first enabled menu in the cycle. Closes menu, if one is open. Default: P",
                getValue: () => this.Config.ActivateKey,
                setValue: value => this.Config.ActivateKey = value
            );
            // cycle right key config
            configMenu.AddKeybind(
                mod: this.ModManifest,
                name: () => "Next Menu Key",
                tooltip: () => "Cycle from one menu to the next. Default: ]",
                getValue: () => this.Config.CycleRightKey,
                setValue: value => this.Config.CycleRightKey = value
            );
            // cycle left key config
            configMenu.AddKeybind(
                mod: this.ModManifest,
                name: () => "Prev Menu Key",
                tooltip: () => "Cycle from one menu to the previous one. (Set this to `None` if you only want one cycle key.) Default: [",
                getValue: () => this.Config.CycleLeftKey,
                setValue: value => this.Config.CycleLeftKey = value
            );

            // menu settings title
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Menu Settings",
                tooltip: () => "Choose the menus you want included in your cycle."
            );
            // menu settings desc
            configMenu.AddParagraph(
                mod: this.ModManifest,
                text: () => "@—> Calendar @—> Daily Quests @—> Special Orders @—> Qi's Special Orders @—> Journal @—>"
            );
            // enable calendar bool config
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Calendar",
                tooltip: () => "Include the `Calendar` in the menu cycle. Default: true",
                getValue: () => this.Config.enableCalendar,
                setValue: value => this.Config.enableCalendar = value
            );
            // enable daily quests bool config
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Daily Quest Board",
                tooltip: () => "Include the `Daily Quest Board` in the menu cycle. Default: true",
                getValue: () => this.Config.enableDailyQuests,
                setValue: value => this.Config.enableDailyQuests = value
            );
            // enable special orders bool config
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Special Orders",
                tooltip: () => "Include the `Special Orders Board` in the menu cycle. Default: true",
                getValue: () => this.Config.enableSpecialOrders,
                setValue: value => this.Config.enableSpecialOrders = value
            );
            // enable qis special orders bool config
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Qi's Special Orders",
                tooltip: () => "Include the `Qi's Special Orders Board` in the menu cycle. Default: true",
                getValue: () => this.Config.enableQiSpecialOrders,
                setValue: value => this.Config.enableQiSpecialOrders = value
            );
            // enable journal bool config
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Journal",
                tooltip: () => "Includes the 'Journal' in the menu cycle. (Default: false)",
                getValue: () => this.Config.enableJournal,
                setValue: value => this.Config.enableJournal = value
            );

            // extra settings title
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Extra Settings",
                tooltip: () => "= hackz! :o ="
            );
            // special orders before unlocked bool config
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Special Orders Before Unlock",
                tooltip: () => "Access the `Special Orders Board` before it's unlocked. Default: false",
                getValue: () => this.Config.SpecialOrdersBeforeUnlocked,
                setValue: value => this.Config.SpecialOrdersBeforeUnlocked = value
            );
            // qi quests before unlocked bool config
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Qi's Special Orders Before Unlock",
                tooltip: () => "Access `Qi's Special Orders Board` before it's unlocked. Default: false",
                getValue: () => this.Config.QiBeforeUnlocked,
                setValue: value => this.Config.QiBeforeUnlocked = value
            );
        }

        /// On Btn Press Event
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e) {
            // ignore if the world isn't loaded
            if (!Context.IsWorldReady)
                return;

            // ActivateKey pressed
            if (e.Button == this.Config.ActivateKey) {
                if (Context.IsPlayerFree) {   
                    // open menu
                    if (this.Config.enableCalendar)
                        Game1.activeClickableMenu = new Billboard();
                    else if (this.Config.enableDailyQuests)
                        Game1.activeClickableMenu = new Billboard(true);
                    else if (this.Config.enableSpecialOrders && (this.Config.SpecialOrdersBeforeUnlocked || SpecialOrder.IsSpecialOrdersBoardUnlocked()))
                        Game1.activeClickableMenu = new SpecialOrdersBoard();
                    else if (this.Config.enableQiSpecialOrders && (this.Config.QiBeforeUnlocked || Game1.netWorldState.Value.GoldenWalnutsFound.Value >= 100))
                        Game1.activeClickableMenu = new SpecialOrdersBoard("Qi");
                    else if (this.Config.enableJournal)
                        Game1.activeClickableMenu = new QuestLog();
                }
                // close menu
                else if (Game1.activeClickableMenu is SpecialOrdersBoard || Game1.activeClickableMenu is Billboard || Game1.activeClickableMenu is QuestLog)
                    Game1.exitActiveMenu();
                return;
            }

            // menu open and CycleRight & LeftCycle keys pressed 
            if ((e.Button == this.Config.CycleRightKey || e.Button == this.Config.CycleLeftKey) && !Context.IsPlayerFree && this.Config.MenuLen > 1) {

                // get active menu type
                string activeMenu = "";
                if (Game1.activeClickableMenu is QuestLog)
                    activeMenu = "journal"; // journal
                else if (Game1.activeClickableMenu is Billboard)
                    if ((Game1.activeClickableMenu as Billboard).calendarDays != null)
                        activeMenu = "calendar"; // calendar
                    else
                        activeMenu = "daily quests"; // daily quests
                else if (Game1.activeClickableMenu is SpecialOrdersBoard)
                    if ((Game1.activeClickableMenu as SpecialOrdersBoard).boardType == "Qi")
                        activeMenu = "qi special orders"; // qi's special orders
                    else
                        activeMenu = "special orders"; // special orders
                else // none of the above menus are open -> bye lol
                    return;

                // create list of enabled menus
                LinkedList<string> menuList = new LinkedList<string>();
                if (this.Config.enableCalendar)
                    menuList.AddLast("calendar");
                if (this.Config.enableDailyQuests)
                    menuList.AddLast("daily quests");
                if (this.Config.enableSpecialOrders && (this.Config.SpecialOrdersBeforeUnlocked || SpecialOrder.IsSpecialOrdersBoardUnlocked()))
                    menuList.AddLast("special orders");
                if (this.Config.enableQiSpecialOrders && (this.Config.QiBeforeUnlocked || Game1.netWorldState.Value.GoldenWalnutsFound.Value >= 100))
                    menuList.AddLast("qi special orders");
                if (this.Config.enableJournal)
                    menuList.AddLast("journal");

                // the next menu in the cycle to open according to the key pressed & current menu
                var current = menuList.Find(activeMenu); // get current menu node
                string nextMenu = "";
                if (e.Button == this.Config.CycleRightKey)
                    nextMenu = (current.Next ?? menuList.First).Value;
                else
                    nextMenu = (current.Previous ?? menuList.Last).Value;

                // open next menu
                switch (nextMenu) {
                    case "calendar":
                        Game1.activeClickableMenu = new Billboard();
                        break;
                    case "daily quests":
                        Game1.activeClickableMenu = new Billboard(true);
                        break;
                    case "special orders":
                        Game1.activeClickableMenu = new SpecialOrdersBoard();
                        break;
                    case "qi special orders":
                        Game1.activeClickableMenu = new SpecialOrdersBoard("Qi");
                        break;
                    case "journal":
                        Game1.activeClickableMenu = new QuestLog();
                        break;
                    default:
                        return;
                }
            }
        }
    }
}
