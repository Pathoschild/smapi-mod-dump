/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/loe2run/FamilyPlanningMod
**
*************************************************/

using StardewValley;

namespace FamilyPlanning
{
    class FamilyData
    {
        public int TotalChildren { get; set; }
        public int BabyQuestionChance { get; set; }

        public FamilyData()
        {
            if(Game1.player.getChildrenCount() > 2)
                TotalChildren = Game1.player.getChildrenCount();
            else
                TotalChildren = 2;

            BabyQuestionChance = 5;
        }
    }
}