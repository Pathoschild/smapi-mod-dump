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
    /// <summary>A command that restores selected prestiged profession to a player's profession list</summary>
    // ReSharper disable once UnusedMember.Global - referenced via reflection
    internal class RestorePrestigedProfessionsCommand : SkillPrestigeCommand
    {
        /// <summary>Construct an instance.</summary>
        public RestorePrestigedProfessionsCommand()
            : base("player_restoreprestigedprofessions", GetDescription()) { }

        /// <summary>Applies the effect of a command when it is called from the console.</summary>
        protected override void Apply(string[] args)
        {
            if (Game1.player == null)
            {
                ModEntry.LogMonitor.Log("A game file must be loaded in order to run this command.");
                return;
            }

            if (PrestigeSet.Instance == null)
            {
                ModEntry.LogMonitor.Log(
                    "Prestige data is null, ensure you have prestige data loaded before running this command");
                return;
            }

            foreach (var prestige in PrestigeSet.Instance.Prestiges)
            {
                foreach (int professionId in prestige.PrestigeProfessionsSelected)
                {
                    if (Game1.player.professions.Contains(professionId))
                    {
                        ModEntry.LogMonitor.Log($"Player already has profession id: {professionId}, skipping add...");
                        continue;
                    }

                    var skill = Skill.AllSkills.SingleOrDefault(x => x.Type.Name == prestige.SkillType.Name);
                    if (skill is null)
                    {
                        ModEntry.LogMonitor.Log($"unable to load skill: {prestige.SkillType.Name}, skipping profession add...");
                        continue;
                    }

                    var profession = skill.Professions.SingleOrDefault(x => x.Id == professionId);
                    if (profession is null)
                    {
                        ModEntry.LogMonitor.Log($"unable to load profession: {professionId}, skipping profession add...");
                        continue;
                    }
                    Logger.LogInformation($"Adding profession {professionId}...");
                    Game1.player.professions.Add(profession.Id);
                    profession.SpecialHandling?.ApplyEffect();
                    ModEntry.LogMonitor.Log($"Profession {professionId}: {profession.DisplayName} added.");
                }
            }
        }

        /// <summary>Get the command's help description.</summary>
        private static string GetDescription()
        {
            return
                "Restore prestiged professions the user has selected that have been cleared from the player, e.g. using the player_resetallprofessions command.\n\n"
                + "Usage: with a save file loaded: player_restoreprestigedprofessions\n";
        }
    }
}
