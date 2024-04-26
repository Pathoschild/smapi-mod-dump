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
using System.Linq;
using SkillPrestige.Logging;
using StardewValley;

namespace SkillPrestige.Framework.Commands
{
    /// <summary>A command that clears all professions from a player's game.</summary>
    // ReSharper disable once UnusedMember.Global - referenced via reflection
    internal class ClearAllProfessionsCommand : SkillPrestigeCommand
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        public ClearAllProfessionsCommand()
            : base("player_clearallprofessions", "Removes all professions for the current game file.\n\nUsage: player_clearallprofessions\n") { }


        /*********
        ** Protected methods
        *********/
        /// <summary>Applies the effect of a command when it is called from the console.</summary>
        protected override void Apply(string[] args)
        {
            if (Game1.player == null)
            {
                ModEntry.LogMonitor.Log("A game file must be loaded in order to run this command.");
                return;
            }

            ModEntry.LogMonitor.Log("This command will remove all of your character's professions.\nIf you have read this and wish to continue confirm with 'y' or 'yes'");
            string response = Console.ReadLine();
            if (response == null || !response.Equals("y", StringComparison.InvariantCultureIgnoreCase) && !response.Equals("yes", StringComparison.InvariantCultureIgnoreCase))
            {
                Logger.LogVerbose("Cancelled clear all professions..");
                return;
            }
            Logger.LogVerbose("Clearing all professions...");

            var specialHandlingsForSkillsRemoved = Skill
                .AllSkills
                .SelectMany(skill => skill.Professions)
                .Where(prof => Game1.player.professions.Contains(prof.Id) && prof.SpecialHandling != null)
                .Select(prof => prof.SpecialHandling);

            Game1.player.professions.Clear();
            foreach (var specialHandling in specialHandlingsForSkillsRemoved)
                specialHandling.RemoveEffect();

            Logger.LogVerbose("Professions cleared.");
        }
    }
}
