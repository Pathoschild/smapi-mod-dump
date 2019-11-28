using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace MapUtilities.Contact
{
    public static class PainTileHandler
    {
        public static Dictionary<Farmer, int> cooldown;

        public static void init()
        {
            cooldown = new Dictionary<Farmer, int>();
        }

        public static void tickUpdate()
        {

            Dictionary<Farmer,int>.KeyCollection cooldowns = cooldown.Keys;
            List<Farmer> players = new List<Farmer>();

            foreach(Farmer player in cooldowns)
            {
                players.Add(player);
            }

            foreach(Farmer player in players)
            {
                cooldown[player] -= 1;
                if (cooldown[player] < 1)
                    cooldown.Remove(player);
            }
        }

        public static bool damagePlayer(Farmer who, int damage, int coolTimer)
        {
            if (!cooldown.ContainsKey(who))
            {
                cooldown[who] = coolTimer;
                who.takeDamage(damage, true, null);
                return true;
            }
            return false;
        }
    }
}
