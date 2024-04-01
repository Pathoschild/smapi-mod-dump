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

namespace AnimalHusbandryMod.animals.events
{
    public class ShaneAct : AbstractAct
    {
        public override string NpcName => "Shane";
        public override string RequiredEvent => "3900074";

        public override string GetAct(AnimalContestItem animalContestInfo, List<AnimalContestItem> history)
        {
            return GetAct(!history.Exists(h => h.Contenders.Contains(NpcName)));
        }

        private string GetAct(bool isFirstTime)
        {
            StringBuilder shaneAct = new StringBuilder();
            if (isFirstTime)
            {
                shaneAct.Append($"/speak Lewis \"{GetDialog("Lewis.FirstTime1")}\"");
                shaneAct.Append($"/speak Shane \"{GetDialog("Shane.FirstTime1")}\"");
                shaneAct.Append($"/speak Lewis \"{GetDialog("Lewis.FirstTime2")}\"");
                shaneAct.Append($"/speak Shane \"{GetDialog("Shane.FirstTime2")}\"");
                shaneAct.Append($"/speak Lewis \"{GetDialog("Lewis.FirstTime3")}\"");
                shaneAct.Append($"/pause 1500/speak Lewis \"{GetDialog("Lewis.FirstTime4")}\"");
                shaneAct.Append($"/speak Shane \"{GetDialog("Shane.FirstTime3")}\"");
                shaneAct.Append($"/speak Lewis \"{GetDialog("Lewis.FirstTime5")}\"");
            }
            else
            {
                shaneAct.Append($"/speak Lewis \"{GetDialog("Lewis.OtherTimes1")}\"");
                shaneAct.Append($"/speak Shane \"{GetDialog("Shane.OtherTimes1")}\"");
                shaneAct.Append($"/speak Lewis \"{GetDialog("Lewis.OtherTimes2")}\"");
                shaneAct.Append($"/speak Shane \"{GetDialog("Shane.OtherTimes2")}\"");
                shaneAct.Append($"/speak Lewis \"{GetDialog("Lewis.OtherTimes3")}\"");
                shaneAct.Append($"/pause 1500/speak Lewis \"{GetDialog("Lewis.OtherTimes4")}\"");
            }
            return shaneAct.ToString();
        }
    }
}
