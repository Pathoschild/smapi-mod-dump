/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.TooManyAnimals.Models;

using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewMods.TooManyAnimals.Interfaces;

/// <inheritdoc />
internal class ControlScheme : IControlScheme
{
    /// <inheritdoc />
    public KeybindList NextPage { get; set; } = new(SButton.DPadRight);

    /// <inheritdoc />
    public KeybindList PreviousPage { get; set; } = new(SButton.DPadLeft);
}