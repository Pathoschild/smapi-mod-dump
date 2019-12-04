using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTrainLoot.GamePatch
{
    class RailroadOverrider
    {
        public static bool prefix_playTrainApproach()
        {
            if (BetterTrainLootMod.Instance.config.showTrainIsComingMessage ||
                BetterTrainLootMod.Instance.config.enableTrainWhistle)
            {
                if (!Game1.currentLocation.isOutdoors || Game1.isFestival())
                    return false;

                if (BetterTrainLootMod.Instance.config.showTrainIsComingMessage)
                {
                    Game1.showGlobalMessage(Game1.content.LoadString("Strings\\Locations:Railroad_TrainComing"));
                }

                if (Game1.soundBank == null)
                    return false;

                if (BetterTrainLootMod.Instance.config.enableTrainWhistle)
                {
                    ICue cue = Game1.soundBank.GetCue("distantTrain");
                    cue.SetVariable("Volume", 100f);
                    cue.Play();
                }
                return false;
            }
            return true;            
        }
    }
}
