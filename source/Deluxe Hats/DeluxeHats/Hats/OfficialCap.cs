using StardewValley;
using System;
using System.Linq;

namespace DeluxeHats.Hats
{
    public static class OfficialCap
    {
        public const string Name = "Official Cap";
        public static void Activate()
        {
            HatService.OnUpdateTicked = (e) =>
            {
                Buff fishingBuff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(x => x.which == HatService.BuffId);
                if (!Game1.currentLocation.name.Contains("Beach"))
                {
                    if (fishingBuff != null)
                    {
                        fishingBuff.millisecondsDuration = 0;
                    }
                    return;
                }
                if (fishingBuff == null)
                {
                    fishingBuff = new Buff(
                        farming: 0,
                        fishing: 2,
                        mining: 0,
                        digging: 0,
                        luck: 0,
                        foraging: 0,
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
                    Game1.buffsDisplay.addOtherBuff(fishingBuff);
                    fishingBuff.description = "Ol' Mariner\n+2 Fishing";
                    fishingBuff.millisecondsDuration = Convert.ToInt32((20f - ((Game1.timeOfDay - 600f) / 100f)) * 43000);
                }
            };
        }

        public static void Disable()
        {
            Buff fishingBuff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(x => x.which == HatService.BuffId);
            if (fishingBuff != null)
            {
                fishingBuff.millisecondsDuration = 0;
            }
        }
    }
}
