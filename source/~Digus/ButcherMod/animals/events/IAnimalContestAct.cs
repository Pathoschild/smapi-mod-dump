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
