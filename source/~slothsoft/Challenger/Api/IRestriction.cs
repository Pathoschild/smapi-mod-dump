/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-challenger
**
*************************************************/

namespace Slothsoft.Challenger.Api;

public interface IRestriction {
    /// <summary>
    /// Returns a string explaining what you can or cannot do in this restriction.
    /// </summary>
    /// <returns></returns>
    string GetDisplayText();

    /// <summary>
    /// Adds this restriction to the game.
    /// </summary>
    void Apply();

    /// <summary>
    /// Removes this restriction from the game.
    /// </summary>
    void Remove();
}