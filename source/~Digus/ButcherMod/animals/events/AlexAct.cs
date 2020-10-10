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
using StardewValley;
using static AnimalHusbandryMod.common.DataLoader;

namespace AnimalHusbandryMod.animals.events
{
    public class AlexAct: AbstractAct
    {
        public override string NpcName => "Alex";
        public override int? RequiredEvent => 2481135;

        public override string GetAct(AnimalContestItem animalContestInfo, List<AnimalContestItem> history)
        {
            return GetAct(animalContestInfo, !history.Exists(h => h.Contenders.Contains(NpcName)));
        }

        private string GetAct(AnimalContestItem animalContestInfo, bool isFirstTime)
        {
            StringBuilder alexAct = new StringBuilder();
            if (isFirstTime)
            {
                alexAct.Append($"/emote Lewis 8/speak Lewis \"{GetDialog("Lewis.FirstTime1")}\"");
                alexAct.Append($"/speak Alex \"{GetDialog("Alex.FirstTime1")}\"");
                alexAct.Append($"/speak Lewis \"{GetDialog("Lewis.FirstTime2")}\"");
                alexAct.Append($"/speak Alex \"{GetDialog("Alex.FirstTime2")}\"/playSound dogWhining/pause 1000/specificTemporarySprite animalContestJoshDogOut/pause 1000");
                alexAct.Append($"/speak Lewis \"{GetDialog("Lewis.FirstTime3")}\"");
                alexAct.Append($"/speak Alex \"{GetDialog("Alex.FirstTime3")}\"");
                alexAct.Append($"/speak Lewis \"{GetDialog("Lewis.FirstTime4")}\"");
                alexAct.Append($"/speak Alex \"{GetDialog("Alex.FirstTime4")}\"");
                alexAct.Append($"/speak Lewis \"{GetDialog("Lewis.FirstTime5")}\"");
                alexAct.Append("/emote Alex 12");
            }
            else
            {
                alexAct.Append($"/speak Lewis \"{GetDialog("Lewis.OtherTimes1")}\"");
                alexAct.Append($"/speak Alex \"{GetDialog("Alex.OtherTimes1")}\"/pause 500/specificTemporarySprite animalContestJoshDogOut/pause 1000");
                alexAct.Append($"/speak Lewis \"{GetDialog("Lewis.OtherTimes2")}\"");
                alexAct.Append($"/speak Alex \"{GetDialog("Alex.OtherTimes2")}\"/pause 300/showFrame Alex 26/pause 100/specificTemporarySprite animalContestJoshDogSteak/playSound dwop");
                alexAct.Append($"/pause 1000/speak Lewis \"{GetDialog("Lewis.OtherTimes3")}\"");
                alexAct.Append($"/speak Alex \"{GetDialog("Alex.OtherTimes3")}\"/playSound dogWhining");
                alexAct.Append($"/speak Lewis \"{GetDialog("Lewis.OtherTimes4")}\"");
                alexAct.Append("/pause 300/specificTemporarySprite animalContestJoshDogOut/pause 100/showFrame Alex 4/emote Alex 12");
            }
            return alexAct.ToString();
        }
    }
}
