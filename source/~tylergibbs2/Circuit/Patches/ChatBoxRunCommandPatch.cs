/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using StardewValley.Menus;

namespace Circuit.Patches
{
    [HarmonyPatch(typeof(ChatBox), "runCommand")]
    internal class ChatBoxRunCommandPatch
    {
        public static bool Prefix(string command)
        {
            return !ChatCommands.HandleCommand(command);
        }
    }
}
