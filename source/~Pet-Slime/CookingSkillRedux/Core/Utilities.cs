/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pet-Slime/StardewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using MoonShared;
using StardewValley;
using System.Linq;

namespace CookingSkill
{
    internal class Utilities
    {
        public static Item BetterCraftingTempItem;

        public static void AddEXP(StardewValley.Farmer who, int amount)
        {
            if (amount > 0)
            {
                SpaceCore.Skills.AddExperience(Game1.getFarmer(who.UniqueMultiplayerID), "moonslime.Cooking", amount);
            }
        }

        public static float GetLevelValue(StardewValley.Farmer who, bool additive = false)
        {
            var player = Game1.getFarmer(who.UniqueMultiplayerID);
            float level = SpaceCore.Skills.GetSkillLevel(player, "moonslime.Cooking") + SpaceCore.Skills.GetSkillBuffLevel(player, "moonslime.Cooking");
            if (additive)
            {
                float sendback = (level * 0.03f);
                return sendback;
            } else
            {
                float sendback = (level * 0.03f) + 1f;
                return sendback;
            }
        }


        public static int GetLevel(StardewValley.Farmer who)
        {
            var player = Game1.getFarmer(who.UniqueMultiplayerID);
            return SpaceCore.Skills.GetSkillLevel(player, "moonslime.Cooking") + SpaceCore.Skills.GetSkillBuffLevel(player, "moonslime.Cooking");
        }
    }
}
