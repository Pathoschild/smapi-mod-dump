/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.EasyAccess.Models.Config;

using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewMods.EasyAccess.Interfaces.Config;

/// <inheritdoc />
internal class ControlScheme : IControlScheme
{
    /// <inheritdoc />
    public KeybindList CollectItems { get; set; } = new(SButton.Delete);

    /// <inheritdoc />
    public KeybindList DispenseItems { get; set; } = new(SButton.Insert);
}