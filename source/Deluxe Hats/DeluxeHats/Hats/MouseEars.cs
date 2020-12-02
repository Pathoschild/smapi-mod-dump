/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/domsim1/stardew-valley-deluxe-hats-mod
**
*************************************************/

using System.Linq;
using StardewValley;

namespace DeluxeHats.Hats
{
    public static class MouseEars
    {
        public const string Name = "Mouse Ears";
        public const string Description = "When you are hit gain Skittish Mouse Buff:\n+4 Speed";
        public static int PlayerOldHP = 0;
        public static void Activate()
        {
            HatService.OnUpdateTicked = (e) =>
            {
                if (PlayerOldHP > Game1.player.health)
                {
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
                        speed: 4,
                        defense: 0,
                        attack: 0,
                        minutesDuration: 1,
                        source: "Deluxe Hats",
                        displaySource: Name)
                        {
                            which = HatService.BuffId,
                        };
                        catBuff.description = "Skittish Mouse\n+4 Speed";
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
