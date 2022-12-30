/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Tools;

#region using directives

using System.Linq;
using StardewValley.Tools;

#endregion using directives

internal static class Utils
{
    /// <summary>Recalculates the stats of all existing scythes.</summary>
    internal static void RevalidateScythes()
    {
        if (Context.IsMainPlayer)
        {
            Utility.iterateAllItems(item =>
            {
                if (item is MeleeWeapon weapon && weapon.isScythe())
                {
                    weapon.RecalculateAppliedForges(true);
                }
            });
        }
        else
        {
            foreach (var scythe in Game1.player.Items.OfType<MeleeWeapon>().Where(w => w.isScythe()))
            {
                scythe.RecalculateAppliedForges(true);
            }
        }
    }
}
