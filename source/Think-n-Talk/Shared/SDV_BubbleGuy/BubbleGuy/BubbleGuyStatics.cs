/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Prism-99/Think-n-Talk
**
*************************************************/



using StardewValley;

namespace SDV_Speaker.Speaker
{
   internal static class BubbleGuyStatics
    {
        public static string ModPath;
        public static string BubbleGuyName;
        public static string BubbleGuyPrefix => "BubbleGuy_";
        public static void Initialize(string sModPath)
        {
            ModPath = sModPath;
            BubbleGuyName = $"{BubbleGuyPrefix}{Game1.player.uniqueMultiplayerID}";
        }
    }
}
