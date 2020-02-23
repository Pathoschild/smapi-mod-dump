using NpcAdventure.Events;
using NpcAdventure.Loader;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NpcAdventure.Driver
{
    public class MailDriver
    {
        private readonly IContentLoader contentLoader;
        private readonly IMonitor monitor;

        public MailDriver(IContentLoader contentLoader, IMonitor monitor)
        {
            this.contentLoader = contentLoader;
            this.monitor = monitor;
        }

        internal void RegisterEvents(ISpecialModEvents events)
        {
            events.MailboxOpen += this.Events_MailboxOpen;
        }

        private void Events_MailboxOpen(object sender, IMailEventArgs e)
        {
            Dictionary<string, string> dictionary = this.contentLoader.LoadStrings("Strings/Mail");

            e.Mailbox.RemoveAt(0);
            e.Player.mailReceived.Add(e.FullLetterKey);

            if (!string.IsNullOrEmpty(e.LetterKey) && dictionary.ContainsKey(e.LetterKey))
            {
                Game1.activeClickableMenu = new LetterViewerMenu(dictionary[e.LetterKey], e.FullLetterKey, false);
                this.monitor.Log($"Draw mail letter `{e.LetterKey}`");
            } else
            {
                this.monitor.Log($"Can't draw undefined mail letter: {e.LetterKey ?? "null"}");
            }
        }
    }
}
