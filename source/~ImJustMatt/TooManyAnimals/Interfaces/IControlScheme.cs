/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.TooManyAnimals.Interfaces;

using StardewModdingAPI.Utilities;

/// <summary>
///     Controls config data.
/// </summary>
internal interface IControlScheme
{
    /// <summary>
    ///     Gets or sets controls to switch to next page.
    /// </summary>
    public KeybindList NextPage { get; set; }

    /// <summary>
    ///     Gets or sets controls to switch to previous page.
    /// </summary>
    public KeybindList PreviousPage { get; set; }

    /// <summary>
    ///     Copies data from one <see cref="IConfigData" /> to another.
    /// </summary>
    /// <param name="other">The <see cref="IConfigData" /> to copy values to.</param>
    /// <typeparam name="TOther">The class/type of the other <see cref="IConfigData" />.</typeparam>
    public void CopyTo<TOther>(TOther other)
        where TOther : IControlScheme
    {
        other.PreviousPage = this.PreviousPage;
        other.NextPage = this.NextPage;
    }
}