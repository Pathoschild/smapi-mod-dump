/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MattDeDuck/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace Fischievements
{
    public class ModData
    {
        public int AchieveIDMF { get; set; }
        public int AchieveIDLF { get; set; }
        public int AchieveIDLA { get; set; }

        public bool CheckAchieveMF { get; set; }
        public bool CheckAchieveLF { get; set; }
        public bool CheckAchieveLA { get; set; }

        public bool PlayerGotMF { get; set; }
        public bool PlayerGotLF { get; set; }
        public bool PlayerGotLA { get; set; }

        public ModData()
        {
            AchieveIDMF = 35;
            AchieveIDLF = 36;
            AchieveIDLA = 37;

            CheckAchieveMF = Game1.achievements.ContainsKey(AchieveIDMF);
            CheckAchieveLF = Game1.achievements.ContainsKey(AchieveIDLF);
            CheckAchieveLA = Game1.achievements.ContainsKey(AchieveIDLA);

            PlayerGotMF = Game1.player.achievements.Contains(35);
            PlayerGotLF = Game1.player.achievements.Contains(36);
            PlayerGotLA = Game1.player.achievements.Contains(37);
        }
    }
}
