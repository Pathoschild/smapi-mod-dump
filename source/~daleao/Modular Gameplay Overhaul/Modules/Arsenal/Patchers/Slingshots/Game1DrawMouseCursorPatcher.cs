/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Slingshots;

#region using directives

using DaLion.Overhaul.Modules.Arsenal.Events;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class Game1DrawMouseCursorPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="Game1DrawMouseCursorPatcher"/> class.</summary>
    internal Game1DrawMouseCursorPatcher()
    {
        this.Target = this.RequireMethod<Game1>(nameof(Game1.drawMouseCursor));
    }

    #region harmony patches

    /// <summary>Hide mouse behind bullseye.</summary>
    [HarmonyPrefix]
    private static bool Game1DrawMouseCursorPrefix()
    {
        return !EventManager.IsEnabled<BullseyeRenderedEvent>(); // don't run original logic
    }

    #endregion harmony patches
}
