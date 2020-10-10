/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JohnsonNicholas/SDVMods
**
*************************************************/

using StardewModdingAPI;

namespace ClimatesOfFerngillRebuild
{    public class WeatherConfig
    {
        //required options
        public SButton WeatherMenuToggle { get; set; } = SButton.Z;
        public bool UseImperialForRainfall { get; set; } = false; 
        public string ClimateType { get; set; } = "enhanced";
        public double DryLightning { get; set; } = .1; // 10%
        public double WhiteOutChances { get; set; } = .05; //5%
        public double DryLightningMinTemp { get; set; } = 34; //34 C
        public bool SnowOnFall28 { get; set; } = false; //default setting - since if true, it will force
        public bool StormTotemChange { get; set; } = true; //rain totems may spawn storms instead of rain totems.
        public bool HazardousWeather { get; set; } = false; //by default hazaradous weather is turned off
        public bool AllowCropDeath { get; set; } = false; //even if you turn hazardous weather on, it won't enable crop death.
        public bool AllowStormsSpringYear1 { get; set; } = false; //default setting - maintains the fact that starting players may not be ready.		
        public double TooColdOutside { get; set; } = 1; //At this temp, it's too cold outside, and you have a hard frost. 1 C or 33.8 F default
        public double TooHotOutside { get; set; } = 39; //At this temp, it's too hot outside, and you can have a heatwave. 39 C or 102.2 F default
        public double DeadCropPercentage { get; set; } = .1; //default setting
        public double CropResistance { get; set; } = .75; //default settting
        public bool DisableAllFog { get; set; } = false; //default setting
        public bool EnableCustomWeatherIcon { get; set; } = true;
        public double ThunderFrenzyOdds { get; set; } = .01; //default setting
        public bool DisableHighRainWind { get; set; } = false; //default setting
        public bool ShowLighterFog { get; set; } = false; //default setting
        public bool RedrawCursor { get; set; } = true; //default setting
        public double VariableRainChance { get; set; } = .185; //default setting
        public double OvercastChance { get; set;} = .05; //default setting
        public double VRChangeChance { get; set;} = .35; //default setting
        public double VRMassiveStepChance { get; set; } = .0935; //default setting		
		public bool ShowBothScales {get; set; } = false; //default setting.
		public string FirstTempScale {get; set; } = "Kraggs"; //default setting to in game scale
		public string SecondTempScale {get; set; } = "Farenheit"; //randomly selected setting, valid values are "Kraggs", "Celsius", "Farenheit" (and, as a joke, "Kelvin") (why am I leaving these notes in the source code)
		public bool Verbose { get; set; } = true; 
        public bool SandstormsInDesertOnly { get; set; } = false; //default setting     
        public double MoreSevereThunderFrenzyOdds { get; set; } = .22; //default setting
        public bool ShowSummitClouds { get; set;} = true; //default setting
        public double RainToSnowConversion { get; set; } = .525; //default setting
        public bool DisplayFogInDesert { get; set;} = false;
        public bool RequireMinRainToWaterCrops { get ; set; } = true;
        public bool ApplyFrostsInWinter { get; set;} = true;
        public int MaxRainFall { get; set; } = 11000;
        public double WeatherSystemChance { get; set; } = .15; //default setting
        public WeatherConfig()
        {
        }
    }
}
