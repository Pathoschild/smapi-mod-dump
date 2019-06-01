using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewValley;

namespace MailFrameworkMod
{
    public class Commands
    {
        public static IMonitor ModMonitor = MailFrameworkModEntry.ModMonitor;

        public static void AddsReceivedMail(string command, string[] args)
        {
            if (Context.IsWorldReady)
            {
                if (args.Length > 0)
                {
                    string mailName = String.Join(" ", args);

                    if (!Game1.player.mailReceived.Contains(mailName))
                    { 

                        Game1.player.mailReceived.Add(mailName);
                        ModMonitor.Log($"Mail '{mailName}' added to the player as received.", LogLevel.Info);
                    }
                    else
                    {
                        ModMonitor.Log($"The player already has the mail '{mailName}' as received.", LogLevel.Info);
                    }
                }
                else
                {
                    ModMonitor.Log($"No mail name was given to the command.", LogLevel.Info);
                }
            }
            else
            {
                ModMonitor.Log("No player add mail as received.", LogLevel.Info);
            }
        }

        public static void RemoveReceivedMail(string command, string[] args)
        {
            if (Context.IsWorldReady)
            {
                if (args.Length > 0)
                {
                    string mailName = String.Join(" ", args);

                    if (Game1.player.mailReceived.Contains(mailName))
                    {

                        Game1.player.mailReceived.Remove(mailName);
                        ModMonitor.Log($"Mail '{mailName}' removed from the player received list.", LogLevel.Info);
                    }
                    else
                    {
                        ModMonitor.Log($"The player does not have the mail '{mailName}' as received.", LogLevel.Info);
                    }
                }
                else
                {
                    ModMonitor.Log($"No mail name was given to the command.", LogLevel.Info);
                }
            }
            else
            {
                ModMonitor.Log("No player remove mail as received.", LogLevel.Info);
            }
        }
    }
}
