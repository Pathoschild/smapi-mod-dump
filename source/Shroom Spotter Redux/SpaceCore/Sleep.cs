/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

namespace SpaceCore
{
    public class Sleep
    {
        public static bool SaveLocation { get; set; } = false;

        internal class Data
        {
            public string Location { get; set; }
            public float X { get; set; }
            public float Y { get; set; }

            public int Year { get; set; }
            public string Season { get; set; }
            public int Day { get; set; }

            public int MineLevel { get; set; }
        }
    }
}
