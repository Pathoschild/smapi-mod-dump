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
            if (!Game1.currentLocation.isOutdoors || Game1.isFestival())
                return false;
           
            if (BetterTrainLootMod.Instance.config.enableMod)
            { 
                if ((BetterTrainLootMod.Instance.config.showTrainIsComingMessage && Game1.player.currentLocation.Name != "Desert")||
                    (BetterTrainLootMod.Instance.config.showDesertTrainIsComingMessage && Game1.player.currentLocation.Name == "Desert") )
                {
                    Game1.showGlobalMessage(Game1.content.LoadString("Strings\\Locations:Railroad_TrainComing"));
                }

                if (Game1.soundBank == null)
                    return false;

                if ((BetterTrainLootMod.Instance.config.enableTrainWhistle && Game1.player.currentLocation.Name != "Desert") ||                    
                    (BetterTrainLootMod.Instance.config.enableDesertTrainWhistle && Game1.player.currentLocation.Name == "Desert"))
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
