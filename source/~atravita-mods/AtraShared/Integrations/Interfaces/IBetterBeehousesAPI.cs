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
 * The following file was copied from: https://github.com/tlitookilakin/BetterBeehouses/blob/master/BetterBeehouses/IBetterBeehousesAPI.cs.
 *
 * The original license is as follows:
 *
MIT License

Copyright (c) 2022 Tlitookilakin

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
 *
 * *******************************************/

namespace AtraShared.Integrations.Interfaces;

/// <summary>
/// API for Better Beehouses.
/// </summary>
/// <remarks>Copied from https://github.com/tlitookilakin/BetterBeehouses/blob/master/BetterBeehouses/IBetterBeehousesAPI.cs .</remarks>
public interface IBetterBeehousesAPI
{
    /// <summary>
    /// Gets if bees can work here.
    /// </summary>
    /// <param name="location">The location</param>
    /// <returns>True if bees can work in this location.</returns>
    public bool GetEnabledHere(GameLocation location);

    /// <summary>
    /// Gets if bees can work here.
    /// </summary>
    /// <param name="location">The location.</param>
    /// <param name="isWinter">If it is winter in this location.</param>
    /// <returns>True if bees can work in this location.</returns>
    public bool GetEnabledHere(GameLocation location, bool isWinter);

    /// <summary>
    /// Returns the number of days to produce honey.
    /// </summary>
    /// <returns>True if bees can work in this location.</returns>
    public int GetDaysToProduce();

    /// <summary>
    /// Return the distance bees will search for flowers.
    /// </summary>
    /// <returns>Tile Radius.</returns>
    public int GetSearchRadius();

    /// <summary>
    /// Returns the value multiplier for honey from bee houses.
    /// </summary>
    /// <returns>Multiplier.</returns>
    public float GetValueMultiplier();
}
