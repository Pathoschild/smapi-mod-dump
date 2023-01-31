/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

namespace CatGiftsRedux;

#nullable enable

/// <summary>
/// The API for this mod.
/// </summary>
public interface ICatGiftReduxAPI
{
    /// <summary>
    /// Adds a picker with a specified weight.
    /// </summary>
    /// <param name="picker">A function that takes a random and returns an Item.</param>
    /// <param name="weight">Weight for the picker.</param>
    public void AddPicker(Func<Random, Item?> picker, double weight = 100);
}
