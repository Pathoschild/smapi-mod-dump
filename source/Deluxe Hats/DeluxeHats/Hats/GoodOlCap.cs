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
    public static class GoodOlCap
    {
        public const string Name= "Good Ol' Cap";
        public const string Description = "When outside in spring, fall or summer get The Forager Buff:\n+2 Foraging";
        public static void Activate()
        {
            HatService.OnUpdateTicked = (e) =>
            {
                Buff goodOlCapBuff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(x => x.which == HatService.BuffId);
                if (Game1.currentLocation.isOutdoors && (Game1.currentSeason == "spring" || Game1.currentSeason == "fall" || Game1.currentSeason == "summer"))
                {
                    if (goodOlCapBuff == null)
                    {
                        goodOlCapBuff = new Buff(
                            farming: 0,
                            fishing: 0,
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
                        Game1.buffsDisplay.addOtherBuff(goodOlCapBuff);
                        goodOlCapBuff.description = "The Forager\n+2 Foraging";
                        goodOlCapBuff.millisecondsDuration = Convert.ToInt32((20f - ((Game1.timeOfDay - 600f) / 100f)) * 43000);
                    }
                }
                else
                {
                    if (goodOlCapBuff != null)
                    {
                        goodOlCapBuff.millisecondsDuration = 0;
                    }
                }
            };
        }

        public static void Disable()
        {
            Buff goodOlCapBuff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(x => x.which == HatService.BuffId);
            if (goodOlCapBuff != null)
            {
                goodOlCapBuff.millisecondsDuration = 0;
            }
        }
    }
}
