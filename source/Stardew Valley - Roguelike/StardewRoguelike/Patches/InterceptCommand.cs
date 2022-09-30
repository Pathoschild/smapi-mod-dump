/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyRoguelike
**
*************************************************/

using StardewValley;
using StardewValley.Menus;

namespace StardewRoguelike.Patches
{
    internal class InterceptCommand : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(ChatBox), "runCommand");

        public static bool Prefix(string command)
        {
            return CommandHandler.Handle(command);
        }
    }

    internal class CustomHelp : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(ChatBox), "showHelp");

        public static bool Prefix(ChatBox __instance)
        {
            __instance.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_Help"));
            __instance.addInfoMessage("stats: shows the current run stats");
            __instance.addInfoMessage("character: opens the character customization menu");
            __instance.addInfoMessage("reset: resets the run");
            __instance.addInfoMessage("stuck: helps ensure progression");
            if (Game1.IsServer)
            {
                __instance.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_HelpKick", "kick"));
                __instance.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_HelpBan", "ban"));
                __instance.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_HelpUnban", "unban"));
            }

            return false;
        }
    }
}
