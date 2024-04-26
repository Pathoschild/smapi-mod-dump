/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using StardewValley;

namespace HarpOfYobaRedux
{
    class BirthdayMagic : IMagic
    {
        public NPC lastBirthday;

        public BirthdayMagic()
        {

        }

        public void doMagic(bool playedToday)
        {
            GameLocation gl = Game1.currentLocation;

            foreach (NPC ch in gl.characters)

                if (ch.isBirthday())
                    if (lastBirthday == null || lastBirthday != ch)
                    {
                        Game1.player.changeFriendship(250, ch);
                        ch.doEmote(20, true);
                        lastBirthday = ch;
                    }
        }
    }
}
