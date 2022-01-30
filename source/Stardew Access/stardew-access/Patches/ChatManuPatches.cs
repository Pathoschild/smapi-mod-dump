/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stardew-access/stardew-access
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class ChatManuPatches
    {
        private static int currentChatMessageIndex = 0;
        private static bool isChatRunning = false;

        internal static void ChatBoxPatch(ChatBox __instance, List<ChatMessage> ___messages)
        {
            try
            {
                string toSpeak = " ";

                if (__instance.chatBox.Selected)
                {
                    bool isPrevArrowPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.PageUp);
                    bool isNextArrowPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.PageDown);

                    if (___messages.Count > 0)
                    {
                        #region To narrate previous and next chat messages
                        if (isNextArrowPressed && !isChatRunning)
                        {
                            _ = CycleThroughChatMessages(true, ___messages);
                        }
                        else if (isPrevArrowPressed && !isChatRunning)
                        {
                            _ = CycleThroughChatMessages(false, ___messages);
                        }
                        #endregion
                    }
                }
                else if (___messages.Count > 0)
                {
                    #region To narrate latest chat message
                    ___messages[___messages.Count - 1].message.ForEach(message =>
                    {
                        toSpeak += $"{message.message}, ";
                    });
                    if (toSpeak != " ")
                        MainClass.screenReader.SayWithChatChecker(toSpeak, false);
                    #endregion
                }
            }
            catch (Exception e)
            {
                MainClass.monitor.Log($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}", LogLevel.Error);
            }
        }

        private static async Task CycleThroughChatMessages(bool increase, List<ChatMessage> ___messages)
        {
            isChatRunning = true;
            await Task.Delay(200);
            string toSpeak = " ";
            if (increase)
            {
                ++currentChatMessageIndex;
                if (currentChatMessageIndex > ___messages.Count - 1)
                {
                    currentChatMessageIndex = ___messages.Count - 1;
                }
            }
            else
            {
                --currentChatMessageIndex;
                if (currentChatMessageIndex < 0)
                {
                    currentChatMessageIndex = 0;
                }
            }
            ___messages[currentChatMessageIndex].message.ForEach(message =>
            {
                toSpeak += $"{message.message}, ";
            });

            MainClass.screenReader.Say(toSpeak, true);
            isChatRunning = false;
        }
    }
}
