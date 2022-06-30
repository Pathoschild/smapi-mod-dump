/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Alchemy;

#region using directives

using Framework;
using Framework.UI;
using System.Collections.Generic;

#endregion using directives

internal class PlayerState
{
    internal HashSet<Formula>? KnownFormulae;
    internal int CauldronLevel;

    internal bool UsingGridView = false;
    internal bool AppliedFiltering = false;
    internal bool ReversedSortOrder = false;
    internal AlchemyMenu.Autofill Autofill = AlchemyMenu.Autofill.Off;
}