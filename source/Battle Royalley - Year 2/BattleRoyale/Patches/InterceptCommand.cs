/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/BattleRoyalley-Year2
**
*************************************************/

using StardewValley;
using StardewValley.Menus;

namespace BattleRoyale.Patches
{
    class InterceptCommand : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(ChatBox), "runCommand");

        public static bool Prefix(string command)
        {
            bool continueExecution = CommandHandler.Handle(command);
            return continueExecution;
        }
    }

    class CustomHelp : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(ChatBox), "showHelp");

        public static bool Prefix(ChatBox __instance)
        {

            __instance.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_Help"));
            __instance.addInfoMessage("spectate: places you into spectator mode");
            __instance.addInfoMessage("play: removes you from spectator mode");
            __instance.addInfoMessage("kill: kills yourself");
            __instance.addInfoMessage("name [new name]: renames yourself");
            __instance.addInfoMessage("character: opens the character customization menu");
            if (Game1.IsServer)
            {
                __instance.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_HelpKick", "kick"));
                __instance.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_HelpBan", "ban"));
                __instance.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_HelpUnban", "unban"));
                __instance.addInfoMessage("start: starts the game");
                __instance.addInfoMessage("lobby: returns the game to the lobby");
            }

            return false;
        }
    }
}
