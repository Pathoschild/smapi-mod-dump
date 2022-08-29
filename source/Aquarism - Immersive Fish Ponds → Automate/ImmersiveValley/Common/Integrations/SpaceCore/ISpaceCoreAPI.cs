/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Common.Integrations.SpaceCore;

#region using directives

using System;
using System.Reflection;

#endregion using directives

public interface ISpaceCoreAPI
{
    string[] GetCustomSkills();

    int GetLevelForCustomSkill(Farmer farmer, string skill);

    void AddExperienceForCustomSkill(Farmer farmer, string skill, int amt);

    int GetProfessionId(string skill, string profession);

    void AddEventCommand(string command, MethodInfo info);

    void RegisterSerializerType(Type type);

    void RegisterCustomProperty(Type declaringType, string name, Type propType, MethodInfo getter, MethodInfo setter);

    void RegisterCustomLocationContext(string name, Func<Random, LocationWeather> getLocationWeatherForTomorrowFunc);
}