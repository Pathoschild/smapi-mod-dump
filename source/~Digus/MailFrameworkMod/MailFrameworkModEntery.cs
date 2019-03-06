using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Harmony;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace MailFrameworkMod
{
    public class MailFrameworkModEntery : Mod
    {
        public static IModHelper ModHelper;
        public static IMonitor ModMonitor;
        public static ModConfig ModConfig;

        /*********
        ** Public methods
        *********/
        /// <summary>
        /// The mod entry point, called after the mod is first loaded.
        /// Here it loads the custom event handlers for the start of the day, after load and after returning to the title screen.
        /// </summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            ModHelper = helper;
            ModMonitor = Monitor;
            ModConfig = helper.ReadConfig<ModConfig>();
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
            helper.Events.GameLoop.Saving += OnSaving;
            var editors = helper.Content.AssetEditors;
            editors.Add(new DataLoader());

            helper.ConsoleCommands.Add("player_addreceivedmail", "Adds a mail as received.\n\nUsage: player_addreceivedmail <value>\n- value: name of the mail.", Commands.AddsReceivedMail);
            helper.ConsoleCommands.Add("player_removereceivedmail", "Remove a mail from the list of received mail.\n\nUsage: player_removereceivedmail <value>\n- value: name of the mail.", Commands.RemoveReceivedMail);

            try
            {
                var harmony = HarmonyInstance.Create("Digus.MailFrameworkMod");

                var gameLetterViewerMenuGetTextColor = typeof(LetterViewerMenu).GetMethod("getTextColor", BindingFlags.NonPublic | BindingFlags.Instance);
                var letterViewerMenuExtendedGetTextColor = typeof(LetterViewerMenuExtended).GetMethod("GetTextColor");
                harmony.Patch(gameLetterViewerMenuGetTextColor, new HarmonyMethod(letterViewerMenuExtendedGetTextColor), null);

                if (!ModConfig.UseOldMethodOfOpeningCustomMail)
                {
                    var gameLocaltionMailbox = typeof(GameLocation).GetMethod("mailbox");
                    var mailControllerMailbox = typeof(MailController).GetMethod("mailbox");
                    harmony.Patch(gameLocaltionMailbox, new HarmonyMethod(mailControllerMailbox), null);
                }
                else
                {
                    WatchLetterMenu();
                }
            }
            catch (Exception e)
            {
                Monitor.Log("Erro patching the GameLocation 'mailbox' Method. Applying old method of listening to menu change events.",LogLevel.Warn);
                Monitor.Log(e.Message+e.StackTrace,LogLevel.Trace);
                WatchLetterMenu();
            }
        }

        /// <summary>
        /// Old method of opening custom mails.
        /// </summary>
        private void WatchLetterMenu()
        {
            Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        }

        /// <summary>
        /// Raised after the game returns to the title screen.
        /// Unloads the menu changed method.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            Helper.Events.Display.MenuChanged -= OnMenuChanged;
            MailController.UnloadMailBox();
        }
        /// <summary>
        /// Raised after the player loads a save slot and the world is initialised.
        /// Loads the menu changed method.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaveLoaded(object sender, EventArgs e)
        {
            Helper.Events.Display.MenuChanged += OnMenuChanged;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>
        /// Raised after the game begins a new day (including when the player loads a save).
        /// Here it updates the mail box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            MailController.UpdateMailBox();

        }

        /// <summary>
        /// Raised after a game menu is opened, closed, or replaced.
        /// Here it invoke the MailController to show a custom mail when the it's a LetterViewerMenu, called from open the mailbox and there is CustomMails to be delivered
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is LetterViewerMenu && this.Helper.Reflection.GetField<string>(e.NewMenu, "mailTitle").GetValue() != null && MailController.HasCustomMail())
            {
                MailController.ShowLetter();
            }
        }


        /// <summary>
        /// Raised before the game begins writes data to the save file (except the initial save creation).
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaving(object sender, EventArgs e)
        {
            MailController.UnloadMailBox();
        }
    }
}
