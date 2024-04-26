/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/namelessto/EnchantedGalaxyWeapons
**
*************************************************/

using StardewValley;
using StardewValley.Enchantments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnchantedGalaxyWeapons
{
    public sealed class ModConfig
    {
        public bool HaveDailySpawnLimit { get; set; } = true;
        public bool ForceInnateEnchantment { get; set; } = false;
        public bool ForceHaveEnchantment { get; set; } = false;
        public bool SkipGalaxyCheck { get; set; } = false;
        public bool SkipInfinityCheck { get; set; } = false;
        public bool HaveGlobalChance { get; set; } = false;
        public bool AllowMoreThanOne { get; set; } = false;
        public int DailySpawnLimit { get; set; } = 5;
        public int AdditionalTriesToSpawn { get; set; } = 0;
        public int AdditionalBarrels { get; set; } = 0;
        public float BaseSpawnChance { get; set; } = 0.6f;
        public float IncreaseSpawnChanceStep { get; set; } = 0.05f;
        public float ChanceForEnchantment { get; set; } = 0.15f;
        public float ChanceForInnate { get; set; } = 0.5f;
        public bool AllowGalSword { get; set; } = true;
        public bool AllowGalDagger { get; set; } = true;
        public bool AllowGalHammer { get; set; } = true;
        public bool AllowInfSword { get; set; } = true;
        public bool AllowInfDagger { get; set; } = true;
        public bool AllowInfHammer { get; set; } = true;
        public bool AllowArtful { get; set; } = true;
        public bool AllowBugKiller { get; set; } = true;
        public bool AllowCrusader { get; set; } = true;
        public bool AllowHaymaker { get; set; } = true;
        public bool AllowVampiric { get; set; } = true;
        public bool KeepVanilla { get; set; } = true;
        public int MaxInnateEnchantments { get; set; } = 2;
        public int MinInnateEnchantments { get; set; } = 0;
        public bool AllowDefense { get; set; } = true;
        public bool AllowWeight { get; set; } = true;
        public bool AllowSlimeGatherer { get; set; } = true;
        public bool AllowSlimeSlayer { get; set; } = true;
        public bool AllowCritPow { get; set; } = true;
        public bool AllowCritChance { get; set; } = true;
        public bool AllowAttack { get; set; } = true;
        public bool AllowSpeed { get; set; } = true;




    }
}
