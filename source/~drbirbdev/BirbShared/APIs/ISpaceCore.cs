/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using StardewValley;
using System;
using System.Reflection;

namespace BirbShared.APIs
{
    public interface ISpaceCore
    {
        string[] GetCustomSkills();
        int GetLevelForCustomSkill(Farmer farmer, string skill);
        void AddExperienceForCustomSkill(Farmer farmer, string skill, int amt);
        int GetProfessionId(string skill, string profession);

        /// Must take (Event, GameLocation, GameTime, string[])
        void AddEventCommand(string command, MethodInfo info);

        /// Must have [XmlType("Mods_SOMETHINGHERE")] attribute (required to start with "Mods_")
        void RegisterSerializerType(Type type);

        void RegisterCustomProperty(Type declaringType, string name, Type propType, MethodInfo getter, MethodInfo setter);

        void RegisterCustomLocationContext(string name, Func<Random, LocationWeather> getLocationWeatherForTomorrowFunc/*, Func<Farmer, string> passoutWakeupLocationFunc, Func<Farmer, Point?> passoutWakeupPointFunc*/ );
    }
}
