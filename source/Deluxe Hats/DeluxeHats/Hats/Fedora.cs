using StardewValley;
using System;
using System.Linq;

namespace DeluxeHats.Hats
{
    public static class Fedora
    {
        public const string Name = "Fedora";
        public const string Description = "When in the Mines or Skull Cavern gain the \"Fortune and glory, kid.\" Buff:\n+2 Luck";
        public static void Activate()
        {
            HatService.OnUpdateTicked = (e) =>
            {
                Buff luckBuff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(x => x.which == HatService.BuffId);
                if (!Game1.currentLocation.name.Contains("Mine"))
                {
                    if (luckBuff != null)
                    {
                        luckBuff.millisecondsDuration = 0;
                    }
                    return;
                }
                if (luckBuff == null)
                {
                    luckBuff = new Buff(
                        farming: 0,
                        fishing: 0,
                        mining: 0,
                        digging: 0,
                        luck: 2,
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
                    Game1.buffsDisplay.addOtherBuff(luckBuff);
                    luckBuff.description = "\"Fortune and glory, kid.\"\n+2 Luck";
                    luckBuff.millisecondsDuration = Convert.ToInt32((20f - ((Game1.timeOfDay - 600f) / 100f)) * 43000);
                }
            };
        }

        public static void Disable()
        {
            Buff luckBuff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(x => x.which == HatService.BuffId);
            if (luckBuff != null)
            {
                luckBuff.millisecondsDuration = 0;
            }
        }
    }
}
