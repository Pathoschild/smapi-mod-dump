using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
            TimeEvents.AfterDayStarted += TimeEvents_AfterDayStarted;
            SaveEvents.AfterReturnToTitle += SaveEvents_AfterReturnToTitle;
            SaveEvents.BeforeSave += TimeEvents_BeforeSave;
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
            SaveEvents.AfterLoad += SaveEvents_AfterLoad;
        }

        /// <summary>
        /// To be invoked after returning to the title screen.
        /// Unloads the menu changed method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveEvents_AfterReturnToTitle(object sender, EventArgs e)
        {
            MenuEvents.MenuChanged -= MenuEvents_MenuChanged;
            MailController.UnloadMailBox();
        }
        /// <summary>
        /// To be invoked after returning loading a game.
        /// Loads the menu changed method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveEvents_AfterLoad(object sender, EventArgs e)
        {
            MenuEvents.MenuChanged += MenuEvents_MenuChanged;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>
        /// The method invoked when the day starts.
        /// Here it updates the mail box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void TimeEvents_AfterDayStarted(object sender, EventArgs e)
        {
            MailController.UpdateMailBox();

        }

        /// <summary>
        /// The method invocade when a menu is changed.
        /// Here it invoke the MailController to show a custom mail when the it's a LetterViewerMenu, called from open the mailbox and there is CustomMails to be delivered
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuEvents_MenuChanged(object sender, EventArgsClickableMenuChanged e)
        {
            
            if (e.NewMenu is LetterViewerMenu && this.Helper.Reflection.GetField<string>(e.NewMenu, "mailTitle").GetValue() != null && MailController.HasCustomMail())
            {
                MailController.ShowLetter();
            }
        }


        /// <summary>
        /// To be invoked before saving a game.
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimeEvents_BeforeSave(object sender, EventArgs e)
        {
            MailController.UnloadMailBox();
        }
    }
}
