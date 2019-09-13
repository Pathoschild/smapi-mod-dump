using FelixDev.StardewMods.FeTK.Framework.Services;
using FelixDev.StardewMods.ToolUpgradeDeliveryService.Compatibility;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Translation = FelixDev.StardewMods.ToolUpgradeDeliveryService.Common.Translation;

namespace FelixDev.StardewMods.ToolUpgradeDeliveryService.Framework
{
    /// <summary>
    /// The <see cref="MailDeliveryService"/> class is responsible for sending Clint's [upgraded-tool] mail to the player and adding the tool
    /// to the player's inventory.
    /// </summary>
    internal class MailDeliveryService
    {
        /// <summary>An ID prefix for the tool-upgrade mails sent by the mail-delivery service.</summary>
        private const string TOOL_MAIL_ID_PREFIX = "ToolUpgrade_";

        /// <summary>The unique ID of the external mod [Rush Orders].</summary>
        private const string MOD_RUSH_ORDERS_MOD_ID = "spacechase0.RushOrders";

        /// <summary>Provides access to the <see cref="IModEvents"/> API provided by SMAPI.</summary>
        private static readonly IModEvents events = ModEntry.ModHelper.Events;      

        /// <summary>Provides access to the <see cref="IModRegistry"/> API provided by SMAPI.</summary>
        private static readonly IModRegistry modRegistry = ModEntry.ModHelper.ModRegistry;

        /// <summary>Provides access to the <see cref="IMonitor"/> API provided by SMAPI.</summary>
        private static readonly IMonitor monitor = ModEntry._Monitor;

        /// <summary>Provides access to the <see cref="ITranslationHelper"/> API provided by SMAPI.</summary>
        private static readonly ITranslationHelper translationHelper = ModEntry.ModHelper.Translation;

        /// <summary>The <see cref="IMailService"/> instance used to add tool-upgrade mails to the game.</summary>
        private readonly IMailService mailService;

        /// <summary>Indicates whether the mail delivery service is currently running.</summary>
        private bool running;

        /// <summary>Indicates whether the player is using the mod [Rush Orders].</summary>
        private bool isPlayerUsingRushedOrders;

        /// <summary>An API to interact with the external mod [Rush Orders].</summary>
        private IRushOrdersApi rushOrdersApi;

        /// <summary>
        /// Create a new instance of the <see cref="MailDeliveryService"/> class.
        /// </summary>
        public MailDeliveryService()
        {
            mailService = ServiceFactory.GetFactory(ModEntry._ModManifest.UniqueID).GetMailService();

            running = false;
        }

        /// <summary>
        /// Start the mail-delivery service.
        /// </summary>
        public void Start()
        {
            if (running)
            {
                monitor.Log("[MailDeliveryService] is already running!", LogLevel.Info);
                return;
            }

            running = true;

            events.GameLoop.GameLaunched += OnGameLaunched;
            events.GameLoop.DayStarted += OnDayStarted;

            mailService.MailOpening += OnMailOpening;
            mailService.MailClosed += OnMailClosed;

            if (isPlayerUsingRushedOrders)
            {
                rushOrdersApi.ToolRushed += OnPlacedRushOrder;
            }
        }

        /// <summary>
        /// Stop the running mail-delivery service.
        /// </summary>
        public void Stop()
        {
            if (!running)
            {
                monitor.Log("[MailDeliveryService] is not running or has already been stopped!", LogLevel.Info);
                return;
            }

            if (isPlayerUsingRushedOrders)
            {
                rushOrdersApi.ToolRushed -= OnPlacedRushOrder;
            }

            mailService.MailOpening -= OnMailOpening;
            mailService.MailClosed -= OnMailClosed;

            events.GameLoop.GameLaunched -= OnGameLaunched;
            events.GameLoop.DayStarted -= OnDayStarted;

            running = false;
        }

        /// <summary>
        /// Called when the game has been launched. Adds compatibility for external mods.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            AddModCompatibility();
        }

        /// <summary>
        /// Called after the game begins a new day (including when the player loads a save).
        /// Checks, if a mail with the upgraded tool should be sent to the player for the next day.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            int daysLeftForToolUpgrade = Game1.player.daysLeftForToolUpgrade.Value;
            Tool upgradeTool = Game1.player.toolBeingUpgraded.Value;

