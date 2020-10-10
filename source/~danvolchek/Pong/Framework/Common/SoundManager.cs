/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

using StardewValley;

namespace Pong.Framework.Common
{
    internal static class SoundManager
    {
        public static void PlayBallWallSound()
        {
            Game1.playSound("smallSelect");
        }

        public static void PlayBallPaddleSound()
        {
            Game1.playSound("smallSelect");
        }

        public static void PlayPointWonSound(bool playerWon)
        {
            Game1.playSound(playerWon ? "achievement" : "trashcan");
        }

        public static void PlayCountdownSound()
        {
            Game1.playSound("throwDownITem");
        }

        public static void PlayKeyPressSound()
        {
            Game1.playSound("bigDeSelect");
        }
    }
}
