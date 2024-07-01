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
using HarmonyLib;
using StardewArchipelago.Archipelago;
using StardewModdingAPI;

namespace StardewArchipelago.GameModifications.CodeInjections.Modded
{
    public class SocializingConfigPatcher
    {
        // namespace SocializingSkill;
        private const string SOCIALIZING_NAMESPACE = "SocializingSkill";
        private const double TALKING_MULTIPLIER = 1.8;
        private const double GIFT_MULTIPLIER = 1.6;
        private const double EVENT_MULTIPLIER = 1.6;
        private const double QUEST_MULTIPLIER = 1.6;
        private const double LOVED_GIFT_MULTIPLIER = 1.2;
        private const double BIRTHDAY_GIFT_MULTIPLIER = 1.2;

        private IMonitor _monitor;
        private IModHelper _modHelper;

        public SocializingConfigPatcher(IMonitor monitor, IModHelper modHelper)
        {
            _monitor = monitor;
            _modHelper = modHelper;
        }

        public void PatchConfigValues()
        {
            // public class ModEntry : Mod
            var socializingModEntryType = AccessTools.TypeByName($"{SOCIALIZING_NAMESPACE}.ModEntry");
            
            // internal class Config
            var socializingConfigType = AccessTools.TypeByName($"{SOCIALIZING_NAMESPACE}.Config");

            // internal static Config Config;
            var configField = AccessTools.Field(socializingModEntryType, "Config");
            var config = configField.GetValue(null);

            // public int ExperienceFromTalking = 5;
            ApplyIntMultiplier(config, "ExperienceFromTalking", TALKING_MULTIPLIER);

            // public int ExperienceFromGifts = 5;
            ApplyIntMultiplier(config, "ExperienceFromGifts", GIFT_MULTIPLIER);

            // public int ExperienceFromEvents = 20;
            ApplyIntMultiplier(config, "ExperienceFromEvents", EVENT_MULTIPLIER);

            // public int ExperienceFromQuests = 50;
            ApplyIntMultiplier(config, "ExperienceFromQuests", QUEST_MULTIPLIER);

            // public float LovedGiftExpMultiplier = 2;
            ApplyFloatMultiplier(config, "LovedGiftExpMultiplier", LOVED_GIFT_MULTIPLIER);

            // public float BirthdayGiftExpMultiplier = 5;
            ApplyFloatMultiplier(config, "BirthdayGiftExpMultiplier", BIRTHDAY_GIFT_MULTIPLIER);
        }

        private void ApplyIntMultiplier(object config, string multiplierFieldName, double multiplier)
        {
            var experienceField = _modHelper.Reflection.GetField<int>(config, multiplierFieldName);
            var originalExperience = experienceField.GetValue();
            experienceField.SetValue((int)(originalExperience * multiplier));
        }

        private void ApplyFloatMultiplier(object config, string multiplierFieldName, double multiplier)
        {
            var experienceField = _modHelper.Reflection.GetField<float>(config, multiplierFieldName);
            var originalExperience = experienceField.GetValue();
            experienceField.SetValue((float)(originalExperience * multiplier));
        }
    }
}
