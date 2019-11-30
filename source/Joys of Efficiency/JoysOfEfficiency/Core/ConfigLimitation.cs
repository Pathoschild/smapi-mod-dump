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
            Conf.AutoCollectRadius = (int)Util.Cap(Conf.AutoCollectRadius, 1, 3);
            Conf.AutoHarvestRadius = (int)Util.Cap(Conf.AutoHarvestRadius, 1, 3);
            Conf.AutoPetRadius = (int)Util.Cap(Conf.AutoPetRadius, 1, 3);
            Conf.AutoWaterRadius = (int)Util.Cap(Conf.AutoWaterRadius, 1, 3);
            Conf.AutoDigRadius = (int)Util.Cap(Conf.AutoDigRadius, 1, 3);
            Conf.AutoShakeRadius = (int)Util.Cap(Conf.AutoShakeRadius, 1, 3);
            Conf.MachineRadius = (int)Util.Cap(Conf.MachineRadius, 1, 3);
            Conf.RadiusCraftingFromChests = (int)Util.Cap(Conf.RadiusCraftingFromChests, 1, 5);
            Conf.IdleTimeout = (int)Util.Cap(Conf.IdleTimeout, 1, 300);
            Conf.ScavengingRadius = (int)Util.Cap(Conf.ScavengingRadius, 1, 3);
            Conf.AnimalHarvestRadius = (int)Util.Cap(Conf.AnimalHarvestRadius, 1, 3);
            Conf.TrialOfExamine = (int)Util.Cap(Conf.TrialOfExamine, 1, 10);
            Conf.RadiusFarmCleanup = (int)Util.Cap(Conf.RadiusFarmCleanup, 1, 3);
        }
    }
}
