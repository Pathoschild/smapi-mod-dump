using System;
using System.Linq;
using StardewValley;

namespace DeluxeHats.Hats
{
    public static class ChefHat
    {
        public const string Name = "Chef Hat";
        public static void Activate()
        {
            HatService.OnUpdateTicked = (e) =>
            {
                if (Game1.buffsDisplay.food == null)
                {
                    return;
                }
                Buff chefBuff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(x => x.which == HatService.BuffId);
                if (Game1.player.isEating)
                {
                    if (chefBuff != null) 
                    {
                        chefBuff.millisecondsDuration = 0;
                    }
                    return;
                }
                if (chefBuff == null)
                {
                    var foodAttributes = HatService.Helper.Reflection.GetField<int[]>(Game1.buffsDisplay.food, "buffAttributes").GetValue();
                    chefBuff = new Buff(
                        farming: foodAttributes[0],
                        fishing: foodAttributes[1],
                        mining: foodAttributes[2],
                        digging: foodAttributes[3],
                        luck: foodAttributes[4],
                        foraging: foodAttributes[5],
                        crafting: foodAttributes[6],
                        maxStamina: foodAttributes[7],
                        magneticRadius: foodAttributes[8],
                        speed: foodAttributes[9],
                        defense: foodAttributes[10],
                        attack: foodAttributes[11],
                        minutesDuration: 1,
                        source: "Deluxe Hats",
                        displaySource: Name)
                    {
                        which = 6284,
                    };
                    chefBuff.description = $"Head Chef\nx2 {Game1.buffsDisplay.food.displaySource}";
                    Game1.buffsDisplay.addOtherBuff(chefBuff);
                }
                chefBuff.millisecondsDuration = Game1.buffsDisplay.food.millisecondsDuration;
            };
        }

        public static void Disable()
        {
            Buff chefBuff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(x => x.which == HatService.BuffId);
            if (chefBuff != null)
            {
                chefBuff.millisecondsDuration = 0;
            }
        }
    }
}
