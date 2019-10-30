using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnimalHusbandryMod.animals.data;
using AnimalHusbandryMod.common;
using StardewModdingAPI;

namespace AnimalHusbandryMod.animals.events
{
    public interface IAnimalContestAct
    {
        string GetAct(AnimalContestItem animalContestInfo, List<AnimalContestItem> history);
        string NpcName { get; }
        bool Available(List<AnimalContestItem> history);
    }
}
