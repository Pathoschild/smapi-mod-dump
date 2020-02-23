using System.Collections.Generic;

namespace CustomGuildChallenges
{
    /// <summary>
    ///     Static monster names used in lookup of kill stats
    /// </summary>
    public static class Monsters
    {
        public static string GreenSlime { get; set; } = "Green Slime";
        public static string FrostJelly { get; set; } = "Frost Jelly";
        public static string Sludge { get; set; } = "Sludge";
        public static string BigSlime { get; set; } = "Big Slime";

        public static string Bug { get; set; } = "Bug";
        public static string Grub { get; set; } = "Grub";
        public static string Fly { get; set; } = "Fly";

        public static string Skeleton { get; set; } = "Skeleton";
        public static string SkeletonMage { get; set; } = "Skeleton Mage";
        public static string SkeletonWarrior { get; set; } = "Skeleton Warrior";
        
        public static string Bat { get; set; } = "Bat";
        public static string FrostBat { get; set; } = "Frost Bat";
        public static string LavaBat { get; set; } = "Lava Bat";
        public static string IridiumBat { get; set; } = "Iridium Bat";

        public static string RockCrab { get; set; } = "Rock Crab";
        public static string LavaCrab { get; set; } = "Lava Crab";
        public static string IridiumCrab { get; set; } = "Iridium Crab";

        public static string ShadowGuy { get; set; } = "Shadow Guy";
        public static string ShadowBrute { get; set; } = "Shadow Brute";
        public static string ShadowShaman { get; set; } = "Shadow Shaman";

        public static string Ghost { get; set; } = "Ghost";
        public static string CarbonGhost { get; set; } = "Carbon Ghost";

        public static string StoneGolem { get; set; } = "Stone Golem";
        public static string WildernessGolem { get; set; } = "Wilderness Golem";

        public static string Duggy { get; set; } = "Duggy";
        public static string DustSpirit { get; set; } = "Dust Spirit";

        public static string Mummy { get; set; } = "Mummy";
        public static string Serpent { get; set; } = "Serpent";
        public static string PepperRex { get; set; } = "Pepper Rex";

        public static string MetalHead { get; set; } = "Metal Head";
        public static string SquidKid { get; set; } = "Squid Kid";
        public static string Fireball { get; set; } = "Fireball";

        // In Monsters.xnb but not used (?)
        public static string Spiker { get; set; } = "Spiker";
        public static string Cat { get; set; } = "Cat";
        public static string Crow { get; set; } = "Crow";
        public static string Frog { get; set; } = "Frog";

        // Added by this mod
        public static string MutantGrub { get; set; } = "Mutant Grub";
        public static string MutantFly { get; set; } = "Mutant Fly";

        public static IList<string> MonsterList = new List<string>()
        {
            GreenSlime,
            FrostJelly,
            Sludge,
            BigSlime,
            Bug,
            Grub,
            Fly,
            Skeleton,
            SkeletonMage,
            SkeletonWarrior,
            Bat,
            FrostBat,
            LavaBat,
            IridiumBat,
            RockCrab,
            LavaCrab,
            IridiumCrab,
            ShadowGuy,
            ShadowBrute,
            ShadowShaman,
            Ghost,
            CarbonGhost,
            StoneGolem,
            WildernessGolem,
            Duggy,
            DustSpirit,
            Mummy,
            Serpent,
            MetalHead,
            SquidKid,
            Fireball,
            Spiker,
            Cat,
            MutantGrub,
            MutantFly,
            Crow,
            Frog,
            Cat,
            PepperRex,
        };
    }
}
