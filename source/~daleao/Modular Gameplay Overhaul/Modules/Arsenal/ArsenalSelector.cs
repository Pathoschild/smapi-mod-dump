/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Arsenal;

#region using directives

using System.Linq;
using DaLion.Shared.Extensions.Stardew;
using StardewValley.Monsters;
using StardewValley.Tools;

#endregion using directives

/// <summary>Smart <see cref="Tool"/> selector.</summary>
internal static class ArsenalSelector
{
    internal static bool TryFor(Farmer who, out int index)
    {
        index = -1;

        var selectable = ArsenalModule.State.SelectableArsenal;
        if (selectable is null)
        {
            return false;
        }

        switch (selectable)
        {
            case MeleeWeapon:
                if (who.currentLocation.characters.OfType<Monster>().Any(m =>
                        m.DistanceTo(who) <= ArsenalModule.Config.Weapons.AutoSelectionRange))
                {
                    index = who.Items.IndexOf(selectable);
                }

                break;

            case Slingshot:
                if (who.currentLocation.characters.OfType<Monster>().Any(m =>
                        m.DistanceTo(who) <= ArsenalModule.Config.Slingshots.AutoSelectionRange))
                {
                    index = who.Items.IndexOf(selectable);
                }

                break;
        }

        return index >= 0;
    }
}
