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
using StardewValley;

namespace AnimalHusbandryMod.animals.events
{
    public class JasAct : AbstractAct
    {
        public override string NpcName => "Jas";

        public override string GetAct(AnimalContestItem animalContestInfo, List<AnimalContestItem> history)
        {
            return GetAct(animalContestInfo, !history.Exists(h => h.Contenders.Contains(NpcName)));
        }

        private string GetAct(AnimalContestItem animalContestInfo, bool isFirstTime)
        {
            StringBuilder jasAct = new StringBuilder();
            string babyAnimalName = AnimalExtension.GetBabyAnimalNameByType(GetJasAnimal(animalContestInfo.MarnieAnimal));
            jasAct.Append("/emote Lewis 40");
            if (isFirstTime)
            {
                jasAct.Append($"/speak Lewis \"{GetDialog("Lewis.FirstTime1")}\"");
                jasAct.Append($"/speak Jas \"{GetDialog("Jas.FirstTime1")}\"");
                jasAct.Append($"/speak Lewis \"{GetDialog("Lewis.FirstTime2")}\"");
                jasAct.Append($"/speak Jas \"{GetDialog("Jas.FirstTime2")}\"");
                jasAct.Append($"/speak Lewis \"{GetDialog("Lewis.FirstTime3")}\"");
                jasAct.Append($"/speak Marnie \"{GetDialog("Marnie.FirstTime")}\"");
                jasAct.Append($"/speak Lewis \"{GetDialog("Lewis.FirstTime4", new { babyAnimalName })}\"");
                jasAct.Append($"/pause 1500/speak Lewis \"{GetDialog("Lewis.FirstTime5")}\"");
                jasAct.Append($"/emote Jas 20/speak Lewis \"{GetDialog("Lewis.FirstTime6")}\"");
            }
            else
            {
                jasAct.Append($"/speak Lewis \"{GetDialog("Lewis.OtherTimes1")}\"");
                jasAct.Append($"/speak Jas \"{GetDialog("Jas.OtherTimes1")}\"");
                jasAct.Append($"/speak Lewis \"{GetDialog("Lewis.OtherTimes2", new { babyAnimalName })}\"");
                jasAct.Append($"/pause 1500/speak Lewis \"{GetDialog("Lewis.OtherTimes3")}\"");
                jasAct.Append($"/emote Jas 20/speak Jas \"{GetDialog("Jas.OtherTimes2")}\"");
                jasAct.Append($"/jump Lewis/emote Lewis 16/speak Lewis \"{GetDialog("Lewis.OtherTimes4")}\"");
            }
            return jasAct.ToString();
        }

        public string GetJasAnimal(string marnieAnimal)
        {
            int i = Array.IndexOf(AnimalContestEventBuilder.MarnieJasPossibleAnimals, marnieAnimal);
            return AnimalContestEventBuilder.MarnieJasPossibleAnimals[((long)Game1.uniqueIDForThisGame + i) % AnimalContestEventBuilder.MarnieJasPossibleAnimals.Length];
        }
    }
}
