using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley;

namespace DeluxeHats.Hats
{
    public static class SailorsCap
    {
        public const string Name = "Sailor's Cap";
        public const string Description = "When tipsy gain the Drunken Sailor Buff:\n+10 Attack";
        public static void Activate()
        {
            HatService.OnUpdateTicked = (e) =>
            {
                Buff tipsyBuff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(x => x.which == Buff.tipsy);
                if (tipsyBuff != null)
                {
                    Buff powerBuff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(x => x.which == HatService.BuffId);
                    if (powerBuff == null)
                    {
                        powerBuff = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, attack: 10, minutesDuration: 1, source: "Deluxe Hats", displaySource: Name) { which = HatService.BuffId };
                        Game1.buffsDisplay.addOtherBuff(powerBuff);
                        Game1.player.startGlowing(Color.OrangeRed * 0.5f, false, 0.08f);
                        powerBuff.description = "Drunken Sailor\n+10 Attack";
                    }
                    powerBuff.millisecondsDuration = tipsyBuff.millisecondsDuration;
                }
                else if (Game1.player.isGlowing && Game1.player.glowingColor == Color.OrangeRed * 0.5f)
                {
                    Game1.player.stopGlowing();
                }
            };
        }

        public static void Disable()
        {
            Buff powerBuff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(x => x.which == HatService.BuffId);
            if (powerBuff != null)
            {
                powerBuff.millisecondsDuration = 0;
            }
        }
    }
}
