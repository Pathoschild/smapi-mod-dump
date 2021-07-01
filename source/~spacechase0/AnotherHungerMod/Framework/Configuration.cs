/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

namespace AnotherHungerMod.Framework
{
    internal class Configuration
    {
        public int FullnessUiX = 10;
        public int FullnessUiY = 350;

        public int MaxFullness = 100;
        public float EdibilityMultiplier = 1;
        public float DrainPer10Min = 0.8f;

        public int PositiveBuffThreshold = 80;
        public int NegativeBuffThreshold = 25;

        public int StarvationDamagePer10Min = 10;

        public int RelationshipHitForNotFeedingSpouse = 50;
    }
}
