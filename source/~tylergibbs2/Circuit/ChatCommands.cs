/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using Circuit.UI;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;

namespace Circuit
{
    internal static class ChatCommands
    {
        public static bool HandleCommand(string command)
        {
            string[] split = command.Split(' ');

            switch (split[0].ToLower())
            {
                case "start":
                    Start();
                    return true;
                case "tasks":
                case "task":
                    Game1.activeClickableMenu = new TaskListMenu();
                    return true;
            }

            return false;
        }

        private static void Start()
        {
            bool started = ModEntry.Instance.TryStart(out string? error);
            if (!started)
            {
                Game1.chatBox.addErrorMessage(error!);
                return;
            }

            string toggleKey = ModEntry.Instance.Config.OpenTaskView.ToString();

            Game1.chatBox.addInfoMessage("The run has started.");
            Game1.chatBox.addMessage($"You can view your tasks at any time with /tasks or by pressing {toggleKey}.", Color.Gray);
            Logger.Log("Run started.", LogLevel.Info);
        }
    }
}
