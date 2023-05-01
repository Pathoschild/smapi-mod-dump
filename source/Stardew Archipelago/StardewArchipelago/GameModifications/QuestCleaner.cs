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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Quests;

namespace StardewArchipelago.GameModifications
{
    public class QuestCleaner
    {
        private const int INITIATION_ID_SECOND_PART = 16;
        private const int RAT_PROBLEM_ID = 26;
        private const int MEET_THE_WIZARD_ID = 1;
        private const int RAISING_ANIMALS_ID = 7;

        public QuestCleaner()
        {

        }

        public void CleanQuests(Farmer player)
        {
            foreach (var quest in player.questLog)
            {
                CleanInitiation(quest);
                CleanRatProblem(player, quest);
                CleanRaisingAnimals(quest);
            }
        }

        private void CleanInitiation(Quest quest)
        {
            if (quest.id.Value != INITIATION_ID_SECOND_PART)
            {
                return;
            }

            quest.questComplete();
        }

        private void CleanRatProblem(Farmer player, Quest quest)
        {
            if (quest.id.Value != RAT_PROBLEM_ID)
            {
                return;
            }

            if (player.hasQuest(MEET_THE_WIZARD_ID) || player.hasOrWillReceiveMail("canReadJunimoText"))
            {
                quest.questComplete();
            }
        }

        private void CleanRaisingAnimals(Quest quest)
        {
            if (quest.id.Value != RAISING_ANIMALS_ID)
            {
                return;
            }

            var farm = Game1.getFarm();
            var hasAnyCoop = farm.isBuildingConstructed("Coop") || farm.isBuildingConstructed("Big Coop") || farm.isBuildingConstructed("Deluxe Coop");

            if (!hasAnyCoop)
            {
                return;
            }

            quest.questComplete();
        }
    }
}
