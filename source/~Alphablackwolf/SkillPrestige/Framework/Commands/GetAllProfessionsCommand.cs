/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Alphablackwolf/SkillPrestige
**
*************************************************/

using System.Linq;
using SkillPrestige.Logging;
using StardewValley;

namespace SkillPrestige.Framework.Commands
{
    /// <summary>A command that resets the player's professions after all professions has been removed.</summary>
    // ReSharper disable once UnusedMember.Global - referenced via reflection
    internal class GetAllProfessionsCommand : SkillPrestigeCommand
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        public GetAllProfessionsCommand()
            : base("player_getallprofessions", "Returns a list of all professions the player has.\n\nUsage: player_getallprofessions", testingCommand: true) { }


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

            Logger.LogInformation("getting list of all professions...");
            foreach (var skill in Skill.AllSkills)
            {
                var obtainedProfessions = skill.Professions.Where(x => Game1.player.professions.Contains(x.Id));
                string professionNames = string.Join(", ", obtainedProfessions.Select(x => x.DisplayName));
                ModEntry.LogMonitor.Log($"{skill.Type.Name} skill (level: {skill.GetSkillLevel()}) professions: {professionNames}");
            }
            Logger.LogInformation("list of all professions retrieved.");
        }
    }
}
