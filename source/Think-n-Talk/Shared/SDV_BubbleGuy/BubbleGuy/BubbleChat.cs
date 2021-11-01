/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Prism-99/Think-n-Talk
**
*************************************************/


using StardewValley.SDKs;
using StardewModdingAPI;

namespace SDV_Speaker.Speaker
{

    internal static class BubbleChat
    {
        private static BubbleGuyManager oManager;
        private static SteamHelper _sdk;

        public static void Initialize(BubbleGuyManager manager)
        {
            oManager = manager;
            _sdk = new SteamHelper();
        }
        public static bool RunCommand(string command)
        {
            string sText = "";
            string[] arParts = command.Split(' ');
            bool bShowBubble = false;
            bool bIsThink = false;
#if LOG_DEBUG
            oManager.oMonitor.Log($"Command '{arParts[0]}", LogLevel.Info);
#endif
            switch (arParts[0])
            {
                case "think":
                    bShowBubble = true;
                    bIsThink = true;
                    break;
                case "talk":
                    bShowBubble = true;
                    break;
                case "clear":
                    oManager.RemoveBubbleGuy(false,false);
                    return false;
                default:
                    break;
            }
            if (bShowBubble)
            {
                if (command.Replace("think", "").Trim() == "" || command.Replace("talk", "").Trim() == "")
                {
                    oManager.RemoveBubbleGuy(false, false);
                }
                else
                {
                    sText = command.Substring(arParts[0].Length + 1);
                    sText = _sdk.FilterDirtyWords(sText);
                    oManager.AddBubbleGuy(bIsThink, sText);
                }
                return false;
            }
            else
            {
                return true;
            }

        }


    }
}
