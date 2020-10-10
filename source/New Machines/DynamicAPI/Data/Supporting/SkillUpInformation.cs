/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

using System.Linq;
using Newtonsoft.Json;

namespace Igorious.StardewValley.DynamicAPI.Data.Supporting
{
    public sealed class SkillUpInformation
    {
        #region	Properties

        [JsonProperty]
        public int Farming { get; set; }

        [JsonProperty]
        public int Fishing { get; set; }

        [JsonProperty]
        public int Mining { get; set; }

        [JsonIgnore]
        private int Combat { get; set; }

        [JsonProperty]
        public int Luck { get; set; }

        [JsonProperty]
        public int Foraging { get; set; }

        [JsonIgnore]
        private int Crafting { get; set; }

        [JsonProperty]
        public int MaxEnergy { get; set; }

        [JsonProperty]
        public int Magnetism { get; set; }

        [JsonProperty]
        public int Speed { get; set; }

        [JsonProperty]
        public int Defence { get; set; }

        #endregion

        #region Serialization

        public static SkillUpInformation Parse(string skillUpInformation)
        {
            var parts = skillUpInformation.Split(' ').Select(int.Parse).ToList();
            return new SkillUpInformation
            {
                Farming = parts[0],
                Fishing = parts[1],
                Mining = parts[2],
                Combat = parts[3],
                Luck = parts[4],
                Foraging = parts[5],
                Crafting = parts[6],
                MaxEnergy = parts[7],
                Magnetism = parts[8],
                Speed = parts[9],
                Defence = parts[10],
            };
        }

        public override string ToString()
        {
            return string.Join(" ", new[]
            {
                Farming,
                Fishing,
                Mining,
                Combat,
                Luck,
                Foraging,
                Crafting,
                MaxEnergy,
                Magnetism,
                Speed,
                Defence,
            });
        }

        #endregion
    }
}