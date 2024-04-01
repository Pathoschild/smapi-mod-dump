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
    public class EmilyAct : AbstractAct
    {

        public override string NpcName => "Emily";
        public override string RequiredEvent => "463391";
        public override string GetAct(AnimalContestItem animalContestInfo, List<AnimalContestItem> history)
        {
            return GetAct(!history.Exists(h => h.Contenders.Contains(NpcName)));
        }

        private string GetAct(bool isFirstTime)
        {
            StringBuilder emilyAct = new StringBuilder();
            if (isFirstTime)
            {
                emilyAct.Append($"/jump Lewis/emote Lewis 16/speak Lewis \"{GetDialog("Lewis.FirstTime1")}\"");
                emilyAct.Append($"/speak Emily \"{GetDialog("Emily.FirstTime1")}\"");
                emilyAct.Append($"/speak Lewis \"{GetDialog("Lewis.FirstTime2")}\"");
                emilyAct.Append($"/speak Emily \"{GetDialog("Emily.FirstTime2")}\"");
                emilyAct.Append($"/speak Lewis \"{GetDialog("Lewis.FirstTime3")}\"");
                emilyAct.Append($"/speak Emily \"{GetDialog("Emily.FirstTime3")}\"");
                emilyAct.Append($"/emote Lewis 8/speak Lewis \"{GetDialog("Lewis.FirstTime4")}\"");
                emilyAct.Append("/pause 500/specificTemporarySprite animalContestEmilyParrotAction");
                emilyAct.Append($"/pause 1000/speak Lewis \"{GetDialog("Lewis.FirstTime5")}\"");
                emilyAct.Append("/pause 1500/specificTemporarySprite animalContestEmilyParrotAction");
                emilyAct.Append($"/pause 1500/speak Emily \"{GetDialog("Emily.FirstTime4")}\"");
                emilyAct.Append($"/speak Lewis \"{GetDialog("Lewis.FirstTime6")}\"");
            }
            else
            {
                emilyAct.Append($"/speak Lewis \"{GetDialog("Lewis.OtherTimes1")}\"");
                emilyAct.Append($"/speak Emily \"{GetDialog("Emily.OtherTimes1")}\"");
                emilyAct.Append($"/speak Lewis \"{GetDialog("Lewis.OtherTimes2")}\"");
                emilyAct.Append("/specificTemporarySprite animalContestEmilyParrotAction");
                emilyAct.Append($"/speak Emily \"{GetDialog("Emily.OtherTimes2")}\"");
                emilyAct.Append("/faceDirection Emily 0/specificTemporarySprite animalContestEmilyBoomBox/playMusic EmilyDance/pause 4460/faceDirection Emily 1 true");
                emilyAct.Append("/specificTemporarySprite animalContestEmilyBoomBoxStart/specificTemporarySprite animalContestEmilyParrotDance");
                emilyAct.Append("/pause 10000/faceDirection Emily 0 true/pause 500/specificTemporarySprite animalContestEmilyParrotStopDance");
                emilyAct.Append("/stopMusic/specificTemporarySprite animalContestEmilyBoomBoxStop/pause 500/faceDirection Emily 1/pause 500");
                emilyAct.Append($"/speak Lewis \"{GetDialog("Lewis.OtherTimes3")}\"");
                emilyAct.Append($"/speak Emily \"{GetDialog("Emily.OtherTimes3")}\"");
                emilyAct.Append($"/pause 1000/speak Lewis \"{GetDialog("Lewis.OtherTimes4")}\"");
            }
            return emilyAct.ToString();
        }

    }
}
