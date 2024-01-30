/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Hikari-BS/StardewMods
**
*************************************************/

namespace FixedWeaponsDamage
{
    /// <summary>
    /// Configurations for the mod.
    /// </summary>
    internal class ModConfig
    {
        private int percentMeleeDamageMultiplier = 100;
        private int percentSlingshotDamageMultiplier = 100;

        public bool EnableMeleeFixedDamage { get; set; } = true;
        public int PercentMeleeDamageMultiplier
        {
            get => percentMeleeDamageMultiplier;

            // evaluate so that curious player won't be able to assign negative number and crash the game (idk though, probably not XD)
            // also set it to 1000 if it's greater than 1000
            set
            {
                value = value < 0 ? 0 : value > 1000 ? 1000 : value;
                percentMeleeDamageMultiplier = value;
            }
        }

        public bool EnableSlingshotFixedDamage { get; set; } = true;
        public int PercentSlingshotDamageMultiplier
        {
            get => percentSlingshotDamageMultiplier;
            set
            {
                value = value < 0 ? 0 : value > 1000 ? 1000 : value;
                percentSlingshotDamageMultiplier = value;
            }
        }
    }
}
