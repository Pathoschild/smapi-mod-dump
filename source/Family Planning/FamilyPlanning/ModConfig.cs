/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/loe2run/FamilyPlanningMod
**
*************************************************/

namespace FamilyPlanning
{
    class ModConfig
    {
        public bool AdoptChildrenWithRoommate { get; set; }
        public bool BabyQuestionMessages { get; set; }

        public ModConfig()
        {
            AdoptChildrenWithRoommate = false;
            BabyQuestionMessages = false;
        }
    }
}