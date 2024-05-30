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
    /// <summary>A command that sets mastery experience levels for a player.</summary>
    // ReSharper disable once UnusedMember.Global - referenced via reflection
    internal class SetMasteryExperienceCommand : SkillPrestigeCommand
    {
        /// <summary>Construct an instance.</summary>
        public SetMasteryExperienceCommand()
            : base("player_setmasteryexperience", GetDescription(), testingCommand: true) { }

        /// <summary>Applies the effect of a command when it is called from the console.</summary>
        protected override void Apply(string[] args)
        {
            if (args.Length < 1)
            {
                ModEntry.LogMonitor.Log(" <value> must be specified");
                return;
            }
            if (!int.TryParse(args[0], out int experienceArgument))
            {
                ModEntry.LogMonitor.Log("experience must be an integer.");
                return;
            }
            if (Game1.player == null)
            {
                ModEntry.LogMonitor.Log("A game file must be loaded in order to run this command.");
                return;
            }
            Logger.LogInformation("Setting mastery experience ...");
            Logger.LogVerbose($"experience argument: {experienceArgument}");
            experienceArgument = experienceArgument.Clamp(0, 100000);
            Logger.LogVerbose($"experience used: {experienceArgument}");

            Game1.stats.Set("MasteryExp", experienceArgument);

        }

        /// <summary>Get the command's help description.</summary>
        private static string GetDescription()
        {
            string skillNames = string.Join(", ", Skill.AllSkills.Select(x => x.Type.Name));
            return
                "Sets the player's specified skill to the specified level of experience.\n\n"
                + "Usage: player_setexperience <skill> <level>\n"
                + $"- skill: the name of the skill (one of {skillNames}).\n"
                + "- level: the target experience level.";
        }
    }
}
