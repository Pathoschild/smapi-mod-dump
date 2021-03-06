/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ophaneom/Stardew-Valley-Mods
**
*************************************************/

using System.Linq;
using StardewModdingAPI;
using StardewValley;
using SpaceCore;

namespace AbilitiesExperienceBars
{
    public static class CompatibilityController
    {
        private static int[] expPerLevel, magicExpPerLevel, cookingExpPerLevel, loveCookingExpPerLevel;
        public static int GetActualAbility(int index, IModHelper helper, ModEntry instance)
        {
            int actualAbility = 0;

            if (helper.ModRegistry.IsLoaded("spacechase0.LuckSkill") && !instance.luckCheck)
            {
                actualAbility = 5;
                instance.luckCheck = true;
                return actualAbility;
            }
            else if (helper.ModRegistry.IsLoaded("spacechase0.CookingSkill") && !instance.cookingCheck)
            {
                actualAbility = 6;
                instance.cookingCheck = true;
                return actualAbility;
            }
            else if (helper.ModRegistry.IsLoaded("blueberry.LoveOfCooking") && !instance.loveCookingCheck)
            {
                actualAbility = 7;
                instance.loveCookingCheck = true;
                return actualAbility;
            }
            else if (helper.ModRegistry.IsLoaded("spacechase0.Magic") && !instance.magicCheck)
            {
                actualAbility = 8;
                instance.magicCheck = true;
                return actualAbility;
            }

            int sendAbilityIndex = actualAbility;
            return sendAbilityIndex;
        }
        public static int[] GetModExp(IModHelper helper, int index)
        {
            loadExpPerLevel(helper);


            int[] send = new int[3];

            if (index == 5)
            {
                int[] playerExperience = Game1.player.experiencePoints.ToArray();
                int luckLevel = Game1.player.luckLevel;

                send[0] = playerExperience[5];
                send[2] = luckLevel;
                if (send[2] < 10)
                {
                    send[1] = expPerLevel[luckLevel];
                }
                else
                {
                    send[1] = 10;
                }
                
            }
            else if (index == 6)
            {
                send[0] = Game1.player.GetCustomSkillExperience(Skills.GetSkill("cooking"));
                send[2] = Game1.player.GetCustomSkillLevel(Skills.GetSkill("cooking"));
                if (send[2] < 10)
                {
                    send[1] = cookingExpPerLevel[send[2]];
                }
                else
                {
                    send[1] = 10;
                }
            }
            else if (index == 7)
            {
                send[0] = Game1.player.GetCustomSkillExperience(Skills.GetSkill("blueberry.LoveOfCooking.CookingSkill"));
                send[2] = Game1.player.GetCustomSkillLevel(Skills.GetSkill("blueberry.LoveOfCooking.CookingSkill"));
                if (send[2] < 10)
                {
                    send[1] = loveCookingExpPerLevel[send[2]];
                }
                else
                {
                    send[1] = 10;
                }
            }
            else if (index == 8)
            {
                send[0] = Game1.player.GetCustomSkillExperience(Skills.GetSkill("magic"));
                send[2] = Game1.player.GetCustomSkillLevel(Skills.GetSkill("magic"));
                if (send[2] < 10)
                {
                    send[1] = magicExpPerLevel[send[2]];
                }
                else
                {
                    send[1] = 10;
                }
            }

            int[] returnValue = send;
            return returnValue;
        }
        private static void loadExpPerLevel(IModHelper helper)
        {
            expPerLevel = new int[] { 100, 380, 770, 1300, 2150, 3300, 4800, 6900, 10000, 15000 };

            if (helper.ModRegistry.IsLoaded("spacechase0.Magic"))
            {
                magicExpPerLevel = Skills.GetSkill("magic").ExperienceCurve;
            }
            if (helper.ModRegistry.IsLoaded("spacechase0.CookingSkill"))
            {
                cookingExpPerLevel = Skills.GetSkill("cooking").ExperienceCurve;
            }
            if (helper.ModRegistry.IsLoaded("blueberry.LoveOfCooking"))
            {
                loveCookingExpPerLevel = Skills.GetSkill("blueberry.LoveOfCooking.CookingSkill").ExperienceCurve;
            }
        }
    }
}
