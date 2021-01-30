/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AairTheGreat/StardewValleyMods
**
*************************************************/

using StardewValley;
using StardewValley.Locations;
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
            bool showMessage = true;
            bool playSound = true;
            if (!Game1.currentLocation.isOutdoors || Game1.isFestival())
                return false;

            if (BetterTrainLootMod.Instance.config.enableMod)
            {
                if (!BetterTrainLootMod.Instance.config.showTrainIsComingMessage
                    || (Game1.player.currentLocation.Name == "Desert" && !BetterTrainLootMod.Instance.config.showDesertTrainIsComingMessage)
                    || (Game1.player.currentLocation is IslandLocation && !BetterTrainLootMod.Instance.config.showIslandTrainIsComingMessage)
                    )
                {
                    showMessage = false;
                }
                   
                if (showMessage)
                {
                    Game1.showGlobalMessage(Game1.content.LoadString("Strings\\Locations:Railroad_TrainComing"));
                }
                
                if (Game1.soundBank == null)
                    return false;

                if (!BetterTrainLootMod.Instance.config.enableTrainWhistle
                    || (Game1.player.currentLocation.Name == "Desert" && !BetterTrainLootMod.Instance.config.enableDesertTrainWhistle)
                    || (Game1.player.currentLocation is IslandLocation && !BetterTrainLootMod.Instance.config.enableIslandTrainWhistle)
                    )
                {
                    playSound = false;
                }
              
                if (playSound)
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
