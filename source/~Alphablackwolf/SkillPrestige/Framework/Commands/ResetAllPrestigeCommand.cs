/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Alphablackwolf/SkillPrestige
**
*************************************************/

using System;
using System.Collections.Generic;
using SkillPrestige.Logging;
using StardewValley;

namespace SkillPrestige.Framework.Commands
{
    /// <summary>A command that resets the player's professions after all professions has been removed.</summary>
    // ReSharper disable once UnusedMember.Global - referenced via reflection
    internal class ResetAllPrestigeCommand : SkillPrestigeCommand
    {
        /// <summary>Construct an instance.</summary>
        public ResetAllPrestigeCommand()
            : base("player_resetallprestige", "Resets all prestige professions and prestige points.\n\nUsage: player_resetallprestige", testingCommand: true) { }

        /// <summary>Applies the effect of a command when it is called from the console.</summary>
        protected override void Apply(string[] args)
        {
            if (Game1.player == null)
            {
                ModEntry.LogMonitor.Log("A game file must be loaded in order to run this command.");
                return;
            }

            ModEntry.LogMonitor.Log(
                "This command will reset your character's prestiged selections and prestige points.\n"
                + "it is recommended you run the player_resetAllProfessions command after running this command.\n"
                + "Please note that this command by itself will only clear the prestige data located in the skills prestige mod folder, "
                + "and *not* the player's gained professions. once this is run all professions already prestiged/purchased will still belong to the player.\n"
                + "If you have read this and wish to continue confirm with 'y' or 'yes'"
            );
            string response = Console.ReadLine();
            if (response == null || (!response.Equals("y", StringComparison.InvariantCultureIgnoreCase) && !response.Equals("yes", StringComparison.InvariantCultureIgnoreCase)))
            {
                Logger.LogVerbose("Cancelled all prestige reset.");
                return;
            }
            Logger.LogInformation($"Resetting all skill prestiges...");
            foreach (var prestige in PrestigeSet.Instance.Prestiges)
            {
                prestige.PrestigePoints = 0;
                prestige.PrestigeProfessionsSelected = new List<int>();
            }

            PrestigeSet.Save();
            Logger.LogInformation("Prestiges reset.");
        }
    }
}
