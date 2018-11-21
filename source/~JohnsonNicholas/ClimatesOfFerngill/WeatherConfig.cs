using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;

namespace ClimatesOfFerngillRebuild
{
    public class WeatherConfig
    {
        //required options
        public SButton WeatherMenuToggle { get; set; } = SButton.Z;
        public string ClimateType { get; set; } = "normal";
        public double ThundersnowOdds { get; set; } = .001; //.1%
        public double BlizzardOdds { get; set; } = .08; // 8%
        public double DryLightning { get; set; } = .1; // 10%
        public double DryLightningMinTemp { get; set; } = 34; //34 C
        public bool SnowOnFall28 { get; set; } = false; //default setting - since if true, it will force
        public bool StormTotemChange { get; set; } = true; //rain totems may spawn storms instead of rain totems.
        public bool HazardousWeather { get; set; } = false; //by default hazaradous weather is turned off
        public bool AllowCropDeath { get; set; } = false; //even if you turn hazardous weather on, it won't enable crop death.
        public bool AllowStormsSpringYear1 { get; set; } = false; //default setting - maintains the fact that starting players may not be ready.
        public bool ShowBothScales { get; set; } = true; //default setting.
        public double TooColdOutside { get; set; } = 1; //At this temp, it's too cold outside, and you have a hard frost. 1 C or 33.8 F default
        public double TooHotOutside { get; set; } = 39; //At this temp, it's too hot outside, and you can have a heatwave. 39 C or 102.2 F default
        public double DeadCropPercentage { get; set; } = .1; //default setting
        public double CropResistance { get; set; } = .75; //default settting
        public double DarkFogChance { get; set; } = .0875; //default setting
        public bool Use12HourTime { get; set; } = false; //default setting
        public bool EnableCustomWeatherIcon { get; set; } = true;
        public double ThunderFrenzyOdds { get; set; } = .01; //default setting
        public bool DarkenLightInFog { get; set; } = true; //default setting
        public bool DisableHighRainWind { get; set; } = false; //default setting
        public bool ShowLighterFog { get; set; } = false; //default setting

        public bool Verbose { get; set; } = true; 
        
        public WeatherConfig()
        {
        }
    }
}
