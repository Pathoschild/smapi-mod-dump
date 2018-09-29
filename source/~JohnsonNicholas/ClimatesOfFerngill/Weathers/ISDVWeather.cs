using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwilightShards.Stardew.Common;

namespace ClimatesOfFerngillRebuild
{
    interface ISDVWeather
    {
       //event handlers
       event EventHandler<WeatherNotificationArgs> OnUpdateStatus;
       void UpdateStatus(string weather, bool status);

       //members 
       bool IsWeatherVisible { get; }
       bool WeatherInProgress { get; }
       SDVTime WeatherExpirationTime { get; }
       SDVTime WeatherBeginTime { get; }
       void SetWeatherBeginTime(SDVTime t);
       void SetWeatherExpirationTime(SDVTime t);
       void SetWeatherTime(SDVTime begin, SDVTime end);
       string WeatherType { get; }

       //these functions control the weather over the date
       void OnNewDay();
       void Reset();

       //these functions are used to draw, update and create the weather
       void DrawWeather();
       void UpdateWeather();
       void CreateWeather();
       void MoveWeather();
       void EndWeather();
    }
}
