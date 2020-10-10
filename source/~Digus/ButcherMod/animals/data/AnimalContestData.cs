/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimalHusbandryMod.animals.data
{
    public class AnimalContestData
    {
        public List<string> ContestDays;
        public int MinPointsToPossibleWin;
        public int MinPointsToGaranteeWin;
        public int FarmAnimalFriendshipForParticipating;
        public int PetFriendshipForParticipating;

        public AnimalContestData()
        {
            ContestDays = new List<string>() { "26 spring", "26 fall" };
            MinPointsToPossibleWin = 11;
            MinPointsToGaranteeWin = 14;
            FarmAnimalFriendshipForParticipating = 90;
            PetFriendshipForParticipating = 48;
        }
    }
}
