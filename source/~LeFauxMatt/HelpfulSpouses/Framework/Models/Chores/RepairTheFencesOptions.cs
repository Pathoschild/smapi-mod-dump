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

/// <summary>Config data for <see cref="RepairTheFences" />.</summary>
internal sealed class RepairTheFencesOptions
{
    /// <summary>Gets or sets the limit to the number of fences that will be repaired.</summary>
    public int FenceLimit { get; set; }
}