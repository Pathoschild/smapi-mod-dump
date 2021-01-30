/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/GenDeathrow/SDV_BlessingsAndCurses
**
*************************************************/

using System;
using BNC.TwitchApp;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using StardewValley;
using static BNC.BuffManager;

namespace BNC.Actions
{
    class Weather : BaseAction
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "weatherType")]
        public int weather;

        public static bool hasTriggeredToday = false;

        public static void clearForNewDay()
        {
            hasTriggeredToday = false;
        }

        public override ActionResponse Handle()
        {

            if(hasTriggeredToday)
                return ActionResponse.Retry;

            Farmer who = Game1.player;
            GameLocation.LocationContext location_context = Game1.currentLocation.GetLocationContext();

            if (location_context == GameLocation.LocationContext.Default)
            {

                if (Game1.netWorldState.Value.WeatherForTomorrow == (Game1.weatherForTomorrow = weather))
                    return ActionResponse.Retry;

                if (!Utility.isFestivalDay(Game1.dayOfMonth + 1, Game1.currentSeason))
                {
                    Game1.netWorldState.Value.WeatherForTomorrow = (Game1.weatherForTomorrow = weather);
                    Game1.pauseThenMessage(2000, $" {this.from} has spoken {Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12822")}", showProgressBar: false);
                    hasTriggeredToday = true;
                }
                else
                {
                    return ActionResponse.Retry;
                }
            }
            else
            {

                if (Game1.netWorldState.Value.GetWeatherForLocation(location_context).weatherForTomorrow.Value == weather)
                    return ActionResponse.Retry;
                else
                {
                    Game1.netWorldState.Value.GetWeatherForLocation(location_context).weatherForTomorrow.Value = weather;
                    Game1.pauseThenMessage(2000, $" {this.from}: {Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12822")}", showProgressBar: false);
                    hasTriggeredToday = true;
                }
            }
            Game1.screenGlow = false;
            who.currentLocation.playSound("thunder");
            Game1.screenGlowOnce(Color.SlateBlue, hold: false);

            DelayedAction.playSoundAfterDelay("rainsound", 2000);

            return ActionResponse.Done;
        }
    }
}
