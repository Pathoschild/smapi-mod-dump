/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using System;
using System.Reflection;
using StardewValley;

namespace DynamicDialogues
{
    public interface ISpaceCoreAPI
    {
        /// Must take (Event, GameLocation, GameTime, string[])
        void AddEventCommand(string command, MethodInfo info);

        void RegisterCustomProperty(Type declaringType, string name, Type propType, MethodInfo getter, MethodInfo setter);

        void RegisterCustomLocationContext(string name, Func<Random, LocationWeather> getLocationWeatherForTomorrowFunc/*, Func<Farmer, string> passoutWakeupLocationFunc, Func<Farmer, Point?> passoutWakeupPointFunc*/ );
    }
}
