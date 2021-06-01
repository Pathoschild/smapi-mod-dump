/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

namespace Outerwear.Models
{
    /// <summary>Represents the effects the outerwear has when equipped.</summary>
    public class OuterwearEffects
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The farming skill added to the player.</summary>
        public int FarmingIncrease { get; set; }

        /// <summary>The mining skill added to the player.</summary>
        public int MiningIncrease { get; set; }

        /// <summary>The foraging skill added to the player.</summary>
        public int ForagingIncrease { get; set; }

        /// <summary>The fishing skill added to the player.</summary>
        public int FishingIncrease { get; set; }

        /// <summary>The combat skill added to the player.</summary>
        public int CombatIncrease { get; set; }

        /// <summary>The max health added to the player.</summary>
        public int MaxHealthIncrease { get; set; }

        /// <summary>The max stamina added to the player.</summary>
        public int MaxStaminaIncrease { get; set; }

        /// <summary>The health regenerated every second.</summary>
        public int HealthRegeneration { get; set; }

        /// <summary>The stamina regenerated every second.</summary>
        public int StaminaRegeneration { get; set; }

        /// <summary>The percent critical chance added to the player.</summary>
        public int CriticalChanceIncrease { get; set; }

        /// <summary>The percent critical power added to the player.</summary>
        public int CriticalPowerIncrease { get; set; }

        /// <summary>The magnetic radius added to the player.</summary>
        public int MagneticRadiusIncrease { get; set; }

        /// <summary>The movement speed added to the player.</summary>
        public int SpeedIncrease { get; set; }

        /// <summary>The defence added to the player.</summary>
        public int DefenceIncrease { get; set; }

        /// <summary>The attack added to the player.</summary>
        public int AttackIncrease { get; set; }

        /// <summary>Whether the outerwear emits light.</summary>
        public bool EmitsLight { get; set; }

        /// <summary>The radius of the emitted light.</summary>
        public float LightRadius { get; set; } = 10;
    }
}
