/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/CompSciLauren/stardew-valley-daily-screenshot-mod
**
*************************************************/

using Xunit;

using DailyScreenshot;
using static DailyScreenshot.ModTrigger;


namespace DailyScreenshotTests
{
    public class ModConfigHelperTest
    {
        [Fact]
        public void IsWeatherConditionEnabled_AnySetInConfig_ReturnsTrueForAllWeather()
        {
            Assert.True(ModConfigHelper.IsWeatherConditionEnabled(WeatherFlags.Any, WeatherFlags.Sunny));
            Assert.True(ModConfigHelper.IsWeatherConditionEnabled(WeatherFlags.Any, WeatherFlags.Rainy));
            Assert.True(ModConfigHelper.IsWeatherConditionEnabled(WeatherFlags.Any, WeatherFlags.Windy));
            Assert.True(ModConfigHelper.IsWeatherConditionEnabled(WeatherFlags.Any, WeatherFlags.Stormy));
            Assert.True(ModConfigHelper.IsWeatherConditionEnabled(WeatherFlags.Any, WeatherFlags.Snowy));
        }

        [Fact]
        public void IsWeatherConditionEnabled_NoneSetInConfig_ReturnsFalseForAllWeather()
        {
            Assert.False(ModConfigHelper.IsWeatherConditionEnabled(WeatherFlags.Weather_None, WeatherFlags.Sunny));
            Assert.False(ModConfigHelper.IsWeatherConditionEnabled(WeatherFlags.Weather_None, WeatherFlags.Rainy));
            Assert.False(ModConfigHelper.IsWeatherConditionEnabled(WeatherFlags.Weather_None, WeatherFlags.Windy));
            Assert.False(ModConfigHelper.IsWeatherConditionEnabled(WeatherFlags.Weather_None, WeatherFlags.Stormy));
            Assert.False(ModConfigHelper.IsWeatherConditionEnabled(WeatherFlags.Weather_None, WeatherFlags.Snowy));
        }

        [Fact]
        public void IsWeatherConditionEnabled_SpecificWeather_ReturnsTrueForSameWeather()
        {
            Assert.True(ModConfigHelper.IsWeatherConditionEnabled(WeatherFlags.Sunny, WeatherFlags.Sunny));
            Assert.True(ModConfigHelper.IsWeatherConditionEnabled(WeatherFlags.Rainy, WeatherFlags.Rainy));
            Assert.True(ModConfigHelper.IsWeatherConditionEnabled(WeatherFlags.Windy, WeatherFlags.Windy));
            Assert.True(ModConfigHelper.IsWeatherConditionEnabled(WeatherFlags.Stormy, WeatherFlags.Stormy));
            Assert.True(ModConfigHelper.IsWeatherConditionEnabled(WeatherFlags.Snowy, WeatherFlags.Snowy));
        }

        [Fact]
        public void IsWeatherConditionEnabled_SpecificWeather_ReturnsFalseForDifferentWeather()
        {
            Assert.False(ModConfigHelper.IsWeatherConditionEnabled(WeatherFlags.Sunny, WeatherFlags.Rainy));
        }
    }
}
