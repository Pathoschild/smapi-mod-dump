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
using System.Linq;
using SkillPrestige.Logging;
using SkillPrestige.Professions;
using StardewValley;

namespace SkillPrestige.Framework.Commands
{
    /// <summary>A command that resets the player's professions after all professions has been removed.</summary>
    // ReSharper disable once UnusedMember.Global - referenced via reflection
    internal class ResetAllProfessionsCommand : SkillPrestigeCommand
    {
        /// <summary>Construct an instance.</summary>
        public ResetAllProfessionsCommand()
            : base("player_resetallprofessions", "Resets professions from all profession mods to only be a single profession tree for each skill.\n\nUsage: player_resetallprofessions") { }

        /// <summary>Applies the effect of a command when it is called from the console.</summary>
        protected override void Apply(string[] args)
        {
            if (ModEntry.ModRegistry.IsLoaded("community.AllProfessions"))
                ModEntry.LogMonitor.Log("Command cannot be run while AllProfessions is still installed. Please remove AllProfessions before proceeding.");

            if (Game1.player == null)
            {
                ModEntry.LogMonitor.Log("A game file must be loaded in order to run this command.");
                return;
            }

            ModEntry.LogMonitor.Log(
                "This command will reset your character's professions to the first profession available for each skill that your skill level warrants.\n"
                + "For example, if your farming skill is level 10, you will only have the Rancher and Coopmaster skills after this command has been run.\n"
                + "If you would prefer to choose your professions, use the player_clearallprofessions command, followed by the player_addprofession command for each profession you wish to add.\n"
                + "If you have read this and wish to continue confirm with 'y' or 'yes'"
            );
            string response = Console.ReadLine();
            if (response == null || (!response.Equals("y", StringComparison.InvariantCultureIgnoreCase) && !response.Equals("yes", StringComparison.InvariantCultureIgnoreCase)))
            {
                Logger.LogVerbose("Cancelled all profession reset.");
                return;
            }
            Logger.LogInformation("Resetting from all professions...");

            var professionsToKeep = new List<Profession>();
            foreach (var skill in Skill.AllSkills)
            {
                int skillLevel = skill.GetSkillLevel();
                Logger.LogVerbose($"Checking for professions to keep for skill {skill.Type.Name} at level {skillLevel}");
                var levelFiveProfession = skill.Professions.FirstOrDefault(x => skillLevel >= 5 && x is TierOneProfession);
                var levelTenProfession = skill.Professions.FirstOrDefault(x => skillLevel >= 10 && x is TierTwoProfession);
                if (levelFiveProfession != null)
                    professionsToKeep.Add(levelFiveProfession);
                if (levelTenProfession != null)
                    professionsToKeep.Add(levelTenProfession);
            }

            var specialHandlingsForSkillsRemoved = Skill
                .AllSkills
                .SelectMany(skill => skill.Professions)
                .Where(prof => Game1.player.professions.Contains(prof.Id) && prof.SpecialHandling != null)
                .Select(prof => prof.SpecialHandling);

            Game1.player.professions.Clear();
            foreach (var profession in professionsToKeep)
                Game1.player.professions.Add(profession.Id);
            foreach (var specialHandling in specialHandlingsForSkillsRemoved)
                specialHandling.RemoveEffect();
            Logger.LogInformation("Professions reset.");
        }
    }
}
