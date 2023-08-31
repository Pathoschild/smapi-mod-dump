/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stardew-access/stardew-access
**
*************************************************/

using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class ChatBoxPatch
    {
        private static int currentChatMessageIndex = 0;
        private static bool isChatRunning = false;

        internal static void UpdatePatch(ChatBox __instance, List<ChatMessage> ___messages)
        {
            try
            {
                string toSpeak = " ";

                if (__instance.chatBox.Selected)
                {
                    bool isPrevButtonPressed = MainClass.Config.ChatMenuNextKey.JustPressed();
                    bool isNextButtonPressed = MainClass.Config.ChatMenuPreviousKey.JustPressed();

                    if (___messages.Count <= 0) return;

                    #region To narrate previous and next chat messages
                    if (isNextButtonPressed && !isChatRunning)
                    {
                        isChatRunning = true;
                        CycleThroughChatMessages(true, ___messages);
                        Task.Delay(200).ContinueWith(_ => { isChatRunning = false; });
                    }
                    else if (isPrevButtonPressed && !isChatRunning)
                    {
                        isChatRunning = true;
                        CycleThroughChatMessages(false, ___messages);
                        Task.Delay(200).ContinueWith(_ => { isChatRunning = false; });
                    }
                    #endregion
                }
                else if (___messages.Count > 0)
                {
                    #region To narrate latest chat message
                    ___messages[^1].message.ForEach(message =>
                    {
                        toSpeak += $"{message.message}, ";
                    });
                    if (toSpeak != " ")
                        MainClass.ScreenReader.SayWithChatChecker(toSpeak, false);
                    #endregion
                }
            }
            catch (Exception e)
            {
                Log.Error($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        private static void CycleThroughChatMessages(bool increase, List<ChatMessage> ___messages)
        {
            string toSpeak = " ";

            currentChatMessageIndex = (increase) ? (Math.Min(currentChatMessageIndex + 1, ___messages.Count - 1)) : (currentChatMessageIndex = Math.Max(currentChatMessageIndex - 1, 0));

            ___messages[currentChatMessageIndex].message.ForEach(message =>
            {
                toSpeak += $"{message.message}, ";
            });

            MainClass.ScreenReader.Say(toSpeak, true);
        }
    }
}
