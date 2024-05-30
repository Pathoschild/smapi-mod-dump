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
    /// <summary>A command that sets experience levels for a player.</summary>
    // ReSharper disable once UnusedMember.Global - referenced via reflection
    internal class SetExperienceCommand : SkillPrestigeCommand
    {
        /// <summary>Construct an instance.</summary>
        public SetExperienceCommand()
            : base("player_setexperience", GetDescription(), testingCommand: true) { }

        /// <summary>Applies the effect of a command when it is called from the console.</summary>
        protected override void Apply(string[] args)
        {
            if (args.Length <= 1)
            {
                ModEntry.LogMonitor.Log("<skill> and <value> must be specified");
                return;
            }
            string skillArgument = args[0];
            if (!Skill.AllSkills.Select(x => x.Type.Name).Contains(skillArgument, StringComparer.InvariantCultureIgnoreCase))
            {
                ModEntry.LogMonitor.Log("<skill> is invalid");
                return;
            }
            if (!int.TryParse(args[1], out int experienceArgument))
            {
                ModEntry.LogMonitor.Log("experience must be an integer.");
                return;
            }
            if (Game1.player == null)
            {
                ModEntry.LogMonitor.Log("A game file must be loaded in order to run this command.");
                return;
            }
            Logger.LogInformation("Setting experience level...");
            Logger.LogVerbose($"experience argument: {experienceArgument}");
            experienceArgument = experienceArgument.Clamp(0, 100000);
            Logger.LogVerbose($"experience used: {experienceArgument}");
            var skill = Skill.AllSkills.Single(x => x.Type.Name.Equals(skillArgument, StringComparison.InvariantCultureIgnoreCase));

            int playerSkillExperience = skill.GetSkillExperience();
            Logger.LogVerbose($"Current experience level for {skill.Type.Name} skill: {playerSkillExperience}");
            Logger.LogVerbose($"Setting {skill.Type.Name} skill to {experienceArgument} experience.");
            ExperienceHandler.DisableExperienceGains = true;
            skill.SetSkillExperience(experienceArgument);
            ExperienceHandler.DisableExperienceGains = false;
            int skillLevel = GetLevel(experienceArgument);
            Logger.LogVerbose($"Setting skill level for {experienceArgument} experience: {skillLevel}");
            skill.SetSkillLevel(skillLevel);

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

        /// <summary>Get the level unlocked with the given experience.</summary>
        /// <param name="experience">The total experience points.</param>
        private static int GetLevel(int experience)
        {
            return experience switch
            {
                < 100 => 0,
                < 380 => 1,
                < 770 => 2,
                < 1300 => 3,
                < 2150 => 4,
                < 3300 => 5,
                < 4800 => 6,
                < 6900 => 7,
                < 10000 => 8,
                _ => experience < 15000 ? 9 : 10
            };
        }
    }
}
