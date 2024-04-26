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
using StardewValley;

namespace SkillPrestige.Framework.Commands
{
    /// <summary>A command that resets the player's professions after all professions has been removed.</summary>
    // ReSharper disable once UnusedMember.Global - referenced via reflection
    internal class ResetPrestigeCommand : SkillPrestigeCommand
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        public ResetPrestigeCommand()
            : base("player_resetprestige", GetDescription(), testingCommand: true) { }


        /*********
        ** Protected methods
        *********/
        /// <summary>Applies the effect of a command when it is called from the console.</summary>
        protected override void Apply(string[] args)
        {
            if (args.Length < 1)
            {
                ModEntry.LogMonitor.Log("<skill> must be specified");
                return;
            }
            if (Game1.player == null)
            {
                ModEntry.LogMonitor.Log("A game file must be loaded in order to run this command.");
                return;
            }
            string skillArgument = args[0];
            ModEntry.LogMonitor.Log(
                $"This command will reset your character's prestiged selections and prestige points for the {skillArgument} skill.\n"
                + "Please note that this command by itself will only clear the prestige data located in the skills prestige mod folder, "
                + "and *not* the player's gained professions. once this is run all professions already prestiged/purchased will still belong to the player."
                + "If you have read this and wish to continue confirm with 'y' or 'yes'"
            );
            string response = Console.ReadLine();
            if (response == null || (!response.Equals("y", StringComparison.InvariantCultureIgnoreCase) && !response.Equals("yes", StringComparison.InvariantCultureIgnoreCase)))
            {
                Logger.LogVerbose($"Cancelled prestige reset for {skillArgument} skill.");
                return;
            }
            Logger.LogInformation($"Resetting prestige data for {skillArgument} skill...");
            var prestige = PrestigeSaveData.CurrentlyLoadedPrestigeSet.Prestiges.Single(x => x.SkillType.Name.Equals(skillArgument, StringComparison.InvariantCultureIgnoreCase));
            prestige.PrestigePoints = 0;
            prestige.PrestigeProfessionsSelected = new List<int>();
            PrestigeSaveData.Instance.Save();
            Logger.LogInformation($"{skillArgument} skill prestige data reset.");
        }

        /// <summary>Get the command's help description.</summary>
        private static string GetDescription()
        {
            string skillNames = string.Join(", ", Skill.AllSkills.Select(x => x.Type.Name));
            return
                "Resets prestiged professions and prestige points for a specific skill.\n\n"
                + "Usage: player_resetprestige <skill>\n"
                + $"- skill: the name of the skill (one of {skillNames}).";
        }
    }
}
