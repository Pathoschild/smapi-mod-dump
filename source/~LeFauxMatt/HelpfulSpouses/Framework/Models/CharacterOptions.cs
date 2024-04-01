/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.HelpfulSpouses.Framework.Models;

using StardewMods.HelpfulSpouses.Framework.Enums;

/// <summary>Represents the options for a character's chores.</summary>
internal sealed class CharacterOptions
{
    private readonly Dictionary<ChoreOption, double> data = new();

    /// <summary>Gets or sets the chance that the character will perform a <see cref="ChoreOption" /> chore.</summary>
    /// <param name="choreOption">The chore to get or set the value for.</param>
    public double this[ChoreOption choreOption]
    {
        get => this.data.GetValueOrDefault(choreOption, 0);
        set => this.data[choreOption] = value;
    }
}