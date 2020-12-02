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
    public static class RedCowboyHat
    {
        public const string Name = "Red Cowboy Hat";
        public const string Description = "While riding the horse and for 30 seconds after dismounting gain the Buckeroo Buff:\n+2 Speed";
        public static void Activate()
        {
            HatService.OnUpdateTicked = (e) =>
            {
                Buff horseBuff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(x => x.which == HatService.BuffId);
                if (Game1.player.isRidingHorse())
                {
                    if (horseBuff == null)
                    {
                        horseBuff = new Buff(
                            farming: 0,
                            fishing: 0,
                            mining: 0,
                            digging: 0,
                            luck: 0,
                            foraging: 0,
                            crafting: 0,
                            maxStamina: 0,
                            magneticRadius: 0,
                            speed: 2,
                            defense: 0,
                            attack: 0,
                            minutesDuration: 1,
                            source: "Deluxe Hats",
                            displaySource: Name)
                        {
                            which = HatService.BuffId,
                        };
                        Game1.buffsDisplay.addOtherBuff(horseBuff);
                        horseBuff.description = "Buckeroo\n+2 Speed";
                        horseBuff.millisecondsDuration = Convert.ToInt32((20f - ((Game1.timeOfDay - 600f) / 100f)) * 43000);
                    }
                    if (horseBuff.millisecondsDuration < 30001)
                    {
                        horseBuff.millisecondsDuration = Convert.ToInt32((20f - ((Game1.timeOfDay - 600f) / 100f)) * 43000);
                    }
                }
                else
                {
                    if (horseBuff != null && horseBuff.millisecondsDuration > 30001)
                    {
                        horseBuff.millisecondsDuration = 30000;
                    }
                }
            };
        }

        public static void Disable()
        {
            Buff horseBuff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(x => x.which == HatService.BuffId);
            if (horseBuff != null)
            {
                horseBuff.millisecondsDuration = 0;
            }
        }
    }
}