            // If the tool upgrade is done by tomorrow, send a tool-upgrade mail for the next day.
            if (daysLeftForToolUpgrade == 1)
            {
                SetToolMailForDay(1, upgradeTool);
                return;
            }

            // Place a tool-upgrade mail in the player's mailbox in the morning if a tool upgrade
            // is finished and and a tool-upgrade mail hasn't been sent yet. (This is mainly useful 
            // to handle tool upgrades where the player installs this mod after the upgrade is 
            // finished, i.e. the mod wasn't running when "daysLeftForToolUpgrade == 1" was true.)
            if (daysLeftForToolUpgrade == 0 && upgradeTool != null)
            {
                if (!mailService.HasMailInMailbox(GetMailIdFromTool(upgradeTool)))
                {
                    SetToolMailForDay(0, upgradeTool);
                }                
            }
        }

        /// <summary>
        /// Called after the player placed a rushed order at Clint's for a faster tool upgrade.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The ordered tool.</param>
        /// <remarks>
        /// This handler makes ToolUpgradeDeliveryService compatible with the mod [RushOrders].
        /// </remarks>
        private void OnPlacedRushOrder(object sender, Tool e)
        {
            // A rushed order as provided by the mod [Rush Orders] always finishes in one day or less.
            SetToolMailForDay(1, e);
        }

        /// <summary>
        /// Called when a mail is being opened by the player. Checks if the attached tool should be displayed.
        /// (i.e. in cases where the tool has already been received by Clint, the tool won't be attached to the mail)
        /// and sets the mail content accordingly (adding a hint that the tool was already received, if applicable).
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">Provides information about the opened mail.</param>
        private void OnMailOpening(object sender, MailOpeningEventArgs e)
        {
            // If the mail is not a tool uprade mail by Clint -> do nothing
            if (!IsToolMail(e.Id) || !(e.Content is ItemMailContent mailContent) 
                || !(mailContent.AttachedItems?.Count > 0))
            {
                return;
            }

            var attachedTool = (Tool)mailContent.AttachedItems[0];
            Tool currentToolUpgrade = Game1.player.toolBeingUpgraded.Value;

            /*
             * Check if the current upgrade tool matches the tool which was assigned to this mail.
             * 
             * Since the upgrade-mail content is generated when the mail is opened, the current upgrade tool 
             * could have changed in the meantime. In this case, the originally attached tool will NOT be included in the mail.
             */
            bool toolMatches = currentToolUpgrade != null
                && currentToolUpgrade.BaseName == attachedTool.BaseName
                && currentToolUpgrade.UpgradeLevel == attachedTool.UpgradeLevel;
            if (!toolMatches)
            {
                mailContent.AttachedItems = null;
                mailContent.Text += translationHelper.Get(Translation.MAIL_TOOL_UPGRADE_TOOL_ALREADY_RECEIVED);

                return;
            }

            // Bonus: Set the water level to full for the upgraded watering can.
            if (attachedTool is WateringCan can)
            {
                can.WaterLeft = can.waterCanMax;
            }
        }

        /// <summary>
        /// Called after a mail has been closed. Checks if the closed mail contained a tool upgrade 
        /// and adds the selected tool - if any - to the player's inventory.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnMailClosed(object sender, MailClosedEventArgs e)
        {
            // If the mail is not a tool uprade mail by Clint or no tool was selected -> do nothing
            if (!IsToolMail(e.Id) || !(e.InteractionRecord is ItemMailInteractionRecord interactionRecord) 
                || interactionRecord.SelectedItems.Count == 0)
            {
                return;
            }

            // The attached tool was selected in the tool-upgrade mail. We now proceed to add 
            // that tool to the player's inventory.

            var selectedTool = (Tool)interactionRecord.SelectedItems[0];

            // Check if tools of the same tool class (Axe, Hoe,...) should be removed from the player's inventory.
            // For example, this adds compatibility for the mod [Rented Tools] (i.e. rented tools will be removed).
            if (ModEntry.ModConfig.RemoveToolDuplicates)
            {
                var removableItems = Game1.player.Items.Where(item => item is Tool tool && tool.BaseName.Equals(selectedTool.BaseName));
                foreach (var item in removableItems)
                {
                    Game1.player.removeItemFromInventory(item);
                }
            }

            // Add selected tool item to the player's inventory
            Game1.player.addItemByMenuIfNecessary(selectedTool);

            // Mark the tool upgrade process as finished, so that Clint won't hand it out when visiting him.
            Game1.player.toolBeingUpgraded.Value = null;
        }

        /// <summary>
        /// Get the ID for a tool mail.
        /// </summary>
        /// <param name="tool">The tool upgrade to get the mail ID for.</param>
        /// <returns>The mail ID for the specified <paramref name="tool"/> upgrade.</returns>
        private string GetMailIdFromTool(Tool tool)
        {
            return TOOL_MAIL_ID_PREFIX + tool.BaseName + tool.UpgradeLevel;
        }

        /// <summary>
        /// Check if the mail with the specified ID is a tool-upgrade mail.
        /// </summary>
        /// <param name="mailId">The mail ID.</param>
        /// <returns><c>true</c> if the mail is a tool-upgrade mail; otherwise, <c>false</c>.</returns>
        private bool IsToolMail(string mailId)
        {
            return mailId.StartsWith(TOOL_MAIL_ID_PREFIX);
        }

        /// <summary>
        /// Add a mail with the specified tool included to the player's mailbox for the next day.
        /// </summary>
        /// <param name="dayOffset">The day offset from the current in-game day.</param>
        /// <param name="tool">The tool to include in the mail.</param>
        private void SetToolMailForDay(int dayOffset, Tool tool)
        {
            string mailId = GetMailIdFromTool(tool);
            string text = GetTranslatedMailTextContent(tool);

            var toolMail = new ItemMail(mailId, text, tool);
            mailService.AddMail(toolMail, dayOffset);
        }

        /// <summary>
        /// Get the mail's translated text content for a tool.
        /// </summary>
        /// <param name="tool">The tool to get the text content for.</param>
        /// <returns>The mail's translated text content.</returns>
        private string GetTranslatedMailTextContent(Tool tool)
        {
            string translationKey;
            switch (tool)
            {
                case Axe _:
                    translationKey = Translation.MAIL_TOOL_UPGRADE_AXE;
                    break;
                case Pickaxe _:
                    translationKey = Translation.MAIL_TOOL_UPGRADE_PICKAXE;
                    break;
                case Hoe _:
                    translationKey = Translation.MAIL_TOOL_UPGRADE_HOE;
                    break;
                case WateringCan _:
                    translationKey = Translation.MAIL_TOOL_UPGRADE_WATERING_CAN;
                    break;
                default:
                    return null;
            }

            return translationHelper.Get(translationKey);
        }

        /// <summary>
        /// Set up compatibility with other external mods.
        /// </summary>
        private void AddModCompatibility()
        {
            // Setup compatibility for the mod [Rush Orders].

            isPlayerUsingRushedOrders = false;

            // Check if the player uses the mod [Rush Orders]. 
            bool isLoaded = modRegistry.IsLoaded(MOD_RUSH_ORDERS_MOD_ID);
            if (!isLoaded)
            {
                return;
            }

            // The API we consume is only available starting with Rush Orders 1.1.4.
            IModInfo mod = modRegistry.Get(MOD_RUSH_ORDERS_MOD_ID);
            if (mod.Manifest.Version.IsOlderThan("1.1.4"))
            {
                monitor.Log($"You are running an unsupported version of the mod [{mod.Manifest.Name}]! " +
                    $"Please use at least [{mod.Manifest.Name} 1.1.4] for compatibility!", LogLevel.Info);
                return;
            }

            rushOrdersApi = modRegistry.GetApi<IRushOrdersApi>(MOD_RUSH_ORDERS_MOD_ID);
            if (rushOrdersApi == null)
            {
                monitor.Log($"Could not add compatibility for the mod [{mod.Manifest.Name}]! " +
                    $"A newer version of the mod [ToolUpgradeDeliveryService] might be needed.", LogLevel.Error);
                return;
            }

            rushOrdersApi.ToolRushed += OnPlacedRushOrder;
            isPlayerUsingRushedOrders = true;
        }
    }
}
