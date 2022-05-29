/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ShivaGuy/StardewMods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using System;

namespace ShivaGuy.Stardew.CommandsValley
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            Helper.ConsoleCommands.Add("cv_cc", "", IsCommunityCenterComplete);
            Helper.ConsoleCommands.Add("cv_joja", "", IsJojaMartComplete);
            Helper.ConsoleCommands.Add("cv_shane8", "", Shane8HeartEventDone);
            Helper.ConsoleCommands.Add("cv_sewer", "", UnlockedSewer);
            Helper.ConsoleCommands.Add("cv_perfection", "", AchievedPerfection);
        }

        /// <summary>AKA Golden Egg unlocked.</summary>
        private void AchievedPerfection(string command, string[] argv)
        {
            Monitor.Log(Game1.MasterPlayer.mailReceived.Contains("Farm_Eternal") ? "Yes" : "...", LogLevel.Info);
        }

        /// <summary>AKA Void Egg unlocked.</summary>
        private void UnlockedSewer(string command, string[] args)
        {
            Monitor.Log(Game1.player.mailReceived.Contains("OpenedSewer") ? "Open" : "...", LogLevel.Info);
        }

        /// <summary>AKA Blue Chicken unlocked.</summary>
        private void Shane8HeartEventDone(string command, string[] args)
        {
            Monitor.Log(Game1.player.eventsSeen.Contains(3900074) ? "Seen" : "...", LogLevel.Info);
        }

        private void IsJojaMartComplete(string commmand, string[] args)
        {
            var mailReceived = Game1.MasterPlayer.mailReceived;
            bool jojaCompleted = mailReceived.Contains("JojaMember")
                || (mailReceived.Contains("jojaCraftsRoom")
                    && mailReceived.Contains("jojaVault")
                    && mailReceived.Contains("jojaFishTank")
                    && mailReceived.Contains("jojaBoilerRoom")
                    && mailReceived.Contains("jojaPantry"));

            Monitor.Log(jojaCompleted ? "Complete" : "...", LogLevel.Info);
        }

        private void IsCommunityCenterComplete(string command, string[] args)
        {
            var mailReceived = Game1.MasterPlayer.mailReceived;
            bool ccCompleted = mailReceived.Contains("ccIsComplete")
                || (mailReceived.Contains("ccCraftsRoom")
                    && mailReceived.Contains("ccVault")
                    && mailReceived.Contains("ccFishTank")
                    && mailReceived.Contains("ccBoilerRoom")
                    && mailReceived.Contains("ccPantry")
                    && mailReceived.Contains("ccBulletin"));

            Monitor.Log(ccCompleted ? "Complete" : "...", LogLevel.Info);
        }
    }
}
