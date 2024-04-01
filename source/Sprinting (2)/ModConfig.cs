/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/thiagomasson/Sprinting
**
*************************************************/

using StardewModdingAPI.Utilities;

namespace Sprinting
{
    internal class ModConfig
    {
        public KeybindList SprintingKey { get; set; } = KeybindList.Parse("LeftControl");

        public bool KeyIsToggle { get; set; } = false;

        public bool CanSprintExhausted { get; set; } = true;

        public int SpeedBoost { get; set; } = 2;

        public float EnergyDrainPerSecond { get; set; } = 1.0f;

        public bool SkillBased { get; set; } = true;

        public bool AllowHorseSprinting { get; set; } = true;

        public int MinimumEnergyToSprint { get; set; } = 15;

        public bool LowEnergyWarning { get; set; } = true;

        public int EnergyToWarn { get; set; } = 25;

        public bool CanRegenExhausted { get; set; } = true;

        public float EnergyRegenRate { get; set; } = 1.0f;

        public float MaxEnergyRegen { get; set; } = 1.0f;

        public int EnergyRegenCooldown { get; set; } = 5;

        public float HealthRegenRate { get; set; } = 0.0f;

        public float MaxHealthRegen { get; set; } = 1.0f;

        public int HealthRegenCooldown { get; set; } = 15;
    }
}
