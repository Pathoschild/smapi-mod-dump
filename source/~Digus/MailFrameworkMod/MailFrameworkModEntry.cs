/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System;
using Harmony;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace MailFrameworkMod
{
    /// <summary>The mod entry class loaded by SMAPI.</summary>
    public class MailFrameworkModEntry : Mod
    {
        public static IModHelper ModHelper;
        public static IMonitor ModMonitor;

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

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
            helper.Events.GameLoop.Saving += OnSaving;
            Helper.Events.GameLoop.SaveLoaded += DataLoader.LoadContentPacks;

            helper.ConsoleCommands.Add("player_addreceivedmail", "Adds a mail as received.\n\nUsage: player_addreceivedmail <value>\n- value: name of the mail.", Commands.AddsReceivedMail);
            helper.ConsoleCommands.Add("player_removereceivedmail", "Remove a mail from the list of received mail.\n\nUsage: player_removereceivedmail <value>\n- value: name of the mail.", Commands.RemoveReceivedMail);
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). All mods are loaded and initialised at this point, so this is a good time to set up mod integrations.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            Helper.Content.AssetEditors.Add(new DataLoader());
            var harmony = HarmonyInstance.Create("Digus.MailFrameworkMod");

            harmony.Patch(
                original: AccessTools.Method(typeof(LetterViewerMenu), "getTextColor"), 
                prefix: new HarmonyMethod(typeof(LetterViewerMenuExtended), nameof(LetterViewerMenuExtended.GetTextColor))
            );
            
            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.mailbox)),
                prefix: new HarmonyMethod(typeof(MailController), nameof(MailController.mailbox_prefix)),
                postfix: new HarmonyMethod(typeof(MailController), nameof(MailController.mailbox_postfix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(CollectionsPage), nameof(CollectionsPage.receiveLeftClick)),
                prefix: new HarmonyMethod(typeof(MailController), nameof(MailController.receiveLeftClick))
            );
        }

        /// <summary>
        /// Raised after the game returns to the title screen.
        /// Unloads the menu changed method.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            MailController.UnloadMailBox();
        }

        /// <summary>
        /// Raised after the game begins a new day (including when the player loads a save).
        /// Here it updates the mail box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            if (MailDao.HasRepositoryChanged())
            {
                Helper.Content.InvalidateCache("Data\\mail");
            }
            MailController.UpdateMailBox();

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
