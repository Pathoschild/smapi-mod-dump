/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Collections.Generic;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Extensions;
using StardewArchipelago.Items.Mail;
using StardewArchipelago.Locations.CodeInjections.Vanilla;
using StardewValley;

namespace StardewArchipelago.Items.Unlocks.Modded
{
    public class ModSkillUnlockManager : IUnlockManager
    {
        public ModSkillUnlockManager()
        {
        }

        public void RegisterUnlocks(IDictionary<string, Func<ReceivedItem, LetterAttachment>> unlocks)
        {
            unlocks.Add($"Magic Level", SendProgressiveMagicLevel);
            unlocks.Add($"Binning Level", SendProgressiveBinningLevel);
            unlocks.Add($"Cooking Level", SendProgressiveCookingLevel);
            unlocks.Add($"Luck Level", SendProgressiveLuckLevel);
            unlocks.Add($"Archaeology Level", SendProgressiveArchaeologyLevel);
            unlocks.Add($"Socializing Level", SendProgressiveSocializingLevel);
        }

        /*public LetterActionAttachment SendExcalibur(ReceivedItem receivedItem)
        {
            var excaliburType = AccessTools.TypeByName("DeepWoodsMod.Excalibur");
            // Time to do research!
            return new LetterActionAttachment(receivedItem, Excalibur);
        }*/

        private LetterAttachment SendProgressiveLuckLevel(ReceivedItem receivedItem)
        {
            foreach (var farmer in Game1.getAllFarmers())
            {
                var experienceForLevelUp = farmer.GetExperienceToNextLevel(Skill.Luck);
                farmer.AddExperience(Skill.Luck, experienceForLevelUp);
                farmer.luckLevel.Value++;
                farmer.newLevels.Add(new Point((int)Skill.Luck, farmer.luckLevel.Value));
            }
            return new LetterInformationAttachment(receivedItem);
        }

        private LetterAttachment SendProgressiveMagicLevel(ReceivedItem receivedItem)
        {
            ReceiveAPLevel("spacechase0.Magic");
            return new LetterInformationAttachment(receivedItem);
        }

        private LetterAttachment SendProgressiveBinningLevel(ReceivedItem receivedItem)
        {
            ReceiveAPLevel("drbirbdev.Binning");
            return new LetterInformationAttachment(receivedItem);
        }

        private LetterAttachment SendProgressiveCookingLevel(ReceivedItem receivedItem)
        {
            ReceiveAPLevel("spacechase0.Cooking");
            return new LetterInformationAttachment(receivedItem);
        }

        private LetterAttachment SendProgressiveSocializingLevel(ReceivedItem receivedItem)
        {
            ReceiveAPLevel("drbirbdev.Socializing");
            return new LetterInformationAttachment(receivedItem);
        }

        private LetterAttachment SendProgressiveArchaeologyLevel(ReceivedItem receivedItem)
        {
            ReceiveAPLevel("moonslime.Archaeology");
            return new LetterInformationAttachment(receivedItem);
        }

        private void ReceiveAPLevel(string skill)
        {
            var farmer = Game1.player;
            var skillsType = AccessTools.TypeByName("SpaceCore.Skills");
            var expField = ModEntry.Instance.Helper.Reflection.GetField<Dictionary<long, Dictionary<string, int>>>(skillsType, "Exp");
            var exp = expField.GetValue();
            var myNewLevelsField = ModEntry.Instance.Helper.Reflection.GetField<List<KeyValuePair<string, int>>>(skillsType, "NewLevels");
            var myNewLevels = myNewLevelsField.GetValue();
            if (!exp.ContainsKey(farmer.UniqueMultiplayerID))
            {
                exp.Add(farmer.UniqueMultiplayerID, new Dictionary<string, int>());
            }

            if (!exp[farmer.UniqueMultiplayerID].ContainsKey(skill))
            {
                exp[farmer.UniqueMultiplayerID].Add(skill, 0);
            }

            var modOldSkillExp = exp[farmer.UniqueMultiplayerID][skill];
            var modPrevSkillLevel = SkillInjections.GetLevel(modOldSkillExp);
            var expToNextLevel = farmer.GetExperienceToNextLevel(modOldSkillExp);
            exp[farmer.UniqueMultiplayerID][skill] += expToNextLevel;
            var modNewSkillExp = exp[farmer.UniqueMultiplayerID][skill];
            var modNewSkillLevel = SkillInjections.GetLevel(modNewSkillExp);
            if (modPrevSkillLevel != modNewSkillLevel)
            {
                for (var i = modPrevSkillLevel + 1; i <= modNewSkillLevel; ++i)
                {
                    myNewLevels.Add(new KeyValuePair<string, int>(skill, i));
                }
            }
        }
    }
}
