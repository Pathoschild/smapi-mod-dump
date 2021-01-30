/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Quipex/ExperienceUpdates
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using System.Collections.Generic;
using System.Linq;

namespace ExperienceUpdates
{
    class ExperienceCalculator
    {
        public static readonly int[] expNeededForLevel = new int[] { 100, 380, 770, 1300, 2150, 3300, 4800, 6900, 10000, 15000 };
        private readonly int[] lastExp = new int[6];
        private readonly Dictionary<int, SparklingText> textsToSkill = new Dictionary<int, SparklingText>();
        private readonly IMonitor monitor;
        private bool running = false;

        public ExperienceCalculator(IMonitor monitor)
        {
            this.monitor = monitor;
        }

        internal void Stop()
        {
            this.running = false;
        }

        internal Dictionary<int, SparklingText> GetUpdatableTexts()
        {
            UpdateState();
            return textsToSkill;
        }

        internal void Reset()
        {
            var newExp = Game1.player.experiencePoints;
            newExp.CopyTo(lastExp, 0);
            monitor.Log($"Resetted experience {newExp}. Resuming experience update listener.");
            this.running = true;
        }

        private void UpdateState()
        {
            var newExp = Game1.player.experiencePoints;
            UpdateTexts();
            if (this.running)
            {
                CheckForSkillUpdates(newExp);
            }
        }

        private void UpdateTexts()
        {
            foreach (var textToSkill in textsToSkill.ToList())
            {
                if (textToSkill.Value.update(Game1.currentGameTime))
                {
                    textsToSkill.Remove(textToSkill.Key);
                }
            }
        }

        private void CheckForSkillUpdates(Netcode.NetArray<int, Netcode.NetInt> newExp)
        {
            for (int skillIndex = 0; skillIndex < newExp.Length; skillIndex++)
            {
                int diff = newExp[skillIndex] - lastExp[skillIndex];
                if (diff != 0)
                {
                    HandleSkillGain(skillIndex, diff, newExp[skillIndex]);
                    lastExp[skillIndex] = newExp[skillIndex];
                }
            }
        }

        private void HandleSkillGain(int skill, int gained, int total)
        {
            LogGainedExp(skill, gained, total);
            AddTextToRender(skill, gained);
        }

        private void LogGainedExp(int skill, int gained, int total)
        {
            int leftTillNext = 0;
            foreach (int neededExp in expNeededForLevel)
            {
                if (total < neededExp)
                {
                    leftTillNext = neededExp - total;
                    break;
                }
            }
            var nextLevelText = leftTillNext > 0 ? $"{leftTillNext} more for next level" : "max level";
            monitor.Log($"Gained +{gained} for {(ExperienceType)skill} ({nextLevelText})", LogLevel.Debug);
        }

        private void AddTextToRender(int skill, int gained)
        {
            if (Game1.activeClickableMenu != null) return;

            textsToSkill.Remove(skill);
            textsToSkill.Add(skill, new SparklingText(Game1.smallFont, "+" + gained,
                SkillColorHelper.GetSkillColor(skill), SkillColorHelper.GetSkillColor(skill), millisecondsDuration: ModEntry.Config.TextDurationMS));
        }
    }
}
