using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnimalHusbandryMod.animals.data;
using StardewValley;

namespace AnimalHusbandryMod.animals.events
{
    public class AbigailAct : AbstractAct
    {
        public override string NpcName => "Abigail";
        public override int? RequiredEvent => 4;

        public override string GetAct(AnimalContestItem animalContestInfo, List<AnimalContestItem> history)
        {
            return GetAct(animalContestInfo, !history.Exists(h => h.Contenders.Contains(NpcName)));
        }

        private string GetAct(AnimalContestItem animalContestInfo, bool isFirstTime)
        {
            StringBuilder abigailAct = new StringBuilder();
            if (isFirstTime)
            {
                abigailAct.Append($"/jump Lewis/emote Lewis 16/speak Lewis \"{GetDialog("Lewis.FirstTime1")}\"");
                abigailAct.Append($"/speak Abigail \"{GetDialog("Abigail.FirstTime1")}\"");
                abigailAct.Append($"/emote Lewis 12/speak Lewis \"{GetDialog("Lewis.FirstTime2")}\"");
                abigailAct.Append($"/speak Abigail \"{GetDialog("Abigail.FirstTime2")}\"");
                abigailAct.Append($"/speak Lewis \"{GetDialog("Lewis.FirstTime3")}\"");
                abigailAct.Append($"/speak Abigail \"{GetDialog("Abigail.FirstTime3")}\"");
                abigailAct.Append($"/textAboveHead Pierre \"{GetDialog("Pierre.FirstTime")}\"/pause 500/emote Abigail 16");
                abigailAct.Append($"/speak Lewis \"{GetDialog("Lewis.FirstTime4")}\"");
            }
            else
            {
                abigailAct.Append($"/emote Lewis 16/speak Lewis \"{GetDialog("Lewis.OtherTimes1")}\"");
                abigailAct.Append($"/speak Abigail \"{GetDialog("Abigail.OtherTimes1")}\"");
                abigailAct.Append($"/speak Lewis \"{GetDialog("Lewis.OtherTimes2")}\"");
                abigailAct.Append($"/speak Abigail \"{GetDialog("Abigail.OtherTimes2")}\"");
                abigailAct.Append($"/emote farmer 16/speak Lewis \"{GetDialog("Lewis.OtherTimes3")}\"");
                abigailAct.Append($"/speak Abigail \"{GetDialog("Abigail.OtherTimes3")}\"");
                abigailAct.Append($"/speak Lewis \"{GetDialog("Lewis.OtherTimes4")}\"");
                abigailAct.Append("/emote Abigail 28");
            }
            return abigailAct.ToString();
        }

        public override bool Available(List<AnimalContestItem> history)
        {
            return base.Available(history) && (!history.Exists(i => i.Contenders.Contains(NpcName)) || Game1.player.mailReceived.Contains("slimeHutchBuilt"));
        }
    }
}
