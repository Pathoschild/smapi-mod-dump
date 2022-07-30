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
using System.Collections.Generic;

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
        public float VampiricEnchantment_RecoveryChance { get; set; } // 0f to 1f;
        public float PreservingEnchantment_PreserveChance { get; set; } // 0f to 1f;
        public int HunterEnchantment_ExtraDropsAmount { get; set; } 
        public int MinerEnchantment_ExtraDropsAmount { get; set; }          
        public int SwiftEnchantment_TimesFaster { get; set; }    
        public int[] MagneticEnchantment_AffectedStones { get; set; }
        public Dictionary<int,int> ItemsThatCanBeUsedAsAmmo { get; set; }
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
            MagneticEnchantment_AffectedStones = new int[]
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
                 850, //Iron ore (volcano version)
            };

            ItemsThatCanBeUsedAsAmmo = new Dictionary<int, int>()
            {
                //Default 
                { 388, 2 }, //Wood
                { 390, 5 }, //Stone
                { 382, 15 }, //Coal
                { 378, 10 }, //Copper ore               
                { 380, 20 }, //Iron ore
                { 384, 30 }, //Gold ore              
                { 386, 50 }, //Iridum ore
                { 441, 20 }, //Explosive ammo
                //New additions
                { 766, 5 }, //Slime             
                { 80, 10 }, //Quartz              
                { 82, 20 }, //Fire Quartz                
                { 84, 20 }, //Frozen Tear
                { 86, 20 }, //Earth Crystal
                { 768, 25 }, //Solar essence
                { 769, 25 }, //Void essence
                { 557, 20 }, //Petrified Slime                
                { 909, 75 }, //Radioactive ore
            };
        }
    }
}
