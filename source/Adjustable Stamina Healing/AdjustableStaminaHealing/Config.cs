/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/pomepome/AdjustableStaminaHealing
**
*************************************************/

using StardewModdingAPI;

namespace AdjustableStaminaHealing
{
    public class Config
    {
        public float HealingValuePerSeconds { get; set; } = 0.5f;
        public int SecondsNeededToStartHealing { get; set; } = 3;
        public bool StopHealingWhileGamePaused { get; set; } = true;
        public bool HealHealth { get; set; } = false;
        public SButton IncreaseKey { get; set; } = SButton.O;
        public SButton DecreaseKey { get; set; } = SButton.P;
    }
}
