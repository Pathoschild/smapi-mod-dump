/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NeroYuki/StardewSurvivalProject
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewSurvivalProject.source.model
{
    public class BodyTemp
    {
        public static double DEFAULT_VALUE = ModConfig.GetInstance().DefaultBodyTemperature;

        public double MinComfortTemp = ModConfig.GetInstance().DefaultMinComfortableTemperature;
        public double MaxComfortTemp = ModConfig.GetInstance().DefaultMaxComfortableTemperature;

        public double DefaultMinComfortTemp = ModConfig.GetInstance().DefaultMinComfortableTemperature;
        public double DefaultMaxComfortTemp = ModConfig.GetInstance().DefaultMaxComfortableTemperature;
        //public double DefaultAvgComfortTemp = 20.0;

        public static double HypotherminaThreshold = ModConfig.GetInstance().HypothermiaBodyTempThreshold;
        public static double FrostbiteThreshold = ModConfig.GetInstance().FrostbiteBodyTempThreshold;
        public static double HeatstrokeThreshold = ModConfig.GetInstance().HeatstrokeBodyTempThreshold;
        public static double BurnThreshold = ModConfig.GetInstance().BurnBodyTempThreshold;

        public double value { get; set; }

        public BodyTemp()
        {
            this.value = DEFAULT_VALUE;
        }

        public void applyComfortTemp(double MinComfortTemp, double MaxComfortTemp)
        {
            this.MinComfortTemp = MinComfortTemp;
            this.MaxComfortTemp = MaxComfortTemp;
        }

        public void BodyTempCalc(EnvTemp envTemp, double fluctuation = 0)
        {
            //LogHelper.Debug($"{MinComfortTemp} {MaxComfortTemp}");
            double envTempVal = envTemp.value;
            double targetBodyTemp = value;
            //currently follow a segmented linear function (adjust to look good on desmos xd)
            if (envTemp.value > MaxComfortTemp)
            {
                // if more than maximum comfort temp
                targetBodyTemp = DEFAULT_VALUE + ModConfig.GetInstance().HighTemperatureSlope * (envTempVal - MaxComfortTemp);
            }
            else if (envTemp.value < MinComfortTemp)
            {
                // if more than maximum comfort temp
                targetBodyTemp = DEFAULT_VALUE + ModConfig.GetInstance().LowTemperatureSlope * (MinComfortTemp - envTempVal);
            }
            else
            {
                targetBodyTemp = DEFAULT_VALUE;
            }
            //gradual temp change instead of abrupted
            value += (targetBodyTemp - value) * ModConfig.GetInstance().TemperatureChangeEasing;
            //fluctuate a bit
            value += fluctuation;
        }

        internal void updateComfortTemp(string hat_name, string shirt_name, string pants_name, string boots_name)
        {
            double DefaultAvgComfortTemp = (DefaultMinComfortTemp + DefaultMaxComfortTemp) / 2;
            double minComfortTempModifier = 1;
            double maxComfortTempModifier = 1;

            data.ClothingTempResistantData hatData = data.ClothingTempResistantDictionary.GetClothingData(hat_name, "hat");
            data.ClothingTempResistantData shirtData = data.ClothingTempResistantDictionary.GetClothingData(shirt_name, "shirt");
            data.ClothingTempResistantData pantsData = data.ClothingTempResistantDictionary.GetClothingData(pants_name, "pants");
            data.ClothingTempResistantData bootsData = data.ClothingTempResistantDictionary.GetClothingData(boots_name, "boots");

            minComfortTempModifier += 0.3 * ((hatData != null)? hatData.coldInsulationModifier : 0)
                + 0.3 * ((shirtData != null) ? shirtData.coldInsulationModifier : 0)
                + 0.2 * ((pantsData != null) ? pantsData.coldInsulationModifier : 0)
                + 0.2 * ((bootsData != null) ? bootsData.coldInsulationModifier : 0);
            maxComfortTempModifier += 0.3 * ((hatData != null) ? hatData.heatInsulationModifier : 0)
                + 0.3 * ((shirtData != null) ? shirtData.heatInsulationModifier : 0)
                + 0.2 * ((pantsData != null) ? pantsData.heatInsulationModifier : 0)
                + 0.2 * ((bootsData != null) ? bootsData.heatInsulationModifier : 0);

            MinComfortTemp = DefaultAvgComfortTemp + (DefaultMinComfortTemp - DefaultAvgComfortTemp) * minComfortTempModifier;
            MaxComfortTemp = DefaultAvgComfortTemp + (DefaultMaxComfortTemp - DefaultAvgComfortTemp) * maxComfortTempModifier;
        }
    }
}
