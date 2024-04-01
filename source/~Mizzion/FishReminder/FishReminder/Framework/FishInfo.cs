/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System.Dynamic;

namespace FishReminder.Framework
{
    internal class FishInfo
    {
        public string FishId { get; set; }

        public string FishName { get; set; }

        public string Difficulty { get; set; }

        public string DartingRandomness { get; set; }

        public string MinSize { get; set; }
        public string MaxSize { get; set; }

        public string Times { get; set; }
        public string Season { get; set; }

        public string Weather { get; set; }

        public string Locations { get; set; }

        public  string MaxDepth { get; set; }

        public string SpawnMultiplier { get; set; }

        public string DepthMultiplier { get; set; }

        public string FishingLevelNeeded { get; set; }

        public string IncludeInFirstCatchTutorial { get; set; }
    }
}
