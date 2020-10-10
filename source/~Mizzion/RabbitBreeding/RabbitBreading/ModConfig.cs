/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

namespace RabbitBreading
{
    internal class ModConfig
    {
        public bool GenderMatters { get; set; } = true;
        public bool AnimalHappinessMatters { get; set; } = true;
        public bool AnimalFriendshipMatters { get; set; } = true;

        public bool SeasonMatters { get; set; } = true;

        public bool AgeMatters { get; set; } = true;
        public bool CanHaveMultiples { get; set; } = true;
        public int MinBabiesToAllow { get; set; } = 2;

        public int MaxBabiesToAllow { get; set; } = 4;
        public float BaseChance { get; set; } = 42.0f;
        public bool ShowDebugInfo { get; set; } = false;
    }
}
