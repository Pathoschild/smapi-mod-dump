/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;

namespace HarpOfYobaRedux
{
    class BoosterMagic : IMagic
    {
        public BoosterMagic()
        {

        }

        public void doMagic(bool playedToday)
        {
            Buff buff = new Buff("22");
            buff.glow = Color.Orange;
            buff.description = "Adventure!";
            buff.millisecondsDuration = 15000 + Game1.random.Next(15000);

            if (!playedToday)
            {
                Game1.player.stamina = Game1.player.maxStamina.Value;
                Game1.player.health = Game1.player.maxHealth;
                buff.millisecondsDuration = 35000 + Game1.random.Next(30000);
            }

            if (!Game1.player.hasBuff("22"))
                Game1.player.applyBuff(buff);
        }
    }
}
