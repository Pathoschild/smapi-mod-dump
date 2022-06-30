/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/YTSC/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;

namespace EnhancedSlingshots
{
    class Config
    {
        public bool EnableGalaxySligshot { get; set; }
        public int GalaxySlingshotPrice { get; set; }
        public int InfinitySlingshotId { get; set; } 
        public float SlingshotAutoFireRate { get; set; }
        public float BugKillerEnchantment_Damage { get; set; }
        public float PreciseEnchantment_Damage { get; set; }
        public float VampiricEnchantment_RecoveryChance { get; set; }
        public float PreservingEnchantment_PreserveChance { get; set; } // 0f to 1f;
        public int HunterEnchantment_ExtraDropsAmount { get; set; } 
        public int MinerEnchantment_ExtraDropsAmount { get; set; }          
        public int SwiftEnchantment_TimesFaster { get; set; }    
        public int[] MagneticEnchantmentStones { get; set; }
        public int[] ItemsThatCanBeUsedAsAmmo { get; set; }
        public Config()
        {
            EnableGalaxySligshot = true;
            GalaxySlingshotPrice = 75000;
            InfinitySlingshotId = 135;
            SlingshotAutoFireRate = 0.5f;
            BugKillerEnchantment_Damage = 1.5f;
            PreciseEnchantment_Damage = 1.5f;
            HunterEnchantment_ExtraDropsAmount = 1;
            MinerEnchantment_ExtraDropsAmount = 1;
            PreservingEnchantment_PreserveChance = 0.5f;
            SwiftEnchantment_TimesFaster = 2;
            VampiricEnchantment_RecoveryChance = 0.09f;
            MagneticEnchantmentStones = new int[]
            {
                 2, //Diamond
                 4, //Ruby
                 6, //Jade
                 8, //Amethyst
                 10, //Topaz
                 12, //Emerald
                 14, //Aquamarine
                 44, //Gem Node
                 46, //Mystic Stone
                 75, //Geode
                 76, //Frozen Geode
                 77, //Magma Geode
                 95, //Radioactive ore
                 290, //Iron ore
                 751, //Copper ore
                 764, //Gold ore
                 765, //Iridium ore
                 819, //Omnigeode
                 843, //Cinder Shard
                 844, //Cinder Shard
                 849, //Copper ore (volcano version)                       
                 850 //Iron ore (volcano version)
            };

            ItemsThatCanBeUsedAsAmmo = new int[]
            {
                909 //Radioactive ore
            };
        }
    }
}
