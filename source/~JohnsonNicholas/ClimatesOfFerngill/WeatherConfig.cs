using Microsoft.Xna.Framework.Input;

namespace ClimatesOfFerngillRebuild
{
    public class WeatherConfig
    {
        //required options
        public Keys Keyboard { get; set; }
        public Buttons Controller { get; set; }
        public string ClimateType { get; set; }
        public double ThundersnowOdds { get; set; }
        public double BlizzardOdds { get; set; }
        public double DryLightning { get; set; }
        public double DryLightningMinTemp { get; set; }
        public bool SnowOnFall28 { get; set; }
        public bool StormTotemChange { get; set; }
        public bool HazardousWeather { get; set; }
        public bool AllowCropDeath { get; set; }
        public bool AllowStormsSpringYear1 { get; set; }
        public bool ShowBothScales { get; set; }
        public double TooColdOutside { get; set; }
        public double TooHotOutside { get; set; }
        public double AffectedOutside { get; set; }
        public int StaminaDrain { get; set; }
        public bool SickMoreThanOnce { get; set; }
        public double DeadCropPercentage { get; set; }
        public double CropResistance { get; set; }
        public double DarkFogChance { get; set; }
        public double ChanceOfGettingSick { get; set; }
        public bool Use12HourTime { get; set; }
        public double BadMoonRising { get; set; }
        public bool EclipseOn { get; set; }
        public double EclipseChance { get; set; }
        public bool SpawnMonsters { get; set; }
        public bool SpawnMonstersAllFarms { get; set; }

        public bool Verbose { get; set; }
        
        public WeatherConfig()
        {
            //set climate type
            ClimateType = "normal";

            //set keyboard key
            Keyboard = Keys.Z;

            //normal climate odds
            ThundersnowOdds = .001; //.1%
            BlizzardOdds = .08; // 8%
            DryLightning = .10; //10%
            DryLightningMinTemp = 34; //34 C, or 93.2 F
            HazardousWeather = false; //normally, hazardous weather is turned off
            TooHotOutside = 39; //At this temp, it's too hot outside, and you can have a heatwave. 39 C or 102.2 F default
            TooColdOutside = 1; //At this temp, it's too cold outside, and you have a hard frost. 1 C or 33.8 F default\

            AllowCropDeath = false; //even if you turn hazardous weather on, it won't enable crop death.
            SnowOnFall28 = false; //default setting - since if true, it will force
            StormTotemChange = true; //rain totems may spawn storms instead of rain totems.
            AllowStormsSpringYear1 = false; //default setting - maintains the fact that starting players may not 
            ShowBothScales = true; //default setting.
            DeadCropPercentage = .1; //default setting
            CropResistance = .75; //default settting
            DarkFogChance = .0875; //default setting
            Use12HourTime = false; //default setting
            BadMoonRising = .004; //default setting

            // be able to deal with lightning strikes

            //stamina options
            AffectedOutside = .65;
            StaminaDrain = 2;
            SickMoreThanOnce = false;
            ChanceOfGettingSick = .7;

            //general mod options
            Verbose = true;

            //eclipse stuff
            EclipseOn = true;
            EclipseChance = .015;
            SpawnMonsters = true;
            SpawnMonstersAllFarms = false;
        }
    }
}
