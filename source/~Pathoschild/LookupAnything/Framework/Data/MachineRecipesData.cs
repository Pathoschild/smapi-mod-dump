/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

#nullable disable

namespace Pathoschild.Stardew.LookupAnything.Framework.Data
{
    /// <summary>Metadata for a machine's recipes.</summary>
    internal class MachineRecipesData
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The machine item ID.</summary>
        public int MachineID { get; set; }

        /// <summary>The machine recipes.</summary>
        public MachineRecipeData[] Recipes { get; set; }
    }
}
