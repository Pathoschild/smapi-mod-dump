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
using StardewValley;

namespace SkillPrestige.Framework.Commands
{
    /// <summary>A command that clears all professions from a player's game.</summary>
    // ReSharper disable once UnusedMember.Global - referenced via reflection
    internal class AddPrestigePointCommand : SkillPrestigeCommand
    {
        /// <summary>Construct an instance.</summary>
        public AddPrestigePointCommand()
            : base("player_addprestigepoint", GetDescription(), testingCommand: true) { }

        /// <summary>Applies the effect of a command when it is called from the console.</summary>
        protected override void Apply(string[] args)
        {
            if (args.Length < 1)
            {
                ModEntry.LogMonitor.Log("<skill> must be specified");
                return;
            }
            string skillArgument = args[0];
            if (!Skill.AllSkills.Select(x => x.Type.Name).Contains(skillArgument, StringComparer.InvariantCultureIgnoreCase))
            {
                ModEntry.LogMonitor.Log("<skill> is invalid");
                return;
            }
            if (Game1.player == null)
            {
                ModEntry.LogMonitor.Log("A game file must be loaded in order to run this command.");
                return;
            }

            var skill = Skill.AllSkills.Single(x =>
                x.Type.Name.Equals(skillArgument, StringComparison.InvariantCultureIgnoreCase));
            var prestige = PrestigeSet.Instance.Prestiges.SingleOrDefault(x => x.SkillType.Name == skill.Type.Name);
            if (prestige is null)
            {
                ModEntry.LogMonitor.Log("Prestige not found, unable to add point.");
                return;
            }
            ModEntry.LogMonitor.Log($"Adding prestige point to {skill.Type.Name} skill...");
            prestige.PrestigePoints++;
            ModEntry.LogMonitor.Log("Prestige point added.");
        }

        /// <summary>Get the command's help description.</summary>
        private static string GetDescription()
        {
            string skillNames = string.Join(", ", Skill.AllSkills.SelectMany(x => x.Type.Name));
            return
                "Adds a prestige point to the specified skill.\n\n"
                + "Usage: player_addprestigepoint <skill>\n"
                + $"- skill: the name of the skill to add a point to. current options are {skillNames}.";
        }
    }
}
