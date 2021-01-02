/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/pomepome/JoysOfEfficiency
**
*************************************************/

using JoysOfEfficiency.Utils;

namespace JoysOfEfficiency.Core
{
    internal class ConfigLimitation
    {
        private static Config Conf => InstanceHolder.Config;
        public static void LimitConfigValues()
        {
            if (Conf.DisableConfigLimitation)
            {
                return;
            }
            Conf.CpuThresholdFishing = Util.Cap(Conf.CpuThresholdFishing, 0, 0.5f);
            Conf.HealthToEatRatio = Util.Cap(Conf.HealthToEatRatio, 0.1f, 0.8f);
            Conf.StaminaToEatRatio = Util.Cap(Conf.StaminaToEatRatio, 0.1f, 0.8f);
            Conf.ThrowPower = Util.Cap(Conf.ThrowPower, 0.0f, 1.0f);
            Conf.AutoCollectRadius = (int)Util.Cap(Conf.AutoCollectRadius, 1, 3);
            Conf.AutoHarvestRadius = (int)Util.Cap(Conf.AutoHarvestRadius, 1, 3);
            Conf.AutoPetRadius = (int)Util.Cap(Conf.AutoPetRadius, 1, 3);
            Conf.AutoWaterRadius = (int)Util.Cap(Conf.AutoWaterRadius, 1, 3);
            Conf.AutoDigRadius = (int)Util.Cap(Conf.AutoDigRadius, 1, 3);
            Conf.AutoShakeRadius = (int)Util.Cap(Conf.AutoShakeRadius, 1, 3);
            Conf.MachineRadius = (int)Util.Cap(Conf.MachineRadius, 1, 3);
            Conf.IdleTimeout = (int)Util.Cap(Conf.IdleTimeout, 1, 300);
            Conf.ScavengingRadius = (int)Util.Cap(Conf.ScavengingRadius, 1, 3);
            Conf.AnimalHarvestRadius = (int)Util.Cap(Conf.AnimalHarvestRadius, 1, 3);
            Conf.TrialOfExamine = (int)Util.Cap(Conf.TrialOfExamine, 1, 50);
            Conf.RadiusFarmCleanup = (int)Util.Cap(Conf.RadiusFarmCleanup, 1, 3);
            Conf.ThresholdStaminaPercentage = (int)Util.Cap(Conf.ThresholdStaminaPercentage, 10f, 60f);
        }
    }
}
