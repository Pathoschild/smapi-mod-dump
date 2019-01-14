using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;

namespace SilentScarecrows
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.Display.MenuChanged += m;
        }

        private void m(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is DialogueBox dialog) {
                Log("Current string "+dialog.getCurrentString());

                if (dialog.getCurrentString().Contains("I haven't encountered any crows yet") || dialog.getCurrentString().Contains("I've scared off"))
                {
                    dialog.closeDialogue();
                }
            }
        }

        internal void Log(string message, LogLevel logLevel = LogLevel.Debug)
        {
#if DEBUG
            this.Monitor.Log(message, logLevel);
#endif
        }
    }
}
