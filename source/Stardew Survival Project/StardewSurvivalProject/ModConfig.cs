/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NeroYuki/StardewSurvivalProject
**
*************************************************/

using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewSurvivalProject
{
    public class ModConfig
    {
        //Feature config
        public bool UsePassiveDrain { get; set; } = true;
        public bool UseOnRunningDrain { get; set; } = true;
        public bool UseOnToolUseDrain { get; set; } = true;
        public bool UseTemperatureModule { get; set; } = true;
        // public bool UseReworkedStaminaDrain { get; set; } = true;
        public bool UseMoodExtension { get; set; } = true;
        public bool DisableHPHealingOnEatingFood { get; set; } = true;
        public bool UseOvernightPassiveDrain { get; set; } = true;
        public bool UseSanityModule { get; set; } = false;
        public bool UseStaminaRework { get; set; } = true;
        public SButton SprintButton { get; set; } = (SButton) Keys.LeftControl;
        public SButton SecondaryLayerButton { get; set; } = (SButton) Keys.LeftShift;
        public bool ScaleHungerRestoredWithTimeFromLastMeal { get; set; } = false;
        public int FriendshipPenaltyOnNotFeedingSpouse { get; set; } = 50;
        public int HealthRestoreOnSleep { get; set; } = 20;

        //UI config
        public int UIOffsetX { get; set; } = 10;
        public int UIOffsetY { get; set; } = 10;
        public float UIScale { get; set; } = 4.0f;
        public bool IsAnchoredDown { get; set; } = false; //not working
        public double EnvironmentTemperatureDisplayLowerBound { get; set; } = -10;
        public double EnvironmentTemperatureDisplayHigherBound { get; set; } = 50;
        public double BodyTemperatureDisplayLowerBound { get; set; } = 28;
        public double BodyTemperatureDisplayHigherBound { get; set; } = 45;
        public bool DisableModItemInfo { get; set; } = false;
        public string TemperatureUnit { get; set; } = "Celcius";
        public string RetexturePreset { get; set; } = "auto";
        public bool IndicateComfortableTemperatureRange { get; set; } = true;

        //Difficulty Setting
        //Thirst and Hunger
        public double PassiveHungerDrainRate { get; set; } = 0.2;
        public double PassiveThirstDrainRate { get; set; } = 0.3;
        public double RunningHungerDrainRate { get; set; } = 0.001;
        public double RunningThirstDrainRate { get; set; } = 0.002;
        public double MeleeWeaponHungerDrain { get; set; } = 0.2;
        public double MeleeWeaponThirstDrain { get; set; } = 0.1;
        public double SlingshotHungerDrain { get; set; } = 0.3;
        public double SlingshotThirstDrain { get; set; } = 0.2;
        public double PickaxeHungerDrain { get; set; } = 0.4;
        public double PickaxeThirstDrain { get; set; } = 0.2;
        public double AxeHungerDrain { get; set; } = 0.4;
        public double AxeThirstDrain { get; set; } = 0.2;
        public double FishingPoleHungerDrain { get; set; } = 1.8;
        public double FishingPoleThirstDrain { get; set; } = 0.6;
        public double HoeHungerDrain { get; set; } = 0.4;
        public double HoeThirstDrain { get; set; } = 0.2;
        public double WateringCanHungerDrain { get; set; } = 0.4;
        public double WateringCanThirstDrain { get; set; } = 0.2;
        public double MilkPailHungerDrain { get; set; } = 0.6;
        public double MilkPailThirstDrain { get; set; } = 0.2;
        public double ShearHungerDrain { get; set; } = 0.4;
        public double ShearThirstDrain { get; set; } = 0.2;

        public double DefaultHydrationGainOnDrinkableItems { get; set; } = 10.0;
        public string EnvironmentHydrationMode { get; set; } = "default";
        public double HydrationGainOnEnvironmentWaterDrinking { get; set; } = 5.0;
        public double HungerGainMultiplierFromItemEdibility { get; set; } = 1.0;
        public int HealthPenaltyOnDehydration { get; set; } = 10;
        public int StaminaPenaltyOnStarvation { get; set; } = 20;

        public double MaxHunger { get; set; } = 100;
        public double MaxThirst { get; set; } = 100;
        public double HungerEffectPercentageThreshold { get; set; } = 25;
        public double ThirstEffectPercentageThreshold { get; set; } = 25;
        public double HungerWellFedEffectPercentageThreshold { get; set; } = 80;
        public double ThirstWellFedEffectPercentageThreshold { get; set; } = 80;

        //Environmental Temperature
        public double EnvironmentBaseTemperature { get; set; } = 25;
        public double DefaultDayNightCycleTemperatureDiffScale { get; set; } = 4;
        public double DefaultTemperatureFluctuationScale { get; set; } = 1;
        public double IndoorTemperatureMultiplier { get; set; } = 0.9;
        public double FarmIndoorTemperatureMultiplier { get; set; } = 0.5;
        //seasonal multiplier
        public double SpringSeasonTemperatureMultiplier { get; set; } = 0.9;
        public double SummerSeasonTemperatureMultiplier { get; set; } = 1.1;
        public double FallSeasonTemperatureMultiplier { get; set; } = 0.9;
        public double WinterSeasonTemperatureMultiplier { get; set; } = 0.2;
        //weather multiplier
        public double SunnyWeatherTemperatureMultiplier { get; set; } = 1.2;
        public double FestivalWeatherTemperatureMultiplier { get; set; } = 1.2;
        public double WeddingWeatherTemperatureMultiplier { get; set; } = 1.2;
        public double StormWeatherTemperatureMultiplier { get; set; } = 0.8;
        public double RainWeatherTemperatureMultiplier { get; set; } = 0.8;
        public double WindySpringWeatherTemperatureMultiplier { get; set; } = 0.9;
        public double WindyFallWeatherTemperatureMultiplier { get; set; } = 0.9;
        public double SnowWeatherTemperatureMultiplier { get; set; } = -2;
        //location setting
        public bool UseCustomLocationTemperatureData { get; set; } = true;
        public bool UseDefaultIndoorTemperatureModifier { get; set; } = true;
        public bool UseDefaultCaveTemperatureModifier { get; set; } = true;
        public bool UseDefaultSkullCavernTemperatureModifier { get; set; } = true;

        //Body Temperature
        public double DefaultBodyTemperature { get; set; } = 37.5;
        public double DefaultMinComfortableTemperature { get; set; } = 16;
        public double DefaultMaxComfortableTemperature { get; set; } = 26;
        public double HypothermiaBodyTempThreshold { get; set; } = 35;
        public double FrostbiteBodyTempThreshold { get; set; } = 30;
        public double HeatstrokeBodyTempThreshold { get; set; } = 38.5;
        public double BurnBodyTempThreshold { get; set; } = 41;
        public double LowTemperatureSlope { get; set; } = -0.17;
        public double HighTemperatureSlope { get; set; } = 0.09;
        public double TemperatureChangeEasing { get; set; } = 0.5;

        //Custom Buff / Debuff
        public double PercentageChanceGettingFever { get; set; } = 2;
        public double AdditionalPercentageChanceGettingFever { get; set; } = 8;
        public double HeatstrokeThirstDrainPerSecond { get; set; } = 0.8;
        public double AdditionalPercentageStaminaDrainOnFever { get; set; } = 200;
        public double StomachacheHungerPercentageDrainPerSecond { get; set; } = 1;
        public double PercentageChanceGettingStomachache { get; set; } = 3;
        public int HealthDrainOnBurnPerSecond { get; set; } = 3;
        public int HealthDrainOnFrostbitePerSecond { get; set; } = 3;

        //Reworked Stamina
        public float StaminaRegenOnNotMovingPerSecond { get; set; } = 2f;
        public float StaminaExtraRegenOnSittingPerSecond { get; set; } = 1f;
        public float StaminaExtraRegenOnNappingPerSecond { get; set; } = 2f;
        //public float StaminaRegenOnNotRunningPerSecond { get; set; } = 1f;
        public double AdditionalDrainOnToolUse { get; set; } = 200;
        public float StaminaDrainOnRunningPerTick { get; set; } = 0.01f;
        public float StaminaDrainOnSprintingPerTick { get; set; } = 0.05f;

        //Mood mechanic
        public double MoodThresholdForBreakdown { get; set; } = 0;
        public double MoodThresholdForUnhappy { get; set; } = 30;
        public double MoodThresholdForHappy { get; set; } = 70;
        public double MoodThresholdForVeryHappy { get; set; } = 100;

        private static ModConfig _instance;

        // This is the static method that controls the access to the singleton
        // instance. On the first run, it creates a singleton object and places
        // it into the static field. On subsequent runs, it returns the client
        // existing object stored in the static field.
        public static ModConfig GetInstance()
        {
            if (_instance == null)
            {
                _instance = new ModConfig();
            }
            return _instance;
        }

        public void SetConfig(ModConfig input, bool CopyUIConfig = false)
        {
            if (_instance == null)
            {
                _instance = new ModConfig();
            }
            if (input != null)
            {
                _instance = input;
            }
        }

    }
}
