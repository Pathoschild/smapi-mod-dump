using System;
using System.Linq;
using StardewValley;

namespace DeluxeHats.Hats
{
    public static class CatEars
    {
        public const string Name = "Cat Ears";
        public static int PlayerOldHP = 0;
        public static void Activate()
        {
            HatService.OnUpdateTicked = (e) =>
            {
                if (PlayerOldHP > Game1.player.health)
                {
                    Game1.playSound("cat");
                    Buff catBuff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(x => x.which == HatService.BuffId);
                    if (catBuff == null)
                    {
                        catBuff = new Buff(
                        farming: 0,
                        fishing: 0,
                        mining: 0,
                        digging: 0,
                        luck: 0,
                        foraging: 0,
                        crafting: 0,
                        maxStamina: 0,
                        magneticRadius: 0,
                        speed: 3,
                        defense: 0,
                        attack: 0,
                        minutesDuration: 1,
                        source: "Deluxe Hats",
                        displaySource: Name)
                        {
                            which = 6284,
                        };
                        catBuff.description = "Skittish Kitty\n+3 Speed";
                        Game1.buffsDisplay.addOtherBuff(catBuff);
                    }
                    catBuff.millisecondsDuration = 1500;
                }
                PlayerOldHP = Game1.player.health;
            };
        }

        public static void Disable()
        {
            PlayerOldHP = 0;
            Buff catBuff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(x => x.which == HatService.BuffId);
            if (catBuff != null)
            {
                catBuff.millisecondsDuration = 0;
            }
        }
    }
}
