/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Xebeth/StardewValley-SpeedMod
**
*************************************************/

using System.Collections.Generic;
using StardewModdingAPI;
using System;

namespace SpeedMod
{
    public class ModConfig
    {
        /// <summary>
        /// Value of the speed boost
        /// </summary>
        public int SpeedModifier { get; set; }
        /// <summary>
        /// The key used to trigger the teleportation (displays a confirmation dialog)
        /// </summary>
        public SButton TeleportHomeKey { get; set; }
        /// <summary>
        /// Allow the player to cancel the teleportation if the activation key is pressed again (any spent energy is not recovered)
        /// </summary>
        public bool CanPlayerInterrupt { get; set; }
        /// <summary>
        /// Allow damage to interrupt the teleportation if the the threshold is reached (any spent energy is not recovered)
        /// </summary>
        public bool CanDamageInterrupt { get; set; }
        /// <summary>
        /// The amount of damage in % of the player's health that would interrupt the teleportation
        /// </summary>
        public int DamageThreshold { get; set; }
        /// <summary>
        /// If checked the damage threshold is evaluated on total health (chance of interruption is linear: easier), otherwise on the remaining health (chance of interruption increases as health declines: harder)
        /// </summary>
        public bool DamageThresholdBasedOnTotalHealth { get; set; }
        /// <summary>
        /// The amount of energy used to teleport (energy will gradually decrease until completed or interrupted)
        /// </summary>
        public int StaminaCost { get; set; }
        /// <summary>
        /// The time period for which the teleportation is on cooldown (resets at the end of the day). Format: HH:MM:SS
        /// </summary>
        public TimeSpan RecastCooldown { get; set; }
        /// <summary>
        /// The time it takes to perform the teleportation.
        /// </summary>
        public int CastCooldown { get; set; }
        /// <summary>
        /// Teleports the player directly to bed or outside the farm (same as the Return Scepter)
        /// </summary>
        public bool TeleportToBed { get; set; }
        /// <summary>
        /// Enables the teleportation effects
        /// </summary>
        public bool EnableTeleportationEffects { get; set; }
        /// <summary>
        /// Enables the teleportation sounds
        /// </summary>
        public bool EnableTeleportationSounds { get; set; }
        /// <summary>
        /// Allows the home teleportation in multiplayer mode
        /// </summary>
        public bool EnabledInMultiplayer { get; set; }

        public IList<string> Test { get; set; }

        public ModConfig()
        {
            TeleportToBed = CanPlayerInterrupt = CanDamageInterrupt = true;
            EnableTeleportationEffects = EnableTeleportationSounds = true;
            DamageThresholdBasedOnTotalHealth = true;
            RecastCooldown = new TimeSpan(0, 7, 0);
            EnabledInMultiplayer = false;
            TeleportHomeKey = SButton.H;
            DamageThreshold = 10;
            SpeedModifier = 2;
            CastCooldown = 5;
            StaminaCost = 50;
        }

        public static int Cooldown(double cooldown, double minValue)
        {
            return (int)Math.Max(cooldown, minValue);
        }

        private static string GetOptionState(bool option)
        {
            return option ? "X" : " ";
        }

        public override string ToString()
        {
            return "Configuration:\n" 
                + $" - Speed modifier:\t\t\t{SpeedModifier}\n"
                + $" - Teleport home key:\t\t\t{TeleportHomeKey}\n"
                + $" - Can interrupt:\t\t\t[{GetOptionState(CanPlayerInterrupt)}]\n"
                + $" - Can damage interrupt:\t\t[{GetOptionState(CanDamageInterrupt)}]\n"
                + $" - Threshold on total health:\t\t[{GetOptionState(DamageThresholdBasedOnTotalHealth)}]\n"
                + $" - Interrupt Damage threshold:\t\t{DamageThreshold}%\n"
                + $" - Teleport energy cost:\t\t{StaminaCost}\n"
                + $" - Teleport cooldown:\t\t\t{RecastCooldown:mm\\:ss}\n"
                + $" - Teleport cast length:\t\t{CastCooldown}s\n"
                + $" - Teleport to bed:\t\t\t[{GetOptionState(TeleportToBed)}]\n"
                + $" - Enabled visual effects:\t\t[{GetOptionState(EnableTeleportationEffects)}]\n"
                + $" - Enabled sounds effects:\t\t[{GetOptionState(EnableTeleportationSounds)}]\n"
                + $" - Enabled in multiplier:\t\t[{GetOptionState(EnabledInMultiplayer)}]\n";
        }
    }
}
