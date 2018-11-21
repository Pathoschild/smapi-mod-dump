using System;

namespace ClimatesOfFerngillRebuild
{
    /// <summary>
    /// This enum tracks weathers added to the system as well as the current weather.
    /// </summary>
    [Flags]
    public enum CurrentWeather
    {
        Unset = 0,
        Sunny = 2,
        Rain = 4,
        Snow = 8,
        Wind = 16,
        Festival = 32,
        Wedding = 64,
        Lightning = 128,
        Blizzard = 256,
        Fog = 512,
        Frost = 1024,
        Heatwave = 2048,
        WhiteOut = 4096,
        ThunderFrenzy = 8192,
        HurricaneRain = 16384,
        BloodMoon = 32768
    }
}