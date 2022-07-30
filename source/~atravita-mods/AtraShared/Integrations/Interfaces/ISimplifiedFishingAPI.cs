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
 * The following file was copied from: https://github.com/TehPers/StardewValleyMods/blob/master/src/TehPers.FishingOverhaul.Api/ISimplifiedFishingApi.cs .
 *
 * The original license is as follows:
 *
 * MIT License
 *
 * Copyright (c) 2017 TehPers
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

namespace AtraShared.Integrations.Interfaces;

/// <summary>
/// Simplified API for working with fishing. Prefer <see cref="IFishingApi"/> if possible.
///
/// You can copy this interface directly into your mod and use it with SMAPI's mod registry.
/// </summary>
public interface ISimplifiedFishingApi
{
    /// <summary>
    /// Gets the fish that can be caught. This does not take into account fish ponds.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer"/> that is fishing.</param>
    /// <param name="depth">The bobber depth.</param>
    /// <returns>The catchable fish as stringified namespaced keys.</returns>
    IEnumerable<string> GetCatchableFish(Farmer farmer, int depth = 4);

    /// <summary>
    /// Gets the trash that can be caught.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer"/> that is fishing.</param>
    /// <returns>The catchable trash as stringified namespaced keys.</returns>
    IEnumerable<string> GetCatchableTrash(Farmer farmer);

    /// <summary>
    /// Gets the treasure that can be caught.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer"/> that is fishing.</param>
    /// <returns>The catchable treasure as stringified namespaced keys.</returns>
    IEnumerable<string> GetCatchableTreasure(Farmer farmer);

    /// <summary>
    /// Gets the chance that a fish would be caught. This does not take into account whether
    /// there are actually fish to catch at the <see cref="Farmer"/>'s location. If no fish
    /// can be caught, then the <see cref="Farmer"/> will always catch trash.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer"/> catching the fish.</param>
    /// <returns>The chance a fish would be caught instead of trash.</returns>
    double GetChanceForFish(Farmer farmer);

    /// <summary>
    /// Gets the chance that treasure will be found during the fishing minigame.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer"/> catching the treasure.</param>
    /// <returns>The chance for treasure to appear during the fishing minigame.</returns>
    double GetChanceForTreasure(Farmer farmer);

    /// <summary>
    /// Modifies the chance that a fish would be caught. The provided callback is invoked every
    /// time <see cref="GetChanceForFish"/> is called to modify the resulting chance before
    /// it's returned. Modifiers are invoked in the order they are registered.
    /// </summary>
    /// <param name="chanceModifier">
    /// The chance modifier function. The input arguments are the <see cref="Farmer"/> and the
    /// calculated chance, and the return value is the modified chance.
    /// </param>
    void ModifyChanceForFish(Func<Farmer, double, double> chanceModifier);

    /// <summary>
    /// Modifies the chance that treasure would be found while fishing. The provided callback
    /// is invoked every time <see cref="GetChanceForTreasure"/> is called to modify the
    /// resulting chance before it's returned. Modifiers are invoked in the order they are
    /// registered.
    /// </summary>
    /// <param name="chanceModifier">
    /// The chance modifier function. The input arguments are the <see cref="Farmer"/> and the
    /// calculated chance, and the return value is the modified chance.
    /// </param>
    void ModifyChanceForTreasure(Func<Farmer, double, double> chanceModifier);

    /// <summary>
    /// Gets whether a fish is legendary.
    /// </summary>
    /// <param name="fishKey">The item key of the fish as a stringified namespaced key.</param>
    /// <returns>Whether that fish is legendary.</returns>
    bool IsLegendary(string fishKey);

    /// <summary>
    /// Gets a <see cref="Farmer"/>'s current fishing streak.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer"/> to get the streak of.</param>
    /// <returns>The <see cref="Farmer"/>'s streak.</returns>
    int GetStreak(Farmer farmer);

    /// <summary>
    /// Sets a <see cref="Farmer"/>'s current fishing streak.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer"/> to set the streak of.</param>
    /// <param name="streak">The <see cref="Farmer"/>'s streak.</param>
    void SetStreak(Farmer farmer, int streak);

    /// <summary>
    /// Selects a random catch. A player may catch either a fish or trash item.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer"/> that is fishing.</param>
    /// <param name="bobberDepth">The bobber's water depth.</param>
    /// <param name="isFish">Whether the caught item is a fish.</param>
    /// <returns>A possible catch.</returns>
    string GetPossibleCatch(Farmer farmer, int bobberDepth, out bool isFish);

    /// <summary>
    /// Selects random treasure.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer"/> that is fishing.</param>
    /// <returns>Possible loot from a treasure chest.</returns>
    IEnumerable<string> GetPossibleTreasure(Farmer farmer);
}