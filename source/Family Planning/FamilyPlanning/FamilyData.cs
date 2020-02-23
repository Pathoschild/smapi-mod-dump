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