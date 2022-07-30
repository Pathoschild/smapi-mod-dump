/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

/***************************
 * The following file was copied from: https://github.com/ShinyFurretz/StardewMods/blob/master/LineSprinklers/ILineSprinklersApi.cs.
 *
 * The original license is as follows:

 * MIT License

Copyright (c) 2019 hootless

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
 * **************************/

using Microsoft.Xna.Framework;

namespace AtraShared.Integrations.Interfaces;

/// <summary>The API which provides access to Line Sprinklers for other mods.</summary>
/// <remarks>Copied from https://github.com/ShinyFurretz/StardewMods/blob/master/LineSprinklers/ILineSprinklersApi.cs .</remarks>
public interface ILineSprinklersApi
{
    /// <summary>Get the maximum supported coverage width or height.</summary>
    int GetMaxGridSize();

    /// <summary>Get the relative tile coverage by supported sprinkler ID. Note that sprinkler IDs may change after a save is loaded due to Json Assets reallocating IDs.</summary>
    IDictionary<int, Vector2[]> GetSprinklerCoverage();
}