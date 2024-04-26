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
    internal class AddProfessionCommand : SkillPrestigeCommand
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        public AddProfessionCommand()
            : base("player_addprofession", GetDescription()) { }


        /*********
        ** Protected methods
        *********/
        /// <summary>Applies the effect of a command when it is called from the console.</summary>
        protected override void Apply(string[] args)
        {
            if (args.Length < 1)
            {
                ModEntry.LogMonitor.Log("<profession> must be specified");
                return;
            }
            string professionArgument = args[0];
            if (!Skill.AllSkills.SelectMany(x => x.Professions).Select(x => x.DisplayName).Contains(professionArgument, StringComparer.InvariantCultureIgnoreCase))
            {
                ModEntry.LogMonitor.Log("<profession> is invalid");
                return;
            }
            if (Game1.player == null)
            {
                ModEntry.LogMonitor.Log("A game file must be loaded in order to run this command.");
                return;
            }
            var profession = Skill.AllSkills.SelectMany(x => x.Professions).Single(x => x.DisplayName.Equals(professionArgument, StringComparison.InvariantCultureIgnoreCase));
            if (Game1.player.professions.Contains(profession.Id))
            {
                ModEntry.LogMonitor.Log("profession already added.");
            }
            Logger.LogInformation($"Adding profession {professionArgument}...");
            Game1.player.professions.Add(profession.Id);
            profession.SpecialHandling?.ApplyEffect();
            Logger.LogInformation($"Profession {professionArgument} added.");
        }

        /// <summary>Get the command's help description.</summary>
        private static string GetDescription()
        {
            string professionNames = string.Join(", ", Skill.AllSkills.SelectMany(x => x.Professions).Select(x => x.DisplayName));
            return
                "Adds the specified profession to the player.\n\n"
                + "Usage: player_addprofession <profession>\n"
                + $"- profession: the name of the profession to add (one of {professionNames}).";
        }
    }
}
