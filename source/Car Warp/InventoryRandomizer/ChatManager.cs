/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Menus;

namespace InventoryRandomizer;

internal static class ChatManager
{
    private static List<ChatMessage> ChatMessages;

    internal static bool IsChatCommandsInstalled = false;

    internal static void GetChatBox()
    {
        ChatMessages = Globals.ReflectionHelper.GetField<List<ChatMessage>>(Game1.chatBox, "messages").GetValue();
    }

    internal static void DisplayCurrentConfigMessage()
    {
        DisplayChatMessage($"Inventory set to randomize every {Globals.Config.SecondsUntilInventoryRandomization} seconds!");
    }

    internal static void DisplayChatMessage(string message)
    {
        Game1.chatBox.addInfoMessage(message);
    }

    internal static void DisplayTimedChatMessage(string message, int time)
    {
        Game1.chatBox.addInfoMessage(message);
        ChatMessages[^1].timeLeftToDisplay = time;
    }

    internal static void ClearPreviousMessages()
    {
        // Chat Commands doesn't like messages being removed
        if (IsChatCommandsInstalled)
            return;

        ChatMessages.RemoveAll(chatMessage =>
            chatMessage.message[0].message.Contains("Randomizing inventory"));
    }

    public static void CheckIfChatCommandsPresent()
    {
        if (Globals.Helper.ModRegistry.IsLoaded("cat.chatcommands"))
        {
            IsChatCommandsInstalled = true;
        }
    }
}
