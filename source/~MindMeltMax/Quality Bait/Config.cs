/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using Newtonsoft.Json;

namespace QualityBait
{
    internal class Config
    {
        public int ChancePercentage { get; set; } = 75;

        public bool BaitMakerQuality { get; set; } = true;

        [JsonIgnore]
        public double Chance => ChancePercentage / 100.0;
    }
}
