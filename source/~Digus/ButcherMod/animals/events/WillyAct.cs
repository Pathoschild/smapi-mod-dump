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
    public class WillyAct : AbstractAct
    {
        public override string NpcName => "Willy";
        public override string RequiredEvent => "711130";

        public override string GetAct(AnimalContestItem animalContestInfo, List<AnimalContestItem> history)
        {
            return GetAct(animalContestInfo, !history.Exists(h => h.Contenders.Contains(NpcName)));
        }

        private string GetAct(AnimalContestItem animalContestInfo, bool isFirstTime)
        {
            StringBuilder willyAct = new StringBuilder();
            if (isFirstTime)
            {
                willyAct.Append($"/jump Lewis/emote Lewis 16/speak Lewis \"{GetDialog("Lewis.FirstTime1")}\"");
                willyAct.Append($"/speak Willy \"{GetDialog("Willy.FirstTime1")}\"");
                willyAct.Append($"/speak Lewis \"{GetDialog("Lewis.FirstTime2")}\"");
                willyAct.Append($"/pause 800/playSound eat/jump Lewis/textAboveHead Lewis \"{GetDialog("Lewis.FirstTime3")}\"");
                willyAct.Append($"/speak Lewis \"{GetDialog("Lewis.FirstTime4")}\"");
                willyAct.Append($"/speak Willy \"{GetDialog("Willy.FirstTime2")}\"");
                willyAct.Append($"/speak Lewis \"{GetDialog("Lewis.FirstTime5")}\"");
                willyAct.Append("/emote Willy 28 true");
            }
            else
            {
                willyAct.Append($"/emote Lewis 16/speak Lewis \"{GetDialog("Lewis.OtherTimes1")}\"");
                willyAct.Append($"/speak Willy \"{GetDialog("Willy.OtherTimes1")}\"");
                willyAct.Append($"/speak Lewis \"{GetDialog("Lewis.OtherTimes2")}\"");
                willyAct.Append($"/pause 800/playSound scissors/jump Lewis/textAboveHead Lewis \"{GetDialog("Lewis.OtherTimes3")}\"");
                willyAct.Append($"/pause 500/speak Lewis \"{GetDialog("Lewis.OtherTimes4")}\"");
                willyAct.Append("/emote Willy 28");
                willyAct.Append($"/speak Lewis \"{GetDialog("Lewis.OtherTimes5")}\"");
            }
            return willyAct.ToString();
        }
    }
}
