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
 * The following file was copied from: https://gitlab.com/kdau/pregnancyrole/-/blob/main/src/Api.cs.
 *
 * The original license is as follows:
 *
 * ﻿Copyright © 2020-2021 Kevin Daughtridge
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

 *
 * *******************************************/

using NetEscapades.EnumGenerators;

namespace AtraShared.Integrations.Interfaces;

[EnumExtensions]
public enum Role
{
    /// <summary>
    /// The farmer or NPC can become pregnant.
    /// </summary>
    Become,

    /// <summary>
    /// The farmer or NPC can make another farmer or NPC pregnant.
    /// </summary>
    Make,

    /// <summary>
    /// The farmer or NPC would always require adoption to have a baby.
    /// </summary>
    Adopt,
}

/// <summary>
/// The API for Pregancy Role.
/// </summary>
/// <remarks>Copied from https://gitlab.com/kdau/pregnancyrole/-/blob/main/src/Api.cs .</remarks>
public interface IPregnancyRoleApi
{
    /// <summary>
    /// Get the pregnancy role of the farmer.
    /// </summary>
    /// <param name="farmer"></param>
    /// <returns></returns>
    Role GetPregnancyRole(Farmer farmer);

    Role GetPregnancyRole(NPC npc);

    /// <summary>
    /// Whether the given farmer would require adoption to have a baby with their current spouse, including another farmer.
    /// </summary>
    /// <param name="farmer">Farmer.</param>
    /// <returns></returns>
    bool WouldNeedAdoption(Farmer farmer);

    /// <summary>
    /// Whether the given NPC would require adoption to have a baby with their current farmer spouse.
    /// </summary>
    /// <param name="npc">NPC.</param>
    /// <returns></returns>
    bool WouldNeedAdoption(NPC npc);
}