/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/rokugin/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterStardrops {
    internal class ModConfig {

        public bool ShowLogging { get; set; } = false;

        public bool EnableAttack { get; set; } = false;
        public int AttackIncreaseAmount { get; set; } = 5;

        public bool EnableDefense { get; set; } = true;
        public int DefenseIncreaseAmount { get; set; } = 3;

        public bool EnableImmunity { get; set; } = true;
        public int ImmunityIncreaseAmount { get; set; } = 3;

        public bool EnableHealth { get; set; } = true;
        public int HealthIncreaseAmount { get; set; } = 15;

        public bool EnableStamina { get; set; } = true;
        public int StaminaIncreaseAmount { get; set; } = 6;

        public bool EnableCombatLevel { get; set; } = false;
        public int CombatLevelIncreaseAmount { get; set; } = 1;

        public bool EnableFarmingLevel { get; set; } = false;
        public int FarmingLevelIncreaseAmount { get; set; } = 1;

        public bool EnableFishingLevel { get; set; } = false;
        public int FishingLevelIncreaseAmount { get; set; } = 1;

        public bool EnableForagingLevel { get; set; } = false;
        public int ForagingLevelIncreaseAmount { get; set; } = 1;

        public bool EnableLuckLevel { get; set; } = false;
        public int LuckLevelIncreaseAmount { get; set; } = 1;

        public bool EnableMiningLevel { get; set; } = false;
        public int MiningLevelIncreaseAmount { get; set; } = 1;

        public bool EnableMagnetic { get; set; } = false;
        public int MagneticIncreaseAmount { get; set; } = 1;

    }
}
