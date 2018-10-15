using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewMods.ToolUpgradeDeliveryService.Framework.Menus;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewMods.ToolUpgradeDeliveryService.Common;

using Constants = StardewMods.ToolUpgradeDeliveryService.Common.Constants;

namespace StardewMods.ToolUpgradeDeliveryService.Framework
{
    /// <summary>
    /// This class is responsible for sending Clint's [upgraded-tool] e-mail to the player.
    /// </summary>
    internal class MailDeliveryService
    {
        private bool running;

        private IMonitor monitor;
        private IReflectionHelper reflectionHelper;
        private ITranslationHelper translationHelper;

        public MailDeliveryService()
        {
            reflectionHelper = ModEntry.CommonServices.ReflectionHelper;
            translationHelper = ModEntry.CommonServices.TranslationHelper;
            monitor = ModEntry.CommonServices.Monitor;

            running = false;
        }

        public void Start()
        {
            if (running)
            {
                monitor.Log("[MuseumDeliveryService] is already running!", LogLevel.Info);
                return;
            }

            running = true;
            TimeEvents.AfterDayStarted += TimeEvents_OnAfterDayStarted;
            MenuEvents.MenuChanged += MenuEvents_OnMenuChanged;
        }

        public void Stop()
        {
            if (!running)
            {
                monitor.Log("[MuseumDeliveryService] is not running or has already been stopped!", LogLevel.Info);
                return;
            }

            TimeEvents.AfterDayStarted -= TimeEvents_OnAfterDayStarted;
            MenuEvents.MenuChanged -= MenuEvents_OnMenuChanged;
            running = false;
        }

        private void TimeEvents_OnAfterDayStarted(object sender, EventArgs e)
        {
            if (Game1.player.daysLeftForToolUpgrade.Value == 1)
            {
                string mail;
                switch (Game1.player.toolBeingUpgraded.Value)
                {
                    case Axe t:
                        mail = Constants.TOOL_UPGRADE + $":{Constants.TOOL_AXE}:{t.UpgradeLevel}";
                        break;
                    case Pickaxe t:
                        mail = Constants.TOOL_UPGRADE + $":{Constants.TOOL_PICKAXE}:{t.UpgradeLevel}";
                        break;
                    case Hoe t:
                        mail = Constants.TOOL_UPGRADE + $":{Constants.TOOL_HOE}:{t.UpgradeLevel}";
                        break;
                    case Shears t:
                        mail = Constants.TOOL_UPGRADE + $":{Constants.TOOL_SHEARS}:{t.UpgradeLevel}";
                        break;
                    case WateringCan t:
                        mail = Constants.TOOL_UPGRADE + $":{Constants.TOOL_WATERING_CAN}:{t.UpgradeLevel}";
                        break;
                    default:
                        return;
                }

                Game1.addMailForTomorrow(mail);
                monitor.Log("Added [tool upgrade] mail to tomorrow's mailbox.", LogLevel.Info);
            }
        }

        private void MenuEvents_OnMenuChanged(object sender, EventArgsClickableMenuChanged e)
        {
            //Game1.player.toolBeingUpgraded.Value = null;
            if (!(e.PriorMenu is LetterViewerMenu) && e.NewMenu is LetterViewerMenu letterViewerMenu)
            {
                var mailTitle = reflectionHelper.GetField<string>(letterViewerMenu, "mailTitle").GetValue();
                if (mailTitle == null || !mailTitle.StartsWith(Constants.TOOL_UPGRADE))
                {
                    return;
                }

                var pToolData = GetMailAssignedTool(mailTitle);
                if (!pToolData.HasValue)
                {
                    monitor.Log("Failed to retrive tool data from mail!", LogLevel.Error);
                }

                Tool toolForMail = Game1.player.toolBeingUpgraded.Value;
                var (toolType, level) = pToolData.Value;  
                
                /*
                 * Check if the current upgrade tool matches with the tool which was assigned to this mail.
                 * 
                 * Since the upgrade mail content is generated when the mail is opened, the current upgrade tool 
                 * could have changed in the meantime. In this case, no tool will be included in this mail.
                 */
                if (toolForMail != null && (toolForMail.GetType() != toolType || toolForMail.UpgradeLevel != level))
                {
                    toolForMail = null;
                }

                // Set the water level to full for the upgraded watering can.
                if (toolForMail is WateringCan can)
                {
                    can.WaterLeft = can.waterCanMax;
                }

                var mailMessage = reflectionHelper.GetField<List<string>>(letterViewerMenu, "mailMessage").GetValue();
                Game1.activeClickableMenu = new LetterViewerMenuForToolUpgrade(mailMessage[0], toolForMail);
            }
        }

        private (Type toolType, int level)? GetMailAssignedTool(string mailKey)
        {
            var keyParts = mailKey.Split(':');
            if (keyParts.Count() != 3)
            {
                return null;
            }

            if (ToolHelper.TryParse(keyParts[1], out Type tool) && int.TryParse(keyParts[2], out int level))
            {
                return (tool, level);
            }

            return null;
        }
    }
}
