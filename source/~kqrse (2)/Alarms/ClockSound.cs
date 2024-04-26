/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods-aedenthorn
**
*************************************************/

using System.Collections.Generic;

namespace Alarms
{
    public class ClockSound
    {
        public string sound = ModEntry.Config.DefaultSound;
        public bool enabled;
        public string notification;
        public int hours = 6;
        public int minutes;
        public bool[] seasons = new bool[4] { true,true,true,true };
        public bool[] daysOfWeek;
        public bool[] daysOfMonth;
    }
}