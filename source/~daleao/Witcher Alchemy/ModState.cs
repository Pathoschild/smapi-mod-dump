/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Alchemy;

#region using directives



#endregion using directives

/// <summary>The ephemeral mod state.</summary>
internal sealed class ModState
{
    internal int Toxicity { get; set; }

    internal bool UsingGridView = false;
    
    internal bool AppliedFiltering = false;
    
    internal bool ReversedSortOrder = false;
}
