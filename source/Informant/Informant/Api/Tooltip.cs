/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-informant
**
*************************************************/

namespace Slothsoft.Informant.Api; 

/// <summary>
/// A class that contains all information to create a tooltip.
/// </summary>
/// <param name="Text">the multiline text to display.</param>
public record Tooltip(string Text) {
    /// <summary>
    /// Optionally displays an icon around the tooltip.
    /// </summary>
    public Icon? Icon { get; init; }
}