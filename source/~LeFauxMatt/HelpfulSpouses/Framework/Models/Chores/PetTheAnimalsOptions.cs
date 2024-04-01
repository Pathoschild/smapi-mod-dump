/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.HelpfulSpouses.Framework.Models.Chores;

using StardewMods.HelpfulSpouses.Framework.Services.Chores;

/// <summary>Config data for <see cref="PetTheAnimals" />.</summary>
internal sealed class PetTheAnimalsOptions
{
    /// <summary>Gets or sets the limit to the number of animals that will be pet.</summary>
    public int AnimalLimit { get; set; }

    /// <summary>Gets or sets the occupant types.</summary>
    public List<string> ValidOccupantTypes { get; set; } = ["Barn", "Coop"];
}