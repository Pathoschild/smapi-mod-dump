using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnimalHusbandryMod.animals.data;
using StardewValley;

namespace AnimalHusbandryMod.animals.events
{
    public class MarnieAct : AbstractAct
    {
        public override string NpcName => "Marnie";

        public override string GetAct(AnimalContestItem animalContestInfo, List<AnimalContestItem> history)
        {
            return GetMarieAct(animalContestInfo.MarnieAnimal, history.Count == 0);
        }

        private string GetMarieAct(string marnieAnimal, bool isFirstTime)
        {
            StringBuilder marnieAct = new StringBuilder();

            string otherTimes = isFirstTime ? "" : GetDialog("Lewis.OtherTimes");
            marnieAct.Append($"/speak Lewis \"{GetDialog("Lewis.Begin", new { otherTimes })}\"/pause 200");

            if (marnieAnimal.Contains("Cow"))
            {
                marnieAct.Append($"/speak Lewis \"{GetDialog("Lewis.Cow")}\"");
                marnieAct.Append($"/speak Marnie \"{GetDialog("Marnie.Astonished")}\"");
                marnieAct.Append($"/speak Lewis \"{GetDialog("Lewis.ContinueEvaluation")}\"");
                marnieAct.Append($"/speak Caroline \"{GetDialog("Caroline")}\"");
            }
            else if (marnieAnimal.Contains("Chicken"))
            {
                marnieAct.Append($"/speak Lewis \"{GetDialog("Lewis.Chicken")}\"");
                marnieAct.Append($"/speak Marnie \"{GetDialog("Marnie.Astonished")}\"");
                marnieAct.Append($"/speak Pierre \"{GetDialog("Pierre.Chicken")}\"");
            }
            else if (marnieAnimal.Contains("Duck"))
            {
                marnieAct.Append($"/speak Lewis \"{GetDialog("Lewis.Duck")}\"");
                marnieAct.Append($"/speak Marnie \"{GetDialog("Marnie.Astonished")}\"");
                marnieAct.Append($"/speak Lewis \"{GetDialog("Lewis.ContinueEvaluation")}\"");
                marnieAct.Append($"/speak Jodi \"{GetDialog("Jodi.Duck")}\"");
            }
            else if (marnieAnimal.Contains("Pig"))
            {
                marnieAct.Append($"/speak Lewis \"{GetDialog("Lewis.Pig")}\"");
                marnieAct.Append($"/speak Marnie \"{GetDialog("Marnie.Astonished")}\"");
                marnieAct.Append($"/speak Lewis \"{GetDialog("Lewis.Astonished")}\"");
                marnieAct.Append($"/speak George \"{GetDialog("George.Pig")}\"");
            }
            else if (marnieAnimal.Contains("Goat"))
            {
                marnieAct.Append($"/speak Lewis \"{GetDialog("Lewis.Goat")}\"");
                marnieAct.Append($"/speak Marnie \"{GetDialog("Marnie.Astonished")}\"");
                marnieAct.Append($"/speak Lewis \"{GetDialog("Lewis.ContinueEvaluation")}\"");
                marnieAct.Append($"/speak Evelyn \"{GetDialog("Evelyn.Goat")}\"");
            }
            else if (marnieAnimal.Contains("Rabbit"))
            {
                marnieAct.Append($"/speak Lewis \"{GetDialog("Lewis.Rabbit")}\"");
                marnieAct.Append($"/speak Marnie \"{GetDialog("Marnie.Astonished")}\"");
                marnieAct.Append("/emote Lewis 40");
                marnieAct.Append($"/speak Jas \"{GetDialog("Jas.Rabbit")}\"");
            }
            else if (marnieAnimal.Contains("Sheep"))
            {
                marnieAct.Append($"/speak Lewis \"{GetDialog("Lewis.Sheep")}\"");
                marnieAct.Append($"/speak Marnie \"{GetDialog("Marnie.Astonished")}\"");
                if (Game1.player.mailReceived.Contains("secretNote21_done"))
                {
                    marnieAct.Append("/emote farmer 16");
                }
                else
                {
                    marnieAct.Append("/emote farmer 8");
                }
                marnieAct.Append($"/speak Jas \"{GetDialog("Jas.Sheep")}\"");
            }
            else
            {
                marnieAct.Append($"/speak Lewis \"{GetDialog("Lewis.Animal")}\"");
                marnieAct.Append($"/speak Marnie \"{GetDialog("Marnie.Astonished")}\"");
                marnieAct.Append($"/pause 500/speak Caroline \"{GetDialog("Caroline")}\"");
            }
            marnieAct.Append($"/speak Lewis \"{GetDialog("Lewis.Evalution")}\"");
            string closurePrefix = isFirstTime
                ? GetDialog("Lewis.ClosurePrefix.FirstTime")
                : GetDialog("Lewis.ClosurePrefix.OtherTimes");
            marnieAct.Append($"/pause 1500/speak Lewis \"{GetDialog("Lewis.Closure", new { closurePrefix })}\"");
            marnieAct.Append($"/speak Marnie \"{GetDialog("Marnie.Thanks")}\"");
            marnieAct.Append($"/speak Lewis \"{GetDialog("Lewis.Ending")}\"");

            return marnieAct.ToString();
        }

        public string ChooseMarnieAnimal(Random random, List<AnimalContestItem> history)
        {
            List<string> animalsPool = new List<string>(AnimalContestEventBuilder.MarnieJasPossibleAnimals);
            if (history.Count > 0)
            {
                List<Tuple<string, int>> animalCount = animalsPool.Select((a) => new Tuple<String, int>(a, history.Count(m => m.MarnieAnimal == a))).ToList();
                int minCount = animalCount.Min(t2 => t2.Item2);
                animalsPool = animalCount.Where(t1 => t1.Item2 == minCount).Select(t => t.Item1).ToList();
            }
            return animalsPool[random.Next(animalsPool.Count - 1)];
        }
    }
}
