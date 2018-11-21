using System;
using TwilightShards.Stardew.Common;

namespace ClimatesOfFerngillRebuild
{
    /// <summary>
    /// This interface describes a custom weather for the Climates of Ferngill mod
    /// </summary>
    interface ISDVWeather
    {
       /// <summary>
       /// This handler handles it's update event
       /// </summary>
       event EventHandler<WeatherNotificationArgs> OnUpdateStatus;
       
       /// <summary>
       /// This function is the update event raiser
       /// </summary>
       /// <param name="weather">The weather type</param>
       /// <param name="status">The status</param>
       void UpdateStatus(string weather, bool status);
    
       /// <summary>
       /// This says if the weather is being drawn to the screen
       /// </summary>
       bool IsWeatherVisible { get; }

       /// <summary>
       /// This says if the weather is active
       /// </summary>
       bool WeatherInProgress { get; }

       /// <summary>
       /// This is the time the weather ends
       /// </summary>
       SDVTime WeatherExpirationTime { get; }

       /// <summary>
       /// This is the time the weather begins
       /// </summary>
       SDVTime WeatherBeginTime { get; }

       /// <summary>
       /// This function controls when the weather starts
       /// </summary>
       /// <param name="t">The time to start the weather</param>
       void SetWeatherBeginTime(SDVTime t);
    
       /// <summary>
       /// This function controls when the weather ends
       /// </summary>
       /// <param name="t">The time the weather ends</param>
       void SetWeatherExpirationTime(SDVTime t);

       /// <summary>
       /// This sets the weather start and end time
       /// </summary>
       /// <param name="begin">The beginning time</param>
       /// <param name="end">The ending time</param>
       void SetWeatherTime(SDVTime begin, SDVTime end);

       /// <summary>
       /// Return the weather type
       /// </summary>
       string WeatherType { get; }

       /// <summary>
       /// This function controls the weather handling new day
       /// </summary>
       void OnNewDay();

       /// <summary>
       /// This function controls the weather handling returning to the main menu
       /// </summary>
       void Reset();

       /// <summary>
       /// This function controls the weather handling drawing
       /// </summary>      
       void DrawWeather();

       /// <summary>
       /// This function is called every 10 in-game minutes
       /// </summary>
       void UpdateWeather();

       /// <summary>
       /// This function controls how they are created.
       /// </summary>
       void CreateWeather();

       /// <summary>
       /// This function controls how they are moved (used during the draw loop). Also runs once a tick.
       /// </summary>
       void MoveWeather();

       /// <summary>
       /// This function is designed for any updates that may occur once a second.
       /// </summary>
       void SecondUpdate();

       /// <summary>
       /// This function controls how they are ended
       /// </summary>
       void EndWeather();
    }
}
