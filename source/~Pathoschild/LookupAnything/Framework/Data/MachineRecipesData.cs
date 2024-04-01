/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

namespace Pathoschild.Stardew.LookupAnything.Framework.Data
{
    /// <summary>Metadata for a machine's recipes.</summary>
    /// <param name="MachineID">The machine's unqualified item ID.</param>
    /// <param name="Recipes">The machine recipes.</param>
    internal record MachineRecipesData(string MachineID, MachineRecipeData[] Recipes);
}
