/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using PyTK.ConsoleCommands;
using PyTK.Types;
using StardewValley;
using System.Threading.Tasks;

namespace HarpOfYobaRedux
{
    class TimeMagic : IMagic
    {
        public TimeMagic()
        {

        }

        public void doMagic(bool playedToday)
        {
            Game1.player.forceTimePass = true;
            Game1.playSound("stardrop");
            STime time = STime.CURRENT + (STime.HOUR * 3);
            int timeInt = (time.hour * 100 + time.minute * 10);
            if (timeInt > 2600)
                timeInt = 2600;

            if (Game1.timeOfDay < 2600) 
                Task.Run(() => {
                    try
                    {
                        CcTime.TimeSkip(timeInt.ToString(), false);
                        }
                    catch { }

                    });
        }
    }
}
