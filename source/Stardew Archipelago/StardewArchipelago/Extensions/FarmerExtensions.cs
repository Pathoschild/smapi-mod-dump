/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using StardewArchipelago.Locations.CodeInjections.Vanilla;
using StardewValley;

namespace StardewArchipelago.Extensions
{
    public static class FarmerExtensions
    {
        public static int AddExperience(this Farmer farmer, Skill skill, int amount)
        {
            return farmer.experiencePoints[(int)skill] += amount;
        }

        public static int GetExperienceToNextLevel(this Farmer farmer, Skill skill)
        {
            return farmer.GetExperienceToNextLevel(farmer.experiencePoints[(int)skill]);
        }

        public static int GetExperienceToNextLevel(this Farmer farmer, int currentExperience)
        {
            switch (currentExperience)
            {
                case < 100:
                    return 100 - currentExperience;
                case < 380:
                    return 380 - currentExperience;
                case < 770:
                    return 770 - currentExperience;
                case < 1300:
                    return 1300 - currentExperience;
                case < 2150:
                    return 2150 - currentExperience;
                case < 3300:
                    return 3300 - currentExperience;
                case < 4800:
                    return 4800 - currentExperience;
                case < 6900:
                    return 6900 - currentExperience;
                case < 10000:
                    return 10000 - currentExperience;
                case < 15000:
                    return 15000 - currentExperience;
            }

            return 0;
        }
    }
}
