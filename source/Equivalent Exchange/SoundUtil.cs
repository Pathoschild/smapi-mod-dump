using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquivalentExchange
{
    class SoundUtil
    {

        //play sound method wired up to handle configurable sound delay
        public static void PlaySound(string sound)
        {
            Game1.playSound(sound);            
        }

        //a nice magicky sound suggested by spacechase0
        public static void PlayMagickySound()
        {
            PlaySound("healSound");
        }

        //sound of cash moneeeeeeeh, git moneeeeeeeh I'm pickle rick
        public static void PlayMoneySound()
        {
            PlaySound("purchaseClick");
        }

        //de facto player-grunting-from-damage sound
        public static void PlayReboundSound()
        {
            PlaySound("ow");
        }
    }
}
