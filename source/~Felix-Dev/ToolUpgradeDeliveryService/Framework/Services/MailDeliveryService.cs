using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.StardewValley.LetterMenu;

namespace StardewMods.ToolUpgradeDeliveryService.Framework
{
    /// <summary>
    /// This class is responsible for sending Clint's [upgraded-tool] mail to the player and adding the tool
    /// to the player's inventory.
    /// </summary>
    internal class MailDeliveryService
    {
        private bool running;

        private IMonitor monitor;
        private IModEvents events;
        private IReflectionHelper reflectionHelper;
        private MailGenerator mailGenerator;

        public MailDeliveryService(MailGenerator generator)
        {
            events = ModEntry.CommonServices.Events;
            reflectionHelper = ModEntry.CommonServices.ReflectionHelper;
            monitor = ModEntry.CommonServices.Monitor;

            mailGenerator = generator ?? throw new ArgumentNullException(nameof(generator));

            running = false;
        }

        public void Start()
        {
            if (running)
            {
                monitor.Log("[MailDeliveryService] is already running!", LogLevel.Info);
                return;
            }

            running = true;

            events.GameLoop.DayStarted += OnDayStarted;
            events.Display.MenuChanged += OnMenuChanged;
        }

        public void Stop()
        {
            if (!running)
            {
                monitor.Log("[MailDeliveryService] is not running or has already been stopped!", LogLevel.Info);
                return;
            }

            events.GameLoop.DayStarted -= OnDayStarted;
            events.Display.MenuChanged -= OnMenuChanged;

            running = false;
        }

        /// <summary>
        /// Called after the game begins a new day (including when the player loads a save).
        /// Checks, if a mail with the upgraded tool should be sent to the player for the next day.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            if (Game1.player.daysLeftForToolUpgrade.Value == 1)
            {
                string mailKey = mailGenerator.GenerateMailKey(Game1.player.toolBeingUpgraded.Value);
                if (mailKey == null)
                {
                    monitor.Log("Failed to generate mail for upgraded tool!", LogLevel.Error);
                    return;
                }

                Game1.addMailForTomorrow(mailKey);
                monitor.Log("Added [tool upgrade] mail to tomorrow's mailbox.", LogLevel.Info);
            }
        }

        /// <summary>
        /// Called after a game menu is opened, closed, or replaced.
        /// Responsible for displaying the actual content of a [Tool-Upgrade] mail, such as 
        /// whether to show an attached tool, set the attached tool.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (!(e.OldMenu is LetterViewerMenu) && e.NewMenu is LetterViewerMenu letterViewerMenu)
            {
                var mailTitle = reflectionHelper.GetField<string>(letterViewerMenu, "mailTitle").GetValue();
                if (mailTitle == null || !mailGenerator.IsToolMail(mailTitle))
                {
                    return;
                }

                ToolUpgradeInfo upgradeInfo = mailGenerator.GetMailAssignedToolUpgrade(mailTitle);
                if (upgradeInfo == null)
                {
                    monitor.Log("Failed to retrive tool data from mail!", LogLevel.Error);
                }

                Tool toolForMail = Game1.player.toolBeingUpgraded.Value;

                /*
                 * Check if the current upgrade tool matches with the tool which was assigned to this mail.
                 * 
                 * Since the upgrade-mail content is generated when the mail is opened, the current upgrade tool 
                 * could have changed in the meantime. In this case, no tool will be included in this mail.
                 */
                if (toolForMail != null && (toolForMail.GetType() != upgradeInfo.ToolType || toolForMail.UpgradeLevel != upgradeInfo.Level))
                {
                    toolForMail = null;
                }

                // Bonus: Set the water level to full for the upgraded watering can.
                if (toolForMail is WateringCan can)
                {
                    can.WaterLeft = can.waterCanMax;
                }

                var mailMessage = reflectionHelper.GetField<List<string>>(letterViewerMenu, "mailMessage").GetValue();

                var itemMenu = new ItemLetterMenuHelper(mailMessage[0], toolForMail);
                itemMenu.MenuClosed += OnToolMailClosed;

                itemMenu.Show();
            }
        }

        /// <summary>
        /// Called after a tool-mail has been closed. Handles adding the selected tool
        /// to the player's inventory.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnToolMailClosed(object sender, ItemLetterMenuClosedEventArgs e)
        {
            // Do nothing if no mail-included tool was selected
            if (e.SelectedItem == null)
            {
                return;
            }

            /*
             * Check if tools of the same tool class (Axe, Hoe,...) should be removed from the player's inventory.
             * For example, this adds compatibility for the mod [Rented Tools] (i.e. rented tools will be removed).
             */
            if (ModEntry.ModConfig.RemoveToolDuplicates)
            {
                var removableItems = Game1.player.Items.Where(item => (item is Tool) && (item as Tool).BaseName.Equals(((Tool)e.SelectedItem).BaseName));
                foreach (var item in removableItems)
                {
                    Game1.player.removeItemFromInventory(item);
                }
            }            

            // Add selected tool item to the player's inventory
            Game1.player.addItemByMenuIfNecessary(e.SelectedItem);

            // Mark the tool upgrade process as finished, so that Clint won't hand it out when visiting him.
            Game1.player.toolBeingUpgraded.Value = null;
        }
    }
}
