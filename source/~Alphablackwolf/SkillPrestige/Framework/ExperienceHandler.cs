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

namespace SkillPrestige.Framework
{
    /// <summary>Handles experience adjustments for skills.</summary>
    internal static class ExperienceHandler
    {
        // ReSharper disable once InconsistentNaming
        private static bool _disableExperienceGains;
        private static bool ExperienceLoaded { get; set; }
        private static int[] LastExperiencePoints { get; set; }

        public static bool DisableExperienceGains
        {
            private get => _disableExperienceGains;
            set
            {
                if (_disableExperienceGains != value)
                    Logger.LogInformation($"{(value ? "Enabling" : "Disabling")} experience gains from prestige points...");
                _disableExperienceGains = value;
                ResetExperience();
            }
        }

        public static void ResetExperience()
        {
            ExperienceLoaded = false;
            LastExperiencePoints = null;
        }

        public static void UpdateExperience()
        {
            if (DisableExperienceGains || !PerSaveOptions.Instance.UseExperienceMultiplier)
                return;
            if (!ExperienceLoaded)
            {
                ExperienceLoaded = true;
                LastExperiencePoints = Game1.player.experiencePoints.ToArray();
                Logger.LogVerbose("Loaded Experience state.");
                return;
            }
            if (Game1.player.experiencePoints.SequenceEqual(LastExperiencePoints))
                return;
            if (Game1.player.experiencePoints.Length != LastExperiencePoints.Length)
            {
                LastExperiencePoints = Game1.player.experiencePoints.ToArray();
                return;
            }
            for (int skillIndex = 0; skillIndex < Game1.player.experiencePoints.Length; skillIndex++)
            {
                bool skillHasAPrestige = Skill.AllSkills.Any(x => x.Type.Ordinal == skillIndex);
                if (!skillHasAPrestige)
                    continue;
                int lastExperienceDetected = LastExperiencePoints[skillIndex];
                int currentExperience = Game1.player.experiencePoints[skillIndex];
                int gainedExperience = currentExperience - lastExperienceDetected;
                decimal skillExperienceFactor = PrestigeSet.Instance.Prestiges.Single(x => x.SkillType.Ordinal == skillIndex).PrestigePoints * PerSaveOptions.Instance.ExperienceMultiplier;
                if (gainedExperience <= 0 || skillExperienceFactor <= 0)
                    continue;
                Logger.LogVerbose($"Detected {gainedExperience} experience gained in {Skill.AllSkills.Single(x => x.Type.Ordinal == skillIndex).Type.Name} skill.");
                const int maxExtraExperience = 100000;
                int extraExperience = (gainedExperience * skillExperienceFactor).Floor().Clamp(0, maxExtraExperience);
                Logger.LogVerbose($"Adding {extraExperience} experience to {Skill.AllSkills.Single(x => x.Type.Ordinal == skillIndex).Type.Name} skill.");
                Game1.player.gainExperience(skillIndex, extraExperience);
            }
            LastExperiencePoints = Game1.player.experiencePoints.ToArray();
        }
    }
}
