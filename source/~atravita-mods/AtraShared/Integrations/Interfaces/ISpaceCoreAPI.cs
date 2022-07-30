/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

/*********************************************
 * The following file was copied from: https://github.com/spacechase0/StardewValleyMods/blob/main/SpaceCore/Api.cs.
 *
 * The original license is as follows:
 *
 * MIT License
 *
 * Copyright (c) 2021 Chase W
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
 *
 * *******************************************/

using System.Reflection;

namespace AtraShared.Integrations.Interfaces;

/// <summary>
/// API to interface with Spacecore's custom skills.
/// </summary>
/// <remarks> Copied from https://github.com/spacechase0/StardewValleyMods/blob/main/SpaceCore/Api.cs. </remarks>
public interface ISpaceCoreAPI
{
    /// <summary>
    /// Gets a list of custom skills.
    /// </summary>
    /// <returns>An array of skill IDs, one for each registered skill.</returns>
    string[] GetCustomSkills();

    /// <summary>
    /// Gets the level for a custom skill.
    /// </summary>
    /// <param name="farmer">Farmer to look in.</param>
    /// <param name="skill">String name of skill.</param>
    /// <returns>Level for the custom skill if found, 0 otherwise.</returns>
    int GetLevelForCustomSkill(Farmer farmer, string skill);

    /// <summary>
    /// Gets the integer professionID for a specific skill and profession.
    /// </summary>
    /// <param name="skill">string name of skill.</param>
    /// <param name="profession">string name for profession.</param>
    /// <exception cref="InvalidOperationException">LINQ single failed, likely profession not found.</exception>
    /// <exception cref="NullReferenceException">Search for skill failed, likely skill not found.</exception>
    /// <returns>integer profession ID.</returns>
    int GetProfessionId(string skill, string profession);

    /// <summary>
    /// Adds an event command.
    /// </summary>
    /// <param name="command">The string command name.</param>
    /// <param name="info">The MethodInfo to run. Must take (Event, GameLocation, GameTime, string[]).</param>
    void AddEventCommand(string command, MethodInfo info);
}

/// <summary>
/// More complete spacecore API.
/// </summary>
public interface ICompleteSpaceCoreAPI : ISpaceCoreAPI
{
    /// <summary>
    /// Registers a type for spacecore to serialize.
    /// </summary>
    /// <param name="type">Type to serialize.</param>
    /// <remarks>Must have [XmlType("Mods_SOMETHINGHERE")] attribute (required to start with "Mods_").</remarks>
    void RegisterSerializerType(Type type);

    /// <summary>
    /// Registers a fake custom property with SpaceCore.
    /// </summary>
    /// <param name="declaringType">The type to attach the property too.</param>
    /// <param name="name">name of the parameter.</param>
    /// <param name="propType">type of the fake property.</param>
    /// <param name="getter">methodinfo for the getter.</param>
    /// <param name="setter">methodinfo for the setter.</param>
    void RegisterCustomProperty(Type declaringType, string name, Type propType, MethodInfo getter, MethodInfo setter);
}