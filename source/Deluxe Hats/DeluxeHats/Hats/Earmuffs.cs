/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/domsim1/stardew-valley-deluxe-hats-mod
**
*************************************************/

using StardewValley;
using System;
using System.Linq;

namespace DeluxeHats.Hats
{
    public static class Earmuffs
    {
        public const string Name = "Earmuffs";
        public const string Description = "When outside in winter get the Season Protection Buff:\n+2 Foraging\n+1 Farming\n+1 Fishing";
        public static void Activate()
        {
            HatService.OnUpdateTicked = (e) =>
            {
                Buff earmuffBuff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(x => x.which == HatService.BuffId);
                if (Game1.currentLocation.isOutdoors && Game1.currentSeason == "winter")
                {
                    if (earmuffBuff == null)
                    {
                        earmuffBuff = new Buff(
                            farming: 1,
                            fishing: 1,
                            mining: 0,
                            digging: 0,
                            luck: 0,
                            foraging: 2,
                            crafting: 0,
                            maxStamina: 0,
                            magneticRadius: 0,
                            speed: 0,
                            defense: 0,
                            attack: 0,
                            minutesDuration: 1,
                            source: "Deluxe Hats",
                            displaySource: Name)
                        {
                            which = HatService.BuffId,
                        };
                        Game1.buffsDisplay.addOtherBuff(earmuffBuff);
                        earmuffBuff.description = "Season Protection\n+2 Foraging\n+1 Farming\n+1 Fishing";
                        earmuffBuff.millisecondsDuration = Convert.ToInt32((20f - ((Game1.timeOfDay - 600f) / 100f)) * 43000);
                    }
                }
                else
                {
                    if (earmuffBuff != null)
                    {
                        earmuffBuff.millisecondsDuration = 0;
                    }
                }
            };
        }

        public static void Disable()
        {
            Buff earmuffBuff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(x => x.which == HatService.BuffId);
            if (earmuffBuff != null)
            {
                earmuffBuff.millisecondsDuration = 0;
            }
        }
    }
}
