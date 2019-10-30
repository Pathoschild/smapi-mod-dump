using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnimalHusbandryMod.animals.data;
using StardewValley;

namespace AnimalHusbandryMod.animals.events
{
    public class JodiAct : AbstractAct
    {
        public override string NpcName => "Jodi";

        public override string GetAct(AnimalContestItem animalContestInfo, List<AnimalContestItem> history)
        {
            return GetAct(animalContestInfo, !history.Exists(h => h.Contenders.Contains(NpcName)));
        }

        private string GetAct(AnimalContestItem animalContestInfo, bool isFirstTime)
        {
            StringBuilder jodiAct = new StringBuilder();
            if (isFirstTime)
            {
                jodiAct.Append($"/speak Lewis \"{GetDialog("Lewis.FirstTime1")}\"");
                jodiAct.Append($"/speak Jodi \"{GetDialog("Jodi.FirstTime1")}\"");
                jodiAct.Append($"/speak Lewis \"{GetDialog("Lewis.FirstTime2")}\"");
                jodiAct.Append($"/speak Jodi \"{GetDialog("Jodi.FirstTime2")}\"");
                jodiAct.Append($"/speak Lewis \"{GetDialog("Lewis.FirstTime3")}\"");
                jodiAct.Append($"/pause 1000/speak Lewis \"{GetDialog("Lewis.FirstTime4")}\"");
            }
            else
            {
                jodiAct.Append($"/speak Lewis \"{GetDialog("Lewis.OtherTimes1")}\"");
                jodiAct.Append($"/speak Jodi \"{GetDialog("Jodi.OtherTimes1")}\"");
                if (Game1.player.isMarried() && "Sam".Equals(Game1.player.spouse))
                {
                    jodiAct.Append($"/speak Sam \"{GetDialog("Sam.Married")}\"");
                    string prefix = Game1.year > 1 
                        ? GetDialog("Jodi.OtherTimes2.PrefixYear2") 
                        : GetDialog("Jodi.OtherTimes2.PrefixYear1");
                    jodiAct.Append($"/speak Jodi \"{GetDialog("Jodi.OtherTimes2", new { prefix })}\"");
                }
                else
                {
                    jodiAct.Append($"/animate Sam false false 1500 33/textAboveHead Sam \"{GetDialog("Sam.Single")}\"/pause 2000");
                }
                jodiAct.Append($"/speak Lewis \"{GetDialog("Lewis.OtherTimes2")}\"");
                jodiAct.Append($"/pause 1500/speak Lewis \"{GetDialog("Lewis.OtherTimes3")}\"");

            }
            jodiAct.Append($"/speak Jodi \"{GetDialog("Jodi.Thanks")}\"");
            return jodiAct.ToString();
        }
    }
}
