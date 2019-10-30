using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnimalHusbandryMod.animals.data;

namespace AnimalHusbandryMod.animals.events
{
    public class MaruAct : AbstractAct
    {
        public override string NpcName => "Maru";

        public override string GetAct(AnimalContestItem animalContestInfo, List<AnimalContestItem> history)
        {
            return GetAct(animalContestInfo, !history.Exists(h => h.Contenders.Contains(NpcName)));
        }

        private string GetAct(AnimalContestItem animalContestInfo, bool isFirstTime)
        {
            StringBuilder maruAct = new StringBuilder();
            if (isFirstTime)
            {
                maruAct.Append($"/emote Lewis 8/speak Lewis \"{GetDialog("Lewis.FirstTime1")}\"");
                maruAct.Append($"/speak Maru \"{GetDialog("Maru.FirstTime1")}\"");
                maruAct.Append($"/speak Lewis \"{GetDialog("Lewis.FirstTime2")}\"");
                maruAct.Append($"/speak Maru \"{GetDialog("Maru.FirstTime2")}\"");
                maruAct.Append($"/speak Lewis \"{GetDialog("Lewis.FirstTime3")}\"");
                maruAct.Append($"/speak Maru \"{GetDialog("Maru.FirstTime3")}\"");
                maruAct.Append($"/speak Lewis \"{GetDialog("Lewis.FirstTime4")}\"");
                maruAct.Append("/emote Maru 28");
            }
            else
            {
                maruAct.Append($"/emote Lewis 16/speak Lewis \"{GetDialog("Lewis.OtherTimes1")}\"");
                maruAct.Append($"/speak Maru \"{GetDialog("Maru.OtherTimes1")}\"");
                maruAct.Append($"/speak Lewis \"{GetDialog("Lewis.OtherTimes2")}\"");
                maruAct.Append($"/speak Maru \"{GetDialog("Maru.OtherTimes2")}\"");
                maruAct.Append($"/pause 500/message \"{GetDialog("Robot.OtherTimes1")}\"");
                maruAct.Append($"/speak Lewis \"{GetDialog("Lewis.OtherTimes3")}\"");
                maruAct.Append($"/speak Maru \"{GetDialog("Maru.OtherTimes3")}\"");
                maruAct.Append($"/pause 500/message \"{GetDialog("Robot.OtherTimes2")}\"");
                maruAct.Append($"/emote Lewis 12/speak Maru \"{GetDialog("Maru.OtherTimes4")}\"");
                maruAct.Append($"/pause 500/message \"{GetDialog("Robot.OtherTimes3")}\"");
                maruAct.Append($"/emote Marnie 16/speak Lewis \"{GetDialog("Lewis.OtherTimes4")}\"");
                maruAct.Append("/emote Maru 28");
            }
            return maruAct.ToString();
        }
    }
}
