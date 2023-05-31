/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Slingshots;

#region using directives

using System.Linq;
using DaLion.Shared.Extensions.Stardew;
using StardewValley.Monsters;

#endregion using directives

/// <summary>Smart <see cref="Tool"/> selector.</summary>
internal static class SlingshotSelector
{
    internal static bool TryFor(Farmer who, out int index)
    {
        index = -1;

        var selectable = SlingshotsModule.State.AutoSelectableSlingshot;
        if (selectable is null)
        {
            return false;
        }

        if (who.currentLocation.characters.OfType<Monster>().Any(m =>
                m.DistanceTo(who) <= SlingshotsModule.Config.AutoSelectionRange))
        {
            index = who.Items.IndexOf(selectable);
        }

        return index >= 0;
    }
}
