/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using AnimalHusbandryMod.animals.data;
using System.Collections.Generic;
using AnimalHusbandryMod.animals;

namespace AnimalHusbandryMod.farmer
{
    public class FarmerData
    {
        public List<PregnancyItem> PregnancyData;
        public List<AnimalStatus> AnimalData;
        public List<AnimalContestItem> AnimalContestData;

        public FarmerData()
        {
            PregnancyData = new List<PregnancyItem>();
            AnimalData = new List<AnimalStatus>();
            AnimalContestData = new List<AnimalContestItem>();
        }
    }
}
